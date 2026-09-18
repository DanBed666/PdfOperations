namespace PdfOperations;

public class Execute8
{
    public static void ExecuteOpe(OperationInput fileInput, OperationDefinition operation)
    {
        OperationContext context = ExecutionBuilder8.SetOperationContext();

        switch (operation.OperationFlow)
        {
            case OperationFlow.FilesToFiles:
                ExecuteFilesToFiles(operation, fileInput, context);
                break;    
            
            case OperationFlow.FilesToSingleFile:
                ExecuteFilesToSingle(operation, fileInput, context);
                break;
        }
    }

    public static void ExecuteFilesToFiles(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        List<FileJob> fileJobs = new List<FileJob>();
        fileJobs = ExecutionBuilder8.SetFileJobsFilesToFiles(operation, fileInput, context);

        foreach (FileJob fileJob in fileJobs)
        {
            operation.FileOperationActionMultiple(fileJob);
        }

        MoveToFinalDir(context.TempDir, fileInput.Dir);
    }
    
    public static void ExecuteFilesToSingle(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        FileJob fileJob = new FileJob();
        fileJob = ExecutionBuilder8.SetFileJobFilesToSingle(operation, fileInput, context);

        operation.FileOperationActionSingle(fileJob);
        MoveToFinalDir(context.TempDir, fileInput.Dir);
    }

    public static void MoveToFinalDir(string tempDir, string finalDir)
    {
        foreach (string file in Directory.GetFiles(tempDir))
        {
            string finalPath = Files8.PrepareFinalPath(finalDir, file);
            File.Move(file, finalPath, true);
        }
    }
}