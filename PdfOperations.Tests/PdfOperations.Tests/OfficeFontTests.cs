using System.Text;
using System.Xml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using PdfOperations.Gui.Models;
using PdfOperations.Gui.ViewModels;

namespace PdfOperations.Tests;

[TestClass]
[DoNotParallelize]
public sealed class OfficeFontTests
{
    private const string W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    private const string A = "http://schemas.openxmlformats.org/drawingml/2006/main";
    private const string P = "http://schemas.openxmlformats.org/presentationml/2006/main";
    private const string R = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    private string root = null!;

    [TestInitialize]
    public void Initialize() => root = Directory.CreateTempSubdirectory("PdfOperationsOfficeFonts-").FullName;

    [TestCleanup]
    public void Cleanup() => Directory.Delete(root, true);

    [TestMethod]
    public void DocxReadsPolishTextAndDirectRobotoBoldItalicWithoutChangingInput()
    {
        string path = Word(Paragraph("Roboto", "Zażółć gęślą jaźń", "<w:b/><w:i/>"));
        byte[] before = File.ReadAllBytes(path);
        var data = OfficeFontReader.Read(path);
        Assert.HasCount(1, data.Fonts);
        Assert.AreEqual("Roboto", data.Fonts[0].Family);
        Assert.AreEqual(700, data.Fonts[0].Weight);
        Assert.IsTrue(data.Fonts[0].Italic);
        CollectionAssert.AreEqual(before, File.ReadAllBytes(path));
    }

    [TestMethod]
    public void DocxUsesBaseStyleButNotAnUnusedStyle()
    {
        string styles = "<w:style w:styleId=\"Base\"><w:rPr><w:rFonts w:ascii=\"Roboto\"/><w:b/></w:rPr></w:style>"
            + "<w:style w:styleId=\"Child\"><w:basedOn w:val=\"Base\"/></w:style>"
            + "<w:style w:styleId=\"Unused\"><w:rPr><w:rFonts w:ascii=\"Unused Font\"/></w:rPr></w:style>";
        var data = OfficeFontReader.Read(Word("<w:p><w:pPr><w:pStyle w:val=\"Child\"/></w:pPr><w:r><w:t>Test</w:t></w:r></w:p>", styles));
        Assert.AreEqual("Roboto", data.Fonts.Single().Family);
        Assert.AreEqual(700, data.Fonts.Single().Weight);
    }

    [TestMethod]
    public void DocxDirectFlagsOverrideInheritedBoldItalicAndComplexScript()
    {
        string styles = "<w:style w:type=\"paragraph\" w:default=\"1\" w:styleId=\"Normal\"><w:rPr><w:rFonts w:ascii=\"Roboto\"/><w:b/><w:i/><w:cs/></w:rPr></w:style>";
        string body = "<w:p><w:r><w:rPr><w:b w:val=\"0\"/><w:i w:val=\"0\"/><w:cs w:val=\"0\"/></w:rPr><w:t>Test</w:t></w:r></w:p>";
        var font = OfficeFontReader.Read(Word(body, styles)).Fonts.Single();
        Assert.AreEqual("Roboto", font.Family);
        Assert.AreEqual(400, font.Weight);
        Assert.IsFalse(font.Italic);
    }

    [TestMethod]
    public void DocxBoldInTwoInheritedStylesTogglesBackToRegular()
    {
        string styles = "<w:style w:styleId=\"Base\"><w:rPr><w:rFonts w:ascii=\"Roboto\"/><w:b/></w:rPr></w:style>"
            + "<w:style w:styleId=\"Child\"><w:basedOn w:val=\"Base\"/><w:rPr><w:b/></w:rPr></w:style>";
        var data = OfficeFontReader.Read(Word("<w:p><w:pPr><w:pStyle w:val=\"Child\"/></w:pPr><w:r><w:t>Test</w:t></w:r></w:p>", styles));
        Assert.AreEqual(400, data.Fonts.Single().Weight);
    }

