# PdfOperations

[English](README.md) | **Polski**

PdfOperations to aplikacja dla systemu Windows do pracy z plikami PDF, obrazami, tekstem i dokumentami biurowymi. Ma interfejs graficzny oparty na Avalonia, dostępny po polsku i angielsku, oraz motyw jasny, ciemny i zgodny z ustawieniami systemu.

Repozytorium zawiera również pierwotną aplikację konsolową. Ten przewodnik opisuje obsługę wersji graficznej.

## Funkcje

| Operacja | Opis |
| --- | --- |
| PDF do TXT | Odczytywanie zaznaczalnego tekstu z plików PDF. |
| PDF do DOCX | Konwersja plików PDF na dokumenty Word za pomocą narzędzia pdf2docx. |
| DOCX do PDF (Word) | Konwersja dokumentów Word przy użyciu zainstalowanego programu Microsoft Word. |
| PDF na obrazy | Zapisywanie stron PDF jako obrazów. |
| Obrazy na PDF | Łączenie obrazów w jeden PDF w wybranej kolejności. |
| Obrazy na TXT | Rozpoznawanie tekstu na obrazach za pomocą Tesseract OCR. |
| Wyodrębnianie obrazów | Wyodrębnianie obrazów osadzonych w plikach PDF. |
| Konwersja LibreOffice | Konwersja dokumentów biurowych do PDF, DOCX, ODT, XLSX, HTML lub TXT, w zakresie obsługiwanym przez LibreOffice. |
| Strony PDF | Wyodrębnianie wybranych, parzystych lub nieparzystych stron; podział po wskazanych stronach albo na pojedyncze strony. |
| Łączenie PDF | Łączenie plików PDF w kolejności widocznej na liście. |
| PDF z fragmentów | Łączenie wybranych zakresów stron z wielu plików PDF i ustalanie ich kolejności. |
| Wyszukiwanie | Wyszukiwanie wielu fraz w plikach PDF, TXT i obrazach; zapis raportu TXT oraz opcjonalny eksport odczytanych wartości do Excela i JSON. |
| Informacje o PDF | Odczytywanie metadanych, liczby stron i informacji o czcionkach. |
| Zamiana tekstu | Zamiana tekstu w dokumentach DOCX, ODT i ODG na podstawie tabeli zamian w Excelu. |

Interfejs zawiera liczniki plików, filtry formatów, wskaźniki postępu, anulowanie operacji, opcję nadpisywania oraz rozwijane szczegóły błędów. Wybrany język i motyw są zapamiętywane między uruchomieniami programu.

## Wymagania i uruchomienie

- System Windows. Skonfigurowane narzędzia zewnętrzne korzystają z plików wykonywalnych dla Windows, w tym wersji x64.
- Wersja zależna od środowiska .NET wymaga odpowiedniej wersji .NET 10 Desktop Runtime. Paczka typu self-contained zawiera własne środowisko .NET.
- Microsoft Word jest wymagany tylko do operacji **DOCX do PDF (Word)**.
- Narzędzia zewnętrzne wraz z plikami pomocniczymi muszą znajdować się w przewidzianym folderze `tools` obok aplikacji.
- Program potrzebuje uprawnień do zapisu w folderze wynikowym i folderach tymczasowych oraz wystarczającej ilości wolnego miejsca na dysku.

Aby uruchomić gotową paczkę, rozpakuj całe archiwum do folderu i otwórz `PdfOperations.Gui.exe`. Nie uruchamiaj programu bezpośrednio z archiwum ani nie przenoś samego pliku EXE bez plików pomocniczych.

Na początek wybierz operację z lewego paska. Wbudowana **Pomoc** jest dostępna w obu językach interfejsu.

## Podstawowa obsługa

1. Wybierz operację z bocznego paska.
2. Kliknij **Wybierz pliki**, aby wskazać pliki wejściowe. Ten przycisk zastępuje obecną listę.
3. Użyj **Dodaj pliki**, aby dopisać kolejne pliki, również z innych folderów.
4. Wskaż folder zapisu i uzupełnij wymagane opcje, na przykład nazwę pliku, zakres stron lub szukane pola.
5. Kliknij **Start**, a następnie **Otwórz folder wynikowy**, aby sprawdzić rezultaty.

