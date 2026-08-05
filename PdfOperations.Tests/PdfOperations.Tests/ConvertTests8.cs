using System.Runtime.ExceptionServices;

namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests8()
{
    [TestMethod]
    public void FileToPdfTest()
    {
        //string fileName = "word.docx";
        string fileName = "test.pdf";
        string format = ".docx";
        //string format = ".pdf";
        int count = 3;
        
        InputClass testInput = IntegrationTestHelper.PrepareFilesLibre(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            Convert.FileToPdf(testInput);
            
            foreach (string file in Directory.GetFiles(tempDir))
            {
                if (Path.GetExtension(file) == ".odt")
                    File.Delete(file);
            }

            foreach (string file in Directory.GetFiles(tempDir))
            {
                IntegrationTestHelper.AssertForOneFile(file, format);
            }
            
            Assert.HasCount(count, Directory.GetFiles(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToPictTest()
    {
        string fileName = "ocr_test.pdf";
        string format = ".jpg";
        int count = 3;
        
        List<InputClass> testFiles = IntegrationTestHelper.PrepareFilesToFiles(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            foreach (InputClass item in testFiles)
            {
                Convert.PdfToPict(item);
            }

            foreach (string file in Directory.GetFiles(tempDir))
            {
                IntegrationTestHelper.AssertForOneFile(file, format);
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
    public void PdfToTxtTest()
    {
        string fileName = "test.pdf";
        string format = ".txt";
        int count = 3;
        
        List<InputClass> testFiles = IntegrationTestHelper.PrepareFilesToFiles(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            foreach (InputClass item in testFiles)
            {
                Convert.PdfToTxt(item);
            }

            foreach (string file in Directory.GetFiles(tempDir))
            {
                IntegrationTestHelper.AssertForOneFile(file, format);
            }
            
            Assert.HasCount(count, Directory.GetFiles(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToTxtTest()
    {
        string fileName = "ocr.jpg";
        string format = ".txt";
        int count = 3;
        
        List<InputClass> testFiles = IntegrationTestHelper.PrepareFilesToFiles(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            foreach (InputClass item in testFiles)
            {
                Convert.PictToTxt(item);
            }

            foreach (string file in Directory.GetFiles(tempDir))
            {
                IntegrationTestHelper.AssertForOneFile(file, format);
            }
            
            Assert.HasCount(count, Directory.GetFiles(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToPdfTest()
    {
        string fileName = "ocr.jpg";
        string format = ".pdf";
        int count = 3;
        
        InputClass testFile = IntegrationTestHelper.PrepareFilesToSingle(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            Convert.PictToPdf(testFile);

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
    public void ExtractPictTest()
    {
        string fileName = "ocr_test.pdf";
        string format = ".jpg";
        int count = 3;
        
        List<InputClass> testFiles = IntegrationTestHelper.PrepareFilesToFiles(fileName, format, count, out string temp);
        string tempDir = temp;
        
        try
        {
            foreach (InputClass item in testFiles)
            {
                Convert.ExtractPict(item);
            }

            foreach (string file in Directory.GetFiles(tempDir))
            {
                IntegrationTestHelper.AssertForOneFile(file, format);
            }
            
            Assert.HasCount(12, Directory.GetFiles(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}