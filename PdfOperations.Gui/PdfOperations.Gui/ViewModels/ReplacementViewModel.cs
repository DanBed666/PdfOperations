using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfOperations.Gui.Models;
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public partial class ReplacementViewModel : FileInputViewModelBase
{
    [ObservableProperty]
    private string placeholderFile = "";

    [ObservableProperty]
    private string outputDirectory = "";
    
    [ObservableProperty]
    private string outputFileName = "";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    public bool CanEditOutputFileName => InputFilesList.Files.Count == 1;

    public ReplacementViewModel()
        : base(
            loc => loc.SelectEditableFiles,
            loc => loc.InputEditableFilesWithFormats,
            ["*.docx", "*.odt", "*.odg"],
            inputTitle: loc => loc.InputEditableFilesWithFormats)
    {
        InputFilesList.FilesChanged += () =>
        {
            SetDefaultOutputDirectoryFromInputFiles(InputFilesList.ToPathArray(), OutputDirectory, value => OutputDirectory = value);
            StartCommand.NotifyCanExecuteChanged();
            RefreshInputFilesState();
        };
    }
    
    private void RefreshInputFilesState()
    {
        string[] files = InputFilesList.ToPathArray();

        SetDefaultOutputDirectoryFromInputFiles(files, OutputDirectory, value => OutputDirectory = value);
        OnPropertyChanged(nameof(CanEditOutputFileName));

        if (files.Length == 1)
            OutputFileName = Path.GetFileName(files[0]);
        else
            OutputFileName = "";
    }
    
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
                StatusMessage = Loc.SelectDocumentFiles;
                return;
            }

            if (string.IsNullOrWhiteSpace(PlaceholderFile) || !File.Exists(PlaceholderFile))
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.SelectExcelReplacementFile;
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
            
            string[] currentInputFiles = InputFilesList.ToPathArray();
            
            if (currentInputFiles.Length == 1 && string.IsNullOrWhiteSpace(OutputFileName))
            {
                OutputFileName = Path.GetFileName(currentInputFiles[0]);
            }

            string[] missingFiles = currentInputFiles
                .Where(file => !File.Exists(file))
                .ToArray();

            if (missingFiles.Length > 0)
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.MissingInputFiles(string.Join(", ", missingFiles.Select(Path.GetFileName)));
                return;
            }

            StatusMessage = Loc.ReplacingText;

            await Task.Run(() =>
            {
                OperationContext context = new OperationContext
                {
                    TempDir = Files.PrepareTempDir()
                };

                OperationInput input = new OperationInput
                {
                    InputFiles = currentInputFiles,
                    PlaceholderFile = PlaceholderFile,
                    Output = "gui"
                };

                HashSet<string> usedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (string inputFile in currentInputFiles)
                {
                    string tempOutputName = currentInputFiles.Length == 1
                        ? OutputFileName
                        : inputFile;

                    FileJob fileJob = new FileJob
                    {
                        InputFile = inputFile,
                        TempPath = Files.PrepareUniqueTempPath(
                            context.TempDir,
                            tempOutputName,
                            Path.GetExtension(inputFile),
                            usedFileNames)
                    };

                    Replacement.ReplaceTextWithPlaceholders(fileJob, input, context);
                }

                foreach (string tempFile in Directory.GetFiles(context.TempDir))
                {
                    string finalPath = Path.Combine(OutputDirectory, Path.GetFileName(tempFile));
                    if (!OverwriteExistingFiles)
                        finalPath = GetAvailablePath(finalPath);

                    File.Move(tempFile, finalPath, overwrite: OverwriteExistingFiles);
                }

                Directory.Delete(context.TempDir, true);
            });

            LastOutputDirectory = OutputDirectory;
            StatusKind = StatusKind.Success;
            StatusMessage = Loc.DoneSavedFilesTo(OutputDirectory);
        }
        catch (Exception ex)
        {
            ShowError(ex);
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
               && !string.IsNullOrWhiteSpace(PlaceholderFile);
    }

    partial void OnOutputDirectoryChanged(string value)
    {
        StartCommand.NotifyCanExecuteChanged();
    }

    partial void OnPlaceholderFileChanged(string value)
    {
        StartCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanOpenOutputFolder))]
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
