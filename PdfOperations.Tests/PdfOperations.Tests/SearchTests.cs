namespace PdfOperations.Tests;
using System.Linq;

[TestClass]
public class SearchTests
{
    [TestMethod]
    public void SearchTxtTest()
    {
        SearchResult searchResult = new SearchResult();
        List<SearchResult> searchResults = new List<SearchResult>();

        OperationInput operationInput = new OperationInput()
        {
            InputFiles = new [] {"search_1.txt", "search_2.txt", "search_3.txt"},
            Output = "lipa.pdf",
            PhraseToFind = "hydraulika",
            Before = 2,
            After = 2
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };

        try
        {
            foreach (string f in Directory.GetFiles(operationContext.TempDir))
            {
                searchResult = Search.GetFoundLines(f, operationInput.PhraseToFind, operationInput.Before,
                    operationInput.After);
                searchResults.Add(searchResult);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }

            Assert.IsTrue(searchResults.Any(group =>
                    group.Lines.Any(line => line.Contains("hydraulika", StringComparison.OrdinalIgnoreCase))));

            Assert.HasCount(6, searchResults);
            
            Assert.IsFalse(searchResults.Any(group =>
                group.Lines.Any(line => line.Contains("hfiewhfuwef", StringComparison.OrdinalIgnoreCase))));

            Assert.HasCount(6, searchResults);
            
            Assert.HasCount(4, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
             if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void SearchPdfTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"},
            Output = "raport.txt",
            PhraseToFind = "testowy",
            Before = 2,
            After = 2
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
        
        foreach (FileJob fileJob in fileJobList)
        {
            Convert.PdfToTxt(fileJob);
        }
        
        FileJob reportJob = new FileJob
        {
            TempPath = Path.Combine(operationContext.TempDir, operationInput.Output)
        };
        
        Search.SearchTempTextFiles(operationInput, operationContext);
        
        Assert.IsTrue(File.Exists(reportJob.TempPath));
        Assert.IsGreaterThan(0, new FileInfo(reportJob.TempPath).Length);
        
        string text = File.ReadAllText(reportJob.TempPath);
        Assert.IsTrue(text.Contains(operationInput.PhraseToFind, StringComparison.OrdinalIgnoreCase));
    }
}