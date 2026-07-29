using System.Text;

namespace PdfOperations;

public class Info
{
    public static void ShowInfo(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];

        foreach (string f in file.inputFiles)
        {
            string output = RunClass.RunWithOutput(tool, f);
            SaveToFile(file.tempPath, output, f);
        }
    }
    
    public static void ShowFontInfo(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfFonts];
        
        foreach (string f in file.inputFiles)
        {
            string output = RunClass.RunWithOutput(tool, f);
            SaveToFile(file.tempPath, output, f);
        }
    }
    
    public static void SaveToFile(string file, string output, string f)
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

        Console.WriteLine(File.Exists(file));
    }
}