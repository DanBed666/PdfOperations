namespace PdfOperations;

public class Files8
{
    public static string PrepareTempDir()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        return tempDir;
    }
    
    public static string PrepareTempPath(string tempDir, string fileOut, string extension)
    {
        string tempPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(fileOut) + extension);
        
        return tempPath;
    }
    
    public static string PrepareFinalPath(string finalDir, string fileOut)
    {
        string finalPath = Path.Combine(finalDir, Path.GetFileName(fileOut));
        
        return finalPath;
    }
}