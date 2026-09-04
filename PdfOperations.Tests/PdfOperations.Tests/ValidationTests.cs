namespace PdfOperations.Tests;

[TestClass]
public class ValidationTests
{
    [TestMethod]
    public void CheckFileFormatTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.pdf";

        string[] inputFiles = TestHelper.SetInputPaths(inputs);
        OperationInput input = TestHelper.SetOperationInput(inputFiles, output: output);
        bool check = CheckParams.CheckFileFormat(input.Output, out string format);
        
        Assert.AreEqual(".pdf", format);
        Assert.IsTrue(check);
    }
    
    [TestMethod]
    public void CheckFormatTest()
    {
        Assert.IsTrue(CheckParams.CheckFormat("pdf"));
        Assert.IsTrue(CheckParams.CheckFormat("jpg"));
        Assert.IsFalse(CheckParams.CheckFormat("xdd"));
    }
    
    [TestMethod]
    public void FixFormatExistTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.pdf";
        string extension = ".jpg";
        string finalOut = "test2.jpg";

        TestInput testInput = TestHelper.PrepareMultiplePathsWithoutContext(inputs, output, extension);

        CheckParams.FixFormatExist(testInput.Operation.Extension, testInput.Input, testInput.Input.Output);
        Assert.AreEqual(finalOut, testInput.Input.Output);
    }
    
    [TestMethod]
    public void FixFormatNotExistTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2";
        string extension = ".pdf";
        string finalOut = "test2.pdf";

        TestInput testInput = TestHelper.PrepareMultiplePathsWithoutContext(inputs, output, extension);

        CheckParams.FixFormatNotExist(testInput.Operation.Extension, testInput.Input, testInput.Input.Output);
        Assert.AreEqual(finalOut, testInput.Input.Output);
    }
    
    [TestMethod]
    public void NormalizeExtensionTest()
    {
        string extension = ".pdf";
        string extension2 = "jpg";
        string res = CheckParams.NormalizeExtension(extension);
        string res2 = CheckParams.NormalizeExtension(extension2);
        
        Assert.AreEqual(".pdf", res);
        Assert.AreEqual(".jpg", res2);
    }
}