    [TestMethod]
    public void DocxResolvesThemeFromDefaultsAndCharacterStyleOverridesParagraph()
    {
        string defaults = "<w:docDefaults><w:rPrDefault><w:rPr><w:rFonts w:asciiTheme=\"minorHAnsi\"/></w:rPr></w:rPrDefault></w:docDefaults>";
        Assert.AreEqual("Roboto", OfficeFontReader.Read(Word("<w:p><w:r><w:t>Test</w:t></w:r></w:p>", defaults, Theme())).Fonts.Single().Family);
        string styles = "<w:style w:type=\"paragraph\" w:default=\"1\" w:styleId=\"Normal\"><w:rPr><w:rFonts w:ascii=\"Arial\"/></w:rPr></w:style>"
            + "<w:style w:type=\"character\" w:styleId=\"Emphasis\"><w:rPr><w:rFonts w:ascii=\"Roboto\"/><w:i/></w:rPr></w:style>";
        var font = OfficeFontReader.Read(Word("<w:p><w:r><w:rPr><w:rStyle w:val=\"Emphasis\"/></w:rPr><w:t>Test</w:t></w:r></w:p>", styles)).Fonts.Single();
        Assert.AreEqual("Roboto", font.Family);
        Assert.IsTrue(font.Italic);
    }

    [TestMethod]
    public void DocxReportsCycleWithoutHanging()
    {
        var data = OfficeFontReader.Read(Word("<w:p><w:pPr><w:pStyle w:val=\"Cycle\"/></w:pPr><w:r><w:t>Test</w:t></w:r></w:p>",
            "<w:style w:styleId=\"Cycle\"><w:basedOn w:val=\"Cycle\"/></w:style>"));
        CollectionAssert.Contains(data.Warnings.ToArray(), "styles");
        Assert.AreEqual("", data.Fonts.Single().Family);
    }

