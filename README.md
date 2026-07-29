# PdfOperations

PdfOperations is a Windows console application for working with PDF files,
images, OCR results, and office documents. It wraps external command-line tools
behind a simpler menu-based interface.

## Features

- Convert office documents to PDF with LibreOffice
- Convert PDF files to TXT
- Convert PDF files to images
- Convert images to PDF
- Convert PDF files to DOCX/ODT with LibreOffice import filters
- Run OCR on images and save the result as TXT
- Merge multiple PDF files into one PDF
- Split one PDF into separate pages
- Create a new PDF from selected pages
- Extract PDF information
- Diagnose fonts used in PDF files
- Extract embedded images from PDF files
- Extract PDF attachments
- Search for phrases in text files and PDF-derived text
- Open generated files or output folders after an operation

## External Tools

The application uses portable command-line tools stored in the `tools` folder:

- LibreOffice / `soffice`
- Poppler, for example `pdftoppm`, `pdftotext`, `pdfinfo`, `pdffonts`
- ImageMagick / `magick`
- Tesseract OCR
- QPDF

## Running

From the published application folder:

```powershell
.\PdfOperations.exe
```

The console menu guides the user through the available operations.
