namespace PdfOperations;

public class CheckParams
{
    public static bool CheckFileFormat(string output, out string formatStart)
    {
        formatStart = Path.GetExtension(output);
        string format = Path.GetExtension(output).Replace(".", "");

        if (!Enum.TryParse(typeof(FileExtension), format, ignoreCase: true, out object? ext))
        {
            Console.WriteLine("Zły format!");
            return false;
        }
        
        Console.WriteLine($"Wybrano format {ext}");
        return true;
    }
    
    public static bool CheckFormat(string output)
    {
        if (!Enum.TryParse(typeof(FileExtension), output, ignoreCase: true, out object? ext))
        {
            Console.WriteLine("Zły format!");
            return false;
        }
        
        Console.WriteLine($"Wybrano format {ext}");
        return true;
    }

    public static string GetOutput()
    {
        Console.WriteLine("Podaj output: ");
        string output = ReadInput.ReadOutputFile();
        return output;
    }

    public static bool CheckIfFormatNotExist(OperationDefinition operation, InputClass input, string output, string format, bool finish)
    {
        if (string.IsNullOrEmpty(format))
        {
            format = operation.Extension;

            if (output[^1] == '.')
                output = output.Replace(".", "");

            input.output = Path.GetFileNameWithoutExtension(output) + format;
            finish = true;
            Console.WriteLine($"Uzupełniono plik o format {operation.Extension}!");
        }
        else
        {
            Console.WriteLine("Format nieobsługiwany!");
            Console.WriteLine("Czy poprawić?");
            string inp = ReadInput.ReadOption();

            if (inp == "t")
            {
                format = operation.Extension;
                input.output = Path.GetFileNameWithoutExtension(output) + format;
                finish = true;
                Console.WriteLine($"Poprawiono format na {operation.Extension}!");
            }
        }

        return finish;
    }
    
    public static bool CheckIfFormatExist(OperationDefinition operation, InputClass input, string output, string format, bool finish)
    {
        Console.WriteLine($"Niepoprawny format! Poprawny format to {operation.Extension}");
        Console.WriteLine("Czy poprawić?");
        string inp = ReadInput.ReadOption();

        if (inp == "t")
        {
            format = operation.Extension;
            input.output = Path.GetFileNameWithoutExtension(output) + format;
            finish = true;
        }
        
        return finish;
    }
}