namespace PdfOperations;

public class OperationDefinition
{
    public string Name { get; init; } = "";
    public string Extension { get; init; } = "";
    public string Filter { get; init; } = "";
    public string Phrase { get; init; } = "";
    public Tool Tool { get; init; }
    public string [] Command { get; init; } = Array.Empty<string>();

    public Action<InputClass> FileOperationAction { get; init; }
}