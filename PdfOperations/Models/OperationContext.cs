namespace PdfOperations;

public class OperationContext
{
    public string TempDir { get; set; } = "";
    public bool Move { get; set; } = true;
    public bool OverWrite { get; set; } = false;
}