    [TestMethod]
    public void DocxIncludesHeaderAndFooter()
    {
        string path = Word(Paragraph("Arial"));
        using (var doc = WordprocessingDocument.Open(path, true))
        {
            Write(doc.MainDocumentPart!.AddNewPart<HeaderPart>(), $"<w:hdr xmlns:w=\"{W}\">{Paragraph("Header Font")}</w:hdr>");
            Write(doc.MainDocumentPart.AddNewPart<FooterPart>(), $"<w:ftr xmlns:w=\"{W}\">{Paragraph("Footer Font")}</w:ftr>");
        }
        var data = OfficeFontReader.Read(path);
        CollectionAssert.AreEquivalent(new[] { "Arial", "Header Font", "Footer Font" }, data.Fonts.Select(f => f.Family).ToArray());
        Assert.IsTrue(data.Fonts.Any(f => f.Family == "Header Font" && f.Source.Contains("header", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void EmbeddedFontDoesNotPretendToBeInstalled()
    {
        string path = Word(Paragraph("Roboto"));
        using (var doc = WordprocessingDocument.Open(path, true))
        {
            var table = doc.MainDocumentPart!.AddNewPart<FontTablePart>();
            var font = table.AddNewPart<FontPart>("application/vnd.openxmlformats-officedocument.obfuscatedFont", "font1");
            using (var stream = font.GetStream()) stream.WriteByte(0);
            Write(table, $"<w:fonts xmlns:w=\"{W}\" xmlns:r=\"{R}\"><w:font w:name=\"Roboto\"><w:embedRegular r:id=\"font1\"/></w:font></w:fonts>");
        }
        var data = OfficeFontReader.Read(path);
        Assert.IsTrue(data.Fonts.Single().Embedded);
        string report = OfficeFontReport.Format(data, new FixedCatalog(FontAvailability.MissingFamily), false);
        StringAssert.Contains(report, "Missing family");
        StringAssert.Contains(report, "Declared embedding: yes");
    }

    [TestMethod]
    [DataRow("docx")]
    [DataRow("pptx")]
    public void UnsupportedTextIsUnknownInsteadOfAvailable(string extension)
    {
        string path = extension == "docx" ? Word(Paragraph("Arial", "مرحبا"))
            : Presentation("<a:rPr><a:latin typeface=\"Arial\"/></a:rPr>", text: "مرحبا");
        var data = OfficeFontReader.Read(path);
        Assert.IsTrue(data.Fonts.All(f => f.Family == ""));
        CollectionAssert.Contains(data.Warnings.ToArray(), "scripts");
        StringAssert.Contains(OfficeFontReport.Format(data, new FixedCatalog(FontAvailability.Available), false), "Unknown");
    }

    [TestMethod]
    public void PptxDirectRobotoBoldItalicAndInputUnchanged()
    {
        string path = Presentation("<a:rPr b=\"1\" i=\"1\"><a:latin typeface=\"Roboto\"/></a:rPr>");
        byte[] before = File.ReadAllBytes(path);
        var font = OfficeFontReader.Read(path).Fonts.Single();
        Assert.AreEqual("Roboto", font.Family);
        Assert.AreEqual(700, font.Weight);
        Assert.IsTrue(font.Italic);
        CollectionAssert.AreEqual(before, File.ReadAllBytes(path));
    }

    [TestMethod]
    public void PptxInheritsMasterTextStyleAndResolvesTheme()
    {
        var data = OfficeFontReader.Read(Presentation("", "<a:lvl1pPr><a:defRPr b=\"1\"><a:latin typeface=\"+mn-lt\"/></a:defRPr></a:lvl1pPr>"));
        Assert.AreEqual("Roboto", data.Fonts.Single().Family);
        Assert.AreEqual(700, data.Fonts.Single().Weight);
    }

    [TestMethod]
    public void PptxLayoutOverridesMasterAndDirectFalseOverridesInheritedBold()
    {
        string layoutShape = "<p:sp><p:nvSpPr><p:nvPr><p:ph type=\"body\" idx=\"1\"/></p:nvPr></p:nvSpPr><p:txBody><a:lstStyle><a:lvl1pPr><a:defRPr b=\"1\"><a:latin typeface=\"Layout Font\"/></a:defRPr></a:lvl1pPr></a:lstStyle></p:txBody></p:sp>";
        string master = "<a:lvl1pPr><a:defRPr><a:latin typeface=\"Master Font\"/></a:defRPr></a:lvl1pPr>";
        var font = OfficeFontReader.Read(Presentation("<a:rPr b=\"0\"/>", master, layoutShape: layoutShape)).Fonts.Single();
        Assert.AreEqual("Layout Font", font.Family);
        Assert.AreEqual(400, font.Weight);
    }

    [TestMethod]
    public void PptxThemeOverrideIsUnknownButExplicitFontStillWorks()
    {
        string path = Presentation("<a:rPr><a:latin typeface=\"+mn-lt\"/></a:rPr>");
        using (var doc = PresentationDocument.Open(path, true))
            Write(doc.PresentationPart!.SlideParts.Single().AddNewPart<ThemeOverridePart>(), $"<a:themeOverride xmlns:a=\"{A}\"/>");
        var data = OfficeFontReader.Read(path);
        Assert.AreEqual("", data.Fonts.Single().Family);
        CollectionAssert.Contains(data.Warnings.ToArray(), "themeOverrides");
        using (var doc = PresentationDocument.Open(path, true))
        {
            var slide = doc.PresentationPart!.SlideParts.Single();
            string contents;
            using (var reader = new StreamReader(slide.GetStream())) contents = reader.ReadToEnd();
            Write(slide, contents.Replace("+mn-lt", "Roboto", StringComparison.Ordinal));
        }
        Assert.AreEqual("Roboto", OfficeFontReader.Read(path).Fonts.Single().Family);
    }

    [TestMethod]
    [DataRow("docx")]
    [DataRow("pptx")]
    public void InvalidPackageThrowsAndReleasesFile(string extension)
    {
        string path = Path.Combine(root, "invalid." + extension);
        File.WriteAllText(path, "invalid package");
        Assert.Throws<Exception>(() => OfficeFontReader.Read(path));
        using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.IsTrue(stream.CanWrite);
    }

    [TestMethod]
    public void XmlReaderRejectsDtd()
    {
        string path = Word(Paragraph("Arial"));
        using (var doc = WordprocessingDocument.Open(path, true))
            Write(doc.MainDocumentPart!, $"<!DOCTYPE document [<!ENTITY x 'text'>]><w:document xmlns:w=\"{W}\"><w:body/></w:document>");
        Assert.Throws<XmlException>(() => OfficeFontReader.Read(path));
    }

    [TestMethod]
    public void CancellationStopsBeforeOpeningAndBeforeFormatting()
    {
        var token = new CancellationToken(true);
        Assert.Throws<OperationCanceledException>(() => OfficeFontReader.Read("missing.docx", token));
        Assert.Throws<OperationCanceledException>(() => WindowsFontCatalog.Read(token));
        var data = new OfficeFontDocument([new("Arial", 400, false, "test", false)], []);
        Assert.Throws<OperationCanceledException>(() => OfficeFontReport.Format(data, new FixedCatalog(FontAvailability.Available), false, token));
    }

    [TestMethod]
    public void ReportsAreLocalizedAndDistinguishMissingFaceFromUnknown()
    {
        var data = new OfficeFontDocument([new("Roboto", 700, true, "slide 1", false), new("", 400, false, "slide 2", false)], ["unresolved"]);
        string pl = OfficeFontReport.Format(data, new FixedCatalog(FontAvailability.MissingFace), true);
        string en = OfficeFontReport.Format(data, new FixedCatalog(FontAvailability.MissingFace), false);
        StringAssert.Contains(pl, "Nie znaleziono dokładnej odmiany");
        StringAssert.Contains(pl, "Nie ustalono");
        StringAssert.Contains(pl, "Kursywa");
        StringAssert.Contains(en, "Exact face not found");
        StringAssert.Contains(en, "Unknown");
        StringAssert.Contains(en, "Italic");
    }

    [TestMethod]
    public void NativeWindowsCatalogDistinguishesFamilyFaceAndUnknown()
    {
        var catalog = WindowsFontCatalog.Read();
        Assert.AreEqual(FontAvailability.MissingFamily, catalog.Check("AbsentFont_91A5DAF4", 400, false));
        Assert.AreEqual(FontAvailability.Unknown, catalog.Check("+mn-lt", 400, false));
        Assert.AreEqual(FontAvailability.Available, catalog.Check("Arial", 400, false));
        Assert.AreEqual(FontAvailability.MissingFace, catalog.Check("Arial", 1, false));
    }

    [TestMethod]
    public void FileFiltersAndPreviewVisibilityAreModeSpecific()
    {
        var vm = new InfoViewModel(InfoMode.FontInfo);
        CollectionAssert.AreEqual(new[] { "*.pdf", "*.docx", "*.pptx" }, vm.FilePatterns);
        CollectionAssert.AreEqual(new[] { "*.pdf" }, new InfoViewModel(InfoMode.PdfInfo).FilePatterns);
        Assert.AreEqual("font_info.txt", vm.OutputFileName);
        var notifications = new List<string?>();
        vm.PropertyChanged += (_, e) => notifications.Add(e.PropertyName);
        vm.ReportText = "preview";
        Assert.IsTrue(vm.HasReport);
        vm.ReportText = "";
        Assert.IsFalse(vm.HasReport);
        Assert.AreEqual(2, notifications.Count(n => n == nameof(vm.HasReport)));
    }

    [TestMethod]
    [DataRow(AppLanguage.Polish)]
    [DataRow(AppLanguage.English)]
    public async Task GuiWritesMixedPdfDocxPptxReportAndPreview(AppLanguage language)
    {
        var vm = ViewModel([TestHelper.SetInputPaths(["test_1.pdf"])[0], Word(Paragraph("Roboto")), Presentation("<a:rPr><a:latin typeface=\"Arial\"/></a:rPr>")]);
        var previous = vm.Loc.Language;
        try
        {
            vm.Loc.Language = language;
            await vm.StartCommand.ExecuteAsync(null);
            Assert.AreEqual(StatusKind.Success, vm.StatusKind, vm.ErrorDetails);
            string report = File.ReadAllText(Path.Combine(root, "font_info.txt"));
            foreach (string family in new[] { "Helvetica", "Roboto", "Arial" })
            {
                StringAssert.Contains(report, family);
                StringAssert.Contains(vm.ReportText, family);
            }
            StringAssert.Contains(vm.ReportText, language == AppLanguage.Polish ? "CZCIONKI DOKUMENTU" : "DOCUMENT FONTS");
            Assert.IsTrue(vm.HasReport);
            Assert.IsFalse(vm.IsBusy);
            Assert.IsEmpty(Directory.GetFiles(root, ".font-report-*.tmp"));
        }
        finally { vm.Loc.Language = previous; }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task GuiHonorsOverwriteInsteadOfAppending(bool overwrite)
    {
        var vm = ViewModel([Word(Paragraph("Roboto"))]);
        vm.OverwriteExistingFiles = overwrite;
        string original = Path.Combine(root, "font_info.txt");
        File.WriteAllText(original, "old report sentinel");
        await vm.StartCommand.ExecuteAsync(null);
        Assert.AreEqual(StatusKind.Success, vm.StatusKind, vm.ErrorDetails);
        string destination = overwrite ? original : Path.Combine(root, "font_info_1.txt");
        StringAssert.Contains(File.ReadAllText(destination), "Roboto");
        Assert.DoesNotContain("old report sentinel", File.ReadAllText(destination));
        if (!overwrite) Assert.AreEqual("old report sentinel", File.ReadAllText(original));
    }

    [TestMethod]
    public async Task GuiFailedSecondDocumentKeepsPreviousReportAndClearsPreview()
    {
        string bad = Path.Combine(root, "bad.docx");
        File.WriteAllText(bad, "not a package");
        var vm = ViewModel([Word(Paragraph("Roboto")), bad]);
        vm.OverwriteExistingFiles = true;
        vm.ReportText = "previous preview";
        string destination = Path.Combine(root, "font_info.txt");
        File.WriteAllText(destination, "old report");
        await vm.StartCommand.ExecuteAsync(null);
        Assert.AreEqual(StatusKind.Error, vm.StatusKind);
        Assert.AreEqual("old report", File.ReadAllText(destination));
        Assert.IsFalse(vm.HasReport);
        Assert.IsFalse(vm.IsBusy);
        Assert.IsEmpty(Directory.GetFiles(root, ".font-report-*.tmp"));
    }

    [TestMethod]
    public async Task GuiCancellationAfterFirstFilePreservesReportAndAllowsRetry()
    {
        var vm = ViewModel([Word(Paragraph("Roboto")), Word(Paragraph("Arial"))]);
        vm.OverwriteExistingFiles = true;
        string destination = Path.Combine(root, "font_info.txt");
        File.WriteAllText(destination, "old report");
        bool cancelOnce = true;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(vm.CompletedFiles) && vm.CompletedFiles == 1 && cancelOnce)
            {
                cancelOnce = false;
                vm.CancelOperationCommand.Execute(null);
            }
        };
        await new InlineContext().Start(() => vm.StartCommand.ExecuteAsync(null));
        Assert.IsFalse(cancelOnce);
        Assert.AreEqual(StatusKind.Info, vm.StatusKind, vm.ErrorDetails);
        Assert.AreEqual("old report", File.ReadAllText(destination));
        Assert.IsFalse(vm.HasReport);
        Assert.IsFalse(vm.IsBusy);
        Assert.IsEmpty(Directory.GetFiles(root, ".font-report-*.tmp"));
        await vm.StartCommand.ExecuteAsync(null);
        Assert.AreEqual(StatusKind.Success, vm.StatusKind, vm.ErrorDetails);
        Assert.IsTrue(vm.HasReport);
    }

    [TestMethod]
    public async Task GuiTruncatesOnlyPreviewNotSavedReport()
    {
        var body = new StringBuilder();
        for (int i = 0; i < 1800; i++) body.Append(Paragraph($"Font{i:D4}_" + new string('X', 80)));
        var vm = ViewModel([Word(body.ToString())]);
        await vm.StartCommand.ExecuteAsync(null);
        Assert.AreEqual(StatusKind.Success, vm.StatusKind, vm.ErrorDetails);
        string report = File.ReadAllText(Path.Combine(root, "font_info.txt"));
        Assert.IsGreaterThan(200_000, report.Length);
        Assert.AreEqual(200_000 + Environment.NewLine.Length + vm.Loc.FontPreviewTruncated.Length, vm.ReportText.Length);
        StringAssert.EndsWith(vm.ReportText, vm.Loc.FontPreviewTruncated);
        StringAssert.Contains(report, "Font1799_");
        Assert.DoesNotContain("Font1799_", vm.ReportText);
    }

    [TestMethod]
    public async Task PdfInfoStillWorksWithoutShowingFontPreview()
    {
        var vm = new InfoViewModel(InfoMode.PdfInfo);
        vm.InputFilesList.SetFiles(TestHelper.SetInputPaths(["test_1.pdf"]));
        vm.OutputDirectory = root;
        await vm.StartCommand.ExecuteAsync(null);
        Assert.AreEqual(StatusKind.Success, vm.StatusKind, vm.ErrorDetails);
        StringAssert.Contains(File.ReadAllText(Path.Combine(root, "pdf_info.txt")), "Pages:");
        Assert.IsFalse(vm.HasReport);
    }

    private InfoViewModel ViewModel(string[] files)
    {
        var vm = new InfoViewModel(InfoMode.FontInfo);
        vm.InputFilesList.SetFiles(files);
        vm.OutputDirectory = root;
        return vm;
    }

    private sealed class FixedCatalog(FontAvailability status) : IInstalledFontCatalog
    {
        public FontAvailability Check(string family, int weight, bool italic) => status;
    }

    // Deliver progress immediately so cancellation between two files is deterministic.
    private sealed class InlineContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback callback, object? state) => callback(state);
        public Task Start(Func<Task> action)
        {
            var previous = Current;
            SetSynchronizationContext(this);
            try { return action(); }
            finally { SetSynchronizationContext(previous); }
        }
    }

    private static string Paragraph(string family, string text = "Test", string flags = "") =>
        $"<w:p><w:r><w:rPr><w:rFonts w:ascii=\"{family}\" w:hAnsi=\"{family}\"/>{flags}</w:rPr><w:t>{text}</w:t></w:r></w:p>";

    private string Word(string body, string? styles = null, string? theme = null)
    {
        string path = Path.Combine(root, Guid.NewGuid() + ".docx");
        using var doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document);
        var main = doc.AddMainDocumentPart();
        Write(main, $"<w:document xmlns:w=\"{W}\"><w:body>{body}</w:body></w:document>");
        if (styles is not null) Write(main.AddNewPart<StyleDefinitionsPart>(), $"<w:styles xmlns:w=\"{W}\">{styles}</w:styles>");
        if (theme is not null) Write(main.AddNewPart<ThemePart>(), theme);
        return path;
    }

