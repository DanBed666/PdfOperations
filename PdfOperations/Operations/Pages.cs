namespace PdfOperations;

public class Pages
{
    public static void CreateWithPages(OperationInput input, List<FileJob> files)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();

        foreach (FileJob file in files)
        {
            arguments.AddRange([file.InputFile, "--pages", ".", input.Pages, "--", file.TempPath]);
            RunClass.Run(tool, arguments);
        }
    }
}