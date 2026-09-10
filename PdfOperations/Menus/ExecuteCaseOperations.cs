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
            if (!InputSearchOpe(operation, out string value)) return null;
            operationInput.PhraseToFind = value;
            
            Console.WriteLine(operation.BeforePrompt);
            Int32.TryParse(Console.ReadLine(), out int before);
            operationInput.Before = -before;
            
            Console.WriteLine(operation.AfterPrompt);
            Int32.TryParse(Console.ReadLine(), out int after);
            operationInput.After = after;
        }
        
        if (operation.AddInfo == "format")
        {
            if (!InputFormatOpe(operation, out string value)) return null;
            operationInput.Format = value;
        }
        
        if (operation.AddInfo == "pages")
        {
            foreach (string file in operationInput.InputFiles)
            {
                int number = Info.GetPdfPagesSingle(file);
                Console.WriteLine($"Liczba stron plik {Path.GetFileName(file)}: {number}");
            }

            if (!InputPagesOpe(operation, out string value)) return null;
            operationInput.Pages = value;
        }
        
        bool finish = false;

        if (operationInput.InputFiles.Length == 1 || operation.OperationFlow == OperationFlow.FilesToSingleFile
                                                  || operation.OperationFlow == OperationFlow.SearchReport
                                                  || operation.OperationFlow == OperationFlow.FilesPagesSingle)
        {
            while (!finish)
            {
                string output = CheckParams.GetOutput();

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

        return operationInput;
    }
    
    public static bool InputSearchOpe(OperationDefinition operation, out string value)
    {
        Console.WriteLine(operation.PhrasePrompt);
        string phrase = ReadInput.ReadOutputFile();
        value = phrase;

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine(Messages.NoSearchPhraseProvided);
            return false;
        }

        return true;
    }
    
    public static bool InputPagesOpe(OperationDefinition operation, out string value)
    {
        Console.WriteLine(operation.PagesPrompt);
        string phrase = ReadInput.ReadOutputFile();
        value = phrase;

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine(Messages.NoPagesProvided);
            return false;
        }

        return true;
    }

    public static bool InputFormatOpe(OperationDefinition operation, out string value)
    {
        Console.WriteLine(operation.FormatPrompt);
        string format = ReadInput.ReadOutputFile();
        value = format;

        if (!CheckParams.CheckFormat(value))
        {
            Console.WriteLine(Messages.InvalidFormat);
            return false;
        }

        return true;
    }
}