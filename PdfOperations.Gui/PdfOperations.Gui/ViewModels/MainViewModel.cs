using CommunityToolkit.Mvvm.ComponentModel;

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
            "PDF to TXT" => new ManyToManyViewModel
            {
                Title = "PDF to TXT"
            },
            _ => null
        };
        
        StatusMessage = string.IsNullOrWhiteSpace(value)
            ? "Ready"
            : $"Selected operation: {value}";
    }
}