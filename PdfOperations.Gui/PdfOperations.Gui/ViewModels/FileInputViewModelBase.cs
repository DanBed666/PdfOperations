using System;
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public abstract class FileInputViewModelBase : ViewModelBase
{
    private readonly Func<LocalizationService, string> fileDialogTitle;
    private readonly Func<LocalizationService, string> fileTypeName;
    private readonly Func<LocalizationService, string>? inputTitle;

    protected FileInputViewModelBase(
        Func<LocalizationService, string> fileDialogTitle,
        Func<LocalizationService, string> fileTypeName,
        string[] filePatterns,
        bool showMoveButtons = false,
        Func<LocalizationService, string>? inputTitle = null)
    {
        this.inputTitle = inputTitle;
        this.fileDialogTitle = fileDialogTitle;
        this.fileTypeName = fileTypeName;
        FilePatterns = filePatterns;
        InputFilesList.ShowMoveButtons = showMoveButtons;
    }

    public string InputTitle => inputTitle?.Invoke(Loc) ?? Loc.InputFiles;

    public string FileDialogTitle => fileDialogTitle(Loc);

    public string FileTypeName => fileTypeName(Loc);

    public string[] FilePatterns { get; }

    public FileListViewModel InputFilesList { get; } = new();

    protected bool TryGetInputFiles(out string[] inputFiles)
    {
        inputFiles = InputFilesList.ToPathArray();

        if (inputFiles.Length == 0)
            return false;

        return true;
    }
}
