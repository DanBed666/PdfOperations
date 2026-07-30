namespace PdfOperations.Tests;

[TestClass]
public class DivideTests8
{
    [TestMethod]
    public void OneToMany()
    {
        string fileName = "ocr_test.pdf";
        string format = ".pdf";
        int count = 3;
        
        List<InputClass> testFiles = TestHelper.PrepareFilesToFiles(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            foreach (InputClass item in testFiles)
            {
                Divide.OneToMany(item);
            }

            foreach (string file in Directory.GetFiles(tempDir))
            {
                TestHelper.AssertForOneFile(file, format);
            }
            
            Assert.HasCount(12, Directory.GetFiles(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void ManyToOne()
    {
        string fileName = "ocr_test.pdf";
        string format = ".pdf";
        int count = 3;
        
        InputClass testFile = TestHelper.PrepareFilesToSingle(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            Divide.ManyToOne(testFile);

            foreach (string file in Directory.GetFiles(tempDir))
            {
                TestHelper.AssertForOneFile(file, format);
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