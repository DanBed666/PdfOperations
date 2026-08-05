using System.Globalization;

namespace PdfOperations.Tests;

[TestClass]
public class PagesTests
{
    [TestMethod]
    public void CreateWithPages()
    {
        string fileName = "ocr_test.pdf";
        string format = ".pdf";
        int count = 3;
        string pages = "2-3";
        
        List<InputClass> testFiles = IntegrationTestHelper.PrepareFilesToFiles(fileName, format, count, out string temp, pages);
        string tempDir = temp;
        
        try
        {
            foreach (InputClass item in testFiles)
            {
                Pages.CreateWithPages(item);
            }

            foreach (string file in Directory.GetFiles(tempDir))
            {
                IntegrationTestHelper.AssertForOneFile(file, format);
            }
            
            Assert.HasCount(3, Directory.GetFiles(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}