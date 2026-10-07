using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PdfOperations.Gui.ViewModels;

public partial class SearchViewModel : ViewModelBase
{
    [ObservableProperty]
    private string title = "Search";

    [ObservableProperty]
    private string inputFilesText = "";

    [ObservableProperty]
    private string[] inputFiles = [];

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "search_report.txt";

    [ObservableProperty]
    private string phrase = "";

    [ObservableProperty]
    private int before = 2;

    [ObservableProperty]
    private int after = 2;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [RelayCommand]
    private async Task Start()
    {
        try
        {
            if (InputFiles.Length == 0)
            {
                StatusMessage = "Select input files.";
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

            if (string.IsNullOrWhiteSpace(Phrase))
            {
                StatusMessage = "Enter search phrase.";
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

            if (string.IsNullOrWhiteSpace(OutputFileName))
                OutputFileName = "search_report.txt";

            if (string.IsNullOrWhiteSpace(Path.GetExtension(OutputFileName)) ||
                !Path.GetExtension(OutputFileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + ".txt";
            }

            StatusMessage = "Searching...";

            string finalPath = "";

            await Task.Run(() =>
            {
                OperationInput operationInput = new OperationInput
                {
                    InputFiles = InputFiles,
                    Output = OutputFileName,
                    PhraseToFind = Phrase,
                    Before = Before,
                    After = After
                };

                OperationDefinition operationDefinition = new OperationDefinition
                {
                    Extension = ".txt"
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
                        string extension = Path.GetExtension(fileJob.InputFile);

                        if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            Convert.PdfToTxt(fileJob);
                        }
                        else if (extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Copy(fileJob.InputFile, fileJob.TempPath, overwrite: true);
                        }
                        else if (IsImageFile(Path.GetExtension(fileJob.InputFile)))
                        {
                            Convert.PictToTxt(fileJob);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported search file: {fileJob.InputFile}");
                        }
                    }

                    foreach (FileJob fileJob in fileJobs)
                    {
                        Search.SearchTempTextFiles(operationInput, operationContext, fileJob);
                    }

                    string tempReportPath = Path.Combine(operationContext.TempDir, OutputFileName);

                    finalPath = Path.Combine(OutputDirectory, Path.GetFileName(tempReportPath));
                    finalPath = GetAvailablePath(finalPath);

                    File.Move(tempReportPath, finalPath);
                }
                finally
                {
                    if (Directory.Exists(operationContext.TempDir))
                        Directory.Delete(operationContext.TempDir, true);
                }
            });

            LastOutputDirectory = OutputDirectory;
            StatusMessage = $"Done. Saved report: {finalPath}";
        }
        catch (Exception e)
        {
            StatusMessage = $"Error: {e.Message}";
        }
    }
    
    private static bool IsImageFile(string extension)
    {
        return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".tif", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".tiff", StringComparison.OrdinalIgnoreCase);
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