namespace PdfOperations;

public class ExecutionBuilder8
{
    public static OperationContext SetOperationContext()
    {
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files8.PrepareTempDir()
        };
        
        return operationContext;
    }
    
    public static List<FileJob> SetFileJobsFilesToFiles(OperationDefinition operation, OperationInput input, OperationContext operationContext)
    {
        List<FileJob> fileJobs = new List<FileJob>();

        foreach (string fileOut in input.InputFiles)
        {
            FileJob fileJob = new FileJob
            {
                InputFile = fileOut,
                TempPath =  Files8.PrepareTempPath(operationContext.TempDir, fileOut, operation.Extension)
            };
            
            fileJobs.Add(fileJob);
        }

        return fileJobs;
    }
    
    public static FileJob SetFileJobFilesToSingle(OperationDefinition operation, OperationInput input, OperationContext operationContext)
    {
        FileJob fileJob = new FileJob
        {
            InputFiles = input.InputFiles,
            TempPath =  Files8.PrepareTempPath(operationContext.TempDir, input.Output, operation.Extension)
        };

        return fileJob;
    }
}