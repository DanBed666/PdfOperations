namespace PdfOperations;

public class CheckParams
{
    public static bool checkParams(string [] files)
    {
        if (files.Length == 0 || files is null)
        {
            return false;
        }
           
        return true;
    }
    
    public static bool checkParams(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return true;
    }
}