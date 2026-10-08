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
    
    public static void CreateWithCustomFiles(OperationInput input, FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();
        
        arguments.AddRange(["--empty", "--pages"]);

        foreach (PdfFragment fragment in input.PdfFragments)
        {
            arguments.AddRange([fragment.FileName!, fragment.PageNumbers]);
        }

        arguments.AddRange(["--", file.TempPath]);
        RunClass.Run(tool, arguments);
    }

    public static void SplitPages(FileJob file, List<int> splitAfterPages, int pageCount)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];

        int startPage = 1;
        int partNumber = 1;

        foreach (int splitAfterPage in splitAfterPages)
        {
            string outputPath = PrepareSplitOutputPath(file.TempPath, partNumber);
            string pages = $"{startPage}-{splitAfterPage}";
            
            RunClass.Run(tool, [file.InputFile, "--pages", ".", pages, "--", outputPath]);

            startPage = splitAfterPage + 1;
            partNumber++;
        }

        string lastOutputPath = PrepareSplitOutputPath(file.TempPath, partNumber);
        string lastPages = $"{startPage}-{pageCount}";
        RunClass.Run(tool, [file.InputFile, "--pages", ".", lastPages, "--", lastOutputPath]);
    }

    public static string PrepareSplitOutputPath(string tempPath, int partNumber)
    {
        string dir = Path.GetDirectoryName(tempPath)!;
        string name = Path.GetFileNameWithoutExtension(tempPath);

        return Path.Combine(dir, $"{name}_part_{partNumber}.pdf");
    }
}