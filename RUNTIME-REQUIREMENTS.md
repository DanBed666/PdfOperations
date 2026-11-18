# Runtime Requirements

[Polski](RUNTIME-REQUIREMENTS.pl.md)

The current release is a **portable ZIP for Windows x64**, without a separate installer.

The `*-win-x64-portable.zip` includes the .NET 10 Desktop Runtime for the GUI and its worker. **No separate .NET installation is needed.** Extract the entire ZIP to a new writable folder and run **`PdfOperations.exe` at its top level**. Keep `current`, `Update.exe` and `.portable` beside it; bundled tools are in `current/tools`.

Normal use does not require administrator rights, but Windows security checks and organizational policies still apply. Output and temporary folders must be writable and have enough free disk space. Velopack updates also need write access and free space in the portable folder. See [Updates](README.md#updates).

## PDF to DOCX

**You do not need to install Visual C++ separately.** The required DLL is included at `current/tools/pdf2docx/_internal/MSVCP140.dll`. The app asks you to accept its terms before your first PDF-to-DOCX conversion. Acceptance is remembered for your Windows account; declining does not block other operations.

Only older packages that shipped without this DLL need the system-wide [Microsoft Visual C++ v14 Redistributable x64](https://aka.ms/vc14/vc_redist.x64.exe), version 14.50.35719 or later, for PDF-to-DOCX conversion. You do not need Visual Studio.

## DOCX to PDF

An installed and activated copy of Microsoft Word is needed only for **DOCX to PDF (Word)**.
Without Word, use the bundled LibreOffice for this conversion.

## Library notices

Other runtime files supplied within the Python, NumPy, LibreOffice and qpdf
bundles are retained with their existing notices. This document does not grant
redistribution rights to those files or replace their license terms.
