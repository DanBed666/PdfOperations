namespace PdfOperations;

public class Divide
{
    public static void OneToMany(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfSeparate];
        
        if (string.IsNullOrEmpty(output))
            output = "defautl8.pdf";
        
        RunClass.Run(tool, input, output);
    }
    
    public static void ManyToOne(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        
        if (string.IsNullOrEmpty(output))
            output = "defautl9.pdf";

        string[] arguments = [..input, output];
        
        RunClass.Run(tool, arguments);
    }
}