namespace PdfOperations.Tests;

[TestClass]
public class DivideTests
{
    [TestMethod]
    public void OneToMany()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"])
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
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobsFilesToFiles(operationDefinition, operationInput, operationContext);
            
            foreach (FileJob fileJob in fileJobList)
            {
                Divide.OneToMany(fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(12, Directory.GetFiles(operationContext.TempDir));

            for (int document = 1; document <= 3; document++)
            {
                for (int page = 1; page <= 4; page++)
                {
                    string outputFile = Path.Combine(operationContext.TempDir,
                        $"ocr_test_{document}_{page}.pdf");
                    Assert.IsTrue(File.Exists(outputFile));
                    Assert.AreEqual(1, Info.GetPdfPagesSingle(outputFile));
                    string[] texts = TestHelper.ReadPdfPageTexts(outputFile);
                    Assert.HasCount(1, texts);
                    Assert.Contains($"OCR TEST PDF {document} - selectable text page {page}", texts[0]);
                }
            }
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ManyToOne()
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["test_3.pdf", "test_1.pdf", "test_2.pdf"]),
            Output =  "final.pdf"
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
            
            Divide.ManyToOne(fileJob);

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
            }
            
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));

            Assert.AreEqual(3, Info.GetPdfPagesSingle(fileJob.TempPath));
            string[] pages = TestHelper.ReadPdfPageTexts(fileJob.TempPath);
            Assert.HasCount(3, pages);
            Assert.Contains("TEST PDF 3", pages[0]);
            Assert.Contains("TEST PDF 1", pages[1]);
            Assert.Contains("TEST PDF 2", pages[2]);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
}
