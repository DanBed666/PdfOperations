namespace PdfOperations;

public class Execute
{
    public static void ExecuteOpe(OperationInput fileInput, OperationDefinition operation)
    {
        OperationContext context = ExecutionBuilder.SetOperationContext();
        List<FileJob> fileJobs = ExecutionBuilder.SetFileJob(fileInput, context, operation);
        
        try
        {
            switch (operation.OperationFlow)
            {
                case OperationFlow.FilesToFiles:
                    ExecuteOpeFilesToFiles(operation, fileJobs);
                    break;

                case OperationFlow.FilesToSingleFile:
                    ExecuteOpeFilesToSingleFile(operation, fileInput, fileJobs);
                    break;

                case OperationFlow.FilesToFilesWithFormat:
                    ExecuteOpeLibre(operation, fileInput, context);
                    break;

                case OperationFlow.SearchReport:
                    ExecuteOpeSearch(operation, fileInput, context, fileJobs);
                    break;
                
                case OperationFlow.FilesPages:
                    ExecuteOpePages(operation, fileInput, fileJobs);
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
    
    public static void ExecuteOpeFilesToFiles(OperationDefinition operation, List<FileJob> fileJobs)
    {
        operation.FileOperationActionMultiple(fileJobs);
    }
    
    public static void ExecuteOpeFilesToSingleFile(OperationDefinition operation, OperationInput fileInput, List<FileJob> fileJobs)
    {
        operation.FileOperationActionSingle(fileInput, fileJobs[0]);
    }
    
    public static void ExecuteOpePages(OperationDefinition operation, OperationInput fileInput, List<FileJob> fileJobs)
    {
        operation.FileOperationActionPages(fileInput, fileJobs);
    }
    
    public static void ExecuteOpeLibre(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        operation.FileOperationActionLibre(fileInput, context);
    }
    
    public static void ExecuteOpeSearch(OperationDefinition operation, OperationInput fileInput, OperationContext context, List<FileJob> fileJobs)
    {
        operation.ReportOperationAction(fileInput, context, fileJobs[0]);
    }
    
    public static void ExecuteRunApp(OperationDefinition operation)
    {
        operation.RunOperationAction(operation);
    }
}