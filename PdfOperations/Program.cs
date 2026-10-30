namespace PdfOperations;

class Program
{
    [STAThread] static int Main(string[] args)
    {
        if (WordConversionWorker.TryRun(args, out int exitCode))
            return exitCode;
        MainMenu.MainMenuF();
        return 0;
    }
}
