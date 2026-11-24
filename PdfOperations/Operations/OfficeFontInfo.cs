namespace PdfOperations;

public enum FontAvailability
{
    Available,
    MissingFamily,
    MissingFace,
    Unknown
}

public sealed record OfficeFontUse(
    string Family,
    int Weight,
    bool Italic,
    string Source,
    bool Embedded);

public sealed record OfficeFontDocument(
    IReadOnlyList<OfficeFontUse> Fonts,
    IReadOnlyList<string> Warnings);

public interface IInstalledFontCatalog
{
    FontAvailability Check(
        string family,
        int weight,
        bool italic);
}