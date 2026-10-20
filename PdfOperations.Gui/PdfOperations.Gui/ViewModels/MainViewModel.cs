using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using PdfOperations.Gui.Models;
using PdfOperations.Gui.Services;

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
        settings = SettingsService.Load();
        Loc.Language = settings.Language;

        RefreshThemes(settings.Theme);
        RefreshOperations();
    }

    [ObservableProperty]
    private OperationMenuItem? selectedOperation;
    
    private readonly Dictionary<string, ViewModelBase> operationViewModels = new();

    [ObservableProperty]
    private ViewModelBase? currentOperationViewModel;

    private AppSettings settings = new();

    public ObservableCollection<ThemeMenuItem> Themes { get; } = new();

    [ObservableProperty]
    private ThemeMenuItem? selectedTheme;

    private void RefreshThemes(AppTheme selectedTheme)
    {
        Themes.Clear();

        foreach (AppTheme theme in Enum.GetValues<AppTheme>())
        {
            Themes.Add(new ThemeMenuItem
            {
                Theme = theme,
                DisplayName = Loc.GetThemeName(theme)
            });
        }

        SelectedTheme = Themes.FirstOrDefault(theme => theme.Theme == selectedTheme);
    }

    partial void OnSelectedThemeChanged(ThemeMenuItem? value)
    {
        Application.Current!.RequestedThemeVariant = value?.Theme switch
        {
            AppTheme.Light => ThemeVariant.Light,
            AppTheme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };

        RefreshStatusColors();
        CurrentOperationViewModel?.RefreshStatusColors();

        if (value is not null)
        {
            settings.Theme = value.Theme;
            SettingsService.Save(settings);
        }
    }
    
    private ViewModelBase? CreateOperationViewModel(string key)
    {
        return key switch
        {
            "LibreOffice" => new LibreOfficeViewModel(),
            "PdfToTxt" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToTxt",
                Title = Loc.GetOperationName("PdfToTxt"),
                InputTitle = Loc.InputPdfFilesWithFormats,
                OutputExtension = ".txt",
                DefaultOutputName = "output",
                FileDialogTitle = Loc.SelectPdfFiles,
                FileTypeName = Loc.InputPdfFilesWithFormats,
                FilePatterns = ["*.pdf"],
                Action = (_, _, fileJob) => Convert.PdfToTxt(fileJob)
            }),
            "PdfToDocx" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToDocx",
                Title = Loc.GetOperationName("PdfToDocx"),
                InputTitle = Loc.InputPdfFilesWithFormats,
                OutputExtension = ".docx",
                DefaultOutputName = "output",
                FileDialogTitle = Loc.SelectPdfFiles,
                FileTypeName = Loc.InputPdfFilesWithFormats,
                FilePatterns = ["*.pdf"],
                Action = (_, _, fileJob) => Convert.PdfToDocx(fileJob)
            }),
            "DocxToPdfWord" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "DocxToPdfWord",
                Title = Loc.GetOperationName("DocxToPdfWord"),
                InputTitle = Loc.InputDocxFilesWithFormats,
                OutputExtension = ".pdf",
                DefaultOutputName = "output",
                FileDialogTitle = Loc.SelectDocxFiles,
                FileTypeName = Loc.InputDocxFilesWithFormats,
                FilePatterns = ["*.docx"],
                Action = (_, _, fileJob) => Convert.DocxToPdfWord(fileJob)
            }),
            "PdfToImage" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "PdfToImage",
                Title = Loc.GetOperationName("PdfToImage"),
                InputTitle = Loc.InputPdfFilesWithFormats,
                OutputExtension = ".jpg",
                DefaultOutputName = "output",
                FileDialogTitle = Loc.SelectPdfFiles,
                FileTypeName = Loc.InputPdfFilesWithFormats,
                FilePatterns = ["*.pdf"],
                MoveAllTempFiles = true,
                Action = (_, _, fileJob) => Convert.PdfToPict(fileJob)
            }),
            "ImagesToPdf" => new ManyToOneViewModel(new GuiOperationDefinition
            {
                Name = "ImagesToPdf",
                Title = Loc.GetOperationName("ImagesToPdf"),
                InputTitle = Loc.InputImageFilesWithFormats,
                OutputExtension = ".pdf",
                DefaultOutputName = "images",
                FileDialogTitle = Loc.SelectImageFiles,
                FileTypeName = Loc.InputImageFilesWithFormats,
                FilePatterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"],
                SingleOutputAction = fileJob => Convert.PictToPdf(fileJob)
            }),
            "ImagesToTxt" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "ImagesToTxt",
                Title = Loc.GetOperationName("ImagesToTxt"),
                InputTitle = Loc.InputImageFilesWithFormats,
                OutputExtension = ".txt",
                DefaultOutputName = "images",
                FileDialogTitle = Loc.SelectImageFiles,
                FileTypeName = Loc.InputImageFilesWithFormats,
                MoveAllTempFiles = true,
                FilePatterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"],
                Action = (_, _, fileJob) => Convert.PictToTxt(fileJob)
            }),
            "ExtractFromPdf" => new ManyToManyViewModel(new GuiOperationDefinition
            {
                Name = "ExtractFromPdf",
                Title = Loc.GetOperationName("ExtractFromPdf"),
                InputTitle = Loc.InputPdfFilesWithFormats,
                OutputExtension = ".jpg",
                DefaultOutputName = "images",
                FileDialogTitle = Loc.SelectPdfFiles,
                FileTypeName = Loc.InputPdfFilesWithFormats,
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
                InputTitle = Loc.InputPdfFilesWithFormats,
                OutputExtension = ".pdf",
                DefaultOutputName = "merged",
                FileDialogTitle = Loc.SelectPdfFiles,
                FileTypeName = Loc.InputPdfFilesWithFormats,
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
    
    partial void OnSelectedOperationChanged(OperationMenuItem? value)
    {
        if (value is null)
        {
            CurrentOperationViewModel = null;
            return;
        }
    
        if (!operationViewModels.TryGetValue(value.Key, out var viewModel))
        {
            viewModel = CreateOperationViewModel(value.Key);
    
            if (viewModel is null)
            {
                CurrentOperationViewModel = null;
                return;
            }
    
            operationViewModels.Add(value.Key, viewModel);
        }
    
        viewModel.RefreshStatusColors();
        CurrentOperationViewModel = viewModel;
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
            AppTheme selectedTheme = SelectedTheme?.Theme ?? AppTheme.System;
            Loc.Language = value;
            
            foreach (ViewModelBase viewModel in operationViewModels.Values)
            {
                viewModel.RefreshLocalization();
            }

            settings.Language = value;
            SettingsService.Save(settings);

            RefreshThemes(selectedTheme);
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
        else
        {
            SelectedOperation = Operations.FirstOrDefault();
        }
    }
}
