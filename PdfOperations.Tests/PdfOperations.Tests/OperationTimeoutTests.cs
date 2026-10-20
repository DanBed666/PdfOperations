namespace PdfOperations.Tests;

[TestClass]
public class OperationTimeoutTests
{
    [TestMethod]
    public void CompletedOperationDoesNotThrow()
    {
        bool completed = false;
        OperationCancellation.Run(TimeSpan.FromSeconds(5), "Test", token =>
        {
            token.ThrowIfCancellationRequested();
            completed = true;
        });
        Assert.IsTrue(completed);
    }

    [TestMethod]
    public void ExpiredOperationThrowsTimeout()
    {
        Assert.Throws<TimeoutException>(() =>
            OperationCancellation.Run(TimeSpan.FromMilliseconds(20), "Test", token =>
            {
                token.WaitHandle.WaitOne(TimeSpan.FromSeconds(5));
                token.ThrowIfCancellationRequested();
            }));
    }

    [TestMethod]
    public void OtherErrorsAreNotReportedAsTimeout()
    {
        Assert.Throws<FileNotFoundException>(() =>
            OperationCancellation.Run(TimeSpan.FromSeconds(5), "Test",
                _ => throw new FileNotFoundException("Expected")));
    }

    [TestMethod]
    public void CancelledSearchDoesNotReadFile()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Assert.Throws<OperationCanceledException>(() =>
            Search.GetFoundLines("missing.txt", "test", 0, 0, "missing.txt", cancellation.Token));
    }

    [TestMethod]
    public void CancelledExcelReadDoesNotOpenWorkbook()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Assert.Throws<OperationCanceledException>(() =>
            Replacement.ReadReplacementsFromExcel("missing.xlsx", cancellation.Token));
    }

    [TestMethod]
    public void CancelledReplacementDoesNotModifyXml()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCancellationTest-").FullName;
        try
        {
            string file = Path.Combine(directory, "content.xml");
            const string original = "<root>PLACEHOLDER</root>";
            File.WriteAllText(file, original);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            List<ReplacementPair> pairs = [new() { Find = "PLACEHOLDER", Replace = "changed" }];
            foreach (string extension in new[] { ".docx", ".odt", ".odg" })
            {
                Assert.Throws<OperationCanceledException>(() =>
                    Replacement.ReplaceTextInFile(file, pairs, extension, cancellation.Token));
                Assert.AreEqual(original, File.ReadAllText(file));
            }
        }
        finally { Directory.Delete(directory, true); }
    }

    [TestMethod]
    public void SearchWithoutCancellationStillReturnsMatches()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsSearchTest-").FullName;
        try
        {
            string file = Path.Combine(directory, "search.txt");
            File.WriteAllText(file, "alpha\fALPHA");
            SearchResult result = Search.GetFoundLines(file, "alpha", 0, 0, file);
            Assert.AreEqual(2, result.Occurences);
        }
        finally { Directory.Delete(directory, true); }
    }

    [TestMethod]
    public void NormalApplicationArgumentsDoNotStartWorker()
    {
        Assert.IsFalse(WordConversionWorker.TryRun([], out int exitCode));
        Assert.AreEqual(0, exitCode);
        Assert.IsFalse(WordConversionWorker.TryRun(["ordinary-argument"], out _));
    }

    [TestMethod]
    public void InvalidWorkerArgumentsReturnFailure()
    {
        Assert.IsTrue(WordConversionWorker.TryRun(["--word-to-pdf-worker"], out int exitCode));
        Assert.AreEqual(1, exitCode);
    }

    [TestMethod]
    public void MissingWordInputFailsWithoutStartingWord()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsWorkerTest-").FullName;
        try
        {
            bool handled = WordConversionWorker.TryRun(
                ["--word-to-pdf-worker", Path.Combine(directory, "missing.docx"),
                    Path.Combine(directory, "output.pdf"), directory], out int exitCode);
            Assert.IsTrue(handled);
            Assert.AreEqual(1, exitCode);
            StringAssert.Contains(File.ReadAllText(Path.Combine(directory, "error.txt")),
                "Input document was not found.");
            Assert.IsFalse(File.Exists(Path.Combine(directory, "word-process.json")));
        }
        finally { Directory.Delete(directory, true); }
    }

    [TestMethod]
    public void WordConversionCannotOverwriteItsSource()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsWordInputTest-").FullName;
        try
        {
            string input = Path.Combine(directory, "input.docx");
            File.WriteAllText(input, "source");
            Assert.Throws<InvalidOperationException>(() =>
                Convert.DocxToPdfWord(new FileJob { InputFile = input, TempPath = input }));
            Assert.AreEqual("source", File.ReadAllText(input));
        }
        finally { Directory.Delete(directory, true); }
    }

    [TestMethod]
    public void ReplacementWithoutCancellationStillUpdatesDocxAndOdfXml()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsReplacementTest-").FullName;
        try
        {
            string file = Path.Combine(directory, "document.xml");
            List<ReplacementPair> pairs = [new() { Find = "PLACEHOLDER", Replace = "changed" }];
            File.WriteAllText(file,
                "<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">" +
                "<w:body><w:p><w:r><w:t>PLACEHOLDER</w:t></w:r></w:p></w:body></w:document>");
            Replacement.ReplaceTextInFile(file, pairs, ".docx");
            StringAssert.Contains(File.ReadAllText(file), ">changed<");

            foreach (string extension in new[] { ".odt", ".odg" })
            {
                File.WriteAllText(file, "<root>PLACEHOLDER</root>");
                Replacement.ReplaceTextInFile(file, pairs, extension);
                Assert.AreEqual("<root>changed</root>", File.ReadAllText(file));
            }
        }
        finally { Directory.Delete(directory, true); }
    }

    [TestMethod]
    public void WordCleanupDoesNotKillOtherProcessTypes()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsWordCleanupTest-").FullName;
        try
        {
            using var process = System.Diagnostics.Process.GetCurrentProcess();
            File.WriteAllText(Path.Combine(directory, "word-process.json"),
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    Id = process.Id,
                    StartTimeUtcTicks = process.StartTime.ToUniversalTime().Ticks
                }));
            typeof(WordConversionWorker).GetMethod("StopRegisteredWord",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
                .Invoke(null, [directory]);
            Assert.IsFalse(process.HasExited);
        }
        finally { Directory.Delete(directory, true); }
    }
}
