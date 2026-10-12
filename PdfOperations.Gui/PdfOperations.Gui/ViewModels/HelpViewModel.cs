namespace PdfOperations.Gui.ViewModels;

public class HelpViewModel : ViewModelBase
{
    public string Title { get; } = "Help";

    public string HelpText { get; } =
        """
        PdfOperations - Help

        Basic workflow:
        1. Select operation from the list.
        2. Select input files.
        3. Select output folder.
        4. Set output name or options if needed.
        5. Click Start.

        Notes:
        - PDF operations use external tools bundled with the app.
        - DOCX to PDF word requires Microsoft Word installed.
        - Replacement uses an Excel file with columns: Lp, Find, Replace, IgnoreCase.
        - Search creates a text report.
        - Split PDF can extract selected pages, even pages, odd pages, or split documents.
        """;
}