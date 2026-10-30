namespace PdfOperations;

public class FileExtractionResult
{
    public string FilePath { get; set; } = "";

    public string FileName => Path.GetFileName(FilePath);

    public Dictionary<string, string[]> Fields { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}