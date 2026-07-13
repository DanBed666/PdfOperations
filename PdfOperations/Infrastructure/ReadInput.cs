namespace PdfOperations;

public class ReadInput
{
    public static string ReadOutputFile()
    {
        string input = Console.ReadLine()!;

        if (input.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (string.IsNullOrEmpty(input))
        {
            return "default";
        }

        return input;
    }

    public static string ReadOption()
    {
        string input = Console.ReadLine()!;
        
        while (string.IsNullOrEmpty(input))
        {
            if (input.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            
            if (input.Trim().Equals("t", StringComparison.OrdinalIgnoreCase))
            {
                return "t";
            }
            
            if (input.Trim().Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                return "n";
            }
            
            Console.WriteLine("Niepoprawna opcja!");
        }

        return input;
    }
}