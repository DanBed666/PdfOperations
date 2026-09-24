namespace PdfOperations.Tests;

[TestClass]
public class FilesTests
{
    [TestMethod]
    public void PrepareTempDirTest()
    {
        string tempDir = Files.PrepareTempDir();
        Assert.IsTrue(Directory.Exists(tempDir));
    }
    
    [TestMethod]
    public void PrepareTempPathTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            Output = "test_1.pdf"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".jpg"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };

        string tempPath = Files.PrepareTempPath(operationContext.TempDir, operationInput.Output, operationDefinition.Extension);
        
        Assert.IsNotNull(tempPath);
    }
    
    [TestMethod]
    public void PrepareTempPathWithoutExtTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            Output = "lipa.pdf"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".jpg"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };

        string tempPath = Files.PrepareTempPath(operationContext.TempDir, operationInput.Output, operationDefinition.Extension);
        string fileNotExt = Files.PrepareTempPathWithoutExt(tempPath);
        
        Assert.IsNotNull(tempPath);
        Assert.IsNotNull(fileNotExt);
    }
    
    [TestMethod]
    public void PrepareFinalPathTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            Dir = Files.PrepareTempDir(),
            Output = "lipa.pdf"
        };
        
        string finalPath = Files.PrepareFinalPath(operationInput.Dir, operationInput.Output);
        
        Assert.IsNotNull(finalPath);
    }
    
    [TestMethod]
    public void SaveToFileTest()
    {
        //Files8.SaveToFile();
    }
}