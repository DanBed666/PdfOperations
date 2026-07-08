namespace PdfOperations.Tests;

[TestClass]
public class SearchTests
{
    [TestMethod]
    public void FindTextTest()
    {
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");

        try
        {
            File.WriteAllLines(path, ["test", "HYDRAULIKA", "cos tam", "hYdRaUliKa ggggggg"]);
            List<List<string>> lista = Search.SearchNewTxt(path, "hydraulika");
            Assert.HasCount(2, lista);

            foreach (List<string> lst in lista)
            {
                Assert.AreEqual("HYDRAULIKA", lst[0]);
                Assert.AreEqual("hYdRaUliKa ggggggg", lst[1]);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            File.Delete(path);
        }
    }
}