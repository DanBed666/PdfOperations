namespace PdfOperations.Tests;

[TestClass]
public class ValidationTests8
{
    [TestMethod]
    public void NormalizeExtensionTest()
    {
        string extension = InputValidator.NormalizeExtension(".pdf");
        Assert.AreEqual("pdf", extension);
    }
    
    [TestMethod]
    public void NormalizePagesTest()
    {
        string pages = InputValidator.NormalizePages("2 - 3");
        string pages2 = InputValidator.NormalizePages(" 4-6 ");
        Assert.AreEqual("2-3", pages);
        Assert.AreEqual("4-6", pages2);
    }
    
    [TestMethod]
    public void IsKnownExtensionTest()
    {
        bool isKnown = InputValidator.IsKnownExtension("docx");
        bool isKnown2 = InputValidator.IsKnownExtension("pdf");
        bool isKnown3 = InputValidator.IsKnownExtension("xxx");
        
        Assert.IsTrue(isKnown);
        Assert.IsTrue(isKnown2);
        Assert.IsFalse(isKnown3);
    }
    
    [TestMethod]
    public void IsExtensionValidForOpeTest()
    {
        string outputExtension = InputValidator.NormalizeExtension(Path.GetExtension("plik.pdf"));
        string outputExtension2 = InputValidator.NormalizeExtension(Path.GetExtension("plik.docx"));
        string opeExt = ".docx";
        
        bool isValid = InputValidator.IsExtensionValidForOpe(outputExtension, opeExt);
        bool isValid2 = InputValidator.IsExtensionValidForOpe(outputExtension2, opeExt);
        
        Assert.IsFalse(isValid);
        Assert.IsTrue(isValid2);
    }

    [TestMethod]
    public void IsPagesFormatValid()
    {
        bool isPagesFormatValid = InputValidator.IsPagesFormatValid("2-3");
        bool isPagesFormatValid2 = InputValidator.IsPagesFormatValid("2x-x3");
        bool isPagesFormatValid3 = InputValidator.IsPagesFormatValid("8-3");
        
        Assert.IsTrue(isPagesFormatValid);
        Assert.IsFalse(isPagesFormatValid2);
        Assert.IsFalse(isPagesFormatValid3);
    }

    [TestMethod]
    public void GetDefaultDirTest()
    {
        string defDir = InputValidator.GetDefaultDir();
        
        Assert.IsNotNull(defDir);
    }
    
    [TestMethod]
    public void SetDefaultOutputFileTest()
    {
        string defName = InputValidator.SetDefaultOutputFile("defaultName", ".pdf");
        
        Assert.IsNotNull(defName);
    }
    
    [TestMethod]
    public void BuildOutputExtTest()
    {
        string output = InputValidator.BuildOutputExt("kaszanka", ".docx");
        
        Assert.AreEqual("kaszanka.docx",  output);
        Assert.IsNotNull(output);
    }
    
    [TestMethod]
    public void CorrectOutputExtTest()
    {
        string output = InputValidator.CorrectOutputExt("kaszana.docx", ".pdf");
        
        Assert.AreEqual("kaszana.pdf",  output);
        Assert.IsNotNull(output);
    }
}