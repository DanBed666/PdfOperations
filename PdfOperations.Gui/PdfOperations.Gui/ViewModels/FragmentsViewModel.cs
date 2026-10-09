using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PdfOperations.Gui.ViewModels;

public partial class FragmentsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string selectedFragmentFile = "";

    [ObservableProperty]
    private string pageNumbers = "";

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "fragments.pdf";

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [ObservableProperty]
    private string selectedFragmentFileInfo = "";

    [ObservableProperty]
    private PdfFragment? selectedFragment;

    public ObservableCollection<PdfFragment> Fragments { get; } = new();

    [RelayCommand]
    private void AddFragment()
    {
        if (string.IsNullOrWhiteSpace(SelectedFragmentFile) || !File.Exists(SelectedFragmentFile))
        {
            StatusMessage = "Select PDF file.";
            return;
        }

        if (string.IsNullOrWhiteSpace(PageNumbers))
        {
            StatusMessage = "Enter pages, for example: 1,3-5.";
            return;
        }

        Fragments.Add(new PdfFragment
        {
            FileName = SelectedFragmentFile,
            PageNumbers = PageNumbers.Trim().Replace(" ", "")
        });

        SelectedFragmentFile = "";
        PageNumbers = "";
        StatusMessage = "Fragment added.";
    }

    [RelayCommand]
    private void RemoveFragment()
    {
        if (SelectedFragment is null)
            return;

        Fragments.Remove(SelectedFragment);
        SelectedFragment = null;
        StatusMessage = "Fragment removed.";
    }

    [RelayCommand]
    private async Task Start()
    {
        try
        {
            if (Fragments.Count == 0)
            {
                StatusMessage = "Add at least one fragment.";
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                StatusMessage = "Select output directory.";
                return;
            }

            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);

            if (string.IsNullOrWhiteSpace(OutputFileName))
                OutputFileName = "fragments.pdf";

            if (!Path.GetExtension(OutputFileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + ".pdf";

            StatusMessage = "Building PDF from fragments...";

            string finalPath = "";

            await Task.Run(() =>
            {
                OperationContext context = new OperationContext
                {
                    TempDir = Files.PrepareTempDir()
                };

                OperationInput input = new OperationInput
                {
                    PdfFragments = Fragments.ToList(),
                    Output = OutputFileName
                };

                OperationDefinition operation = new OperationDefinition
                {
                    Extension = ".pdf"
                };

                try
                {
                    FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operation, input, context);

                    Pages.CreateWithCustomFiles(input, fileJob);

                    finalPath = Path.Combine(OutputDirectory, Path.GetFileName(fileJob.TempPath));
                    finalPath = GetAvailablePath(finalPath);

                    File.Move(fileJob.TempPath, finalPath);
                }
                finally
                {
                    if (Directory.Exists(context.TempDir))
                        Directory.Delete(context.TempDir, true);
                }
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