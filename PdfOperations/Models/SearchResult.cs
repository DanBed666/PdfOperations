namespace PdfOperations;

public class SearchResult
{
    public string FilePath { get; set; } = "";
    public int PageNumber { get; set; } = 0;
    public int LineNumber { get; set; } = 0;
    public int Occurences { get; set; } = 0;
    public List<string> Lines { get; set; } = new();
}