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

public partial class LibreOfficeViewModel : FileInputViewModelBase
{
    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string selectedFormat = "pdf";

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string lastOutputDirectory = "";

    public List<string> Formats { get; } =
    [
        "pdf",
        "docx",
        "odt",
        "xlsx",
        "html",
        "txt"
    ];
    
    public LibreOfficeViewModel() 
        : base(
            "Select files",
            "Office documents (*.docx, *.odt, *.odg, *.doc, *.rtf, *.xlsx, *.ods, *.pptx, *.odp)",
            ["*.docx", "*.odt", "*.doc", "*.odg", "*.rtf", "*.xlsx", "*.ods", "*.pptx", "*.odp"],
            inputTitle: LocalizationService.Instance.InputOfficeFilesWithFormats)
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
                Directory.CreateDirectory(OutputDirectory);

            if (string.IsNullOrWhiteSpace(SelectedFormat))
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.SelectOutputFormat;
                return;
            }

            int savedFilesCount = 0;

            StatusMessage = Loc.ConvertingTo(SelectedFormat);

            await Task.Run(() =>
            {
                HashSet<string> usedOutputPaths = new(StringComparer.OrdinalIgnoreCase);

                foreach (string inputFile in currentInputFiles)
                {
                    OperationContext context = new() { TempDir = Files.PrepareTempDir() };
                    OperationInput input = new()
                    {
                        InputFiles = [inputFile],
                        Format = SelectedFormat
                    };

                    try
                    {
                        Convert.FileToPdf(input, context);

                        string expectedFile = Path.Combine(context.TempDir,
                            Path.GetFileNameWithoutExtension(inputFile) + "." + SelectedFormat);

                        if (!File.Exists(expectedFile))
                            throw new InvalidOperationException(
                                $"LibreOffice didn't create file for: {inputFile}");

                        if (SelectedFormat == "pdf")
                            Convert.ValidatePdfOutput(expectedFile);

                        string name = Path.GetFileNameWithoutExtension(inputFile);
                        string extension = "." + SelectedFormat;
                        string targetPath = Path.Combine(OutputDirectory, name + extension);
                        int counter = 1;

                        while (usedOutputPaths.Contains(targetPath)
                               || Directory.Exists(targetPath)
                               || (!OverwriteExistingFiles && File.Exists(targetPath)))
                        {
                            targetPath = Path.Combine(
                                OutputDirectory, $"{name}_{counter++}{extension}");
                        }

                        usedOutputPaths.Add(targetPath);

                        File.Move(expectedFile, targetPath, overwrite: OverwriteExistingFiles);

                        savedFilesCount++;
                    }
                    finally
                    {
                        if (Directory.Exists(context.TempDir))
                            Directory.Delete(context.TempDir, true);
                    }
                }
            });

            LastOutputDirectory = OutputDirectory;
            StatusKind = StatusKind.Success;
            StatusMessage = Loc.DoneSavedFiles(savedFilesCount, OutputDirectory);
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
               && !string.IsNullOrWhiteSpace(SelectedFormat);
    }

    partial void OnOutputDirectoryChanged(string value)
    {
        StartCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedFormatChanged(string value)
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
