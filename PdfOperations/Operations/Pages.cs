namespace PdfOperations;

public class Pages
{
    public static void CreateWithPages(OperationInput input, FileJob file)
    {
        CreateWithPages(input, file, CancellationToken.None);
    }

    public static void CreateWithPages(OperationInput input, FileJob file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.InputFile, "--pages", ".", input.Pages, "--", file.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.SelectPages, cancellationToken, [0, 3]);
    }
    
    public static void CreateWithCustomFiles(OperationInput input, FileJob file)
    {
        CreateWithCustomFiles(input, file, CancellationToken.None);
    }

    public static void CreateWithCustomFiles(OperationInput input, FileJob file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();
        
        arguments.AddRange(["--empty", "--pages"]);

        foreach (PdfFragment fragment in input.PdfFragments)
        {
            cancellationToken.ThrowIfCancellationRequested();
            arguments.AddRange([fragment.FileName!, fragment.PageNumbers]);
        }

        arguments.AddRange(["--", file.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.BuildFragments, cancellationToken, [0, 3]);
    }

    public static void SplitPages(FileJob file, List<int> splitAfterPages, int pageCount)
    {
        SplitPages(file, splitAfterPages, pageCount, CancellationToken.None);
    }

    public static void SplitPages(FileJob file, List<int> splitAfterPages, int pageCount, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];

        int startPage = 1;
        int partNumber = 1;

        foreach (int splitAfterPage in splitAfterPages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string outputPath = PrepareSplitOutputPath(file.TempPath, partNumber);
            string pages = $"{startPage}-{splitAfterPage}";

            RunClass.Run(tool, [file.InputFile, "--pages", ".", pages, "--", outputPath],
                OperationTimeouts.SplitPdf, cancellationToken, [0, 3]);

            startPage = splitAfterPage + 1;
            partNumber++;
        }

        string lastOutputPath = PrepareSplitOutputPath(file.TempPath, partNumber);
        string lastPages = $"{startPage}-{pageCount}";

        RunClass.Run(tool, [file.InputFile, "--pages", ".", lastPages, "--", lastOutputPath],
            OperationTimeouts.SplitPdf, cancellationToken, [0, 3]);
    }

    public static string PrepareSplitOutputPath(string tempPath, int partNumber)
    {
        string dir = Path.GetDirectoryName(tempPath)!;
        string name = Path.GetFileNameWithoutExtension(tempPath);

        return Path.Combine(dir, $"{name}_part_{partNumber}.pdf");
    }
}
