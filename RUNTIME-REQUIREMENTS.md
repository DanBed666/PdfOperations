# Runtime Requirements

[Polski](RUNTIME-REQUIREMENTS.pl.md)

This is a framework-dependent Windows x64 package, not a self-contained
publish output. Install the .NET 10 Desktop Runtime (x64) to run the GUI and
its worker. Extract the entire archive and start `PdfOperations.Gui.exe`.

PDF-to-DOCX conversion additionally requires the Microsoft Visual C++ v14
Redistributable for x64, version 14.50.35719 or newer. Download it directly from
Microsoft:

https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist

Microsoft Word is required only for Word-based conversion.

The separately copied `tools/pdf2docx/_internal/MSVCP140.dll` is intentionally not
included. The converter uses the installed system runtime instead. Do not copy
this file from another computer or add it back from System32 when packaging.

Other runtime files supplied within the Python, NumPy, LibreOffice and qpdf
bundles are retained with their existing notices. This document does not grant
redistribution rights to those files or replace their license terms.

No Visual Studio installation is required for end users. Installing prerequisites
may require administrator approval. No installer is bundled or run automatically.
