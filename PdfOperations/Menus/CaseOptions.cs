namespace PdfOperations;

public class CaseOptions
{
    public static void ExecuteManyRun(OperationDefinition ope)
    {
        Console.WriteLine(Messages.ChooseFiles);
        string [] input = Files.AddFiles(ope.Filter);
        
        RunClass.RunFiles(input);
    }
    
    public static void ExecuteManyRunApp(OperationDefinition ope)
    {
        Console.WriteLine(Messages.ChooseFiles);
        string [] input = Files.AddFiles(ope.Filter);

        Console.WriteLine(Messages.ChooseApp);
        string app = Console.ReadLine();
        string appConv = "";

        if (app.Equals("w"))
        {
            appConv = "winword.exe";
            RunClass.RunFiles(input, appConv);
        }
        else if (app.Equals("d"))
        {
            RunClass.RunFilesDraw(input);
        }
    }
}