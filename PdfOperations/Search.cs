namespace PdfOperations;

public class Search
{
    public static string filename = "rnd_fname.txt";
    public static void SearchPicture(string input, string phrase)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        
        string[] arguments = [input, Path.ChangeExtension(filename, null), "-l", "pol"];
        RunClass.Run(tool, arguments);
        SearchTxt(filename, phrase);
    }

    public static void SearchPdf(string input, string phrase)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        RunClass.Run(tool, input, Path.ChangeExtension(filename, null));
        SearchTxt(filename, phrase);
    }

    public static void SearchTxt(string input, string phrase)
    {
        string [] test = File.ReadAllLines(input);

        foreach (string line in test)
        {
            if (line.ToLower().Contains(phrase))
            {
                File.WriteAllText("wyniknowy.txt", line);
            }
        }
    }
}