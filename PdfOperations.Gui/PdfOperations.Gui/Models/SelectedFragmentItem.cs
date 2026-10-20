using CommunityToolkit.Mvvm.ComponentModel;

namespace PdfOperations.Gui.Models;

public partial class SelectedFragmentItem : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayText))]
    private int number;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayText))]
    private string fileName = "";

    [ObservableProperty]
    private string pageNumbers = "";

    public string DisplayText => $"{Number}. {FileName}";
}
