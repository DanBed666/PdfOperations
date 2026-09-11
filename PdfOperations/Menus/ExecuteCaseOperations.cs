namespace PdfOperations;

public class ExecuteCaseOperations
{
    public static OperationInput InputOpe(OperationDefinition operation)
    {
        OperationInput operationInput = new OperationInput();

        Console.WriteLine(Messages.CancelInfo);
        Console.WriteLine(operation.InputPrompt);

        if (operation.AddInfo != "fragments")
        {
            operationInput.InputFiles = Files.AddFiles(operation.Filter);
            
            if (operationInput.InputFiles.Length == 0)
            {
                Console.WriteLine(Messages.NoFileSelected);
                return null;
            }
        }
        else
        {
            List<PdfFragment> pdfFragments = new List<PdfFragment>();
            pdfFragments = ReadInput.GetFragmentsList(operation.Filter);
            
            operationInput.PdfFragments = pdfFragments;
            
            if (pdfFragments.Count == 0)
            {
                Console.WriteLine(Messages.NoFileSelected);
                return null;
            }
        }

        if (operation.AddInfo == "replace")
        {
            Console.WriteLine("Podaj plik z placeholderami");
            operationInput.PlaceholderFile = Files.AddFile(operation.FilterPlc);
            
            if (operationInput.PlaceholderFile == "")
            {
                Console.WriteLine(Messages.NoFileSelected);
                return null;
            }
        }
        
        foreach (string file in operationInput.InputFiles)
        {
            Console.WriteLine(Messages.ChoosenFile + Path.GetFullPath(file));
        }
        
        if (operationInput.InputFiles.Length == 1)
            Files.OpenPath(operationInput.InputFiles[0], "file");

        if (operation.AddInfo == "search")
        {
            operationInput.PhraseToFind = InputSearchOpe(operation, operationInput);
            
            Console.WriteLine(operation.BeforePrompt);
            Int32.TryParse(Console.ReadLine(), out int before);
            operationInput.Before = -before;
            
            Console.WriteLine(operation.AfterPrompt);
            Int32.TryParse(Console.ReadLine(), out int after);
            operationInput.After = after;
        }
        
        if (operation.AddInfo == "format")
        {
            operationInput.Format = InputFormatOpe(operation, operationInput);
        }
        
        if (operation.AddInfo == "pages")
        {
            foreach (string file in operationInput.InputFiles)
            {
                int number = Info.GetPdfPagesSingle(file);
                Console.WriteLine($"Liczba stron plik {Path.GetFileName(file)}: {number}");
            }

            operationInput.Pages = InputPagesOpe(operation, operationInput);
        }
        
        bool finish = false;

        if (operationInput.InputFiles.Length == 1 || operation.OperationFlow == OperationFlow.FilesToSingleFile
                                                  || operation.OperationFlow == OperationFlow.SearchReport
                                                  || operation.OperationFlow == OperationFlow.FilesPagesSingle)
        {
            while (!finish)
            {
                Console.WriteLine(Messages.EnterOutputName);
                string output = ReadInput.ReadOutputFile(operation, operationInput);

                if (!CheckParams.CheckFileFormat(output, out string format))
                {
                    if (CheckParams.CheckIfFormatNotExist(operation, operationInput, output, format, finish))
                        break;
                }
                else if (!format.Equals(operation.Extension))
                {
                    if (CheckParams.CheckIfFormatExist(operation, operationInput, output, format, finish))
                        break;
                }
                else
                {
                    operationInput.Output = output;
                    break;
                }
            }
        }
        
        operationInput.Dir = Files.AddDirectory();
        Console.WriteLine($"{Messages.ChoosenDirectory} {operationInput.Dir}");

        return operationInput;
    }
    
    public static string InputSearchOpe(OperationDefinition operation, OperationInput input)
    {
        while (true)
        {
            Console.WriteLine(operation.PhrasePrompt);
            string phrase = ReadInput.ReadOutputFile(operation, input);

            if (string.IsNullOrEmpty(phrase))
            {
                Console.WriteLine(Messages.NoSearchPhraseProvided);
                continue;
            }

            return phrase;
        }
    }
    
    public static string InputPagesOpe(OperationDefinition operation, OperationInput input)
    {
        while (true)
        {
            Console.WriteLine(operation.PagesPrompt);
            string pages = ReadInput.ReadOutputFile(operation, input);

            if (string.IsNullOrEmpty(pages))
            {
                Console.WriteLine(Messages.NoPagesProvided);
                continue;
            }

            return pages;
        }
    }

    public static string InputFormatOpe(OperationDefinition operation, OperationInput input)
    {
        while (true)
        {
            Console.WriteLine(operation.FormatPrompt);
            string format = ReadInput.ReadOutputFile(operation, input);
            
            if (string.IsNullOrEmpty(format))
            {
                Console.WriteLine(Messages.NoFormatProvided);
                continue;
            }

            if (!CheckParams.CheckFormat(format))
            {
                Console.WriteLine(Messages.InvalidFormat);
                continue;
            }

            return format;
        }
    }
}