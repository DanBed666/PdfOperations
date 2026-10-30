namespace PdfOperations;

public static class OperationTimeouts
{
    public static TimeSpan WordToPdf => TimeSpan.FromSeconds(120);
    public static TimeSpan SearchText => TimeSpan.FromSeconds(120);
    public static TimeSpan Replacement => TimeSpan.FromSeconds(180);

    // Limits apply to each tool invocation, not to a whole batch of input files.
    public static TimeSpan LibreOffice => TimeSpan.FromSeconds(120);
    public static TimeSpan PdfToTxt => TimeSpan.FromSeconds(120);
    public static TimeSpan PdfToDocx => TimeSpan.FromSeconds(300);
    public static TimeSpan PdfToImages => TimeSpan.FromSeconds(300);
    public static TimeSpan ImageToTxt => TimeSpan.FromSeconds(300);
    public static TimeSpan ImageToPdf => TimeSpan.FromSeconds(120);
    public static TimeSpan ExtractImages => TimeSpan.FromSeconds(180);
    public static TimeSpan SplitPdf => TimeSpan.FromSeconds(180);
    public static TimeSpan MergePdf => TimeSpan.FromSeconds(300);
    public static TimeSpan DecryptPdf => TimeSpan.FromSeconds(120);
    public static TimeSpan SelectPages => TimeSpan.FromSeconds(180);
    public static TimeSpan BuildFragments => TimeSpan.FromSeconds(300);
    public static TimeSpan PdfInfo => TimeSpan.FromSeconds(30);
    public static TimeSpan PdfPageCount => TimeSpan.FromSeconds(30);
    public static TimeSpan PdfFontInfo => TimeSpan.FromSeconds(60);
}
