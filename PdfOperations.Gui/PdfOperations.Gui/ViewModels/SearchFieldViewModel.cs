using CommunityToolkit.Mvvm.ComponentModel;

namespace PdfOperations.Gui.ViewModels;

public partial class SearchFieldViewModel : ObservableObject
{
    [ObservableProperty]
    private string phrase = "";
}