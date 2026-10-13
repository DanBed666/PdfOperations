using CommunityToolkit.Mvvm.ComponentModel;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.Services;

public partial class LocalizationService : ObservableObject
{
    public static LocalizationService Instance { get; } = new();

    [ObservableProperty]
    private AppLanguage language = AppLanguage.Polish;

    public string Browse => Language == AppLanguage.Polish ? "Przeglądaj" : "Browse";
    public string Add => Language == AppLanguage.Polish ? "Dodaj" : "Add";
    public string Remove => Language == AppLanguage.Polish ? "Usuń" : "Remove";
    public string Clear => Language == AppLanguage.Polish ? "Wyczyść" : "Clear";
    public string Start => "Start";
    
    public string MoveUp => Language == AppLanguage.Polish ? "W górę" : "Up";
    public string MoveDown => Language == AppLanguage.Polish ? "W dół" : "Down";

    public string OpenOutputFolder => Language == AppLanguage.Polish ? "Otwórz folder wynikowy" : "Open output folder";

    public string InputFiles => Language == AppLanguage.Polish ? "Pliki wejściowe" : "Input files";
    public string InputPdfFiles => Language == AppLanguage.Polish ? "Pliki PDF wejściowe" : "Input PDF files";
    public string InputDocumentFiles => Language == AppLanguage.Polish ? "Dokumenty wejściowe" : "Input document files";
    public string OutputDirectory => Language == AppLanguage.Polish ? "Folder wynikowy" : "Output directory";
    public string OutputFileName => Language == AppLanguage.Polish ? "Nazwa pliku wynikowego" : "Output file name";
    public string OutputFormat => Language == AppLanguage.Polish ? "Format wynikowy" : "Output format";
    public string ReportFileName => Language == AppLanguage.Polish ? "Nazwa pliku raportu" : "Report file name";
    public string SearchPhrase => Language == AppLanguage.Polish ? "Szukana fraza" : "Search phrase";
    public string LinesBefore => Language == AppLanguage.Polish ? "Linie przed" : "Lines before";
    public string LinesAfter => Language == AppLanguage.Polish ? "Linie po" : "Lines after";
    public string PageMode => Language == AppLanguage.Polish ? "Tryb stron" : "Page mode";
    public string Pages => Language == AppLanguage.Polish ? "Strony" : "Pages";
    public string SplitAfterPages => Language == AppLanguage.Polish ? "Podziel po stronach" : "Split after pages";
    public string ExamplePages => Language == AppLanguage.Polish ? "Przykład: 1,3-5" : "Example: 1,3-5";
    public string ExampleSplitAfterPages => Language == AppLanguage.Polish ? "Przykład: 4,10,15" : "Example: 4,10,15";
    public string ExcelReplacementFile => Language == AppLanguage.Polish ? "Plik Excel z zamianami" : "Excel replacement file";
    public string Replacement => Language == AppLanguage.Polish ? "Zamiana tekstu" : "Replacement";
    public string LibreOfficeConversion => Language == AppLanguage.Polish ? "Konwersja LibreOffice" : "LibreOffice conversion";
    public string FragmentPdfFile => Language == AppLanguage.Polish ? "Plik PDF fragmentu" : "Fragment PDF file";
    public string AddFragment => Language == AppLanguage.Polish ? "Dodaj fragment" : "Add fragment";
    public string RemoveSelected => Language == AppLanguage.Polish ? "Usuń wybrany" : "Remove selected";
    public string Fragments => Language == AppLanguage.Polish ? "Fragmenty" : "Fragments";
    public string BuildPdfFromFragments => Language == AppLanguage.Polish ? "Zbuduj PDF z fragmentów" : "Build PDF from fragments";

