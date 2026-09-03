using System.Runtime.ExceptionServices;

namespace PdfOperations.Tests;

[TestClass]
public class ConvertTests8()
{
    [TestMethod]
    public void FileToPdfTest()
    {
        string [] inputs = new [] {"word_1.docx", "word_2.docx", "word_3.docx"};
        string extension = "pdf";
        int count = 3;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, format: extension);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);

        try
        {
            Convert.FileToPdf(input, context);

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, CheckParams.NormalizeExtension(extension));
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToPictTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".jpg";
        int count = 12;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);
        
        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToPict(fileJob);
            }

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PdfToTxtTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".txt";
        int count = 3;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PdfToTxt(fileJob);
            }

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToTxtTest()
    {
        string [] inputs = new [] {"ocr_1.jpg", "ocr_2.jpg", "ocr_3.jpg"};
        string extension = ".txt";
        int count = 3;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.PictToTxt(fileJob);
            }

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PictToPdfTest()
    {
        string [] inputs = new [] {"ocr_1.jpg", "ocr_2.jpg", "ocr_3.jpg"};
        string extension = ".pdf";
        string output = "final.pdf";
        int count = 1;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);
        
        FileJob fileJob = ExecutionBuilder.SetFileJob(input, context, operation);

        try
        {
            Convert.PictToPdf(fileJob);

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ExtractPictTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".jpg";
        int count = 12;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles);
        OperationContext context = TestHelper8.SetOperationContext();
        Directory.CreateDirectory(context.TempDir);
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                Convert.ExtractPict(fileJob);
            }

            foreach (string file in Directory.GetFiles(context.TempDir))
            {
                TestHelper8.AssertForOneFile(file, operation.Extension);
            }
            
            Assert.HasCount(count, Directory.GetFiles(context.TempDir));
        }
        finally
        {
            if (Directory.Exists(context.TempDir))
                Directory.Delete(context.TempDir, true);
        }
    }
}