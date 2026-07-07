namespace PdfOperations;

public class ToFileFormat
{
    public static string ToFormatFile(FileExtension extension)
    {
        return extension switch 
        {
            FileExtension.Docx => "docx",
            FileExtension.Odt => "odt",
            FileExtension.Txt => "txt",
            FileExtension.Rtf => "rtf",
            FileExtension.Html => "html",
            FileExtension.Xlsx => "xlsx",
            FileExtension.Csv => "csv",
            FileExtension.Pptx => "pptx",
            FileExtension.Png => "png",
            FileExtension.Jpg => "jpg",
            _ => throw new ArgumentOutOfRangeException(nameof(extension), extension, null)
        };
    }
}