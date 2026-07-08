namespace PdfOperations;

public class Divide
{
    public static void OneToMany(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfSeparate];
        List<string> arguments = new List<string>();
        
        if (string.IsNullOrEmpty(output))
        {
            output = Files.SaveEmptyFile(input, ".pdf");
        }
        
        if (File.Exists(output))
        {
            output = Files.SaveExistingFile(output, ".pdf");    
        }
        
        arguments.AddRange([input, output]);
        RunClass.Run(tool, arguments);
    }
    
    public static void ManyToOne(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        List<string> arguments = new List<string>();
        
        if (string.IsNullOrEmpty(output))
        {
            output = Files.SaveEmptyFile(input[0], ".pdf");
        }
        
        if (File.Exists(output))
        {
            output = Files.SaveExistingFile(output, ".pdf");    
        }

        arguments.AddRange([..input, output]);
        RunClass.Run(tool, arguments);
    }
}