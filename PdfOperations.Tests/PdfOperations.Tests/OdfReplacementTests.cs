using System.IO.Compression;
using System.Xml.Linq;
using ClosedXML.Excel;
using LoadOptions = System.Xml.Linq.LoadOptions;
using SaveOptions = System.Xml.Linq.SaveOptions;

namespace PdfOperations.Tests;

[TestClass]
public class OdfReplacementTests
{
    private static readonly XNamespace Text = "urn:oasis:names:tc:opendocument:xmlns:text:1.0";
    private static readonly XNamespace Office = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";
    private static readonly XNamespace XLink = "http://www.w3.org/1999/xlink";
    private const string FormattedParagraph = """
        <text:p><text:span text:style-name="Bold">Before [STA</text:span><text:span text:style-name="Italic">TU</text:span><text:span text:style-name="Bold">S] after</text:span></text:p>
        """;

    [TestMethod]
    [DataRow(".odt", "[STATUS]", false, true)]
    [DataRow(".odt", "[status]", true, true)]
    [DataRow(".odt", "[status]", false, false)]
    [DataRow(".odg", "[STATUS]", false, true)]
    [DataRow(".odg", "[status]", true, true)]
    [DataRow(".odg", "[status]", false, false)]
    public void ReplacesAcrossFormattingAndRespectsIgnoreCase(string extension, string find, bool ignoreCase, bool replaced)
    {
        XDocument result = ReplaceXml(extension, FormattedParagraph,
            [new() { Find = find, Replace = "Done", IgnoreCase = ignoreCase }]);
        XElement[] spans = result.Descendants(Text + "span").ToArray();
        Assert.HasCount(3, spans);
        Assert.AreEqual(replaced ? "Before Done" : "Before [STA", ReadText(spans[0]));
        Assert.AreEqual(replaced ? "" : "TU", ReadText(spans[1]));
        Assert.AreEqual(replaced ? " after" : "S] after", ReadText(spans[2]));
        CollectionAssert.AreEqual(new[] { "Bold", "Italic", "Bold" },
            spans.Select(span => (string?)span.Attribute(Text + "style-name")).ToArray());
        Assert.IsTrue(XNode.DeepEquals(Document(extension, FormattedParagraph).Root!.Element(Office + "automatic-styles"),
            result.Root!.Element(Office + "automatic-styles")));
    }

    [TestMethod]
    [DataRow(".odt", "X")]
    [DataRow(".odg", "X")]
    [DataRow(".odt", "A much longer value")]
    [DataRow(".odg", "A much longer value")]
    [DataRow(".odt", "")]
    [DataRow(".odg", "")]
    [DataRow(".odt", " <A&B>  \tC\nD ")]
    [DataRow(".odg", " <A&B>  \tC\nD ")]
    public void ReplacesMultipleMatchesAndPreservesSpecialCharacters(string extension, string replacement)
    {
        const string body = "<text:p>[STATUS] [STA<text:span text:style-name='Bold'>TUS] [STATUS] end</text:span></text:p>";
        XDocument result = ReplaceXml(extension, body, [new() { Find = "[STATUS]", Replace = replacement }]);
        Assert.AreEqual($"{replacement} {replacement} {replacement} end", ReadText(result.Descendants(Text + "p").Single()));
    }

    [TestMethod]
    [DataRow(".odt")]
    [DataRow(".odg")]
    public void HandlesNestedSpansLinksAndExplicitSpacesWithoutChangingAttributes(string extension)
    {
        const string body = """
            <text:p text:style-name="STATUS"><text:span text:style-name="Bold">[STA<text:span text:style-name="Italic">TUS</text:span></text:span><text:s text:c="2"/><text:a xlink:href="https://example.com/STATUS">VALUE]</text:a></text:p>
            """;
        XDocument result = ReplaceXml(extension, body,
        [
            new() { Find = "[status  value]", Replace = "Ready", IgnoreCase = true },
            new() { Find = "STATUS", Replace = "Wrong" }
        ]);
        Assert.AreEqual("Ready", ReadText(result.Descendants(Text + "p").Single()));
        Assert.AreEqual("STATUS", (string?)result.Descendants(Text + "p").Single().Attribute(Text + "style-name"));
        Assert.AreEqual("https://example.com/STATUS", (string?)result.Descendants(Text + "a").Single().Attribute(XLink + "href"));
        Assert.AreEqual("Ready", ReadText(result.Descendants(Text + "span").First()));
    }

