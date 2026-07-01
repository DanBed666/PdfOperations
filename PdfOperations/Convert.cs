namespace PdfOperations;

public static class Convert
{
    public static void FileToPdf(string file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.LibreOffice];
        RunClass.Run(tool, "--headless", "--convert-to", "pdf", file);
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
    
    public static void PictToTxt(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        RunClass.Run(tool, input, output, "-l", "pol");
    }
    
    public static void PictToPdf(string input, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Magick];
        RunClass.Run(tool, input, output);
    }
}