using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public class HelpViewModel : ViewModelBase
{
    public string Title => Loc.GetOperationName("Help");

    public string RequirementsTitle => Loc.Language == AppLanguage.Polish ? "Wersja portable na Windows x64" : "Portable version on Windows x64";
    public string RequirementsText => Loc.Language == AppLanguage.Polish
        ? """
          Obecne wydanie jest dostępne jako portable ZIP, bez osobnego instalatora. Zawiera .NET 10 Desktop Runtime i wymagane biblioteki Visual C++; nie trzeba instalować ich osobno.
          Warunki biblioteki Visual C++ zaakceptuj przed pierwszą konwersją PDF na DOCX. Odmowa nie blokuje pozostałych operacji.
          Tylko opcja DOCX do PDF (Word) wymaga zainstalowanego i aktywowanego programu Microsoft Word. Bez Worda wybierz Konwersja LibreOffice.
          Rozpakuj cały ZIP do nowego folderu z prawem zapisu i uruchom PdfOperations.exe z głównego poziomu. Zachowaj current, Update.exe i .portable. LibreOffice i narzędzia OCR są w current/tools. Własne dokumenty zapisuj poza current.
          Do zwykłego korzystania z portable nie potrzeba uprawnień administratora, ale zabezpieczenia Windows i zasady organizacji nadal obowiązują.
          Aktualizacje: O programie → Sprawdź aktualizacje → Pobierz aktualizację. Widać postęp pobierania i przygotowywania; pobieranie można anulować. Zakończ konwersje i zamknij inne okna tej samej kopii, zanim wybierzesz Uruchom ponownie i zaktualizuj. Potrzebne są internet, prawo zapisu i wolne miejsce. Pierwsza aktualizacja może pobrać całą paczkę, kolejne mogą korzystać z pakietów różnicowych.
          Starszą paczkę bez Velopack zastąp ręcznie, rozpakowując nowy ZIP do nowego folderu. Do uruchomienia wystarczy ZIP z aplikacją; pliki .nupkg i releases.win.json są dla mechanizmu aktualizacji.
          """
        : """
          The current release is a portable ZIP, without a separate installer. It includes the .NET 10 Desktop Runtime and required Visual C++ libraries; you do not need to install them separately.
          Accept the Visual C++ library terms before your first PDF-to-DOCX conversion. Declining does not block other operations.
          Only DOCX to PDF (Word) needs an installed and activated copy of Microsoft Word. Without Word, choose LibreOffice conversion.
          Extract the entire ZIP to a new writable folder and run PdfOperations.exe at its top level. Keep current, Update.exe and .portable together. LibreOffice and OCR tools are in current/tools. Save your own documents outside current.
          Normal use of portable does not require administrator rights, but Windows security checks and organizational policies still apply.
          Updates: About → Check for updates → Download update. Download and preparation progress are shown; downloading can be cancelled. Finish conversions and close other windows of the same copy before choosing Restart and update. Internet access, write permissions and free space are required. The first update may download the full package; later updates can use differential packages.
          Replace an older package without Velopack manually by extracting the new ZIP to a new folder. Only the application ZIP is needed to run the app; .nupkg files and releases.win.json are for the updater.
          """;

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

        Informacje o czcionkach

        - Wybierz Informacje o czcionkach, dodaj PDF, DOCX lub PPTX, wskaż folder zapisu i kliknij Start. Możesz mieszać te formaty na jednej liście. Osobna operacja Informacje nadal przyjmuje tylko PDF.
        - Domyślna nazwa raportu to font_info.txt. Po powodzeniu całej operacji pojawi się podgląd pod przyciskami; Otwórz folder wynikowy otwiera miejsce zapisu TXT.
        - Dla PDF narzędzie pdffonts podaje czcionki i osadzenie w PDF, bez porównania z katalogiem Windows. Dla DOCX/PPTX aplikacja analizuje formatowanie, obsługiwane style i motywy oraz sprawdza rodziny i odmiany w Windows.
        - Raport DOCX/PPTX zawiera nazwę rodziny, grubość (400 = zwykła, 700 = pogrubiona), kursywę, zadeklarowane osadzenie i miejsca wystąpienia. Dla DOCX są to części XML i akapity, nie numery stron; dla PPTX slajdy i akapity. Język etykiet zależy od języka aplikacji przy rozpoczęciu analizy.
        - Dostępna oznacza znalezioną rodzinę i odmianę. Brak rodziny oznacza brak w katalogu Windows. Nie znaleziono dokładnej odmiany oznacza brak żądanego połączenia grubości i pochylenia; czcionki zmienne i odmiany syntetyczne wymagają weryfikacji. Nie ustalono oznacza niejednoznaczny wynik, nie potwierdzony brak czcionki.
        - Zadeklarowane osadzenie nie potwierdza instalacji czcionki, poprawności jej danych ani użycia przez LibreOffice. Operacja nie zmienia dokumentów, nie pobiera i nie instaluje czcionek. Analiza DOCX/PPTX nie wymaga Worda, PowerPointa, LibreOffice, internetu ani administratora.
        - Podgląd ma limit 200 000 znaków; jego skrócenie nie skraca TXT. Każda pozycja czcionki DOCX/PPTX pokazuje do 100 różnych miejsc wystąpienia również w TXT. Nowe uruchomienie czyści podgląd. Błąd lub anulowanie przed końcowym zapisem raportu pozostawia poprzedni plik bez zmian.
        - DOCX obejmuje tekst główny, nagłówki, stopki, przypisy i komentarze. PPTX obejmuje tekst slajdów z obsługiwanym dziedziczeniem formatowania. Obsługiwany jest tekst łaciński, w tym polskie znaki. DOC/PPT i inne formaty biurowe nie są przyjmowane.
        - Analiza nie zastępuje renderowania dokumentu. Warunkowe style tabel, wykresy, SmartArt, obiekty osadzone, tekst graficzny, czcionki punktorów i numeracji, notatki prezentera i tekst tylko na wzorcu slajdu nie są w pełni obsługiwane. Ostrzeżenia nie wyliczają każdego pominiętego elementu.
        - Dostępność czcionki nie gwarantuje identycznego układu w LibreOffice. Sprawdź wynik konwersji. Brakujące rodziny i odmiany uzyskaj od wydawcy i instaluj zgodnie z licencją oraz zasadami organizacji, a następnie ponów analizę i konwersję.

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
        - Kliknij „Pobierz szablon Excel” pod wyborem arkusza i wybierz miejsce zapisu pliku replacement-template.xlsx. Szablon powstaje lokalnie, bez internetu; jego zapis nie uruchamia zamiany dokumentów.
        - Otwórz szablon w Excelu lub LibreOffice Calc, zastąp przykłady własnymi zamianami, dodaj kolejne wiersze i zapisz jako .xlsx. Następnie wskaż zapisany plik przez „Przeglądaj” przy polu „Arkusz Excel z zamianami”.
        - Używany jest pierwszy arkusz. Zachowaj pierwszy wiersz z nagłówkami i kolejność kolumn: A = Lp (numer), B = Find (tekst szukany), C = Replace (tekst do wstawienia), D = IgnoreCase (TRUE ignoruje wielkość liter, FALSE ją uwzględnia).
        - Szablon jest edytowalny i wspólny dla obu języków. Zawiera przykłady {{COMPANY}} → Example Studio i {{CITY}} → Warsaw. Możesz szukać zwykłego tekstu; nawiasy {{...}} nie są wymagane. W kolumnie IgnoreCase zachowaj zapis TRUE/FALSE, nie tłumacz go.
        - Puste Find pomija wiersz, a puste Replace usuwa znaleziony tekst. Zamiany są wykonywane w kolejności wierszy, więc późniejsza zamiana może zmienić wynik wcześniejszej.
        - Obsługiwane są dokumenty edytowalne, na przykład DOCX, ODT i ODG.
        - W głównej treści DOCX fraza może mieć różne formatowanie w obrębie jednego akapitu. Wstawiony tekst przejmuje formatowanie początku trafienia; tekst poza zamianą zachowuje swoje style. Dopasowanie nie przechodzi przez granice akapitów, tabulatory ani podziały wiersza. Nagłówki i stopki nie są przetwarzane.
        - Jeśli dokument jest otwarty w innym programie, zapis może się nie udać.
        - W ODT i ODG zamiana również rozpoznaje frazy rozdzielone formatowaniem w jednym akapicie lub nagłówku, także w polach tekstowych. Nowy tekst przejmuje formatowanie początku trafienia. Style i adresy linków pozostają bez zmian; dopasowanie nie przechodzi przez tabulatory, podziały wiersza, pola automatyczne ani osadzone obiekty.

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

        Font info

        - Choose Font info, add PDF, DOCX or PPTX files, select an output folder and click Start. You can mix these formats in one list. The separate Info operation still accepts only PDFs.
        - The default report name is font_info.txt. After the entire operation succeeds, a preview appears below the buttons; Open output folder opens the TXT file's location.
        - For PDF, pdffonts reports fonts and PDF embedding without comparing them against the Windows catalog. For DOCX/PPTX, the app reads formatting, supported styles and themes and checks font families and styles in Windows.
        - The DOCX/PPTX report includes family names, weight (400 = regular, 700 = bold), italic formatting, declared embedding and locations. DOCX locations are XML parts and paragraphs, not page numbers; PPTX locations are slides and paragraphs. Report labels follow the app language when inspection starts.
        - Available means a matching family and style was found. Missing family means the family was not found in the Windows catalog. Exact face not found means the requested weight/style combination was not found; variable fonts and synthetic styles require verification. Unknown means an unresolved result, not a confirmed missing font.
        - Declared embedding does not confirm font installation, valid font data or use by LibreOffice. The operation does not modify documents or download or install fonts. DOCX/PPTX inspection does not require Word, PowerPoint, LibreOffice, internet access or administrator rights.
        - The preview is limited to 200,000 characters; truncating it does not truncate the TXT report. Each DOCX/PPTX font entry lists up to 100 distinct locations, including in TXT. A new run clears the preview. An error or cancellation before the report is finalized leaves the previous file unchanged.
        - DOCX inspection includes the main text, headers, footers, footnotes, endnotes and comments. PPTX inspection covers slide text with supported formatting inheritance. Latin text, including Polish characters, is supported. DOC/PPT and other Office formats are not accepted.
        - Inspection does not replace document rendering. Conditional table styles, charts, SmartArt, embedded objects, graphical text, bullet/numbering fonts, presenter notes and text only on a slide master are not fully covered. Warnings do not identify every omitted element.
        - Font availability does not guarantee identical layout in LibreOffice. Check the conversion result. Obtain missing families and styles from their publisher and install them according to their licenses and your organization's policies, then repeat inspection and conversion.

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
        - Click “Download Excel template” below the workbook picker and choose where to save replacement-template.xlsx. The template is generated locally without internet access; saving it does not start document replacement.
        - Open the template in Excel or LibreOffice Calc, replace the examples with your own substitutions, add more rows and save as .xlsx. Then select the saved file using Browse next to Excel replacement file.
        - The first worksheet is used. Keep the header row and column order: A = Lp (number), B = Find (text to find), C = Replace (replacement text), D = IgnoreCase (TRUE ignores letter case, FALSE respects it).
        - The editable template is the same in both languages. It includes {{COMPANY}} → Example Studio and {{CITY}} → Warsaw. You can search for ordinary text; {{...}} braces are not required. Keep TRUE/FALSE in IgnoreCase without translating them.
        - An empty Find skips the row; an empty Replace deletes the matched text. Replacements run in row order, so a later replacement may change the result of an earlier one.
        - Editable documents are supported, for example DOCX, ODT and ODG.
        - In the DOCX document body, a phrase can span different formatting within one paragraph. Replacement text uses the formatting at the start of the match; surrounding text keeps its styles. Matches do not cross paragraph boundaries, tabs or line breaks. Headers and footers are not processed.
        - If the document is open in another program, saving may fail.
        - In ODT and ODG, replacement also matches phrases split by formatting within one paragraph or heading, including text boxes. New text uses the formatting at the start of the match. Styles and link addresses remain unchanged; matches do not cross tabs, line breaks, fields or embedded objects.

        Problems

        - If an operation fails, check the message at the top of the window.
        - If you cannot find the result, check the output folder and input file type.
        - If a file cannot be saved, check whether it is open in another program.
        """;
}
