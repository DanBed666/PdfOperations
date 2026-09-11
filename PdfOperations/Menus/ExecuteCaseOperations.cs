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
            try
            {
                operationInput.PhraseToFind = ReadInput.ReadTextOrCancel(operation.PhrasePrompt, Messages.NoSearchPhraseProvided);
                operationInput.Before = ReadInput.ReadNumberOrCancel(operation.BeforePrompt, Messages.NoLinesProvided);
                operationInput.After = ReadInput.ReadNumberOrCancel(operation.AfterPrompt, Messages.NoLinesProvided);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        
        if (operation.AddInfo == "format")
        {
            try
            {
                operationInput.Format = ReadInput.ReadTextOrCancel(operation.FormatPrompt, Messages.NoFormatProvided);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        
        if (operation.AddInfo == "pages")
        {
            foreach (string file in operationInput.InputFiles)
            {
                int number = Info.GetPdfPagesSingle(file);
                Console.WriteLine($"Liczba stron plik {Path.GetFileName(file)}: {number}");
            }
            
            try
            {
                operationInput.Pages = ReadInput.ReadTextOrCancel(operation.PagesPrompt, Messages.NoPagesProvided);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
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

                if (CheckParams.TryPrepareOutput(operation, operationInput, output))
                    break;
            }
        }
        
        operationInput.Dir = Files.AddDirectory();
        Console.WriteLine($"{Messages.ChoosenDirectory} {operationInput.Dir}");

        return operationInput;
    }
}