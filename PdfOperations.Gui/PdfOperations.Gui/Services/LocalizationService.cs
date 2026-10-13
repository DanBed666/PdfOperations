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

    partial void OnLanguageChanged(AppLanguage value)
    {
        OnPropertyChanged(string.Empty);
    }
}
