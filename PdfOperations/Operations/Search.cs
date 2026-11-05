using System.Text.RegularExpressions;

namespace PdfOperations;

public class Search
{
    public static void SearchTempTextFiles(OperationInput input, OperationContext context, FileJob fileJob)
    {
        SearchTempTextFiles(input, context, fileJob, CancellationToken.None);
    }

    public static List<SearchHit> SearchTempTextFiles(
        OperationInput input,
        OperationContext context,
        FileJob fileJob,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string[] phrases = input.PhrasesToFind.Length > 0
            ? input.PhrasesToFind
            : [input.PhraseToFind];

        if (phrases.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Search phrases cannot be empty.", nameof(input));

        var hits = new List<SearchHit>();
        
        OperationCancellation.Run(
            OperationTimeouts.SearchText,
            "Text search",
            token =>
            {
                string reportPath = Path.Combine(context.TempDir, input.Output);

                foreach (string phrase in phrases.Select(value => value.Trim()))
                {
                    token.ThrowIfCancellationRequested();

                    SearchResult foundLines = GetFoundLines(
                        fileJob.TempPath,
                        phrase,
                        input.Before,
                        input.After,
                        fileJob.InputFile,
                        token,
                        saveWholeLine: input.SearchFields.Any(f => f.SaveWholeLine &&
                                                                   f.Phrase.Equals(phrase, StringComparison.OrdinalIgnoreCase)),
                        knownPhrases: phrases);

                    if (phrases.Length > 1)
                        foundLines.Lines.Insert(0, $"Fraza: {phrase}");

                    token.ThrowIfCancellationRequested();
                    Files.SaveToFile(foundLines, reportPath);
                    hits.AddRange(foundLines.Hits);
                }

                token.ThrowIfCancellationRequested();
                File.Delete(fileJob.TempPath);
            },
            cancellationToken);

        return hits;
    }
    
    public static SearchResult GetFoundLines(string inputTempPath, string phrase, int before, int after,
        string inputFile, CancellationToken cancellationToken = default,
        bool saveWholeLine = false, string[]? knownPhrases = null)
    {
        cancellationToken.ThrowIfCancellationRequested();
        phrase = phrase.Trim();
        ArgumentException.ThrowIfNullOrWhiteSpace(phrase);
        knownPhrases ??= [phrase];
        var labels = knownPhrases.Select(field => field.Trim().TrimEnd(':').TrimEnd())
            .Where(label => label.Length > 0).Select(Regex.Escape).ToArray();
        // Infer an unselected label only after a comma, not from every colon in the value.
        const string inferredLabel = @"(?<=,)\s*\p{L}[\p{L}\p{M}\p{N}_-]*(?:\s+\p{L}[\p{L}\p{M}\p{N}_-]*)*\s*:(?!//)";
        string boundary = labels.Length == 0 ? inferredLabel
            : $@"(?<!\w)(?:{string.Join("|", labels)})\s*:|{inferredLabel}";
        Regex? nextLabel = saveWholeLine ? null
            : new Regex(boundary, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
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
                
                int matchIndex = lines[l].IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
                
                if (matchIndex >= 0)
                {
                    searchResult.PageNumber = p + 1;
                    searchResult.LineNumber = l + 1;
                    occ += Regex.Matches(lines[l], Regex.Escape(phrase), RegexOptions.IgnoreCase).Count;
                    
                    string value = saveWholeLine
                        ? lines[l].Trim()
                        : ReadValueAfterPhrase(lines, l, matchIndex + phrase.Length, knownPhrases, nextLabel);

                    searchResult.Hits.Add(new SearchHit(inputFile, p + 1, l + 1, phrase, value));
                
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
                            string offset = k > 0 ? $"+{k}" : k.ToString();
                            linesFound.Add($"[{offset}] {lines[idx]}");
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

    private static string ReadValueAfterPhrase(string[] lines, int lineIndex,
        int valueStart, string[] knownPhrases, Regex? nextLabel)
    {
        string value = lines[lineIndex][valueStart..].Trim();
        if (value.StartsWith(':'))
            value = value[1..].TrimStart();

        if (value.Length > 0)
            return BeforeNextLabel(value, nextLabel, lines, lineIndex);

        for (int i = lineIndex + 1; i < lines.Length; i++)
        {
            string next = lines[i].Trim();
            if (next.Length == 0)
                continue;

            bool isAnotherField = knownPhrases.Any(field =>
            {
                string label = field.Trim().TrimEnd(':').TrimEnd();
                return label.Length > 0 && next.StartsWith(label, StringComparison.OrdinalIgnoreCase)
                    && (next.Length == label.Length || next[label.Length] == ':' || char.IsWhiteSpace(next[label.Length]));
            });

            return isAnotherField ? "" : BeforeNextLabel(next, nextLabel, lines, i);
        }

        return "";
    }

    private static string BeforeNextLabel(string value, Regex? nextLabel, string[] lines, int lineIndex)
    {
        if (nextLabel?.Match(value) is { Success: true } match)
        {
            string prefix = value[..match.Index].TrimEnd();
            return prefix.EndsWith(',') || prefix.EndsWith(';') ? prefix[..^1].TrimEnd() : prefix;
        }

        // An unselected acronym without a colon is ambiguous. Require numeric values on both sides.
        if (lineIndex + 1 < lines.Length && value.Contains(','))
        {
            var wrapped = Regex.Match(value, @"^(?<number>[0-9]+)\s*,\s*\p{Lu}{2,}$");
            if (wrapped.Success && Regex.IsMatch(lines[lineIndex + 1].Trim(), @"^[0-9]+\s*,?$"))
                return wrapped.Groups["number"].Value;
        }

        return value;
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

            for (int n = start; n < end; n++)
                lines[n] = Regex.Replace(lines[n], @"^\[(?:0|[+-][0-9]+)\] ", "");

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
                    
                    string nextLabel = value.Split(':', 2)[0].Trim();
                    if (input.PhrasesToFind.Any(field =>
                            field.Equals(nextLabel, StringComparison.OrdinalIgnoreCase)))
                    {
                        value = "";
                    }
                }

                results.Add(new ExtractedValue
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    DesiredValue = value
                });
            }

            i = end;
        }

        return results.ToArray();
    }
    