    [TestMethod]
    [DataRow(".odt")]
    [DataRow(".odg")]
    public void AppliesRulesOnceInOrderIncludingIgnoreCase(string extension)
    {
        XDocument result = ReplaceXml(extension, "<text:h text:outline-level='1'>A<text:span>B AB</text:span></text:h>",
        [
            new() { Find = "", Replace = "ignored" },
            new() { Find = "ab", Replace = "ab ab", IgnoreCase = true },
            new() { Find = "ab ab", Replace = "Done" }
        ]);
        Assert.AreEqual("Done Done", ReadText(result.Descendants(Text + "h").Single()));
    }

    [TestMethod]
    [DataRow(".odt", "<text:tab/>")]
    [DataRow(".odg", "<text:tab/>")]
    [DataRow(".odt", "<text:line-break/>")]
    [DataRow(".odg", "<text:line-break/>")]
    [DataRow(".odt", "<text:page-number>1</text:page-number>")]
    [DataRow(".odg", "<text:page-number>1</text:page-number>")]
    [DataRow(".odt", "<draw:frame/>")]
    [DataRow(".odg", "<draw:frame/>")]
    public void DoesNotJoinTextAcrossBreaksFieldsOrObjects(string extension, string boundary)
    {
        string body = $"<text:p>[STA{boundary}TUS]</text:p>";
        XDocument result = ReplaceXml(extension, body, [new() { Find = "[STATUS]", Replace = "Done" }]);
        Assert.IsTrue(XNode.DeepEquals(Document(extension, body), result));
    }

    [TestMethod]
    [DataRow(".odt")]
    [DataRow(".odg")]
    public void DoesNotJoinParagraphsAndLeavesUnmatchedXmlUntouched(string extension)
    {
        const string body = "<text:p>[STA</text:p><text:p>TUS]<text:s text:c='3'/><text:span text:style-name='Bold'>unchanged</text:span></text:p>";
        XDocument result = ReplaceXml(extension, body, [new() { Find = "[STATUS]", Replace = "Done" }]);
        Assert.IsTrue(XNode.DeepEquals(Document(extension, body), result));
    }

    [TestMethod]
    [DataRow(".odt")]
    [DataRow(".odg")]
    public void TreatsNestedTextBoxesAsSeparateParagraphs(string extension)
    {
        const string body = """
            <text:p>[STA<draw:frame><draw:text-box><text:p>[STA<text:span>TUS]</text:span></text:p></draw:text-box></draw:frame>TUS]</text:p>
            """;
        XDocument result = ReplaceXml(extension, body, [new() { Find = "[STATUS]", Replace = "Done" }]);
        Assert.AreEqual("[STADoneTUS]", ReadText(result.Descendants(Text + "p").First()));
    }

