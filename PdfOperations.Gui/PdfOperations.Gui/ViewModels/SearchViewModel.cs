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
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public partial class SearchViewModel : FileInputViewModelBase
{
    [ObservableProperty]
    private string title = "Search";
    
    [ObservableProperty]
    private string outputDirectory = "";

    [ObservableProperty]
    private string outputFileName = "search_report.txt";

    [ObservableProperty]
    private bool exportToExcel;

    [ObservableProperty]
    private int before = 2;

    [ObservableProperty]
    private int after = 2;

    [ObservableProperty]
    private string lastOutputDirectory = "";
    
    [ObservableProperty]
    private bool exportToJson;
    
    public SearchViewModel() 
        : base(
            loc => loc.SelectFilesButton,
            loc => loc.InputSearchFilesWithFormats,
            ["*.pdf", "*.txt", "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"],
            inputTitle: loc => loc.InputSearchFilesWithFormats)
    {
        Title = Loc.GetOperationName("Search");
        AddSearchField();
        
        InputFilesList.FilesChanged += () =>
        {
            SetDefaultOutputDirectoryFromInputFiles(InputFilesList.ToPathArray(), OutputDirectory, value => OutputDirectory = value);
            StartCommand.NotifyCanExecuteChanged();
        };
    }
    
    public ObservableCollection<SearchFieldViewModel> SearchFields { get; } = [];
    
    [RelayCommand]
    private void AddSearchField()
    {
        if (IsBusy)
            return;
    
        var field = new SearchFieldViewModel();
        field.PropertyChanged += SearchField_PropertyChanged;
        SearchFields.Add(field);
        StartCommand.NotifyCanExecuteChanged();
    }
    
    [RelayCommand]
    private void RemoveSearchField(SearchFieldViewModel? field)
    {
        if (IsBusy || field is null || !SearchFields.Remove(field))
            return;
    
        field.PropertyChanged -= SearchField_PropertyChanged;
    
        if (SearchFields.Count == 0)
            AddSearchField();
    
        StartCommand.NotifyCanExecuteChanged();
    }
    
    private void SearchField_PropertyChanged(
        object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SearchFieldViewModel.Phrase))
            StartCommand.NotifyCanExecuteChanged();
    }
    
    private string[] GetSearchPhrases()
    {
        return SearchFields
            .Select(field => field.Phrase.Trim().TrimEnd(':').TrimEnd())
            .Where(phrase => !string.IsNullOrWhiteSpace(phrase))
            .ToArray();
    }
    
    public override void RefreshLocalization()
    {
        Title = Loc.GetOperationName("Search");
        base.RefreshLocalization();
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task Start()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        var cancellationToken = OperationToken;
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

            string[] currentSearchPhrases = GetSearchPhrases();

            if (currentSearchPhrases.Length == 0)
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.EnterSearchPhrase;
                return;
            }

            if (currentSearchPhrases.Distinct(StringComparer.OrdinalIgnoreCase).Count()
                != currentSearchPhrases.Length)
            {
                StatusKind = StatusKind.Error;
                StatusMessage = Loc.DuplicateSearchFields;
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
                OutputFileName = "search_report.txt";

            if (string.IsNullOrWhiteSpace(Path.GetExtension(OutputFileName)) ||
                !Path.GetExtension(OutputFileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + ".txt";
            }

            StatusMessage = Loc.Searching;

            string finalPath = "";
            string finalExcelPath = "";
            string finalJsonPath = "";
            int processedFilesCount = 0;
            IProgress<int> progress = BeginFileProgress(currentInputFiles.Length);
            ExportLabels exportLabels = Loc.SearchExportLabels;

            await Task.Run(() =>
            {
                OperationInput operationInput = new OperationInput
                {
                    InputFiles = currentInputFiles,
                    Output = OutputFileName,
                    PhrasesToFind = currentSearchPhrases,
                    Before = Before,
                    After = After
                };

                OperationDefinition operationDefinition = new OperationDefinition
                {
                    Extension = ".txt"
                };

                OperationContext operationContext = new OperationContext
                {
                    TempDir = Files.PrepareTempDir()
                };

                TemporaryDirectoryCleanup.Run(operationContext.TempDir, () =>
                {
                    OperationContext textContext = new OperationContext
                    {
                        TempDir = Path.Combine(operationContext.TempDir, "texts")
                    };

                    Directory.CreateDirectory(textContext.TempDir);

                    List<FileJob> fileJobs = ExecutionBuilder.SetFileJobsFilesToFiles(
                        operationDefinition,
                        operationInput,
                        textContext);

                    foreach (FileJob fileJob in fileJobs)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        string extension = Path.GetExtension(fileJob.InputFile);

                        if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            Convert.PdfToTxtWithPageBreaks(fileJob, cancellationToken);
                        }
                        else if (extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Copy(fileJob.InputFile, fileJob.TempPath, overwrite: true);
                        }
                        else if (IsImageFile(Path.GetExtension(fileJob.InputFile)))
                        {
                            Convert.PictToTxt(fileJob, cancellationToken);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported search file: {fileJob.InputFile}");
                        }
                    }

                    foreach (FileJob fileJob in fileJobs)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        Search.SearchTempTextFiles(operationInput, operationContext, fileJob, cancellationToken);
                        progress.Report(++processedFilesCount);
                    }

                    string tempReportPath = Path.Combine(operationContext.TempDir, OutputFileName);

                    finalPath = Path.Combine(OutputDirectory, Path.GetFileName(tempReportPath));
                    if (!OverwriteExistingFiles)
                        finalPath = GetAvailablePath(finalPath);

                    cancellationToken.ThrowIfCancellationRequested();
                    
                    FileExtractionResult [] results = ExportToExcel || ExportToJson
                        ? Search.GetFileExtractionResults(operationInput, tempReportPath, cancellationToken)
                        : [];
                    
                    string tempJsonPath = Path.ChangeExtension(tempReportPath, ".json");
                    string tempExcelPath = Path.ChangeExtension(tempReportPath, ".xlsx");

                    if (ExportToExcel)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        ExcelExporter.SaveFieldsToExcel(results, operationInput.PhrasesToFind, tempExcelPath, exportLabels);

                        finalExcelPath = Path.ChangeExtension(finalPath, ".xlsx");
                        if (!OverwriteExistingFiles)
                            finalExcelPath = GetAvailablePath(finalExcelPath);
                    }
                    
                    if (ExportToJson)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        JsonExporter.SaveFieldsToJson(results, tempJsonPath, exportLabels);
                        finalJsonPath = Path.ChangeExtension(finalPath, ".json");
                        if (!OverwriteExistingFiles)
                            finalJsonPath = GetAvailablePath(finalJsonPath);
                    }
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    File.Move(tempReportPath, finalPath, overwrite: OverwriteExistingFiles);
                    
                    if (ExportToExcel)
                        File.Move(tempExcelPath, finalExcelPath, overwrite: OverwriteExistingFiles);
                    
                    if (ExportToJson)
                        File.Move(tempJsonPath, finalJsonPath, overwrite: OverwriteExistingFiles);
                });
            });

            LastOutputDirectory = OutputDirectory;
            StatusKind = StatusKind.Success;
            
            var savedPaths = new List<string> { finalPath };
            if (ExportToExcel) savedPaths.Add(finalExcelPath);
            if (ExportToJson) savedPaths.Add(finalJsonPath);
            
            StatusMessage = string.Join(Environment.NewLine,
                savedPaths.Select(path => Loc.DoneSavedReport(path)));
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
    
    private static bool IsImageFile(string extension)
    {
        return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".tif", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".tiff", StringComparison.OrdinalIgnoreCase);
    }

    private bool CanStart()
    {
        return !IsBusy
               && !InputFilesList.IsEmpty()
               && !string.IsNullOrWhiteSpace(OutputDirectory)
               && !string.IsNullOrWhiteSpace(OutputFileName)
               && GetSearchPhrases().Length > 0;
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
