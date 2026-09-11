namespace PdfOperations;

public class ReadInput
{
    public static string ReadOutputFile(OperationDefinition operation, OperationInput input)
    {
        string output = Console.ReadLine()!;

        if (string.IsNullOrWhiteSpace(output))
        {
            string extension = CheckParams.GetEffectiveExtension(operation, input);
            output = operation.DefaultOutputName + extension;
            Console.WriteLine($"Zapisano do {output}");
        }

        if (output.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
            throw new OperationCanceledException(Messages.OperationCancelled);

        return output;
    }
    
    public static string ReadTextOrCancel(string prompt, string errMessage)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string element = Console.ReadLine()!;
            
            if (element.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
                throw new OperationCanceledException(Messages.OperationCancelled);

            if (string.IsNullOrWhiteSpace(element))
            {
                Console.WriteLine(errMessage);
                continue;
            }

            return element.Trim();
        }
    }
    
    public static int ReadNumberOrCancel(string prompt, string errMessage)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string element = Console.ReadLine()!;
            
            if (element.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
                throw new OperationCanceledException(Messages.OperationCancelled);

            if (string.IsNullOrWhiteSpace(element))
            {
                Console.WriteLine(errMessage);
                continue;
            }

            if (int.TryParse(element, out int result))
                return result;
            
            Console.WriteLine(Messages.InvalidNumber);
        }
    }
    
    public static string ReadOption()
    {
        while (true)
        {
            string input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input))
                return "n";

            if (input.Trim().Equals("t", StringComparison.OrdinalIgnoreCase) ||
                input.Trim().Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                return input;
            }

            if (input.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
                throw new OperationCanceledException(Messages.OperationCancelled);

            Console.WriteLine(Messages.InvalidOption);
        }
    }

    public static PdfFragment AddFragment8(string filter)
    {
        while (true)
        {
            string file = Files.AddFile(filter);
            Console.WriteLine(Info.GetPdfPagesSingle(file));

            string pages = ReadTextOrCancel(Messages.EnterPages, Messages.NoPagesProvided);

            PdfFragment pdfFragment = new PdfFragment()
            {
                FileName = file,
                PageNumbers = pages
            };

            return pdfFragment;
        }
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

            if (string.IsNullOrWhiteSpace(opt))
            {
                Console.WriteLine(Messages.NoOptionProvided);
                continue;
            }

            if (opt == "n")
                break;

            if (opt == "t")
            {
                pdfFragments.Add(AddFragment8(filter));
            }
        }

        return pdfFragments;
    }
}