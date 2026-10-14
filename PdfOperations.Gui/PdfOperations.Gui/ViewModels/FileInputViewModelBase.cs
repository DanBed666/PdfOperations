namespace PdfOperations.Gui.ViewModels;

public abstract class FileInputViewModelBase : ViewModelBase
{
    protected FileInputViewModelBase(
        string fileDialogTitle,
        string fileTypeName,
        string[] filePatterns,
        bool showMoveButtons = false,
        string? inputTitle = null)
    {
        InputTitle = inputTitle ?? Loc.InputFiles;
        FileDialogTitle = fileDialogTitle;
        FileTypeName = fileTypeName;
        FilePatterns = filePatterns;
        InputFilesList.ShowMoveButtons = showMoveButtons;
    }

    public string InputTitle { get; }

    public string FileDialogTitle { get; }

    public string FileTypeName { get; }

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
