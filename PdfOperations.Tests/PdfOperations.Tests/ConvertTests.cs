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
    public void FileToPdfTestSingle()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["word_1.docx"]),
            Format = "pdf",
            Output = "word_1.pdf"
        };

        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        try
        {
            Convert.FileToPdf(operationInput, operationContext);

            Assert.IsTrue(File.Exists(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.IsGreaterThan(0, new FileInfo(Path.Combine(operationContext.TempDir, operationInput.Output)).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
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

        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
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
    public void PdfToDocxTestSingle()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf"]),
            Output = "word.docx"
        };

        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".docx"
        };
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToDocx(fileJob);
            }

            Assert.IsTrue(File.Exists(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.IsGreaterThan(0, new FileInfo(Path.Combine(operationContext.TempDir, operationInput.Output)).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
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
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
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
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
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
    public void PdfToTxtTestSingle()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_1.pdf"]),
            Output = "text.txt"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToTxt(fileJob);
            }

            Assert.IsTrue(File.Exists(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.IsGreaterThan(0, new FileInfo(Path.Combine(operationContext.TempDir, operationInput.Output)).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
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
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
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
    public void PictToTxtTestSingle()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_1.jpg"]),
            Output = "test.txt"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PictToTxt(fileJob);
            }

            Assert.IsTrue(File.Exists(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.IsGreaterThan(0, new FileInfo(Path.Combine(operationContext.TempDir, operationInput.Output)).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
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
        
        try
        {
            FileJob fileJob = ExecutionBuilder.SetFileJobFilesToSingle(operationDefinition, operationInput, operationContext);
            Convert.PictToPdf(fileJob);

            Assert.IsTrue(File.Exists(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(Path.Combine(operationContext.TempDir, operationInput.Output)));
            Assert.IsGreaterThan(0, new FileInfo(Path.Combine(operationContext.TempDir, operationInput.Output)).Length);
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
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
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
    
    [TestMethod]
    public void DocxToPdfTest()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["word_1.docx", "word_2.docx", "word_3.docx"]),
        };

        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".pdf"
        };

        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
            foreach (FileJob fileJob in fileJobList)
            {
                Assert.IsTrue(File.Exists(fileJob.InputFile));
                Convert.DocxToPdfWord(fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
                byte[] header = File.ReadAllBytes(file).Take(4).ToArray();
                CollectionAssert.AreEqual(new byte[] { 37, 80, 68, 70 }, header);
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
    public void DocxToPdfTestSingle()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["word_1.docx"]),
            Output = "newpdf.pdf"
        };

        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".pdf"
        };
        
        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
            foreach (FileJob fileJob in fileJobList)
            {
                Assert.IsTrue(File.Exists(fileJob.InputFile));
                Convert.DocxToPdfWord(fileJob);
            }

            string file = Path.Combine(operationContext.TempDir, operationInput.Output);
            
            Assert.IsTrue(File.Exists(file));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
            Assert.IsGreaterThan(0, new FileInfo(file).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
            byte[] header = File.ReadAllBytes(file).Take(4).ToArray();
            CollectionAssert.AreEqual(new byte[] { 37, 80, 68, 70 }, header);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ValidatePdfOutputTest()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Convert.ValidatePdfOutput(TestHelper.SetInputPaths(["test_1.pdx"])[0]);
        });
        
        Assert.Throws<InvalidOperationException>(() =>
        {
            Convert.ValidatePdfOutput(TestHelper.SetInputPaths(["empty.pdf"])[0]);
        });
        
        Assert.Throws<InvalidOperationException>(() =>
        {
            Convert.ValidatePdfOutput(TestHelper.SetInputPaths(["invalid_header.pdf"])[0]);
        });
        
        Assert.Throws<InvalidOperationException>(() =>
        {
            Convert.ValidatePdfOutput(TestHelper.SetInputPaths(["short_header.pdf"])[0]);
        });
        
        Convert.ValidatePdfOutput(TestHelper.SetInputPaths(["valid.pdf"])[0]);
    }
}