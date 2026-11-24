using System.Runtime.InteropServices;
using System.Text;
using static PdfOperations.WindowsFontNative;
namespace PdfOperations;

public sealed class WindowsFontCatalog : IInstalledFontCatalog
{
    private readonly Dictionary<string, HashSet<(int Weight, bool Italic)>> faces =
        new(StringComparer.OrdinalIgnoreCase);
    private WindowsFontCatalog() { }

    public static WindowsFontCatalog Read(CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Windows font catalog required.");
        cancellationToken.ThrowIfCancellationRequested();
        var result = new WindowsFontCatalog();
        IFactory? factory = null;
        IFontCollection? collection = null;
        try
        {
            var iid = typeof(IFactory).GUID;
            Marshal.ThrowExceptionForHR(DWriteCreateFactory(0, ref iid, out factory));
            Marshal.ThrowExceptionForHR(factory.GetSystemFontCollection(out collection, true));
            for (uint i = 0; i < collection.GetFontFamilyCount(); i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                IFontFamily? family = null;
                ILocalizedStrings? names = null;
                try
                {
                    Marshal.ThrowExceptionForHR(collection.GetFontFamily(i, out family));
                    Marshal.ThrowExceptionForHR(family.GetFamilyNames(out names));
                    var styles = new HashSet<(int Weight, bool Italic)>();
                    for (uint j = 0; j < family.GetFontCount(); j++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        IFont? font = null;
                        try
                        {
                            Marshal.ThrowExceptionForHR(family.GetFont(j, out font));
                            styles.Add(((int)font.GetWeight(), font.GetStyle() != 0));
                        }
                        finally { Release(font); }
                    }
                    for (uint j = 0; j < names.GetCount(); j++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        Marshal.ThrowExceptionForHR(names.GetStringLength(j, out uint length));
                        var name = new StringBuilder(checked((int)length + 1));
                        Marshal.ThrowExceptionForHR(names.GetString(j, name, length + 1));
                        string key = name.ToString().Trim();
                        if (!result.faces.TryGetValue(key, out var known))
                            result.faces[key] = known = new();
                        known.UnionWith(styles);
                    }
                }
                finally { Release(names); Release(family); }
            }
            return result;
        }
        finally { Release(collection); Release(factory); }
    }

    public FontAvailability Check(string family, int weight, bool italic)
    {
        if (string.IsNullOrWhiteSpace(family) || family.StartsWith('+'))
            return FontAvailability.Unknown;
        if (!faces.TryGetValue(family.Trim(), out var styles))
            return FontAvailability.MissingFamily;
        return styles.Contains((weight, italic))
            ? FontAvailability.Available : FontAvailability.MissingFace;
    }

    private static void Release(object? value)
    {
        if (value is not null && Marshal.IsComObject(value))
            Marshal.ReleaseComObject(value);
    }
}