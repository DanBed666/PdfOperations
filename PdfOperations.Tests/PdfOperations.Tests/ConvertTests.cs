namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests()
{
    [TestMethod]
    public void FileToPdfTest(string [] files)
    {
        //Convert.FileToPdf();
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
    public void PictToTxtTest(string input, string output)
    {
        //Convert.PictToTxt();
    }
    
    [TestMethod]
    public void PictToPdfTest(string [] input, string output)
    {
        //Convert.PictToPdf();
    }
}