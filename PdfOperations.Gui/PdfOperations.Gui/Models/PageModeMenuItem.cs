using CommunityToolkit.Mvvm.ComponentModel;

namespace PdfOperations.Gui.Models;

public partial class PageModeMenuItem : ObservableObject
{
    public PageSelectionMode Mode { get; init; }

    [ObservableProperty]
    private string displayName = "";
}
