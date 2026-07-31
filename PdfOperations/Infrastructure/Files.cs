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

    public static string GetUniqueFileName(string path, string extension)
    {
        int i = 1;
        string finalPath = path;

        while (File.Exists(finalPath))
        {
            finalPath = Path.Combine(Path.GetDirectoryName(path)!, $"{Path.GetFileNameWithoutExtension(path)}_{i++}{extension}");
        }

        return finalPath;
    }

    public static void FilesToFilesFlow(InputClass input, OperationDefinition operation)
    {
        if (input.inputFiles.Length > 1)
        {
            foreach (string file in input.inputFiles)
            {
                input.inputFile = file;
                string name = Path.GetFileNameWithoutExtension(input.inputFile) + input.extension;
                input.tempPath = Path.Combine(input.tempDir, name);
                ExecuteOpe(input, operation);
            }
        }
        else
        {
            input.tempPath = Path.Combine(input.tempDir, Path.GetFileName(input.output));
            ExecuteOpe(input, operation);
        }
    }
    
    public static void FilesToSingleFlow(InputClass input, OperationDefinition operation)
    {
        string name = Path.GetFileNameWithoutExtension(input.inputFile) + input.extension;
        input.tempPath = Path.Combine(input.tempDir, name);
        ExecuteOpe(input, operation);
    }

    public static void PrepareTempPath(InputClass input, OperationDefinition operation)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        input.tempDir = tempDir;

        if (operation.OperationFlow == OperationFlow.FilesToFiles)
        {
            FilesToFilesFlow(input, operation);
        }
        else if (operation.OperationFlow == OperationFlow.SearchReport)
        {
            input.move = false;
            operation.OperationFlow = OperationFlow.FilesToFiles;
            FilesToFilesFlow(input, operation);
            
            operation.OperationFlow = OperationFlow.SearchReport;
            ExecuteOpe(input, operation);
        }
        else
        {
            FilesToSingleFlow(input, operation);
        }
        
        Console.WriteLine("Trwa zapisywane...");

        foreach (string file in Directory.GetFiles(Path.GetDirectoryName(input.tempPath)!))
        {
            string finalPath = PrepareFinalPath(input, file);
            
            if (input.move)
                File.Move(file, finalPath);
        }

        //Directory.Delete(input.tempDir);
    }
    
    public static void ExecuteOpe(InputClass input, OperationDefinition operation)
    {
        try
        {
            Console.WriteLine("Trwa konwersja...");

            if (operation.OperationFlow == OperationFlow.SearchReport)
            {
                operation.ReportOperationAction(input);
            }
            else
            {
                operation.FileOperationAction(input);
            }

            Console.WriteLine("Operacja zakończona pomyślnie!");
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
            Console.WriteLine("Szczegóły zapisano w logs/error_log.txt");
        }
    }

    public static string PrepareFinalPath(InputClass input, string file = "")
    {
        string finalPath = "";
        
        if (string.IsNullOrEmpty(input.output))
            finalPath = Path.Combine(input.dir, Path.GetFileName(file));
        else
            finalPath = Path.Combine(input.dir, input.output);

        if (File.Exists(finalPath))
            finalPath = GetUniqueFileName(finalPath, input.extension);

        return finalPath;
    }
}