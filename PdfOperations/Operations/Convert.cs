namespace PdfOperations;

public static class Convert
{
    public static void FileToPdf(string [] files, string format, string directory)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.LibreOffice];

        string profileDir = Path.Combine(Path.GetTempPath(), "PdfOperationsProfile", Guid.NewGuid().ToString());
        string profileUri = new Uri(profileDir + Path.DirectorySeparatorChar).AbsoluteUri;
        List<string> arguments = new List<string>();
        List<string> arguments2 = new List<string>();

        if (string.IsNullOrEmpty(directory))
        {
            Console.WriteLine("Nie podano katalogu! Dlatego plik zostanie utworzony w katalogu domyślnym!");
        }

        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetExtension(files[i]).Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                && format.Equals("docx"))
            {
                arguments.AddRange([$"-env:UserInstallation={profileUri}", "--infilter=writer_pdf_import", "--convert-to", "odt", ..files, "--outdir", directory]);
                arguments2.AddRange([$"-env:UserInstallation={profileUri}", "--convert-to", format, ..files, "--outdir", directory]);
            }
            
            arguments.AddRange([$"-env:UserInstallation={profileUri}", "--headless", "--convert-to", format, ..files, "--outdir", directory]);
        }
        
        RunClass.Run(tool, arguments);
        RunClass.Run(tool, arguments2);
    }
    
    public static void PdfToPict(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToPpm];
        List<string> arguments = new List<string>();

        arguments.AddRange(["-r", "300", "-jpeg", input, output]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PdfToTxt(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([input, output]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToTxt(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([input, output, "-l", "pol"]);
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToPdf(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        List<string> arguments = new List<string>();
        
        arguments.AddRange([..input, output]);
        RunClass.Run(tool, arguments);
    }
}