namespace PdfOperations;

public class Files
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
        //ReadInput options
        
        string dir = "";
        
        if (opt.ToLower().Equals("t"))
            dir = Dialog.SelectDirectory();
        else
            Console.WriteLine("Dodano plik do folderu domyślnego");
        
        return dir;
    } 

    public static string [] ReadFile(string input)
    {
        return File.ReadAllLines(input);
    }

    public static void SaveToFile(List<List<string>> found, string output)
    {
        List<string> outputLines = new();
        
        foreach (List<String> lista in found)
        {
            outputLines.AddRange(lista);
        }
        
        File.WriteAllLines(output, outputLines);
    }
    
    public static void ViewFile(string path)
    {
        Console.WriteLine("Czy chcesz zrobić podgląd pliku (T/N)");
        string opt = ReadInput.ReadOption();
        //ReadInput options
        
        if (opt.ToLower().Equals("t"))
            RunClass.RunFile(path);
    }

    public static string GetUniqueFileName(string path, OperationDefinition operation)
    {
        int i = 1;
        string finalPath = path;

        while (File.Exists(finalPath))
        {
            finalPath = Path.Combine(Path.GetDirectoryName(path)!, $"{Path.GetFileNameWithoutExtension(path)}_{i++}{operation.Extension}");
        }

        return finalPath;
    }

    public static void PrepareTempPath(InputClass input, OperationDefinition operation)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        input.tempDir = tempDir;

        if (operation.OperationFlow == OperationFlow.FilesToFiles)
        {
            if (input.inputFiles.Length > 1)
            {
                foreach (string file in input.inputFiles)
                {
                    input.inputFile = file;
                    input.tempPath = Path.Combine(input.tempDir, Path.GetFileName(input.inputFile));
                    ExecuteOpe(input, operation);
                }
            }
            else
            {
                input.tempPath = Path.Combine(input.tempDir, Path.GetFileName(input.output));
                ExecuteOpe(input, operation);
            }
        }
        else
        {
            input.tempPath = Path.Combine(input.tempDir, Path.GetFileName(input.output));
            ExecuteOpe(input, operation);
        }
        
        Console.WriteLine("Trwa zapisywane...");
        PrepareFinalPath(input, operation);
    }
    
    public static void ExecuteOpe(InputClass input, OperationDefinition operation)
    {
        try
        {
            Console.WriteLine("Trwa konwersja...");
            
            operation.FileOperationAction(input);
            Console.WriteLine("Operacja zakończona pomyślnie!");
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
            Console.WriteLine("Szczegóły zapisano w logs/error_log.txt");
        }
    }

    public static void PrepareFinalPath(InputClass input, OperationDefinition operation)
    {
        foreach (string file in Directory.GetFiles(Path.GetDirectoryName(input.tempPath)!))
        {
            string finalPath = Path.Combine(input.dir, Path.GetFileName(file));

            if (File.Exists(finalPath))
                finalPath = GetUniqueFileName(finalPath, operation);

            File.Move(file, finalPath);
        }
    }
}