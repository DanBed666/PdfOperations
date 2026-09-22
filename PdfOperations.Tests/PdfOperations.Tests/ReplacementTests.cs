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
            InputFiles = new [] {"word_1.docx", "word_8.docx", "word_3.docx"},
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files8.PrepareTempDir()
        };
        
        List<FileJob> fileJobList = ExecutionBuilder8.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Replacement.ReplaceTextWithPlaceholders(fileJob, operationInput, operationContext);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                TestHelper.AssertForOneFile(file, Path.GetExtension(file));
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
            Assert.HasCount(3, Directory.GetDirectories(operationContext.TempDir));

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                string dir = Path.Combine(operationContext.TempDir, $"extract_{Path.GetFileNameWithoutExtension(file)}");

                ZipFile.ExtractToDirectory(file, dir);
                string path = Path.Combine(dir, "word", "document.xml");
                text += File.ReadAllText(path);
            }
            
            Assert.Contains("[MENTOS]", text);
            Assert.AreEqual(8, text.Split("[MENTOS]").Length - 1);
            Assert.HasCount(8, Regex.Matches(text, Regex.Escape("[MENTOS]")));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
}