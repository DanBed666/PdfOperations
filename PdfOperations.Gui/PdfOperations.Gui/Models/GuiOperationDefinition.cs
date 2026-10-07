using System;

namespace PdfOperations.Gui.Models;

public class GuiOperationDefinition
{
    public string Name { get; init; } = "";
    public string Title { get; init; } = "";
    public string InputTitle { get; init; } = "Input file";
    public string OutputExtension { get; init; } = "";
    public string DefaultOutputName { get; init; } = "output";
    public string FileDialogTitle { get; init; } = "Select input file";
    public string[] FilePatterns { get; init; } = [];
    public Action<OperationInput, OperationContext, FileJob>? Action { get; init; }
    public Action<FileJob>? SingleOutputAction { get; init; }
}