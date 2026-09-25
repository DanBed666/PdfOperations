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
        
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(new [] {"word_search_1.docx", "word_search_2.docx", "word_search_3.docx"}),
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
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Replacement.ReplaceTextWithPlaceholders(fileJob, operationInput, operationContext);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                //Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
            Assert.HasCount(3, Directory.GetDirectories(operationContext.TempDir));

            foreach (string dir in Directory.GetDirectories(operationContext.TempDir))
            {
                string file = Path.Combine(dir, "word", "document.xml");
                text += File.ReadAllText(file);
            }
            
            Assert.Contains("[PLACEHOLDER]", text);
            Assert.AreEqual(21, text.Split("[PLACEHOLDER]").Length - 1);
            Assert.HasCount(21, Regex.Matches(text, Regex.Escape("[PLACEHOLDER]")));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
}