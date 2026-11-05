namespace PdfOperations;

public sealed record SearchHit(string FilePath, int Page, int Line, string Phrase, string Value);
public class SearchResult
{
    public string FilePath { get; set; } = "";
    public int PageNumber { get; set; } = 0;
    public int LineNumber { get; set; } = 0;
    public int Occurences { get; set; } = 0;
    public List<string> Lines { get; set; } = new();
    public List<SearchHit> Hits { get; } = [];
}