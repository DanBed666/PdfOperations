using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PdfOperations.Gui.ViewModels;

public partial class ReplacementViewModel : ViewModelBase
{
    [ObservableProperty]
    private string inputFilesText = "";

    [ObservableProperty]
    private string[] inputFiles = [];

    [ObservableProperty]
    private string placeholderFile = "";

    [ObservableProperty]
    private string outputDirectory = "";

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
                StatusMessage = "Select document files.";
                return;
            }

            if (string.IsNullOrWhiteSpace(PlaceholderFile) || !File.Exists(PlaceholderFile))
            {
                StatusMessage = "Select Excel replacement file.";
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                StatusMessage = "Select output directory.";
                return;
            }

            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);

            string[] missingFiles = InputFiles
                .Where(file => !File.Exists(file))
                .ToArray();

            if (missingFiles.Length > 0)
            {
                StatusMessage = "Missing input files: " + string.Join(", ", missingFiles.Select(Path.GetFileName));
                return;
            }

            StatusMessage = "Replacing text...";

            await Task.Run(() =>
            {
                OperationContext context = new OperationContext
                {
                    TempDir = Files.PrepareTempDir()
                };

                OperationInput input = new OperationInput
                {
                    InputFiles = InputFiles,
                    PlaceholderFile = PlaceholderFile,
                    Output = "gui"
                };

                foreach (string inputFile in InputFiles)
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
            StatusMessage = $"Done. Saved files to: {OutputDirectory}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void OpenOutputFolder()
    {
        if (string.IsNullOrWhiteSpace(LastOutputDirectory) || !Directory.Exists(LastOutputDirectory))
            return;

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