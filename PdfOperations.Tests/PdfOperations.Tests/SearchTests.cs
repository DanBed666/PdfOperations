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
            InputFiles = TestHelper.SetInputPaths(new [] {"search_1.txt", "search_2.txt", "search_3.txt"}),
            Output = "lipa.txt",
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
            List<FileJob> fileJobs = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
            foreach (string file in operationInput.InputFiles)
            {
                File.Copy(file, Path.Combine(operationContext.TempDir, Path.GetFileName(file)));
            }
            
            foreach (FileJob fileJob in fileJobs)
            {
                searchResult = Search.GetFoundLines(fileJob.TempPath, operationInput.PhraseToFind, operationInput.Before,
                    operationInput.After, fileJob.InputFile);
                searchResults.Add(searchResult);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }

            Assert.IsTrue(searchResults.Any(result =>
                    result.Lines.Any(line => line.Contains(operationInput.PhraseToFind, StringComparison.OrdinalIgnoreCase))));

            int occurences = searchResults.Sum(result => result.Occurences);
            Assert.AreEqual(8, occurences);

            List<SearchResult> searchResults2 = new List<SearchResult>();
            operationInput.PhraseToFind = "welcome";

            foreach (FileJob fileJob in fileJobs)
            {
                searchResult = Search.GetFoundLines(fileJob.TempPath, operationInput.PhraseToFind, operationInput.Before,
                    operationInput.After, fileJob.InputFile);
                searchResults2.Add(searchResult);
            }
            
            Assert.IsFalse(searchResults2.Any(result =>
                result.Lines.Any(line => line.Contains(operationInput.PhraseToFind, StringComparison.OrdinalIgnoreCase))));

            int occurences2 = searchResults2.Sum(result => result.Occurences);
            Assert.AreEqual(0, occurences2);
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
            InputFiles = TestHelper.SetInputPaths(new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"}),
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
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToTxt(fileJob);
            }

            FileJob reportJob = new FileJob
            {
                TempPath = Path.Combine(operationContext.TempDir, operationInput.Output)
            };

            foreach (FileJob fileJob in fileJobList)
            {
                Search.SearchTempTextFiles(operationInput, operationContext, fileJob);
            }

            Assert.IsTrue(File.Exists(reportJob.TempPath));
            Assert.IsGreaterThan(0, new FileInfo(reportJob.TempPath).Length);

            string text = File.ReadAllText(reportJob.TempPath);
            Assert.IsTrue(text.Contains(operationInput.PhraseToFind, StringComparison.OrdinalIgnoreCase));

            foreach (string file in operationInput.InputFiles)
            {
                Assert.Contains(file, text);
            }
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
}