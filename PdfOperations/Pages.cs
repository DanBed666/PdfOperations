namespace PdfOperations;

public class Pages
{
    public static void CreateWithPages(string input, string pages, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        
        if (string.IsNullOrEmpty(output))
        {
            string fileName = Path.GetFileNameWithoutExtension(input);
            output = $"{fileName}.pdf";
        }
        
        RunClass.Run(tool, input, "--pages", ".", pages, "--", output);
    }
}