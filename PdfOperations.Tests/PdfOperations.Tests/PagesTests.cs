namespace PdfOperations.Tests;

[TestClass]
public class PagesTests
{
    [TestMethod]
    public void CreateWithPagesTest()
    {
        int suma = 0;

        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"]),
            Pages = "2-3"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".pdf"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files8.PrepareTempDir()
        };
        
        List<FileJob> fileJobList = ExecutionBuilder8.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Pages.CreateWithPages(operationInput, fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                TestHelper.AssertForOneFile(file, operationDefinition.Extension);
                Assert.AreEqual(2, Info.GetPdfPagesSingle(file));
                suma += Info.GetPdfPagesSingle(file);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
            Assert.AreEqual(6, suma);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void CreateWithPagesCustomTest()
    {
        /*
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
        */
    }
}