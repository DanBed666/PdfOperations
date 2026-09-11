using System.Collections.ObjectModel;

namespace PdfOperations;

public class OperationPaths
{
    public static IReadOnlyDictionary<int, OperationDefinition> OperationDefinitions =>
        new ReadOnlyDictionary<int, OperationDefinition>(new Dictionary<int, OperationDefinition>
        {
            [1] = new OperationDefinition
            {
                Name = "Konwersja plików dowolnego typu (LibreOffice)",
                Filter = FileFilters.LibreOfficeFiles,
                AddInfo = "format",
                OperationFlow = OperationFlow.FilesToFilesWithFormat,
                FileOperationActionLibre = Convert.FileToPdf,
                InputPrompt = "Wybierz pliki do konwersji przez LibreOffice:",
                FormatPrompt = "Podaj format pliku wynikowego:",
                OutputPrompt = "Podaj nazwę pliku wynikowego:",
                DefaultOutputName = "converted"
            },
            [2] = new OperationDefinition
            {
                Name = "Rozdziel Pdf na obrazy",
                Filter = FileFilters.PdfFiles,
                Extension = ".jpg",
                FileOperationActionMultiple = Convert.PdfToPict,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz pliki PDF do konwersji na obrazy:",
                OutputPrompt = "Podaj nazwę obrazu wynikowego:",
                DefaultOutputName = "page"
            },
            [3] = new OperationDefinition
            {
                Name = "Złącz obrazy do pliku pdf",
                Filter = FileFilters.PictFiles,
                Extension = ".pdf",
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Convert.PictToPdf,
                InputPrompt = "Wybierz obrazy do połączenia w PDF:",
                OutputPrompt = "Podaj nazwę pliku PDF:",
                DefaultOutputName = "merged_images"
            },
            [4] = new OperationDefinition
            {
                Name = "Odczytaj tekst z pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PdfToTxt,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz pliki PDF do konwersji na tekst:",
                OutputPrompt = "Podaj nazwę pliku tekstowego:",
                DefaultOutputName = "pdf_text"
            },
            [5] = new OperationDefinition
            {
                Name = "Odczytaj tekst z obrazu",
                Filter = FileFilters.PictFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PictToTxt,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz obrazy do odczytu tekstu OCR:",
                OutputPrompt = "Podaj nazwę pliku tekstowego:",
                DefaultOutputName = "image_text"
            },
            [6] = new OperationDefinition
            {
                Name = "Wyciągnij obrazy z pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".jpg",
                FileOperationActionMultiple = Convert.ExtractPict,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz pliki PDF, z których chcesz wyciągnąć obrazy:",
                OutputPrompt = "Podaj nazwę obrazu wynikowego:",
                DefaultOutputName = "pdf_image"
            },
            [7] = new OperationDefinition
            {
                Name = "Utwórz pdf z wyznaczonymi stronami",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.FilesPages,
                FileOperationActionPages = Pages.CreateWithPages,
                AddInfo = "pages",
                Extension = ".pdf",
                InputPrompt = "Wybierz pliki PDF do utworzenia nowych plików ze wskazanymi stronami:",
                PagesPrompt = "Podaj strony do zostawienia, np. 1,3-5:",
                OutputPrompt = "Podaj nazwę pliku PDF:",
                DefaultOutputName = "selected_pages"
            },
            [8] = new OperationDefinition
            {
                Name = "Rozdziel na oddzielne pliki pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".pdf",
                FileOperationActionMultiple = Divide.OneToMany,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz pliki PDF do rozdzielenia na strony:",
                OutputPrompt = "Podaj wzorzec nazwy plików wynikowych:",
                DefaultOutputName = "page"
            },
            [9] = new OperationDefinition
            {
                Name = "Złącz wiele plików pdf do jednego",
                Filter = FileFilters.PdfFiles,
                Extension = ".pdf",
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Divide.ManyToOne,
                InputPrompt = "Wybierz pliki PDF do połączenia. Kolejność zostanie ustalona według nazw plików:",
                OutputPrompt = "Podaj nazwę połączonego pliku PDF:",
                DefaultOutputName = "merged_pdf"
            },
            [10] = new OperationDefinition
            {
                Name = "Znajdź szukaną frazę w pliku pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PdfToTxt,
                ReportOperationAction = Search.SearchPdf,
                AddInfo = "search",
                OperationFlow = OperationFlow.SearchReport,
                InputPrompt = "Wybierz pliki PDF, w których chcesz wyszukać tekst:",
                OutputPrompt = "Podaj nazwę raportu tekstowego:",
                BeforePrompt = "Ile linii przed wynikiem pokazać:",
                AfterPrompt = "Ile linii po wyniku pokazać:",
                PhrasePrompt = "Wpisz tekst, którego chcesz szukać:",
                DefaultOutputName = "pdf_search_report"
            },
            [11] = new OperationDefinition
            {
                Name = "Znajdź szukaną frazę w obrazie",
                Filter = FileFilters.PictFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PictToTxt,
                ReportOperationAction = Search.SearchPicture,
                AddInfo = "search",
                OperationFlow = OperationFlow.SearchReport,
                InputPrompt = "Wybierz obrazy, w których chcesz wyszukać tekst:",
                OutputPrompt = "Podaj nazwę raportu tekstowego:",
                BeforePrompt = "Ile linii przed wynikiem pokazać:",
                AfterPrompt = "Ile linii po wyniku pokazać:",
                PhrasePrompt = "Wpisz tekst, którego chcesz szukać:",
                DefaultOutputName = "image_search_report"
            },
            [12] = new OperationDefinition
            {
                Name = "Wyświetl informacje o pliku pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Info.ShowInfo,
                InputPrompt = "Wybierz pliki PDF do odczytu informacji:",
                OutputPrompt = "Podaj nazwę raportu z informacjami:",
                DefaultOutputName = "pdf_info_report"
            },
            [13] = new OperationDefinition
            {
                Name = "Wyświetl informacje o czcionce w pliku pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Info.ShowFontInfo,
                InputPrompt = "Wybierz pliki PDF do sprawdzenia czcionek:",
                OutputPrompt = "Podaj nazwę raportu z czcionkami:",
                DefaultOutputName = "pdf_font_report"
            },
            [14] = new OperationDefinition
            {
                Name = "Zamień tekst PDF na placeholdery",
                OperationFlow = OperationFlow.FilesReplacement,
                Filter = FileFilters.WordFiles,
                FilterPlc = FileFilters.TxtFiles,
                FileOperationActionReplace = Replacement.ReplaceTextWithPlaceholders,
                AddInfo = "replace",
                InputPrompt = "Wybierz pliki do zamiany na placeholdery:",
                DefaultOutputName = "document_placeholders"
            },
            [15] = new OperationDefinition
            {
                Name = "Wypełnij placeholdery w PDF",
                OperationFlow = OperationFlow.FilesToFiles,
                Filter = FileFilters.WordFiles,
                FilterPlc = FileFilters.TxtFiles,
                FileOperationActionMultiple = Replacement.ReplacePlaceholdersWithText,
                AddInfo = "replace",
                InputPrompt = "Wybierz pliki do wypełnienia placeholderów",
                DefaultOutputName = "filled_document"
            },
            [16] = new OperationDefinition
            {
                Name = "Złóż dokument PDF z wybranych stron",
                OperationFlow = OperationFlow.FilesPagesSingle,
                Extension = ".pdf",
                Filter = FileFilters.PdfFiles,
                FileOperationActionPages = Pages.CreateWithCustomFiles,
                AddInfo = "fragments",
                InputPrompt = "Wybierz plik PDF:",
                DefaultOutputName = "fragmented_pdf"
            },
            [17] = new OperationDefinition
            {
                Name = "Otwórz wiele plików z użyciem programu domyślnego",
                OperationFlow = OperationFlow.RunApp,
                RunOperationAction = CaseOptions.ExecuteManyRun,
                InputPrompt = "Wybierz pliki do otwarcia:"
            },
            [18] = new OperationDefinition
            {
                Name = "Otwórz wiele plików z użyciem programu wybranego",
                OperationFlow = OperationFlow.RunApp,
                RunOperationAction = CaseOptions.ExecuteManyRunApp,
                InputPrompt = "Wybierz pliki do otwarcia wybranym programem:",
                AppPrompt = "Wybierz program: w - Word, d - LibreOffice Draw, Enter - domyślny:"
            },
            [19] = new OperationDefinition
            {
                Name = "Wyjście",
            }
        }
    );
}