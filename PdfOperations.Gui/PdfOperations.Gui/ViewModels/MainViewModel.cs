using CommunityToolkit.Mvvm.ComponentModel;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string[] Operations { get; } =
    [
        "PDF to TXT",
        "PDF to DOCX",
        "DOCX to PDF word",
        "PDF to Image",
        "Images to PDF",
        "Images to TXT",
        "Extract from PDF",
        "LibreOffice conversion",
        "Split PDF",
        "Merge PDF",
        "Search",
        "Info",
        "Font info",
        "Replacement"
    ];

    [ObservableProperty]
    private string? selectedOperation;

    [ObservableProperty]
    private string statusMessage = "Ready";
    
    [ObservableProperty]
    private ViewModelBase? currentOperationViewModel;
    
    partial void OnSelectedOperationChanged(string? value)
    {
        CurrentOperationViewModel = value switch
        {
            "LibreOffice conversion" => new LibreOfficeViewModel(),
            "PDF to TXT" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToTxt",
                Title = "PDF to TXT",
                InputTitle = "Input PDF files",
                OutputExtension = ".txt",
                DefaultOutputName = "output",
                FileDialogTitle = "Select PDF files",
                FilePatterns = ["*.pdf"],
                Action = (_, _, fileJob) => Convert.PdfToTxt(fileJob)
            }),
            "PDF to DOCX" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToDocx",
                Title = "PDF to DOCX",
                InputTitle = "Input PDF files",
                OutputExtension = ".docx",
                DefaultOutputName = "output",
                FileDialogTitle = "Select PDF files",
                FilePatterns = ["*.pdf"],
                Action = (_, _, fileJob) => Convert.PdfToDocx(fileJob)
            }),
            "DOCX to PDF word" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "DocxToPdfWord",
                Title = "DOCX to PDF word",
                InputTitle = "Input DOCX files",
                OutputExtension = ".pdf",
                DefaultOutputName = "output",
                FileDialogTitle = "Select DOCX files",
                FilePatterns = ["*.docx"],
                Action = (_, _, fileJob) => Convert.DocxToPdfWord(fileJob)
            }),
            "PDF to Image" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToImage",
                Title = "PDF to Image",
                InputTitle = "Input PDF files",
                OutputExtension = ".jpg",
                DefaultOutputName = "output",
                FileDialogTitle = "Select PDF files",
                FilePatterns = ["*.pdf"],
                MoveAllTempFiles = true,
                Action = (_, _, fileJob) => Convert.PdfToPict(fileJob)
            }),
            "Images to PDF" => new ManyToOneViewModel(new GuiOperationDefinition
            {
                Name = "ImagesToPdf",
                Title = "Images to PDF",
                OutputExtension = ".pdf",
                DefaultOutputName = "images",
                FileDialogTitle = "Select image files",
                FilePatterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"],
                SingleOutputAction = fileJob => Convert.PictToPdf(fileJob)
            }),
            "Images to TXT" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "ImagesToTxt",
                Title = "Images to TXT",
                OutputExtension = ".txt",
                DefaultOutputName = "images",
                FileDialogTitle = "Select image files",
                MoveAllTempFiles = true,
                FilePatterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"],
                Action = (_, _, fileJob) => Convert.PictToTxt(fileJob)
            }),
            "Extract from PDF" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "ExtractFromPdf",
                Title = "Extract from PDF",
                OutputExtension = ".jpg",
                DefaultOutputName = "images",
                FileDialogTitle = "Select image files",
                MoveAllTempFiles = true,
                FilePatterns = ["*.pdf"],
                Action = (_, _, fileJob) => Convert.ExtractPict(fileJob)
            }),
            "Split PDF" => new PagesViewModel
            {
                Title = "PDF pages"
            },
            "Merge PDF" => new ManyToOneViewModel(new GuiOperationDefinition
            {
                Name = "MergePdf",
                Title = "Merge PDF",
                OutputExtension = ".pdf",
                DefaultOutputName = "merged",
                FileDialogTitle = "Select PDF files",
                FilePatterns = ["*.pdf"],
                SingleOutputAction = fileJob => Divide.ManyToOne(fileJob)
            }),
            "Search" => new SearchViewModel(),
            "Info" => new InfoViewModel(),
            "Font info" => new InfoViewModel(InfoMode.FontInfo),
            "Replacement" => new ReplacementViewModel(),
            _ => null
        };
        
        StatusMessage = string.IsNullOrWhiteSpace(value)
            ? "Ready"
            : $"Selected operation: {value}";
    }
}