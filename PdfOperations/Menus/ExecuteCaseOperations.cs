namespace PdfOperations;

public class ExecuteCaseOperations
{
    public static OperationInput InputOpe(OperationDefinition operation)
    {
        OperationInput operationInput = new OperationInput();

        Console.WriteLine("Podaj nazwę pdf: ");
        operationInput.InputFiles = Files.AddFiles(operation.Filter);

        foreach (string file in operationInput.InputFiles)
        {
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
        }
        
        if (operationInput.InputFiles.Length == 1)
            Files.ViewFile(operationInput.InputFiles[0]);

        if (operation.AddInfo == "search")
        {
            if (!InputSearchOpe(out string value)) return null;
            operationInput.PhraseToFind = value;
            
            Console.WriteLine("Linie przed: ");
            Int32.TryParse(Console.ReadLine(), out int before);
            operationInput.Before = -before;
            
            Console.WriteLine("Linie po: ");
            Int32.TryParse(Console.ReadLine(), out int after);
            operationInput.After = after;
        }
        
        if (operation.AddInfo == "format")
        {
            if (!InputFormatOpe(out string value)) return null;
            operationInput.Format = value;
        }
        
        if (operation.AddInfo == "pages")
        {
            if (!InputPagesOpe(out string value)) return null;
            operationInput.Pages = value;
        }

        bool finish = false;

        if (operationInput.InputFiles.Length == 1 || operation.OperationFlow == OperationFlow.FilesToSingleFile
                                         || operation.OperationFlow == OperationFlow.SearchReport)
        {
            while (!finish)
            {
                string output = CheckParams.GetOutput();

                if (!CheckParams.CheckFileFormat(output, out string format))
                {
                    if (CheckParams.CheckIfFormatNotExist(operation, operationInput, output, format, finish))
                        break;
                }
                else if (!format.Equals(operation.Extension))
                {
                    if (CheckParams.CheckIfFormatExist(operation, operationInput, output, format, finish))
                        break;
                }
                else
                {
                    operationInput.Output = output;
                    break;
                }
            }
        }
        
        operationInput.Dir = Files.AddDirectory();

        return operationInput;
    }
    
    public static bool InputSearchOpe(out string value)
    {
        Console.WriteLine("Podaj fraze: ");
        string phrase = ReadInput.ReadOutputFile();
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
        string phrase = ReadInput.ReadOutputFile();
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
        string format = ReadInput.ReadOutputFile();
        value = format;

        if (!CheckParams.CheckFormat(value))
        {
            Console.WriteLine("Niepoprawny format!");
            return false;
        }

        return true;
    }
}