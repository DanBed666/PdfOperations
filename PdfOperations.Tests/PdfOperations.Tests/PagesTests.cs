namespace PdfOperations.Tests;

[TestClass]
public class PagesTests
{
    [TestMethod]
    public void CreateWithPages()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        string pages = "2-3";
        int count = 3;

        TestInput testInput = TestHelper.PrepareMultipleInputsPages(inputs, extension, pages);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Pages.CreateWithPages(testInput.Input, fileJob);
            }

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
}