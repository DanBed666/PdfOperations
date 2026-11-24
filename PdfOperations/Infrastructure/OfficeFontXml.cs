using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace PdfOperations;

internal sealed class OfficeFontXml
{
    private readonly CancellationToken cancellationToken;
    private readonly Dictionary<OpenXmlPart, XDocument> cache = new();
    private long totalCharacters;

    public OfficeFontXml(CancellationToken cancellationToken)
    {
        this.cancellationToken = cancellationToken;
    }

    public XDocument? Read(OpenXmlPart? part)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (part is null)
            return null;

        if (cache.TryGetValue(part, out var existing))
            return existing;

        if (cache.Count >= 10_000)
            throw new InvalidDataException("Too many document parts.");

        using var stream = part.GetStream(FileMode.Open, FileAccess.Read);

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = 32_000_000
        };

        using var reader = XmlReader.Create(stream, settings);
        var document = XDocument.Load(reader);

        cancellationToken.ThrowIfCancellationRequested();

        totalCharacters += document
            .ToString(SaveOptions.DisableFormatting).Length;

        if (totalCharacters > 128_000_000)
            throw new InvalidDataException("Document XML exceeds the limit.");

        cache.Add(part, document);
        return document;
    }
}