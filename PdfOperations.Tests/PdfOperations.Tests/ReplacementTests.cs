using System.IO.Compression;
using System.Text.RegularExpressions;

namespace PdfOperations.Tests;

[TestClass]
public class ReplacementTests
{
    [TestMethod]
    public void ReplaceTextWithPlaceholdersTest()
    {
        string [] inputs = new [] {"word_1.docx", "word_8.docx", "word_3.docx"};
        string plcFile = TestHelper.SetFilePath("plc.txt");
        int count = 3;
        string text = "";

        TestInput testInput = TestHelper.PrepareMultipleInputsReplacement(inputs, plcFile);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Replacement.ReplaceTextWithPlaceholders(fileJob, testInput.Input, testInput.Context);
            }

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, Path.GetExtension(file));
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
            Assert.HasCount(count, Directory.GetDirectories(testInput.Context.TempDir));

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                string dir = Path.Combine(testInput.Context.TempDir, $"extract_{Path.GetFileNameWithoutExtension(file)}");

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
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
}