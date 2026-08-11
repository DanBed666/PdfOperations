namespace PdfOperations;

public class Execute
{
    public static void SaveToTempDir(OperationDefinition operation, OperationInput fileInput, OperationContext context, List<FileJob> fileJobs)
    {
        foreach (FileJob fileJob in fileJobs)
        {
            try
            {
                switch (operation.OperationFlow)
                {
                    case OperationFlow.FilesToFiles:
                        ExecuteOpeFilesToFiles(operation, fileJob);
                        break;

                    case OperationFlow.FilesToSingleFile:
                        ExecuteOpeFilesToSingleFile(operation, fileInput, fileJob);
                        break;

                    case OperationFlow.FilesToFilesWithFormat:
                        ExecuteOpeLibre(operation, fileInput, context);
                        break;

                    case OperationFlow.SearchReport:
                        ExecuteOpeSearch(operation, fileInput, context, fileJob);
                        break;

                    case OperationFlow.FilesPages:
                        ExecuteOpePages(operation, fileInput, fileJob);
                        break;

                    case OperationFlow.RunApp:
                        ExecuteRunApp(operation);
                        break;

                    default:
                        Console.WriteLine("Brak flow");
                        break;
                }

                Console.WriteLine("Operacja zakończona pomyślnie!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }

    public static void MoveToFinalDir(OperationDefinition operation, List<FileJob> fileJobs)
    {
        Dictionary <string, string> existing = new Dictionary<string, string>();
        existing = Files8.FindNotExisitingFiles(fileJobs);

        if (existing.Count != 0)
        {
            Console.WriteLine("Znaleziono istniejące pliki! Czy chcesz nadpisać (T/N):");
            string opt = ReadInput.ReadOption();

            if (opt == "t")
            {
                Files8.OverWriteFile(existing);
            }
            else if (opt == "n")
            {
                Files8.SaveWithUniqueFileName(operation, existing);
            }
        }
    }
    
    public static void ExecuteOpe(OperationInput fileInput, OperationDefinition operation)
    {
        OperationContext context = ExecutionBuilder.SetOperationContext();
        List<FileJob> fileJobs = ExecutionBuilder.SetFileJob(fileInput, context, operation);
        
        SaveToTempDir(operation, fileInput, context, fileJobs);
        MoveToFinalDir(operation, fileJobs);
    }
    
    public static void ExecuteOpeFilesToFiles(OperationDefinition operation, FileJob fileJob)
    {
        operation.FileOperationActionMultiple(fileJob);
    }
    
    public static void ExecuteOpeFilesToSingleFile(OperationDefinition operation, OperationInput fileInput, FileJob fileJob)
    {
        operation.FileOperationActionSingle(fileInput, fileJob);
    }
    
    public static void ExecuteOpePages(OperationDefinition operation, OperationInput fileInput, FileJob fileJob)
    {
        operation.FileOperationActionPages(fileInput, fileJob);
    }
    
    public static void ExecuteOpeLibre(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        operation.FileOperationActionLibre(fileInput, context);
    }
    
    public static void ExecuteOpeSearch(OperationDefinition operation, OperationInput fileInput, OperationContext context, FileJob fileJob)
    {
        operation.ReportOperationAction(fileInput, context, fileJob);
    }
    
    public static void ExecuteRunApp(OperationDefinition operation)
    {
        operation.RunOperationAction(operation);
    }
}