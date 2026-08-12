namespace PdfOperations;

public class Files8
{
    public static string [] AddFiles(string filter)
    {
        string [] files = Dialog.SelectFiles(filter);
        return files;
    }
    
    public static string AddDirectory()
    {
        Console.WriteLine("Czy chcesz dodać plik do folderu (T/N)");
        string opt = ReadInput.ReadOption();

        string dir = "";

        if (opt.ToLower().Equals("t"))
        {
            dir = Dialog.SelectDirectory();
        }
        else if (opt.ToLower().Equals("n"))
        {
            dir = GetDefaultDirectory();
            Console.WriteLine("Dodano plik do folderu domyślnego");
        }

        return dir;
    }

    public static string GetDefaultDirectory()
    {
        string defDir = Path.Combine(AppContext.BaseDirectory, "output");
        
        if (!Directory.Exists(defDir))
            Directory.CreateDirectory(defDir);

        return defDir;
    }

    public static string PrepareTempDir()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        return tempDir;
    }

    public static string PrepareTempPathMultiple(string tempDir, string file, string extension)
    {
        string tempPath = "";
        string name = Path.GetFileNameWithoutExtension(file) + extension;
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
    
    public static void SaveWithUniqueFileName(string extension, Dictionary <string, string> existing)
    {
        int i = 1;

        foreach (KeyValuePair<string, string> item in existing)
        {
            string finalPath = Path.Combine(Path.GetDirectoryName(item.Value)!, 
                $"{Path.GetFileNameWithoutExtension(item.Value)}_{i++}{extension}");
            
            File.Move(item.Key, finalPath);
        }
    }

    public static Dictionary <string, string> FindNotExisitingFiles(List<FileJob> fileJobs)
    {
        Dictionary <string, string> existing = new Dictionary<string, string>();

        foreach (FileJob fileJob in fileJobs)
        {
            if (File.Exists(fileJob.FinalPath))
            {
                existing.Add(fileJob.TempPath, fileJob.FinalPath);
            }
            else
            {
                File.Move(fileJob.TempPath, fileJob.FinalPath);
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
    
    public static void ViewFile(string path)
    {
        Console.WriteLine("Czy chcesz zrobić podgląd pliku (T/N)");
        string opt = ReadInput.ReadOption();
        //ReadInput options
        
        if (opt.ToLower().Equals("t"))
            RunClass.RunFile(path);
    }
    
    public static string [] ReadFile(string input)
    {
        return File.ReadAllLines(input);
    }
}