namespace PdfOperations.Tests;

[TestClass]
public class InfoTests
{
    [TestMethod]
    public void ShowInfoTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".txt";
        string output = "final.txt";
        int count = 1;

        TestInput testInput = TestHelper.PrepareInputWithOutputFormat(inputs, extension, output);
        FileJob fileJob = ExecutionBuilder.SetFileJob(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            Info.ShowInfo(fileJob);

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ShowFontInfoTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".txt";
        string output = "final.txt";
        int count = 1;

        TestInput testInput = TestHelper.PrepareInputWithOutputFormat(inputs, extension, output);
        FileJob fileJob = ExecutionBuilder.SetFileJob(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            Info.ShowFontInfo(fileJob);

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void SaveInfoTest()
    {
        string[] inputs = new[] { "search_1.txt", "search_2.txt", "search_3.txt" };
        string extension = ".txt";
        string output = "final.txt";
        int count = 1;

        TestInput testInput = TestHelper.PrepareInputWithOutputFormat(inputs, extension, output);
        FileJob fileJob = ExecutionBuilder.SetFileJob(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            Info.SaveToFile(fileJob.TempPath, "losowy xdd\n", testInput.Input.InputFiles);

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
}