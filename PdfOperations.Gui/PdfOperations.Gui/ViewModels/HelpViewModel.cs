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

        Wyszukiwanie i eksport wyników

        - W sekcji „Szukane pola” wpisz własną frazę. Przyciskiem „Dodaj pole” możesz dodać kolejne, a przyciskiem „Usuń” usunąć niepotrzebne pole.
        - Puste pola są pomijane. Wpisz co najmniej jedną frazę; nazwy pól nie mogą się powtarzać, także przy różnej wielkości liter.
        - Raport TXT zawiera wystąpienia wszystkich fraz. Wyszukiwanie nie rozróżnia wielkich i małych liter i znajduje również frazy wewnątrz dłuższego tekstu.
        - Opcje liczby linii przed i po wyniku określają, ile otaczającego tekstu trafi do raportu.
        - Eksport do Excela i JSON jest opcjonalny. Możesz wybrać jeden lub oba formaty; raport TXT zostanie zapisany również wtedy.
        - Dla każdej frazy możesz zapisać całą znalezioną linię lub tylko tekst po frazie. Jeżeli po frazie i opcjonalnym dwukropku nie ma tekstu, program sprawdzi następną niepustą linię tej samej strony, niezależnie od ustawień raportu TXT.
        - Jeśli następna linia jest inną szukaną etykietą, pole pozostanie puste. Etykieta, której nie dodano do szukanych pól, może jednak zostać odczytana jako wartość pustego pola. Sprawdź wyeksportowane dane.
        - Gdy opcja „Eksportuj każde trafienie jako osobny wpis” jest niezaznaczona, każdy plik ma jeden wiersz w Excelu lub jeden obiekt w JSON. Wszystkie różne, niepuste wartości danej frazy trafiają do osobnych linii jednej komórki Excela lub do tablicy w JSON. To nie tylko ostatni wynik; identyczne wartości są zapisywane raz.
        - Wybierz „Eksportuj każde trafienie jako osobny wpis”, aby otrzymać osobny wiersz lub obiekt dla każdej pasującej linii danej frazy, ze stroną i numerem linii. Powtórzenia zostaną zachowane, podobnie jak zera na początku wartości.
        - Przykład: dwa różne telefony znalezione w różnych liniach jednego PDF to jedna komórka z dwiema wartościami w podsumowaniu albo dwa wiersze po włączeniu osobnych wpisów. Kilka wystąpień tej samej frazy w jednej linii nadal daje jeden wpis dla tej linii.
        - Tryb eksportu nie zmienia raportu TXT. Oznaczenia [0], [-1] i [+1] wskazują odpowiednio linię z wynikiem, poprzednią i następną.
        - Język aplikacji w chwili uruchomienia wyszukiwania określa język stałych nagłówków i nazwy arkusza Excel oraz kluczy JSON: NazwaPliku, SciezkaPliku i Pola albo FileName, FilePath i Fields.
        - Wpisane frazy oraz znalezione wartości nie są tłumaczone. Zmiana języka aplikacji nie zmienia już zapisanych plików.
        - Kilka nazwisk i numerów w jednym dokumencie nie zostanie automatycznie połączonych w pary.

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

        Search and result export

        - Enter your own phrase under Search fields. Use Add field to add more phrases and Remove to remove an unwanted field.
        - Empty fields are ignored. Enter at least one phrase; field names must be unique, ignoring letter case.
        - The TXT report contains matches for all phrases. Search is case-insensitive and also finds phrases within longer text.
        - The lines-before and lines-after options control how much surrounding text is included in the report.
        - Excel and JSON exports are optional. Select either or both formats; the TXT report is still saved.
        - For each phrase, save the entire matching line or just the text after the phrase. If nothing follows the phrase and optional colon, the next nonempty line on the same page is checked, regardless of the TXT context settings.
        - If the next line is another search label, the value remains empty. A label that is not among the search fields may instead be read as the value of an empty field. Check the exported data.
        - With “Export each match as a separate record” unchecked, each file has one Excel row or JSON object. All distinct, nonempty values for each phrase appear on separate lines in one Excel cell or in a JSON array. This keeps more than just the last result; identical values are stored once.
        - Select “Export each match as a separate record” for one row or object per matching line per phrase, with its page and line number. Repeated values and leading zeros are preserved.
        - Example: two different phone numbers found on different lines of one PDF produce one cell with two values in summary mode, or two rows with separate records enabled. Multiple occurrences of the same phrase on one line still produce one record for that line.
        - The export mode does not change the TXT report. [0], [-1] and [+1] mark the matching, preceding and following lines respectively.
        - The app language when search starts determines the fixed Excel headers and worksheet name, and the JSON keys: FileName, FilePath and Fields, or NazwaPliku, SciezkaPliku and Pola.
        - Your phrases and extracted values are not translated. Changing the app language does not change previously saved files.
        - Multiple names and numbers in one document are not automatically paired.

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
