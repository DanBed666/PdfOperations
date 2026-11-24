using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PdfOperations;

internal static class OfficeWordFormatting
{
    private static readonly XNamespace W =
        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    public static IReadOnlyList<XElement> GetLayers(
        XElement run, XElement paragraph, OfficeWordStyles styles)
    {
        var layers = new List<XElement>();
        var direct = run.Element(W + "rPr");
        if (direct is not null)
            layers.Add(direct);

        string? characterStyle =
            (string?)direct?.Element(W + "rStyle")?.Attribute(W + "val");
        layers.AddRange(styles.GetProperties(characterStyle));

        string? paragraphStyle = (string?)paragraph
            .Element(W + "pPr")?.Element(W + "pStyle")?.Attribute(W + "val");
        layers.AddRange(styles.GetProperties(
            paragraphStyle ?? styles.DefaultParagraphStyle));

        if (styles.DefaultRunProperties is { } defaults)
            layers.Add(defaults);
        return layers;
    }

    public static string? ResolveFamily(
        IEnumerable<XElement> layers, string script, XDocument? theme)
    {
        foreach (var layer in layers)
        {
            var fonts = layer.Element(W + "rFonts");
            string? reference = (string?)fonts?.Attribute(W + (script + "Theme"));
            if (reference is not null)
                return OfficeFontTheme.Resolve(theme, reference);

            string? family = (string?)fonts?.Attribute(W + script);
            if (!string.IsNullOrWhiteSpace(family))
                return family.Trim();
        }
        return null;
    }

    public static bool ResolveToggle(
        XElement run, IReadOnlyList<XElement> layers,
        XElement? defaults, string property)
    {
        var direct = run.Element(W + "rPr");
        bool value = Enabled(defaults?.Element(W + property));
        foreach (var layer in layers.Reverse())
        {
            if (ReferenceEquals(layer, direct) || ReferenceEquals(layer, defaults))
                continue;
            if (Enabled(layer.Element(W + property)))
                value = !value;
        }
        return direct?.Element(W + property) is { } flag ? Enabled(flag) : value;
    }

    private static bool Enabled(XElement? element) =>
        element is not null &&
        (string?)element.Attribute(W + "val") is null or "1" or "true" or "on";
}
