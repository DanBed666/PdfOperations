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

public partial class ManyToOneViewModel : ViewModelBase
{
    private readonly GuiOperationDefinition operation;

    public ManyToOneViewModel(GuiOperationDefinition operation)
    {
        this.operation = operation;
        Title = operation.Title;
        OutputFileName = operation.DefaultOutputName + operation.OutputExtension;
        InputFilesList.ShowMoveButtons = true;
        InputFilesList.FilesChanged += () =>
        {
            SetDefaultOutputDirectoryFromInputFiles(InputFilesList.ToPathArray(), OutputDirectory, value => OutputDirectory = value);
            StartCommand.NotifyCanExecuteChanged();
        };
    }

    public string InputTitle => operation.Name == "ImagesToPdf"
        ? Loc.InputImageFilesWithFormats
        : Loc.InputPdfFilesWithFormats;

    public string FileDialogTitle => operation.Name == "ImagesToPdf"
        ? Loc.SelectImageFiles
        : Loc.SelectPdfFiles;
    public string FileTypeName => InputTitle;

    public string[] FilePatterns => operation.FilePatterns;

    public override void RefreshLocalization()
    {
        Title = Loc.GetOperationName(operation.Name);
        base.RefreshLocalization();
    }

    [ObservableProperty]
    private string title = "Many to one operation";

    public FileListViewModel InputFilesList { get; } = new();
    
    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "output.pdf";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [RelayCommand(CanExecute = nameof(CanStart))]
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
                StatusMessage = Loc.SelectInputFiles;
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

            if (string.IsNullOrWhiteSpace(OutputFileName))
                OutputFileName = operation.DefaultOutputName + operation.OutputExtension;

            if (string.IsNullOrWhiteSpace(Path.GetExtension(OutputFileName)) ||
                !Path.GetExtension(OutputFileName).Equals(operation.OutputExtension, StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + operation.OutputExtension;
            }

            StatusMessage = Loc.Starting(Title);

            string finalPath = "";

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
                    FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(
                        operationDefinition,
                        operationInput,
                        operationContext);

                    operation.SingleOutputAction?.Invoke(fileJob);

                    finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                    if (!OverwriteExistingFiles)
                        finalPath = GetAvailablePath(finalPath);

                    File.Move(fileJob.TempPath, finalPath, overwrite: OverwriteExistingFiles);
                }
                finally
                {
                    if (Directory.Exists(operationContext.TempDir))
                        Directory.Delete(operationContext.TempDir, true);
                }
            });

            LastOutputDirectory = OutputDirectory;
            StatusKind = StatusKind.Success;
            StatusMessage = Loc.DoneSaved(finalPath);
        }
        catch (Exception e)
        {
            ShowError(e);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanStart()
    {
        return !InputFilesList.IsEmpty()
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
