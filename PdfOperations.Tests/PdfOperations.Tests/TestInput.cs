namespace PdfOperations.Tests;

public class TestInput
{
    public string[] InputFiles { get; set; } = [];
    public OperationInput Input { get; set; } = new();
    public OperationContext Context { get; set; } = new();
    public OperationDefinition Operation { get; set; } = new();
}