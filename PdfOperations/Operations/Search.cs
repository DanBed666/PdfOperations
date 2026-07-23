namespace PdfOperations;

public class Search
{
    public static void SearchPicture(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.inputFile, Path.ChangeExtension(file.inputFile, null), "-l", "pol"]);
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(file.inputFile, file.phrase), file.outputPath);
    }

    public static void SearchPdf(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.inputFile, file.outputPath]);
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(file.inputFile, file.phrase), file.outputPath);
    }

    public static void SearchMultiplePdf(string [] input, string phrase, string outputDir)
    {
        int i = 0;
        
        foreach (string file in input)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string newOutputName = Path.Combine(outputDir, $"{i + 1}_{name}.txt");
            
            //SearchPdf(file, phrase, newOutputName);
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
            
            //SearchPicture(file, phrase, newOutputName);
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