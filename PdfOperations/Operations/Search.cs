using System.Text.RegularExpressions;

namespace PdfOperations;

public class Search
{
    public static void SearchTempTextFiles(OperationInput input, OperationContext context, FileJob fileJob)
    {
        SearchTempTextFiles(input, context, fileJob, CancellationToken.None);
    }

    public static void SearchTempTextFiles(OperationInput input, OperationContext context, FileJob fileJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OperationCancellation.Run(OperationTimeouts.SearchText, "Text search", token =>
        {
            SearchResult foundLines = GetFoundLines(fileJob.TempPath, input.PhraseToFind,
                input.Before, input.After, fileJob.InputFile, token);
            token.ThrowIfCancellationRequested();
            Files.SaveToFile(foundLines, Path.Combine(context.TempDir, input.Output));
            token.ThrowIfCancellationRequested();
            File.Delete(fileJob.TempPath);
        }, cancellationToken);
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
    
    public static ExtractedValue[] GetExtractedValues(OperationInput input, string reportPath)
    {
        string phrase = input.PhraseToFind.Trim().TrimEnd(':').TrimEnd();
        if (string.IsNullOrWhiteSpace(phrase))
            throw new ArgumentException("Search phrase cannot be empty.", nameof(input));

        string[] lines = File.ReadAllLines(reportPath);
        var results = new List<ExtractedValue>();
        const string separator = "------------------------------------";

        for (int i = 0; i + 3 < lines.Length; i++)
        {
            string filePath = lines[i].Trim();

            // Only read document content inside a complete report result block.
            if (!Path.IsPathFullyQualified(filePath) ||
                !lines[i + 1].StartsWith("Strona: ", StringComparison.Ordinal) ||
                !lines[i + 2].StartsWith("Linia: ", StringComparison.Ordinal) ||
                lines[i + 3].Trim() != separator)
                continue;

            int start = i + 4;
            int end = start;
            while (end < lines.Length && lines[end].Trim() != separator)
                end++;

            for (int j = start; j < end; j++)
            {
                string line = lines[j].Trim();
                int colon = line.IndexOf(':');
                string label = colon >= 0 ? line[..colon].Trim() : line;
                if (!label.Equals(phrase, StringComparison.OrdinalIgnoreCase))
                    continue;

                string value = colon >= 0 ? line[(colon + 1)..].Trim() : "";
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = lines.Skip(j + 1).Take(end - j - 1)
                        .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text))
                        ?.Trim() ?? "";
                }

                results.Add(new ExtractedValue
                {
                    FileName = Path.GetFileName(filePath),
                    DesiredValue = value
                });
            }

            i = end;
        }

        return results.ToArray();
    }
}
