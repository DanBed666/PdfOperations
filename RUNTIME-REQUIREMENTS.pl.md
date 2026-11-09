# Wymagania do uruchomienia

[English](RUNTIME-REQUIREMENTS.md)

Paczka jest przeznaczona dla systemu Windows x64 i wymaga osobno zainstalowanego
środowiska .NET. Nie zawiera własnej kopii tego środowiska. Aby uruchomić interfejs
graficzny i jego proces pomocniczy, zainstaluj [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
W sekcji **.NET Desktop Runtime** wybierz instalator **Windows x64** (nie SDK).
Wypakuj całe archiwum i uruchom `PdfOperations.Gui.exe`.

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

Użytkownik nie musi instalować Visual Studio. Instalacja wymaganych składników
może wymagać zgody administratora. Paczka nie zawiera instalatora i nie uruchamia
żadnego instalatora automatycznie.
