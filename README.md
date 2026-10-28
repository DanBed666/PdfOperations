# PdfOperations

PdfOperations is a Windows desktop application for working with PDF files, images, text and office documents. It provides an Avalonia GUI in Polish and English, with light, dark and system themes.

The repository also contains the original console application. This guide describes the GUI workflow.

## Features

| Operation | Description |
| --- | --- |
| PDF to TXT | Extract selectable text from PDF files. |
| PDF to DOCX | Convert PDF files into Word documents using the pdf2docx tool. |
| DOCX to PDF (Word) | Convert Word documents using installed Microsoft Word. |
| PDF to images | Render PDF pages as images. |
| Images to PDF | Combine images into a PDF in the selected order. |
| Images to TXT | Recognize text in images with Tesseract OCR. |
| Extract images | Extract embedded images from PDF files. |
| LibreOffice conversion | Convert office documents to PDF, DOCX, ODT, XLSX, HTML or TXT, where supported by LibreOffice. |
| PDF pages | Extract selected, even or odd pages; split after selected pages or into individual pages. |
| Merge PDF | Combine PDFs in the order shown in the file list. |
| PDF from fragments | Combine selected page ranges from multiple PDFs and arrange their order. |
| Search | Search multiple phrases in PDF, TXT and image files; save a TXT report and optionally export extracted values to Excel and JSON. |
| PDF information | Read PDF metadata, page counts and font information. |
| Text replacement | Replace text in DOCX, ODT and ODG documents using an Excel replacement table. |

The GUI includes file counts, file-type filters, progress indicators, cancellation, overwrite control and expandable error details. Language and theme preferences are saved between sessions.

## Requirements and startup

- Windows. The configured external tools use Windows executables, including x64 builds.
- For a framework-dependent build, install the matching .NET 10 Desktop Runtime. A self-contained package includes its .NET runtime.
- Microsoft Word is required only for the **DOCX to PDF (Word)** operation.
- External tools and their supporting files must be present in the expected `tools` directory next to the application.
- The application needs write access to the output and temporary directories, with sufficient free disk space.

For a packaged build, extract the whole archive to a folder and run `PdfOperations.Gui.exe`. Do not run it directly from the archive or move only the executable away from its supporting files.

Choose an operation from the left sidebar to begin. The built-in **Help** view is available in both interface languages.

## General usage

1. Choose an operation from the sidebar.
2. Use **Select files** to choose input files. This replaces the current list.
3. Use **Add files** to append more files, including files from other folders.
4. Set the output folder and any required options, such as a file name, page range or search fields.
5. Click **Start**, then use **Open output folder** to inspect the results.

**Remove** removes selected entries from the list, while **Clear** empties the list. Neither deletes the source files from disk. Multiple entries can be selected for removal.

Where order matters, use **Up** and **Down** before starting. PDF merging uses the displayed list order, not a new alphabetical sort at conversion time.

An empty output-folder field is filled from the first input file where supported. An already selected output folder is not replaced automatically. Check this folder before starting, especially when inputs come from several locations.

When **Overwrite existing files** is off, conflicting output names receive numeric suffixes. Enabling it allows existing destination files to be replaced. Separate inputs with the same base name still need distinct output names within one batch. Keep backups when overwriting.

**Cancel** requests that the operation stop. Cancellation is not an undo action: files already saved may remain in the output folder. Temporary-file cleanup is attempted after completion, failure and cancellation; locked files can still prevent cleanup.

## PDF pages and fragments

Page numbers start at 1. Custom selections support individual pages and ranges, for example `1`, `2-5` or `1,3-5,8`.

- **Selected pages** creates a PDF containing the requested pages.
- **Even pages** and **Odd pages** select pages automatically.
- **Split after pages** treats each entered number as the last page of the preceding part. For example, splitting a seven-page PDF after pages `2,5` produces parts containing pages 1-2, 3-5 and 6-7.
- **Split every page** creates a separate PDF for each page.

Use **PDF from fragments** to assemble page ranges from several source PDFs. Add the ranges, check the page counts, and reorder the fragment list before starting.

## Search and extraction

Search accepts PDF, TXT and image files. Use the file-type filter in the selection dialog to avoid accidentally mixing PDFs with previously generated TXT reports.

Enter one phrase under **Search fields**, then use **Add field** for additional phrases. Empty fields are ignored. At least one phrase is required, and field names must be unique regardless of letter case.

The TXT report contains case-insensitive matches for all phrases, including matches within longer text. The lines-before and lines-after settings control the surrounding context included in the report.

### Excel and JSON exports

Select **Also export to Excel**, **Also export to JSON**, or both. The TXT report is saved as well.

To extract values, use labels found in the documents. Labels are arbitrary user-entered phrases, not a built-in list of phone numbers, tax identifiers or addresses. For example:

```text
Reference: 00123
Company:
Example Studio
```

Searching for `Reference` and `Company` can extract `00123` and `Example Studio`. Extraction matches the entire label, ignoring case and surrounding whitespace; matching only part of a longer label in the TXT report does not necessarily produce an extracted value.

