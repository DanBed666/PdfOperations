namespace PdfOperations.Tests;

[TestClass]
public class InfoTests
{
    [TestMethod]
    public void ShowInfoTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf", "test_2.pdf", "test_3.pdf"]),
            Output = "final.txt"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operationDefinition, operationInput, operationContext);

        try
        {
            Info.ShowInfo(fileJob);

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ShowFontInfoTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf", "test_2.pdf", "test_3.pdf"]),
            Output = "final.txt"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operationDefinition, operationInput, operationContext);

        try
        {
            Info.ShowFontInfo(fileJob);

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void SaveInfoTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["search_1.txt", "search_2.txt", "search_3.txt"]),
            Output = "final.txt"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operationDefinition, operationInput, operationContext);

        try
        {
            Info.SaveToFile(fileJob.TempPath, "losowy xdd\n", operationInput.InputFiles);

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }

    [TestMethod]
    public void GetPdfPagesTest()
    {
        string [] inputs = new[] { "ocr_test_1.pdf", "test_2.pdf" };
        string[] inputPaths = TestHelper.SetInputPaths(inputs);

        Assert.AreEqual(4, Info.GetPdfPagesSingle(inputPaths[0]));
        Assert.AreEqual(1, Info.GetPdfPagesSingle(inputPaths[1]));
    }
}