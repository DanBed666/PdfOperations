namespace PdfOperations;

public class OperationDefinition
{
    public string Name { get; init; } = "";
    public string Extension { get; init; } = "";
    public string Filter { get; init; } = "";
    
    public Action<InputClass, OperationDefinition> MultipleFilesAction { get; init; }
    public Action<string, string> FileOperationAction { get; init; }
}