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
                Phrase = "format",
                OperationFlow = OperationFlow.FilesToFilesWithFormat,
                FileOperationAction = Convert.FileToPdf
            },
            [2] = new OperationDefinition
            {
                Name = "Pdf to Picture",
                Filter = FileFilters.PdfFiles,
                FileOperationAction = Convert.PdfToPict,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [3] = new OperationDefinition
            {
                Name = "Pictures to Pdf",
                Filter = FileFilters.PictFiles,
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationAction = Convert.PictToPdf
            },
            [4] = new OperationDefinition
            {
                Name = "Pdf to Text",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationAction = Convert.PdfToTxt,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [5] = new OperationDefinition
            {
                Name = "Picture To Text",
                Filter = FileFilters.PictFiles,
                FileOperationAction = Convert.PictToTxt,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [6] = new OperationDefinition
            {
                Name = "Extract Pictures from Pdf",
                Filter = FileFilters.PdfFiles,
                FileOperationAction = Info.ExtractPict,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [7] = new OperationDefinition
            {
                Name = "Create Pdf with pages",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.FilesToFiles,
                FileOperationAction = Pages.CreateWithPages,
                Phrase = "pages",
                Extension = ".pdf"
            },
            [8] = new OperationDefinition
            {
                Name = "Pdf Separate",
                Filter = FileFilters.PdfFiles,
                Extension = "%d",
                FileOperationAction = Divide.OneToMany,
                OperationFlow = OperationFlow.FilesToFiles
            },
            [9] = new OperationDefinition
            {
                Name = "Pdf Unite",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.FilesToSingleFile,
                FileOperationAction = Divide.ManyToOne
            },
            [10] = new OperationDefinition
            {
                Name = "Search Text in Pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationAction = Search.SearchPdf,
                Phrase = "search",
                OperationFlow = OperationFlow.FilesToFilesWithSearch
            },
            [11] = new OperationDefinition
            {
                Name = "Search Text in Picture",
                Filter = FileFilters.PictFiles,
                Extension = ".txt",
                FileOperationAction = Search.SearchPicture,
                Phrase = "search",
                OperationFlow = OperationFlow.FilesToFilesWithSearch
            },
            [12] = new OperationDefinition
            {
                Name = "Pdf Info",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.PdfReport,
                FileOperationAction = Info.ShowInfo
            },
            [13] = new OperationDefinition
            {
                Name = "Pdf Font",
                Filter = FileFilters.PdfFiles,
                OperationFlow = OperationFlow.PdfReport,
                FileOperationAction = Info.ShowFontInfo
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