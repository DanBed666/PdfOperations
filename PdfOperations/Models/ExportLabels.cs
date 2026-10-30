namespace PdfOperations;

public sealed record ExportLabels
{
    public string WorksheetName { get; init; } = "Results";
    public string FileNameHeader { get; init; } = "File name";
    public string FilePathHeader { get; init; } = "File path";
    public string ValueHeader { get; init; } = "Value";
    public string FileNameProperty { get; init; } = "FileName";
    public string FilePathProperty { get; init; } = "FilePath";
    public string ValueProperty { get; init; } = "DesiredValue";
    public string FieldsProperty { get; init; } = "Fields";
}
