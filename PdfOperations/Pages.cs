namespace PdfOperations;

public class Pages
{
    public static void CreateWithPages(string input, string pages, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        RunClass.Run(tool, input, "--pages", ".", pages, "--", output);
    }
}