namespace PdfOperations;

public class CheckParams
{
    public static bool IsExpectedFormat(string format, OperationInput operationInput, string output, bool finish)
    {
        Console.WriteLine($"{Messages.InvalidFormat} {Messages.ExpectedFormat} {GetEffectiveExtension(format, operationInput)}");
        
        if (string.IsNullOrEmpty(format))
        {
            finish = FixFormatExist(format, operationInput, output);
            Console.WriteLine($"{Messages.FormatAdded} {GetEffectiveExtension(format, operationInput)}!");
        }
        else 
        {
            Console.WriteLine(Messages.FixFormatQuestion);
            string inp = ReadInput.ReadOption();

            if (inp == "t")
            {
                finish = FixFormatExist(format, operationInput, output);
            }
        }

        return finish;
    }

    public static bool FixFormatExist(string format, OperationInput operationInput, string output)
    {
        operationInput.Output = Path.GetFileNameWithoutExtension(output) + format;
        return true;
    }
    
    public static string GetEffectiveExtension(string format, OperationInput input)
    {
        if (!string.IsNullOrEmpty(format))
            return format;
        
        if (!string.IsNullOrEmpty(input.Format))
            return "." + input.Format;
        
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
    
    public static bool TryPrepareOutput8(OperationDefinition operation, OperationInput operationInput, string output)
    {
        string format = Path.GetExtension(output);
        
        if (!IsFormatValid(format) && !string.IsNullOrWhiteSpace(format))
            return false;

        if (operation.OperationFlow != OperationFlow.FilesToFilesWithFormat &&
            operation.OperationFlow != OperationFlow.FilesReplacement)
        {
            if (!format.Equals(operation.Extension))
            {
                return IsExpectedFormat(operation.Extension, operationInput, output, false);
            }
        }
        else
        {
            if (operation.OperationFlow == OperationFlow.FilesToFilesWithFormat)
            {
                if (Path.GetExtension(output) != NormalizeExtension(operationInput.Format))
                {
                    return IsExpectedFormat(operationInput.Format, operationInput, output, false);
                }
            }
            else if (operation.OperationFlow == OperationFlow.FilesReplacement)
            {
                if (Path.GetExtension(output) != NormalizeExtension(Path.GetExtension(operationInput.InputFiles[0])))
                {
                    return IsExpectedFormat(Path.GetExtension(operationInput.InputFiles[0]), operationInput, output, false);
                }
            }
        }

        operationInput.Output = output;
        return true;
    }

    public static bool TryPrepareOutput(OperationDefinition operation, OperationInput operationInput, string output)
    {
        string format = Path.GetExtension(output);
        
        if (!IsFormatValid(format) && !string.IsNullOrWhiteSpace(format))
            return false;

        if (operation.OperationFlow != OperationFlow.FilesToFilesWithFormat &&
            operation.OperationFlow != OperationFlow.FilesReplacement)
        {
            if (!format.Equals(operation.Extension))
            {
                return IsExpectedFormat(operation.Extension, operationInput, output, false);
            }
        }
        else
        {
            if (operation.OperationFlow == OperationFlow.FilesToFilesWithFormat)
            {
                if (Path.GetExtension(output) != NormalizeExtension(operationInput.Format))
                {
                    return IsExpectedFormat(operationInput.Format, operationInput, output, false);
                }
            }
            else if (operation.OperationFlow == OperationFlow.FilesReplacement)
            {
                if (Path.GetExtension(output) != NormalizeExtension(Path.GetExtension(operationInput.InputFiles[0])))
                {
                    return IsExpectedFormat(Path.GetExtension(operationInput.InputFiles[0]), operationInput, output, false);
                }
            }
        }

        operationInput.Output = output;
        return true;
    }
    
    public static bool IsFormatValid(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
            return false;

        string normalize = format.Trim().TrimStart('.');

        if (!Enum.TryParse(normalize, ignoreCase: true, out FileExtension ext))
        {
            Console.WriteLine($"{Messages.InvalidFormat}: {normalize}");
            return false;
        }
        
        Console.WriteLine($"{Messages.ChoosenFormat} {ext}");

        return true;
    }
    
    public static bool IsSearchPhraseValid(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
            return false;

        return true;
    }

    public static bool IsValidPageFormat(string pages)
    {
        if (string.IsNullOrWhiteSpace(pages))
        {
            Console.WriteLine(Messages.NoPagesProvided);
            return false;
        }

        string [] parts = pages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string part in parts)
        {
            if (int.TryParse(part, out int pageNumbuh))
            {
                if (pageNumbuh <= 0)
                    return false;

                continue;
            }

            string[] range = pages.Split('-', StringSplitOptions.TrimEntries);

            if (range.Length != 2)
                return false;

            if (!int.TryParse(range[0], out int start))
                return false;

            if (!int.TryParse(range[1], out int finish))
                return false;

            if (start <= 0 || finish <= 0)
                return false;

            if (finish < start)
                return false;
        }

        return true;
    }
}