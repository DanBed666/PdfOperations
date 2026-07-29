using System.Globalization;

namespace PdfOperations.Tests;

[TestClass]
public class PagesTests
{
    [TestMethod]
    public void CreateWithPages()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        List<InputClass> inputFiles = new List<InputClass>();
        string inputPath = Path.Combine(dir, "ocr_test_1.pdf");
        string format = ".pdf";

        InputClass input = new InputClass
        {
            inputFile = inputPath,
            phrase = "2-3",
            tempPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputPath = Path.Combine(dir, "ocr_test_2.pdf");
        
        InputClass input2 = new InputClass
        {
            inputFile = inputPath,
            phrase = "2-3",
            tempPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputPath = Path.Combine(dir, "ocr_test_3.pdf");
        
        InputClass input3 = new InputClass
        {
            inputFile = inputPath,
            phrase = "2-3",
            tempPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputFiles.AddRange([input, input2, input3]);

        try
        {
            foreach (InputClass item in inputFiles)
            {
                Pages.CreateWithPages(item);
            }
            
            foreach (string file in Directory.GetFiles(tempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(format, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(tempDir));
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}