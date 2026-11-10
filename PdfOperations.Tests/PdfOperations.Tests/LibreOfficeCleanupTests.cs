namespace PdfOperations.Tests;

[TestClass]
public class LibreOfficeCleanupTests
{
    [TestMethod]
    [DataRow("valid")]
    [DataRow("missing")]
    [DataRow("empty")]
    [DataRow("truncated")]
    public void LockedProfileDoesNotDecideWhetherPdfIsSaved(string outputKind)
    {
        string root = Directory.CreateTempSubdirectory("PdfOperationsLibreCleanupTest-").FullName;
        string work = "";
        FileStream? held = null;
        try
        {
            string destination = Directory.CreateDirectory(Path.Combine(root, "out")).FullName;
            var context = new OperationContext { TempDir = Directory.CreateDirectory(Path.Combine(root, "stage")).FullName };
            var input = new OperationInput { InputFiles = [TestHelper.SetInputPath("word_1.docx")], Format = "pdf", Dir = destination };
            string result = Path.Combine(destination, "word_1.pdf");
            var warnings = new List<string>();
            var operation = new OperationDefinition
            {
                FileOperationActionLibre = (request, ctx) =>
                    Convert.RunLibreOfficeConversion(request, ctx, CancellationToken.None, warnings.Add, directory =>
                    {
                        work = directory;
                        string temp = Directory.CreateDirectory(Path.Combine(work, "temp")).FullName;
                        held = File.Open(Path.Combine(temp, "gpgme-test"), FileMode.CreateNew,
                            FileAccess.ReadWrite, FileShare.None);

                        // Use real LibreOffice for success; simulate bad output after a zero exit for the negative cases.
                        string staged = Path.Combine(ctx.TempDir, "word_1.pdf");
                        if (outputKind == "valid") Convert.RunLibreOffice(request, ctx, CancellationToken.None, directory);
                        if (outputKind == "empty") File.WriteAllBytes(staged, []);
                        if (outputKind == "truncated") File.WriteAllText(staged, "%PDF-1.7\n1 0 obj\n");
                    })
            };

            if (outputKind == "valid")
            {
                Execute.ExecuteFormat(operation, input, context);
                Assert.Contains("WORD_1", TestHelper.ReadPdfText(result));
                CollectionAssert.AreEqual(new[] { work }, warnings);
            }
            else
            {
                var error = Assert.Throws<InvalidOperationException>(() => Execute.ExecuteFormat(operation, input, context));
                Assert.IsFalse(File.Exists(result));
                Assert.IsEmpty(warnings);
                StringAssert.Contains((string)error.Data[TemporaryDirectoryCleanup.ErrorDataKey]!, work);
            }
            Assert.IsTrue(File.Exists(Path.Combine(work, "temp", "gpgme-test")));
        }
        finally
        {
            held?.Dispose();
            if (work.Length != 0) TemporaryDirectoryCleanup.Delete(work);
            TemporaryDirectoryCleanup.Delete(root);
        }
    }
}
