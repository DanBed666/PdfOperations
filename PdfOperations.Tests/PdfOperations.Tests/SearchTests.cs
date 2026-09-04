namespace PdfOperations.Tests;
using System.Linq;

[TestClass]
public class SearchTests
{
    [TestMethod]
    public void SearchTxtTest()
    {
        string [] inputs = new [] {"search_1.txt", "search_2.txt", "search_3.txt"};
        string extension = ".txt";
        int count = 3;
        string phrase = "hydraulika";
        List<List<string>> allFound = new List<List<string>>();

        TestInput testInput = TestHelper.PrepareMultipleInputsSearch(inputs, phrase, extension, -2, 2);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);
        
        foreach (FileJob fileJob in fileJobList)
        {
            File.Copy(fileJob.InputFile, fileJob.TempPath);
        }
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                string originalFile = Files.FindOriginalFileForTemp(fileJob.TempPath, fileJob.InputFiles);
                List<List<string>> result = Search.SearchNewTxt(fileJob.TempPath, originalFile, testInput.Input.PhraseToFind, 
                    testInput.Input.Before, testInput.Input.After);
                allFound.AddRange(result);
                Files.SaveToFile(result, Path.Combine(testInput.Context.TempDir, "output.txt"));
            }
    
            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, extension);
            }

            Assert.IsTrue(allFound.Any(group =>
                    group.Any(line => line.Contains("hydraulika", StringComparison.OrdinalIgnoreCase))));

            Assert.HasCount(6, allFound);
            
            Assert.IsFalse(allFound.Any(group =>
                group.Any(line => line.Contains("hfiewhfuwef", StringComparison.OrdinalIgnoreCase))));

            Assert.HasCount(6, allFound);
            
            Assert.HasCount(4, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
             if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void SearchPictureTest()
    {
        string [] inputs = new [] {"ocr_1.jpg", "ocr_2.jpg", "ocr_3.jpg"};
        string extension = ".txt";
        int count = 3;
        string phrase = "testowy";
        
        TestInput testInput = TestHelper.PrepareMultipleInputsSearch(inputs, phrase, extension, -2, 2);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);
        
        foreach (FileJob fileJob in fileJobList)
        {
            Convert.PictToTxt(fileJob);
        }

        FileJob reportJob = new FileJob
        {
            TempPath = Path.Combine(testInput.Context.TempDir, "raport.txt")
        };
        
        Search.SearchPicture(testInput.Input, testInput.Context, reportJob);
        
        Assert.IsTrue(File.Exists(reportJob.TempPath));
        Assert.IsGreaterThan(0, new FileInfo(reportJob.TempPath).Length);
        
        string text = File.ReadAllText(reportJob.TempPath);
        Assert.IsTrue(text.Contains(phrase, StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void SearchPdfTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".txt";
        int count = 3;
        string phrase = "testowy";
        
        TestInput testInput = TestHelper.PrepareMultipleInputsSearch(inputs, phrase, extension, -2, 2);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);
        
        foreach (FileJob fileJob in fileJobList)
        {
            Convert.PdfToTxt(fileJob);
        }
        
        FileJob reportJob = new FileJob
        {
            TempPath = Path.Combine(testInput.Context.TempDir, "raport.txt")
        };
        
        Search.SearchPdf(testInput.Input, testInput.Context, reportJob);
        
        Assert.IsTrue(File.Exists(reportJob.TempPath));
        Assert.IsGreaterThan(0, new FileInfo(reportJob.TempPath).Length);
        
        string text = File.ReadAllText(reportJob.TempPath);
        Assert.IsTrue(text.Contains(phrase, StringComparison.OrdinalIgnoreCase));
    }
}