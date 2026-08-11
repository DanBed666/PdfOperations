namespace PdfOperations;

public class FileJob
{
    public string InputFile { get; set; } = "";
    public string [] InputFiles { get; set; } = [];
    public string TempPath { get; set; } = "";
    public string FinalPath { get; set; } = "";
}