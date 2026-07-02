namespace PdfOperations;

public class Dialog
{
    public static string SelectFile(string filter)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Wybierz plik",
            Filter = filter,
            AutoUpgradeEnabled = true,
            RestoreDirectory = true,
            Multiselect = true
        };
        
        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : "";
    }
    
    public static string [] SelectFiles(string filter)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Wybierz pliki",
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
            Description = "Wybierz folder",
            ShowNewFolderButton = true,
        };
        
        return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : "";
    }
    
    public static string [] SelectDirectories()
    {
        using var dialog = new FolderBrowserDialog()
        {
            Description = "Wybierz foldery",
            ShowNewFolderButton = true,
        };
        
        return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPaths : [];
    }
}