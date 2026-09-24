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
            InputFiles = TestHelper.SetInputPaths(new [] {"word_1.docx", "word_8.docx", "word_3.docx"})
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
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
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