namespace PdfOperations.Tests;

[TestClass]
public class PagesTests
{
    [TestMethod]
    public void CreateWithPagesTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        string pages = "2-3";
        int count = 3;
        int suma = 0;

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
                Assert.AreEqual(2, Info.GetPdfPagesSingle(file));
                suma += Info.GetPdfPagesSingle(file);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
            Assert.AreEqual(6, suma);
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void CreateWithPagesCustomTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        string pages = "2-4";
        string output = "final.pdf";
        int count = 1;
        List<PdfFragment> pdfFragments = new List<PdfFragment>();

        TestInput testInput = TestHelper.PrepareInputWithOutputPages(inputs, extension, pages, output, pdfFragments);
        FileJob fileJob = ExecutionBuilder.SetFileJobFragment(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            Pages.CreateWithCustomFiles(testInput.Input, fileJob);

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
                Assert.AreEqual(9, Info.GetPdfPagesSingle(file));
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            //if (Directory.Exists(testInput.Context.TempDir))
                //Directory.Delete(testInput.Context.TempDir, true);
        }
    }
}