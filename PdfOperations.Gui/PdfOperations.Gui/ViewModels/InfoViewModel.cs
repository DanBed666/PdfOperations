using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public partial class InfoViewModel : FileInputViewModelBase
{
    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "pdf_info.txt";

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [ObservableProperty]
    private InfoMode mode = InfoMode.PdfInfo;

    [ObservableProperty]
    private string title = "PDF info";

    [ObservableProperty]
    private string defaultOutputFileName = "pdf_info.txt";
    
    public InfoViewModel(InfoMode mode) : base("Select PDF files", ["*.pdf"])
    {
        this.mode = mode;
    }
    
    public InfoViewModel() : this(InfoMode.PdfInfo)
    {
        
    }

    [RelayCommand]
    private async Task Start()
    {
        try
        {
            if (InputFilesList.IsEmpty())
            {
                StatusMessage = "Select PDF files.";
                return;
            }
            
            string[] currentInputFiles = InputFilesList.ToPathArray();

            string[] missingFiles = currentInputFiles
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
                Directory.CreateDirectory(OutputDirectory);
            }

            if (string.IsNullOrWhiteSpace(OutputFileName))
            {
                OutputFileName = "pdf_info.txt";
            }

            if (!Path.GetExtension(OutputFileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + ".txt";
            }

            StatusMessage = "Reading PDF info...";

            string finalPath = Path.Combine(OutputDirectory, OutputFileName);
            finalPath = GetAvailablePath(finalPath);

            await Task.Run(() =>
            {
                FileJob fileJob = new FileJob
                {
                    InputFiles = currentInputFiles,
                    TempPath = finalPath
                };

                if (Mode == InfoMode.FontInfo)
                    Info.ShowFontInfo(fileJob);
                else
                    Info.ShowInfo(fileJob);
            });

            LastOutputDirectory = OutputDirectory;
            StatusMessage = $"Done. Saved: {finalPath}";
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