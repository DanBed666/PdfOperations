using System.Runtime.InteropServices;
using System.Text;
namespace PdfOperations;

internal static class WindowsFontNative
{
    [DllImport("dwrite.dll", ExactSpelling = true)]
    internal static extern int DWriteCreateFactory(uint type, ref Guid iid, out IFactory factory);

    [ComImport, Guid("B859EE5A-D838-4B5B-A2E8-1ADC7D93DB48"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFactory
    {
        [PreserveSig] int GetSystemFontCollection(out IFontCollection collection, [MarshalAs(UnmanagedType.Bool)] bool checkForUpdates);
    }
    [ComImport, Guid("A84CEE02-3EEA-4EEE-A827-87C1A02A0FCC"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFontCollection
    {
        [PreserveSig] uint GetFontFamilyCount();
        [PreserveSig] int GetFontFamily(uint index, out IFontFamily family);
    }
    [ComImport, Guid("DA20D8EF-812A-4C43-9802-62EC4ABD7ADD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFontFamily
    {
        [PreserveSig] int GetFontCollection(out IFontCollection collection);
        [PreserveSig] uint GetFontCount();
        [PreserveSig] int GetFont(uint index, out IFont font);
        [PreserveSig] int GetFamilyNames(out ILocalizedStrings names);
    }
    [ComImport, Guid("ACD16696-8C14-4F5D-877E-FE3FC1D32737"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFont
    {
        [PreserveSig] int GetFontFamily(out IFontFamily family);
        [PreserveSig] uint GetWeight();
        [PreserveSig] uint GetStretch();
        [PreserveSig] uint GetStyle();
    }
    [ComImport, Guid("08256209-099A-4B34-B86D-C22B110E7771"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ILocalizedStrings
    {
        [PreserveSig] uint GetCount();
        [PreserveSig] int FindLocaleName([MarshalAs(UnmanagedType.LPWStr)] string name, out uint index, [MarshalAs(UnmanagedType.Bool)] out bool exists);
        [PreserveSig] int GetLocaleNameLength(uint index, out uint length);
        [PreserveSig] int GetLocaleName(uint index, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, uint size);
        [PreserveSig] int GetStringLength(uint index, out uint length);
        [PreserveSig] int GetString(uint index, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, uint size);
    }
}