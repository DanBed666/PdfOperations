using System;
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
    [ObservableProperty]
    private string selectedFragmentFile = "";

    [ObservableProperty]
    private string pageNumbers = "";

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "fragments.pdf";

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [ObservableProperty]
    private string selectedFragmentFileInfo = "";

    [ObservableProperty]
    private PdfFragment? selectedFragment;

    public ObservableCollection<PdfFragment> Fragments { get; } = new();

    public FragmentsViewModel()
    {
        StatusMessage = Loc.Ready;
    }

    [RelayCommand]
    private void AddFragment()
    {
        if (string.IsNullOrWhiteSpace(SelectedFragmentFile) || !File.Exists(SelectedFragmentFile))
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.SelectPdfFile;
            return;
        }

        if (string.IsNullOrWhiteSpace(PageNumbers))
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.EnterPages;
            return;
        }
        
        string normalizedPages = PageNumbers.Trim().Replace(" ", "");
        
        if (!InputValidator.IsPagesFormatValid(normalizedPages))
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.InvalidPagesFormat;
            return;
        }
        
        int pageCount;
        
        try
        {
            pageCount = Info.GetPdfPagesSingle(SelectedFragmentFile);
        }
        catch (Exception ex)
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.CannotReadPdfPageCount(ex.Message);
            return;
        }
        
        if (!ArePagesInRange(normalizedPages, pageCount, out string pagesErrorMessage))
        {
            StatusKind = StatusKind.Error;
            StatusMessage = pagesErrorMessage;
            return;
        }

        Fragments.Add(new PdfFragment
        {
            FileName = SelectedFragmentFile,
            PageNumbers = normalizedPages
        });

        SelectedFragmentFile = "";
        PageNumbers = "";
        StatusKind = StatusKind.Success;
        StatusMessage = Loc.FragmentAdded;
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
    
        int index = Fragments.IndexOf(SelectedFragment);
    
        if (index <= 0)
            return;
    
        Fragments.Move(index, index - 1);
        StatusKind = StatusKind.Success;
        StatusMessage = Loc.FragmentMovedUp;
    }
    
    [RelayCommand]
    private void MoveFragmentDown()
    {
        if (SelectedFragment is null)
            return;
    
        int index = Fragments.IndexOf(SelectedFragment);
    
        if (index < 0 || index >= Fragments.Count - 1)
            return;
    
        Fragments.Move(index, index + 1);
        StatusKind = StatusKind.Success;
        StatusMessage = Loc.FragmentMovedDown;
    }

    [RelayCommand]
    private async Task Start()
    {
        if (IsBusy)
            return;

        IsBusy = true;
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

                OperationInput input = new OperationInput
                {
                    PdfFragments = Fragments.ToList(),
                    Output = OutputFileName
                };

                OperationDefinition operation = new OperationDefinition
                {
                    Extension = ".pdf"
                };

                try
                {
                    FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operation, input, context);

                    Pages.CreateWithCustomFiles(input, fileJob);

                    finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                    finalPath = GetAvailablePath(finalPath);

                    File.Move(fileJob.TempPath, finalPath);
                }
                finally
                {
                    if (Directory.Exists(context.TempDir))
                        Directory.Delete(context.TempDir, true);
                }
            });

            LastOutputDirectory = OutputDirectory;
            StatusKind = StatusKind.Success;
            StatusMessage = Loc.DoneSaved(finalPath);
        }
        catch (Exception ex)
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.Error(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
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
