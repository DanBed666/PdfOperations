using System.Xml.Linq;

namespace PdfOperations;

internal static class OfficeFontTheme
{
    private static readonly XNamespace Drawing =
        "http://schemas.openxmlformats.org/drawingml/2006/main";

    public static string? Resolve(
        XDocument? theme,
        string? reference)
    {
        if (theme is null || string.IsNullOrWhiteSpace(reference))
            return null;

        var selection = reference.Trim() switch
        {
            "majorAscii" or "majorHAnsi" or "+mj-lt"
                => (Group: "majorFont", Script: "latin"),

            "minorAscii" or "minorHAnsi" or "+mn-lt"
                => (Group: "minorFont", Script: "latin"),

            "majorEastAsia" or "+mj-ea"
                => (Group: "majorFont", Script: "ea"),

            "minorEastAsia" or "+mn-ea"
                => (Group: "minorFont", Script: "ea"),

            "majorBidi" or "+mj-cs"
                => (Group: "majorFont", Script: "cs"),

            "minorBidi" or "+mn-cs"
                => (Group: "minorFont", Script: "cs"),

            _ => (Group: "", Script: "")
        };

        if (selection.Group.Length == 0)
            return null;

        var font = theme.Root?
            .Element(Drawing + "themeElements")?
            .Element(Drawing + "fontScheme")?
            .Element(Drawing + selection.Group)?
            .Element(Drawing + selection.Script);

        string? name = ((string?)font?.Attribute("typeface"))?.Trim();

        return string.IsNullOrWhiteSpace(name) ? null : name;
    }
}