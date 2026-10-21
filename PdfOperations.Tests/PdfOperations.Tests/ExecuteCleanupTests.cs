using System.IO.Compression;

namespace PdfOperations.Tests;

[TestClass]
[DoNotParallelize]
public class ExecuteCleanupTests
{
    [TestMethod]
    public void ConsoleOperationRemovesTemporaryFilesAfterSuccess()
    {
        string destination = Directory.CreateTempSubdirectory("PdfOperationsExecuteTest-").FullName;
        string temporary = "";
        TextReader previousInput = Console.In;
        using var answers = new StringReader("n\n");
        try
        {
            Console.SetIn(answers);
            Execute.ExecuteOpe(new OperationInput { Dir = destination }, new OperationDefinition
            {
                OperationFlow = OperationFlow.FilesToFilesWithFormat,
                FileOperationActionLibre = (_, context) =>
                {
                    temporary = context.TempDir;
                    string nested = Directory.CreateDirectory(Path.Combine(temporary, "extracted")).FullName;
                    File.WriteAllText(Path.Combine(nested, "document.xml"), "working data");
                    File.WriteAllText(Path.Combine(temporary, "result.txt"), "result");
                }
            });
            Assert.IsFalse(string.IsNullOrEmpty(temporary));
            Assert.IsFalse(Directory.Exists(temporary));
            Assert.AreEqual("result", File.ReadAllText(Path.Combine(destination, "result.txt")));
        }
        finally
        {
            Console.SetIn(previousInput);
            if (!string.IsNullOrEmpty(temporary)) TemporaryDirectoryCleanup.Delete(temporary);
            TemporaryDirectoryCleanup.Delete(destination);
        }
    }

    [TestMethod]
    public void ConsoleOperationRemovesPartialFilesAfterFailure()
    {
        string temporary = "";
        var original = new InvalidOperationException("Expected conversion failure.");
        try
        {
            InvalidOperationException actual = Assert.Throws<InvalidOperationException>(() =>
                Execute.ExecuteOpe(new OperationInput(), new OperationDefinition
                {
                    OperationFlow = OperationFlow.FilesToFilesWithFormat,
                    FileOperationActionLibre = (_, context) =>
                    {
                        temporary = context.TempDir;
                        File.WriteAllText(Path.Combine(temporary, "partial.tmp"), "partial");
                        throw original;
                    }
                }));
            Assert.AreSame(original, actual);
            Assert.IsFalse(string.IsNullOrEmpty(temporary));
            Assert.IsFalse(Directory.Exists(temporary));
        }
        finally
        {
            if (!string.IsNullOrEmpty(temporary)) TemporaryDirectoryCleanup.Delete(temporary);
        }
    }

    [TestMethod]
    public void ReplacementFailureRemovesExtractedDocument()
    {
        string root = Directory.CreateTempSubdirectory("PdfOperationsReplaceCleanupTest-").FullName;
        string source = Path.Combine(root, "input.docx");
        string work = Directory.CreateDirectory(Path.Combine(root, "work")).FullName;
        try
        {
            using (var archive = ZipFile.Open(source, ZipArchiveMode.Create))
            using (var writer = new StreamWriter(archive.CreateEntry("word/document.xml").Open()))
                writer.Write("<document />");

            bool extracted = false;
            Assert.Throws<FileNotFoundException>(() => TemporaryDirectoryCleanup.Run(work, () =>
            {
                try
                {
                    Replacement.ReplaceTextWithPlaceholders(
                        new FileJob { InputFile = source, TempPath = Path.Combine(work, "output.docx") },
                        new OperationInput { PlaceholderFile = Path.Combine(root, "missing.xlsx"), Output = "output" },
                        new OperationContext { TempDir = work });
                }
                catch
                {
                    extracted = Directory.GetDirectories(work).Length > 0;
                    throw;
                }
            }));
            Assert.IsTrue(extracted);
            Assert.IsFalse(Directory.Exists(work));
            Assert.IsTrue(File.Exists(source));
        }
        finally { TemporaryDirectoryCleanup.Delete(root); }
    }
}
