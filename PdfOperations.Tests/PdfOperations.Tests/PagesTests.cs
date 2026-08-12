using System.Globalization;

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

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, pages: pages);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Pages.CreateWithPages(input, fileJob);
            }

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
}