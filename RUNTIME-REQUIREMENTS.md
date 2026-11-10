# Runtime Requirements

[Polski](RUNTIME-REQUIREMENTS.pl.md)

Choose one Windows x64 package:

- **Installer:** includes .NET 10 for the GUI and its worker. It installs to `%LOCALAPPDATA%\Programs\PdfOperations` for the current user without administrator rights. Start **PdfOperations** from the Start menu.
- **Portable ZIP (slim):** requires the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). Under **.NET Desktop Runtime**, choose **Windows x64** (not the SDK). Extract the entire application ZIP to a folder of your choice and start `PdfOperations.Gui.exe`.

PDF-to-DOCX conversion additionally requires [Microsoft Visual C++ v14 Redistributable x64](https://aka.ms/vc14/vc_redist.x64.exe),
version 14.50.35719 or later.

An installed and activated copy of Microsoft Word is needed only for **DOCX to PDF (Word)**.
Without Word, use the bundled LibreOffice for this conversion.

The separately copied `tools/pdf2docx/_internal/MSVCP140.dll` is intentionally not
included. The converter uses the installed system runtime instead. Do not copy
this file from another computer or add it back from System32 when packaging.

Other runtime files supplied within the Python, NumPy, LibreOffice and qpdf
bundles are retained with their existing notices. This document does not grant
redistribution rights to those files or replace their license terms.

No Visual Studio installation is required for end users. Installing prerequisites
such as the system-wide Visual C++ Redistributable may require administrator approval.
