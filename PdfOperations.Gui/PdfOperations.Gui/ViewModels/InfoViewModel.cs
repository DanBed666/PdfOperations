using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfOperations.Gui.Models;
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public partial class InfoViewModel : FileInputViewModelBase
{
    private const int PreviewCharacterLimit = 200_000;

    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "pdf_info.txt";

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [ObservableProperty]
    private InfoMode mode = InfoMode.PdfInfo;

    [ObservableProperty]
    private string title = "PDF info";

    [ObservableProperty]
    private string defaultOutputFileName = "pdf_info.txt";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasReport))]
    private string reportText = "";

    public bool HasReport => !string.IsNullOrWhiteSpace(ReportText);
    
    public InfoViewModel(InfoMode mode)
        : base(
            loc => mode == InfoMode.FontInfo
                ? loc.SelectFontInputFiles : loc.SelectPdfFiles,
            loc => mode == InfoMode.FontInfo
                ? loc.InputFontFilesWithFormats : loc.InputPdfFilesWithFormats,
            mode == InfoMode.FontInfo
                ? ["*.pdf", "*.docx", "*.pptx"] : ["*.pdf"],
            inputTitle: loc => mode == InfoMode.FontInfo
                ? loc.InputFontFilesWithFormats : loc.InputPdfFilesWithFormats)
    {
        this.mode = mode;
        if (mode == InfoMode.FontInfo)
            OutputFileName = DefaultOutputFileName = "font_info.txt";

        Title = Loc.GetOperationName(
            mode == InfoMode.FontInfo ? "FontInfo" : "Info");

        InputFilesList.FilesChanged += () =>
        {
            SetDefaultOutputDirectoryFromInputFiles(
                InputFilesList.ToPathArray(), OutputDirectory,
                value => OutputDirectory = value);
            StartCommand.NotifyCanExecuteChanged();
        };
    }
    
    public InfoViewModel() : this(InfoMode.PdfInfo)
    {
        
    }

    public override void RefreshLocalization()
    {
        Title = Loc.GetOperationName(Mode == InfoMode.FontInfo ? "FontInfo" : "Info");
        base.RefreshLocalization();
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task Start()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ReportText = "";
        var cancellationToken = OperationToken;
        StatusKind = StatusKind.Info;
        string? stagedReport = null;
        bool ownsStagedReport = false;

        try
        {
            if (InputFilesList.IsEmpty())
            {
                StatusKind = StatusKind.Error;
                StatusMessage = FileDialogTitle;
                return;
            }
            
            string[] currentInputFiles = InputFilesList.ToPathArray();
            
            if (currentInputFiles.Any(file => !FilePatterns.Any(pattern =>
                    Path.GetExtension(file).Equals(
                        pattern[1..], StringComparison.OrdinalIgnoreCase))))
            {
                throw new NotSupportedException(FileTypeName);
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
                OutputFileName = DefaultOutputFileName;
            }

            if (!Path.GetExtension(OutputFileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + ".txt";
            }

            StatusMessage = Mode == InfoMode.FontInfo ? Loc.ReadingFontInfo : Loc.ReadingPdfInfo;

            string finalPath = Path.Combine(OutputDirectory, OutputFileName);
            if (!OverwriteExistingFiles)
                finalPath = GetAvailablePath(finalPath);

            int processedFilesCount = 0;
            IProgress<int> progress = BeginFileProgress(currentInputFiles.Length);
            bool fontMode = Mode == InfoMode.FontInfo;
            bool polish = Loc.Language == AppLanguage.Polish;
            bool overwrite = OverwriteExistingFiles;
            var preview = new StringBuilder();
            bool previewTruncated = false;

            void AppendPreview(string text)
            {
                int count = Math.Min(text.Length, PreviewCharacterLimit - preview.Length);
                preview.Append(text, 0, count);
                previewTruncated |= count < text.Length;
            }

            if (fontMode)
                stagedReport = Path.Combine(
                    Path.GetDirectoryName(finalPath)!, $".font-report-{Guid.NewGuid():N}.tmp");

            await Task.Run(() =>
            {
                IInstalledFontCatalog? catalog = null;
                if (fontMode && currentInputFiles.Any(file =>
                        !Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase)))
                    catalog = WindowsFontCatalog.Read(cancellationToken);

                if (fontMode)
                {
                    using var reservation = new FileStream(
                        stagedReport!, FileMode.CreateNew, FileAccess.Write);
                    ownsStagedReport = true;
                }

                foreach (string inputFile in currentInputFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (fontMode)
                    {
                        string report;
                        string previewReport;

                        if (Path.GetExtension(inputFile)
                            .Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            report = RunClass.RunWithOutput(
                                ToolPaths.ToolPathsDict[Tool.PdfFonts],
                                OperationTimeouts.PdfFontInfo, cancellationToken, inputFile);

                            previewReport = report;
                        }
                        else
                        {
                            var document = OfficeFontReader.Read(inputFile, cancellationToken);

                            report = OfficeFontReport.Format(
                                document, catalog!, polish, cancellationToken);

                            previewReport = OfficeFontReport.Format(
                                document, catalog!, polish, cancellationToken, compact: true);
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        Info.SaveToFile(stagedReport!, report, inputFile);

                        AppendPreview(Path.GetFileName(inputFile) + Environment.NewLine);
                        AppendPreview(previewReport);
                        AppendPreview(Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        Info.ShowInfo(new FileJob
                        {
                            InputFiles = [inputFile],
                            TempPath = finalPath
                        }, cancellationToken);
                    }
                    progress.Report(++processedFilesCount);
                }

                cancellationToken.ThrowIfCancellationRequested();
                if (fontMode)
                {
                    File.Move(stagedReport!, finalPath, overwrite);
                    ownsStagedReport = false;
                }
            });

            if (previewTruncated && preview.Length > 0 && char.IsHighSurrogate(preview[^1]))
                preview.Length--;
            ReportText = preview.ToString();
            if (previewTruncated)
                ReportText += Environment.NewLine + Loc.FontPreviewTruncated;

            LastOutputDirectory = OutputDirectory;
            StatusKind = StatusKind.Success;
            StatusMessage = Loc.DoneSaved(finalPath);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            if (ownsStagedReport && stagedReport is not null)
            {
                try { File.Delete(stagedReport); }
                catch (IOException ex) { Debug.WriteLine(ex); }
                catch (UnauthorizedAccessException ex) { Debug.WriteLine(ex); }
            }
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
