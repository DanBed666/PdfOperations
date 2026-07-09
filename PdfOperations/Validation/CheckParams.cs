namespace PdfOperations;

public class CheckParams
{
    public static bool CheckInputParams(string [] files)
    {
        if (files.Length == 0 || files is null)
        {
            return false;
        }
           
        return true;
    }
    
    public static bool CheckInputParams(string input, string dir, string output)
    {
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("No input file provided!");
            return false;
        }

        if (string.IsNullOrEmpty(dir))
        {
            Console.WriteLine("Domyslny dir");
            Files.GetDefaultDirectory();
        }

        if (string.IsNullOrEmpty(output))
        {
            Console.WriteLine("Domyslny plik");
            Files.GetDefaultOutputFile(input);
        }

        if (File.Exists(output))
        {
            Files.GetAvailableFileName(output);
        }

        if (!CheckFileFormat(output))
        {
            return false;
        }

        return true;
    }

    public static bool CheckFileFormat(string output)
    {
        string format = Path.GetExtension(output).Replace(".", "");

        if (!Enum.TryParse(typeof(FileExtension), format, ignoreCase: true, out object? ext))
        {
            return false;
        }
        
        Console.WriteLine($"Wybrano format {ext}");
        return true;
    }
}