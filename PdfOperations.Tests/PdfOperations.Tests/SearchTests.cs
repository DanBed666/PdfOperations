namespace PdfOperations.Tests;

[TestClass]
public class SearchTests
{
    [TestMethod]
    public void FindTextTest()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        string file1 = Path.Combine(tempDir, "plik.txt");
        File.WriteAllLines(file1, new[]{"test", "HYDRAULIKA", "cos tam", "hYdRaUliKa ggggggg"});
        string file2 = Path.Combine(tempDir, "plik2.txt");
        File.WriteAllLines(file2, new[]{"test", "HYDRAULIKA", "cos tam", "hYdRaUliKa ggggggg"});
        string file3 = Path.Combine(tempDir, "plik3.txt");
        File.WriteAllLines(file3, new[]{"test", "HYDRAULIKA", "cos tam", "hYdRaUliKa ggggggg"});
        
        List<InputClass> inputFiles = new List<InputClass>();
        List<List<string>> found = new List<List<string>>();

        InputClass input = new InputClass
        {
            tempPath = file1,
            phrase = "hydraulika",
            before = -2,
            after = 2
        };
        
        InputClass input2 = new InputClass
        {
            tempPath = file2,
            phrase = "hydraulika",
            before = -2,
            after = 2
        };
        
        InputClass input3 = new InputClass
        {
            tempPath = file3,
            phrase = "hydraulika",
            before = -2,
            after = 2
        };
        
        inputFiles.AddRange([input, input2, input3]);

        try
        {
            foreach (InputClass file in inputFiles)
            {
                found = Search.SearchNewTxt(file);
            }
            
            Assert.HasCount(2, found);

            foreach (List<string> lst in found)
            {
                CollectionAssert.Contains(lst, "HYDRAULIKA");
                CollectionAssert.Contains(lst, "hYdRaUliKa ggggggg");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            File.Delete(file1);
            File.Delete(file2);
            File.Delete(file3);
        }
    }
}