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

    public string OpenOutputFolder => Language == AppLanguage.Polish ? "Otwórz folder z wynikami" : "Open output folder";

    public string InputFiles => Language == AppLanguage.Polish ? "Pliki wejściowe" : "Input files";
    public string InputPdfFiles => Language == AppLanguage.Polish ? "Pliki PDF" : "Input PDF files";
    public string InputDocumentFiles => Language == AppLanguage.Polish ? "Dokumenty" : "Input document files";
    public string OutputDirectory => Language == AppLanguage.Polish ? "Folder zapisu" : "Output directory";
    public string OutputFileName => Language == AppLanguage.Polish ? "Nazwa pliku" : "Output file name";
    public string OutputFormat => Language == AppLanguage.Polish ? "Format pliku" : "Output format";
    public string ReportFileName => Language == AppLanguage.Polish ? "Nazwa raportu" : "Report file name";
    public string SearchPhrase => Language == AppLanguage.Polish ? "Szukana fraza" : "Search phrase";
    public string LinesBefore => Language == AppLanguage.Polish ? "Liczba linii przed" : "Lines before";
    public string LinesAfter => Language == AppLanguage.Polish ? "Liczba linii po" : "Lines after";
    public string PageMode => Language == AppLanguage.Polish ? "Sposób wyboru stron" : "Page mode";
    public string Pages => Language == AppLanguage.Polish ? "Strony" : "Pages";
    public string SplitAfterPages => Language == AppLanguage.Polish ? "Podziel po podanych stronach" : "Split after pages";
    public string ExamplePages => Language == AppLanguage.Polish ? "Przykład: 1,3-5" : "Example: 1,3-5";
    public string ExampleSplitAfterPages => Language == AppLanguage.Polish ? "Przykład: 4,10,15" : "Example: 4,10,15";
    public string ExcelReplacementFile => Language == AppLanguage.Polish ? "Arkusz Excel z zamianami" : "Excel replacement file";
    public string Replacement => Language == AppLanguage.Polish ? "Zamiana tekstu" : "Replacement";
    public string LibreOfficeConversion => Language == AppLanguage.Polish ? "Konwersja LibreOffice" : "LibreOffice conversion";
    public string FragmentPdfFile => Language == AppLanguage.Polish ? "Plik PDF fragmentu" : "Fragment PDF file";
    public string AddFragment => Language == AppLanguage.Polish ? "Dodaj fragment" : "Add fragment";
    public string RemoveSelected => Language == AppLanguage.Polish ? "Usuń wybrany" : "Remove selected";
    public string Fragments => Language == AppLanguage.Polish ? "Fragmenty" : "Fragments";
    public string BuildPdfFromFragments => Language == AppLanguage.Polish ? "Utwórz PDF z fragmentów" : "Build PDF from fragments";

    public string Ready => Language == AppLanguage.Polish ? "Gotowe" : "Ready";
    public string SelectInputFile => Language == AppLanguage.Polish ? "Wybierz plik do przetworzenia." : "Select input file.";
    public string SelectInputFiles => Language == AppLanguage.Polish ? "Wybierz pliki do przetworzenia." : "Select input files.";
    public string SelectPdfFile => Language == AppLanguage.Polish ? "Wybierz plik PDF." : "Select PDF file.";
    public string SelectPdfFiles => Language == AppLanguage.Polish ? "Wybierz pliki PDF." : "Select PDF files.";
    public string SelectDocumentFiles => Language == AppLanguage.Polish ? "Wybierz dokumenty do przetworzenia." : "Select document files.";
    public string SelectExcelReplacementFile => Language == AppLanguage.Polish ? "Wybierz arkusz Excel z zamianami." : "Select Excel replacement file.";
    public string SelectOutputDirectory => Language == AppLanguage.Polish ? "Wybierz folder zapisu." : "Select output directory.";
    public string SelectOutputFormat => Language == AppLanguage.Polish ? "Wybierz format pliku." : "Select output format.";
    public string OutputDirectoryDoesNotExist => Language == AppLanguage.Polish ? "Podany folder zapisu nie istnieje." : "Output directory does not exist.";
    public string EnterSearchPhrase => Language == AppLanguage.Polish ? "Wpisz szukaną frazę." : "Enter search phrase.";
    public string EnterPages => Language == AppLanguage.Polish ? "Wpisz strony, na przykład: 1,3-5." : "Enter pages, for example: 1,3-5.";
    public string InvalidPagesFormat => Language == AppLanguage.Polish ? "Nieprawidłowy format stron. Użyj na przykład: 1,3-5." : "Invalid pages format. Use for example: 1,3-5.";
    public string EnterSplitPages => Language == AppLanguage.Polish ? "Wpisz strony podziału, na przykład: 4,10,15." : "Enter split pages, for example: 4,10,15.";
    public string Searching => Language == AppLanguage.Polish ? "Wyszukiwanie..." : "Searching...";
    public string ReadingPdfInfo => Language == AppLanguage.Polish ? "Odczytywanie informacji o PDF..." : "Reading PDF info...";
    public string ReplacingText => Language == AppLanguage.Polish ? "Trwa zamiana tekstu..." : "Replacing text...";
    public string CreatingPdfWithSelectedPages => Language == AppLanguage.Polish ? "Tworzenie PDF z wybranymi stronami..." : "Creating PDF with selected pages...";
    public string BuildingPdfFromFragments => Language == AppLanguage.Polish ? "Tworzenie PDF z fragmentów..." : "Building PDF from fragments...";
    public string FragmentAdded => Language == AppLanguage.Polish ? "Dodano fragment." : "Fragment added.";
    public string FragmentRemoved => Language == AppLanguage.Polish ? "Usunięto fragment." : "Fragment removed.";
    public string FragmentsCleared => Language == AppLanguage.Polish ? "Wyczyszczono fragmenty." : "Fragments cleared.";
    public string FragmentMovedUp => Language == AppLanguage.Polish ? "Przesunięto fragment w górę." : "Fragment moved up.";
    public string FragmentMovedDown => Language == AppLanguage.Polish ? "Przesunięto fragment w dół." : "Fragment moved down.";
    public string AddAtLeastOneFragment => Language == AppLanguage.Polish ? "Dodaj co najmniej jeden fragment." : "Add at least one fragment.";
    public string NoOutputFolderToOpen => Language == AppLanguage.Polish ? "Nie ma jeszcze folderu z wynikami do otwarcia." : "No output folder to open.";
    public string CustomPages => Language == AppLanguage.Polish
        ? "Wybrane strony"
        : "Custom pages";

    public string EvenPages => Language == AppLanguage.Polish
        ? "Strony parzyste"
        : "Even pages";

    public string OddPages => Language == AppLanguage.Polish
        ? "Strony nieparzyste"
        : "Odd pages";

    public string SplitByPages => Language == AppLanguage.Polish
        ? "Podziel po podanych stronach"
        : "Split by pages";

    public string SplitEveryPage => Language == AppLanguage.Polish
        ? "Podziel każdą stronę osobno"
        : "Split every page";
    
    public string GetPageModeName(PageSelectionMode mode)
    {
        return mode switch
        {
            PageSelectionMode.CustomPages => CustomPages,
            PageSelectionMode.EvenPages => EvenPages,
            PageSelectionMode.OddPages => OddPages,
            PageSelectionMode.SplitByPages => SplitByPages,
            PageSelectionMode.SplitEveryPage => SplitEveryPage,
            _ => mode.ToString()
        };
    }

    public string GetOperationName(string key)
    {
        return key switch
        {
            "PdfToTxt" => "PDF to TXT",
            "PdfToDocx" => "PDF to DOCX",
            "DocxToPdfWord" => Language == AppLanguage.Polish ? "DOCX do PDF (Word)" : "DOCX to PDF word",
            "PdfToImage" => Language == AppLanguage.Polish ? "PDF na obrazy" : "PDF to Image",
            "ImagesToPdf" => Language == AppLanguage.Polish ? "Obrazy na PDF" : "Images to PDF",
            "ImagesToTxt" => Language == AppLanguage.Polish ? "Obrazy na TXT" : "Images to TXT",
            "ExtractFromPdf" => Language == AppLanguage.Polish ? "Wyodrębnij obrazy z PDF" : "Extract from PDF",
            "LibreOffice" => Language == AppLanguage.Polish ? "Konwersja LibreOffice" : "LibreOffice conversion",
            "SplitPdf" => Language == AppLanguage.Polish ? "Podziel PDF" : "Split PDF",
            "MergePdf" => Language == AppLanguage.Polish ? "Połącz PDF" : "Merge PDF",
            "Search" => Language == AppLanguage.Polish ? "Szukaj" : "Search",
            "Info" => Language == AppLanguage.Polish ? "Informacje" : "Info",
            "FontInfo" => Language == AppLanguage.Polish ? "Informacje o czcionkach" : "Font info",
            "Replacement" => Language == AppLanguage.Polish ? "Zamiana tekstu" : "Replacement",
            "Fragments" => Language == AppLanguage.Polish ? "Utwórz PDF z fragmentów" : "Build PDF from fragments",
            "Help" => Language == AppLanguage.Polish ? "Pomoc" : "Help",
            _ => key
        };
    }

    public string MissingInputFiles(string files)
    {
        return Language == AppLanguage.Polish
            ? $"Nie znaleziono plików: {files}"
            : $"Missing input files: {files}";
    }

    public string Starting(string title)
    {
        return Language == AppLanguage.Polish
            ? $"Uruchamianie: {title}..."
            : $"Starting {title}...";
    }

    public string ConvertingTo(string format)
    {
        return Language == AppLanguage.Polish
            ? $"Konwertowanie do formatu {format}..."
            : $"Converting to {format}...";
    }

    public string CannotReadPdfPageCount(string message)
    {
        return Language == AppLanguage.Polish
            ? $"Nie udało się odczytać liczby stron PDF: {message}"
            : $"Cannot read PDF page count: {message}";
    }

    public string NoPagesSelected(string file)
    {
        return Language == AppLanguage.Polish
            ? $"Nie wybrano żadnych stron z pliku {file}."
            : $"No pages selected for {file}.";
    }

    public string PageRangeOutsideDocument(string range, int pageCount)
    {
        return Language == AppLanguage.Polish
            ? $"Zakres stron {range} wykracza poza dokument. Liczba stron: {pageCount}."
            : $"Page range {range} is outside document page count ({pageCount}).";
    }

    public string PageOutsideDocument(int page, int pageCount)
    {
        return Language == AppLanguage.Polish
            ? $"Strona {page} wykracza poza dokument. Liczba stron: {pageCount}."
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
            ? $"Strona podziału musi być mniejsza niż liczba stron w dokumencie ({pageCount})."
            : $"Split page must be smaller than page count ({pageCount}).";
    }

    public string SplitPagesCannotContainDuplicates => Language == AppLanguage.Polish
        ? "Strony podziału nie mogą się powtarzać."
        : "Split pages cannot contain duplicates.";

    public string SplitPagesMustBeAscending => Language == AppLanguage.Polish
        ? "Strony podziału muszą być podane rosnąco."
        : "Split pages must be in ascending order.";

    public string PagesInfo(int pageCount)
    {
        return Language == AppLanguage.Polish
            ? $"liczba stron: {pageCount}"
            : $"pages: {pageCount}";
    }

    public string CannotReadPages(string message)
    {
        return Language == AppLanguage.Polish
            ? $"nie udało się odczytać liczby stron: {message}"
            : $"cannot read pages: {message}";
    }

    public string DoneSaved(string path)
    {
        return Language == AppLanguage.Polish
            ? $"Gotowe. Zapisano plik: {path}"
            : $"Done. Saved: {path}";
    }

    public string DoneSavedFiles(int count, string directory)
    {
        return Language == AppLanguage.Polish
            ? $"Gotowe. Zapisano {count} plików w folderze: {directory}"
            : $"Done. Saved {count} files to: {directory}";
    }

    public string DoneSavedFilesTo(string directory)
    {
        return Language == AppLanguage.Polish
            ? $"Gotowe. Zapisano pliki w folderze: {directory}"
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
