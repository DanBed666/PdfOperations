namespace PdfOperations;

public class Help
{
    public static void ShowHelp()
    {
        string helpPath = Path.Combine(AppContext.BaseDirectory, "help.txt");
        
        if (File.Exists(helpPath))
            RunClass.RunFile(helpPath);
        else
            Console.WriteLine(Messages.NoHelpFile);
    }
}