    public string Ready => Language == AppLanguage.Polish ? "Gotowe" : "Ready";
    public string SelectInputFile => Language == AppLanguage.Polish ? "Wybierz plik wejściowy." : "Select input file.";
    public string SelectInputFiles => Language == AppLanguage.Polish ? "Wybierz pliki wejściowe." : "Select input files.";
    public string SelectPdfFile => Language == AppLanguage.Polish ? "Wybierz plik PDF." : "Select PDF file.";
    public string SelectPdfFiles => Language == AppLanguage.Polish ? "Wybierz pliki PDF." : "Select PDF files.";
    public string SelectDocumentFiles => Language == AppLanguage.Polish ? "Wybierz dokumenty." : "Select document files.";
    public string SelectExcelReplacementFile => Language == AppLanguage.Polish ? "Wybierz plik Excel z zamianami." : "Select Excel replacement file.";
    public string SelectOutputDirectory => Language == AppLanguage.Polish ? "Wybierz folder wynikowy." : "Select output directory.";
    public string SelectOutputFormat => Language == AppLanguage.Polish ? "Wybierz format wynikowy." : "Select output format.";
    public string OutputDirectoryDoesNotExist => Language == AppLanguage.Polish ? "Folder wynikowy nie istnieje." : "Output directory does not exist.";
    public string EnterSearchPhrase => Language == AppLanguage.Polish ? "Wpisz szukaną frazę." : "Enter search phrase.";
    public string EnterPages => Language == AppLanguage.Polish ? "Wpisz strony, na przykład: 1,3-5." : "Enter pages, for example: 1,3-5.";
    public string InvalidPagesFormat => Language == AppLanguage.Polish ? "Nieprawidłowy format stron. Użyj na przykład: 1,3-5." : "Invalid pages format. Use for example: 1,3-5.";
    public string EnterSplitPages => Language == AppLanguage.Polish ? "Wpisz strony podziału, na przykład: 4,10,15." : "Enter split pages, for example: 4,10,15.";
    public string Searching => Language == AppLanguage.Polish ? "Wyszukiwanie..." : "Searching...";
    public string ReadingPdfInfo => Language == AppLanguage.Polish ? "Odczytywanie informacji PDF..." : "Reading PDF info...";
    public string ReplacingText => Language == AppLanguage.Polish ? "Zamienianie tekstu..." : "Replacing text...";
    public string CreatingPdfWithSelectedPages => Language == AppLanguage.Polish ? "Tworzenie PDF z wybranymi stronami..." : "Creating PDF with selected pages...";
    public string BuildingPdfFromFragments => Language == AppLanguage.Polish ? "Budowanie PDF z fragmentów..." : "Building PDF from fragments...";
    public string FragmentAdded => Language == AppLanguage.Polish ? "Dodano fragment." : "Fragment added.";
    public string FragmentRemoved => Language == AppLanguage.Polish ? "Usunięto fragment." : "Fragment removed.";
    public string FragmentsCleared => Language == AppLanguage.Polish ? "Wyczyszczono fragmenty." : "Fragments cleared.";
    public string FragmentMovedUp => Language == AppLanguage.Polish ? "Przesunięto fragment w górę." : "Fragment moved up.";
    public string FragmentMovedDown => Language == AppLanguage.Polish ? "Przesunięto fragment w dół." : "Fragment moved down.";
    public string AddAtLeastOneFragment => Language == AppLanguage.Polish ? "Dodaj co najmniej jeden fragment." : "Add at least one fragment.";
    public string NoOutputFolderToOpen => Language == AppLanguage.Polish ? "Brak folderu wynikowego do otwarcia." : "No output folder to open.";

    public string GetOperationName(string key)
    {
        return key switch
        {
            "PdfToTxt" => "PDF to TXT",
            "PdfToDocx" => "PDF to DOCX",
            "DocxToPdfWord" => Language == AppLanguage.Polish ? "DOCX do PDF (Word)" : "DOCX to PDF word",
            "PdfToImage" => Language == AppLanguage.Polish ? "PDF do obrazów" : "PDF to Image",
            "ImagesToPdf" => Language == AppLanguage.Polish ? "Obrazy do PDF" : "Images to PDF",
            "ImagesToTxt" => Language == AppLanguage.Polish ? "Obrazy do TXT" : "Images to TXT",
            "ExtractFromPdf" => Language == AppLanguage.Polish ? "Wyciągnij obrazy z PDF" : "Extract from PDF",
            "LibreOffice" => Language == AppLanguage.Polish ? "Konwersja LibreOffice" : "LibreOffice conversion",
            "SplitPdf" => Language == AppLanguage.Polish ? "Podziel PDF" : "Split PDF",
            "MergePdf" => Language == AppLanguage.Polish ? "Połącz PDF" : "Merge PDF",
            "Search" => Language == AppLanguage.Polish ? "Wyszukiwanie" : "Search",
            "Info" => "Info",
            "FontInfo" => Language == AppLanguage.Polish ? "Info o czcionkach" : "Font info",
            "Replacement" => Language == AppLanguage.Polish ? "Zamiana tekstu" : "Replacement",
            "Fragments" => Language == AppLanguage.Polish ? "Zbuduj PDF z fragmentów" : "Build PDF from fragments",
            "Help" => Language == AppLanguage.Polish ? "Pomoc" : "Help",
            _ => key
        };
    }

