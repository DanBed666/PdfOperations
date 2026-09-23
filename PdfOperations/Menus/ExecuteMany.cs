namespace PdfOperations;

public class ExecuteMany
{
    public static void ExecuteManyRun(OperationDefinition ope)
    {
        Console.WriteLine(Messages.ChooseFiles);
        string []? input = UserInput.ReadFilesOrNull(ope.Filter, Messages.ChooseFiles);
        
        RunClass.RunFiles(input!);
    }
    
    public static void ExecuteManyRunApp(OperationDefinition ope)
    {
        Console.WriteLine(Messages.ChooseFiles);
        string []? input = UserInput.ReadFilesOrNull(ope.Filter, Messages.ChooseFiles);

        Console.WriteLine(Messages.ChooseApp);
        string app = Console.ReadLine()!;
        string appConv = "";
        
        if (app.Trim().Equals(":q", StringComparison.OrdinalIgnoreCase))
            throw new OperationCanceledException(Messages.OperationCancelled);

        if (app.Equals("w"))
        {
            appConv = "winword.exe";
            RunClass.RunFiles(input!, appConv);
        }
        else if (app.Equals("d"))
        {
            RunClass.RunFilesDraw(input!);
        }
    }
}