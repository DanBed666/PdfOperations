namespace PdfOperations;

public class CheckParams
{
    public static bool TryPrepareOutputParams(string input, string? dir, string? output, 
        out string finalDir, out string finalOutput)
    {
        finalDir = dir ?? "";
        finalOutput = output ?? "";
        
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("No input file provided!");
            return false;
        }

        if (string.IsNullOrEmpty(dir))
        {
            Console.WriteLine("Domyslny dir");
            finalDir = Files.GetDefaultDirectory();
        }

        if (string.IsNullOrEmpty(finalOutput))
        {
            Console.WriteLine("Domyslny plik");
            finalOutput = Files.GetDefaultOutputFile(input);
        }
        
        finalOutput = Path.Combine(finalDir, finalOutput);

        if (File.Exists(finalOutput))
        {
            finalOutput = Files.GetAvailableFileName(finalOutput);
        }

        if (!CheckFileFormat(finalOutput))
        {
            return false;
        }

        return true;
    }
    
    public static bool TryPrepareOutputParamsArg(string input, string arg, string mess, string? dir, string? output, 
        out string finalDir, out string finalOutput)
    {
        finalDir = dir ?? "";
        finalOutput = output ?? "";
        
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("No input file provided!");
            return false;
        }
        
        if (string.IsNullOrEmpty(arg))
        {
            Console.WriteLine(mess);
            return false;
        }

        if (string.IsNullOrEmpty(dir))
        {
            Console.WriteLine("Domyslny dir");
            finalDir = Files.GetDefaultDirectory();
        }

        if (string.IsNullOrEmpty(finalOutput))
        {
            Console.WriteLine("Domyslny plik");
            finalOutput = Files.GetDefaultOutputFile(input);
        }
        
        finalOutput = Path.Combine(finalDir, finalOutput);

        if (File.Exists(finalOutput))
        {
            finalOutput = Files.GetAvailableFileName(finalOutput);
        }

        if (!CheckFileFormat(finalOutput))
        {
            return false;
        }

        return true;
    }
    
    public static bool TryPrepareOutputParams(string [] input, string? dir, string? output, 
            out string finalDir, out string finalOutput)
        {
            finalDir = dir ?? "";
            finalOutput = output ?? "";
            
            if (input.Length == 0)
            {
                Console.WriteLine("No input file provided!");
                return false;
            }
    
            if (string.IsNullOrEmpty(dir))
            {
                Console.WriteLine("Domyslny dir");
                finalDir = Files.GetDefaultDirectory();
            }
    
            if (string.IsNullOrEmpty(finalOutput))
            {
                Console.WriteLine("Domyslny plik");
                string dirDef = Path.Combine(AppContext.BaseDirectory, "output");
                Directory.CreateDirectory(dirDef);
                string outfile = Path.Combine(dirDef, "output");
                finalOutput = Files.GetDefaultOutputFile(outfile);
            }
            
            finalOutput = Path.Combine(finalDir, finalOutput);
    
            if (File.Exists(finalOutput))
            {
                finalOutput = Files.GetAvailableFileName(finalOutput);
            }
    
            if (!CheckFileFormat(finalOutput))
            {
                return false;
            }
    
            return true;
        }
    
    public static bool TryPrepareOutputOnlyDir(string [] input, string arg, string mess, string? dir, 
        out string finalDir)
    {
        finalDir = dir ?? "";
            
        if (input.Length == 0)
        {
            Console.WriteLine("No input file provided!");
            return false;
        }

        if (string.IsNullOrEmpty(arg))
        {
            Console.WriteLine(mess);
            return false;
        }
    
        if (string.IsNullOrEmpty(dir))
        {
            Console.WriteLine("Domyslny dir");
            finalDir = Files.GetDefaultDirectory();
        }

        if (!CheckFormat(arg))
        {
            return false;
        }
    
        return true;
    }
    
    public static bool TryPrepareOutputOnlyDirLight(string [] input, string? dir, 
        out string finalDir)
    {
        finalDir = dir ?? "";
            
        if (input.Length == 0)
        {
            Console.WriteLine("No input file provided!");
            return false;
        }

        if (string.IsNullOrEmpty(dir))
        {
            Console.WriteLine("Domyslny dir");
            finalDir = Files.GetDefaultDirectory();
        }

        return true;
    }

    public static bool CheckFileFormat(string output)
    {
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
    
    public static bool TryPrepareOutputParamsMultiple(string input, string? dir, string? output, 
        out string finalDir, out string finalOutput)
    {
        finalDir = dir ?? "";
        finalOutput = output ?? "";
        
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("No input file provided!");
            return false;
        }

        if (string.IsNullOrEmpty(dir))
        {
            Console.WriteLine("Domyslny dir");
            finalDir = Files.GetDefaultDirectory();
        }

        if (string.IsNullOrEmpty(finalOutput))
        {
            Console.WriteLine("Domyslna nazwa");
            finalOutput = "output";
        }

        return true;
    }
}