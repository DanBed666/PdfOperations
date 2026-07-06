namespace PdfOperations;

public static class Convert
{
    public static void FileToPdf(string [] files, string directory)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.LibreOffice];

        string profileDir = Path.Combine(Path.GetTempPath(), "PdfOperationsProfile", Guid.NewGuid().ToString());
        string profileUri = new Uri(profileDir + Path.DirectorySeparatorChar).AbsoluteUri;

        string[] arguments = [$"-env:UserInstallation={profileUri}", "--headless", "--convert-to", "pdf", ..files, "--outdir", directory];
        
        RunClass.Run(tool, arguments);
    }
    
    public static void PdfToPict(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToPpm];
        
        if (string.IsNullOrEmpty(output))
        {
            string fileName = Path.GetFileNameWithoutExtension(input);
            output = $"{fileName}";
        }
        
        RunClass.Run(tool, "-r", "300", "-jpeg", input, output);
    }
    
    public static void PdfToTxt(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];

        if (string.IsNullOrEmpty(output))
        {
            output = Files.SaveEmptyFile(input);
        }
        
        if (File.Exists(output))
        {
            output = Files.SaveExistingFile(output);    
        }

        RunClass.Run(tool, input, output);
    }
    
    public static void PictToTxt(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        
        if (string.IsNullOrEmpty(output))
        {
            string fileName = Path.GetFileNameWithoutExtension(input);
            output = $"{fileName}.txt";
        }
        
        string[] arguments = [input, output, "-l", "pol"];
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToPdf(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        
        if (string.IsNullOrEmpty(output))
        {
            string fileName = Path.GetFileNameWithoutExtension(input[0]);
            output = $"{fileName}.pdf";
        }
        
        string[] arguments = [..input, output];
        RunClass.Run(tool, arguments);
    }
}