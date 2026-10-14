using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public class HelpViewModel : ViewModelBase
{
    public string Title => Loc.GetOperationName("Help");

    public string HelpText => Loc.Language == AppLanguage.Polish
        ? PolishHelpText
        : EnglishHelpText;

    private const string PolishHelpText =
        """
        Jak używać programu

        1. Wybierz operację z listy po lewej stronie.
        2. Dodaj pliki, które chcesz przetworzyć.
        3. Wybierz folder zapisu.
        4. Uzupełnij nazwę pliku lub dodatkowe opcje, jeśli są wymagane.
        5. Kliknij Start.

        Pliki wejściowe

        - Wybierz pliki - wybiera pliki od nowa i zastępuje obecną listę.
        - Dodaj pliki - dopisuje kolejne pliki do listy. Przycisk działa po wybraniu pierwszych plików.
        - Usuń - usuwa zaznaczone pozycje z listy, ale nie usuwa plików z dysku.
        - Wyczyść - czyści całą listę, ale nie usuwa plików z dysku.
        - Przy łączeniu PDF kolejność plików na liście ma znaczenie.

        Strony PDF

        - Zapis 1,3-5 oznacza strony 1, 3, 4 i 5.
        - Strony parzyste i nieparzyste są wybierane automatycznie.
        - Podział po podanych stronach - wpisana strona kończy poprzedni plik.
        - Podział każdej strony osobno - każda strona trafia do osobnego pliku PDF.

        Zamiana tekstu

        - Zamiana tekstu korzysta z arkusza Excel z tekstem do znalezienia i wartością do podstawienia.
        - Obsługiwane są dokumenty edytowalne, na przykład DOCX, ODT i ODG.
        - Jeśli dokument jest otwarty w innym programie, zapis może się nie udać.

        Wymagania

        - Konwersja DOCX do PDF przez Word wymaga zainstalowanego Microsoft Word.
        - Konwersje LibreOffice wymagają dostępnego LibreOffice.
        - Folder tools musi znajdować się obok aplikacji w wersji release.

        Problemy

        - Jeśli operacja się nie uda, sprawdź komunikat na górze okna.
        - Jeśli nie widzisz wyniku, sprawdź folder zapisu i typ pliku wejściowego.
        - Jeśli plik nie chce się zapisać, sprawdź, czy nie jest otwarty w innym programie.
        """;

    private const string EnglishHelpText =
        """
        How to use the app

        1. Select an operation from the list on the left.
        2. Add the files you want to process.
        3. Select an output folder.
        4. Fill in the output file name or extra options if they are required.
        5. Click Start.

        Input files

        - Select files - selects files again and replaces the current list.
        - Add files - appends more files to the list. The button becomes available after selecting the first files.
        - Remove - removes selected items from the list, but does not delete files from disk.
        - Clear - clears the whole list, but does not delete files from disk.
        - For PDF merge, the order of files in the list matters.

        PDF pages

        - 1,3-5 means pages 1, 3, 4 and 5.
        - Even and odd pages are selected automatically.
        - Split after pages - the entered page becomes the last page of the previous output file.
        - Split every page - each page is saved as a separate PDF file.

        Replacement

        - Text replacement uses an Excel file with text to find and replacement values.
        - Editable documents are supported, for example DOCX, ODT and ODG.
        - If the document is open in another program, saving may fail.

        Requirements

        - DOCX to PDF through Word requires Microsoft Word.
        - LibreOffice conversions require LibreOffice.
        - The tools folder must be available next to the release application.

        Problems

        - If an operation fails, check the message at the top of the window.
        - If you cannot find the result, check the output folder and input file type.
        - If a file cannot be saved, check whether it is open in another program.
        """;
}
