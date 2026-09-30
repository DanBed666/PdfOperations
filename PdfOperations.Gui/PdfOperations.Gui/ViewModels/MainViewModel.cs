using CommunityToolkit.Mvvm.ComponentModel;

namespace PdfOperations.Gui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] public partial string Greeting { get; set; } = "Welcome to Avalonia!";
    
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

    private string? _selectedOperation;

    public string? SelectedOperation
    {
        get => _selectedOperation;
        set
        {
            if (_selectedOperation == value)
                return;

            _selectedOperation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    public string StatusMessage =>
        string.IsNullOrWhiteSpace(SelectedOperation)
            ? "Ready"
            : $"Selected operation: {SelectedOperation}";
}