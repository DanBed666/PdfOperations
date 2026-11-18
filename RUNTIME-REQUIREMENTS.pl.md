# Wymagania do uruchomienia

[English](RUNTIME-REQUIREMENTS.md)

Obecne wydanie to **przenośna paczka ZIP dla Windows x64**, bez osobnego instalatora.

Plik `*-win-x64-portable.zip` zawiera .NET 10 Desktop Runtime dla aplikacji graficznej i jej procesu pomocniczego. **Nie trzeba osobno instalować .NET.** Rozpakuj cały ZIP do nowego folderu z prawem zapisu i uruchom **`PdfOperations.exe` z jego głównego poziomu**. Zachowaj obok niego `current`, `Update.exe` i `.portable`; dołączone narzędzia znajdują się w `current/tools`.

Zwykłe korzystanie z programu nie wymaga uprawnień administratora, ale nadal obowiązują zabezpieczenia Windows i zasady organizacji. Foldery wynikowe i tymczasowe muszą mieć prawo zapisu i wystarczającą ilość wolnego miejsca. Aktualizacje Velopack wymagają także prawa zapisu i wolnego miejsca w folderze portable. Zobacz [Aktualizacje](README.pl.md#aktualizacje).

## PDF na DOCX

**Nie trzeba osobno instalować Visual C++.** Wymagana biblioteka jest dołączona jako `current/tools/pdf2docx/_internal/MSVCP140.dll`. Program pyta o akceptację jej warunków przed pierwszą konwersją PDF na DOCX i zapamiętuje zgodę dla Twojego konta Windows. Odmowa nie blokuje innych operacji.

Tylko starsze paczki wydane bez tej biblioteki wymagają do konwersji PDF na DOCX systemowego pakietu [Microsoft Visual C++ v14 Redistributable x64](https://aka.ms/vc14/vc_redist.x64.exe) w wersji 14.50.35719 lub nowszej. Visual Studio nie jest potrzebne.

## DOCX na PDF

Zainstalowany i aktywowany Microsoft Word jest potrzebny tylko do opcji **DOCX do PDF (Word)**.
Bez Worda można wykonać tę konwersję przez dołączone LibreOffice.

## Informacje o bibliotekach

Pozostałe pliki środowisk uruchomieniowych dostarczone w pakietach Python, NumPy,
LibreOffice i qpdf pozostawiono wraz z ich dotychczasowymi informacjami licencyjnymi.
Ten dokument nie przyznaje praw do rozpowszechniania tych plików ani nie zastępuje
warunków ich licencji.
