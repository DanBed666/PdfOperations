namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests()
{
    [TestMethod]
    public void FileToPdfTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["word_1.docx", "word_2.docx", "word_3.docx"]),
            Format = "pdf"
        };

        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        try
        {
            Convert.FileToPdf(operationInput, operationContext);

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToDocxTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf", "test_2.pdf", "test_3.pdf"]),
        };

        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".docx"
        };
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToDocx(fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToPictTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"])
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".jpg"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToPict(fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(12, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToTxtTest()
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

        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToTxt(fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToTxtTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_1.jpg", "ocr_2.jpg", "ocr_3.jpg"])
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PictToTxt(fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToPdfTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_1.jpg", "ocr_2.jpg", "ocr_3.jpg"]),
            Output = "final.pdf"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".pdf"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operationDefinition, operationInput, operationContext);

        try
        {
            Convert.PictToPdf(fileJob);

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
    public void ExtractPictTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"])
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".jpg"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.ExtractPict(fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(12, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
}