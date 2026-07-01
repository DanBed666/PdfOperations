namespace PdfOperations;

public static class Convert
{
    public static void FileToPdf()
    {
        
    }
    
    public static void PdfToPict()
    {
        
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
    
    public static void PictToPdf()
    {
        
    }
}