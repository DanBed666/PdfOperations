namespace PdfOperations;

public class Search
{
    public static string filename = "rnd_fname.txt";
    public static void SearchPicture(string input, string phrase, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();

        if (string.IsNullOrEmpty(phrase))
        {
            Console.WriteLine("Nie podano hasła do wyszukiwania!");
            return;
        }
        
        arguments.AddRange([input, Path.ChangeExtension(filename, null), "-l", "pol"]);
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(filename, phrase), output);
    }

    public static void SearchPdf(string input, string phrase, string output)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();
        
        if (string.IsNullOrEmpty(phrase))
        {
            Console.WriteLine("Nie podano hasła do wyszukiwania!");
            return;
        }
        
        arguments.AddRange([input, filename]);
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(filename, phrase), output);
    }

    public static void SearchMultiplePdf(string [] input, string phrase, string outputDir)
    {
        int i = 0;
        
        foreach (string file in input)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string newOutputName = Path.Combine(outputDir, $"{i + 1}_{name}.txt");
            
            SearchPdf(file, phrase, newOutputName);
            i++;
        }
    }
    
    public static void SearchMultiplePict(string [] input, string phrase, string outputDir)
    {
        int i = 0;
        
        foreach (string file in input)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string newOutputName = Path.Combine(outputDir, $"{i + 1}_{name}.txt");
            
            SearchPicture(file, phrase, newOutputName);
            i++;
        }
    }

    public static List<List<string>> SearchNewTxt(string input, string phrase)
    {
        List<List<string>> found = new();
        string[] test = Files.ReadFile(input);

        for (int i = 0; i < test.Length; i++)
        {
            if (test[i].Contains(phrase.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                List<string> lines = new List<string>();
                
                for (int k = -2; k <= 2; k++)
                {
                    int idx = i + k;
                    
                    if (idx >= 0 && idx < test.Length)
                    {
                        lines.Add(test[idx]);
                    }
                }
                
                lines.Add("------------------------------------");
                found.Add(lines);
            }
        }

        if (found.Count == 0)
        {
            List<string> lines = new List<string>();
            lines.Add("Nie znaleziono podanej frazy w pliku!");
            found.Add(lines);
        }
        
        return found;
    }
}