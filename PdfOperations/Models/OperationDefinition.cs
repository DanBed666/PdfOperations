namespace PdfOperations;

public class OperationDefinition
{
    public string Name { get; init; } = "";
    public string Extension { get; set; } = "";
    public string Filter { get; init; } = "";
    public string AddInfo { get; init; } = "";
    public OperationFlow OperationFlow { get; set; }
    public Action<FileJob> FileOperationActionSingle { get; init; }
    public Action<FileJob> FileOperationActionMultiple { get; init; }
    public Action<OperationInput, FileJob> FileOperationActionPages { get; init; }
    public Action<OperationInput, OperationContext> FileOperationActionLibre { get; init; }
    public Action<OperationInput, OperationContext, FileJob> ReportOperationAction { get; init; }
    public Action<OperationDefinition> RunOperationAction { get; init; }
    public string InputPrompt { get; init; } = Messages.ChooseFiles;
    public string OutputPrompt { get; init; } = Messages.EnterOutputName;
    public string PagesPrompt { get; init; } = Messages.EnterPages;
    public string BeforePrompt { get; init; } = Messages.EnterBeforeLines;
    public string AfterPrompt { get; init; } = Messages.EnterAfterLines;
    public string PhrasePrompt { get; init; } = Messages.EnterPhrase;
}