# PdfOperations

PdfOperations is a Windows console application for working with PDF files, images, OCR and office documents.

The application is designed as a simple menu-based tool. It uses external command-line tools such as LibreOffice, Poppler, qpdf, Tesseract OCR and ImageMagick.

## Features

PdfOperations can:

- convert office documents with LibreOffice,
- convert PDF files to images,
- convert images to PDF,
- extract text from PDF files,
- extract text from images with OCR,
- extract images from PDF files,
- create new PDF files from selected pages,
- split PDF files into separate pages,
- merge PDF files into one PDF,
- search for phrases in PDF files and images,
- show PDF metadata,
- show PDF font information,
- replace text using placeholder files,
- open multiple files with the default or selected application.

## General usage

Run the application and choose an option from the menu.

Most operations follow this pattern:

1. Choose one or more input files.
2. Optionally preview the selected file.
3. Provide additional data, such as output format, page range or search phrase.
4. Choose the output folder.
5. The program creates output files in the selected folder.

At most prompts you can type `:q` to cancel the current operation.

For yes/no questions:

- `t` = yes
- `n` = no
- `Enter` = no

If you do not provide an output file name, the program uses a default name.

## Page ranges

Some PDF operations ask for page numbers.

Examples:

- `1`
- `2-5`
- `1,3-5,8`

The exact page syntax depends on the underlying PDF tool.

## Placeholder replacement

Placeholder replacement uses a text file with `Find:` and `Replace:` pairs.

Example:

Find: Jan Kowalski
Replace: {{FULL_NAME}}

Find: Gdansk
Replace: {{CITY}}

Find: 123/2026
Replace: {{CASE_NUMBER}}

The application searches for the text after `Find:` and replaces it with the text after `Replace:`.

To reverse the operation, switch the values:

Find: {{FULL_NAME}}
Replace: Jan Kowalski

## Notes

- PDF merging sorts files by file name.
- Some damaged PDF files may be processed with warnings.
- LibreOffice conversion may depend on installed fonts.
- OCR quality depends on image quality and available Tesseract languages.
- Some operations create temporary files during processing.

## External tools

PdfOperations can use the following external tools:

- LibreOffice
- Poppler
- qpdf
- Tesseract OCR
- ImageMagick

These tools must be available in the expected application folders for all operations to work correctly.

## Project status

This project is currently a console-based utility. A graphical interface may be added in the future.