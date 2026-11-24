using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace PdfOperations;

public static class OfficeWordFontReader
{
    private static readonly XNamespace W =
        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    private static readonly XNamespace A =
        "http://schemas.openxmlformats.org/drawingml/2006/main";

    public static OfficeFontDocument Read(
        string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var document = WordprocessingDocument.Open(stream, false,
            new OpenSettings { MaxCharactersInPart = 32_000_000 });
        var main = document.MainDocumentPart
            ?? throw new InvalidDataException("Missing main document part.");

        var xml = new OfficeFontXml(cancellationToken);
        var fonts = new List<OfficeFontUse>();
        var warnings = new HashSet<string>(StringComparer.Ordinal);
        var styles = new OfficeWordStyles(
            xml.Read(main.StyleDefinitionsPart), warnings, cancellationToken);
        var theme = xml.Read(main.ThemePart);
        var fontTable = xml.Read(main.FontTablePart);

        var parts = new List<OpenXmlPart> { main };
        parts.AddRange(main.HeaderParts);
        parts.AddRange(main.FooterParts);
        if (main.FootnotesPart is { } footnotes) parts.Add(footnotes);
        if (main.EndnotesPart is { } endnotes) parts.Add(endnotes);
        if (main.WordprocessingCommentsPart is { } comments) parts.Add(comments);

        foreach (var part in parts)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var content = xml.Read(part)!;
            if (content.Descendants(W + "tbl").Any()) warnings.Add("tables");
            if (content.Descendants(A + "t").Any() ||
                content.Descendants(W + "altChunk").Any() ||
                content.Descendants(W + "object").Any()) warnings.Add("objects");

            int paragraphNumber = 0;
            foreach (var paragraph in content.Descendants(W + "p"))
            {
                cancellationToken.ThrowIfCancellationRequested();
                string source = $"{part.Uri}: p. {++paragraphNumber}";
                foreach (var run in paragraph.Descendants(W + "r")
                    .Where(r => r.Ancestors(W + "p").FirstOrDefault() == paragraph))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    foreach (var font in OfficeWordRunReader.Read(run, paragraph,
                        styles, theme, main.FontTablePart, fontTable, source, warnings))
                    {
                        if (fonts.Count >= 100_000)
                            throw new InvalidDataException("Too many font uses.");
                        fonts.Add(font);
                    }
                }
            }
        }
        if (fonts.Count == 0) warnings.Add("empty");
        return new OfficeFontDocument(fonts.Distinct().ToArray(), warnings.Order().ToArray());
    }
}