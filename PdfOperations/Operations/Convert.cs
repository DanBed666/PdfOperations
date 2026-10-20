namespace PdfOperations;

using System.IO.Compression;
using System.Xml.Linq;

public static class Convert
{
    public static void FileToPdf(OperationInput fileInput, OperationContext context)
    {
        string workDirectory = Directory.CreateTempSubdirectory("PdfOperationsLibre-").FullName;
        Exception? operationError = null;
        try
        {
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
            RunClass.Run(ToolPaths.ToolPathsDict[Tool.LibreOffice], arguments, OperationTimeouts.LibreOffice);
        }
        catch (Exception ex)
        {
            operationError = ex;
            throw;
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(workDirectory, operationError);
        }
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
        WordConversionWorker.ConvertFile(fileJob);
    }

    public static void ValidatePdfOutput(string path)
    {
        if (!File.Exists(path))
            throw new InvalidOperationException($"PDF file was not created: {path}");

        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Length == 0)
            throw new InvalidOperationException($"PDF file is empty: {path}");

        byte[] header = File.ReadAllBytes(path).Take(4).ToArray();

        if (header.Length < 4 ||
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
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToPpm];
        List<string> arguments = new List<string>();
        
        string fileNotExt = Files.PrepareTempPathWithoutExt(fileJob.TempPath);
        arguments.AddRange(["-r", "300", "-jpeg", fileJob.InputFile, fileNotExt]);
        RunClass.Run(tool, arguments, OperationTimeouts.PdfToImages);
    }
    
    public static void PdfToTxt(FileJob fileJob)
    {
        PdfToTxt(fileJob, keepPageBreaks: false);
    }

    public static void PdfToTxtWithPageBreaks(FileJob fileJob)
    {
        PdfToTxt(fileJob, keepPageBreaks: true);
    }

    private static void PdfToTxt(FileJob fileJob, bool keepPageBreaks)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();

        if (!keepPageBreaks)
            arguments.Add("-nopgbrk");

        arguments.AddRange([fileJob.InputFile, fileJob.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.PdfToTxt);
    }
    
    public static void PdfToDocx(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Pdf2Docx];
        List<string> arguments = new List<string>();

        arguments.AddRange([fileJob.InputFile, fileJob.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.PdfToDocx);
    }
    
    public static void PictToTxt(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();
        
        string fileNotExt = Files.PrepareTempPathWithoutExt(fileJob.TempPath);
        arguments.AddRange([fileJob.InputFile, fileNotExt, "-l", "pol"]);
        RunClass.Run(tool, arguments, OperationTimeouts.ImageToTxt);
    }
    
    public static void PictToPdf(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..fileJob.InputFiles, fileJob.TempPath]);
        RunClass.Run(tool, arguments, OperationTimeouts.ImageToPdf);
    }
    
    public static void ExtractPict(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfImages];
        List<string> arguments = new List<string>();

        string fileNotExt = Files.PrepareTempPathWithoutExt(fileJob.TempPath);
        arguments.AddRange(["-all", fileJob.InputFile, fileNotExt]);
        RunClass.Run(tool, arguments, OperationTimeouts.ExtractImages);
    }
}
