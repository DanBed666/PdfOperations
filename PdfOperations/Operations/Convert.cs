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