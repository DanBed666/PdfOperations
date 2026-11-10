namespace PdfOperations;

using System.IO.Compression;
using System.Xml.Linq;

public static class Convert
{
    public static void FileToPdf(OperationInput fileInput, OperationContext context)
    {
        FileToPdf(fileInput, context, CancellationToken.None);
    }

    public static void FileToPdf(OperationInput fileInput, OperationContext context, CancellationToken cancellationToken)
        => FileToPdf(fileInput, context, cancellationToken,
            path => Console.Error.WriteLine($"Ostrzeżenie: nie usunięto folderu tymczasowego: {path}"));

    public static void FileToPdf(OperationInput fileInput, OperationContext context, CancellationToken cancellationToken,
        Action<string> reportCleanupWarning)
        => RunLibreOfficeConversion(fileInput, context, cancellationToken, reportCleanupWarning,
            workDirectory => RunLibreOffice(fileInput, context, cancellationToken, workDirectory));

    internal static void RunLibreOfficeConversion(OperationInput fileInput, OperationContext context,
        CancellationToken cancellationToken, Action<string> reportCleanupWarning, Action<string> convert)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string workDirectory = Directory.CreateTempSubdirectory("PdfOperationsLibre-").FullName;
        Exception? operationError = null;
        try
        {
            convert(workDirectory);
            foreach (string input in fileInput.InputFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string output = Path.Combine(context.TempDir, Path.GetFileNameWithoutExtension(input) + "." + fileInput.Format);
                if (!File.Exists(output))
                    throw new InvalidOperationException($"LibreOffice didn't create file for: {input}");
                if (fileInput.Format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ValidatePdfOutput(output);
                    RunClass.Run(ToolPaths.ToolPathsDict[Tool.Qpdf], ["--check", output],
                        OperationTimeouts.PdfInfo, cancellationToken);
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch (Exception ex)
        {
            operationError = ex;
            throw;
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(workDirectory, operationError, reportCleanupWarning);
        }
    }

    internal static void RunLibreOffice(OperationInput fileInput, OperationContext context,
        CancellationToken cancellationToken, string workDirectory)
    {
        LibreOfficeDiskGuard.Check([workDirectory, context.TempDir]);

        string[] inputs = fileInput.InputFiles.Select(path =>
            fileInput.Format.Equals("txt", StringComparison.OrdinalIgnoreCase)
            && Path.GetExtension(path).Equals(".docx", StringComparison.OrdinalIgnoreCase)
                ? PrepareDocxForText(path, workDirectory) : path).ToArray();

        string profileUri = new Uri(
            Path.Combine(workDirectory, "profile") + Path.DirectorySeparatorChar).AbsoluteUri;
        List<string> arguments = [$"-env:UserInstallation={profileUri}", "--headless",
            "--nologo", "--nodefault", "--nofirststartwizard", "--norestore"];

        if (inputs.All(f => Path.GetExtension(f).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            && fileInput.Format.Equals("odt", StringComparison.OrdinalIgnoreCase))
            arguments.Add("--infilter=writer_pdf_import");

        arguments.AddRange(["--convert-to", fileInput.Format, ..inputs, "--outdir", context.TempDir]);

        string libreTemp = Path.Combine(workDirectory, "temp");
        Directory.CreateDirectory(libreTemp);
        RunClass.RunGuarded(
            ToolPaths.ToolPathsDict[Tool.LibreOffice], arguments,
            OperationTimeouts.LibreOffice, cancellationToken, libreTemp,
            () => LibreOfficeDiskGuard.Check([workDirectory, context.TempDir]));
    }

    private static string PrepareDocxForText(string input, string workDirectory)
    {
        string directory = Path.Combine(workDirectory, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string copy = Path.Combine(directory, Path.GetFileName(input));
        File.Copy(input, copy);

        using var zip = ZipFile.Open(copy, ZipArchiveMode.Update);
        var entry = zip.GetEntry("word/document.xml")
            ?? throw new InvalidDataException("DOCX does not contain word/document.xml.");
        XDocument document;
        using (var stream = entry.Open())
            document = XDocument.Load(stream, LoadOptions.PreserveWhitespace);

        XNamespace w = document.Root!.Name.Namespace;
        XElement? body = document.Root.Element(w + "body");
        var lastContent = body?.Elements().LastOrDefault(e => e.Name != w + "sectPr");
        if (lastContent?.Name != w + "tbl")
            return copy;

        // Work around LibreOffice's TXT export loop for a document ending in a table.
        var paragraph = new XElement(w + "p");
        var section = body!.Element(w + "sectPr");
        if (section is null)
            body.Add(paragraph);
        else
            section.AddBeforeSelf(paragraph);

        entry.Delete();
        using (var stream = zip.CreateEntry("word/document.xml").Open())
            document.Save(stream);
        return copy;
    }

    public static void DocxToPdfWord(FileJob fileJob)
    {
        DocxToPdfWord(fileJob, CancellationToken.None);
    }

    public static void DocxToPdfWord(FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        WordConversionWorker.ConvertFile(fileJob, cancellationToken);
    }

    public static void ValidatePdfOutput(string path)
    {
        if (!File.Exists(path))
            throw new InvalidOperationException($"PDF file was not created: {path}");

        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Length == 0)
            throw new InvalidOperationException($"PDF file is empty: {path}");

        using var stream = File.OpenRead(path);
        Span<byte> header = stackalloc byte[4];

        if (stream.ReadAtLeast(header, header.Length, throwOnEndOfStream: false) < header.Length ||
            header[0] != '%' ||
            header[1] != 'P' ||
            header[2] != 'D' ||
            header[3] != 'F')
        {
            throw new InvalidOperationException($"Created file is not a valid PDF: {path}");
        }
    }
    
    public static void PdfToPict(FileJob fileJob)
    {
        PdfToPict(fileJob, CancellationToken.None);
    }

    public static void PdfToPict(FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToPpm];
        List<string> arguments = new List<string>();
        
        string fileNotExt = Files.PrepareTempPathWithoutExt(fileJob.TempPath);
        arguments.AddRange(["-r", "300", "-jpeg", fileJob.InputFile, fileNotExt]);
        RunClass.Run(tool, arguments, OperationTimeouts.PdfToImages, cancellationToken);
    }
    
    public static void PdfToTxt(FileJob fileJob)
    {
        PdfToTxt(fileJob, CancellationToken.None);
    }

    public static void PdfToTxt(FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PdfToTxt(fileJob, keepPageBreaks: false, cancellationToken);
    }

    public static void PdfToTxtWithPageBreaks(FileJob fileJob)
    {
        PdfToTxtWithPageBreaks(fileJob, CancellationToken.None);
    }

    public static void PdfToTxtWithPageBreaks(FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PdfToTxt(fileJob, keepPageBreaks: true, cancellationToken);
    }

    private static void PdfToTxt(FileJob fileJob, bool keepPageBreaks, CancellationToken cancellationToken)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();

        if (!keepPageBreaks)
            arguments.Add("-nopgbrk");

        arguments.AddRange([fileJob.InputFile, fileJob.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.PdfToTxt, cancellationToken);
    }
    
    public static void PdfToDocx(FileJob fileJob)
    {
        PdfToDocx(fileJob, CancellationToken.None);
    }

    public static void PdfToDocx(FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.Pdf2Docx];
        List<string> arguments = new List<string>();

        arguments.AddRange([fileJob.InputFile, fileJob.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.PdfToDocx, cancellationToken);
    }
    
    public static void PictToTxtWithTables(FileJob fileJob) =>
        PictToTxtWithTables(fileJob, CancellationToken.None);

    public static void PictToTxtWithTables(FileJob fileJob, CancellationToken cancellationToken)
    {
        OperationCancellation.Run(OperationTimeouts.ImageToTxt, "Image to text", token =>
        {
            if (!TableOcr.TryConvert(fileJob, token))
                PictToTxt(fileJob, token);
        }, cancellationToken);
    }

    public static void PictToTxt(FileJob fileJob)
    {
        PictToTxt(fileJob, CancellationToken.None);
    }

    public static void PictToTxt(FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();
        
        string fileNotExt = Files.PrepareTempPathWithoutExt(fileJob.TempPath);
        arguments.AddRange([fileJob.InputFile, fileNotExt, "-l", "pol"]);
        RunClass.Run(tool, arguments, OperationTimeouts.ImageToTxt, cancellationToken);
    }
    
    public static void PictToPdf(FileJob fileJob)
    {
        PictToPdf(fileJob, CancellationToken.None);
    }

    public static void PictToPdf(FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..fileJob.InputFiles, "-auto-orient", fileJob.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.ImageToPdf, cancellationToken);
    }
    
    public static void ExtractPict(FileJob fileJob)
    {
        ExtractPict(fileJob, CancellationToken.None);
    }

    public static void ExtractPict(FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string tool = ToolPaths.ToolPathsDict[Tool.PdfImages];
        List<string> arguments = new List<string>();

        string fileNotExt = Files.PrepareTempPathWithoutExt(fileJob.TempPath);
        arguments.AddRange(["-all", fileJob.InputFile, fileNotExt]);
        RunClass.Run(tool, arguments, OperationTimeouts.ExtractImages, cancellationToken);
    }
}
