namespace PdfOperations;

public class Search
{
    public static string filename = "rnd_fname.txt";
    public static void SearchPicture(string input, string phrase)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();

        if (string.IsNullOrEmpty(phrase))
        {
            Console.WriteLine("Nie podano hasła do wyszukwania!");
            return;
        }
        
        arguments.AddRange([input, Path.ChangeExtension(filename, null), "-l", "pol"]);
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(filename, phrase));
    }

    public static void SearchPdf(string input, string phrase)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();
        
        if (string.IsNullOrEmpty(phrase))
        {
            Console.WriteLine("Nie podano hasła do wyszukwiania!");
            return;
        }

        arguments.AddRange([input, filename]);
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(filename, phrase));
    }

    public static List<String> SearchNewTxt(string input, string phrase)
    {
        string[] test = Files.ReadFile(input);
        List<String> found = new List<string>();

        foreach (string line in test)
        {
            if (line.Contains(phrase, StringComparison.OrdinalIgnoreCase))
            {
                found.Add(line);
            }
        }
        
        return found;
    }
}