namespace PdfOperations;

public class Divide
{
    public static void OneToMany(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfSeparate];
        List<string> arguments = new List<string>();

        string name = Path.GetFileNameWithoutExtension(file.TempPath) + "_%d" + Path.GetExtension(file.TempPath);
        file.TempPath = Path.Combine(Path.GetDirectoryName(file.TempPath)!, name);
            
        arguments.AddRange([file.InputFile, file.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void ManyToOne(OperationInput input, FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..input.InputFiles, file.TempPath]);
        RunClass.Run(tool, arguments);
    }
}