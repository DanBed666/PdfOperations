using System.Runtime.ExceptionServices;

namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests()
{
    [TestMethod]
    public void FileToPdfTest()
    {
        string format = ".docx";
        string formatEnd = "pdf";
        string input = Path.Combine(AppContext.BaseDirectory, "TestData", $"word{format}");
        string input2 = Path.Combine(AppContext.BaseDirectory, "TestData", $"word2{format}");
        string input3 = Path.Combine(AppContext.BaseDirectory, "TestData", $"word3{format}");

        string [] inputFiles = new []{input, input2, input3};
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        string [] outputFiles = new string[inputFiles.LongLength];

        for (int i = 0; i < inputFiles.Length; i++)
        {
            outputFiles[i] = Path.Combine(dir, Path.GetFileNameWithoutExtension(inputFiles[i]) + ".pdf");
        }

        try
        {
            Convert.FileToPdf(inputFiles, formatEnd, dir);
            Assert.HasCount(3, outputFiles);

            for (int i = 0; i < outputFiles.Length; i++)
            {
                Assert.IsTrue(File.Exists(outputFiles[i]));
                Assert.IsTrue(Path.GetExtension(outputFiles[i]).Equals(".pdf", StringComparison.OrdinalIgnoreCase));
                Assert.IsGreaterThan(0, new FileInfo(outputFiles[i]).Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }
    
    [TestMethod]
    public void PdfToPictTest()
    {
        string key = "klucz";
        string input = Path.Combine(AppContext.BaseDirectory, "TestData", "pdf_test.pdf");
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        string output = Path.Combine(dir, key);
        
        try
        {
            Convert.PdfToPict(input, output);
            
            string [] outputFiles = Directory.GetFiles(dir, $"*{key}*");
            Assert.HasCount(6, outputFiles);

            for (int i = 1; i <= outputFiles.Length; i++)
            {
                string outputFile = Path.Combine(dir, $"{key}-{i}.jpg");
                
                Assert.IsTrue(File.Exists(outputFile));
                Assert.IsGreaterThan(0, new FileInfo(outputFile).Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }
    
    [TestMethod]
    public void PdfToTxtTest()
    {
        string input = Path.Combine(AppContext.BaseDirectory, "TestData", "kompy.pdf");
        string output = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");

        try
        {
            Convert.PdfToTxt(input, output);
            Assert.IsTrue(File.Exists(output));
            Assert.IsGreaterThan(0, new FileInfo(output).Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            File.Delete(output);
        }
    }
    
    [TestMethod]
    public void PictToTxtTest()
    {
        string input = Path.Combine(AppContext.BaseDirectory, "TestData", "obraz.png");
        string output = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}");
        
        string outputTest = output + ".txt";

        try
        {
            Convert.PictToTxt(input, output);
            Assert.IsTrue(File.Exists(outputTest));
            Assert.IsGreaterThan(0, new FileInfo(outputTest).Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            File.Delete(output);
        }
    }
    
    [TestMethod]
    public void PictToPdfTest()
    {
        string input = Path.Combine(AppContext.BaseDirectory, "TestData", "strona1.png");
        string input2 = Path.Combine(AppContext.BaseDirectory, "TestData", "strona2.png");
        string input3 = Path.Combine(AppContext.BaseDirectory, "TestData", "strona3.png");

        string[] inputFiles = new string[] { input, input2, input3 };
        string output = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
        
        try
        {
            Convert.PictToPdf(inputFiles, output);
            
            Assert.IsTrue(File.Exists(output));
            Assert.IsGreaterThan(0, new FileInfo(output).Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            File.Delete(output);
        }
    }
}