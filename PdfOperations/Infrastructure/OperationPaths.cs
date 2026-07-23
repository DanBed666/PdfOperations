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
                Filter = FileFilters.LibreOfficeFiles
            },
            [2] = new OperationDefinition
            {
                Name = "Pdf to Picture",
                Filter = FileFilters.PdfFiles,
                MultipleFilesAction = Files.MultipleConv,
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
                MultipleFilesAction = Files.MultipleConv,
                FileOperationAction = Convert.PdfToTxt
            },
            [5] = new OperationDefinition
            {
                Name = "Picture To Text",
                Filter = FileFilters.PictFiles,
                MultipleFilesAction = Files.MultipleConv,
                FileOperationAction = Convert.PictToTxt
            },
            [6] = new OperationDefinition
            {
                Name = "Extract Pictures from Pdf",
                Filter = FileFilters.PdfFiles,
                MultipleFilesAction = Files.MultipleConv,
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
                MultipleFilesAction = Files.MultipleConv,
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
                MultipleFilesAction = Files.MultipleConv,
                FileOperationAction = Search.SearchPdf
            },
            [11] = new OperationDefinition
            {
                Name = "Search Text in Picture",
                Filter = FileFilters.PictFiles,
                Extension = ".txt",
                MultipleFilesAction = Files.MultipleConv,
                FileOperationAction = Search.SearchPicture
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