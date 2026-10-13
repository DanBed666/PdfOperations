using CommunityToolkit.Mvvm.ComponentModel;
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public LocalizationService Loc => LocalizationService.Instance;
}