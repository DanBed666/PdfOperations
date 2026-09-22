namespace PdfOperations.Tests;

[TestClass]
public class ValidationTests8
{
    [TestMethod]
    public void NormalizeExtensionTest()
    {
        string extension = InputValidator.NormalizeExtension(".pdf");
    }
    
    [TestMethod]
    public void NormalizePagesTest()
    {
        string pages = InputValidator.NormalizePages("2-3");
    }
    
    [TestMethod]
    public void IsKnownExtensionTest()
    {
        bool isKnown = InputValidator.IsKnownExtension(".docx");
    }
    
    [TestMethod]
    public void IsExtensionValidForOpeTest()
    {
        string outputExtension = Path.GetExtension("plik.pdf");
        bool isValid = InputValidator.IsExtensionValidForOpe(outputExtension, ".pdf");
    }

    [TestMethod]
    public void IsPagesFormatValid()
    {
        bool isPagesFormatValid = InputValidator.IsPagesFormatValid("2-3");
    }

    [TestMethod]
    public void GetDefaultDirTest()
    {
        string defDir = InputValidator.GetDefaultDir();
    }
    
    [TestMethod]
    public void SetDefaultOutputFileTest()
    {
        string defName = InputValidator.SetDefaultOutputFile("defaultName", ".pdf");
    }
    
    [TestMethod]
    public void BuildOutputExtTest()
    {
        string output = InputValidator.BuildOutputExt("kaszanka", ".docx");
    }
    
    [TestMethod]
    public void CorrectOutputExtTest()
    {
        string output = InputValidator.CorrectOutputExt("kaszana.docx", ".pdf");
    }
}