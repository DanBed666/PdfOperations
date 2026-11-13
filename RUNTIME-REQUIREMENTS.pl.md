# Wymagania do uruchomienia

[English](RUNTIME-REQUIREMENTS.md)

Wybierz jedną wersję dla Windows x64:

- **Instalator:** zawiera .NET 10 dla aplikacji graficznej i jej procesu pomocniczego. Instaluje program dla bieżącego użytkownika w `%LOCALAPPDATA%\Programs\PdfOperations`, bez uprawnień administratora. Aby uruchomić program, wybierz **PdfOperations** w menu Start.
- **Wersja przenośna (`*-win-x64-portable.zip`):** zawiera środowisko .NET 10 Desktop Runtime, więc nie trzeba osobno instalować .NET. Rozpakuj **całe** archiwum ZIP do wybranego folderu i uruchom z niego `PdfOperations.Gui.exe`.

Konwersja PDF do DOCX wymaga dodatkowo [Microsoft Visual C++ v14 Redistributable x64](https://aka.ms/vc14/vc_redist.x64.exe)
w wersji 14.50.35719 lub nowszej.

Zainstalowany i aktywowany Microsoft Word jest potrzebny tylko do opcji **DOCX do PDF (Word)**.
Bez Worda można wykonać tę konwersję przez dołączone LibreOffice.

## Informacje o bibliotekach

Celowo nie dołączono osobno kopiowanego pliku
`tools/pdf2docx/_internal/MSVCP140.dll`. Konwerter korzysta zamiast niego
z zainstalowanego środowiska systemowego. Podczas przygotowywania paczki nie kopiuj
tego pliku z innego komputera ani z folderu System32.

Pozostałe pliki środowisk uruchomieniowych dostarczone w pakietach Python, NumPy,
LibreOffice i qpdf pozostawiono wraz z ich dotychczasowymi informacjami licencyjnymi.
Ten dokument nie przyznaje praw do rozpowszechniania tych plików ani nie zastępuje
warunków ich licencji.

Użytkownik nie musi instalować Visual Studio. Osobna instalacja składników systemowych, takich jak pakiet Visual C++ Redistributable, może wymagać zgody administratora.
