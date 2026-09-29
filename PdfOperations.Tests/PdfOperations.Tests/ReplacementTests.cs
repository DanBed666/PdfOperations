using System.IO.Compression;
using System.Text.RegularExpressions;

namespace PdfOperations.Tests;

[TestClass]
public class ReplacementTests
{
    [TestMethod]
    public void ReplaceTextWithPlaceholdersTest()
    {
        string text = "";
        string fileXml = "";
        
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(new [] {"word_search_1.docx", "word_search_2.odg", "word_search_3.odt"}),
            PlaceholderFile = TestHelper.SetInputPath("plik.xlsx")
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ""
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
                Replacement.ReplaceTextWithPlaceholders(fileJob, operationInput, operationContext);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
            Assert.HasCount(3, Directory.GetDirectories(operationContext.TempDir));

            foreach (string dir in Directory.GetDirectories(operationContext.TempDir))
            {
                if (Directory.Exists(Path.Combine(dir, "word")))
                    fileXml = Path.Combine(dir, "word", "document.xml");
                else
                    fileXml = Path.Combine(dir, "content.xml");
                
                text += File.ReadAllText(fileXml);
            }
            
            Assert.Contains("[PLACEHOLDER]", text);
            Assert.AreEqual(21, text.Split("[PLACEHOLDER]").Length - 1);
            Assert.HasCount(21, Regex.Matches(text, Regex.Escape("[PLACEHOLDER]")));
            
            string text_docx = File.ReadAllText(Path.Combine(operationContext.TempDir, "word_search_1", "word", "document.xml"));
            string text_odg = File.ReadAllText(Path.Combine(operationContext.TempDir, "word_search_2", "content.xml"));
            string text_odt = File.ReadAllText(Path.Combine(operationContext.TempDir, "word_search_3", "content.xml"));
            
            Assert.HasCount(6, Regex.Matches(text_docx, Regex.Escape("[PLACEHOLDER]")));
            Assert.HasCount(6, Regex.Matches(text_odg, Regex.Escape("[PLACEHOLDER]")));
            Assert.HasCount(9, Regex.Matches(text_odt, Regex.Escape("[PLACEHOLDER]")));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ReplaceTextWithPlaceholdersExcelTest()
    {
        string text = "";
        
        OperationInput operationInput = new OperationInput()
        {
            PlaceholderFile = TestHelper.SetInputPath("plik.xlsx")
        };

        List<ReplacementPair> replacementPairs = Replacement.ReadReplacementsFromExcel(operationInput.PlaceholderFile);

        Assert.AreEqual("strona", replacementPairs[0].Find);
        Assert.AreEqual("[REDACTED]", replacementPairs[1].Replace);
        Assert.AreEqual("False", replacementPairs[2].IgnoreCase.ToString());
    }
}