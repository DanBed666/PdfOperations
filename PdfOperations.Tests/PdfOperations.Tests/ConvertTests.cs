namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests()
{
    [TestMethod]
    public void FileToPdfTest()
    {
        string input = Path.Combine(AppContext.BaseDirectory, "TestData", "word.docx");
        string input2 = Path.Combine(AppContext.BaseDirectory, "TestData", "word2.docx");
        string input3 = Path.Combine(AppContext.BaseDirectory, "TestData", "word3.docx");

        string [] inputFiles = new []{input, input2, input3};
        string [] outputFiles = new string[inputFiles.LongLength];

        for (int i = 0; i < inputFiles.Length; i++)
        {
            outputFiles[i] = inputFiles[i].Replace(".docx", ".pdf");
        }

        try
        {
            Convert.FileToPdf(inputFiles);
            Assert.HasCount(3, outputFiles);

            for (int i = 0; i < outputFiles.Length; i++)
            {
                Assert.IsTrue(File.Exists(outputFiles[i]));
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
            for (int i = 0; i < outputFiles.Length; i++)
            {
                File.Delete(outputFiles[i]);
            }
        }
    }
    
    [TestMethod]
    public void PdfToPictTest(string input, string output)
    {
        //Convert.PdfToPict();
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
    public void PictToPdfTest(string [] input, string output)
    {
        //Convert.PictToPdf();
    }
}