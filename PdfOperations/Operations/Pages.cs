namespace PdfOperations;

public class Pages
{
    public static void CreateWithPages(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([file.inputFile, "--pages", ".", file.phrase, "--", file.outputPath]);
        RunClass.Run(tool, arguments);
    }
}