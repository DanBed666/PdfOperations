namespace PdfOperations.Tests;

[TestClass]
public class PagesTests
{
    [TestMethod]
    public void CreateWithPages()
    {
        string input = Path.Combine(AppContext.BaseDirectory, "TestData", "pdf_test.pdf");
        string pages = "2-3";
        string output = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
        
        try
        {
            Pages.CreateWithPages(input, pages, output);
            
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