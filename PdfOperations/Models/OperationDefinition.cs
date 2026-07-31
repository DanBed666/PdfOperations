namespace PdfOperations;

public class OperationDefinition
{
    public string Name { get; init; } = "";
    public string Extension { get; init; } = "";
    public string Filter { get; init; } = "";
    public string Phrase { get; init; } = "";
    public OperationFlow OperationFlow { get; set; }
    public bool SingleFile { get; set; } = false;

    public Action<InputClass> FileOperationAction { get; init; }
    
    public Action<InputClass> ReportOperationAction { get; init; }
    public Action<OperationDefinition> RunOperationAction { get; init; }
}