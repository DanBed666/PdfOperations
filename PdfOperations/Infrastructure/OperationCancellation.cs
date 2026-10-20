namespace PdfOperations;

public static class OperationCancellation
{
    public static void Run(TimeSpan timeout, string operation, Action<CancellationToken> action)
    {
        using var cancellation = new CancellationTokenSource(timeout);
        try
        {
            action(cancellation.Token);
            cancellation.Token.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"{operation} exceeded the time limit of {timeout.TotalSeconds:0.###} seconds.");
        }
    }
}
