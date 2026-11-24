using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
namespace PdfOperations;

public static class OfficePresentationFontReader
{
    private static readonly XNamespace A = "http://schemas.openxmlformats.org/drawingml/2006/main";
    private static readonly XNamespace P = "http://schemas.openxmlformats.org/presentationml/2006/main";
    private static readonly XNamespace R = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    public static OfficeFontDocument Read(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var document = PresentationDocument.Open(stream, false,
            new OpenSettings { MaxCharactersInPart = 32_000_000 });
        var main = document.PresentationPart
            ?? throw new InvalidDataException("Missing presentation part.");
        var xml = new OfficeFontXml(cancellationToken);
        var root = xml.Read(main)!;
        var fonts = new List<OfficeFontUse>();
        var warnings = new HashSet<string>(StringComparer.Ordinal);
        int slideNumber = 0;

        foreach (var slideId in root.Descendants(P + "sldId"))
        {
            cancellationToken.ThrowIfCancellationRequested();
            string id = (string?)slideId.Attribute(R + "id")
                ?? throw new InvalidDataException("Missing slide relationship.");
            var slide = main.GetPartById(id) as SlidePart
                ?? throw new InvalidDataException("Invalid slide relationship.");
            var layout = slide.SlideLayoutPart;
            var master = layout?.SlideMasterPart;
            var content = xml.Read(slide)!;
            var theme = xml.Read(master?.ThemePart);
            bool themeOverride = slide.Parts.Concat(layout?.Parts ?? Enumerable.Empty<IdPartPair>())
                .Any(pair => pair.OpenXmlPart is ThemeOverridePart);
            if (themeOverride) warnings.Add("themeOverrides");
            if (content.Descendants(P + "graphicFrame").Any()) warnings.Add("objects");
            slideNumber++;
            int paragraphNumber = 0;
            
            foreach (var paragraph in content.Descendants(A + "p"))
            {
                cancellationToken.ThrowIfCancellationRequested();
                string source = $"slide {slideNumber}: p. {++paragraphNumber}";
                
                foreach (var run in paragraph.Elements().Where(e => e.Name == A + "r" || e.Name == A + "fld"))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string text = run.Element(A + "t")?.Value ?? "";
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    var layers = OfficePresentationStyles.GetLayers(
                        run, paragraph, xml.Read(layout), xml.Read(master), root);
                    string? family = OfficePresentationFormatting.ResolveFamily(layers, theme, themeOverride);
                    bool bold = OfficePresentationFormatting.ResolveFlag(layers, "b");
                    bool italic = OfficePresentationFormatting.ResolveFlag(layers, "i");
                    bool unsupported = text.Any(c => c > 0x024f && c is not
                        (>= '\u0300' and <= '\u036f' or >= '\u1e00' and <= '\u1eff'
                        or >= '\u2000' and <= '\u206f' or >= '\u20a0' and <= '\u20cf'));
                    if (unsupported) { family = null; warnings.Add("scripts"); }
                    if (!paragraph.Ancestors(P + "sp").Any()) { family = null; warnings.Add("objects"); }
                    if (string.IsNullOrWhiteSpace(family)) warnings.Add("unresolved");
                    string face = bold ? (italic ? "boldItalic" : "bold") : (italic ? "italic" : "regular");
                    
                    bool embedded = !string.IsNullOrWhiteSpace(family) && root.Descendants(P + "embeddedFont")
                        .Where(f => string.Equals((string?)f.Element(P + "font")?.Attribute("typeface"),
                            family, StringComparison.OrdinalIgnoreCase))
                        .Any(f => (string?)f.Element(P + face)?.Attribute(R + "id") is { } fontId
                            && main.Parts.Any(pair => pair.RelationshipId == fontId));
                    
                    if (fonts.Count >= 100_000) throw new InvalidDataException("Too many font uses.");
                    fonts.Add(new OfficeFontUse(family ?? "", bold ? 700 : 400, italic, source, embedded));
                }
            }
        }
        
        if (fonts.Count == 0) warnings.Add("empty");
        return new OfficeFontDocument(fonts.Distinct().ToArray(), warnings.Order().ToArray());
    }
}
