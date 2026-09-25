namespace PdfOperations.Tests;

[TestClass]
public class FilesTests
{
    [TestMethod]
    public void PrepareTempDirTest()
    {
        string tempDir = Files.PrepareTempDir();
        
        Assert.IsNotNull(tempDir);
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
        Assert.AreEqual("test_1.jpg", Path.GetFileName(tempPath));
        Assert.AreEqual("test_1", Path.GetFileNameWithoutExtension(tempPath));
        Assert.AreEqual(".jpg", Path.GetExtension(tempPath));
        Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(tempPath)));
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

        Assert.IsNotNull(fileNotExt);
        Assert.AreEqual("lipa", Path.GetFileName(fileNotExt));
        Assert.AreEqual("", Path.GetExtension(fileNotExt));
        Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(fileNotExt)));
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
        Assert.AreEqual("lipa.pdf", Path.GetFileName(finalPath));
        Assert.AreEqual("lipa", Path.GetFileNameWithoutExtension(finalPath));
        Assert.AreEqual(".pdf", Path.GetExtension(finalPath));
        Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(finalPath)));
    }
    
    [TestMethod]
    public void SaveToFileTest()
    {
        List<SearchResult> searchResults = new List<SearchResult>
        {
            new SearchResult
            {
                FilePath = "test_1.txt",
                PageNumber = 1,
                LineNumber = 1,
                Occurences = 2,
                Lines = new List<string>
                {
                    "test_1.txt",
                    "Strona: 1",
                    "Linia: 1",
                    "Wystąpienia: 2",
                    "HYDRAULIKA hydraulika",
                    "-----------------------------"
                }
            },
            new SearchResult
            {
                FilePath = "test_2.txt",
                PageNumber = 2,
                LineNumber = 5,
                Occurences = 1,
                Lines = new List<string>
                {
                    "test_2.txt",
                    "Strona: 2",
                    "Linia: 5",
                    "Wystąpienia: 1",
                    "hydraulika",
                    "--------------------------"
                }
            }
        };

        string tempPath = Path.Combine(Files.PrepareTempDir(), "raport.txt");

        foreach (SearchResult searchResult in searchResults)
        {
            Files.SaveToFile(searchResult, tempPath);
        }
        
        Assert.IsTrue(File.Exists(tempPath));
        Assert.AreEqual("raport.txt", Path.GetFileName(tempPath));
        Assert.AreEqual("raport", Path.GetFileNameWithoutExtension(tempPath));
        Assert.AreEqual(".txt", Path.GetExtension(tempPath));
        Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(tempPath)));
        Assert.IsGreaterThan(0, new FileInfo(tempPath).Length);
        
        string text = File.ReadAllText(tempPath);
        Assert.Contains("test_1.txt", text);
        Assert.Contains("hydraulika", text);
        Assert.Contains("Linia", text);
    }
}