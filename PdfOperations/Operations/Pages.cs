namespace PdfOperations;

public class Pages
{
    public static void CreateWithPages(string input, string pages, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([input, "--pages", ".", pages, "--", output]);
        RunClass.Run(tool, arguments);
    }
}