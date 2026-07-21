namespace PdfOperations;

public class ErrorLogger
{
    public static void Log(Exception ex, string? message = null)
    {
        string logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDir);

        string logFile = Path.Combine(logDir, "error_log.txt");

        string text = 
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]" +
                $"{ex.GetType().Name}: {ex.Message}\n" +
                $"{ex.StackTrace}\n" +
                "----------------------------------\n";
        
        File.AppendAllText(logFile, text);
    }
}