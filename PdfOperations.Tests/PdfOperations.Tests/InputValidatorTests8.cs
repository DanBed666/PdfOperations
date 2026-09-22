namespace PdfOperations.Tests;

[TestClass]
public class ValidationTests8
{
    [TestMethod]
    public static void NormalizeExtensionTest()
    {
        string extension = InputValidator.NormalizeExtension(".pdf");
    }
    
    [TestMethod]
    public static void NormalizePagesTest()
    {
        string pages = InputValidator.NormalizePages("2-3");
    }
    
    [TestMethod]
    public static void IsKnownExtensionTest()
    {
        bool isKnown = InputValidator.IsKnownExtension(".docx");
    }
    
    [TestMethod]
    public static void IsExtensionValidForOpeTest()
    {
        string outputExtension = Path.GetExtension("plik.pdf");
        bool isValid = InputValidator.IsExtensionValidForOpe(outputExtension, ".pdf");
    }

    [TestMethod]
    public static void IsPagesFormatValid()
    {
        bool isPagesFormatValid = InputValidator.IsPagesFormatValid("2-3");
    }

    [TestMethod]
    public static void GetDefaultDirTest()
    {
        string defDir = InputValidator.GetDefaultDir();
    }
    
    [TestMethod]
    public static void SetDefaultOutputFileTest()
    {
        string defName = InputValidator.SetDefaultOutputFile("defaultName", ".pdf");
    }
    
    [TestMethod]
    public static void BuildOutputExtTest()
    {
        string output = InputValidator.BuildOutputExt("kaszanka", ".docx");
    }
    
    [TestMethod]
    public static void CorrectOutputExtTest()
    {
        string output = InputValidator.CorrectOutputExt("kaszana.docx", ".pdf");
    }
}