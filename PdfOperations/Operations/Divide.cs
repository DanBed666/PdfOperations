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
    
    public static void ManyToOne(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..DecryptFiles(file), file.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static string [] DecryptFiles(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> decrypted = new List<string>();
        string tempDir = Path.Combine(Path.GetDirectoryName(file.TempPath)!, "decrypted");
        Directory.CreateDirectory(tempDir);
        
        foreach (string f in FileSorter.SortFilesByNumberAndName(file.InputFiles))
        {
            List<string> arguments = new List<string>();
            
            string unlocked = Path.Combine(tempDir, 
                Path.GetFileNameWithoutExtension(f) + "_odblokowany" + Path.GetExtension(f));
            
            arguments.AddRange(["--decrypt", f, unlocked]);
            RunClass.Run(tool, arguments);
            decrypted.Add(unlocked);
        }
        
        return decrypted.ToArray();
    }
}