namespace PdfOperations;

public class Divide
{
    public static void OneToMany(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfSeparate];
        
        if (string.IsNullOrEmpty(output))
        {
            output = Files.SaveEmptyFile(input, ".pdf");
        }
        
        if (File.Exists(output))
        {
            output = Files.SaveExistingFile(output, ".pdf");    
        }
        
        RunClass.Run(tool, input, output);
    }
    
    public static void ManyToOne(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        
        if (string.IsNullOrEmpty(output))
        {
            output = Files.SaveEmptyFile(input[0], ".pdf");
        }
        
        if (File.Exists(output))
        {
            output = Files.SaveExistingFile(output, ".pdf");    
        }

        string[] arguments = [..input, output];
        
        RunClass.Run(tool, arguments);
    }
}