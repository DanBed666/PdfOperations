using System;
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
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string lastOutputDirectory = "";

    public ReplacementViewModel()
        : base(
            "Select editable files",
            "Editable documents (*.docx, *.odt, *.odg)",
            ["*.docx", "*.odt", "*.odg"],
            inputTitle: LocalizationService.Instance.InputEditableFilesWithFormats)
    {
        StatusMessage = Loc.Ready;
        InputFilesList.FilesChanged += () =>
        {
            SetDefaultOutputDirectoryFromInputFiles(InputFilesList.ToPathArray(), OutputDirectory, value => OutputDirectory = value);
            StartCommand.NotifyCanExecuteChanged();
        };
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

                foreach (string inputFile in currentInputFiles)
                {
                    FileJob fileJob = new FileJob
                    {
                        InputFile = inputFile,
                        TempPath = Path.Combine(context.TempDir, Path.GetFileName(inputFile))
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
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.Error(ex.Message);
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
