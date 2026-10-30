namespace PdfOperations;

public static class TemporaryDirectoryCleanup
{
    public const string ErrorDataKey = "TemporaryDirectoryCleanupError";

    public static void Run(string directory, Action operation, Action? beforeDelete = null)
    {
        Exception? operationError = null;
        try
        {
            operation();
        }
        catch (Exception ex)
        {
            operationError = ex;
            throw;
        }
        finally
        {
            // Attempt directory cleanup even if stopping the external process fails.
            try
            {
                beforeDelete?.Invoke();
            }
            catch (Exception ex)
            {
                if (operationError is null)
                {
                    operationError = ex;
                    throw;
                }

                AddError(operationError, ex);
            }
            finally
            {
                Delete(directory, operationError);
            }
        }
    }

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
        AddError(operationError, cleanupError);
    }

    private static void AddError(Exception operationError, Exception cleanupError)
    {
        string details = cleanupError.ToString();
        if (operationError.Data[ErrorDataKey] is string previous)
            details = previous + Environment.NewLine + Environment.NewLine + details;
        operationError.Data[ErrorDataKey] = details;
    }
}
