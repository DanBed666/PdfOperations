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
                FileOperationActionLibre = Convert.FileToPdf
            },
            [2] = new OperationDefinition
            {
                Name = "Pdf to Picture",
                Filter = FileFilters.PdfFiles,
                FileOperationActionMultiple = Convert.PdfToPict,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [3] = new OperationDefinition
            {
                Name = "Pictures to Pdf",
                Filter = FileFilters.PictFiles,
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Convert.PictToPdf
            },
            [4] = new OperationDefinition
            {
                Name = "Pdf to Text",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PdfToTxt,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [5] = new OperationDefinition
            {
                Name = "Picture To Text",
                Filter = FileFilters.PictFiles,
                FileOperationActionMultiple = Convert.PictToTxt,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [6] = new OperationDefinition
            {
                Name = "Extract Pictures from Pdf",
                Filter = FileFilters.PdfFiles,
                FileOperationActionMultiple = Convert.ExtractPict,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [7] = new OperationDefinition
            {
                Name = "Create Pdf with pages",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.FilesPages,
                FileOperationActionPages = Pages.CreateWithPages,
                AddInfo = "pages",
                Extension = ".pdf"
            },
            [8] = new OperationDefinition
            {
                Name = "Pdf Separate",
                Filter = FileFilters.PdfFiles,
                Extension = ".pdf",
                FileOperationActionMultiple = Divide.OneToMany,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [9] = new OperationDefinition
            {
                Name = "Pdf Unite",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Divide.ManyToOne
            },
            [10] = new OperationDefinition
            {
                Name = "Search Text in Pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PdfToTxt,
                ReportOperationAction = Search.SearchPdf,
                AddInfo = "search",
                OperationFlow = OperationFlow.SearchReport
            },
            [11] = new OperationDefinition
            {
                Name = "Search Text in Picture",
                Filter = FileFilters.PictFiles,
                Extension = ".txt",
                FileOperationActionMultiple = Convert.PictToTxt,
                ReportOperationAction = Search.SearchPicture,
                AddInfo = "search",
                OperationFlow = OperationFlow.SearchReport
            },
            [12] = new OperationDefinition
            {
                Name = "Pdf Info",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Info.ShowInfo
            },
            [13] = new OperationDefinition
            {
                Name = "Pdf Font",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationActionSingle = Info.ShowFontInfo
            },
            [14] = new OperationDefinition
            {
                Name = "Run App",
                OperationFlow = OperationFlow.RunApp,
                RunOperationAction = CaseOptions.ExecuteManyRun
            },
            [15] = new OperationDefinition
            {
                Name = "Run App with type",
                OperationFlow = OperationFlow.RunApp,
                RunOperationAction = CaseOptions.ExecuteManyRunApp
            },
            [16] = new OperationDefinition
            {
                Name = "Quit",
            }
        }
    );
}