using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PdfOperations;

internal static class OfficePresentationStyles
{
    private static readonly XNamespace A =
        "http://schemas.openxmlformats.org/drawingml/2006/main";
    private static readonly XNamespace P =
        "http://schemas.openxmlformats.org/presentationml/2006/main";

    public static IReadOnlyList<XElement> GetLayers(
        XElement run, XElement paragraph, XDocument? layout,
        XDocument? master, XDocument presentation)
    {
        var layers = new List<XElement>();
        int level = OfficePresentationFormatting.ReadLevel(paragraph);
        var shape = paragraph.Ancestors(P + "sp").FirstOrDefault();
        var placeholder = shape?.Element(P + "nvSpPr")?
            .Element(P + "nvPr")?.Element(P + "ph");

        string index = (string?)placeholder?.Attribute("idx") ?? "0";
        var layoutShape = placeholder is null ? null :
            FindPlaceholder(layout, "idx", index, "0");
        var layoutPlaceholder = layoutShape?.Element(P + "nvSpPr")?
            .Element(P + "nvPr")?.Element(P + "ph");
        string type = (string?)placeholder?.Attribute("type")
            ?? (string?)layoutPlaceholder?.Attribute("type") ?? "obj";
        string masterType = type switch
        {
            "ctrTitle" => "title",
            "obj" or "subTitle" => "body",
            _ => type
        };
        var masterShape = placeholder is null ? null :
            FindPlaceholder(master, "type", masterType, "obj");
        string masterStyle = placeholder is null ? "otherStyle" :
            masterType == "title" ? "titleStyle" :
            masterType == "body" ? "bodyStyle" : "otherStyle";

        Add(run.Element(A + "rPr"));
        Add(paragraph.Element(A + "pPr")?.Element(A + "defRPr"));
        AddShape(shape, false);
        AddShape(layoutShape, true);
        AddShape(masterShape, true);
        layers.AddRange(OfficePresentationFormatting.GetLevelProperties(
            master?.Root?.Element(P + "txStyles")?.Element(P + masterStyle), level));
        layers.AddRange(OfficePresentationFormatting.GetLevelProperties(
            presentation.Root?.Element(P + "defaultTextStyle"), level));
        return layers;

        void Add(XElement? properties)
        {
            if (properties is not null) layers.Add(properties);
        }
        void AddShape(XElement? source, bool includeParagraph)
        {
            var body = source?.Element(P + "txBody");
            if (includeParagraph)
                Add(body?.Elements(A + "p").FirstOrDefault(p =>
                    OfficePresentationFormatting.ReadLevel(p) == level)?
                    .Element(A + "pPr")?.Element(A + "defRPr"));
            layers.AddRange(OfficePresentationFormatting.GetLevelProperties(
                body?.Element(A + "lstStyle"), level));
        }
    }

    private static XElement? FindPlaceholder(
        XDocument? document, string attribute, string value, string defaultValue) =>
        document?.Descendants(P + "sp").FirstOrDefault(shape =>
            shape.Element(P + "nvSpPr")?.Element(P + "nvPr")?
                .Element(P + "ph") is { } placeholder &&
            ((string?)placeholder.Attribute(attribute) ?? defaultValue) == value);
}