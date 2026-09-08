namespace PdfOperations;

public class Replacement
{
    public static void ReplaceTextWithPlaceholders(OperationInput input, FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.InputFile, "--pages", ".", input.Pages, "--", file.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void ReplacePlaceholdersWithText(OperationInput input, FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.InputFile, "--pages", ".", input.Pages, "--", file.TempPath]);
        RunClass.Run(tool, arguments);
    }
}