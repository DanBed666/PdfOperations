namespace PdfOperations;

public class ExecuteCaseOperations
{
    public static void InputInfo(OperationDefinition operation)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(operation.Filter);
        
        foreach (string file in input)
        {
            InputClass inputFile = new InputClass
            {
                inputFile = file,
            };

            ExecuteOpe(inputFile, operation);
        }
    }
    
    public static void InputOpe(OperationDefinition operation)
    {
        if (operation.OperationFlow == OperationFlow.PdfReport)
        {
            InputInfo(operation);
            return;
        }
        
        string output = "";
        string phrase = "";
        
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(operation.Filter);

        if (input.Length == 0) return;

        foreach (string file in input)
        {
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
        }

        if (operation.Phrase == "search")
        {
            if (!InputSearchOpe(out string value)) return;
            phrase = value;
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
        
        if (input.Length == 1)
        {
            Files.ViewFile(input[0]);
            Console.WriteLine("Podaj output: ");
            output = Console.ReadLine()!;

            if (!CheckParams.CheckFileFormat(output))
                return;
        }
        
        string dir = Files.AddDirectory();

        if (operation.OperationFlow == OperationFlow.FilesToFiles)
        {
            foreach (string file in input)
            {
                InputClass inputFile = new InputClass
                {
                    inputFile = file,
                    outputFile = output,
                    dir = dir,
                    phrase = phrase
                };

                PreparePath(inputFile, operation);
            }
        }
        else
        {
            InputClass inputFile = new InputClass
            {
                inputFiles = input,
                outputFile = output,
                dir = dir,
                phrase = phrase
            };
            
            ExecuteOpe(inputFile, operation);
        }

        Files.ViewFile(dir);
        
        /*
        foreach (string fileInput in input)
        {
            InputClass inputClass = new InputClass
            {
                inputFiles = fileInput,
                outputFile = output,
                dir = dir,
                phrase = phrase
            };
            
            operation.GetOutputPathMultipleAction(inputClass, operation);
        }
        */
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
    
    public static void GetOutputPathMultiple(InputClass inputClass, OperationDefinition operation)
    {
        int i = 1;

        /*
        if (inputClass.inputFiles.Length >= 2)
        {
            foreach (string file in inputClass.inputFiles)
            {
                string name = Path.GetFileNameWithoutExtension(file);

                inputClass.outputPath = Path.Combine(inputClass.dir, $"{name}{operation.Extension}");
                
                if (File.Exists(inputClass.outputPath))
                {
                    inputClass.outputPath = Path.Combine(inputClass.dir, $"{name}_{i}{operation.Extension}");
                    i++;
                }
                
                ExecuteOpe(inputClass, operation);
            }
        }
        else
        {
            inputClass.outputPath = Path.Combine(inputClass.dir, inputClass.outputFile);
            ExecuteOpe(inputClass, operation);
        }
        */
    }
    
    public static void ExecuteOpe(InputClass input, OperationDefinition operation)
    {
        try
        {
            Console.WriteLine("Trwa konwersja...");
            
            operation.FileOperationAction(input);
            Console.WriteLine("Operacja zakończona pomyślnie!");
            Console.WriteLine($"Zapisano w: {Path.GetFullPath(input.outputPath)}");
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
            Console.WriteLine("Szczegóły zapisano w logs/error_log.txt");
        }
    }
}