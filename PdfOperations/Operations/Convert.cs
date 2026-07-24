namespace PdfOperations;

public static class Convert
{
    public static void FileToPdf(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.LibreOffice];

        string profileDir = Path.Combine(Path.GetTempPath(), "PdfOperationsProfile", Guid.NewGuid().ToString());
        string profileUri = new Uri(profileDir + Path.DirectorySeparatorChar).AbsoluteUri;
        List<string> arguments = new List<string>();
        List<string> arguments2 = new List<string>();

        if (string.IsNullOrEmpty(file.dir))
        {
            Console.WriteLine("Nie podano katalogu! Dlatego plik zostanie utworzony w katalogu domyślnym!");
        }

        for (int i = 0; i < file.inputFiles.Length; i++)
        {
            if (Path.GetExtension(file.inputFiles[i]).Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                && file.phrase.Equals("docx"))
            {
                arguments.AddRange([$"-env:UserInstallation={profileUri}", "--headless", "--infilter=writer_pdf_import", "--convert-to", "odt", ..file.inputFiles, "--outdir", file.dir]);
                arguments2.AddRange([$"-env:UserInstallation={profileUri}", "--headless", "--convert-to", file.phrase, ..file.inputFiles, "--outdir", file.dir]);
            }
            
            arguments.AddRange([$"-env:UserInstallation={profileUri}", "--headless", "--convert-to", file.phrase, ..file.inputFiles, "--outdir", file.dir]);
        }
        
        RunClass.Run(tool, arguments);
        RunClass.Run(tool, arguments2);
    }
    
    public static void PdfToPict(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToPpm];
        List<string> arguments = new List<string>();

        arguments.AddRange(["-r", "300", "-jpeg", file.inputFile, file.outputPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PdfToTxt(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([file.inputFile, file.outputPath]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToTxt(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([file.inputFile, file.outputPath, "-l", "pol"]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToPdf(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..file.inputFiles, file.outputFile]);
        RunClass.Run(tool, arguments);
    }
}