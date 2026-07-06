namespace PdfOperations;

public class Divide
{
    public static void OneToMany(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfSeparate];
        
        if (string.IsNullOrEmpty(output))
        {
            string fileName = Path.GetFileNameWithoutExtension(input);
            output = $"{fileName}.pdf";
        }
        
        RunClass.Run(tool, input, output);
    }
    
    public static void ManyToOne(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        
        if (string.IsNullOrEmpty(output))
        {
            string fileName = Path.GetFileNameWithoutExtension(input[0]);
            output = $"{fileName}.pdf";
        }

        string[] arguments = [..input, output];
        
        RunClass.Run(tool, arguments);
    }
}