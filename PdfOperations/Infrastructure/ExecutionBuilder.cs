namespace PdfOperations;

public class ExecutionBuilder
{
    public static OperationContext SetOperationContext()
    {
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
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
            };

            if (input.InputFiles.Length == 1)
                fileJob.TempPath = Files.PrepareTempPath(operationContext.TempDir, input.Output, operation.Extension);
            else
                fileJob.TempPath = Files.PrepareTempPath(operationContext.TempDir, fileOut, operation.Extension);

            fileJobs.Add(fileJob);
        }

        return fileJobs;
    }
    
    public static FileJob SetFileJobFilesToSingle(OperationDefinition operation, OperationInput input, OperationContext operationContext)
    {
        FileJob fileJob = new FileJob
        {
            InputFiles = input.InputFiles,
            TempPath =  Files.PrepareTempPath(operationContext.TempDir, input.Output, operation.Extension)
        };

        return fileJob;
    }
}