using CommunityToolkit.Mvvm.ComponentModel;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string[] Operations { get; } =
    [
        "PDF to TXT",
        "PDF to DOCX",
        "Images to PDF",
        "Split PDF",
        "Merge PDF",
        "Search",
        "Info",
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
            
            "Replacement" => new ReplacementViewModel(),
            
            _ => null
        };
        
        StatusMessage = string.IsNullOrWhiteSpace(value)
            ? "Ready"
            : $"Selected operation: {value}";
    }
}