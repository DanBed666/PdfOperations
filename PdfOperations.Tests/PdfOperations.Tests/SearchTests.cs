namespace PdfOperations.Tests;
using System.Linq;

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
                foreach (string f in Directory.GetFiles(tempDir))
                {
                    Search.SearchNewTxt(f, file);
                }
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
    
    [TestMethod]
    public void SearchTxtTest()
    {
        string fileName = "search.txt";
        string format = ".txt";
        int count = 3;
        string phrase = "hydraulika";
        List<List<string>> found = new List<List<string>>();
        List<List<string>> allFound = new List<List<string>>();
            
        List<InputClass> testFiles = TestHelper.PrepareFilesToFiles(fileName, format, count, out string temp, phrase, -2, 2);
        string tempDir = temp;
        
        foreach (InputClass file in testFiles)
        {
            File.Copy(file.inputFile, file.tempPath);
        }
        
        try
        {
            foreach (InputClass file in testFiles)
            {
                foreach (string f in Directory.GetFiles(tempDir))
                {
                    List<List<string>> result = Search.SearchNewTxt(f, file);
                    allFound.AddRange(result);
                    Files.SaveToFile(Search.SearchNewTxt(f, file), Path.Combine(tempDir, "output.txt"));
                }
            }
    
            foreach (string file in Directory.GetFiles(tempDir))
            {
                TestHelper.AssertForOneFile(file, format);
            }

            Assert.IsTrue(allFound.Any(group =>
                    group.Any(line => line.Contains("hydraulika", StringComparison.OrdinalIgnoreCase))));
            
            //Assert.AreEqual(15, found.Sum(x => x.Count));
            Assert.AreEqual(2, allFound.Sum(group => group.Count(line => line.Contains("hydraulika", StringComparison.OrdinalIgnoreCase))));
            
            Assert.HasCount(4, Directory.GetFiles(tempDir));
        }
        finally
        {
            //if (Directory.Exists(tempDir))
                //Directory.Delete(tempDir, true);
        }    
    }
}