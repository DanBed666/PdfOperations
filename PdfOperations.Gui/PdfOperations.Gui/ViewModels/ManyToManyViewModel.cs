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
        StatusMessage = Loc.Ready;
        OutputFileName = operation.DefaultOutputName + operation.OutputExtension;
        InputFilesList.FilesChanged += () => RefreshInputFilesState();
    }
    
    [ObservableProperty]
    private string title = "Many to many operation";

    [ObservableProperty]
    private string statusMessage = "Ready";
    
    public FileListViewModel InputFilesList { get; } = new();

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "output.txt";
    
    [ObservableProperty]
    private string lastOutputDirectory = "";
    public bool CanEditOutputFileName => InputFilesList.Files.Count <= 1;

    private void RefreshInputFilesState()
    {
        string[] files = InputFilesList.ToPathArray();

        OnPropertyChanged(nameof(CanEditOutputFileName));

        if (files.Length == 1)
        {
            OutputFileName = Path.GetFileNameWithoutExtension(files[0]) + operation.OutputExtension;
        }
        else if (files.Length > 1)
        {
            OutputFileName = "Generated from input file names";
        }
        else
        {
            OutputFileName = "";
        }
    }
    
    [RelayCommand]
    private async Task Start()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            if (InputFilesList.IsEmpty())
            {
                StatusMessage = Loc.SelectInputFile;
                return;
            }
            
            string[] currentInputFiles = InputFilesList.ToPathArray();

            string[] missingFiles = currentInputFiles
                .Where(file => !File.Exists(file))
                .ToArray();

            if (missingFiles.Length > 0)
            {
                StatusMessage = Loc.MissingInputFiles(string.Join(", ", missingFiles.Select(Path.GetFileName)));
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                StatusMessage = Loc.SelectOutputDirectory;
                return;
            }

            if (!Directory.Exists(OutputDirectory))
            {
                StatusMessage = Loc.OutputDirectoryDoesNotExist;
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputFileName))
                OutputFileName = currentInputFiles.Length == 1
                    ? Path.GetFileNameWithoutExtension(currentInputFiles[0]) + operation.OutputExtension
                    : operation.DefaultOutputName + operation.OutputExtension;

            if (string.IsNullOrWhiteSpace(Path.GetExtension(OutputFileName)) ||
                !Path.GetExtension(OutputFileName).Equals(operation.OutputExtension, StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + operation.OutputExtension;
            }

            StatusMessage = Loc.Starting(Title);

            string finalPath = "";
            
            int savedFilesCount = 0;

            await Task.Run(() =>
            {
                OperationInput operationInput = new OperationInput
                {
                    InputFiles = currentInputFiles,
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

                        finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                        finalPath = GetAvailablePath(finalPath);

                        File.Move(fileJob.TempPath, finalPath);
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
                ? Loc.DoneSaved(finalPath)
                : Loc.DoneSavedFiles(savedFilesCount, OutputDirectory);
        }
        catch (Exception e)
        {
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
