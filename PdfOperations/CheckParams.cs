namespace PdfOperations;

public class CheckParams
{
    public static bool checkParams(string [] files, string dir)
    {
        if (files.Length == 0 || files is null)
        {
            return false;
        }
           
        return true;
    }
    
    public static bool checkParams(string input, string output)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return true;
    }
    
    public static bool checkParams(string input, string pages, string output)
    {
        
    }
}