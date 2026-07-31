namespace PdfOperations;

public class InputClass
{
    public string inputFile { get; set; }
    public string [] inputFiles { get; set; }
    public string dir { get; set; }
    public string tempDir { get; set; }
    public string phrase { get; set; }
    public string pages { get; set; }
    public string format { get; set; }
    public string tempPath { get; set; }
    public string reportPath { get; set; }
    public string output { get; set; }
    public int before { get; set; }
    public int after { get; set; }
    public string extension { get; set; }
    public bool move { get; set; }
}