    private string Presentation(string runProperties, string masterTextStyle = "", string text = "Test", string layoutShape = "")
    {
        string path = Path.Combine(root, Guid.NewGuid() + ".pptx");
        using var doc = PresentationDocument.Create(path, PresentationDocumentType.Presentation);
        var pres = doc.AddPresentationPart();
        Write(pres, $"<p:presentation xmlns:p=\"{P}\" xmlns:r=\"{R}\"><p:sldIdLst><p:sldId id=\"256\" r:id=\"slide1\"/></p:sldIdLst></p:presentation>");
        var slide = pres.AddNewPart<SlidePart>("slide1");
        Write(slide, $"<p:sld xmlns:p=\"{P}\" xmlns:a=\"{A}\"><p:cSld><p:spTree><p:sp><p:nvSpPr><p:nvPr><p:ph type=\"body\" idx=\"1\"/></p:nvPr></p:nvSpPr><p:txBody><a:bodyPr/><a:lstStyle/><a:p><a:r>{runProperties}<a:t>{text}</a:t></a:r></a:p></p:txBody></p:sp></p:spTree></p:cSld></p:sld>");
        var layout = slide.AddNewPart<SlideLayoutPart>();
        Write(layout, $"<p:sldLayout xmlns:p=\"{P}\" xmlns:a=\"{A}\"><p:cSld><p:spTree>{layoutShape}</p:spTree></p:cSld></p:sldLayout>");
        var master = layout.AddNewPart<SlideMasterPart>();
        Write(master, $"<p:sldMaster xmlns:p=\"{P}\" xmlns:a=\"{A}\"><p:txStyles><p:bodyStyle>{masterTextStyle}</p:bodyStyle></p:txStyles></p:sldMaster>");
        Write(master.AddNewPart<ThemePart>(), Theme());
        return path;
    }

    private static string Theme() => $"<a:theme xmlns:a=\"{A}\"><a:themeElements><a:fontScheme name=\"Test\"><a:majorFont><a:latin typeface=\"Aptos Display\"/></a:majorFont><a:minorFont><a:latin typeface=\"Roboto\"/></a:minorFont></a:fontScheme></a:themeElements></a:theme>";

    private static void Write(OpenXmlPart part, string xml)
    {
        using var writer = new StreamWriter(part.GetStream(FileMode.Create, FileAccess.Write));
        writer.Write(xml);
    }
}