    [TestMethod]
    public void CancellationLeavesXmlUnchanged()
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string path = Path.Combine(directory, "content.xml");
            Document(".odt", FormattedParagraph).Save(path);
            byte[] before = File.ReadAllBytes(path);
            Assert.Throws<OperationCanceledException>(() => Replacement.ReplaceTextInOdfXml(path,
                [new() { Find = "[STATUS]", Replace = "Done" }], new CancellationToken(true)));
            CollectionAssert.AreEqual(before, File.ReadAllBytes(path));
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    [DataRow(".odt", "word_search_3.odt")]
    [DataRow(".odg", "word_search_2.odg")]
    public void ReplacesFormattedDocumentUsingRulesFromExcel(string extension, string fixture)
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string source = Path.Combine(directory, "input" + extension);
            File.Copy(TestHelper.SetInputPath(fixture), source);
            using (ZipArchive archive = ZipFile.Open(source, ZipArchiveMode.Update))
            {
                archive.GetEntry("content.xml")!.Delete();
                using Stream xml = archive.CreateEntry("content.xml").Open();
                Document(extension, FormattedParagraph).Save(xml, SaveOptions.DisableFormatting);
            }
            string excel = Path.Combine(directory, "rules.xlsx");
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add("Replacements").Cell("A1").InsertData(new object[][]
                {
                    ["Lp", "Find", "Replace", "IgnoreCase"],
                    [1, "[status]", "Completed", "TRUE"]
                });
                workbook.SaveAs(excel);
            }
            string output = Path.Combine(directory, "result" + extension);
            Replacement.ReplaceTextWithPlaceholders(
                new FileJob { InputFile = source, TempPath = output },
                new OperationInput { InputFiles = [source], Output = "result" + extension, PlaceholderFile = excel },
                new OperationContext { TempDir = directory });
            using ZipArchive result = ZipFile.OpenRead(output);
            using Stream resultXml = result.GetEntry("content.xml")!.Open();
            XDocument document = XDocument.Load(resultXml);
            Assert.AreEqual("Before Completed after", ReadText(document.Descendants(Text + "p").Single()));
            Assert.AreEqual("Bold", (string?)document.Descendants(Text + "span").First().Attribute(Text + "style-name"));
            using ZipArchive original = ZipFile.OpenRead(source);
            foreach (ZipArchiveEntry entry in original.Entries.Where(entry => entry.FullName != "content.xml" && !entry.FullName.EndsWith('/')))
            {
                using var expected = new MemoryStream();
                using var actual = new MemoryStream();
                using Stream input = entry.Open();
                using Stream saved = result.GetEntry(entry.FullName)!.Open();
                input.CopyTo(expected);
                saved.CopyTo(actual);
                CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), entry.FullName);
            }
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    private static XDocument Document(string extension, string paragraphs)
    {
        string body = extension == ".odt" ? $"<office:text>{paragraphs}</office:text>"
            : $"<office:drawing><draw:page draw:name='Page1'><draw:frame><draw:text-box>{paragraphs}</draw:text-box></draw:frame></draw:page></office:drawing>";
        return XDocument.Parse($"""
            <office:document-content xmlns:office="{Office}" xmlns:text="{Text}" xmlns:xlink="{XLink}"
                xmlns:draw="urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"
                xmlns:style="urn:oasis:names:tc:opendocument:xmlns:style:1.0"
                xmlns:fo="urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0" office:version="1.2">
              <office:automatic-styles>
                <style:style style:name="Bold" style:family="text"><style:text-properties fo:font-weight="bold"/></style:style>
                <style:style style:name="Italic" style:family="text"><style:text-properties fo:font-style="italic"/></style:style>
              </office:automatic-styles>
              <office:body>{body}</office:body>
            </office:document-content>
            """, LoadOptions.PreserveWhitespace);
    }

    private static string ReadText(XNode node) => node switch
    {
        XText value => value.Value,
        XElement element when element.Name == Text + "s" => new string(' ', (int?)element.Attribute(Text + "c") ?? 1),
        XElement element when element.Name == Text + "tab" => "\t",
        XElement element when element.Name == Text + "line-break" => "\n",
        XElement element => string.Concat(element.Nodes().Select(ReadText)),
        _ => ""
    };

    private static XDocument ReplaceXml(string extension, string body, List<ReplacementPair> pairs)
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string path = Path.Combine(directory, "content.xml");
            Document(extension, body).Save(path, SaveOptions.DisableFormatting);
            Replacement.ReplaceTextInFile(path, pairs, extension);
            return XDocument.Load(path, LoadOptions.PreserveWhitespace);
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }
}
