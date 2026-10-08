using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public partial class PagesViewModel : ViewModelBase
{
    [ObservableProperty]
    private string title = "PDF pages";

    [ObservableProperty]
    private string inputFilesText = "";

    [ObservableProperty]
    private string[] inputFiles = [];

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
    private string selectedPageMode = "Custom pages";

    [ObservableProperty]
    private string splitAfterPages = "";
    
    public List<string> PageModes { get; } =
    [
        "Custom pages",
        "Even pages",
        "Odd pages",
        "Split by pages"
    ];
    
    public bool IsCustomPagesMode => SelectedMode == PageSelectionMode.CustomPages;

    public bool IsSplitByPagesMode => SelectedMode == PageSelectionMode.SplitByPages;

    public bool CanEditOutputFileName => InputFiles.Length == 1;
    
    partial void OnSelectedModeChanged(PageSelectionMode value)
    {
        OnPropertyChanged(nameof(IsCustomPagesMode));
        OnPropertyChanged(nameof(IsSplitByPagesMode));
    }
    
    partial void OnSelectedPageModeChanged(string value)
    {
        SelectedMode = value switch
        {
            "Even pages" => PageSelectionMode.EvenPages,
            "Odd pages" => PageSelectionMode.OddPages,
            "Split by pages" => PageSelectionMode.SplitByPages,
            _ => PageSelectionMode.CustomPages
        };
    }

    partial void OnInputFilesChanged(string[] value)
    {
        OnPropertyChanged(nameof(CanEditOutputFileName));

        if (value.Length == 1)
            OutputFileName = Path.GetFileNameWithoutExtension(value[0]) + ".pdf";
        else if (value.Length > 1)
            OutputFileName = "Generated from input file names";
        else
            OutputFileName = "";
    }
    
    private static bool TryParseSplitAfterPages(string value, int pageCount, out List<int> splitAfterPages, out string errorMessage)
    {
        splitAfterPages = new List<int>();
        errorMessage = "";

        string[] parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
        {
            errorMessage = "Enter split pages, for example: 4,10,15.";
            return false;
        }

        foreach (string part in parts)
        {
            if (!int.TryParse(part, out int page))
            {
                errorMessage = $"Invalid split page: {part}.";
                return false;
            }

            if (page < 1)
            {
                errorMessage = "Split page must be greater than 0.";
                return false;
            }

            if (page >= pageCount)
            {
                errorMessage = $"Split page must be smaller than page count ({pageCount}).";
                return false;
            }

            splitAfterPages.Add(page);
        }

        if (splitAfterPages.Count != splitAfterPages.Distinct().Count())
        {
            errorMessage = "Split pages cannot contain duplicates.";
            return false;
        }

        if (!splitAfterPages.SequenceEqual(splitAfterPages.OrderBy(page => page)))
        {
            errorMessage = "Split pages must be in ascending order.";
            return false;
        }

        return true;
    }

    [RelayCommand]
    private async Task Start()
    {
        try
        {
            if (InputFiles.Length == 0)
            {
                StatusMessage = "Select input PDF files.";
                return;
            }

            string[] missingFiles = InputFiles
                .Where(file => !File.Exists(file))
                .ToArray();

            if (missingFiles.Length > 0)
            {
                StatusMessage = "Missing input files: " + string.Join(", ", missingFiles.Select(Path.GetFileName));
                return;
            }

            if (SelectedMode == PageSelectionMode.CustomPages && string.IsNullOrWhiteSpace(Pages))
            {
                StatusMessage = "Enter pages, for example: 1,3-5.";
                return;
            }
            
            if (SelectedMode == PageSelectionMode.SplitByPages && string.IsNullOrWhiteSpace(SplitAfterPages))
            {
                StatusMessage = "Enter split pages, for example: 4,10,15.";
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                StatusMessage = "Select output directory.";
                return;
            }

            if (!Directory.Exists(OutputDirectory))
            {
                StatusMessage = "Output directory does not exist.";
                return;
            }

            StatusMessage = "Creating PDF with selected pages...";

            int savedFilesCount = 0;
            string finalPath = "";

            await Task.Run(() =>
            {
                OperationInput operationInput = new OperationInput
                {
                    InputFiles = InputFiles,
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
                        
                        operationInput.Pages = SelectedMode switch
                        {
                            PageSelectionMode.CustomPages => Pages,
                            PageSelectionMode.EvenPages => PagesRangeBuilder.BuildEvenPages(pageCount),
                            PageSelectionMode.OddPages => PagesRangeBuilder.BuildOddPages(pageCount),
                            PageSelectionMode.SplitByPages => "",
                            _ => Pages
                        };
                        
                        if (SelectedMode == PageSelectionMode.SplitByPages)
                        {
                            if (!TryParseSplitAfterPages(SplitAfterPages, pageCount, out List<int> splitAfterPages, out string errorMessage))
                            {
                                StatusMessage = errorMessage;
                                return;
                            }

                            PdfOperations.Pages.SplitPages(fileJob, splitAfterPages, pageCount);

                            foreach (string tempFile in Directory.GetFiles(operationContext.TempDir))
                            {
                                string finalPath = Path.Combine(OutputDirectory, Path.GetFileName(tempFile));
                                finalPath = GetAvailablePath(finalPath);

                                File.Move(tempFile, finalPath);
                                savedFilesCount++;
                            }

                            continue;
                        }
                        
                        PdfOperations.Pages.CreateWithPages(operationInput, fileJob);

                        finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                        finalPath = GetAvailablePath(finalPath);

                        File.Move(fileJob.TempPath, finalPath);
                        savedFilesCount++;
                    }
                    
                    StatusMessage = savedFilesCount == 1
                        ? $"Done. Saved: {finalPath}"
                        : $"Done. Saved {savedFilesCount} files to: {OutputDirectory}";
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
            StatusMessage = $"Error: {e.Message}";
        }
    }

    [RelayCommand]
    private void OpenOutputFolder()
    {
        if (string.IsNullOrWhiteSpace(LastOutputDirectory) ||
            !Directory.Exists(LastOutputDirectory))
        {
            StatusMessage = "No output folder to open.";
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