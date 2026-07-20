namespace PdfOperations;

public class Divide
{
    public static void OneToMany(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfSeparate];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([input, output]);
        RunClass.Run(tool, arguments);
    }
    
    public static void OnesToMany(string [] input, string outputDir)
    {
        
      
        foreach (string file in input)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string newOutputName = Path.Combine(outputDir, $"%d_{name}.pdf");
            
            OneToMany(file, newOutputName);
        }
    }
    
    public static void ManyToOne(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..input, output]);
        RunClass.Run(tool, arguments);
    }
}