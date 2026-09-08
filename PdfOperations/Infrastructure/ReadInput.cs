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

    public static PdfFragment AddFragment8(string filter)
    {
        string file = Files.AddFile(filter);
        Console.WriteLine("Numery podaj: ");
        string pages = Console.ReadLine()!;

        PdfFragment pdfFragment = new PdfFragment()
        {
            FileName = file,
            PageNumbers = pages
        };

        return pdfFragment;
    }

    public static List<PdfFragment> GetFragmentsList(string filter)
    {
        List<PdfFragment> pdfFragments = new List<PdfFragment>();
        string opt = "";
        
        pdfFragments.Add(AddFragment8(filter));

        do
        {
            Console.WriteLine("Czy chcesz dodać fragment (T/N)");
            opt = ReadOption();

            if (opt == "n")
                break;
                
            pdfFragments.Add(AddFragment8(filter));
        } 
        while (opt != "n");

        return pdfFragments;
    }
}