namespace PdfOperations;

public class Search
{
    public static void SearchTempTextFiles(OperationInput input, OperationContext context)
    {
        foreach (string f in Directory.GetFiles(context.TempDir))
        {
            //string originalInput = Files8.FindOriginalFileForTemp(f, file.InputFiles);
            List<List<string>> foundLines = GetFoundLines(f, input.PhraseToFind, input.Before, input.After);
            Files8.SaveToFile(foundLines, Path.Combine(context.TempDir, input.Output));
            File.Delete(f);
        }
    }
    
    public static List<List<string>> GetFoundLines(string inputPath, string phrase, int before, int after)
    {
        List<List<string>> found = new();
        string[] inputLines = File.ReadAllLines(inputPath);

        for (int i = 0; i < inputLines.Length; i++)
        {
            if (inputLines[i].Contains(phrase.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                List<string> lines = new List<string>();
                
                lines.Add(inputPath);
                lines.Add("\n");
                
                for (int k = -before; k <= after; k++)
                {
                    int idx = i + k;
                    
                    if (idx >= 0 && idx < inputLines.Length)
                    {
                        lines.Add(inputLines[idx]);
                    }
                }
                
                lines.Add("------------------------------------");
                lines.Add("\n");
                found.Add(lines);
            }
        }

        if (found.Count == 0)
        {
            List<string> lines = new List<string>();
            lines.Add(inputPath);
            lines.Add("\n");
            lines.Add("Nie znaleziono podanej frazy w pliku!");
            lines.Add("------------------------------------");
            lines.Add("\n");
            found.Add(lines);
        }
        
        return found;
    }
}