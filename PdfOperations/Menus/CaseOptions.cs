namespace PdfOperations;

public class CaseOptions
{
    public static void ExecuteManyRun(OperationDefinition ope)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files8.AddFiles(ope.Filter);
        
        RunClass.RunFiles(input);
    }
    
    public static void ExecuteManyRunApp(OperationDefinition ope)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files8.AddFiles(ope.Filter);

        string app = Console.ReadLine();
        string appConv = "";

        if (app.Equals("w"))
        {
            appConv = "winword.exe";
            RunClass.RunFiles(input, appConv);
        }
        else if (app.Equals("d"))
        {
            string dir = AppContext.BaseDirectory;

            string soffice = Path.Combine(
                dir,
                "tools",
                "libreoffice",
                "program",
                "soffice.exe"
            );

            appConv = soffice;
            RunClass.RunFilesDraw(input, appConv);
        }
    }
}