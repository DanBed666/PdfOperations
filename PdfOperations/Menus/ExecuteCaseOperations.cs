namespace PdfOperations;

public class ExecuteCaseOperations
{
    public static void InputOpe(OperationDefinition operation)
    {
        InputClass input = new InputClass();
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] files = Files.AddFiles(operation.Filter);

        if (files.Length == 0)
            return;
        
        input.inputFiles = files;

        foreach (string file in files)
        {
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
        }
        
        if (files.Length == 1)
            Files.ViewFile(files[0]);

        if (operation.Phrase == "search")
        {
            if (!InputSearchOpe(out string value)) return;
            input.phrase = value;
            
            Console.WriteLine("Linie przed: ");
            Int32.TryParse(Console.ReadLine(), out int before);
            input.before = -before;
            
            Console.WriteLine("Linie po: ");
            Int32.TryParse(Console.ReadLine(), out int after);
            input.after = after;
        }
        
        if (operation.Phrase == "format")
        {
            if (!InputFormatOpe(out string value)) return;
            input.format = value;
        }
        
        if (operation.Phrase == "pages")
        {
            if (!InputPagesOpe(out string value)) return;
            input.pages = value;
        }
        
        if (files.Length == 1 || operation.OperationFlow == OperationFlow.FilesToSingleFile)
        {
            Console.WriteLine("Podaj output: ");
            string output = ReadInput.ReadOutputFile();

            if (!CheckParams.CheckFileFormat(output))
                return;

            input.output = output;
        }
        
        string dir = Files.AddDirectory();

        if (string.IsNullOrEmpty(dir))
        {
            dir = Path.Combine(AppContext.BaseDirectory, "output");
        }

        input.dir = dir;
        
        Files.PrepareTempPath(input, operation);

        Files.ViewFile(dir);
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