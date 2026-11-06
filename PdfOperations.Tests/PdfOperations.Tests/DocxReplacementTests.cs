using System.IO.Compression;
using System.Xml.Linq;
using ClosedXML.Excel;
using LoadOptions = System.Xml.Linq.LoadOptions;
using SaveOptions = System.Xml.Linq.SaveOptions;

namespace PdfOperations.Tests;

[TestClass]
public class DocxReplacementTests
{
    private static readonly XNamespace W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    [TestMethod]
    [DataRow("[STATUS]", false, true)]
    [DataRow("[status]", true, true)]
    [DataRow("[status]", false, false)]
    public void ReplacesAcrossFormattingAndRespectsIgnoreCase(string find, bool ignoreCase, bool replaced)
    {
        const string body = """
            <w:p>
              <w:r><w:rPr><w:b/></w:rPr><w:t>Before [STA</w:t></w:r>
              <w:r><w:rPr><w:i/></w:rPr><w:t>TU</w:t></w:r>
              <w:r><w:rPr><w:u w:val="single"/></w:rPr><w:t>S] after</w:t></w:r>
            </w:p>
            """;
        XDocument result = ReplaceXml(body, [new() { Find = find, Replace = "Done", IgnoreCase = ignoreCase }]);
        XElement[] runs = result.Descendants(W + "r").ToArray();

        Assert.HasCount(3, runs);
        Assert.AreEqual(replaced ? "Before Done" : "Before [STA", runs[0].Element(W + "t")!.Value);
        Assert.AreEqual(replaced ? "" : "TU", runs[1].Element(W + "t")!.Value);
        Assert.AreEqual(replaced ? " after" : "S] after", runs[2].Element(W + "t")!.Value);
        Assert.IsNotNull(runs[0].Element(W + "rPr")!.Element(W + "b"));
        Assert.IsNotNull(runs[1].Element(W + "rPr")!.Element(W + "i"));
        Assert.AreEqual("single", (string?)runs[2].Element(W + "rPr")!.Element(W + "u")!.Attribute(W + "val"));
        if (replaced)
            Assert.AreEqual("preserve", (string?)runs[2].Element(W + "t")!.Attribute(XNamespace.Xml + "space"));
    }

    [TestMethod]
    [DataRow("X")]
    [DataRow("A much longer value")]
    [DataRow("")]
    [DataRow(" <A&B> ")]
    public void ReplacesMultipleMatchesWithoutCorruptingOffsets(string replacement)
    {
        const string body = """
            <w:p><w:r><w:t>[STATUS] [STA</w:t></w:r><w:r><w:t/></w:r>
            <w:r><w:t>TUS] [STATUS] end</w:t></w:r></w:p>
            """;
        XDocument result = ReplaceXml(body, [new() { Find = "[STATUS]", Replace = replacement }]);
        Assert.AreEqual($"{replacement} {replacement} {replacement} end", Text(result));
    }

    [TestMethod]
    public void AppliesRulesInOrderAndDoesNotRecursivelyReplaceInsertedText()
    {
        XDocument result = ReplaceXml("<w:p><w:r><w:t>A</w:t></w:r><w:r><w:t>B AB</w:t></w:r></w:p>",
        [
            new() { Find = "", Replace = "ignored" },
            new() { Find = "AB", Replace = "ABAB" },
            new() { Find = "ABAB", Replace = "Done" }
        ]);
        Assert.AreEqual("Done Done", Text(result));
    }

    [TestMethod]
    [DataRow("<w:tab/>")]
    [DataRow("<w:br/>")]
    [DataRow("<w:cr/>")]
    [DataRow("<w:drawing/>")]
    [DataRow("<w:fldChar w:fldCharType='begin'/>")]
    [DataRow("<w:instrText>PAGE</w:instrText>")]
    [DataRow("<w:fldSimple w:instr='PAGE'><w:r><w:t>1</w:t></w:r></w:fldSimple>")]
    public void DoesNotJoinTextAcrossNonTextBoundaries(string boundary)
    {
        string body = $"<w:p><w:r><w:t>[STA</w:t>{boundary}<w:t>TUS]</w:t></w:r></w:p>";
        XDocument original = Document(body);
        XDocument result = ReplaceXml(body, [new() { Find = "[STATUS]", Replace = "Done" }]);
        Assert.IsTrue(XNode.DeepEquals(original, result));
    }

