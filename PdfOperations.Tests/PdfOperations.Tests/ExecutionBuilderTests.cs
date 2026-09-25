using System.Configuration;

namespace PdfOperations.Tests;

[TestClass]
public class ExecutionBuilderTests
{
    [TestMethod]
    public void SetFilesToFilesJobTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf", "test_2.pdf", "test_3.pdf"])
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };

        List<FileJob> fileJobs = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);

        foreach (FileJob fileJob in fileJobs)
        {
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(fileJob.TempPath));
        }

        Assert.IsTrue(Directory.Exists(operationContext.TempDir));
        Assert.AreEqual("test_1.pdf", Path.GetFileName(operationInput.InputFiles[0]));
        Assert.AreEqual("test_2.pdf", Path.GetFileName(operationInput.InputFiles[1]));
        Assert.AreEqual("test_3.pdf", Path.GetFileName(operationInput.InputFiles[2]));
        
        Assert.AreEqual("test_1.txt", Path.GetFileName(fileJobs[0].TempPath));
        Assert.AreEqual("test_2.txt", Path.GetFileName(fileJobs[1].TempPath));
        Assert.AreEqual("test_3.txt", Path.GetFileName(fileJobs[2].TempPath));
    }
    
    [TestMethod]
    public void SetFilesToFilesJobTestOne()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf"]),
            Output = "lemonade.txt"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };

        List<FileJob> fileJobs = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
        
        foreach (FileJob fileJob in fileJobs)
        {
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(fileJob.TempPath));
        }
        
        Assert.IsTrue(Directory.Exists(operationContext.TempDir));
        
        Assert.AreEqual("test_1.pdf", Path.GetFileName(operationInput.InputFiles[0]));
        Assert.AreEqual("lemonade.txt", operationInput.Output);
        Assert.AreEqual("lemonade.txt", Path.GetFileName(fileJobs[0].TempPath));
    }
    
    [TestMethod]
    public void SetFilesToSingleJobTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf", "test_2.pdf", "test_3.pdf"]),
            Output = "obraz.jpg"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".jpg"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };

        FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operationDefinition, operationInput, operationContext);
        Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(fileJob.TempPath));
        
        Assert.IsTrue(Directory.Exists(operationContext.TempDir));
        Assert.AreEqual("test_1.pdf", Path.GetFileName(operationInput.InputFiles[0]));
        Assert.AreEqual("test_2.pdf", Path.GetFileName(operationInput.InputFiles[1]));
        Assert.AreEqual("test_3.pdf", Path.GetFileName(operationInput.InputFiles[2]));
        
        Assert.AreEqual(operationInput.Output, Path.GetFileName(fileJob.TempPath));
    }
    
    [TestMethod]
    public void SetOperationContextTest()
    {
        OperationContext operationContext = ExecutionBuilder.SetOperationContext();
        Assert.IsTrue(Directory.Exists(operationContext.TempDir));
    }
}