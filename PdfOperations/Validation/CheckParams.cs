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

    public static string GetEffectiveExtension(OperationDefinition operation, OperationInput input)
    {
        if (!string.IsNullOrEmpty(operation.Extension))
            return operation.Extension;
        
        if (!string.IsNullOrEmpty(input.Format))
            return input.Format;
        
        if (input.InputFiles.Length > 0 && !string.IsNullOrEmpty(input.InputFiles[0]))
            return Path.GetExtension(input.InputFiles[0]);

        return "";
    }

    public static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            return "";

        return extension.StartsWith(".") ? extension : "." + extension;
    }

    public static bool TryPrepareOutput(OperationDefinition operation, OperationInput operationInput, string output)
    {
        if (!CheckFileFormat(output, out string format))
        {
            return CheckIfFormatNotExist(operation, operationInput, output, format, false);
        }
        
        if (!format.Equals(operation.Extension))
        {
            return CheckIfFormatExist(operation, operationInput, output, format, false);
        }

        operationInput.Output = output;
        return true;
    }
}