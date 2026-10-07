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

public partial class ManyToOneViewModel : ViewModelBase
{
    private readonly GuiOperationDefinition operation;

    public ManyToOneViewModel(GuiOperationDefinition operation)
    {
        this.operation = operation;
        Title = operation.Title;
        OutputFileName = operation.DefaultOutputName + operation.OutputExtension;
    }

    public string FileDialogTitle => operation.FileDialogTitle;

    public string[] FilePatterns => operation.FilePatterns;

    [ObservableProperty]
    private string title = "Many to one operation";

    [ObservableProperty]
    private string inputFilesText = "";

    [ObservableProperty]
    private string[] inputFiles = [];

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "output.pdf";

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
                OutputFileName = operation.DefaultOutputName + operation.OutputExtension;

            if (string.IsNullOrWhiteSpace(Path.GetExtension(OutputFileName)) ||
                !Path.GetExtension(OutputFileName).Equals(operation.OutputExtension, StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + operation.OutputExtension;
            }

            StatusMessage = $"Starting {Title}...";

            string finalPath = "";

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
                    FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(
                        operationDefinition,
                        operationInput,
                        operationContext);

                    operation.SingleOutputAction?.Invoke(fileJob);

                    finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                    finalPath = GetAvailablePath(finalPath);

                    File.Move(fileJob.TempPath, finalPath);
                }
                finally
                {
                    if (Directory.Exists(operationContext.TempDir))
                        Directory.Delete(operationContext.TempDir, true);
                }
            });

            LastOutputDirectory = OutputDirectory;
            StatusMessage = $"Done. Saved: {finalPath}";
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