    public string MissingInputFiles(string files)
    {
        return Language == AppLanguage.Polish
            ? $"Brak plików wejściowych: {files}"
            : $"Missing input files: {files}";
    }

    public string Starting(string title)
    {
        return Language == AppLanguage.Polish
            ? $"Rozpoczynanie: {title}..."
            : $"Starting {title}...";
    }

    public string ConvertingTo(string format)
    {
        return Language == AppLanguage.Polish
            ? $"Konwersja do {format}..."
            : $"Converting to {format}...";
    }

    public string CannotReadPdfPageCount(string message)
    {
        return Language == AppLanguage.Polish
            ? $"Nie można odczytać liczby stron PDF: {message}"
            : $"Cannot read PDF page count: {message}";
    }

    public string NoPagesSelected(string file)
    {
        return Language == AppLanguage.Polish
            ? $"Nie wybrano stron dla {file}."
            : $"No pages selected for {file}.";
    }

    public string PageRangeOutsideDocument(string range, int pageCount)
    {
        return Language == AppLanguage.Polish
            ? $"Zakres stron {range} jest poza liczbą stron dokumentu ({pageCount})."
            : $"Page range {range} is outside document page count ({pageCount}).";
    }

    public string PageOutsideDocument(int page, int pageCount)
    {
        return Language == AppLanguage.Polish
            ? $"Strona {page} jest poza liczbą stron dokumentu ({pageCount})."
            : $"Page {page} is outside document page count ({pageCount}).";
    }

    public string InvalidPageRange(string range)
    {
        return Language == AppLanguage.Polish
            ? $"Nieprawidłowy zakres stron: {range}."
            : $"Invalid page range: {range}.";
    }

    public string InvalidSplitPage(string page)
    {
        return Language == AppLanguage.Polish
            ? $"Nieprawidłowa strona podziału: {page}."
            : $"Invalid split page: {page}.";
    }

    public string SplitPageMustBeGreaterThanZero => Language == AppLanguage.Polish
        ? "Strona podziału musi być większa niż 0."
        : "Split page must be greater than 0.";

    public string SplitPageMustBeSmallerThanPageCount(int pageCount)
    {
        return Language == AppLanguage.Polish
            ? $"Strona podziału musi być mniejsza niż liczba stron ({pageCount})."
            : $"Split page must be smaller than page count ({pageCount}).";
    }

    public string SplitPagesCannotContainDuplicates => Language == AppLanguage.Polish
        ? "Strony podziału nie mogą się powtarzać."
        : "Split pages cannot contain duplicates.";

    public string SplitPagesMustBeAscending => Language == AppLanguage.Polish
        ? "Strony podziału muszą być rosnąco."
        : "Split pages must be in ascending order.";

    public string PagesInfo(int pageCount)
    {
        return Language == AppLanguage.Polish
            ? $"strony: {pageCount}"
            : $"pages: {pageCount}";
    }

    public string CannotReadPages(string message)
    {
        return Language == AppLanguage.Polish
            ? $"nie można odczytać stron: {message}"
            : $"cannot read pages: {message}";
    }

    public string DoneSaved(string path)
    {
        return Language == AppLanguage.Polish
            ? $"Gotowe. Zapisano: {path}"
            : $"Done. Saved: {path}";
    }

    public string DoneSavedFiles(int count, string directory)
    {
        return Language == AppLanguage.Polish
            ? $"Gotowe. Zapisano {count} plików do: {directory}"
            : $"Done. Saved {count} files to: {directory}";
    }

    public string DoneSavedFilesTo(string directory)
    {
        return Language == AppLanguage.Polish
            ? $"Gotowe. Zapisano pliki do: {directory}"
            : $"Done. Saved files to: {directory}";
    }

    public string DoneSavedReport(string path)
    {
        return Language == AppLanguage.Polish
            ? $"Gotowe. Zapisano raport: {path}"
            : $"Done. Saved report: {path}";
    }

    public string Error(string message)
    {
        return Language == AppLanguage.Polish
            ? $"Błąd: {message}"
            : $"Error: {message}";
    }

    partial void OnLanguageChanged(AppLanguage value)
    {
        OnPropertyChanged(string.Empty);
    }
}
