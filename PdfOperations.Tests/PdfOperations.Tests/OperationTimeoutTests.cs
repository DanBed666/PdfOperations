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
    [DataRow(".docx", false)]
    [DataRow(".docx", true)]
    [DataRow(".odt", false)]
    [DataRow(".odt", true)]
    [DataRow(".odg", false)]
    [DataRow(".odg", true)]
    public void ReplacementWithoutCancellationStillUpdatesDocxAndOdfXml(string extension, bool withDeclaration)
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsReplacementTest-").FullName;
        try
        {
            string file = Path.Combine(directory, "document.xml");
            List<ReplacementPair> pairs = [new() { Find = "PLACEHOLDER", Replace = "changed" }];
            const string wordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            const string textNamespace = "urn:oasis:names:tc:opendocument:xmlns:text:1.0";
            string body = extension == ".odt"
                ? "<office:text><text:p>PLACEHOLDER</text:p></office:text>"
                : "<office:drawing><draw:page><draw:frame><draw:text-box>" +
                  "<text:p>PLACEHOLDER</text:p></draw:text-box></draw:frame></draw:page></office:drawing>";
            string original = extension == ".docx"
                ? $"<w:document xmlns:w=\"{wordNamespace}\"><w:body><w:p><w:r>" +
                  "<w:t>PLACEHOLDER</w:t></w:r></w:p></w:body></w:document>"
                : "<office:document-content xmlns:office=\"urn:oasis:names:tc:opendocument:xmlns:office:1.0\" " +
                  $"xmlns:text=\"{textNamespace}\" xmlns:draw=\"urn:oasis:names:tc:opendocument:xmlns:drawing:1.0\">" +
                  $"<office:body>{body}</office:body></office:document-content>";
            if (withDeclaration)
                original = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + original;

            var expected = System.Xml.Linq.XDocument.Parse(original);
            System.Xml.Linq.XName textElement = System.Xml.Linq.XName.Get(
                extension == ".docx" ? "t" : "p", extension == ".docx" ? wordNamespace : textNamespace);
            var expectedText = expected.Descendants(textElement).Single();
            expectedText.Value = "changed";
            if (extension == ".docx")
                expectedText.SetAttributeValue(System.Xml.Linq.XNamespace.Xml + "space", "preserve");

            File.WriteAllText(file, original);
            Replacement.ReplaceTextInFile(file, pairs, extension);
            var actual = System.Xml.Linq.XDocument.Load(file);
            Assert.AreEqual("changed", actual.Descendants(textElement).Single().Value);
            // Serialization may add a declaration; compare document content and structure instead.
            Assert.IsTrue(System.Xml.Linq.XNode.DeepEquals(expected.Root, actual.Root));
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
