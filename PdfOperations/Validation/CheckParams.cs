namespace PdfOperations;

public class CheckParams
{
    public static bool CheckFileFormat(string output, out string formatStart)
    {
        formatStart = Path.GetExtension(output);
        string format = Path.GetExtension(output).Replace(".", "");

        if (!Enum.TryParse(typeof(FileExtension), format, ignoreCase: true, out object? ext))
        {
            Console.WriteLine(Messages.InvalidFormat);
            return false;
        }
        
        Console.WriteLine($"{Messages.ChoosenFormat} {ext}");
        return true;
    }
    
    public static bool CheckFormat(string output)
    {
        if (!Enum.TryParse(typeof(FileExtension), output, ignoreCase: true, out object? ext))
        {
            Console.WriteLine(Messages.InvalidFormat);
            return false;
        }
        
        Console.WriteLine($"{Messages.ChoosenFormat} {ext}");
        return true;
    }

    public static string GetOutput()
    {
        Console.WriteLine(Messages.EnterOutputName);
        string output = ReadInput.ReadOutputFile();
        return output;
    }

    public static bool CheckIfFormatNotExist(OperationDefinition operation, OperationInput operationInput, string output, string format, bool finish)
    {
        if (string.IsNullOrEmpty(format))
        {
            finish = FixFormatNotExist(operation.Extension, operationInput, output);
            Console.WriteLine($"{Messages.FormatAdded} {operation.Extension}!");
        }
        else
        {
            Console.WriteLine(Messages.UnsupportedFormat);
            Console.WriteLine(Messages.FixFormatQuestion);
            string inp = ReadInput.ReadOption();

            if (inp == "t")
            {
                finish = FixFormatExist(operation.Extension, operationInput, output);
                Console.WriteLine($"{Messages.FormatFixed} {operation.Extension}!");
            }
        }

        return finish;
    }
    
    public static bool CheckIfFormatExist(OperationDefinition operation, OperationInput operationInput, string output, string format, bool finish)
    {
        Console.WriteLine($"{Messages.InvalidFormat} {Messages.ExpectedFormat} {operation.Extension}");
        Console.WriteLine(Messages.FixFormatQuestion);
        string inp = ReadInput.ReadOption();

        if (inp == "t")
        {
            finish = FixFormatExist(operation.Extension, operationInput, output);
        }
        
        return finish;
    }

    public static bool FixFormatExist(string format, OperationInput operationInput, string output)
    {
        operationInput.Output = Path.GetFileNameWithoutExtension(output) + format;
        return true;
    }
    
    public static bool FixFormatNotExist(string format, OperationInput operationInput, string output)
    {
        if (output[^1] == '.')
            output = output.Replace(".", "");
        
        operationInput.Output = Path.GetFileNameWithoutExtension(output) + format;
        return true;
    }

    public static string NormalizeExtension(string extension)
    {
        return extension.StartsWith(".") ? extension : "." + extension;
    }
}