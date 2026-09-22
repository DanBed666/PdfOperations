namespace PdfOperations.Tests;

[TestClass]
public class FilesTests
{
    [TestMethod]
    public void PrepareTempDirTest()
    {
        string tempDir = Files.PrepareTempDir();
    }
    
    [TestMethod]
    public void PrepareTempPathTest()
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

        Files.PrepareTempPath(operationContext.TempDir, operationInput.Output, operationDefinition.Extension);
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
    }
    
    [TestMethod]
    public void SaveToFileTest()
    {
        //Files8.SaveToFile();
    }
}