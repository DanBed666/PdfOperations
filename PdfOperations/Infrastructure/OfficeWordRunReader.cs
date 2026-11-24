using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace PdfOperations;

internal static class OfficeWordRunReader
{
    private static readonly XNamespace W =
        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    private static readonly XNamespace R =
        "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    public static IEnumerable<OfficeFontUse> Read(
        XElement run, XElement paragraph, OfficeWordStyles styles,
        XDocument? theme, FontTablePart? fontTable,
        XDocument? fontTableXml, string source, ISet<string> warnings)
    {
        var layers = OfficeWordFormatting.GetLayers(run, paragraph, styles);
        
        bool bold = OfficeWordFormatting.ResolveToggle(
            run, layers, styles.DefaultRunProperties, "b");
        
        bool italic = OfficeWordFormatting.ResolveToggle(
            run, layers, styles.DefaultRunProperties, "i");
        
        string text = string.Concat(run.Elements(W + "t").Select(t => t.Value));

        if (!string.IsNullOrWhiteSpace(text))
        {
            bool unsupported = text.Any(c => c > 0x024f &&
                !(c >= 0x0300 && c <= 0x036f) &&
                !(c >= 0x1e00 && c <= 0x1eff) &&
                !(c >= 0x2000 && c <= 0x206f) &&
                !(c >= 0x20a0 && c <= 0x20cf));
            
            var complexScript = layers.Select(layer => layer.Element(W + "cs"))
                .FirstOrDefault(value => value is not null);
            unsupported |= complexScript is not null &&
                (string?)complexScript.Attribute(W + "val") is null or "1" or "true" or "on";
            
            if (unsupported)
                warnings.Add("scripts");

            var scripts = new List<string>();
            if (text.Any(c => c < 128)) scripts.Add("ascii");
            if (text.Any(c => c >= 128)) scripts.Add("hAnsi");

            foreach (string script in scripts)
            {
                string? family = unsupported ? null :
                    OfficeWordFormatting.ResolveFamily(layers, script, theme);
                yield return Create(family);
            }
        }

        foreach (var symbol in run.Elements(W + "sym"))
            yield return Create((string?)symbol.Attribute(W + "font"));

        OfficeFontUse Create(string? family)
        {
            family = family?.Trim() ?? "";
            if (family.Length == 0) warnings.Add("unresolved");
            
            string kind = bold ? (italic ? "embedBoldItalic" : "embedBold")
                : (italic ? "embedItalic" : "embedRegular");
            
            bool embedded = fontTable is not null && family.Length > 0 &&
                (fontTableXml?.Descendants(W + "font") ?? Enumerable.Empty<XElement>())
                .Where(f => string.Equals((string?)f.Attribute(W + "name"),
                    family, StringComparison.OrdinalIgnoreCase))
                .Any(f => (string?)f.Element(W + kind)?.Attribute(R + "id") is { } id
                    && fontTable.Parts.Any(p => p.RelationshipId == id
                        && p.OpenXmlPart is FontPart));
            
            return new OfficeFontUse(family, bold ? 700 : 400, italic, source, embedded);
        }
    }
}
