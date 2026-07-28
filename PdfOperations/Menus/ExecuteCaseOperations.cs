namespace PdfOperations;

public class ExecuteCaseOperations
{
    public static void InputOpe(OperationDefinition operation)
    {
        string output = "";
        string phrase = "";
        int before = 0;
        int after = 0;
        
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(operation.Filter);

        if (input.Length == 0) return;

        foreach (string file in input)
        {
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
        }
        
        if (input.Length == 1)
            Files.ViewFile(input[0]);

        if (operation.Phrase == "search")
        {
            if (!InputSearchOpe(out string value)) return;
            phrase = value;
            
            Console.WriteLine("Linie przed: ");
            Int32.TryParse(Console.ReadLine(), out before);
            before = -before;
            
            Console.WriteLine("Linie po: ");
            Int32.TryParse(Console.ReadLine(), out after);
            after = after;
        }
        
        if (operation.Phrase == "format")
        {
            if (!InputFormatOpe(out string value)) return;
            phrase = value;
        }
        
        if (operation.Phrase == "pages")
        {
            if (!InputPagesOpe(out string value)) return;
            phrase = value;
        }
        
        if (input.Length == 1 || operation.OperationFlow == OperationFlow.FilesToSingleFile)
        {
            Console.WriteLine("Podaj output: ");
            output = Console.ReadLine()!;

            if (!CheckParams.CheckFileFormat(output))
                return;
        }
        
        string dir = Files.AddDirectory();

        if (operation.OperationFlow == OperationFlow.FilesToFiles)
        {
            if (input.Length > 1)
            {
                foreach (string file in input)
                {
                    InputClass inputFile = PrepareInput(file, output, dir, phrase, before, after);
                    PreparePath(inputFile, operation);
                }
            }
            else
            {
                InputClass inputFile = PrepareInput(input[0], output, dir, phrase, before, after);
                PreparePathSingle(inputFile, operation);
            }
        }
        else
        {
            InputClass inputFileAlt = PrepareInput(input, output, dir, phrase);
            
            if (operation.OperationFlow == OperationFlow.FilesToFilesWithFormat)
                PreparePathLibre(inputFileAlt, operation);
            else
            {
                PreparePathSingle(inputFileAlt, operation);
            }
        }

        Files.ViewFile(dir);
    }

    public static InputClass PrepareInput(string file, string output, string dir, string phrase ,int before = 0, int after = 0)
    {
        InputClass inputFile = new InputClass
        {
            inputFile = file,
            outputFile = output,
            dir = dir,
            phrase = phrase,
            before = before,
            after = after
        };
        
        return inputFile;
    }
    
    public static InputClass PrepareInput(string [] files, string output, string dir, string phrase)
    {
        InputClass inputFile = new InputClass
        {
            inputFiles = files,
            outputFile = output,
            dir = dir,
            phrase = phrase,
        };
        
        return inputFile;
    }

    public static bool InputSearchOpe(out string value)
    {
        Console.WriteLine("Podaj fraze: ");
        string phrase = Console.ReadLine()!;
        value = phrase;

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine("Nie podano frazy!");
            return false;
        }

        return true;
    }
    
    public static bool InputPagesOpe(out string value)
    {
        Console.WriteLine("Podaj strony: ");
        string phrase = Console.ReadLine()!;
        value = phrase;

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine("Nie podano frazy!");
            return false;
        }

        return true;
    }

    public static bool InputFormatOpe(out string value)
    {
        Console.WriteLine("Podaj format: ");
        string format = Console.ReadLine()!;
        value = format;

        if (!CheckParams.CheckFormat(value))
        {
            Console.WriteLine("Niepoprawny format!");
            return false;
        }

        return true;
    }

    public static void PreparePath(InputClass input, OperationDefinition operation)
    {
        int i = 1;

        string name = Path.GetFileNameWithoutExtension(input.inputFile);
        input.outputPath = Path.Combine(input.dir, $"{name}{operation.Extension}");
        
        while (Path.Exists(input.outputPath))
        {
            input.outputPath = Path.Combine(input.dir, $"{name}_{i}{operation.Extension}");
            i++;
        }
                
        ExecuteOpe(input, operation);
    }

    public static void PreparePathSingle(InputClass input, OperationDefinition operation)
    {
        int i = 1;
        
        input.outputPath = Path.Combine(input.dir, input.outputFile);

        if (string.IsNullOrEmpty(Path.GetExtension(input.outputFile)))
        {
            input.outputPath = Path.Combine(input.dir, input.outputFile + operation.Extension);
        }
        
        while (Path.Exists(input.outputPath))
        {
            string name = Path.GetFileNameWithoutExtension(input.outputFile);
            input.outputPath = Path.Combine(input.dir, $"{name}_{i}{operation.Extension}");
            i++;
        }
                
        ExecuteOpe(input, operation);
    }
    
    public static void PreparePathLibre(InputClass input, OperationDefinition operation)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        input.tempDir = tempDir;

        ExecuteOpe(input, operation);
    }
    
    public static void ExecuteOpe(InputClass input, OperationDefinition operation)
    {
        try
        {
            Console.WriteLine("Trwa konwersja...");
            
            operation.FileOperationAction(input);
            Console.WriteLine("Operacja zakończona pomyślnie!");

            if (operation.OperationFlow != OperationFlow.FilesToFilesWithFormat)
            {
                if (!string.IsNullOrEmpty(Path.GetFullPath(input.outputPath)))
                    Console.WriteLine($"Zapisano w: {Path.GetFullPath(input.outputPath)}");
            }
            else
            {
                if (!string.IsNullOrEmpty(Path.GetFullPath(input.dir)))
                    Console.WriteLine($"Zapisano w: {Path.GetFullPath(input.dir)}");
            }
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
            Console.WriteLine("Szczegóły zapisano w logs/error_log.txt");
        }
    }
}