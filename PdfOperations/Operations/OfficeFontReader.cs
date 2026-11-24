namespace PdfOperations;

public static class OfficeFontReader
{
    public static OfficeFontDocument Read(
        string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".docx" => OfficeWordFontReader.Read(path, cancellationToken),
            ".pptx" => OfficePresentationFontReader.Read(path, cancellationToken),
            _ => throw new NotSupportedException("Supported formats: DOCX, PPTX.")
        };
    }
}