using System;
using System.Reflection;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public string Title => Loc.GetOperationName("About");
    public string Author => "DanBed666";
    public Uri RepositoryUrl { get; } = new("https://github.com/DanBed666/PdfOperations");
    public Uri CodexUrl { get; } = new("https://openai.com/codex/");

    public string AppVersion { get; } = typeof(AboutViewModel).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion.Split('+')[0]
        ?? typeof(AboutViewModel).Assembly.GetName().Version?.ToString(3)
        ?? "";

    public ProjectCredit[] Libraries { get; } =
    [
        new("Avalonia", new Uri("https://github.com/AvaloniaUI/Avalonia")),
        new("CommunityToolkit.Mvvm", new Uri("https://github.com/CommunityToolkit/dotnet")),
        new("ClosedXML", new Uri("https://github.com/ClosedXML/ClosedXML")),
        new("Inter", new Uri("https://rsms.me/inter/"))
    ];

    public ProjectCredit[] Tools { get; } =
    [
        new("LibreOffice", new Uri("https://www.libreoffice.org/")),
        new("Poppler", new Uri("https://poppler.freedesktop.org/")),
        new("qpdf", new Uri("https://qpdf.sourceforge.io/")),
        new("Tesseract OCR", new Uri("https://github.com/tesseract-ocr/tesseract")),
        new("ImageMagick", new Uri("https://imagemagick.org/")),
        new("pdf2docx", new Uri("https://github.com/ArtifexSoftware/pdf2docx"))
    ];
}
