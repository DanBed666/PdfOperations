namespace PdfOperations;

public class Pages
{
    public static void CreateWithPages(OperationInput input, FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.InputFile, "--pages", ".", input.Pages, "--", file.TempPath]);
        RunClass.Run(tool, arguments);
    }
}