using System.Diagnostics;

namespace PdfOperations.Tests;

[TestClass]
public class OperationCancellationTests
{
    [TestMethod]
    public void CancelledTokenDoesNotStartExternalProcess()
    {
        var token = new CancellationToken(canceled: true);
        OperationCanceledException first = Assert.Throws<OperationCanceledException>(() =>
            RunClass.Run("missing-program.exe", [], TimeSpan.FromSeconds(5), token));
        OperationCanceledException second = Assert.Throws<OperationCanceledException>(() =>
            RunClass.RunWithOutput("missing-program.exe", TimeSpan.FromSeconds(5), token));
        Assert.AreEqual(token, first.CancellationToken);
        Assert.AreEqual(token, second.CancellationToken);
    }

    [TestMethod]
    public void UserCancellationIsNotReportedAsTimeout()
    {
        using var cancellation = new CancellationTokenSource();
        OperationCanceledException actual = Assert.Throws<OperationCanceledException>(() =>
            OperationCancellation.Run(TimeSpan.FromSeconds(5), "Test", token =>
            {
                cancellation.Cancel();
                token.ThrowIfCancellationRequested();
            }, cancellation.Token));
        Assert.AreEqual(cancellation.Token, actual.CancellationToken);
    }

    [TestMethod]
    public void TimeoutStillWorksWithUserCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        Assert.Throws<TimeoutException>(() =>
            OperationCancellation.Run(TimeSpan.FromMilliseconds(20), "Test", token =>
            {
                Assert.IsTrue(token.WaitHandle.WaitOne(TimeSpan.FromSeconds(5)));
                token.ThrowIfCancellationRequested();
            }, cancellation.Token));
        Assert.IsFalse(cancellation.IsCancellationRequested);
    }

    [TestMethod]
    public void AllOperationEntriesHonorPreCancelledToken()
    {
        var token = new CancellationToken(canceled: true);
        var file = new FileJob();
        var input = new OperationInput();
        var context = new OperationContext();
        Action[] operations =
        [
            () => Convert.FileToPdf(input, context, token),
            () => Convert.DocxToPdfWord(file, token),
            () => Convert.PdfToTxt(file, token),
            () => Convert.PdfToTxtWithPageBreaks(file, token),
            () => Convert.PdfToDocx(file, token),
            () => Convert.PdfToPict(file, token),
            () => Convert.PictToTxt(file, token),
            () => Convert.PictToPdf(file, token),
            () => Convert.ExtractPict(file, token),
            () => Divide.ManyToOne(file, token),
            () => Divide.OneToMany(file, token),
            () => Divide.DecryptFiles(file, token),
            () => Pages.CreateWithPages(input, file, token),
            () => Pages.CreateWithCustomFiles(input, file, token),
            () => Pages.SplitPages(file, [], 1, token),
            () => Info.ShowInfo(file, token),
            () => Info.ShowFontInfo(file, token),
            () => Info.GetPdfPagesSingle("missing.pdf", token),
            () => Search.SearchTempTextFiles(input, context, file, token),
            () => Replacement.ReplaceTextWithPlaceholders(file, input, context, token),
            () => WordConversionWorker.ConvertFile(file, token)
        ];
        foreach (Action operation in operations)
        {
            OperationCanceledException actual = Assert.Throws<OperationCanceledException>(operation);
            Assert.AreEqual(token, actual.CancellationToken);
        }
    }

    [TestMethod]
    public void RunCancelsRunningProcess()
    {
        CheckRunningProcessCancellation(captureOutput: false);
    }

    [TestMethod]
    public void RunWithOutputCancelsRunningProcess()
    {
        CheckRunningProcessCancellation(captureOutput: true);
    }

    private static void CheckRunningProcessCancellation(bool captureOutput)
    {
        if (!OperatingSystem.IsWindows())
            Assert.Inconclusive("Uses the Windows command interpreter.");
        string shell = Path.Combine(Environment.SystemDirectory, "cmd.exe");
        string[] arguments = ["/d", "/c", "ping -n 60 127.0.0.1 > nul"];
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        var watch = Stopwatch.StartNew();
        OperationCanceledException actual = Assert.Throws<OperationCanceledException>(() =>
        {
            if (captureOutput)
                RunClass.RunWithOutput(shell, TimeSpan.FromSeconds(30), cancellation.Token, arguments);
            else
                RunClass.Run(shell, [.. arguments], TimeSpan.FromSeconds(30), cancellation.Token);
        });
        Assert.AreEqual(cancellation.Token, actual.CancellationToken);
        Assert.IsNull(actual.InnerException, "Process shutdown did not finish cleanly.");
        Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(10));
    }
}
