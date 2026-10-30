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

        Dictionary<string, string> readTxts = new Dictionary<string, string>();
        
        try
        {
            Convert.FileToPdf(operationInput, operationContext);

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
                readTxts[Path.GetFileName(file)] = TestHelper.ReadPdfText(file);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));

            Assert.Contains("WORD_1", readTxts["word_1.pdf"]);
            Assert.Contains("Page 1", readTxts["word_1.pdf"]);
            Assert.Contains("dokumentu", readTxts["word_1.pdf"]);
            Assert.Contains("WORD_2", readTxts["word_2.pdf"]);
            Assert.Contains("automated", readTxts["word_2.pdf"]);
            Assert.Contains("zawiera", readTxts["word_2.pdf"]);
            Assert.Contains("WORD_3", readTxts["word_3.pdf"]);
            Assert.Contains("tests", readTxts["word_3.pdf"]);
            Assert.Contains("strukture", readTxts["word_3.pdf"]);
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
            string file = Path.Combine(operationContext.TempDir, operationInput.Output);
            
            Assert.IsTrue(File.Exists(file));
            Assert.IsGreaterThan(0, new FileInfo(file).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
            
            string text = TestHelper.ReadPdfText(file);

            Assert.Contains("WORD_1", text);
            Assert.Contains("akapitow", text);
            Assert.Contains("fixture", text);
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
        
        Dictionary<string, string> readTxts = new Dictionary<string, string>();

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
                readTxts[Path.GetFileName(file)] = TestHelper.ReadDocxText(file);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
            
            Assert.Contains("TEST PDF 1", readTxts["test_1.docx"]);
            Assert.Contains("testowy", readTxts["test_1.docx"]);
            Assert.Contains("Linia 3", readTxts["test_1.docx"]);
            Assert.Contains("PDF 2", readTxts["test_2.docx"]);
            Assert.Contains("zaznaczalny", readTxts["test_2.docx"]);
            Assert.Contains("laczenia", readTxts["test_2.docx"]);
            Assert.Contains("TEST PDF 3", readTxts["test_3.docx"]);
            Assert.Contains("Linia 2", readTxts["test_3.docx"]);
            Assert.Contains("tresci", readTxts["test_3.docx"]);
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

            string file = Path.Combine(operationContext.TempDir, operationInput.Output);

            Assert.IsTrue(File.Exists(file));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
            Assert.IsGreaterThan(0, new FileInfo(file).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));

            string text = TestHelper.ReadDocxText(file);
            Assert.Contains("TEST PDF 1", text);
            Assert.Contains("Linia 3", text);
            Assert.Contains("Koniec neutralnej tresci testowej", text);
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

        Dictionary<string, string> readTxts = new Dictionary<string, string>();
        
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

                readTxts[Path.GetFileName(file)] = TestHelper.ReadTextText(file);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
            
            Assert.Contains("PDF 1", readTxts["test_1.txt"]);
            Assert.Contains("zwyklych", readTxts["test_1.txt"]);
            Assert.Contains("kontrolny", readTxts["test_1.txt"]);
            Assert.Contains("PDF 2", readTxts["test_2.txt"]);
            Assert.Contains("laczenia", readTxts["test_2.txt"]);
            Assert.Contains("testow", readTxts["test_2.txt"]);
            Assert.Contains("PDF 3", readTxts["test_3.txt"]);
            Assert.Contains("Koniec", readTxts["test_3.txt"]);
            Assert.Contains("testowej", readTxts["test_3.txt"]);
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

            string file = Path.Combine(operationContext.TempDir, operationInput.Output);
            string text = TestHelper.ReadTextText(file);

            Assert.IsTrue(File.Exists(file));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
            Assert.IsGreaterThan(0, new FileInfo(file).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
            
            Assert.Contains("PDF 1", text);
            Assert.Contains("neutralnej", text);
            Assert.Contains("wyszukiwania", text);
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
        
        Dictionary<string, string> readTxts = new Dictionary<string, string>();
        
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
                
                readTxts[Path.GetFileName(file)] = TestHelper.ReadTextText(file);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
            
            Assert.Contains("OCR IMAGE 1", readTxts["ocr_1.txt"]);
            Assert.Contains("wejsciowy", readTxts["ocr_1.txt"]);
            Assert.Contains("neutralny", readTxts["ocr_1.txt"]);
            Assert.Contains("OCR IMAGE 2", readTxts["ocr_2.txt"]);
            Assert.Contains("duzy", readTxts["ocr_2.txt"]);
            Assert.Contains("czytelny", readTxts["ocr_2.txt"]);
            Assert.Contains("OCR IMAGE 3", readTxts["ocr_3.txt"]);
            Assert.Contains("obrazie", readTxts["ocr_3.txt"]);
            Assert.Contains("clean", readTxts["ocr_3.txt"]);
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

            string file = Path.Combine(operationContext.TempDir, operationInput.Output);
            string text = TestHelper.ReadTextText(file);

            Assert.IsTrue(File.Exists(file));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
            Assert.IsGreaterThan(0, new FileInfo(file).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
            
            Assert.Contains("OCR IMAGE 1", text);
            Assert.Contains("HYDRAULIKA", text);
            Assert.Contains("czytelny", text);
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
            string file = Path.Combine(operationContext.TempDir, operationInput.Output);

            Assert.IsTrue(File.Exists(file));
            Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
            Assert.IsGreaterThan(0, new FileInfo(file).Length);
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
            Assert.AreEqual(3, Info.GetPdfPagesSingle(file));

            // This PDF contains images, so render its pages before running OCR.
            string imagesDir = Path.Combine(operationContext.TempDir, "rendered");
            Directory.CreateDirectory(imagesDir);
            Convert.PdfToPict(new FileJob
            {
                InputFile = file,
                TempPath = Path.Combine(imagesDir, "page.jpg")
            });

            Assert.HasCount(3, Directory.GetFiles(imagesDir, "*.jpg"));

            for (int page = 1; page <= 3; page++)
            {
                FileJob imageJob = new()
                {
                    InputFile = Path.Combine(imagesDir, $"page-{page}.jpg"),
                    TempPath = Path.Combine(imagesDir, $"page-{page}.txt")
                };
                Convert.PictToTxt(imageJob);

                string text = TestHelper.ReadTextText(imageJob.TempPath);
                Assert.Contains($"OCR IMAGE {page}", text);
                Assert.Contains("HYDRAULIKA", text);
            }
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
            Extension = ""
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        string[] allowedExtensions =
        [
            ".jpg", ".png", ".tif", ".jp2",
            ".jb2e", ".jb2g", ".ccitt", ".params"
        ];
        
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
                Assert.Contains(Path.GetExtension(file).ToLowerInvariant(), allowedExtensions);
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

                string text = TestHelper.ReadPdfText(file);
                string marker = Path.GetFileNameWithoutExtension(file).ToUpperInvariant();
                Assert.Contains(marker, text);
                Assert.Contains("Page 1", text);
                Assert.Contains("fixture", text);
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

            string text = TestHelper.ReadPdfText(file);
            Assert.Contains("WORD_1", text);
            Assert.Contains("akapitow", text);
            Assert.Contains("fixture", text);
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
