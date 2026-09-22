namespace PdfOperations.Tests;

[TestClass]
public class FilesTests
{
    [TestMethod]
    public void PrepareTempDirTest()
    {
        string tempDir = Files8.PrepareTempDir();
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
            TempDir = Files8.PrepareTempDir()
        };

        Files8.PrepareTempPath(operationContext.TempDir, operationInput.Output, operationDefinition.Extension);
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
            TempDir = Files8.PrepareTempDir()
        };

        string tempPath = Files8.PrepareTempPath(operationContext.TempDir, operationInput.Output, operationDefinition.Extension);
        string fileNotExt = Files8.PrepareTempPathWithoutExt(tempPath);
    }
    
    [TestMethod]
    public void PrepareFinalPathTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            Dir = Files8.PrepareTempDir(),
            Output = "lipa.pdf"
        };
        
        string finalPath = Files8.PrepareFinalPath(operationInput.Dir, operationInput.Output);
    }
    
    [TestMethod]
    public void SaveToFileTest()
    {
        //Files8.SaveToFile();
    }
}