using System.Text.RegularExpressions;

namespace PdfOperations;

public class Search
{
    public static void SearchTempTextFiles(OperationInput input, OperationContext context)
    {
        foreach (string f in Directory.GetFiles(context.TempDir))
        {
            SearchResult foundLines = GetFoundLines(f, input.PhraseToFind, input.Before, input.After);
            Files.SaveToFile(foundLines, Path.Combine(context.TempDir, input.Output));
            File.Delete(f);
        }
    }
    
    public static SearchResult GetFoundLines(string inputPath, string phrase, int before, int after)
    {
        int occ = 0;
        
        SearchResult searchResult = new SearchResult()
        {
            FilePath = inputPath
        };
        
        string [] pages = File.ReadAllText(inputPath).Split("\f");
        List<string> linesFound = new List<string>();

        for (int p = 0; p < pages.Length; p++)
        {
            string [] lines = pages[p].Split("\n");

            for (int l = 0; l < lines.Length; l++)
            {
                if (lines[l].Contains(phrase.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    searchResult.PageNumber = p + 1;
                    searchResult.LineNumber = l + 1;
                    occ += Regex.Matches(lines[l], Regex.Escape(phrase), RegexOptions.IgnoreCase).Count;
                
                    linesFound.Add(inputPath);
                    linesFound.Add("Strona: " + searchResult.PageNumber);
                    linesFound.Add("Linia: " + searchResult.LineNumber);
                    linesFound.Add("------------------------------------");
                    linesFound.Add("\n");
                
                    for (int k = -before; k <= after; k++)
                    {
                        int idx = l + k;
                    
                        if (idx >= 0 && idx < lines.Length)
                        {
                            linesFound.Add(lines[idx]);
                        }
                    }
                
                    linesFound.Add("------------------------------------");
                    linesFound.Add("\n");
                }
            }
        }
        
        searchResult.Occurences = occ;

        if (occ == 0)
        {
            searchResult.Lines = new List<string>
            {
                inputPath,
                "------------------------------------",
                "",
                "Nie znaleziono podanej frazy w pliku!",
                "------------------------------------",
                ""
            };

            return searchResult;
        }

        linesFound.Add("Wystąpienia: " + searchResult.Occurences);
        searchResult.Lines = linesFound;
        return searchResult;
    }
}