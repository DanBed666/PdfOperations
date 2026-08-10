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
    
    public static List<FileJob> SetFileJob(OperationInput input, OperationContext operationContext, 
        OperationDefinition operation)
    {
        List<FileJob> fileJobs = new List<FileJob>();

        foreach (string file in input.InputFiles)
        {
            FileJob fileJob = new FileJob();
            fileJob.InputFile = file;
            
            if (operation.OperationFlow == OperationFlow.FilesToFiles)
            {
                if (input.InputFiles.Length > 1)
                    fileJob.TempPath = Files8.PrepareTempPathMultiple(fileJob.InputFile, operationContext, operation);
                else
                    fileJob.TempPath = Files8.PrepareTempPathMultipleOne(fileJob.InputFile, operationContext, operation);
            }
            else if (operation.OperationFlow == OperationFlow.FilesToSingleFile)
            {
                fileJob.TempPath = Files8.PrepareTempPathSingle(fileJob.InputFile, operationContext, operation);
            }

            fileJob.FinalPath = Files8.PrepareFinalOutputPath(input, fileJob.TempPath);
            fileJobs.Add(fileJob);
        }

        return fileJobs;
    }
}