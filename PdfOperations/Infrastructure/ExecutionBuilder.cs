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
                fileJob.TempPath = Files8.PrepareTempPathMultiple(operationContext.TempDir, fileJob.InputFile, operation.Extension);
            else
                fileJob.TempPath = Files8.PrepareTempPathSingle(operationContext.TempDir, input.Output);

            fileJob.FinalPath = Files8.PrepareFinalOutputPath(input.Dir, fileJob.TempPath);
            fileJobs.Add(fileJob);
        }

        return fileJobs;
    }
    
    public static FileJob SetFileJob(OperationInput input, OperationContext operationContext, OperationDefinition operation)
    {
        FileJob fileJob = new FileJob();
        
        fileJob.InputFiles = input.InputFiles;

        if (string.IsNullOrEmpty(input.Output))
            input.Output = "default" + operation.Extension;
        
        fileJob.TempPath = Files8.PrepareTempPathSingle(operationContext.TempDir, input.Output);
        fileJob.FinalPath = Files8.PrepareFinalOutputPath(input.Dir, fileJob.TempPath);

        return fileJob;
    }
    
    public static List<FileJob> SetFileJobLibre(OperationInput input, OperationContext operationContext)
    {
        List<FileJob> fileJobs = new List<FileJob>();

            foreach (string file in input.InputFiles)
            {
                FileJob fileJob = new FileJob();
                fileJob.InputFiles = input.InputFiles;
                fileJob.TempPath = Files8.PrepareTempPathMultiple(operationContext.TempDir, file, input.Format);
                fileJob.FinalPath = Files8.PrepareFinalOutputPath(input.Dir, fileJob.TempPath);
                fileJobs.Add(fileJob);
            }
            
            foreach (FileJob fj in fileJobs)
            {
                Console.WriteLine("Final: " + fj.FinalPath);
            }

            return fileJobs;
    }
}