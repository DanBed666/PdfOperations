namespace PdfOperations.Tests;

[TestClass]
public class DivideTests
{
    [TestMethod]
    public void OneToMany()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".pdf";
        int count = 3;

        TestInput testInput = TestHelper.PrepareMultipleInputs(inputs, extension);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Divide.OneToMany(fileJob);
            }

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
    public void ManyToOne()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".pdf";
        string output = "final.pdf";
        int count = 1;

        TestInput testInput = TestHelper.PrepareInputWithOutputFormat(inputs, extension, output);
        FileJob fileJob = ExecutionBuilder.SetFileJob(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            Divide.ManyToOne(fileJob);

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