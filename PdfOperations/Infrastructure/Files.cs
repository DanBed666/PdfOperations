namespace PdfOperations;

public class Files
{
    public static string PrepareTempDir()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        return tempDir;
    }
    
    public static string PrepareTempPath(string tempDir, string fileOut, string extension)
    {
        return Path.Combine(tempDir, Path.GetFileNameWithoutExtension(fileOut) + extension);
    }
    
    public static string PrepareTempPathWithoutExt(string tempFile)
    {
        return Path.Combine(Path.GetDirectoryName(tempFile)!, Path.GetFileNameWithoutExtension(tempFile));
    }
    
    public static string PrepareFinalPath(string finalDir, string fileOut)
    {
        return Path.Combine(finalDir, Path.GetFileName(fileOut));
    }
    
    public static void SaveToFile(SearchResult found, string output)
    {
        List<string> outputLines = new List<string>();
        
        foreach (string lista in found.Lines)
        {
            outputLines.AddRange(lista);
        }
        
        if (!File.Exists(output))
            File.WriteAllLines(output, outputLines);
        else
            File.AppendAllLines(output, outputLines);
    }
}