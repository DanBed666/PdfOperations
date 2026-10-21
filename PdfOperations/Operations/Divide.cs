namespace PdfOperations;

public class Divide
{
    public static void OneToMany(FileJob file)
    {
        OneToMany(file, CancellationToken.None);
    }

    public static void OneToMany(FileJob file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.PdfSeparate];
        List<string> arguments = new List<string>();

        string name = Path.GetFileNameWithoutExtension(file.TempPath) + "_%d" + Path.GetExtension(file.TempPath);
        file.TempPath = Path.Combine(Path.GetDirectoryName(file.TempPath)!, name);
            
        arguments.AddRange([file.InputFile, file.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.SplitPdf, cancellationToken, 0, 3);
    }
    
    public static void ManyToOne(FileJob file)
    {
        ManyToOne(file, CancellationToken.None);
    }

    public static void ManyToOne(FileJob file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.PdfUnite];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..DecryptFiles(file, cancellationToken), file.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.MergePdf, cancellationToken);
    }
    
    public static string [] DecryptFiles(FileJob file)
    {
        return DecryptFiles(file, CancellationToken.None);
    }

    public static string[] DecryptFiles(FileJob file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> decrypted = new List<string>();
        string tempDir = Path.Combine(Path.GetDirectoryName(file.TempPath)!, "decrypted");
        Directory.CreateDirectory(tempDir);
        
        foreach (string f in file.InputFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<string> arguments = new List<string>();
            
            string unlocked = Path.Combine(tempDir, 
                Path.GetFileNameWithoutExtension(f) + "_odblokowany" + Path.GetExtension(f));
            
            arguments.AddRange(["--decrypt", f, unlocked]);
            RunClass.Run(tool, arguments, OperationTimeouts.DecryptPdf, cancellationToken, 0, 3);
            decrypted.Add(unlocked);
        }
        
        return decrypted.ToArray();
    }
}
