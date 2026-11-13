# Runtime Requirements

[Polski](RUNTIME-REQUIREMENTS.pl.md)

Choose one Windows x64 package:

- **Installer:** includes .NET 10 for the GUI and its worker. It installs to `%LOCALAPPDATA%\Programs\PdfOperations` for the current user without administrator rights. Start **PdfOperations** from the Start menu.
- **Portable ZIP (`*-win-x64-portable.zip`):** includes the .NET 10 Desktop Runtime, so you do not need to install .NET separately. Extract the **entire** ZIP to a folder of your choice and run `PdfOperations.Gui.exe` from that folder.

## PDF to DOCX

With either package, **you do not need to install Visual C++ separately** if it contains `tools/pdf2docx/_internal/MSVCP140.dll`. The installer displays the library terms during setup. The portable version asks before your first PDF-to-DOCX conversion. Acceptance is remembered for your Windows account; declining in the portable version does not block other operations.

Only older packages that shipped without this DLL need the system-wide [Microsoft Visual C++ v14 Redistributable x64](https://aka.ms/vc14/vc_redist.x64.exe), version 14.50.35719 or later, for PDF-to-DOCX conversion. You do not need Visual Studio.

## DOCX to PDF

An installed and activated copy of Microsoft Word is needed only for **DOCX to PDF (Word)**.
Without Word, use the bundled LibreOffice for this conversion.

## Library notices

Other runtime files supplied within the Python, NumPy, LibreOffice and qpdf
bundles are retained with their existing notices. This document does not grant
redistribution rights to those files or replace their license terms.
