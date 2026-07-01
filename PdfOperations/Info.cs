namespace PdfOperations;

public class Info
{
    public static void ShowInfo(string input)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];
        RunClass.Run(tool, input);
    }
}