**Usuń** usuwa zaznaczone pozycje z listy, a **Wyczyść** opróżnia całą listę. Żaden z tych przycisków nie usuwa plików źródłowych z dysku. Możesz zaznaczyć i usunąć z listy kilka pozycji jednocześnie.

W operacjach, w których kolejność ma znaczenie, ustaw ją przyciskami **W górę** i **W dół** przed uruchomieniem. Łączenie PDF zachowuje kolejność widoczną na liście, bez ponownego sortowania alfabetycznego podczas konwersji.

Tam, gdzie jest to obsługiwane, puste pole folderu zapisu jest uzupełniane na podstawie pierwszego pliku wejściowego. Wcześniej wybrany folder nie jest automatycznie zmieniany. Sprawdź go przed rozpoczęciem, szczególnie jeśli pliki pochodzą z różnych lokalizacji.

Gdy opcja **Nadpisz istniejące pliki** jest wyłączona, przy konflikcie nazw program dodaje do nazwy wynikowej kolejne numery. Włączenie tej opcji pozwala zastępować istniejące pliki docelowe. Różne pliki wejściowe o tej samej nazwie bazowej nadal otrzymują odrębne nazwy wynikowe w ramach jednej partii. Przy nadpisywaniu zachowaj kopie zapasowe.

Przycisk **Anuluj** zgłasza żądanie zatrzymania operacji. Anulowanie nie cofa wykonanych działań: wcześniej zapisane pliki mogą pozostać w folderze wynikowym. Program próbuje usunąć pliki tymczasowe po zakończeniu, błędzie i anulowaniu; blokada pliku przez inny proces może jednak uniemożliwić sprzątanie.

## Strony i fragmenty PDF

Numeracja stron zaczyna się od 1. Możesz wskazywać pojedyncze strony i zakresy, na przykład `1`, `2-5` lub `1,3-5,8`.

- Wybór wskazanych stron tworzy PDF zawierający żądane strony.
- Tryby stron parzystych i nieparzystych wybierają strony automatycznie.
- Podział po podanych stronach traktuje każdą wpisaną liczbę jako numer ostatniej strony poprzedniej części. Na przykład podział siedmiostronicowego PDF po stronach `2,5` tworzy części zawierające strony 1-2, 3-5 i 6-7.
- Podział każdej strony osobno zapisuje każdą stronę do oddzielnego PDF.

Operacja **Utwórz PDF z fragmentów** pozwala złożyć dokument z zakresów stron pochodzących z kilku plików PDF. Dodaj zakresy, sprawdź liczbę stron i ustaw kolejność fragmentów przed uruchomieniem.

## Wyszukiwanie i odczytywanie wartości

Wyszukiwanie obsługuje pliki PDF, TXT i obrazy. Korzystaj z filtra typu pliku w oknie wyboru, aby przypadkowo nie zaznaczyć razem dokumentów PDF i wcześniej wygenerowanych raportów TXT.

Wpisz frazę w sekcji **Szukane pola**, a kolejne dodawaj przyciskiem **Dodaj pole**. Puste pola są pomijane. Wymagana jest co najmniej jedna fraza, a nazwy pól nie mogą się powtarzać, także przy różnej wielkości liter.

Raport TXT zawiera wystąpienia wszystkich fraz, również wewnątrz dłuższego tekstu. Wyszukiwanie nie rozróżnia wielkich i małych liter. Ustawienia liczby linii przed i po wyniku określają, ile otaczającego tekstu zostanie uwzględnione w raporcie.

### Eksport do Excela i JSON

Zaznacz **Zapisz również do Excela**, **Zapisz również do JSON** lub obie opcje. Raport TXT także zostanie zapisany.

Aby odczytywać wartości, podawaj etykiety występujące w dokumentach. Są to dowolne frazy wpisane przez użytkownika, a nie wbudowana lista numerów telefonów, identyfikatorów podatkowych czy adresów. Przykładowy dokument:

```text
Numer sprawy: 00123
Firma:
Przykładowe Studio
```

Wyszukiwanie pól `Numer sprawy` i `Firma` pozwala odczytać wartości `00123` i `Przykładowe Studio`. Odczyt wymaga zgodności całej etykiety, z pominięciem wielkości liter i białych znaków na jej początku i końcu. Znalezienie fragmentu dłuższej etykiety w raporcie TXT nie musi więc oznaczać odczytania wartości do eksportu.

