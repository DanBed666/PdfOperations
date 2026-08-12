namespace PdfOperations.Tests;

[TestClass]
public class InfoTests
{
    [TestMethod]
    public void ShowInfoTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".txt";
        string output = "final.pdf";
        int count = 1;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);
        
        FileJob fileJob = ExecutionBuilder.SetFileJob(input, context);

        try
        {
            Info.ShowInfo(fileJob);

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ShowFontInfoTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".txt";
        string output = "final.pdf";
        int count = 1;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);
        
        FileJob fileJob = ExecutionBuilder.SetFileJob(input, context);

        try
        {
            Info.ShowFontInfo(fileJob);

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
}