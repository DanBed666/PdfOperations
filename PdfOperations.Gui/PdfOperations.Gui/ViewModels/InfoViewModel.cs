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
        Title = mode == InfoMode.FontInfo
            ? Loc.GetOperationName("FontInfo")
            : Loc.GetOperationName("Info");
        StatusMessage = Loc.Ready;
    }
    
    public InfoViewModel() : this(InfoMode.PdfInfo)
    {
        
    }

    [RelayCommand]
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
                StatusMessage = Loc.SelectPdfFiles;
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

            StatusMessage = Loc.ReadingPdfInfo;

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
            StatusKind = StatusKind.Success;
            StatusMessage = Loc.DoneSaved(finalPath);
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

    [RelayCommand]
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
