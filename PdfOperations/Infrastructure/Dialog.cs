namespace PdfOperations;

public class Dialog
{
    public static string [] SelectFiles(string filter)
    {
        using var dialog = new OpenFileDialog
        {
            Title = Messages.ChooseFiles,
            Filter = filter,
            AutoUpgradeEnabled = true,
            RestoreDirectory = true,
            Multiselect = true
        };
        
        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames : [];
    }
    
    public static string SelectDirectory()
    {
        using var dialog = new FolderBrowserDialog()
        {
            Description = Messages.ChooseDirectory,
            ShowNewFolderButton = true,
        };
        
        return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : "";
    }
}