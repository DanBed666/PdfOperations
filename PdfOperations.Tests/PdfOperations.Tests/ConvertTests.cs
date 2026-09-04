namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests()
{
    [TestMethod]
    public void FileToPdfTest()
    {
        string [] inputs = new [] {"word_1.docx", "word_2.docx", "word_3.docx"};
        string extension = ".pdf";
        int count = 3;

        TestInput testInput = TestHelper.PrepareMultipleInputsFormat(inputs, extension);
        
        try
        {
            Convert.FileToPdf(testInput.Input, testInput.Context);

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, CheckParams.NormalizeExtension(extension));
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToPictTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".jpg";
        int count = 12;

        TestInput testInput = TestHelper.PrepareMultipleInputs(inputs, extension);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToPict(fileJob);
            }

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToTxtTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".txt";
        int count = 3;

        TestInput testInput = TestHelper.PrepareMultipleInputs(inputs, extension);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToTxt(fileJob);
            }

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToTxtTest()
    {
        string [] inputs = new [] {"ocr_1.jpg", "ocr_2.jpg", "ocr_3.jpg"};
        string extension = ".txt";
        int count = 3;

        TestInput testInput = TestHelper.PrepareMultipleInputs(inputs, extension);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PictToTxt(fileJob);
            }

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToPdfTest()
    {
        string [] inputs = new [] {"ocr_1.jpg", "ocr_2.jpg", "ocr_3.jpg"};
        string extension = ".pdf";
        string output = "final.pdf";
        int count = 1;

        TestInput testInput = TestHelper.PrepareInputWithOutputFormat(inputs, extension, output);
        FileJob fileJob = ExecutionBuilder.SetFileJob(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            Convert.PictToPdf(fileJob);

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ExtractPictTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".jpg";
        int count = 12;

        TestInput testInput = TestHelper.PrepareMultipleInputs(inputs, extension);
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.ExtractPict(fileJob);
            }

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                TestHelper.AssertForOneFile(file, testInput.Operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(testInput.Context.TempDir));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
}