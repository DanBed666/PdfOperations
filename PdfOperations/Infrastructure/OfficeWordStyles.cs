using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace PdfOperations;

internal sealed class OfficeWordStyles
{
    private static readonly XNamespace W =
        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    private readonly Dictionary<string, XElement> styles;
    private readonly ISet<string> warnings;
    private readonly CancellationToken token;

    public XElement? DefaultRunProperties { get; }
    public string? DefaultParagraphStyle { get; }

    public OfficeWordStyles(
        XDocument? document,
        ISet<string> warnings,
        CancellationToken token)
    {
        this.warnings = warnings;
        this.token = token;

        styles = (document?.Root?.Elements(W + "style")
                  ?? Enumerable.Empty<XElement>())
            .Where(s => s.Attribute(W + "styleId") is not null)
            .GroupBy(s => (string)s.Attribute(W + "styleId")!)
            .ToDictionary(group => group.Key, group => group.First());

        DefaultRunProperties = document?.Root?
            .Element(W + "docDefaults")?
            .Element(W + "rPrDefault")?
            .Element(W + "rPr");

        DefaultParagraphStyle = styles.Values
            .Where(s => (string?)s.Attribute(W + "type") == "paragraph")
            .Where(s => (string?)s.Attribute(W + "default")
                        is "1" or "true" or "on")
            .Select(s => (string?)s.Attribute(W + "styleId"))
            .FirstOrDefault();
    }

    public IEnumerable<XElement> GetProperties(string? styleId)
    {
        var visited = new HashSet<string>();

        while (!string.IsNullOrEmpty(styleId))
        {
            token.ThrowIfCancellationRequested();

            if (!visited.Add(styleId) || visited.Count > 64 ||
                !styles.TryGetValue(styleId, out var style))
            {
                warnings.Add("styles");
                yield break;
            }

            if (style.Element(W + "rPr") is { } properties)
                yield return properties;

            styleId = (string?)style
                .Element(W + "basedOn")?.Attribute(W + "val");
        }
    }
}