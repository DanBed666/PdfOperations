namespace PdfOperations;

public class InputValidator
{
    public static string NormalizeExtension(string extension)
    {
        return extension.Trim().TrimStart('.').ToLowerInvariant();
    }
    
    public static string NormalizePages(string pages)
    {
        return pages.Trim();
    }
    
    public static bool IsOutputValid(string output)
    {
        return false;
    }
    
    public static bool IsKnownExtension(string extension)
    {
        if (!Enum.TryParse(extension, ignoreCase: true, out FileExtension format))
        {
            Console.WriteLine($"{Messages.UnsupportedFormat}: {extension}");
            return false;
        }
        
        return true;
    }
    
    public static bool IsExtensionValidForOpe(string outputExtension, string opeExtension)
    {
        return outputExtension == NormalizeExtension(opeExtension);
    }

    public static bool IsPagesFormatValid(string pages)
    {
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

    public static string GetDefaultDir()
    { 
        string dir = Path.Combine(AppContext.BaseDirectory, "output");
        
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return dir;
    }
    
    public static string SetDefaultOutputFile()
    {
        return "default";
    }
    
    public static string BuildOutputExt(string output, string opeExtension)
    {
        return output.Trim() + opeExtension;
    }

    public static bool AskForFixOutputExt(string outputExtension, string opeExtension)
    {
        Console.WriteLine($"{Messages.InvalidFormat} {outputExtension} {Messages.ExpectedFormat} {opeExtension}");

        return UserInput.ReadOption(Messages.FixFormatQuestion) == "t";
    }
}