- Each input file produces one Excel row or JSON object. Full paths distinguish files with the same name in different folders.
- Each search field produces an Excel column or an entry under the JSON fields object.
- Identical values within one file and field are stored once. Different values are separate lines in one Excel cell or items in a JSON array.
- Missing values produce an empty Excel cell or an empty JSON array.
- Values are stored as text, preserving leading zeros.
- The interface language at the start of the operation determines the fixed Excel headers, worksheet name and JSON keys. Search phrases and extracted values are not translated.

| Export label | English | Polish |
| --- | --- | --- |
| Excel worksheet | `Results` | `Wyniki` |
| Excel file name | `File name` | `Nazwa pliku` |
| Excel file path | `File path` | `Ścieżka pliku` |
| JSON file name | `FileName` | `NazwaPliku` |
| JSON file path | `FilePath` | `SciezkaPliku` |
| JSON fields | `Fields` | `Pola` |

If another program consumes the exported JSON, account for these language-dependent keys.

### Extraction limitations

Values are read after a colon on the same line or from the next nonempty line within the report context. Increase the lines-after setting if the value lies outside that context.

If the next line is another search label, the value remains empty. An unknown label can instead be mistaken for a value. For example, when searching only for `Reference`, an empty `Reference:` followed by `Address: Main Street` may yield `Address: Main Street`. Review exported data before using it.

Results are grouped by file, not by person or record within a document. Several names and numbers in one file are not automatically paired.

PDF search reads the PDF text layer; it does not automatically OCR scanned PDF pages. For scans without selectable text, render the pages as images first and use OCR or image search. OCR accuracy depends on image quality and available language data.

## Text replacement

Text replacement accepts DOCX, ODT and ODG files, not PDF files or scanned images. It uses the **first worksheet of an Excel file**. The first row is treated as a header; subsequent rows are read by column position:

| Column | Purpose | Example |
| --- | --- | --- |
| A | Number (`Lp`) | `1` |
| B | Text to find (`Find`) | `{{COMPANY}}` |
| C | Replacement text (`Replace`) | `Example Studio` |
| D | Ignore case (`IgnoreCase`) | `TRUE` or `FALSE` |

Rows with an empty Find cell are skipped. An empty Replace cell removes the matched text. To reverse a substitution, swap the Find and Replace values.

Use copies of important documents and inspect the output. In DOCX files, a phrase split across multiple XML text nodes, for example by formatting changes, may not be replaced as one phrase.

## External tools and output notes

| Tool | Used for |
| --- | --- |
| LibreOffice | Office-document conversion. |
| Poppler | PDF text, page rendering, image extraction, merging and information. |
| qpdf | PDF page operations. |
| Tesseract OCR | Text recognition in images. |
| ImageMagick | Image-to-PDF conversion. |
| pdf2docx executable | PDF-to-DOCX conversion. |

Executable locations are defined in [ToolPaths.cs](PdfOperations/Infrastructure/ToolPaths.cs). Keep the expected subdirectory names and the tools' supporting libraries and data files, not just their executables. Installing a tool somewhere else or adding it to PATH does not change these configured locations.

- LibreOffice input formats include DOCX, ODT, DOC, ODG, RTF, XLSX, ODS, PPTX and ODP. Not every input/output combination is supported by LibreOffice.
- LibreOffice HTML output is stored in a dedicated folder with its associated images and other generated assets. Keep these files together.
- Extracted PDF images can have different formats, such as PNG or JPEG; extraction is not the same as rendering every page as JPEG.
- Office conversions can depend on installed fonts and document layout. Check the resulting files.
- Password-protected or damaged documents may fail with an error. Expand the error details for the underlying tool message.
- Time limits apply to individual tool invocations or processing steps, not one fixed limit for the entire batch. Defaults are defined in [OperationTimeouts.cs](PdfOperations/Infrastructure/OperationTimeouts.cs).
- LibreOffice conversion monitors temporary data size and free space. The current defaults are a 2 GiB temporary-data limit and a 2 GiB minimum free-space threshold on monitored drives. See [LibreOfficeDiskGuard.cs](PdfOperations/Infrastructure/LibreOfficeDiskGuard.cs).

Language and theme settings are stored in `%APPDATA%\PdfOperations\settings.json`. This does not save the current file lists or unfinished operations across application restarts.

## Build and test

Development requires Windows and the .NET 10 SDK. From the repository root:

```powershell
dotnet build PdfOperations.Gui/PdfOperations.Gui/PdfOperations.Gui.csproj -c Release
dotnet run --project PdfOperations.Gui/PdfOperations.Gui/PdfOperations.Gui.csproj -c Release
```

The external tools are ignored by Git and must be supplied separately. The GUI build copies tools from `PdfOperations/bin/Release/net10.0-windows/tools` into its Release output. Ensure packaged builds also contain the complete `tools` directory next to the GUI executable; a successful .NET build alone does not verify these dependencies.

Run the automated tests:

```powershell
dotnet test PdfOperations.Tests/PdfOperations.Tests/PdfOperations.Tests.csproj -c Release
```

Some tests are integration tests and require the relevant external tools; Word conversion tests also require Microsoft Word. The test project's `TestData` directory is copied to the test output during the build.

To run the original console application instead:

```powershell
dotnet run --project PdfOperations/PdfOperations.csproj -c Release
```
