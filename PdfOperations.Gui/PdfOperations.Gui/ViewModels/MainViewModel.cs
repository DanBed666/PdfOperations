using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfOperations.Gui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string[] Operations { get; } =
    [
        "PDF to TXT",
        "PDF to DOCX",
        "Images to PDF",
        "Split PDF",
        "Merge PDF",
        "Search",
        "Info",
        "Replacement"
    ];

    [ObservableProperty]
    private string? selectedOperation;

    [ObservableProperty]
    private string statusMessage = "Ready";
    
    [ObservableProperty]
    private string inputFile = "";

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "output.txt";
    
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

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                StatusMessage = "Select output directory.";
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputFileName))
                OutputFileName = Path.GetFileNameWithoutExtension(InputFile) + ".txt";

            if (Path.GetExtension(OutputFileName).Equals("") ||
                !Path.GetExtension(OutputFileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + ".txt";
            }

            OperationInput operationInput = new OperationInput
            {
                InputFiles = [InputFile],
                Output = OutputFileName
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

                FileJob fileJob = fileJobs[0];

                Convert.PdfToTxt(fileJob);

                string finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));

                if (File.Exists(finalPath))
                    File.Delete(finalPath);

                File.Move(fileJob.TempPath, finalPath);

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
    partial void OnSelectedOperationChanged(string? value)
    {
        StatusMessage = string.IsNullOrWhiteSpace(value)
            ? "Ready"
            : $"Selected operation: {value}";
    }
}