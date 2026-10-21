using System;

namespace PdfOperations.Gui.Models;

using System.Threading;

public class GuiOperationDefinition
{
    public string Name { get; init; } = "";
    public string Title { get; init; } = "";
    public string InputTitle { get; init; } = "Input file";
    public string OutputExtension { get; init; } = "";
    public string DefaultOutputName { get; init; } = "output";
    public string FileDialogTitle { get; init; } = "Select input file";
    public string FileTypeName { get; init; } = "File input";
    public string[] FilePatterns { get; init; } = [];
    public Action<OperationInput, OperationContext, FileJob, CancellationToken>? Action { get; init; }
    public Action<FileJob, CancellationToken>? SingleOutputAction { get; init; }
    public bool MoveAllTempFiles { get; init; } = false;
}
