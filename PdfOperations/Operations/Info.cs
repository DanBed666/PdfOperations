using System.Text;

namespace PdfOperations;

public class Info
{
    public static void ShowInfo(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];
        string output = "";

        foreach (string f in file.InputFiles)
        {
            output = RunClass.RunWithOutput(tool, f);
        }
        
        SaveToFile(file.TempPath, output, file.InputFiles);
    }

    public static int GetPdfPagesSingle(string input)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];
        string output = "";
        int number = 0;

        output = RunClass.RunWithOutput(tool, input);
        string line = output.Split("\n").FirstOrDefault(x => x.StartsWith("Pages: "))!;
        number = int.Parse(line.Split(":")[1].Trim());

        return number;
    }
    
    public static void ShowFontInfo(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfFonts];
        string output = "";

        foreach (string f in file.InputFiles)
        {
            output = RunClass.RunWithOutput(tool, f);
        }
        
        SaveToFile(file.TempPath, output, file.InputFiles);
        
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