using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private static readonly string[] OperationKeys =
    [
        "PdfToTxt",
        "PdfToDocx",
        "DocxToPdfWord",
        "PdfToImage",
        "ImagesToPdf",
        "ImagesToTxt",
        "ExtractFromPdf",
        "LibreOffice",
        "SplitPdf",
        "MergePdf",
        "Search",
        "Info",
        "FontInfo",
        "Replacement",
        "Fragments",
        "Help"
    ];

    public ObservableCollection<OperationMenuItem> Operations { get; } = new();

    public MainViewModel()
    {
        RefreshOperations();
    }

    [ObservableProperty]
    private OperationMenuItem? selectedOperation;

    [ObservableProperty]
    private ViewModelBase? currentOperationViewModel;
    
    partial void OnSelectedOperationChanged(OperationMenuItem? value)
    {
        CurrentOperationViewModel = value?.Key switch
        {
            "LibreOffice" => new LibreOfficeViewModel(),
            "PdfToTxt" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToTxt",
                Title = Loc.GetOperationName("PdfToTxt"),
                InputTitle = "Input PDF files",
                OutputExtension = ".txt",
                DefaultOutputName = "output",
                FileDialogTitle = "Select PDF files",
                FilePatterns = ["*.pdf"],
                Action = (_, _, fileJob) => Convert.PdfToTxt(fileJob)
            }),
            "PdfToDocx" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToDocx",
                Title = Loc.GetOperationName("PdfToDocx"),
                InputTitle = "Input PDF files",
                OutputExtension = ".docx",
                DefaultOutputName = "output",
                FileDialogTitle = "Select PDF files",
                FilePatterns = ["*.pdf"],
                Action = (_, _, fileJob) => Convert.PdfToDocx(fileJob)
            }),
            "DocxToPdfWord" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "DocxToPdfWord",
                Title = Loc.GetOperationName("DocxToPdfWord"),
                InputTitle = "Input DOCX files",
                OutputExtension = ".pdf",
                DefaultOutputName = "output",
                FileDialogTitle = "Select DOCX files",
                FilePatterns = ["*.docx"],
                Action = (_, _, fileJob) => Convert.DocxToPdfWord(fileJob)
            }),
            "PdfToImage" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToImage",
                Title = Loc.GetOperationName("PdfToImage"),
                InputTitle = "Input PDF files",
                OutputExtension = ".jpg",
                DefaultOutputName = "output",
                FileDialogTitle = "Select PDF files",
                FilePatterns = ["*.pdf"],
                MoveAllTempFiles = true,
                Action = (_, _, fileJob) => Convert.PdfToPict(fileJob)
            }),
            "ImagesToPdf" => new ManyToOneViewModel(new GuiOperationDefinition
            {
                Name = "ImagesToPdf",
                Title = Loc.GetOperationName("ImagesToPdf"),
                OutputExtension = ".pdf",
                DefaultOutputName = "images",
                FileDialogTitle = "Select image files",
                FilePatterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"],
                SingleOutputAction = fileJob => Convert.PictToPdf(fileJob)
            }),
            "ImagesToTxt" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "ImagesToTxt",
                Title = Loc.GetOperationName("ImagesToTxt"),
                OutputExtension = ".txt",
                DefaultOutputName = "images",
                FileDialogTitle = "Select image files",
                MoveAllTempFiles = true,
                FilePatterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"],
                Action = (_, _, fileJob) => Convert.PictToTxt(fileJob)
            }),
            "ExtractFromPdf" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "ExtractFromPdf",
                Title = Loc.GetOperationName("ExtractFromPdf"),
                OutputExtension = ".jpg",
                DefaultOutputName = "images",
                FileDialogTitle = "Select image files",
                MoveAllTempFiles = true,
                FilePatterns = ["*.pdf"],
                Action = (_, _, fileJob) => Convert.ExtractPict(fileJob)
            }),
            "SplitPdf" => new PagesViewModel
            {
                Title = Loc.GetOperationName("SplitPdf")
            },
            "MergePdf" => new ManyToOneViewModel(new GuiOperationDefinition
            {
                Name = "MergePdf",
                Title = Loc.GetOperationName("MergePdf"),
                OutputExtension = ".pdf",
                DefaultOutputName = "merged",
                FileDialogTitle = "Select PDF files",
                FilePatterns = ["*.pdf"],
                SingleOutputAction = fileJob => Divide.ManyToOne(fileJob)
            }),
            "Search" => new SearchViewModel(),
            "Info" => new InfoViewModel(),
            "FontInfo" => new InfoViewModel(InfoMode.FontInfo),
            "Replacement" => new ReplacementViewModel(),
            "Fragments" => new FragmentsViewModel(),
            "Help" => new HelpViewModel(),
            _ => null
        };
    }
    
    public AppLanguage[] Languages { get; } =
    [
        AppLanguage.Polish,
        AppLanguage.English
    ];

    public AppLanguage SelectedLanguage
    {
        get => Loc.Language;
        set
        {
            if (Loc.Language == value)
                return;

            string? selectedKey = SelectedOperation?.Key;
            Loc.Language = value;
            RefreshOperations(selectedKey);
        }
    }

    private void RefreshOperations(string? selectedKey = null)
    {
        Operations.Clear();

        foreach (string key in OperationKeys)
        {
            Operations.Add(new OperationMenuItem
            {
                Key = key,
                DisplayName = Loc.GetOperationName(key)
            });
        }

        if (selectedKey is not null)
        {
            SelectedOperation = Operations.FirstOrDefault(operation => operation.Key == selectedKey);
        }
    }
}
