namespace PdfOperations;

public class Dialog
{
    public static string SelectFile()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Wybierz plik",
            Filter = "Pliki PDF (*.pdf)|*.pdf|Wszystkie pliki (*.*)|*.*",
            AutoUpgradeEnabled = true,
            RestoreDirectory = true,
            Multiselect = true
        };
        
        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : "";
    }
    
    public static string [] SelectFiles()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Wybierz pliki",
            Filter = "Pliki PDF (*.pdf)|*.pdf|Wszystkie pliki (*.*)|*.*",
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