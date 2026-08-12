namespace PdfOperations.Tests;
using System.Linq;

[TestClass]
public class SearchTests
{
    /*
    [TestMethod]
    public void SearchTxtTest()
    {
        string fileName = "search.txt";
        string format = ".txt";
        int count = 3;
        string phrase = "hydraulika";
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
                List<List<string>> result = Search.SearchNewTxt(file.tempPath, file);
                allFound.AddRange(result);
                Files.SaveToFile(result, Path.Combine(tempDir, "output.txt"));
            }
    
            foreach (string file in Directory.GetFiles(tempDir))
            {
                TestHelper.AssertForOneFile(file, format);
            }

            Assert.IsTrue(allFound.Any(group =>
                    group.Any(line => line.Contains("hydraulika", StringComparison.OrdinalIgnoreCase))));

            Assert.HasCount(6, allFound);
            
            Assert.IsFalse(allFound.Any(group =>
                group.Any(line => line.Contains("hfiewhfuwef", StringComparison.OrdinalIgnoreCase))));

            Assert.HasCount(6, allFound);
            
            Assert.HasCount(4, Directory.GetFiles(tempDir));
        }
        finally
        {
             if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }   
        
    }
    */
}