namespace PdfOperations;

public class ReplacementPair
{
    public int Lp { get; set; } = 0;
    public string Find { get; set; } = "";
    public string Replace { get; set; } = "";
    public bool IgnoreCase { get; set; } = false;
}