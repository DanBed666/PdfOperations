namespace PdfOperations.Tests;

[TestClass]
public class DivideTests
{
    [TestMethod]
    public void OneToMany()
    {
        string input = Path.Combine(AppContext.BaseDirectory, "TestData", "pdf_test.pdf");
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        string output = Path.Combine(dir, $"test8-%d.pdf");

        try
        {
            Divide.OneToMany(input, output);

            string [] outputFiles = Directory.GetFiles(dir, "test8-*.pdf");
            Assert.HasCount(6, outputFiles);

            for (int page = 1; page <= 6; page++)
            {
                string outputFile = Path.Combine(dir, $"test8-{page}.pdf");
                
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
        
        //Divide.OneToMany();
    }
    
    [TestMethod]
    public void ManyToOne(string [] input, string output)
    {
        //Divide.ManyToOne();
    }
}