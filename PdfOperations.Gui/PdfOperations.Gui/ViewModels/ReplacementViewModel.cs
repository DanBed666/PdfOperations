using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    public ReplacementViewModel() : base("Select editable files", ["*.docx", "*.odt", "*.odg"])
    {
        StatusMessage = Loc.Ready;
    }
    
    [RelayCommand]
    private async Task Start()
    {
        try
        {
            if (InputFilesList.IsEmpty())
            {
                StatusMessage = Loc.SelectDocumentFiles;
                return;
            }

            if (string.IsNullOrWhiteSpace(PlaceholderFile) || !File.Exists(PlaceholderFile))
            {
                StatusMessage = Loc.SelectExcelReplacementFile;
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
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
                    finalPath = GetAvailablePath(finalPath);

                    File.Move(tempFile, finalPath);
                }

                Directory.Delete(context.TempDir, true);
            });

            LastOutputDirectory = OutputDirectory;
            StatusMessage = Loc.DoneSavedFilesTo(OutputDirectory);
        }
        catch (Exception ex)
        {
            StatusMessage = Loc.Error(ex.Message);
        }
    }

    [RelayCommand]
    private void OpenOutputFolder()
    {
        if (string.IsNullOrWhiteSpace(LastOutputDirectory) || !Directory.Exists(LastOutputDirectory))
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
