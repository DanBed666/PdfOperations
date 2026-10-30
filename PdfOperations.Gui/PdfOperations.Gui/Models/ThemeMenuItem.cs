namespace PdfOperations.Gui.Models;

public class ThemeMenuItem
{
    public AppTheme Theme { get; set; }

    public string DisplayName { get; set; } = "";

    public override string ToString()
    {
        return DisplayName;
    }
}