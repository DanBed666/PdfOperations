namespace PdfOperations;

public static class Convert
{
    public static void FileToPdf(OperationInput fileInput, OperationContext context)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.LibreOffice];

        string profileDir = Path.Combine(Path.GetTempPath(), "PdfOperationsProfile", Guid.NewGuid().ToString());
        string profileUri = new Uri(profileDir + Path.DirectorySeparatorChar).AbsoluteUri;
        List<string> arguments = new List<string>();
        List<string> arguments2 = new List<string>();

        bool pdfToOdt =
            fileInput.InputFiles.All(f => Path.GetExtension(f).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
             && fileInput.Format.Equals("odt", StringComparison.OrdinalIgnoreCase);
            
        if (pdfToOdt)    
        {
            arguments.AddRange([$"-env:UserInstallation={profileUri}", "--headless", 
                "--nologo",
                "--nodefault",
                "--nofirststartwizard",
                "--norestore","--infilter=writer_pdf_import", "--convert-to", "odt", ..fileInput.InputFiles, "--outdir", context.TempDir]);
        }
        else
        {
            arguments.AddRange([$"-env:UserInstallation={profileUri}", "--headless", 
                "--nologo",
                "--nodefault",
                "--nofirststartwizard",
                "--norestore","--convert-to", fileInput.Format, ..fileInput.InputFiles, "--outdir", context.TempDir]);
        }
        
        RunClass.Run(tool, arguments);
        
        if (arguments2.Count > 0)
        {
            RunClass.Run(tool, arguments2);
        }
    }

    public static void DocxToPdfWord(FileJob fileJob)
    {
        Type? wordType = Type.GetTypeFromProgID("Word.Application");
        
        if (wordType is null)
            throw new InvalidOperationException("Microsoft Word is not installed or cannot be started.");

        dynamic? word = null;
        dynamic? document = null;

        try
        {
            word = Activator.CreateInstance(wordType);
            word.Visible = false;
            
            document = word.Documents.Open(fileJob.InputFile, ReadOnly: true);
            document.ExportAsFixedFormat(fileJob.TempPath, 17);
        }
        finally 
        {
            if (document is not null)
                document.Close(false);

            if (word is not null)
                word.Quit(false);
        }

        if (!File.Exists(fileJob.TempPath) || new FileInfo(fileJob.TempPath).Length == 0)
            throw new InvalidOperationException($"Word did not create a valid PDF: {fileJob.TempPath}");
        
        ValidatePdfOutput(fileJob.TempPath);
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
        RunClass.Run(tool, arguments);
    }
    
    public static void PdfToTxt(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();

        arguments.AddRange([fileJob.InputFile, fileJob.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PdfToDocx(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Pdf2Docx];
        List<string> arguments = new List<string>();

        arguments.AddRange([fileJob.InputFile, fileJob.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToTxt(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();
        
        string fileNotExt = Files.PrepareTempPathWithoutExt(fileJob.TempPath);
        arguments.AddRange([fileJob.InputFile, fileNotExt, "-l", "pol"]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToPdf(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..fileJob.InputFiles, fileJob.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void ExtractPict(FileJob fileJob)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfImages];
        List<string> arguments = new List<string>();

        arguments.AddRange(["-all", fileJob.InputFile, fileJob.TempPath]);
        RunClass.Run(tool, arguments);
    }
}