    [TestMethod]
    public void DoesNotJoinParagraphsOrTableCells()
    {
        const string body = """
            <w:p><w:r><w:t>[STA</w:t></w:r></w:p><w:p><w:r><w:t>TUS]</w:t></w:r></w:p>
            <w:tbl><w:tr><w:tc><w:p><w:r><w:t>[STA</w:t></w:r></w:p></w:tc>
            <w:tc><w:p><w:r><w:t>TUS]</w:t></w:r></w:p></w:tc></w:tr></w:tbl>
            """;
        XDocument result = ReplaceXml(body, [new() { Find = "[STATUS]", Replace = "Done" }]);
        Assert.IsTrue(XNode.DeepEquals(Document(body), result));
    }

    [TestMethod]
    public void ReplacesInsideTableAndTextBoxParagraphsIndependently()
    {
        const string body = """
            <w:tbl><w:tr><w:tc><w:p><w:r><w:t>[STA</w:t></w:r>
            <w:r><w:t>TUS]</w:t></w:r></w:p></w:tc></w:tr></w:tbl>
            <w:p><w:r><w:t>[STA</w:t></w:r><w:r><w:pict><w:txbxContent>
            <w:p><w:r><w:t>[STA</w:t></w:r><w:r><w:t>TUS]</w:t></w:r></w:p>
            </w:txbxContent></w:pict></w:r><w:r><w:t>TUS]</w:t></w:r></w:p>
            """;
        XDocument result = ReplaceXml(body, [new() { Find = "[STATUS]", Replace = "Done" }]);
        Assert.AreEqual("Done[STADoneTUS]", Text(result));
    }

    [TestMethod]
    public void CancellationLeavesXmlUnchanged()
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string path = Path.Combine(directory, "document.xml");
            Document("<w:p><w:r><w:t>Old</w:t></w:r></w:p>").Save(path);
            byte[] before = File.ReadAllBytes(path);
            Assert.Throws<OperationCanceledException>(() => Replacement.ReplaceTextInDocxXml(path,
                [new() { Find = "Old", Replace = "New" }], new CancellationToken(true)));
            CollectionAssert.AreEqual(before, File.ReadAllBytes(path));
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    public void ReplacesFormattedDocxUsingRulesFromExcel()
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string source = Path.Combine(directory, "input.docx");
            File.Copy(TestHelper.SetInputPath("word_search_1.docx"), source);
            using (ZipArchive archive = ZipFile.Open(source, ZipArchiveMode.Update))
            {
                archive.GetEntry("word/document.xml")!.Delete();
                using Stream xml = archive.CreateEntry("word/document.xml").Open();
                Document("""
                    <w:p><w:r><w:rPr><w:b/></w:rPr><w:t>[STA</w:t></w:r>
                    <w:r><w:rPr><w:i/></w:rPr><w:t>TU</w:t></w:r>
                    <w:r><w:t>S]</w:t></w:r></w:p>
                    """).Save(xml);
            }
            string excel = Path.Combine(directory, "rules.xlsx");
            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Replacements");
                sheet.Cell("A1").InsertData(new object[][]
                {
                    ["Lp", "Find", "Replace", "IgnoreCase"],
                    [1, "[status]", "Completed", "TRUE"]
                });
                workbook.SaveAs(excel);
            }
            string output = Path.Combine(directory, "result.docx");
            Replacement.ReplaceTextWithPlaceholders(
                new FileJob { InputFile = source, TempPath = output },
                new OperationInput { InputFiles = [source], Output = "result.docx", PlaceholderFile = excel },
                new OperationContext { TempDir = directory });

            using ZipArchive result = ZipFile.OpenRead(output);
            using Stream resultXml = result.GetEntry("word/document.xml")!.Open();
            XDocument document = XDocument.Load(resultXml);
            Assert.AreEqual("Completed", Text(document));
            Assert.IsNotNull(document.Descendants(W + "r").First().Element(W + "rPr")!.Element(W + "b"));
            Assert.IsNotNull(result.GetEntry("[Content_Types].xml"));
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    private static XDocument Document(string body) => XDocument.Parse(
        $"<w:document xmlns:w='{W}'><w:body>{body}</w:body></w:document>", LoadOptions.PreserveWhitespace);

    private static string Text(XDocument document) => string.Concat(document.Descendants(W + "t").Select(t => t.Value));

    private static XDocument ReplaceXml(string body, List<ReplacementPair> pairs)
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string path = Path.Combine(directory, "document.xml");
            Document(body).Save(path, SaveOptions.DisableFormatting);
            Replacement.ReplaceTextInDocxXml(path, pairs);
            return XDocument.Load(path, LoadOptions.PreserveWhitespace);
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }
}