    public static FileExtractionResult[] GetFileExtractionResults(
        OperationInput input, string reportPath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var results = input.InputFiles
            .Select(path => Path.GetFullPath(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => new FileExtractionResult { FilePath = path })
            .ToArray();

        foreach (string phrase in input.PhrasesToFind)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var phraseInput = new OperationInput
            {
                PhraseToFind = phrase,
                PhrasesToFind = input.PhrasesToFind
            };

            var valuesByFile = GetExtractedValues(phraseInput, reportPath)
                .ToLookup(value => Path.GetFullPath(value.FilePath),
                    StringComparer.OrdinalIgnoreCase);

            foreach (FileExtractionResult result in results)
            {
                cancellationToken.ThrowIfCancellationRequested();
                result.Fields[phrase] = valuesByFile[result.FilePath]
                    .Select(value => value.DesiredValue.Trim())
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();
            }
        }

        return results;
    }
    
    public static FileExtractionResult[] SummarizeHits(OperationInput input, IEnumerable<SearchHit> hits)
    {
        var byFile = hits.ToLookup(h => Path.GetFullPath(h.FilePath), StringComparer.OrdinalIgnoreCase);
        return input.InputFiles.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path =>
            {
                var result = new FileExtractionResult { FilePath = path };
                var byPhrase = byFile[path].ToLookup(h => h.Phrase, StringComparer.OrdinalIgnoreCase);
                foreach (string phrase in input.PhrasesToFind)
                    result.Fields[phrase] = byPhrase[phrase].Select(h => h.Value)
                        .Where(v => !string.IsNullOrWhiteSpace(v)).Distinct(StringComparer.Ordinal).ToArray();
                return result;
            }).ToArray();
    }
}
