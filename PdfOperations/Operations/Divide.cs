namespace PdfOperations;

public class Divide
{
    public static void OneToMany(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfSeparate];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([file.inputFile, file.tempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void ManyToOne(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..file.inputFiles, file.tempPath]);
        RunClass.Run(tool, arguments);
    }
}