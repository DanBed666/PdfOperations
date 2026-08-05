namespace PdfOperations.Tests;

[TestClass]
public class InfoTests
{
    [TestMethod]
    public void ShowInfoTest()
    {
        string fileName = "ocr_test.pdf";
        string format = ".txt";
        int count = 3;
        
        InputClass testFile = IntegrationTestHelper.PrepareFilesToSingle(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            Info.ShowInfo(testFile);

            foreach (string file in Directory.GetFiles(tempDir))
            {
                IntegrationTestHelper.AssertForOneFile(file, format);
            }
            
            Assert.HasCount(1, Directory.GetFiles(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void ShowFontInfoTest()
    {
        string fileName = "test.pdf";
        string format = ".txt";
        int count = 3;
        
        InputClass testFile = IntegrationTestHelper.PrepareFilesToSingle(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            Info.ShowFontInfo(testFile);

            foreach (string file in Directory.GetFiles(tempDir))
            {
                IntegrationTestHelper.AssertForOneFile(file, format);
            }
            
            Assert.HasCount(1, Directory.GetFiles(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}