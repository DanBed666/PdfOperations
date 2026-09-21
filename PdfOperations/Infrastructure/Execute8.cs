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
            
            case OperationFlow.FilesPages:
                ExecutePages(operation, fileInput, context);
                break; 
            
            case OperationFlow.FilesToFilesWithFormat:
                ExecuteFormat(operation, fileInput, context);
                break;
            
            case OperationFlow.SearchReport:
                ExecuteSearch(operation, fileInput, context);
                break;
            
            default:
                Console.WriteLine(Messages.MissingFlow);
                break;
        }
    }

    public static void ExecuteFilesToFiles(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        List<FileJob> fileJobs = ExecutionBuilder8.SetFileJobsFilesToFiles(operation, fileInput, context);

        foreach (FileJob fileJob in fileJobs)
        {
            operation.FileOperationActionMultiple(fileJob);
        }

        MoveToFinalDir(context.TempDir, fileInput.Dir);
    }
    
    public static void ExecutePages(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        List<FileJob> fileJobs = ExecutionBuilder8.SetFileJobsFilesToFiles(operation, fileInput, context);

        foreach (FileJob fileJob in fileJobs)
        {
            operation.FileOperationActionPages(fileInput, fileJob);
        }

        Dictionary<string, string> conflicts = MoveNewFilesAndCollectConflicts(context.TempDir, fileInput.Dir);
        
        if (conflicts.Count > 0)
        {
            bool overwrite = AskForOverwrite();
            MoveConflicts(conflicts, overwrite);
        }
    }
    
    public static void ExecuteFormat(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        operation.FileOperationActionLibre(fileInput, context);

        Dictionary<string, string> conflicts = MoveNewFilesAndCollectConflicts(context.TempDir, fileInput.Dir);

        if (conflicts.Count > 0)
        {
            bool overwrite = AskForOverwrite();
            MoveConflicts(conflicts, overwrite);
        }
    }
    
    public static void ExecuteSearch(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        List<FileJob> fileJobs = ExecutionBuilder8.SetFileJobsFilesToFiles(operation, fileInput, context);

        foreach (FileJob fileJob in fileJobs)
        {
            operation.FileOperationActionMultiple(fileJob);
        }

        operation.ReportOperationAction(fileInput, context);

        Dictionary<string, string> conflicts = MoveNewFilesAndCollectConflicts(context.TempDir, fileInput.Dir);

        if (conflicts.Count > 0)
        {
            bool overwrite = AskForOverwrite();
            MoveConflicts(conflicts, overwrite);
        }
    }
    
    public static void ExecuteFilesToSingle(OperationDefinition operation, OperationInput fileInput, OperationContext context)
    {
        FileJob fileJob = ExecutionBuilder8.SetFileJobFilesToSingle(operation, fileInput, context);

        operation.FileOperationActionSingle(fileJob);
        Dictionary<string, string> conflicts = MoveNewFilesAndCollectConflicts(context.TempDir, fileInput.Dir);

        if (conflicts.Count > 0)
        {
            bool overwrite = AskForOverwrite();
            MoveConflicts(conflicts, overwrite);
        }
    }
    
    public static void ExecuteRunApp(OperationDefinition operation)
    {
        operation.RunOperationAction(operation);
    }

    public static Dictionary<string, string> MoveNewFilesAndCollectConflicts(string tempDir, string finalDir)
    {
        Dictionary<string, string> conflicts = new Dictionary<string, string>();
        
        foreach (string file in Directory.GetFiles(tempDir))
        {
            string finalPath = Files8.PrepareFinalPath(finalDir, file);
            
            if (Path.Exists(finalPath))
                conflicts[file] = finalPath;
            else
                File.Move(file, finalPath);
        }

        return conflicts;
    }

    public static void MoveToFinalDir(string tempDir, string finalDir)
    {
        foreach (string file in Directory.GetFiles(tempDir))
        {
            string finalPath = Files8.PrepareFinalPath(finalDir, file);

            File.Move(file, finalPath, true);
        }
    }
    
    public static bool AskForOverwrite()
    {
        return UserInput.ReadOption(Messages.OverwriteFilesQuestion) == "t";
    }

    public static void MoveConflicts(Dictionary<string, string> conflicts, bool overwrite)
    {
        foreach (KeyValuePair<string, string> files in conflicts)
        {
            if (!overwrite)
            {
                int i = 1;

                while (true)
                {
                    string newFinalPath = Path.Combine(Path.GetDirectoryName(files.Value)!,
                        Path.GetFileNameWithoutExtension(files.Value) + $"_{i++}" + Path.GetExtension(files.Value));

                    if (!Path.Exists(newFinalPath))
                    {
                        File.Move(files.Key, newFinalPath);
                        break;
                    }
                }
            }
            else
            {
                File.Move(files.Key, files.Value, true);
            }
        }
    }
}