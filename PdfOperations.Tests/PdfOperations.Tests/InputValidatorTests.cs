namespace PdfOperations.Tests;

[TestClass]
public class ValidationTests
{
    [TestMethod]
    public void CheckFileFormatTest()
    {
        string output = "test2.pdf";
        bool check = CheckParams.IsFormatValid(Path.GetExtension(output));
        
        string output2 = "test2.hhh";
        bool check2 = CheckParams.IsFormatValid(Path.GetExtension(output2));
        
        string output3 = "aaaaaa";
        bool check3 = CheckParams.IsFormatValid(Path.GetExtension(output3));
        
        Assert.AreEqual(".pdf", Path.GetExtension(output));
        Assert.IsTrue(check);
        Assert.IsFalse(check2);
        Assert.IsFalse(check3);
    }
    
    [TestMethod]
    public void IsValidPageFormatTest()
    {
        string pages = "2-5";
        bool check = CheckParams.IsValidPageFormat(pages);
        
        string pages2 = "1,3-6";
        bool check2 = CheckParams.IsValidPageFormat(pages2);
        
        string pages3 = "5-2";
        bool check3 = CheckParams.IsValidPageFormat(pages3);
        
        string pages4 = "xxxx8-9yyyy";
        bool check4 = CheckParams.IsValidPageFormat(pages4);

        Assert.IsTrue(check);
        Assert.IsTrue(check2);
        Assert.IsFalse(check3);
        Assert.IsFalse(check4);
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
    public void NormalizeExtensionTest()
    {
        string extension = ".pdf";
        string extension2 = "jpg";
        string res = CheckParams.NormalizeExtension(extension);
        string res2 = CheckParams.NormalizeExtension(extension2);
        
        Assert.AreEqual(".pdf", res);
        Assert.AreEqual(".jpg", res2);
    }
    
    [TestMethod]
    public void TryPrepareOutputTest()
    {
        string [] inputs = new [] {"test_1.pdf", "test_2.pdf", "test_3.pdf"};
        string extension = ".jpg";
        string output = "filename";

        TestInput testInput = TestHelper.PrepareInputWithOutputFormat(inputs, extension, output);
        CheckParams.TryPrepareOutput(testInput.Operation, testInput.Input, testInput.Input.Output);
        
        Assert.AreEqual("filename.jpg", testInput.Input.Output);
    }
    
    [TestMethod]
    public void GetEffectiveExtensionTest()
    {
        string [] inputs = new [] {"test_1.pdf"};
        string extension = "";
        string output = "filename";

        TestInput testInput = TestHelper.PrepareInputWithOutputFormat(inputs, extension, output);
        //testInput.Input.Format = ".xml";
        string ext = CheckParams.GetEffectiveExtension(testInput.Operation.Extension, testInput.Input);
        Assert.AreEqual(".pdf", ext);
    }
}