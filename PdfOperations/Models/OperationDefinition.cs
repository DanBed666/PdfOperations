namespace PdfOperations;

public class OperationDefinition
{
    public string Name { get; init; } = "";
    public string Extension { get; init; } = "";
    public string Filter { get; init; } = "";
    public OperationFlow Flow { get; init; }
    public string Message { get; init; } = "";
}