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
        - Aby wyodrębnić wartości, wpisuj etykiety występujące w dokumentach. Eksport wymaga zgodności całej etykiety, a nie tylko jej fragmentu.
        - Wartość jest odczytywana po dwukropku w tej samej linii albo z następnej niepustej linii. Ta linia musi mieścić się w zakresie raportu, więc w razie potrzeby zwiększ liczbę linii po wyniku.
        - Jeśli następna linia jest inną szukaną etykietą, pole pozostanie puste. Etykieta, której nie dodano do szukanych pól, może jednak zostać odczytana jako wartość pustego pola. Sprawdź wyeksportowane dane.
        - W Excelu każdy plik ma osobny wiersz, a każda fraza osobną kolumnę. Nazwa i pełna ścieżka pozwalają odróżnić pliki o tej samej nazwie.
        - Powtarzające się identyczne wartości w obrębie pliku i pola są zapisywane tylko raz. Różne wartości trafiają do osobnych linii w jednej komórce Excela, a w JSON do tablicy.
        - Brak wartości oznacza pustą komórkę w Excelu lub pustą tablicę w JSON. Wartości są zapisywane jako tekst, aby zachować na przykład zera na początku.
        - Język aplikacji w chwili uruchomienia wyszukiwania określa język stałych nagłówków i nazwy arkusza Excel oraz kluczy JSON: NazwaPliku, SciezkaPliku i Pola albo FileName, FilePath i Fields.
        - Wpisane frazy oraz znalezione wartości nie są tłumaczone. Zmiana języka aplikacji nie zmienia już zapisanych plików.
        - Wyniki są grupowane według pliku, nie według osoby. Kilka nazwisk i numerów w jednym dokumencie nie zostanie automatycznie połączonych w pary.

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
        - To extract values, enter labels that appear in your documents. Export requires a match for the entire label, not just part of it.
        - A value is read after the colon on the same line or from the next nonempty line. That line must be included in the report, so increase the lines-after setting when needed.
        - If the next line is another search label, the value remains empty. A label that is not among the search fields may instead be read as the value of an empty field. Check the exported data.
        - In Excel, each file has a separate row and each phrase has a separate column. The file name and full path distinguish files with the same name.
        - Identical values within a file and field are saved only once. Different values appear on separate lines in one Excel cell and as an array in JSON.
        - Missing values produce an empty Excel cell or an empty JSON array. Values are saved as text to preserve details such as leading zeros.
        - The app language when search starts determines the fixed Excel headers and worksheet name, and the JSON keys: FileName, FilePath and Fields, or NazwaPliku, SciezkaPliku and Pola.
        - Your phrases and extracted values are not translated. Changing the app language does not change previously saved files.
        - Results are grouped by file, not by person. Multiple names and numbers in one document are not automatically paired.

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
