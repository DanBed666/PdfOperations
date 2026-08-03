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
}