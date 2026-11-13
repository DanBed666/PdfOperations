namespace PdfOperations.Tests;

[TestClass]
public class VcRuntimeTermsTests
{
    [TestMethod]
    public void OnlyBundledLibraryWithoutConsentRequiresPrompt()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var library = Path.Combine(directory.FullName, "MSVCP140.dll");
            Assert.IsFalse(VcRuntimeTerms.RequiresConsent(library, () => throw new AssertFailedException()));
            File.WriteAllBytes(library, []);
            Assert.IsTrue(VcRuntimeTerms.RequiresConsent(library, () => false));
            Assert.IsFalse(VcRuntimeTerms.RequiresConsent(library, () => true));
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [TestMethod]
    [DataRow(true, "tak", true)]
    [DataRow(false, "YES", true)]
    [DataRow(true, "", false)]
    [DataRow(false, "no", false)]
    [DataRow(true, "yes", false)]
    public void ConsoleShowsLocalizedTermsAndRequiresExplicitAnswer(bool polish, string answer, bool expected)
    {
        using var output = new StringWriter();
        using var input = new StringReader(answer);
        Assert.AreEqual(expected, VcRuntimeTerms.PromptConsole(input, output, polish));
        StringAssert.Contains(output.ToString(), polish ? "Wersja warunków: 1" : "Terms version: 1");
        StringAssert.Contains(output.ToString(), "MSVCP140.dll");
    }
}
