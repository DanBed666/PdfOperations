namespace PdfOperations;

public static class Convert
{
    public static void FileToPdf(string [] files)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.LibreOffice];

        string profileDir = Path.Combine(Path.GetTempPath(), "PdfOperationsProfile", Guid.NewGuid().ToString());
        string profileUri = new Uri(profileDir + Path.DirectorySeparatorChar).AbsoluteUri;

        string[] arguments = [$"-env:UserInstallation={profileUri}", "--headless", "--convert-to", "pdf", ..files];
        
        RunClass.Run(tool, arguments);
    }
    
    public static void PdfToPict(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToPpm];
        RunClass.Run(tool, "-r", "300", "-jpeg", input, output);
    }
    
    public static void PdfToTxt(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        RunClass.Run(tool, input, output);
    }
    
    public static void PictToPdf(string [] input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        
        string[] arguments = [..input, output, "-l", "pol"];
        RunClass.Run(tool, arguments);
    }
    
    public static void PictToTxt(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        RunClass.Run(tool, input, output);
    }
}