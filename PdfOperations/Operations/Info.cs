namespace PdfOperations;

public class Info
{
    public static void ShowInfo(string input)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfInfo];
        string output = RunClass.RunWithOutput(tool, input);
        Console.WriteLine(output);
    }
    
    public static void ShowFontInfo(string input)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfFonts];
        string output = RunClass.RunWithOutput(tool, input);
        Console.WriteLine(output);
    }
    
    public static void ExtractPict(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfImages];
        List<string> arguments = new List<string>();
        
        arguments.AddRange(["-all", input, output]);
        RunClass.Run(tool, arguments);
    }

    public static void ExtractPicts(string [] input, string outputDir)
    {
        int i = 0;
        
        foreach (string file in input)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string newOutputName = Path.Combine(outputDir, $"{i + 1}_{name}");
            
            ExtractPict(file, newOutputName);
            i++;
        }
    }
}