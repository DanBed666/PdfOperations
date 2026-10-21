namespace PdfOperations;

public static class OperationCancellation
{
    public static void Run(TimeSpan timeout, string operation, Action<CancellationToken> action,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var deadline = new CancellationTokenSource(timeout);
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(
            deadline.Token, cancellationToken);
        try
        {
            action(cancellation.Token);
            cancellation.Token.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException("The operation was cancelled.", ex, cancellationToken);
        }
        catch (OperationCanceledException) when (deadline.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"{operation} exceeded the time limit of {timeout.TotalSeconds:0.###} seconds.");
        }
    }
}
