using System.Collections.ObjectModel;

namespace PdfOperations;

public class OperationPaths
{
    public static IReadOnlyDictionary<int, OperationDefinition> OperationDefinitions =>
        new ReadOnlyDictionary<int, OperationDefinition>(new Dictionary<int, OperationDefinition>
        {
            [1] = new OperationDefinition
            {
                Name = "File to File (LibreOffice)",
                Filter = FileFilters.LibreOfficeFiles,
                AddInfo = "format",
                OperationFlow = OperationFlow.FilesToFilesWithFormat,
                FileOperationActionLibre = Convert.FileToPdf,
                InputPrompt = "Wybierz pliki do konwersji przez LibreOffice:",
                OutputPrompt = "Podaj nazwę pliku wynikowego:"
            },
            [2] = new OperationDefinition
            {
                Name = "Pdf to Picture",
                Filter = FileFilters.PdfFiles,
                Extension = ".jpg",
                FileOperationActionMultiple = Convert.PdfToPict,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz pliki PDF do konwersji na obrazy:",
                OutputPrompt = "Podaj nazwę obrazu wynikowego:"
            },
            [3] = new OperationDefinition
            {
                Name = "Pictures to Pdf",
                Filter = FileFilters.PictFiles,
                Extension = ".pdf",
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Convert.PictToPdf,
                InputPrompt = "Wybierz obrazy do połączenia w PDF:",
                OutputPrompt = "Podaj nazwę pliku PDF:"
            },
            [4] = new OperationDefinition
            {
                Name = "Pdf to Text",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PdfToTxt,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz pliki PDF do konwersji na tekst:",
                OutputPrompt = "Podaj nazwę pliku tekstowego:"
            },
            [5] = new OperationDefinition
            {
                Name = "Picture To Text",
                Filter = FileFilters.PictFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PictToTxt,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz obrazy do odczytu tekstu OCR:",
                OutputPrompt = "Podaj nazwę pliku tekstowego:"
            },
            [6] = new OperationDefinition
            {
                Name = "Extract Pictures from Pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".jpg",
                FileOperationActionMultiple = Convert.ExtractPict,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz pliki PDF, z których chcesz wyciągnąć obrazy:",
                OutputPrompt = "Podaj nazwę obrazu wynikowego:"
            },
            [7] = new OperationDefinition
            {
                Name = "Create Pdf with pages",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.FilesPages,
                FileOperationActionPages = Pages.CreateWithPages,
                AddInfo = "pages",
                Extension = ".pdf",
                InputPrompt = "Wybierz pliki PDF do utworzenia nowych plików ze wskazanymi stronami:",
                PagesPrompt = "Podaj strony do zostawienia, np. 1,3-5:",
                OutputPrompt = "Podaj nazwę pliku PDF:"
            },
            [8] = new OperationDefinition
            {
                Name = "Pdf Separate",
                Filter = FileFilters.PdfFiles,
                Extension = ".pdf",
                FileOperationActionMultiple = Divide.OneToMany,
                OperationFlow = OperationFlow.FilesToFiles,
                InputPrompt = "Wybierz pliki PDF do rozdzielenia na strony:",
                OutputPrompt = "Podaj wzorzec nazwy plików wynikowych:"
            },
            [9] = new OperationDefinition
            {
                Name = "Pdf Unite",
                Filter = FileFilters.PdfFiles,
                Extension = ".pdf",
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Divide.ManyToOne,
                InputPrompt = "Wybierz pliki PDF do połączenia:",
                OutputPrompt = "Podaj nazwę połączonego pliku PDF:"
            },
            [10] = new OperationDefinition
            {
                Name = "Search Text in Pdf",
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
                PhrasePrompt = "Wpisz tekst, którego chcesz szukać:"
            },
            [11] = new OperationDefinition
            {
                Name = "Search Text in Picture",
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
                PhrasePrompt = "Wpisz tekst, którego chcesz szukać:"
            },
            [12] = new OperationDefinition
            {
                Name = "Pdf Info",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Info.ShowInfo,
                InputPrompt = "Wybierz pliki PDF do odczytu informacji:",
                OutputPrompt = "Podaj nazwę raportu z informacjami:"
            },
            [13] = new OperationDefinition
            {
                Name = "Pdf Font",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Info.ShowFontInfo,
                InputPrompt = "Wybierz pliki PDF do sprawdzenia czcionek:",
                OutputPrompt = "Podaj nazwę raportu z czcionkami:"
            },
            [14] = new OperationDefinition
            {
                Name = "Run App",
                OperationFlow = OperationFlow.RunApp,
                RunOperationAction = CaseOptions.ExecuteManyRun,
                InputPrompt = "Wybierz pliki do otwarcia:"
            },
            [15] = new OperationDefinition
            {
                Name = "Run App with type",
                OperationFlow = OperationFlow.RunApp,
                RunOperationAction = CaseOptions.ExecuteManyRunApp,
                InputPrompt = "Wybierz pliki do otwarcia wybranym programem:"
            },
            [16] = new OperationDefinition
            {
                Name = "Quit",
            }
        }
    );
}