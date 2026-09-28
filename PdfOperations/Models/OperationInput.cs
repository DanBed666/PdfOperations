namespace PdfOperations;

public class OperationInput
{
    public string[] InputFiles { get; set; } = [];
    public string PlaceholderFile { get; set; } = "";
    public List<PdfFragment> PdfFragments { get; set; } = new List<PdfFragment>();
    public string Dir { get; set; } = "";
    public string PhraseToFind { get; set; } = "";
    public string Pages { get; set; } = "";
    public string Format { get; set; } = "";
    public string Output { get; set; } = "";
    public int Before { get; set; } = 0;
    public int After { get; set; } = 0;
}