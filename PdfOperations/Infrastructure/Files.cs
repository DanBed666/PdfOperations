namespace PdfOperations;

public class Files
{
    public static string PrepareTempDir()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        return tempDir;
    }

    public static string PrepareTempPathMultiple(string tempDir, string file, string extension)
    {
        string tempPath = "";
        string name = Path.GetFileNameWithoutExtension(file) + CheckParams.NormalizeExtension(extension);
        tempPath = Path.Combine(tempDir, name);

        return tempPath;
    }

    public static string PrepareTempPathSingle(string tempDir, string output)
    {
        string tempPath = Path.Combine(tempDir, output);
        
        return tempPath;
    }
    
    public static string PrepareFinalOutputPath(string outputDir, string tempPath)
    {
        string finalPath = "";
        finalPath = Path.Combine(outputDir, Path.GetFileName(tempPath));

        return finalPath;
    }
    
    public static void SaveWithUniqueFileName(Dictionary <string, string> existing)
    {
        foreach (KeyValuePair<string, string> item in existing)
        {
            int i = 1;
            string finalPath;
            string directory = Path.GetDirectoryName(item.Value)!;
            string fileName = Path.GetFileNameWithoutExtension(item.Value);
            string extension = Path.GetExtension(item.Value);

            do
            {
                finalPath = Path.Combine(directory, $"{fileName}_{i}{extension}");
                i++;
            } 
            while (File.Exists(finalPath));
            
            File.Move(item.Key, finalPath);
        }
    }
    
    public static Dictionary <string, string> MoveNewFilesAndReturnConflicts(string finalDir, string tempDir)
    {
        Dictionary <string, string> existing = new Dictionary<string, string>();

        foreach (string tempFile in Directory.GetFiles(tempDir))
        {
            string finalPath = Path.Combine(finalDir, Path.GetFileName(tempFile));
            
            if (File.Exists(finalPath))
            {
                existing.Add(tempFile, finalPath);
            }
            else
            {
                File.Move(tempFile, finalPath);
            }
        }

        return existing;
    }
    
    public static Dictionary <string, string> MoveNewFilesAndReturnConflictsWithExcept(string finalDir, string tempDir, 
        OperationInput input)
    {
        Dictionary <string, string> existing = new Dictionary<string, string>();

        string [] tempDirFiles = Directory.GetFiles(tempDir);
        
        if (tempDirFiles.Length == 1)
        {
            Console.WriteLine("plik: " + tempDirFiles[0]);
            string output = "";

            output = Path.Combine(Path.GetDirectoryName(tempDirFiles[0])!, input.Output);
            Console.WriteLine("final = " + output);
            File.Move(tempDirFiles[0], output, true);
        }
        
        string [] tempDirNewFiles = Directory.GetFiles(tempDir);

        foreach (string file in tempDirNewFiles)
        {
            if (input.ExceptFormat != null)
            {
                if (Path.GetExtension(file) == input.ExceptFormat)
                    continue;
            }
            
            Console.WriteLine("plik8: " + file);
            string finalPath = Path.Combine(finalDir, Path.GetFileName(file));
            
            if (File.Exists(finalPath))
            {
                existing.Add(file, finalPath);
            }
            else
            {
                File.Move(file, finalPath);
            }
        }

        return existing;
    }

    public static void OverWriteFile(Dictionary <string, string> existing)
    {
        foreach (KeyValuePair<string, string> item in existing)
        {
            File.Move(item.Key, item.Value, overwrite: true);
        }
    }

    public static void SaveToFile(List<List<string>> found, string output)
    {
        List<string> outputLines = new List<string>();
        
        foreach (List<String> lista in found)
        {
            outputLines.AddRange(lista);
        }
        
        if (!File.Exists(output))
            File.WriteAllLines(output, outputLines);
        else
            File.AppendAllLines(output, outputLines);
    }
    
    public static void OpenPath(string path, string type)
    {
        if (type == "file")
            Console.WriteLine(Messages.PreviewFileQuestion);
        
        if (type == "dir")
            Console.WriteLine(Messages.PreviewFolderQuestion);
        
        string opt = ReadInput.ReadOption();
        
        if (opt.ToLower().Equals("t"))
            RunClass.RunFile(path);
    }
    
    public static string [] ReadFile(string input)
    {
        return File.ReadAllLines(input);
    }

    public static string FindOriginalFileForTemp(string tempFile, string [] inputFiles)
    {
        string tempName = Path.GetFileNameWithoutExtension(tempFile);

        string? originalFile = inputFiles.FirstOrDefault(fileName =>
            Path.GetFileNameWithoutExtension(fileName).Equals(tempName, StringComparison.OrdinalIgnoreCase));

        return originalFile ?? tempName;
    }
}