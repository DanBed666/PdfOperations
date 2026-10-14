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
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public partial class PagesViewModel : FileInputViewModelBase
{
    [ObservableProperty]
    private string title = "PDF pages";

    [ObservableProperty]
    private string pages = "";

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "";

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [ObservableProperty]
    private string inputFilesInfo = "";
    
    [ObservableProperty]
    private PageSelectionMode selectedMode = PageSelectionMode.CustomPages;

    [ObservableProperty]
    private string splitAfterPages = "";
    
    public ObservableCollection<PageModeMenuItem> PageModes { get; } = new();

    [ObservableProperty]
    private PageModeMenuItem? selectedPageMode;
    
    public bool IsCustomPagesMode => SelectedMode == PageSelectionMode.CustomPages;

    public bool IsSplitByPagesMode => SelectedMode == PageSelectionMode.SplitByPages;

    public bool CanEditOutputFileName => InputFilesList.Files.Count <= 1;
    
    public PagesViewModel()
        : base(
            "Select PDF files",
            "PDF files (*.pdf)",
            ["*.pdf"],
            inputTitle: LocalizationService.Instance.InputPdfFilesWithFormats)
    {
        StatusMessage = Loc.Ready;
        RefreshPageModes();
        SelectedPageMode = PageModes.FirstOrDefault(mode => mode.Mode == PageSelectionMode.CustomPages);
        InputFilesList.FilesChanged += () => RefreshInputFilesInfo();
    }
    
    private void RefreshPageModes()
    {
        PageSelectionMode? selectedMode = SelectedPageMode?.Mode;

        PageModes.Clear();

        PageModes.Add(new PageModeMenuItem
        {
            Mode = PageSelectionMode.CustomPages,
            DisplayName = Loc.GetPageModeName(PageSelectionMode.CustomPages)
        });

        PageModes.Add(new PageModeMenuItem
        {
            Mode = PageSelectionMode.EvenPages,
            DisplayName = Loc.GetPageModeName(PageSelectionMode.EvenPages)
        });

        PageModes.Add(new PageModeMenuItem
        {
            Mode = PageSelectionMode.OddPages,
            DisplayName = Loc.GetPageModeName(PageSelectionMode.OddPages)
        });

        PageModes.Add(new PageModeMenuItem
        {
            Mode = PageSelectionMode.SplitByPages,
            DisplayName = Loc.GetPageModeName(PageSelectionMode.SplitByPages)
        });

        PageModes.Add(new PageModeMenuItem
        {
            Mode = PageSelectionMode.SplitEveryPage,
            DisplayName = Loc.GetPageModeName(PageSelectionMode.SplitEveryPage)
        });

        if (selectedMode is not null)
            SelectedPageMode = PageModes.FirstOrDefault(mode => mode.Mode == selectedMode);
    }
    
    partial void OnSelectedModeChanged(PageSelectionMode value)
    {
        OnPropertyChanged(nameof(IsCustomPagesMode));
        OnPropertyChanged(nameof(IsSplitByPagesMode));
    }
    
    public void RefreshInputFilesInfo()
    {
        string[] files = InputFilesList.ToPathArray();

        InputFilesInfo = string.Join(Environment.NewLine, files.Select(path =>
        {
            try
            {
                return $"{Path.GetFileName(path)} - {Loc.PagesInfo(Info.GetPdfPagesSingle(path))}";
            }
            catch (Exception ex)
            {
                return $"{Path.GetFileName(path)} - {Loc.CannotReadPages(ex.Message)}";
            }
        }));

        OnPropertyChanged(nameof(CanEditOutputFileName));
    }
    
    private bool IsCustomPagesInRange(string pages, int pageCount, out string errorMessage)
    {
        errorMessage = "";

        string[] parts = pages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string part in parts)
        {
            if (part.Contains('-'))
            {
                string[] range = part.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (range.Length != 2)
                {
                    errorMessage = Loc.InvalidPageRange(part);
                    return false;
                }

                int start = int.Parse(range[0]);
                int end = int.Parse(range[1]);

                if (start < 1 || end < 1 || start > end || end > pageCount)
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
    
    partial void OnSelectedPageModeChanged(PageModeMenuItem? value)
    {
        SelectedMode = value?.Mode ?? PageSelectionMode.CustomPages;
    }
    
    private bool TryParseSplitAfterPages(string value, int pageCount, out List<int> splitAfterPages, out string errorMessage)
    {
        splitAfterPages = new List<int>();
        errorMessage = "";

        string[] parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
        {
            errorMessage = Loc.EnterSplitPages;
            return false;
        }

        foreach (string part in parts)
        {
            if (!int.TryParse(part, out int page))
            {
                errorMessage = Loc.InvalidSplitPage(part);
                return false;
            }

            if (page < 1)
            {
                errorMessage = Loc.SplitPageMustBeGreaterThanZero;
                return false;
            }

            if (page > pageCount)
            {
                errorMessage = Loc.SplitPageMustBeSmallerThanPageCount(pageCount);
                return false;
            }

            splitAfterPages.Add(page - 1);
        }

        if (splitAfterPages.Count != splitAfterPages.Distinct().Count())
        {
            errorMessage = Loc.SplitPagesCannotContainDuplicates;
            return false;
        }

        if (!splitAfterPages.SequenceEqual(splitAfterPages.OrderBy(page => page)))
        {
            errorMessage = Loc.SplitPagesMustBeAscending;
            return false;
        }

        return true;
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
            if (InputFilesList.IsEmpty())
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.SelectPdfFiles;
                return;
            }
            
            string[] currentInputFiles = InputFilesList.ToPathArray();

            string[] missingFiles = currentInputFiles
                .Where(file => !File.Exists(file))
                .ToArray();

            if (missingFiles.Length > 0)
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.MissingInputFiles(string.Join(", ", missingFiles.Select(Path.GetFileName)));
                return;
            }

            if (SelectedMode == PageSelectionMode.CustomPages && string.IsNullOrWhiteSpace(Pages))
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.EnterPages;
                return;
            }
            
            if (SelectedMode == PageSelectionMode.CustomPages && !InputValidator.IsPagesFormatValid(Pages))
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.InvalidPagesFormat;
                return;
            }
            
            if (SelectedMode == PageSelectionMode.SplitByPages && string.IsNullOrWhiteSpace(SplitAfterPages))
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.EnterSplitPages;
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.SelectOutputDirectory;
                return;
            }

            if (!Directory.Exists(OutputDirectory))
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.OutputDirectoryDoesNotExist;
                return;
            }

            StatusMessage = Loc.CreatingPdfWithSelectedPages;

            int savedFilesCount = 0;
            string finalPath = "";

            await Task.Run(() =>
            {
                OperationInput operationInput = new OperationInput
                {
                    InputFiles = currentInputFiles,
                    Pages = Pages,
                    Output = OutputFileName
                };

                OperationDefinition operationDefinition = new OperationDefinition
                {
                    Extension = ".pdf"
                };

                OperationContext operationContext = new OperationContext
                {
                    TempDir = Files.PrepareTempDir()
                };

                try
                {
                    List<FileJob> fileJobs = ExecutionBuilder.SetFileJobsFilesToFiles(
                        operationDefinition,
                        operationInput,
                        operationContext);

                    foreach (FileJob fileJob in fileJobs)
                    {
                        int pageCount = Info.GetPdfPagesSingle(fileJob.InputFile);

                        if (SelectedMode == PageSelectionMode.CustomPages &&
                            !IsCustomPagesInRange(Pages, pageCount, out string pagesErrorMessage))
                        {
                            StatusKind = StatusKind.Error;
                            StatusMessage = $"{Path.GetFileName(fileJob.InputFile)}: {pagesErrorMessage}";
                            return;
                        }

                        if (SelectedMode == PageSelectionMode.SplitEveryPage)
                        {
                            List<int> splitAfterPages = Enumerable
                                .Range(1, pageCount - 1)
                                .ToList();

                            PdfOperations.Pages.SplitPages(fileJob, splitAfterPages, pageCount);

                            foreach (string tempFile in Directory.GetFiles(operationContext.TempDir))
                            {
                                finalPath = Path.Combine(OutputDirectory, Path.GetFileName(tempFile));
                                if (!OverwriteExistingFiles)
                                    finalPath = GetAvailablePath(finalPath);

                                File.Move(tempFile, finalPath, overwrite: OverwriteExistingFiles);
                                savedFilesCount++;
                            }

                            continue;
                        }

                        if (SelectedMode == PageSelectionMode.SplitByPages)
                        {
                            if (!TryParseSplitAfterPages(SplitAfterPages, pageCount, out List<int> splitAfterPages, out string errorMessage))
                            {
                                StatusKind = StatusKind.Error;
                                StatusMessage = $"{Path.GetFileName(fileJob.InputFile)}: {errorMessage}";
                                return;
                            }

                            PdfOperations.Pages.SplitPages(fileJob, splitAfterPages, pageCount);

                            foreach (string tempFile in Directory.GetFiles(operationContext.TempDir))
                            {
                                finalPath = Path.Combine(OutputDirectory, Path.GetFileName(tempFile));
                                if (!OverwriteExistingFiles)
                                    finalPath = GetAvailablePath(finalPath);

                                File.Move(tempFile, finalPath, overwrite: OverwriteExistingFiles);
                                savedFilesCount++;
                            }

                            continue;
                        }

                        operationInput.Pages = SelectedMode switch
                        {
                            PageSelectionMode.CustomPages => Pages,
                            PageSelectionMode.EvenPages => PagesRangeBuilder.BuildEvenPages(pageCount),
                            PageSelectionMode.OddPages => PagesRangeBuilder.BuildOddPages(pageCount),
                            _ => Pages
                        };

                        if (string.IsNullOrWhiteSpace(operationInput.Pages))
                        {
                            StatusKind = StatusKind.Error;
                            StatusMessage = Loc.NoPagesSelected(Path.GetFileName(fileJob.InputFile));
                            return;
                        }

                        PdfOperations.Pages.CreateWithPages(operationInput, fileJob);

                        finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                        if (!OverwriteExistingFiles)
                            finalPath = GetAvailablePath(finalPath);

                        File.Move(fileJob.TempPath, finalPath, overwrite: OverwriteExistingFiles);
                        savedFilesCount++;
                    }
                    
                    StatusKind = StatusKind.Success;
                    StatusMessage = savedFilesCount == 1
                        ? Loc.DoneSaved(finalPath)
                        : Loc.DoneSavedFiles(savedFilesCount, OutputDirectory);
                }
                finally
                {
                    if (Directory.Exists(operationContext.TempDir))
                        Directory.Delete(operationContext.TempDir, true);
                }
            });

            LastOutputDirectory = OutputDirectory;
        }
        catch (Exception e)
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.Error(e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenOutputFolder()
    {
        if (string.IsNullOrWhiteSpace(LastOutputDirectory) ||
            !Directory.Exists(LastOutputDirectory))
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

        string directory = Path.GetDirectoryName(path)!;
        string name = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path);

        int i = 1;

        while (true)
        {
            string candidate = Path.Combine(directory, $"{name}_{i}{extension}");

            if (!File.Exists(candidate))
                return candidate;

            i++;
        }
    }
}
