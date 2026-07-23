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
                Phrase = "format"
            },
            [2] = new OperationDefinition
            {
                Name = "Pdf to Picture",
                Filter = FileFilters.PdfFiles,
                FileOperationAction = Convert.PdfToPict
            },
            [3] = new OperationDefinition
            {
                Name = "Pictures to Pdf",
                Filter = FileFilters.PictFiles
            },
            [4] = new OperationDefinition
            {
                Name = "Pdf to Text",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationAction = Convert.PdfToTxt
            },
            [5] = new OperationDefinition
            {
                Name = "Picture To Text",
                Filter = FileFilters.PictFiles,
                FileOperationAction = Convert.PictToTxt
            },
            [6] = new OperationDefinition
            {
                Name = "Extract Pictures from Pdf",
                Filter = FileFilters.PdfFiles,
                FileOperationAction = Info.ExtractPict
            },
            [7] = new OperationDefinition
            {
                Name = "Create Pdf with pages",
                Filter = FileFilters.PdfFiles,
            },
            [8] = new OperationDefinition
            {
                Name = "Pdf Separate",
                Filter = FileFilters.PdfFiles,
                Extension = "%d",
                FileOperationAction = Divide.OneToMany
            },
            [9] = new OperationDefinition
            {
                Name = "Pdf Unite",
                Filter = FileFilters.PdfFiles
            },
            [10] = new OperationDefinition
            {
                Name = "Search Text in Pdf",
                Filter = FileFilters.PdfFiles,
                Extension = ".txt",
                FileOperationAction = Search.SearchPdf,
                Phrase = "search"
            },
            [11] = new OperationDefinition
            {
                Name = "Search Text in Picture",
                Filter = FileFilters.PictFiles,
                Extension = ".txt",
                FileOperationAction = Search.SearchPicture,
                Phrase = "search"
            },
            [12] = new OperationDefinition
            {
                Name = "Pdf Info",
                Filter = FileFilters.PdfFiles
            },
            [13] = new OperationDefinition
            {
                Name = "Pdf Font",
                Filter = FileFilters.PdfFiles
            },
        }
    );
}