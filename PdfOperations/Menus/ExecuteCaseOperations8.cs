namespace PdfOperations;

public class ExecuteCaseOperations8
{
    public static OperationInput? InputOpe(OperationDefinition operation)
    {
        OperationInput operationInput = new OperationInput();
        Console.WriteLine(Messages.CancelInfo);
        
        //Set input files

        if (operation.OperationFlow != OperationFlow.FilesPagesFragments)
        {
            string []? files = UserInput.ReadFilesOrNull(operation.Filter, Messages.ChooseFiles);

            if (files is null || files.Length == 0)
            {
                Console.WriteLine(Messages.NoFileSelected);
                return null;
            }
            
            operationInput.InputFiles = files;
            
            foreach (string file in operationInput.InputFiles)
            {
                Console.WriteLine(Messages.ChoosenFile + Path.GetFullPath(file));
            }

            if (operationInput.InputFiles.Length == 1)
            {
                string opt = UserInput.ReadOption(Messages.PreviewFileQuestion);
                
                if (opt.Equals("t"))
                    RunClass.RunFile(operationInput.InputFiles[0]);
            }
        }

        //Set file page fragment
        
        if (operation.OperationFlow == OperationFlow.FilesPagesFragments)
        {
            List<PdfFragment> pdfFragments = UserInput.ReadFragments(operation.Filter);
            operationInput.PdfFragments = pdfFragments;
        }
        
        //Set output file

        if (operationInput.InputFiles.Length == 1 || operation.OperationFlow == OperationFlow.FilesToSingleFile
                              || operation.OperationFlow == OperationFlow.SearchReport
                              || operation.OperationFlow == OperationFlow.FilesPagesSingle
                              || operation.OperationFlow == OperationFlow.FilesPagesFragments)
        {
            string output = UserInput.ReadOutputOrCancel(operation);
            operationInput.Output = output;
        }
        
        //Set placeholder file
        
        if (operation.OperationFlow == OperationFlow.FilesReplacement)
        {
            Console.WriteLine(Messages.ChooseFiles);
            string? file = UserInput.ReadFileOrNull(operation.Filter, Messages.ChooseFiles);

            if (string.IsNullOrEmpty(file))
            {
                Console.WriteLine(Messages.NoFileSelected);
                return null;
            }

            operationInput.PlaceholderFile = file;
        }
        
        //Set pages

        if (operation.OperationFlow == OperationFlow.FilesPages)
        {
            foreach (string file in operationInput.InputFiles)
            {
                int ile = Info.GetPdfPagesSingle(file);
                Console.WriteLine($"{Messages.PagesCount} {file}: {ile}");
            }

            string pages = UserInput.ReadPagesOrCancel(Messages.EnterPages);
            operationInput.Pages = pages;
        }
        
        //Set search fields
        
        if (operation.OperationFlow == OperationFlow.SearchReport)
        {
            string searchPhrase = UserInput.ReadRequiredText(Messages.EnterSearchPhrase);
            int before = UserInput.ReadIntOrDefaultZero(Messages.EnterBeforeLines);
            int after = UserInput.ReadIntOrDefaultZero(Messages.EnterAfterLines);
            
            operationInput.PhraseToFind = searchPhrase;
            operationInput.Before = before;
            operationInput.After = after;
        }
        
        //Set libre format
        
        if (operation.OperationFlow == OperationFlow.FilesToFilesWithFormat)
        {
            string format = UserInput.ReadFormatOrCancel();
            operationInput.Format = InputValidator.NormalizeExtension(format);
        }

        //Set directory
        
        operationInput.Dir = UserInput.ReadDirectoryOrDefault();

        return operationInput;
    }
}