namespace PdfOperations;

public class ExecutionBuilder
{
    public static OperationContext SetOperationContext()
    {
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files8.PrepareTempDir()
        };
        
        return operationContext;
    }
    
    public static List<FileJob> SetFileJobList(OperationInput input, OperationContext operationContext, 
        OperationDefinition operation)
    {
        List<FileJob> fileJobs = new List<FileJob>();

        foreach (string file in input.InputFiles)
        {
            FileJob fileJob = new FileJob();
            fileJob.InputFile = file;

            if (input.InputFiles.Length > 1)
                fileJob.TempPath = Files8.PrepareTempPathMultiple(fileJob.InputFile, operationContext, operation);
            else
                fileJob.TempPath = Files8.PrepareTempPathMultipleOne(input, operationContext);

            fileJob.FinalPath = Files8.PrepareFinalOutputPath(input, fileJob.TempPath);
            fileJobs.Add(fileJob);
        }

        return fileJobs;
    }
    
    public static FileJob SetFileJob(OperationInput input, OperationContext operationContext)
    {
        FileJob fileJob = new FileJob();
        
        fileJob.InputFiles = input.InputFiles;
        fileJob.TempPath = Files8.PrepareTempPathSingle(input, operationContext);
        fileJob.FinalPath = Files8.PrepareFinalOutputPath(input, fileJob.TempPath);

        return fileJob;
    }
}