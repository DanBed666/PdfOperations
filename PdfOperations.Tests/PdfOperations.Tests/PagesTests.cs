namespace PdfOperations.Tests;

[TestClass]
public class PagesTests
{
    [TestMethod]
    public void CreateWithPagesTest()
    {
        int suma = 0;

        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"]),
            Pages = "2-3"
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
                Pages.CreateWithPages(operationInput, fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
                Assert.AreEqual(2, Info.GetPdfPagesSingle(file));
                suma += Info.GetPdfPagesSingle(file);
            }
            
            Assert.HasCount(3, Directory.GetFiles(operationContext.TempDir));
            Assert.AreEqual(6, suma);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void CreateWithPagesTestSingle()
    {
        int suma = 0;

        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["ocr_test_1.pdf"]),
            Pages = "2-3",
            Output = "end.pdf"
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
                Pages.CreateWithPages(operationInput, fileJob);
            }

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
                Assert.AreEqual(2, Info.GetPdfPagesSingle(file));
                suma += Info.GetPdfPagesSingle(file);
            }
            
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
            Assert.AreEqual(2, suma);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
    
    [TestMethod]
    public void CreateWithPagesCustomTest()
    {
        List<PdfFragment> pdfFragments = new List<PdfFragment>();

        PdfFragment pdfFragment = new PdfFragment()
        {
            FileName = TestHelper.SetInputPath("ocr_test_1.pdf"),
            PageNumbers = "2-3"
        };
        
        PdfFragment pdfFragment2 = new PdfFragment()
        {
            FileName = TestHelper.SetInputPath("ocr_test_2.pdf"),
            PageNumbers = "1,4"
        };
        
        PdfFragment pdfFragment3 = new PdfFragment()
        {
            FileName = TestHelper.SetInputPath("ocr_test_3.pdf"),
            PageNumbers = "1,3-4"
        };
        
        pdfFragments.Add(pdfFragment);
        pdfFragments.Add(pdfFragment2);
        pdfFragments.Add(pdfFragment3);

        OperationInput operationInput = new OperationInput()
        {
            PdfFragments = pdfFragments,
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
            Pages.CreateWithCustomFiles(operationInput, fileJob);

            foreach (string file in Directory.GetFiles(operationContext.TempDir))
            {
                Assert.IsTrue(File.Exists(file));
                Assert.AreEqual(operationDefinition.Extension, Path.GetExtension(file));
                Assert.IsGreaterThan(0, new FileInfo(file).Length);
                Assert.AreEqual(7, Info.GetPdfPagesSingle(file));
            }
            
            Assert.HasCount(1, Directory.GetFiles(operationContext.TempDir));
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
        }
    }
}