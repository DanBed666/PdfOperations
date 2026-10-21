using System.Text;

namespace PdfOperations;

public class Info
{
    public static void ShowInfo(FileJob file)
    {
        ShowInfo(file, CancellationToken.None);
    }

    public static void ShowInfo(FileJob file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];
        string output = "";

        foreach (string f in file.InputFiles)
        {
            output = RunClass.RunWithOutput(tool, OperationTimeouts.PdfInfo, cancellationToken, f);
            cancellationToken.ThrowIfCancellationRequested();
            SaveToFile(file.TempPath, output, f);
        }
    }

    public static int GetPdfPagesSingle(string input)
    {
        return GetPdfPagesSingle(input, CancellationToken.None);
    }

    public static int GetPdfPagesSingle(string input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];
        string output = "";
        int number = 0;

        output = RunClass.RunWithOutput(tool, OperationTimeouts.PdfPageCount, cancellationToken, input);
        string line = output.Split("\n").FirstOrDefault(x => x.StartsWith("Pages: "))!;
        number = int.Parse(line.Split(":")[1].Trim());

        return number;
    }
    
    public static void ShowFontInfo(FileJob file)
    {
        ShowFontInfo(file, CancellationToken.None);
    }

    public static void ShowFontInfo(FileJob file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.PdfFonts];
        string output = "";

        foreach (string f in file.InputFiles)
        {
            output = RunClass.RunWithOutput(tool, OperationTimeouts.PdfFontInfo, cancellationToken, f);
            cancellationToken.ThrowIfCancellationRequested();
            SaveToFile(file.TempPath, output, f);
        }
    }
    
    public static void SaveToFile(string file, string output, string inputFile)
    {
        if (!File.Exists(file))
        {
            File.WriteAllText(file, inputFile, new UTF8Encoding(true));
            File.AppendAllText(file, "\n\n");
            File.AppendAllText(file, output);
            File.AppendAllText(file, "-------------------------");
            File.AppendAllText(file, "\n\n");
        }
        else
        {
            File.AppendAllText(file, inputFile, new UTF8Encoding(true));
            File.AppendAllText(file, "\n\n");
            File.AppendAllText(file, output);
            File.AppendAllText(file, "-------------------------");
            File.AppendAllText(file, "\n\n");
        }
    }
}
