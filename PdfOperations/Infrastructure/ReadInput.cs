namespace PdfOperations;

public class ReadInput
{
    public static string ReadOutputFile()
    {
        string input = Console.ReadLine()!;

        if (input.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(Messages.OperationCancelled);
            return null;
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
                Console.WriteLine(Messages.OperationCancelled);
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
            
            Console.WriteLine(Messages.InvalidOption);
        }

        return input;
    }
}