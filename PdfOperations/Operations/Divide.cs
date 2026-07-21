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
    
    public static void ManyToOne(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..input, output]);
        RunClass.Run(tool, arguments);
    }
}