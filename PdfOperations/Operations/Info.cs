namespace PdfOperations;

public class Info
{
    public static void ShowInfo(string input)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];
        string output = RunClass.RunWithOutput(tool, input);
        Console.WriteLine(output);
    }
}