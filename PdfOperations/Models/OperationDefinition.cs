namespace PdfOperations;

public class OperationDefinition
{
    public string Name { get; init; } = "";
    public string Extension { get; set; } = "";
    public string Filter { get; init; } = "";
    public string AddInfo { get; init; } = "";
    public OperationFlow OperationFlow { get; set; }
    public bool SingleFile { get; set; } = false;

    public Action<FileJob> FileOperationActionSingle { get; init; }
    public Action<FileJob> FileOperationActionMultiple { get; init; }
    public Action<OperationInput, FileJob> FileOperationActionPages { get; init; }
    public Action<OperationInput, OperationContext> FileOperationActionLibre { get; init; }
    
    public Action<OperationInput, OperationContext, FileJob> ReportOperationAction { get; init; }
    public Action<OperationDefinition> RunOperationAction { get; init; }
}