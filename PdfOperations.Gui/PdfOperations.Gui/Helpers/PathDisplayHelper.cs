using System.Text;
using Avalonia.Data.Converters;

namespace PdfOperations.Gui.Helpers;

public static class PathDisplayHelper
{
    public static IValueConverter TooltipConverter { get; } =
        new FuncValueConverter<string?, string?>(path =>
            string.IsNullOrWhiteSpace(path) ? null : Format(path));

    public static string Format(string path)
    {
        bool hasDriveRoot = path.Length >= 3
                            && char.IsAsciiLetter(path[0])
                            && path[1] == ':'
                            && IsSeparator(path[2]);
        var text = new StringBuilder(path.Length);

        for (int i = 0; i < path.Length; i++)
        {
            text.Append(path[i]);

            // Display-only break hints: keep the drive root together, wrap between folders.
            if (hasDriveRoot && (i == 1 || i == 2))
                text.Append('\u2060');
            else if (IsSeparator(path[i]) && i > 1 && i < path.Length - 1
                     && !IsSeparator(path[i + 1]))
                text.Append('\u200B');
        }

        return text.ToString();
    }

    private static bool IsSeparator(char value) => value is '\\' or '/';
}
