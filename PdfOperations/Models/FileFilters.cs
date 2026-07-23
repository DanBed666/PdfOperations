namespace PdfOperations;

public class FileFilters
{
    public const string LibreOfficeFiles =
        "LibreOffice supported files (*.pdf;*.doc;*.docx;*.odt;*.rtf;*.txt;*.xls;*.xlsx;*.ods;*.csv;*.ppt;*.pptx;*.odp;*.odg;*.fodg;*.svg;*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff)|*.pdf;*.doc;*.docx;*.odt;*.rtf;*.txt;*.xls;*.xlsx;*.ods;*.csv;*.ppt;*.pptx;*.odp;*.odg;*.fodg;*.svg;*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff|" +
        "PDF files (*.pdf)|*.pdf|" +
        "Text files (*.txt)|*.txt|" +
        "Word files (*.doc;*.docx;*.odt;*.rtf)|*.doc;*.docx;*.odt;*.rtf|" +
        "Spreadsheets (*.xls;*.xlsx;*.ods;*.csv)|*.xls;*.xlsx;*.ods;*.csv|" +
        "Presentations (*.ppt;*.pptx;*.odp)|*.ppt;*.pptx;*.odp|" +
        "Draw files (*.odg;*.fodg;*.svg)|*.odg;*.fodg;*.svg|" +
        "Image files (*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff)|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff|" +
        "All files (*.*)|*.*";
    
    public const string PdfFiles =
        "Pliki PDF (*.pdf*)|*.pdf*|" +
        "All files (*.*)|*.*";
    
    public const string PictFiles =
        "Image files (*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.bmp)|*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.bmp|" +
        "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
        "PNG files (*.png)|*.png|" +
        "TIFF files (*.tif;*.tiff)|*.tif;*.tiff|" +
        "All files (*.*)|*.*";
}