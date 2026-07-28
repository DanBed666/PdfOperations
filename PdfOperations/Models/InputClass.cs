namespace PdfOperations;

public class InputClass
{
    public string inputFile { get; set; }
    public string [] inputFiles { get; set; }
    public string outputFile { get; set; }
    public string dir { get; set; }
    public string tempDir { get; set; }
    public string phrase { get; set; }
    public string outputPath { get; set; }
    public int before { get; set; } = 0;
    public int after { get; set; } = 0;
}