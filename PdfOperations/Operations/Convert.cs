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
        
        for (int i = 0; i < fileInput.InputFiles.Length; i++)
        {
            if (Path.GetExtension(fileInput.InputFiles[i]).Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                && fileInput.Format.Equals("docx"))
            {
                arguments.AddRange([$"-env:UserInstallation={profileUri}", "--headless", 
                    "--nologo",
                    "--nodefault",
                    "--nofirststartwizard",
                    "--norestore","--infilter=writer_pdf_import", "--convert-to", "odt", ..fileInput.InputFiles, "--outdir", context.TempDir]);
                
                string [] filesOdt = new string[fileInput.InputFiles.Length];
                
                for (int k = 0; k < fileInput.InputFiles.Length; k++)
                {
                    filesOdt[k] = Path.Combine(context.TempDir, Path.GetFileNameWithoutExtension(fileInput.InputFiles[k]) + ".odt");
                }
                
                arguments2.AddRange([$"-env:UserInstallation={profileUri}", "--headless", 
                    "--nologo",
                    "--nodefault",
                    "--nofirststartwizard",
                    "--norestore","--convert-to", fileInput.Format, ..filesOdt, "--outdir", context.TempDir]);
            }
            else
            {
                arguments.AddRange([$"-env:UserInstallation={profileUri}", "--headless", 
                    "--nologo",
                    "--nodefault",
                    "--nofirststartwizard",
                    "--norestore","--convert-to", fileInput.Format, ..fileInput.InputFiles, "--outdir", context.TempDir]);
            }
        }
        
        RunClass.Run(tool, arguments);
        RunClass.Run(tool, arguments2);
    }
    
    public static void PdfToPict(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToPpm];
        List<string> arguments = new List<string>();

        arguments.AddRange(["-r", "300", "-jpeg", file.InputFile, file.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PdfToTxt(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.InputFile, file.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToTxt(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([file.InputFile, file.TempPath, "-l", "pol"]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToPdf(OperationInput input, FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..input.InputFiles, file.TempPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void ExtractPict(FileJob file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfImages];
        List<string> arguments = new List<string>();

        arguments.AddRange(["-all", file.InputFile, file.TempPath]);
        RunClass.Run(tool, arguments);
    }
}