- Każdy plik wejściowy otrzymuje jeden wiersz w Excelu lub jeden obiekt w JSON. Pełne ścieżki rozróżniają pliki o tej samej nazwie z różnych folderów.
- Każde szukane pole otrzymuje kolumnę w Excelu lub wpis w obiekcie pól w JSON.
- Identyczne wartości w obrębie jednego pliku i pola są zapisywane tylko raz. Różne wartości trafiają do osobnych linii w jednej komórce Excela lub do elementów tablicy w JSON.
- Brak wartości oznacza pustą komórkę w Excelu lub pustą tablicę w JSON.
- Wartości są zapisywane jako tekst, dzięki czemu zachowują zera na początku.
- Język interfejsu w chwili rozpoczęcia operacji określa język stałych nagłówków Excela, nazwę arkusza i klucze JSON. Szukane frazy i odczytane wartości nie są tłumaczone.

| Element eksportu | Angielski | Polski |
| --- | --- | --- |
| Arkusz Excel | `Results` | `Wyniki` |
| Nazwa pliku w Excelu | `File name` | `Nazwa pliku` |
| Ścieżka pliku w Excelu | `File path` | `Ścieżka pliku` |
| Nazwa pliku w JSON | `FileName` | `NazwaPliku` |
| Ścieżka pliku w JSON | `FilePath` | `SciezkaPliku` |
| Pola w JSON | `Fields` | `Pola` |

Jeśli inny program odczytuje wyeksportowany JSON, uwzględnij klucze zależne od wybranego języka.

### Ograniczenia odczytu

Wartość jest odczytywana po dwukropku w tej samej linii lub z następnej niepustej linii mieszczącej się w raporcie. Jeśli wartość znajduje się poza tym zakresem, zwiększ liczbę linii po wyniku.

Jeśli następna linia jest inną szukaną etykietą, wartość pozostaje pusta. Nieznana etykieta może jednak zostać potraktowana jako wartość. Na przykład przy wyszukiwaniu wyłącznie pola `Numer sprawy`, pusta etykieta `Numer sprawy:` z następującą po niej linią `Adres: ul. Główna` może dać wynik `Adres: ul. Główna`. Sprawdź wyeksportowane dane przed ich wykorzystaniem.

Wyniki są grupowane według pliku, a nie według osoby lub rekordu w dokumencie. Kilka nazwisk i numerów w jednym pliku nie zostanie automatycznie połączonych w pary.

Wyszukiwanie w PDF odczytuje warstwę tekstową; nie wykonuje automatycznie OCR zeskanowanych stron PDF. W przypadku skanów bez zaznaczalnego tekstu najpierw zapisz strony jako obrazy, a następnie użyj OCR lub wyszukiwania w obrazach. Dokładność OCR zależy od jakości obrazów i dostępnych danych językowych.

## Zamiana tekstu

Zamiana tekstu obsługuje dokumenty DOCX, ODT i ODG, a nie pliki PDF ani zeskanowane obrazy. Korzysta z **pierwszego arkusza pliku Excel**. Pierwszy wiersz jest traktowany jako nagłówek, a kolejne wiersze są odczytywane według położenia kolumn:

| Kolumna | Przeznaczenie | Przykład |
| --- | --- | --- |
| A | Numer (`Lp`) | `1` |
| B | Tekst do znalezienia (`Find`) | `{{FIRMA}}` |
| C | Tekst do wstawienia (`Replace`) | `Przykładowe Studio` |
| D | Ignorowanie wielkości liter (`IgnoreCase`) | `TRUE` lub `FALSE` |

Wiersze z pustą komórką Find są pomijane. Pusta komórka Replace powoduje usunięcie znalezionego tekstu. Aby odwrócić zamianę, zamień miejscami wartości Find i Replace.

Pracuj na kopiach ważnych dokumentów i sprawdzaj wyniki. W plikach DOCX fraza podzielona między kilka węzłów tekstowych XML, na przykład wskutek zmian formatowania, może nie zostać zamieniona jako całość.

## Narzędzia zewnętrzne i uwagi o wynikach

