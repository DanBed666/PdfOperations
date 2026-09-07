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
    
    public static List<String> GetPdfPages(string [] inputs)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];
        string output = "";
        List<string> lines = new List<string>();

        foreach (string f in inputs)
        {
            output = RunClass.RunWithOutput(tool, f);
            string line = output.Split("\n").FirstOrDefault(x => x.StartsWith("Pages: "))!;
            int number = int.Parse(line.Split(":")[1].Trim());
            lines.Add($"Liczba stron plik {Path.GetFileName(f)}: {number}");
        }

        return lines;
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