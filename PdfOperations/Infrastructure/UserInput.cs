namespace PdfOperations;

public class UserInput
{
    public static string ReadOptionalText()
    {
        string input = Console.ReadLine()!;
        
        if (input.Equals(":q"))
            throw new OperationCanceledException(Messages.OperationCancelled);
        
        return input;
    }
    
    public static string ReadRequiredText(string msg)
    {
        while (true)
        {
            Console.WriteLine(msg);
            string input = Console.ReadLine()!;

            if (input.Equals(":q"))
                throw new OperationCanceledException(Messages.OperationCancelled);
            
            if (string.IsNullOrWhiteSpace(input))
                continue;
            
            return input;
        }
    }

    public static int ReadIntOrDefaultZero(string msg)
    {
        while (true)
        {
            Console.WriteLine(msg);
            string numbuh = Console.ReadLine()!;
            
            if (numbuh.Equals(":q"))
                throw new OperationCanceledException(Messages.OperationCancelled);

            if (string.IsNullOrWhiteSpace(numbuh))
                return 0;

            if (int.TryParse(numbuh, out int result))
                return result;
            
            Console.WriteLine(Messages.InvalidNumber);
        }
    }
    
    public static int? ReadRequiredInt(string msg)
    {
        while (true)
        {
            Console.WriteLine(msg);
            string numbuh = Console.ReadLine()!;
            
            if (numbuh.Equals(":q"))
                throw new OperationCanceledException(Messages.OperationCancelled);

            if (int.TryParse(numbuh, out int result))
                return result;
            
            Console.WriteLine(Messages.InvalidNumber);
        }
    }
    
    public static string []? ReadFilesOrNull(string filter, string msg)
    {
        Console.WriteLine(msg);
        string [] files = Dialog.SelectFiles(filter);

        if (files.Length == 0)
            return null;
        
        return files;
    }
    
    public static string? ReadFileOrNull(string filter,string msg)
    {
        Console.WriteLine(msg);
        string file = Dialog.SelectFile(filter);

        if (string.IsNullOrWhiteSpace(file))
            return null;
        
        return file;
    }
    
    public static string ReadDirectoryOrDefault()
    {
        ReadOption(Messages.ChooseOutputDirectoryQuestion);
        Console.WriteLine(Messages.ChooseDirectory);
        string directory = Dialog.SelectDirectory();

        if (string.IsNullOrWhiteSpace(directory))
            return InputValidator.GetDefaultDir();

        return directory;
    }

    public static string ReadOption(string msg)
    {
        while (true)
        {
            Console.WriteLine(msg);
            string opt = Console.ReadLine()!;
            
            if (opt.Equals(":q"))
                throw new OperationCanceledException(Messages.OperationCancelled);

            if (string.IsNullOrWhiteSpace(opt))
                return "n";
            
            if (opt == "t" || opt == "n")
                return opt;
            
            Console.WriteLine(Messages.InvalidOption);
        }
    }

    public static string ReadFormatOrCancel()
    {
        while (true)
        {
            Console.WriteLine(Messages.EnterFormat);
            string format = Console.ReadLine()!;

            if (format.Equals(":q"))
                throw new OperationCanceledException(Messages.OperationCancelled);
            
            if (string.IsNullOrWhiteSpace(format))
                continue;

            format = InputValidator.NormalizeExtension(format);
            
            if (!InputValidator.IsKnownExtension(format))
                continue;
            
            return format;
        }
    }
    
    public static string ReadPagesOrCancel(string msg)
    {
        while (true)
        {
            Console.WriteLine(msg);
            string pages = Console.ReadLine()!;

            if (pages.Equals(":q"))
                throw new OperationCanceledException(Messages.OperationCancelled);
            
            if (string.IsNullOrWhiteSpace(pages))
                continue;

            pages = InputValidator.NormalizePages(pages);
            
            if (!InputValidator.IsPagesFormatValid(pages))
                continue;
            
            return pages;
        }
    }
    
    public static string ReadOutputOrCancel(OperationDefinition ope)
    {
        while (true)
        {
            Console.WriteLine(Messages.EnterOutputName);
            string outputFile = Console.ReadLine()!;

            if (outputFile.Equals(":q"))
                throw new OperationCanceledException(Messages.OperationCancelled);

            if (string.IsNullOrWhiteSpace(outputFile))
                return InputValidator.SetDefaultOutputFile(ope.DefaultOutputName, ope.Extension);

            string outputExtension = Path.GetExtension(outputFile);
            outputExtension = InputValidator.NormalizeExtension(outputExtension);

            if (string.IsNullOrWhiteSpace(outputExtension))
            {
                Console.WriteLine(Messages.NoFormatProvided);
                return InputValidator.BuildOutputExt(outputFile, ope.Extension);
            }

            if (!InputValidator.IsKnownExtension(outputExtension))
                continue;

            if (!InputValidator.IsExtensionValidForOpe(outputExtension, ope.Extension))
            {
                Console.WriteLine(Messages.InvalidFormat);
                
                if (InputValidator.AskForFixOutputExt(outputExtension, ope.Extension))
                    return InputValidator.CorrectOutputExt(outputFile, ope.Extension);

                continue;
            }
            
            return outputFile;
        }
    }

    public static List<PdfFragment> ReadFragments(string filter)
    {
        List<PdfFragment> fragments = new List<PdfFragment>();
        
        while (true)
        {
            string? file = ReadFileOrNull(filter, Messages.ChooseFiles);
            Console.WriteLine($"{Messages.PagesCount} {file}: {Info.GetPdfPagesSingle(file!)}");
            string pages = ReadPagesOrCancel(Messages.EnterPages);

            PdfFragment pdfFragment = new PdfFragment()
            {
                FileName = file,
                PageNumbers = pages
            };

            fragments.Add(pdfFragment);
            
            string opt = ReadOption(Messages.AddNextFileQuestion);

            if (opt.Equals("t"))
                continue;

            return fragments;
        }
    }
}