namespace PdfOperations.Gui.ViewModels;

public abstract class FileInputViewModelBase : ViewModelBase
{
    protected FileInputViewModelBase(
        string fileDialogTitle,
        string[] filePatterns,
        bool showMoveButtons = false)
    {
        FileDialogTitle = fileDialogTitle;
        FilePatterns = filePatterns;
        InputFilesList.ShowMoveButtons = showMoveButtons;
    }

    public string FileDialogTitle { get; }

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