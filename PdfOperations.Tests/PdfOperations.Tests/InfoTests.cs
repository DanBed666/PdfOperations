namespace PdfOperations.Tests;

[TestClass]
public class InfoTests
{
    [TestMethod]
    public void ShowInfoTest()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        string inputPath = Path.Combine(dir, "test.pdf");
        string inputPath2 = Path.Combine(dir, "test2.pdf");
        string inputPath3 = Path.Combine(dir, "test3.pdf");
        string[] inputs = new[] { inputPath, inputPath2, inputPath3 };
        string format = ".txt";

        InputClass input = new InputClass
        {
            inputFiles = inputs,
            tempPath = Path.Combine(tempDir, "zapis.txt")
        };
        
        try
        {
            Info.ShowInfo(input);

            Assert.IsTrue(File.Exists(input.tempPath));
            Assert.AreEqual(format, Path.GetExtension(input.tempPath));
            Assert.IsGreaterThan(0, new FileInfo(input.tempPath).Length);
            
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
    
    [TestMethod]
    public void ShowFontInfoTest()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        string inputPath = Path.Combine(dir, "test.pdf");
        string inputPath2 = Path.Combine(dir, "test2.pdf");
        string inputPath3 = Path.Combine(dir, "test3.pdf");
        string[] inputs = new[] { inputPath, inputPath2, inputPath3 };
        string format = ".txt";

        InputClass input = new InputClass
        {
            inputFiles = inputs,
            tempPath = Path.Combine(tempDir, "zapis.txt")
        };
        
        try
        {
            Info.ShowFontInfo(input);

            Assert.IsTrue(File.Exists(input.tempPath));
            Assert.AreEqual(format, Path.GetExtension(input.tempPath));
            Assert.IsGreaterThan(0, new FileInfo(input.tempPath).Length);
            
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