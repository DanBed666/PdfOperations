using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;

namespace PdfOperations.Gui.Views;

internal static class VcRuntimeTermsDialog
{
    public static async Task<bool> ShowAsync(bool polish)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime
            { MainWindow: { } owner })
            return false;

        var accept = new Button { Content = polish ? "Akceptuję" : "I accept" };
        var decline = new Button { Content = polish ? "Nie akceptuję" : "I decline" };
        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal, Spacing = 8, HorizontalAlignment = HorizontalAlignment.Right,
            Children = { decline, accept }
        };
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            Content = new TextBlock { Text = VcRuntimeTerms.Read(polish), TextWrapping = TextWrapping.Wrap }
        };
        var grid = new Grid { RowDefinitions = new RowDefinitions("*,Auto"), Margin = new Thickness(16), RowSpacing = 12 };
        Grid.SetRow(buttons, 1);
        grid.Children.Add(scroll);
        grid.Children.Add(buttons);
        var dialog = new Window
        {
            Title = polish ? "Warunki korzystania z Microsoft Visual C++" : "Microsoft Visual C++ terms",
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Width = 620, Height = 460, ShowInTaskbar = false, Content = grid
        };
        accept.Click += (_, _) => dialog.Close(true);
        decline.Click += (_, _) => dialog.Close(false);
        return await dialog.ShowDialog<bool>(owner);
    }
}
