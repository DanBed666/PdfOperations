using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    private string inputFile = "";

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "output.txt";
    
    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [RelayCommand]
    private void StartPdfToTxt()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(InputFile))
            {
                StatusMessage = "Select input PDF file.";
                return;
            }

            if (!File.Exists(InputFile))
            {
                StatusMessage = "Input file does not exist.";
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
                OutputFileName = Path.GetFileNameWithoutExtension(InputFile) + operation.OutputExtension;

            if (Path.GetExtension(OutputFileName).Equals("") ||
                !Path.GetExtension(OutputFileName).Equals(operation.OutputExtension, StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + operation.OutputExtension;
            }

            StatusMessage = "Converting PDF to TXT...";

            OperationInput operationInput = new OperationInput
            {
                InputFiles = [InputFile],
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

                FileJob fileJob = fileJobs[0];
                operation.Action?.Invoke(operationInput, operationContext, fileJob);

                string finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                finalPath = GetAvailablePath(finalPath);

                File.Move(fileJob.TempPath, finalPath);
                LastOutputDirectory = OutputDirectory;
                StatusMessage = $"Saved: {finalPath}";
            }
            finally
            {
                if (Directory.Exists(operationContext.TempDir))
                    Directory.Delete(operationContext.TempDir, true);
            }
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