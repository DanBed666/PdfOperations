using System.Text;
namespace PdfOperations;

public static class OfficeFontReport
{
    private static string GetStyleName(int weight, bool italic, bool polish)
    {
        var (pl, en) = weight switch
        {
            100 => ("Bardzo cienka", "Thin"),
            200 => ("Cienka", "Extra Light"),
            300 => ("Lekka", "Light"),
            350 => ("Półlekka", "Semi Light"),
            400 => ("Zwykła", "Regular"),
            500 => ("Średnia", "Medium"),
            600 => ("Półpogrubiona", "Semi Bold"),
            700 => ("Pogrubiona", "Bold"),
            800 => ("Bardzo pogrubiona", "Extra Bold"),
            900 => ("Gruba", "Black"),
            950 => ("Bardzo gruba", "Extra Black"),
            _ => ($"Niestandardowa grubość ({weight})", $"Custom weight ({weight})")
        };

        if (italic)
        {
            pl = weight == 400 ? "Kursywa" : $"{pl} kursywa";
            en = weight == 400 ? "Italic" : $"{en} Italic";
        }

        return polish ? $"{pl} ({en})" : en;
    }

    public static string Create(string path, IInstalledFontCatalog catalog,
        bool polish, CancellationToken cancellationToken = default)
    {
        var document = OfficeFontReader.Read(path, cancellationToken);
        return Format(document, catalog, polish, cancellationToken);
    }

    public static string Format(OfficeFontDocument document, IInstalledFontCatalog catalog,
        bool polish, CancellationToken cancellationToken = default, bool compact = false)
    {
        var result = new StringBuilder();
        result.AppendLine(polish ? "CZCIONKI DOKUMENTU" : "DOCUMENT FONTS");
        result.AppendLine(polish
            ? "Dostępność w Windows nie gwarantuje identycznego wyglądu konwersji LibreOffice."
            : "Windows availability does not guarantee identical LibreOffice rendering.");
        result.AppendLine(polish
            ? "Odmiany zmienne i syntetyczne wymagają weryfikacji. Zadeklarowane osadzenie nie potwierdza poprawności pliku czcionki."
            : "Variable fonts and synthetic styles require verification. Declared embedding does not verify the font file.");

        foreach (var group in document.Fonts
            .GroupBy(f => new { f.Family, f.Weight, f.Italic, f.Embedded })
            .OrderBy(g => g.Key.Family, StringComparer.OrdinalIgnoreCase)
            .ThenBy(g => g.Key.Weight).ThenBy(g => g.Key.Italic))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var font = group.Key;
            var status = string.IsNullOrWhiteSpace(font.Family)
                ? FontAvailability.Unknown : catalog.Check(font.Family, font.Weight, font.Italic);
            string description = status switch
            {
                FontAvailability.Available => polish ? "Dostępna" : "Available",
                FontAvailability.MissingFamily => polish ? "Brak rodziny" : "Missing family",
                FontAvailability.MissingFace => polish ? "Nie znaleziono dokładnej odmiany" : "Exact face not found",
                _ => polish ? "Nie ustalono" : "Unknown"
            };
            string family = string.IsNullOrWhiteSpace(font.Family) ? "?" : font.Family;
            string style = GetStyleName(font.Weight, font.Italic, polish);
            string embedded = font.Embedded ? (polish ? "tak" : "yes") : (polish ? "nie wykryto" : "not detected");

            result.AppendLine();
            result.AppendLine($"{family} | {style} | {description}");
            if (compact)
                continue;

            result.AppendLine($"{(polish ? "Zadeklarowane osadzenie" : "Declared embedding")}: {embedded}");

            var sources = group.Select(f => f.Source).Distinct().ToArray();
            result.AppendLine($"{(polish ? "Miejsca" : "Locations")}: {string.Join("; ", sources.Take(100))}");
            if (sources.Length > 100)
                result.AppendLine(polish ? "Pokazano pierwsze 100 miejsc." : "Only the first 100 locations are shown.");
        }

        foreach (string warning in document.Warnings)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string text = warning switch
            {
                "scripts" => polish ? "Nieobsługiwany zakres pisma: czcionka nieustalona." : "Unsupported writing system: font unresolved.",
                "objects" => polish ? "Wykresy, obiekty osadzone i część tekstu graficznego nie są w pełni analizowane." : "Charts, embedded objects and some graphical text are not fully inspected.",
                "tables" => polish ? "Warunkowe style tabel Worda wymagają osobnej weryfikacji." : "Conditional Word table styles require separate verification.",
                "styles" => polish ? "Brakujący lub zapętlony styl: dziedziczenie może być niepełne." : "Missing or cyclic style: inheritance may be incomplete.",
                "themeOverrides" => polish ? "Nadpisany motyw: część czcionek pozostaje nieustalona." : "Theme override: some fonts remain unresolved.",
                "empty" => polish ? "Nie znaleziono obsługiwanego tekstu. Dokument nadal może wymagać czcionek." : "No supported text found. The document may still require fonts.",
                _ => polish ? "Nie wszystkie czcionki udało się ustalić." : "Some fonts could not be resolved."
            };
            result.AppendLine($"{(polish ? "UWAGA" : "NOTE")}: {text}");
        }
        return result.ToString();
    }
}
