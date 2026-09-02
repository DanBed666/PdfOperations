using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;

namespace PdfOperations.Tests;
using System.Linq;

[TestClass]
public class SearchTests
{
    [TestMethod]
    public void SearchTxtTest()
    {
        string [] inputs = new [] {"search_1.txt", "search_2.txt", "search_3.txt"};
        string format = ".txt";
        int count = 3;
        string phrase = "hydraulika";
        List<List<string>> allFound = new List<List<string>>();
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(format);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, phrase: phrase, before: -2, after: 2);
        OperationContext context = TestHelper8.SetOperationContext();
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);
        
        foreach (FileJob fileJob in fileJobList)
        {
            File.Copy(fileJob.InputFile, fileJob.TempPath);
        }
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                List<List<string>> result = Search.SearchNewTxt(fileJob.TempPath, input.PhraseToFind, input.Before, input.After);
                allFound.AddRange(result);
                Files8.SaveToFile(result, Path.Combine(context.TempDir, "output.txt"));
            }
    
            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, format);
            }

            Assert.IsTrue(allFound.Any(group =>
                    group.Any(line => line.Contains("hydraulika", StringComparison.OrdinalIgnoreCase))));

            Assert.HasCount(6, allFound);
            
            Assert.IsFalse(allFound.Any(group =>
                group.Any(line => line.Contains("hfiewhfuwef", StringComparison.OrdinalIgnoreCase))));

            Assert.HasCount(6, allFound);
            
            Assert.HasCount(4, Directory.GetFiles(context.TempDir));
        }
        finally
        {
             //if (Directory.Exists(context.TempDir))
                //Directory.Delete(context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void SearchPictureTest()
    {
        string [] inputs = new [] {"ocr_1.jpg", "ocr_2.jpg", "ocr_3.jpg"};
        string format = ".txt";
        int count = 3;
        string phrase = "testowy";
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(format);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, phrase: phrase, before: -2, after: 2);
        OperationContext context = TestHelper8.SetOperationContext();
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);
        
        foreach (FileJob fileJob in fileJobList)
        {
            Convert.PictToTxt(fileJob);
        }

        FileJob reportJob = new FileJob
        {
            TempPath = Path.Combine(context.TempDir, "raport.txt")
        };
        
        Search.SearchPicture(input, context, reportJob);
        
        Assert.IsTrue(File.Exists(reportJob.TempPath));
        Assert.IsGreaterThan(0, new FileInfo(reportJob.TempPath).Length);
        
        string text = File.ReadAllText(reportJob.TempPath);
        Assert.IsTrue(text.Contains(phrase, StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void SearchPdfTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string format = ".txt";
        int count = 3;
        string phrase = "testowy";
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(format);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, phrase: phrase, before: -2, after: 2);
        OperationContext context = TestHelper8.SetOperationContext();
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);
        
        foreach (FileJob fileJob in fileJobList)
        {
            Convert.PdfToTxt(fileJob);
        }
        
        FileJob reportJob = new FileJob
        {
            TempPath = Path.Combine(context.TempDir, "raport.txt")
        };
        
        Search.SearchPdf(input, context, reportJob);
        
        Assert.IsTrue(File.Exists(reportJob.TempPath));
        Assert.IsGreaterThan(0, new FileInfo(reportJob.TempPath).Length);
        
        string text = File.ReadAllText(reportJob.TempPath);
        Assert.IsTrue(text.Contains(phrase, StringComparison.OrdinalIgnoreCase));
    }
}