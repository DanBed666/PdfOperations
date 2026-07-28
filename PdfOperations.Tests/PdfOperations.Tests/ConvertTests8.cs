using System.Runtime.ExceptionServices;

namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests8()
{
    [TestMethod]
    public void FileToPdfTest()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        string inputPath = Path.Combine(dir, "word.docx");
        string inputPath2 = Path.Combine(dir, "kompy.docx");
        string inputPath3 = Path.Combine(dir, "pdf_test.docx");
        string[] inputs = new[] { inputPath, inputPath2, inputPath3 };
        
        string format = ".pdf";

        InputClass input = new InputClass
        {
            inputFiles = inputs,
            phrase = "pdf",
            dir = tempDir
        };
        
        try
        {
            Convert.FileToPdf(input);
            Assert.HasCount(3, Directory.GetFiles(tempDir));

            foreach (string file in Directory.GetFiles(tempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(format, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToPictTest()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        List<InputClass> inputFiles = new List<InputClass>();
        string inputPath = Path.Combine(dir, "ocr_test_1.pdf");
        string format = ".jpg";

        InputClass input = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath))
        };
        
        inputPath = Path.Combine(dir, "ocr_test_2.pdf");
        
        InputClass input2 = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath))
        };
        
        inputPath = Path.Combine(dir, "ocr_test_3.pdf");
        
        InputClass input3 = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath))
        };
        
        inputFiles.AddRange([input, input2, input3]);

        try
        {
            foreach (InputClass item in inputFiles)
            {
                Convert.PdfToPict(item);
            }

            foreach (string file in Directory.GetFiles(tempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(format, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(12, Directory.GetFiles(tempDir));
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToTxtTest()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        List<InputClass> inputFiles = new List<InputClass>();
        string inputPath = Path.Combine(dir, "test.pdf");
        string format = ".txt";

        InputClass input = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputPath = Path.Combine(dir, "test2.pdf");
        
        InputClass input2 = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputPath = Path.Combine(dir, "test3.pdf");
        
        InputClass input3 = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputFiles.AddRange([input, input2, input3]);

        try
        {
            foreach (InputClass item in inputFiles)
            {
                Convert.PdfToTxt(item);
            }
            
            foreach (string file in Directory.GetFiles(tempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(format, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(tempDir));
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToTxtTest()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        List<InputClass> inputFiles = new List<InputClass>();
        string inputPath = Path.Combine(dir, "ocr1.jpg");
        string format = "";

        InputClass input = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputPath = Path.Combine(dir, "ocr2.jpg");
        
        InputClass input2 = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputPath = Path.Combine(dir, "ocr3.jpg");
        
        InputClass input3 = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputFiles.AddRange([input, input2, input3]);

        try
        {
            foreach (InputClass item in inputFiles)
            {
                Convert.PictToTxt(item);
            }
            
            foreach (string file in Directory.GetFiles(tempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(".txt", Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(3, Directory.GetFiles(tempDir));
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToPdfTest()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        string inputPath = Path.Combine(dir, "ocr1.jpg");
        string inputPath2 = Path.Combine(dir, "ocr2.jpg");
        string inputPath3 = Path.Combine(dir, "ocr3.jpg");
        string[] inputs = new[] { inputPath, inputPath2, inputPath3 };
        string format = ".pdf";

        InputClass input = new InputClass
        {
            inputFiles = inputs,
            outputPath = Path.Combine(tempDir, "output.pdf")
        };
        
        try
        {
            Convert.PictToPdf(input);

            Assert.IsTrue(File.Exists(input.outputPath));
            Assert.AreEqual(format, Path.GetExtension(input.outputPath));
            Assert.IsGreaterThan(0, new FileInfo(input.outputPath).Length);
            
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    
    [TestMethod]
    public void ExtractPictTest()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        List<InputClass> inputFiles = new List<InputClass>();
        string inputPath = Path.Combine(dir, "ocr_test_1.pdf");
        string format = ".jpg";

        InputClass input = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputPath = Path.Combine(dir, "ocr_test_2.pdf");
        
        InputClass input2 = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputPath = Path.Combine(dir, "ocr_test_3.pdf");
        
        InputClass input3 = new InputClass
        {
            inputFile = inputPath,
            outputPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + format)
        };
        
        inputFiles.AddRange([input, input2, input3]);

        try
        {
            foreach (InputClass item in inputFiles)
            {
                Convert.ExtractPict(item);
            }
            
            foreach (string file in Directory.GetFiles(tempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(format, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(12, Directory.GetFiles(tempDir));
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}