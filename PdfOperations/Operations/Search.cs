using System.Text.RegularExpressions;

namespace PdfOperations;

public class Search
{
    public static void SearchTempTextFiles(OperationInput input, OperationContext context, FileJob fileJob)
    {
        OperationCancellation.Run(OperationTimeouts.SearchText, "Text search", token =>
        {
            SearchResult foundLines = GetFoundLines(fileJob.TempPath, input.PhraseToFind,
                input.Before, input.After, fileJob.InputFile, token);
            token.ThrowIfCancellationRequested();
            Files.SaveToFile(foundLines, Path.Combine(context.TempDir, input.Output));
            token.ThrowIfCancellationRequested();
            File.Delete(fileJob.TempPath);
        });
    }
    
    public static SearchResult GetFoundLines(string inputTempPath, string phrase, int before, int after,
        string inputFile, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        int occ = 0;
        
        SearchResult searchResult = new SearchResult()
        {
            FilePath = inputTempPath
        };
        
        string [] pages = File.ReadAllText(inputTempPath).Split("\f");
        cancellationToken.ThrowIfCancellationRequested();
        List<string> linesFound = new List<string>();

        for (int p = 0; p < pages.Length; p++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string [] lines = pages[p].Split("\n");

            for (int l = 0; l < lines.Length; l++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (lines[l].Contains(phrase.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    searchResult.PageNumber = p + 1;
                    searchResult.LineNumber = l + 1;
                    occ += Regex.Matches(lines[l], Regex.Escape(phrase), RegexOptions.IgnoreCase).Count;
                
                    linesFound.Add("\n");
                    linesFound.Add(inputFile);
                    linesFound.Add("Strona: " + searchResult.PageNumber);
                    linesFound.Add("Linia: " + searchResult.LineNumber);
                    linesFound.Add("------------------------------------");
                    linesFound.Add("\n");
                
                    for (int k = -before; k <= after; k++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
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
        
        cancellationToken.ThrowIfCancellationRequested();
        searchResult.Occurences = occ;

        if (occ == 0)
        {
            searchResult.Lines = new List<string>
            {
                "\n",
                inputFile,
                "------------------------------------",
                "",
                "Nie znaleziono podanej frazy w pliku!",
                "------------------------------------",
                ""
            };

            return searchResult;
        }

        linesFound.Add("Wystąpienia w całym pliku: " + searchResult.Occurences);
        searchResult.Lines = linesFound;
        return searchResult;
    }
}
