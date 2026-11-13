using Microsoft.Win32;

namespace PdfOperations;

public static class VcRuntimeTerms
{
    // The per-user key and revision are also used by the Inno Setup wizard.
    private const string Revision = "vc-runtime-v1";
    private const string Key = @"HKEY_CURRENT_USER\Software\DanBed666\PdfOperations\Terms";

    public static bool NeedsAcceptance => RequiresConsent(
        Path.Combine(ToolPaths.ToolsDir, "pdf2docx", "_internal", "MSVCP140.dll"),
        () => Equals(Registry.GetValue(Key, Revision, null), 1));

    internal static bool RequiresConsent(string library, Func<bool> accepted) =>
        File.Exists(library) && !accepted();

    public static string Read(bool polish) =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "terms", Revision, polish ? "pl.txt" : "en.txt"));

    public static void Accept() => Registry.SetValue(Key, Revision, 1, RegistryValueKind.DWord);

    internal static bool PromptConsole(TextReader input, TextWriter output, bool polish)
    {
        output.WriteLine(Read(polish));
        output.WriteLine(polish ? "Wpisz TAK, aby zaakceptować. Enter oznacza odmowę."
                                : "Type YES to accept. Press Enter to decline.");
        return string.Equals(input.ReadLine()?.Trim(), polish ? "TAK" : "YES",
            StringComparison.OrdinalIgnoreCase);
    }
}
