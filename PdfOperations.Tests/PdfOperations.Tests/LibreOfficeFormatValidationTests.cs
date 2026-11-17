using PdfOperations.Gui.Models;
using PdfOperations.Gui.ViewModels;

namespace PdfOperations.Tests;

[TestClass]
[DoNotParallelize]
public sealed class LibreOfficeFormatValidationTests
{
    [TestMethod]
    [DataRow("document.pdf", "pdf", true)]
    [DataRow("document.PDF", "pdf", true)]
    [DataRow("document.docx", "DOCX", true)]
    [DataRow("document.odg", ".odg", true)]
    [DataRow("document.odt", " odt ", true)]
    [DataRow("document.txt", "txt:Text (encoded):UTF8", true)]
    [DataRow("document.pdf", "pdf:writer_pdf_Export", true)]
    [DataRow("document.pdf", "odg", false)]
    [DataRow("document.docx", "pdf", false)]
    [DataRow("document.doc", "docx", false)]
    [DataRow("document.pdf.docx", "pdf", false)]
    [DataRow("document", "", false)]
    public void ComparesExtensionsIgnoringCaseAndExportFilter(string input, string format, bool expected)
    {
        Assert.AreEqual(expected, Convert.IsSameFormat(input, format));
    }

    [TestMethod]
    public void CoreRejectsEntireMixedBatchBeforeCallingConverter()
    {
        bool called = false;
        var input = new OperationInput { InputFiles = ["first.docx", "second.PDF"], Format = "pdf" };
        var error = Assert.Throws<ArgumentException>(() => Convert.RunLibreOfficeConversion(
            input, new OperationContext(), CancellationToken.None,
            _ => Assert.Fail("Cleanup should not be necessary before conversion."),
            _ => called = true));
        Assert.AreEqual("fileInput", error.ParamName);
        Assert.IsFalse(called);
    }

    [TestMethod]
    public void PublicConversionEntryPointRejectsSameFormat()
    {
        var input = new OperationInput { InputFiles = ["document.docx"], Format = "docx" };
        Assert.Throws<ArgumentException>(() => Convert.FileToPdf(input, new OperationContext()));
    }

    [TestMethod]
    public void DifferentFormatsStillReachConverter()
    {
        string root = Directory.CreateTempSubdirectory("PdfOperationsFormatTest-").FullName;
        bool called = false;
        try
        {
            var input = new OperationInput { InputFiles = ["document.pdf"], Format = "odg" };
            Convert.RunLibreOfficeConversion(input, new OperationContext { TempDir = root },
                CancellationToken.None, _ => Assert.Fail("Unexpected cleanup warning."), _ =>
                {
                    called = true;
                    File.WriteAllText(Path.Combine(root, "document.odg"), "converter fixture");
                });
            Assert.IsTrue(called);
        }
        finally { TemporaryDirectoryCleanup.Delete(root); }
    }

    [TestMethod]
    [DataRow(AppLanguage.Polish, false)]
    [DataRow(AppLanguage.English, false)]
    [DataRow(AppLanguage.Polish, true)]
    [DataRow(AppLanguage.English, true)]
    public async Task GuiWarnsWithoutCreatingOutputOrChangingInputs(AppLanguage language, bool mixed)
    {
        string root = Directory.CreateTempSubdirectory("PdfOperationsFormatGuiTest-").FullName;
        var vm = new LibreOfficeViewModel();
        var originalLanguage = vm.Loc.Language;
        try
        {
            string pdf = Path.Combine(root, "same.PDF");
            string docx = Path.Combine(root, "different.docx");
            File.WriteAllText(pdf, "original pdf fixture");
            File.WriteAllText(docx, "original docx fixture");
            vm.Loc.Language = language;
            vm.InputFilesList.SetFiles(mixed ? [docx, pdf] : [pdf]);
            vm.SelectedFormat = "pdf";
            vm.OutputDirectory = Path.Combine(root, "output");
            vm.OverwriteExistingFiles = true;
            await vm.StartCommand.ExecuteAsync(null);

            Assert.AreEqual(StatusKind.Warning, vm.StatusKind);
            Assert.AreEqual(vm.Loc.SameFormatConversion("same.PDF"), vm.StatusMessage);
            Assert.StartsWith(language == AppLanguage.Polish ? "Te pliki" : "These files", vm.StatusMessage);
            Assert.IsFalse(vm.IsBusy);
            Assert.AreEqual("", vm.ErrorDetails);
            Assert.IsFalse(Directory.Exists(vm.OutputDirectory));
            Assert.AreEqual("original pdf fixture", File.ReadAllText(pdf));
            Assert.AreEqual("original docx fixture", File.ReadAllText(docx));
            Assert.HasCount(mixed ? 2 : 1, vm.InputFilesList.Files);
            Assert.IsTrue(vm.StartCommand.CanExecute(null));
        }
        finally
        {
            vm.Loc.Language = originalLanguage;
            TemporaryDirectoryCleanup.Delete(root);
        }
    }
}