| Narzędzie | Zastosowanie |
| --- | --- |
| LibreOffice | Konwersja dokumentów biurowych. |
| Poppler | Odczyt tekstu z PDF, renderowanie stron, wyodrębnianie obrazów, łączenie i odczyt informacji. |
| qpdf | Operacje na stronach PDF. |
| Tesseract OCR | Rozpoznawanie tekstu na obrazach. |
| ImageMagick | Konwersja obrazów do PDF. |
| Program pdf2docx | Konwersja PDF do DOCX. |

Lokalizacje plików wykonywalnych są określone w [ToolPaths.cs](PdfOperations/Infrastructure/ToolPaths.cs). Zachowaj przewidziane nazwy podfolderów oraz biblioteki i dane pomocnicze narzędzi, nie tylko same pliki wykonywalne. Zainstalowanie narzędzia w innym miejscu lub dodanie go do PATH nie zmienia tych skonfigurowanych lokalizacji.

- Formaty wejściowe konwersji LibreOffice to DOCX, ODT, DOC, ODG, RTF, XLSX, ODS, PPTX i ODP. LibreOffice nie obsługuje każdej kombinacji formatu wejściowego i wyjściowego.
- Wyniki konwersji LibreOffice do HTML są zapisywane w osobnym folderze wraz z obrazami i innymi wygenerowanymi plikami pomocniczymi. Przechowuj je razem.
- Obrazy wyodrębnione z PDF mogą mieć różne formaty, na przykład PNG lub JPEG. Wyodrębnianie obrazów nie jest tym samym co zapisywanie każdej strony jako JPEG.
- Konwersje dokumentów biurowych mogą zależeć od zainstalowanych czcionek i układu dokumentu. Sprawdź pliki wynikowe.
- Dokumenty zabezpieczone hasłem lub uszkodzone mogą powodować błąd. Rozwiń szczegóły błędu, aby zobaczyć komunikat używanego narzędzia.
- Limity czasu dotyczą pojedynczych wywołań narzędzi lub etapów przetwarzania, a nie jednego wspólnego limitu dla całej partii. Wartości domyślne są określone w [OperationTimeouts.cs](PdfOperations/Infrastructure/OperationTimeouts.cs).
- Konwersja LibreOffice monitoruje rozmiar danych tymczasowych i wolne miejsce. Obecne wartości domyślne to limit 2 GiB danych tymczasowych oraz wymagane minimum 2 GiB wolnego miejsca na monitorowanych dyskach. Szczegóły znajdują się w [LibreOfficeDiskGuard.cs](PdfOperations/Infrastructure/LibreOfficeDiskGuard.cs).

Ustawienia języka i motywu są zapisywane w `%APPDATA%\PdfOperations\settings.json`. Nie oznacza to zapisywania bieżących list plików ani niedokończonych operacji między uruchomieniami aplikacji.

## Kompilacja i testy

Praca nad kodem wymaga systemu Windows i .NET 10 SDK. W głównym katalogu repozytorium wykonaj:

```powershell
dotnet build PdfOperations.Gui/PdfOperations.Gui/PdfOperations.Gui.csproj -c Release
dotnet run --project PdfOperations.Gui/PdfOperations.Gui/PdfOperations.Gui.csproj -c Release
```

Narzędzia zewnętrzne są ignorowane przez Git i trzeba je dostarczyć osobno. Podczas kompilacji GUI w konfiguracji Release narzędzia są kopiowane z `PdfOperations/bin/Release/net10.0-windows/tools` do folderu wynikowego GUI. Upewnij się, że gotowa paczka także zawiera kompletny folder `tools` obok pliku wykonywalnego GUI. Sama udana kompilacja .NET nie potwierdza obecności tych zależności.

Uruchomienie testów automatycznych:

```powershell
dotnet test PdfOperations.Tests/PdfOperations.Tests/PdfOperations.Tests.csproj -c Release
```

Część testów to testy integracyjne wymagające odpowiednich narzędzi zewnętrznych. Testy konwersji przez Word wymagają również programu Microsoft Word. Folder `TestData` projektu testowego jest kopiowany do folderu wynikowego testów podczas kompilacji.

Aby zamiast GUI uruchomić pierwotną aplikację konsolową:

```powershell
dotnet run --project PdfOperations/PdfOperations.csproj -c Release
```
