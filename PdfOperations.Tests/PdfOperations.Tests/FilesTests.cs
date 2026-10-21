namespace PdfOperations.Tests;

[TestClass]
public class FilesTests
{
    [TestMethod]
    public void PrepareTempDirTest()
    {
        string tempDir = Files.PrepareTempDir();

        try
        {
            Assert.IsNotNull(tempDir);
            Assert.IsTrue(Directory.Exists(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
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

        try
        {
            string tempPath = Files.PrepareTempPath(operationContext.TempDir, operationInput.Output, operationDefinition.Extension);
        
            Assert.IsNotNull(tempPath);
            Assert.AreEqual("test_1.jpg", Path.GetFileName(tempPath));
            Assert.AreEqual("test_1", Path.GetFileNameWithoutExtension(tempPath));
            Assert.AreEqual(".jpg", Path.GetExtension(tempPath));
            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(tempPath)));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
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

        try
        {
            string tempPath = Files.PrepareTempPath(operationContext.TempDir, operationInput.Output, operationDefinition.Extension);
            string fileNotExt = Files.PrepareTempPathWithoutExt(tempPath);

            Assert.IsNotNull(fileNotExt);
            Assert.AreEqual("lipa", Path.GetFileName(fileNotExt));
            Assert.AreEqual("", Path.GetExtension(fileNotExt));
            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(fileNotExt)));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PrepareFinalPathTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            Dir = Files.PrepareTempDir(),
            Output = "lipa.pdf"
        };

        try
        {
            string finalPath = Files.PrepareFinalPath(operationInput.Dir, operationInput.Output);

            Assert.IsNotNull(finalPath);
            Assert.AreEqual("lipa.pdf", Path.GetFileName(finalPath));
            Assert.AreEqual("lipa", Path.GetFileNameWithoutExtension(finalPath));
            Assert.AreEqual(".pdf", Path.GetExtension(finalPath));
            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(finalPath)));
        }
        finally
        {
            if (Directory.Exists(operationInput.Dir))
                Directory.Delete(operationInput.Dir, true);
        }
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

        string tempDir = "";
        
        try
        {
            tempDir = Files.PrepareTempDir();
            string tempPath = Path.Combine(tempDir, "raport.txt");

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
            
            Assert.Contains("test_2.txt", text);
            Assert.Contains("Linia: 5", text);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void PrepareUniqueTempPathTest()
    {
        HashSet<string> usedOutputPaths = new(StringComparer.OrdinalIgnoreCase);
        string[] tempPaths = new string [3];
        
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf", "test_1.odg", "test_1.docx"])
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".jpg"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };

        try
        {
            int i = 0;

            foreach (string file in operationInput.InputFiles)
            {
                string tempPath = Files.PrepareUniqueTempPath(operationContext.TempDir, file,
                    operationDefinition.Extension, usedOutputPaths);

                Assert.IsNotNull(tempPath);
                Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(tempPath)));

                tempPaths[i] = tempPath;
                i++;
            }

            Assert.AreEqual("test_1.jpg", Path.GetFileName(tempPaths[0]));
            Assert.AreEqual("test_1", Path.GetFileNameWithoutExtension(tempPaths[0]));
            Assert.AreEqual(".jpg", Path.GetExtension(tempPaths[0]));

            Assert.AreEqual("test_1_1.jpg", Path.GetFileName(tempPaths[1]));
            Assert.AreEqual("test_1_1", Path.GetFileNameWithoutExtension(tempPaths[1]));
            Assert.AreEqual(".jpg", Path.GetExtension(tempPaths[1]));

            Assert.AreEqual("test_1_2.jpg", Path.GetFileName(tempPaths[2]));
            Assert.AreEqual("test_1_2", Path.GetFileNameWithoutExtension(tempPaths[2]));
            Assert.AreEqual(".jpg", Path.GetExtension(tempPaths[2]));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
}