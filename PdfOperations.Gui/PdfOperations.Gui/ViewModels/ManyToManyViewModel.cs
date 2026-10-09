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

public partial class ManyToManyViewModel : ViewModelBase
{
    private readonly GuiOperationDefinition operation;
    
    public string InputTitle => operation.InputTitle;
    public string FileDialogTitle => operation.FileDialogTitle;
    public string[] FilePatterns => operation.FilePatterns;

    public ManyToManyViewModel(GuiOperationDefinition operation)
    {
        this.operation = operation;
        Title = operation.Title;
        OutputFileName = operation.DefaultOutputName + operation.OutputExtension;
    }
    
    [ObservableProperty]
    private string title = "Many to many operation";

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string [] inputFiles = [];
    
    [ObservableProperty]
    private string inputFilesText = "";

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "output.txt";
    
    [ObservableProperty]
    private string lastOutputDirectory = "";
    public bool CanEditOutputFileName => InputFiles.Length == 1;
    
    [RelayCommand]
    private async Task Start()
    {
        try
        {
            if (InputFiles.Length == 0)
            {
                StatusMessage = "Select input file.";
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
                OutputFileName = InputFiles.Length == 1
                    ? Path.GetFileNameWithoutExtension(InputFiles[0]) + operation.OutputExtension
                    : operation.DefaultOutputName + operation.OutputExtension;

            if (string.IsNullOrWhiteSpace(Path.GetExtension(OutputFileName)) ||
                !Path.GetExtension(OutputFileName).Equals(operation.OutputExtension, StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + operation.OutputExtension;
            }

            StatusMessage = $"Starting {Title}...";

            string finalPath = "";
            
            int savedFilesCount = 0;

            await Task.Run(() =>
            {
                OperationInput operationInput = new OperationInput
                {
                    InputFiles = InputFiles,
                    Output = OutputFileName
                };

                OperationDefinition operationDefinition = new OperationDefinition
                {
                    Extension = operation.OutputExtension
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
                        operation.Action?.Invoke(operationInput, operationContext, fileJob);

                        if (operation.MoveAllTempFiles)
                        {
                            foreach (string tempFile in Directory.GetFiles(operationContext.TempDir))
                            {
                                string finalPath = Path.Combine(OutputDirectory, Path.GetFileName(tempFile));
                                finalPath = GetAvailablePath(finalPath);

                                File.Move(tempFile, finalPath);
                                savedFilesCount++;
                            }

                            continue;
                        }

                        string finalSinglePath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                        finalSinglePath = GetAvailablePath(finalSinglePath);

                        File.Move(fileJob.TempPath, finalSinglePath);
                        savedFilesCount++;
                    }
                }
                finally
                {
                    if (Directory.Exists(operationContext.TempDir))
                        Directory.Delete(operationContext.TempDir, true);
                }
            });

            LastOutputDirectory = OutputDirectory;
            
            StatusMessage = savedFilesCount == 1
                ? $"Done. Saved: {finalPath}"
                : $"Done. Saved {savedFilesCount} files to: {OutputDirectory}";
        }
        catch (Exception e)
        {
            StatusMessage = $"Error: {e.Message}";
        }
    }
    
    partial void OnInputFilesChanged(string[] value)
    {
        OnPropertyChanged(nameof(CanEditOutputFileName));

        if (value.Length == 1)
        {
            OutputFileName = Path.GetFileNameWithoutExtension(value[0]) + operation.OutputExtension;
        }
        else if (value.Length > 1)
        {
            OutputFileName = "Generated from input file names";
        }
        else
        {
            OutputFileName = "";
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