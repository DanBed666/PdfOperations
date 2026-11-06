using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ClosedXML.Excel;

namespace PdfOperations;

public class Replacement
{
    public static void ReplaceTextWithPlaceholders(FileJob file, OperationInput input, OperationContext context)
    {
        ReplaceTextWithPlaceholders(file, input, context, CancellationToken.None);
    }

    public static void ReplaceTextWithPlaceholders(FileJob file, OperationInput input, OperationContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OperationCancellation.Run(OperationTimeouts.Replacement, "Text replacement", token =>
        {
            token.ThrowIfCancellationRequested();
            string extractedDir = ExtractDocumentToTemp(file.InputFile, file.TempPath);
            token.ThrowIfCancellationRequested();
            string xmlPath = GetEditableXmlPath(extractedDir, file.InputFile);
            List<ReplacementPair> replacementPairs = ReadReplacementsFromExcel(input.PlaceholderFile, token);
            ReplaceTextInFile(xmlPath, replacementPairs, Path.GetExtension(file.InputFile), token);
            token.ThrowIfCancellationRequested();
            CreateDocumentFromDirectory(input, extractedDir, file.TempPath, Path.GetExtension(file.InputFile));
            token.ThrowIfCancellationRequested();
        }, cancellationToken);
    }
    
    public static string ExtractDocumentToTemp(string inputFile, string tempPath)
    {
        string tempDir = Path.GetDirectoryName(tempPath)!;
        string tempName = Path.GetFileNameWithoutExtension(tempPath);
        string tempDirExt = Path.Combine(tempDir, $"{tempName}_extracted_{Guid.NewGuid():N}");
        
        ZipFile.ExtractToDirectory(inputFile, tempDirExt);

        return tempDirExt;
    }

    public static string GetEditableXmlPath(string extractedDir, string inputFile)
    {
        string extension = Path.GetExtension(inputFile);
        
        if (extension.Equals(".docx"))
            return Path.Combine(extractedDir, "word", "document.xml");
        
        if (extension.Equals(".odg") || extension.Equals(".odt"))
            return Path.Combine(extractedDir, "content.xml");

        throw new InvalidOperationException($"Nieprawidłowe rozszerzenie {extension}");
    }

