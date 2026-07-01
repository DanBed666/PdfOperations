namespace PdfOperations;

public class Dialog
{
    public static string SelectFile()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Nowe okno",
            Filter = "Pliki PDF (*.pdf)|*.pdf|Wszystkie pliki (*.*)|*.*",
            Multiselect = true
        };
        
        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
    }
}