namespace PdfOperations;

public class ReadInput
{
    public static string ReadOutputFile(OperationDefinition operation, OperationInput input)
    {
        string output = Console.ReadLine()!;

        if (string.IsNullOrEmpty(output))
        {
            output = operation.DefaultOutputName;
            string extension = CheckParams.GetEffectiveExtension(operation, input);
            Console.WriteLine($"Zapisano do {output}{extension}");
        }

        if (output.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(Messages.OperationCancelled);
            return null;
        }

        return output;
    }
    
    public static string ReadOption()
    {
        string input = Console.ReadLine()!;

        if (string.IsNullOrEmpty(input))
            return "n";

        if (input.Trim().Equals("t", StringComparison.OrdinalIgnoreCase) || input.Trim().Equals("n", StringComparison.OrdinalIgnoreCase))
        {
            return input;
        }
        
        if (input.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(Messages.OperationCancelled);
            return null;
        }

        Console.WriteLine(Messages.InvalidOption);

        return input;
    }

    public static PdfFragment AddFragment8(string filter)
    {
        string file = Files.AddFile(filter);
        Console.WriteLine(Info.GetPdfPagesSingle(file));
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

        while(true)
        {
            Console.WriteLine("Czy chcesz dodać fragment (T/N)");
            opt = ReadOption();
            
            if (string.IsNullOrEmpty(opt))
                continue;

            if (opt == "n")
                break;

            if (opt == "t")
            {
                pdfFragments.Add(AddFragment8(filter));
                break;
            }
        }

        return pdfFragments;
    }
}