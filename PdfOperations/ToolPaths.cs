using System.Collections.ObjectModel;

namespace PdfOperations;

public class ToolPaths
{
    private static readonly string ToolsDir = Path.Combine(AppContext.BaseDirectory, "tools");

    public static readonly IReadOnlyDictionary<Tool, string> ToolPathsDict =
        new ReadOnlyDictionary<Tool, string>(new Dictionary<Tool, string>
            {
                [Tool.LibreOffice] = Path.Combine(ToolsDir, "LibreOfficePortable", "App",
                        "libreoffice", "program", "soffice.com"),
                [Tool.PdfToPpm] = Path.Combine(ToolsDir, "poppler-26.02.0", "Library",
                    "bin", "pdftoppm.exe"),
                [Tool.Magick] = Path.Combine(ToolsDir, "ImageMagick-7.1.2-26-portable-Q16-x64", 
                    "magick.exe"),
                [Tool.PdfUnite] = Path.Combine(ToolsDir, "poppler-26.02.0", "Library",
                    "bin", "pdfunite.exe"),
                [Tool.PdfToText] = Path.Combine(ToolsDir, "poppler-26.02.0", "Library",
                    "bin", "pdftotext.exe"),
                [Tool.Tesseract] = Path.Combine(ToolsDir, "Tesseract-OCR", "tesseract.exe"),
                [Tool.Qpdf] = Path.Combine(ToolsDir, "qpdf-12.3.2-msvc64", "bin", "qpdf.exe"),
                [Tool.PdfInfo] = Path.Combine(ToolsDir, "poppler-26.02.0", "Library",
                    "bin", "pdfinfo.exe"), 
                [Tool.PdfSeparate] = Path.Combine(ToolsDir, "poppler-26.02.0", "Library",
                    "bin", "pdfseparate.exe"),
            }
        );
}