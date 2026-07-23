namespace PdfOperations;

public class ExecuteCaseOperations
{
    public static void InputOpe(OperationDefinition operation)
    {
        string output = "";
        
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(operation.Filter);
        
        if (!Files.CheckFiles(input))
        {
            return;
        }

        foreach (string file in input)
        {
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
        }
        
        if (input.Length == 1)
        {
            Files.ViewFile(input[0]);
            Console.WriteLine("Podaj output: ");
            output = Console.ReadLine()!;
        }
        
        string dir = Files.AddDirectory();
        
        InputClass inputClass = new InputClass
        {
            inputFiles = input,
            outputFile = output,
            dir = dir
        };

        if (BuildPaths(inputClass))
        {
            ExecuteOpe(inputClass, operation);
        }
    }
    
    public static bool BuildPaths(InputClass inputClass)
    {
        if (inputClass.inputFiles.Length == 0)
        {
            Console.WriteLine("Brak pliku wejściowego! Anulowano operacje!");
            return false;
        }
        
        if (string.IsNullOrEmpty(inputClass.dir))
        {
            Console.WriteLine("Domyslny dir");
            inputClass.dir = Files.GetDefaultDirectory();
        }

        if (inputClass.inputFiles.Length == 1)
        {
            if (string.IsNullOrEmpty(inputClass.outputFile))
            {
                Console.WriteLine("Domyslny plik");
                inputClass.outputFile = Files.GetDefaultOutputFile(inputClass.inputFiles[0]);
            }
        }

        return true;
    }
    
    public static void ExecuteOpe(InputClass inputClass, OperationDefinition operation)
    {
        try
        {
            Console.WriteLine("Trwa konwersja...");
            operation.MultipleFilesAction(inputClass, operation);
            Console.WriteLine("Operacja zakończona pomyślnie!");

            Console.WriteLine($"Zapisano w: {Path.GetFullPath(inputClass.dir)}");
            Files.ViewFile(inputClass.dir);
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
            Console.WriteLine("Szczegóły zapisano w logs/error_log.txt");
        }
    }
}