    public static List<ReplacementPair> ReadReplacementsFromExcel(string placeholderFile,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        List<ReplacementPair> replacementPairs = new List<ReplacementPair>();
        
        using XLWorkbook workbook = new XLWorkbook(placeholderFile);
        cancellationToken.ThrowIfCancellationRequested();
        IXLWorksheet worksheet = workbook.Worksheets.FirstOrDefault() ??
                                 throw new InvalidOperationException("Plik excel nie zawiera żadnego arkusza!");

        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
        {
            cancellationToken.ThrowIfCancellationRequested();
            string find = row.Cell(2).GetString();
            string replace = row.Cell(3).GetString();

            if (string.IsNullOrWhiteSpace(find))
                continue;

            int lp = 0;
            int.TryParse(row.Cell(1).GetString(), out lp);

            bool ignoreCase = false;
            bool.TryParse(row.Cell(4).GetString(), out ignoreCase);

            ReplacementPair replacementPair = new ReplacementPair()
            {
                Lp = lp,
                Find = find,
                Replace = replace,
                IgnoreCase = ignoreCase
            };
            
            replacementPairs.Add(replacementPair);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return replacementPairs;
    }
    
    public static void ReplaceTextInFile(string xmlPath, List<ReplacementPair> pairs, string extension,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            ReplaceTextInDocxXml(xmlPath, pairs, cancellationToken);
        else if (extension.Equals(".odt", StringComparison.OrdinalIgnoreCase) ||
                 extension.Equals(".odg", StringComparison.OrdinalIgnoreCase))
            ReplaceTextInOdfXml(xmlPath, pairs, cancellationToken);
        else
            throw new InvalidOperationException($"Nieobsługiwany format: {extension}");
    }

    public static void ReplaceTextInDocxXml(string xmlPath, List<ReplacementPair> replacementPairs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        XDocument doc = XDocument.Load(xmlPath, System.Xml.Linq.LoadOptions.PreserveWhitespace);
        cancellationToken.ThrowIfCancellationRequested();
        XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        foreach (XElement paragraph in doc.Descendants(w + "p").ToArray())
        {
            var nodes = new List<XElement>();
            CollectText(paragraph);
            FlushText();

            void FlushText()
            {
                ReplaceTextAcrossNodes(nodes, replacementPairs, node => node.Value, (node, value) =>
                {
                    node.Value = value;
                    node.SetAttributeValue(XNamespace.Xml + "space", "preserve");
                }, cancellationToken);
                nodes.Clear();
            }

            void CollectText(XElement element)
            {
                cancellationToken.ThrowIfCancellationRequested();
                // Do not join text across breaks, fields, drawings or nested paragraphs.
                if ((element.Name == w + "p" && element != paragraph) ||
                    (element.Name.Namespace == w && element.Name.LocalName is
                        "tab" or "br" or "cr" or "drawing" or "pict" or "object" or
                        "fldChar" or "instrText" or "fldSimple" or "sym" or
                        "noBreakHyphen" or "softHyphen" or "del"))
                {
                    FlushText();
                    return;
                }

                if (element.Name == w + "t")
                    nodes.Add(element);
                else
                    foreach (XElement child in element.Elements())
                        CollectText(child);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        doc.Save(xmlPath, System.Xml.Linq.SaveOptions.DisableFormatting);
        cancellationToken.ThrowIfCancellationRequested();
    }

    private static void ReplaceTextAcrossNodes<T>(List<T> nodes, List<ReplacementPair> pairs,
        Func<T, string> getText, Action<T, string> setText, CancellationToken cancellationToken)
    {
        if (nodes.Count == 0)
            return;

        foreach (ReplacementPair pair in pairs)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(pair.Find))
                continue;

            int offset = 0;
            var spans = nodes.Select(node =>
            {
                int start = offset;
                offset += getText(node).Length;
                return (Node: node, Start: start, End: offset);
            }).ToArray();
            string text = string.Concat(nodes.Select(getText));
            StringComparison comparison = pair.IgnoreCase
                ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var matches = new List<int>();

            for (int position = 0; position <= text.Length - pair.Find.Length;)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int found = text.IndexOf(pair.Find, position, comparison);
                if (found < 0)
                    break;
                matches.Add(found);
                position = found + pair.Find.Length;
            }

            // Work backwards so earlier matches keep their original node offsets.
            foreach (int start in matches.AsEnumerable().Reverse())
            {
                int end = start + pair.Find.Length;
                foreach (var span in spans)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    int left = Math.Max(start, span.Start);
                    int right = Math.Min(end, span.End);
                    if (left >= right)
                        continue;

                    string value = getText(span.Node).Remove(left - span.Start, right - left);
                    if (start >= span.Start && start < span.End)
                        value = value.Insert(start - span.Start, pair.Replace ?? "");
                    setText(span.Node, value);
                }
            }
        }
    }
    
    public static void ReplaceTextInOdfXml(string xmlPath, List<ReplacementPair> replacementPairs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        XDocument doc = XDocument.Load(xmlPath, System.Xml.Linq.LoadOptions.PreserveWhitespace);
        cancellationToken.ThrowIfCancellationRequested();
        XNamespace text = "urn:oasis:names:tc:opendocument:xmlns:text:1.0";
        XNamespace office = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";

        var paragraphs = doc.Descendants(office + "body").Descendants()
            .Where(element => element.Name == text + "p" || element.Name == text + "h").ToArray();
        foreach (XElement paragraph in paragraphs)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var nodes = new List<XText>();
            var originals = new List<(XText Node, string Value, XElement? Space)>();
            CollectText(paragraph);
            FlushText();

            void FlushText()
            {
                ReplaceTextAcrossNodes(nodes, replacementPairs, node => node.Value,
                    (node, value) => node.Value = value, cancellationToken);
                foreach (var original in originals)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (original.Node.Value != original.Value)
                        original.Node.ReplaceWith(EncodeOdfText(original.Node.Value, text));
                    else if (original.Space is not null)
                        original.Node.ReplaceWith(original.Space);
                }
                nodes.Clear();
                originals.Clear();
            }

            void CollectText(XElement element)
            {
                foreach (XNode child in element.Nodes().ToArray())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (child is XText node)
                    {
                        nodes.Add(node);
                        originals.Add((node, node.Value, null));
                    }
                    else if (child is XElement inline)
                    {
                        if (inline.Name == text + "span" || inline.Name == text + "a")
                            CollectText(inline);
                        else if (inline.Name == text + "s")
                        {
                            // ODF stores explicit spaces as elements, not ordinary text.
                            var spaces = new XText(new string(' ', (int?)inline.Attribute(text + "c") ?? 1));
                            inline.ReplaceWith(spaces);
                            nodes.Add(spaces);
                            originals.Add((spaces, spaces.Value, inline));
                        }
                        else if (!(inline.Name.Namespace == text && inline.Name.LocalName is
                            "bookmark" or "bookmark-start" or "bookmark-end" or
                            "reference-mark" or "reference-mark-start" or "reference-mark-end"))
                        {
                            // Breaks, fields and embedded objects separate searchable text.
                            FlushText();
                        }
                    }
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        doc.Save(xmlPath, System.Xml.Linq.SaveOptions.DisableFormatting);
        cancellationToken.ThrowIfCancellationRequested();
    }

    private static IEnumerable<XNode> EncodeOdfText(string value, XNamespace text)
    {
        for (int index = 0; index < value.Length;)
        {
            char character = value[index];
            int start = index++;
            if (character == ' ')
            {
                while (index < value.Length && value[index] == ' ')
                    index++;
                yield return new XElement(text + "s", new XAttribute(text + "c", index - start));
            }
            else if (character == '\t')
                yield return new XElement(text + "tab");
            else if (character is '\r' or '\n')
            {
                if (character == '\r' && index < value.Length && value[index] == '\n')
                    index++;
                yield return new XElement(text + "line-break");
            }
            else
            {
                while (index < value.Length && value[index] is not (' ' or '\t' or '\r' or '\n'))
                    index++;
                yield return new XText(value[start..index]);
            }
        }
    }

    public static void CreateDocumentFromDirectory(OperationInput input, string extDir, string tempPath, string extension)
    {
        if (string.IsNullOrWhiteSpace(input.Output) && input.InputFiles.Length > 1)
            ZipFile.CreateFromDirectory(extDir, tempPath + extension);
        else
            ZipFile.CreateFromDirectory(extDir, tempPath);
    }

    public static void UpdateDocxDates(string tempDir)
    {
        string corePath = Path.Combine(tempDir, "docProps", "core.xml");

        if (!File.Exists(corePath))
            return;
        
        string core = File.ReadAllText(corePath);
        string now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        
        core = Regex.Replace(
            core,
            @"<dcterms:created\b[^>]*>.*?</dcterms:created>",
            $@"<dcterms:created xsi:type=""dcterms:W3CDTF"">{now}</dcterms:created>",
            RegexOptions.Singleline);

        core = Regex.Replace(
            core,
            @"<dcterms:modified\b[^>]*>.*?</dcterms:modified>",
            $@"<dcterms:modified xsi:type=""dcterms:W3CDTF"">{now}</dcterms:modified>",
            RegexOptions.Singleline);
        
        File.WriteAllText(corePath, core);
    }
}
