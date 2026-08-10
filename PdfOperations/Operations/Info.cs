using System.Text;

namespace PdfOperations;

public class Info
{
    public static void ShowInfo(OperationInput fileInput, FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];

        string output = RunClass.RunWithOutput(tool, file.InputFile);
        SaveToFile(file.TempPath, output, fileInput.InputFiles);
        
    }
    
    public static void ShowFontInfo(OperationInput fileInput, FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfFonts];

        string output = RunClass.RunWithOutput(tool, file.InputFile);
        SaveToFile(file.TempPath, output, fileInput.InputFiles);
        
    }
    
    public static void SaveToFile(string file, string output, string [] inputFiles)
    {
        foreach (string f in inputFiles)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, f, new UTF8Encoding(true));
                File.AppendAllText(file, "\n\n");
                File.AppendAllText(file, output);
                File.AppendAllText(file, "-------------------------");
                File.AppendAllText(file, "\n\n");
            }
            else
            {
                File.AppendAllText(file, f, new UTF8Encoding(true));
                File.AppendAllText(file, "\n\n");
                File.AppendAllText(file, output);
                File.AppendAllText(file, "-------------------------");
                File.AppendAllText(file, "\n\n");
            }
        }

        Console.WriteLine(File.Exists(file));
    }
}