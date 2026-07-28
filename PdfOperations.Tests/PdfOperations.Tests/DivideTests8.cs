namespace PdfOperations.Tests;

[TestClass]
public class DivideTests8
{
    [TestMethod]
    public void OneToMany()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        List<InputClass> inputFiles = new List<InputClass>();
        string inputPath = Path.Combine(dir, "ocr_test_1.pdf");
        string format = "%d.pdf";

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
                Divide.OneToMany(item);
            }
            
            foreach (string file in Directory.GetFiles(tempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(".pdf", Path.GetExtension(file));
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
    public void ManyToOne()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "TestData");
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        
        string inputPath = Path.Combine(dir, "ocr_test_1.pdf");
        string inputPath2 = Path.Combine(dir, "ocr_test_2.pdf");
        string inputPath3 = Path.Combine(dir, "ocr_test_3.pdf");
        string[] inputs = new[] { inputPath, inputPath2, inputPath3 };
        string format = ".pdf";

        InputClass input = new InputClass
        {
            inputFiles = inputs,
            outputFile = Path.Combine(tempDir, "output.pdf")
        };
        
        try
        {
            Divide.ManyToOne(input);

            Assert.IsTrue(File.Exists(input.outputFile));
            Assert.AreEqual(format, Path.GetExtension(input.outputFile));
            Assert.IsGreaterThan(0, new FileInfo(input.outputFile).Length);
            
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