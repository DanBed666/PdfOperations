using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;

namespace PdfOperations.Tests;

[TestClass]
public class DocxTextPreparationTests
{
    private const string WordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    [TestMethod]
    public void AddsParagraphBetweenFinalTableAndSection()
    {
        CheckPreparation("<w:tbl/><w:sectPr/>", WordNamespace, (original, prepared) =>
        {
            XNamespace w = WordNamespace;
            CollectionAssert.AreEqual(new[] { "tbl", "p", "sectPr" },
                prepared.Root!.Element(w + "body")!.Elements().Select(e => e.Name.LocalName).ToArray());
        });
    }

    [TestMethod]
    public void AddsParagraphAfterFinalTableWithoutSection()
    {
        CheckPreparation("<w:tbl/>", WordNamespace, (original, prepared) =>
        {
            XNamespace w = WordNamespace;
            CollectionAssert.AreEqual(new[] { "tbl", "p" },
                prepared.Root!.Element(w + "body")!.Elements().Select(e => e.Name.LocalName).ToArray());
        });
    }

    [TestMethod]
    public void DoesNotAddAnotherParagraphWhenOneAlreadyExists()
    {
        CheckPreparation("<w:tbl/><w:p><w:r><w:t>End</w:t></w:r></w:p><w:sectPr/>",
            WordNamespace, (original, prepared) => Assert.IsTrue(XNode.DeepEquals(original, prepared)));
    }

    [TestMethod]
    public void SupportsStrictDocxNamespace()
    {
        const string strictNamespace = "http://purl.oclc.org/ooxml/wordprocessingml/main";
        CheckPreparation("<w:tbl/><w:sectPr/>", strictNamespace, (original, prepared) =>
        {
            XNamespace w = strictNamespace;
            Assert.AreEqual(w + "p",
                prepared.Root!.Element(w + "body")!.Elements().ElementAt(1).Name);
        });
    }

    [TestMethod]
    public void PreservesTextWithoutTables()
    {
        CheckPreparation("<w:p><w:r><w:t xml:space=\"preserve\">  text  </w:t></w:r></w:p>",
            WordNamespace, (original, prepared) => Assert.IsTrue(XNode.DeepEquals(original, prepared)));
    }

    private static void CheckPreparation(string body, string wordNamespace, Action<XDocument, XDocument> check)
    {
        string root = Directory.CreateTempSubdirectory("PdfOperationsDocxPreparationTest-").FullName;
        try
        {
            string input = Path.Combine(root, "sample.docx");
            string xml = $"<w:document xmlns:w=\"{wordNamespace}\"><w:body>{body}</w:body></w:document>";
            using (var zip = ZipFile.Open(input, ZipArchiveMode.Create))
            {
                using (var writer = new StreamWriter(zip.CreateEntry("word/document.xml").Open()))
                    writer.Write(xml);
                using (var writer = new StreamWriter(zip.CreateEntry("word/other.xml").Open()))
                    writer.Write("<unchanged/>");
            }
            byte[] originalBytes = File.ReadAllBytes(input);
            string work = Path.Combine(root, "work");
            Directory.CreateDirectory(work);
            MethodInfo prepare = typeof(Convert).GetMethod("PrepareDocxForText",
                BindingFlags.NonPublic | BindingFlags.Static)!;
            string copy = (string)prepare.Invoke(null, [input, work])!;
            string secondCopy = (string)prepare.Invoke(null, [input, work])!;
            Assert.AreNotEqual(copy, secondCopy);
            Assert.AreEqual(Path.GetFileName(input), Path.GetFileName(copy));
            CollectionAssert.AreEqual(originalBytes, File.ReadAllBytes(input));
            using var result = ZipFile.OpenRead(copy);
            using var content = result.GetEntry("word/document.xml")!.Open();
            check(XDocument.Parse(xml), XDocument.Load(content));
            using var other = new StreamReader(result.GetEntry("word/other.xml")!.Open());
            Assert.AreEqual("<unchanged/>", other.ReadToEnd());
        }
        finally { Directory.Delete(root, true); }
    }
}
