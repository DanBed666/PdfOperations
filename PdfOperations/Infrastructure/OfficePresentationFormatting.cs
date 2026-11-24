using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PdfOperations;

internal static class OfficePresentationFormatting
{
    private static readonly XNamespace A =
        "http://schemas.openxmlformats.org/drawingml/2006/main";

    public static string? ResolveFamily(
        IEnumerable<XElement> layers,
        XDocument? theme,
        bool hasThemeOverride = false)
    {
        foreach (var layer in layers)
        {
            string? family = ((string?)layer
                .Element(A + "latin")?.Attribute("typeface"))?.Trim();

            if (string.IsNullOrWhiteSpace(family))
                continue;

            if (!family.StartsWith('+'))
                return family;

            return hasThemeOverride
                ? null
                : OfficeFontTheme.Resolve(theme, family);
        }

        return null;
    }

    public static bool ResolveFlag(
        IEnumerable<XElement> layers, string property)
    {
        foreach (var layer in layers)
        {
            string? value = (string?)layer.Attribute(property);
            if (value is not null)
                return value is "1" or "true" or "on";
        }

        return false;
    }

    public static int ReadLevel(XElement paragraph)
    {
        string? value = (string?)paragraph
            .Element(A + "pPr")?.Attribute("lvl");

        return int.TryParse(value, out int level) && level is >= 0 and <= 8
            ? level
            : 0;
    }

    public static IEnumerable<XElement> GetLevelProperties(
        XElement? listStyle, int level)
    {
        if (level is < 0 or > 8)
            throw new ArgumentOutOfRangeException(nameof(level));

        var specific = listStyle?
            .Element(A + $"lvl{level + 1}pPr")?.Element(A + "defRPr");
        if (specific is not null)
            yield return specific;

        var defaults = listStyle?
            .Element(A + "defPPr")?.Element(A + "defRPr");
        if (defaults is not null)
            yield return defaults;
    }
}