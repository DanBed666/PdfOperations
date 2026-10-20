using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ClosedXML.Excel;

namespace PdfOperations;

public class Replacement
{
    public static void ReplaceTextWithPlaceholders(FileJob file, OperationInput input, OperationContext context)
    {
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
        });
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
        XDocument doc = XDocument.Load(xmlPath);
        cancellationToken.ThrowIfCancellationRequested();
        XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        foreach (XElement textNode in doc.Descendants(w + "t"))
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (ReplacementPair pair in replacementPairs)
            {
                cancellationToken.ThrowIfCancellationRequested();
                StringComparison stringComparison = pair.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                
                textNode.Value = textNode.Value.Replace(pair.Find, pair.Replace, stringComparison);
            }
        }
        
        cancellationToken.ThrowIfCancellationRequested();
        doc.Save(xmlPath);
        cancellationToken.ThrowIfCancellationRequested();
    }
    
    public static void ReplaceTextInOdfXml(string xmlPath, List<ReplacementPair> replacementPairs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string text = File.ReadAllText(xmlPath);
        cancellationToken.ThrowIfCancellationRequested();

        foreach (ReplacementPair pair in replacementPairs)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (pair.IgnoreCase)
                text = text.Replace(pair.Find, pair.Replace, StringComparison.OrdinalIgnoreCase);
            
            text = text.Replace(pair.Find, pair.Replace);
        }
        
        cancellationToken.ThrowIfCancellationRequested();
        File.WriteAllText(xmlPath, text);
        cancellationToken.ThrowIfCancellationRequested();
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
