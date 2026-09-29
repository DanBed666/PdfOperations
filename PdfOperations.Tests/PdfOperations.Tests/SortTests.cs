namespace PdfOperations.Tests;

[TestClass]
public class SortTests
{
    [TestMethod]
    public void SortNumbersTest()
    {
        string [] sorted = FileSorter.SortFilesByNumberAndName(TestHelper.SetInputPaths([
            "test_1.pdf", "word_8.docx", "test_10.pdf", "ocr_test_3.pdf", "test_2.pdf"
        ]));

        Assert.IsNotEmpty(sorted);
        Assert.AreEqual("ocr_test_3.pdf", Path.GetFileName(sorted[0]));
        Assert.AreEqual("test_1.pdf", Path.GetFileName(sorted[1]));
        Assert.AreEqual("test_2.pdf", Path.GetFileName(sorted[2]));
        Assert.AreEqual("test_10.pdf", Path.GetFileName(sorted[3]));
        Assert.AreEqual("word_8.docx", Path.GetFileName(sorted[4]));
    }
}