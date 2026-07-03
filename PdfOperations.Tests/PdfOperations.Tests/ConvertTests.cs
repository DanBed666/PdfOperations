namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests()
{
    [TestMethod]
    public void FileToPdfTest(string [] files)
    {
        Convert.FileToPdf();
    }
    
    [TestMethod]
    public void PdfToPictTest(string input, string output)
    {
        Convert.PdfToPict();
    }
    
    [TestMethod]
    public void PdfToTxtTest(string input, string output)
    {
        Convert.PdfToTxt();
    }
    
    [TestMethod]
    public void PictToTxtTest(string input, string output)
    {
        Convert.PictToTxt();
    }
    
    [TestMethod]
    public void PictToPdfTest(string [] input, string output)
    {
        Convert.PictToPdf();
    }
}