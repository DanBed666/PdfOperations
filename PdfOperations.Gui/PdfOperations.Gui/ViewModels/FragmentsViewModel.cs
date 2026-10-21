using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public partial class FragmentsViewModel : ViewModelBase
{
    private string[] selectedFragmentFiles = [];
    
    private readonly List<(string Path, int? PageCount, Exception? Error)>
        selectedFileDetails = new();

    public string SelectedFragmentFilesText =>
        string.Join(Environment.NewLine, selectedFragmentFiles);
    
    public string SelectedFragmentFilesInfo =>
        string.Join(Environment.NewLine, selectedFileDetails.Select(file =>
        {
            string info = file.Error is not null
                ? Loc.ErrorSummary(file.Error)
                : file.PageCount is int count ? Loc.PagesInfo(count) : "";

            return $"{Path.GetFileName(file.Path)} - {info}";
        }));

    [ObservableProperty]
    private string pageNumbers = "";

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "fragments.pdf";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    public void SetSelectedFragmentFiles(IEnumerable<string> paths)
    {
        if (IsBusy)
            return;

        selectedFragmentFiles = paths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        
        selectedFileDetails.Clear();

        foreach (string path in selectedFragmentFiles)
        {
            try
            {
                int pageCount = Info.GetPdfPagesSingle(path);
                selectedFileDetails.Add((path, pageCount, null));
            }
            catch (Exception ex)
            {
                selectedFileDetails.Add((path, null, ex));
                ShowError(ex);
            }
        }
        
        SetDefaultOutputDirectoryFromInputFiles(
            selectedFragmentFiles,
            OutputDirectory,
            directory => OutputDirectory = directory);

        OnPropertyChanged(nameof(SelectedFragmentFilesText));
        OnPropertyChanged(nameof(SelectedFragmentFilesInfo));
    }
    
    [ObservableProperty]
    private SelectedFragmentItem? selectedFragment;

    public ObservableCollection<SelectedFragmentItem> Fragments { get; } = new();

    public FragmentsViewModel()
    {
        Fragments.CollectionChanged += (_, _) =>
        {
            for (int i = 0; i < Fragments.Count; i++)
                Fragments[i].Number = i + 1;

            StartCommand.NotifyCanExecuteChanged();
        };
    }

    [RelayCommand]
    private void AddFragment()
    {
        if (IsBusy)
            return;

        if (selectedFragmentFiles.Length == 0)
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.SelectPdfFiles;
            return;
        }

        if (string.IsNullOrWhiteSpace(PageNumbers))
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.EnterPages;
            return;
        }
        
        string normalizedPages = PageNumbers.Trim().Replace(" ", "");
        
        if (normalizedPages.Split(',').Any(string.IsNullOrWhiteSpace)
            || !InputValidator.IsPagesFormatValid(normalizedPages))
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.InvalidPagesFormat;
            return;
        }

        var pendingFragments = new List<SelectedFragmentItem>();

        try
        {
            foreach (string path in selectedFragmentFiles)
            {
                int pageCount = Info.GetPdfPagesSingle(path);

                if (!ArePagesInRange(normalizedPages, pageCount, out string error))
                {
                    StatusKind = StatusKind.Error;
                    StatusMessage = $"{Path.GetFileName(path)}: {error}";
                    return;
                }

                pendingFragments.Add(new SelectedFragmentItem
                {
                    FileName = path,
                    PageNumbers = normalizedPages
                });
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
            return;
        }

        foreach (SelectedFragmentItem fragment in pendingFragments)
        {
            Fragments.Add(fragment);
        }

        SetSelectedFragmentFiles(Array.Empty<string>());
        PageNumbers = "";
        StatusKind = StatusKind.Success;
        StatusMessage = Loc.FragmentsAdded(pendingFragments.Count);
    }
    
    private bool ArePagesInRange(string pages, int pageCount, out string errorMessage)
    {
        errorMessage = "";

        string[] parts = pages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string part in parts)
        {
            if (part.Contains('-'))
            {
                string[] range = part.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                int start = int.Parse(range[0]);
                int end = int.Parse(range[1]);

                if (start < 1 || end < 1 || start > pageCount || end > pageCount)
                {
                    errorMessage = Loc.PageRangeOutsideDocument(part, pageCount);
                    return false;
                }
            }
            else
            {
                int page = int.Parse(part);

                if (page < 1 || page > pageCount)
                {
                    errorMessage = Loc.PageOutsideDocument(page, pageCount);
                    return false;
                }
            }
        }

        return true;
    }

    [RelayCommand]
    private void RemoveFragment()
    {
        if (SelectedFragment is null)
            return;

        Fragments.Remove(SelectedFragment);
        SelectedFragment = null;
        StatusKind = StatusKind.Success;
        StatusMessage = Loc.FragmentRemoved;
    }
    
    [RelayCommand]
    private void ClearFragments()
    {
        Fragments.Clear();
        SelectedFragment = null;
        StatusKind = StatusKind.Success;
        StatusMessage = Loc.FragmentsCleared;
    }
    
    [RelayCommand]
    private void MoveFragmentUp()
    {
        if (SelectedFragment is null)
            return;

        SelectedFragmentItem movedFragment = SelectedFragment;
        int index = Fragments.IndexOf(SelectedFragment);
    
        if (index <= 0)
            return;
    
        Fragments.Move(index, index - 1);
        SelectedFragment = movedFragment;
        
        StatusKind = StatusKind.Success;
        StatusMessage = Loc.FragmentMovedUp;
    }
    
    [RelayCommand]
    private void MoveFragmentDown()
    {
        if (SelectedFragment is null)
            return;
    
        SelectedFragmentItem movedFragment = SelectedFragment;
        int index = Fragments.IndexOf(SelectedFragment);
    
        if (index < 0 || index >= Fragments.Count - 1)
            return;
    
        Fragments.Move(index, index + 1);
        SelectedFragment = movedFragment;
        
        StatusKind = StatusKind.Success;
        StatusMessage = Loc.FragmentMovedDown;
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task Start()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        var cancellationToken = OperationToken;
        StatusKind = StatusKind.Info;

        try
        {
            if (Fragments.Count == 0)
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.AddAtLeastOneFragment;
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.SelectOutputDirectory;
                return;
            }

            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);

            if (string.IsNullOrWhiteSpace(OutputFileName))
                OutputFileName = "fragments.pdf";

            if (!Path.GetExtension(OutputFileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + ".pdf";

            StatusMessage = Loc.BuildingPdfFromFragments;

            string finalPath = "";

            await Task.Run(() =>
            {
                OperationContext context = new OperationContext
                {
                    TempDir = Files.PrepareTempDir()
                };

                TemporaryDirectoryCleanup.Run(context.TempDir, () =>
                {
                    OperationInput input = new OperationInput
                    {
                        PdfFragments = Fragments.Select(fragment => new PdfFragment
                        {
                            FileName = fragment.FileName,
                            PageNumbers = fragment.PageNumbers
                        }).ToList(),
                        Output = OutputFileName
                    };

                    OperationDefinition operation = new OperationDefinition
                    {
                        Extension = ".pdf"
                    };

                    FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operation, input, context);

                    cancellationToken.ThrowIfCancellationRequested();
                    Pages.CreateWithCustomFiles(input, fileJob, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                    if (!OverwriteExistingFiles)
                        finalPath = GetAvailablePath(finalPath);

                    File.Move(fileJob.TempPath, finalPath, overwrite: OverwriteExistingFiles);
                });
            });

            LastOutputDirectory = OutputDirectory;
            StatusKind = StatusKind.Success;
            StatusMessage = Loc.DoneSaved(finalPath);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanStart()
    {
        return Fragments.Count > 0
               && !string.IsNullOrWhiteSpace(OutputDirectory)
               && !string.IsNullOrWhiteSpace(OutputFileName);
    }

    partial void OnOutputDirectoryChanged(string value)
    {
        StartCommand.NotifyCanExecuteChanged();
    }

    partial void OnOutputFileNameChanged(string value)
    {
        StartCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanOpenOutputFolder))]
    private void OpenOutputFolder()
    {
        if (string.IsNullOrWhiteSpace(LastOutputDirectory) || !Directory.Exists(LastOutputDirectory))
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.NoOutputFolderToOpen;
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = LastOutputDirectory,
            UseShellExecute = true
        });
    }

    private bool CanOpenOutputFolder()
    {
        return CanOpenDirectory(LastOutputDirectory);
    }

    partial void OnLastOutputDirectoryChanged(string value)
    {
        OpenOutputFolderCommand.NotifyCanExecuteChanged();
    }

    private static string GetAvailablePath(string path)
    {
        if (!File.Exists(path))
            return path;

        string dir = Path.GetDirectoryName(path)!;
        string name = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path);

        int counter = 1;

        while (true)
        {
            string candidate = Path.Combine(dir, $"{name}_{counter}{extension}");

            if (!File.Exists(candidate))
                return candidate;

            counter++;
        }
    }
}
