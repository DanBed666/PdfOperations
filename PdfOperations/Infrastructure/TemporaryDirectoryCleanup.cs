namespace PdfOperations;

public static class TemporaryDirectoryCleanup
{
    public const string ErrorDataKey = "TemporaryDirectoryCleanupError";

    public static void Delete(string directory, Exception? operationError = null)
    {
        const int attempts = 10;
        Exception? lastError = null;
        for (int attempt = 0; attempt < attempts; attempt++)
        {
            try
            {
                Directory.Delete(directory, recursive: true);
                return;
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                lastError = ex;
                if (attempt < attempts - 1)
                    Thread.Sleep(200);
            }
        }

        var cleanupError = new IOException(
            $"Could not remove temporary directory '{directory}'. It was left on disk.", lastError);
        if (operationError is null)
            throw cleanupError;

        // Keep the original failure; callers can show this additional cleanup diagnostic.
        operationError.Data[ErrorDataKey] = cleanupError.ToString();
    }
}
