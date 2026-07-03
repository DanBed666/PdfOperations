namespace PdfOperations;

public class Search
{
    public static string filename = "rnd_fname.txt";
    public static void SearchPicture(string input, string phrase)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        
        string[] arguments = [input, Path.ChangeExtension(filename, null), "-l", "pol"];
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(filename, phrase));
    }

    public static void SearchPdf(string input, string phrase)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        RunClass.Run(tool, input, filename);
        Files.SaveToFile(SearchNewTxt(filename, phrase));
    }

    public static List<String> SearchNewTxt(string input, string phrase)
    {
        string[] test = Files.ReadFile(input);
        List<String> found = new List<string>();

        foreach (string line in test)
        {
            string l = line.ToLower();
            
            if (l.Contains(phrase))
            {
                found.Add(line);
            }
        }
        
        return found;
    }
}