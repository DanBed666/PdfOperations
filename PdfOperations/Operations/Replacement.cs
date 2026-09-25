using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ClosedXML.Excel;

namespace PdfOperations;

public class Replacement
{
    public static void ReplaceTextWithPlaceholders(FileJob file, OperationInput input, OperationContext context)
    {
        string extractedDir = ExtractDocumentToTemp(file.InputFile, context.TempDir);
        string xmlPath = GetEditableXmlPath(extractedDir, file.InputFile);
        List<ReplacementPair> replacementPairs = ReadReplacementsFromExcel(input.PlaceholderFile);
        ReplaceTextInFile(xmlPath, replacementPairs);
        CreateDocumentFromDirectory(input, extractedDir, file.TempPath, Path.GetExtension(file.InputFile));
    }
    
    public static string ExtractDocumentToTemp(string inputFile, string tempDir)
    {
        string tempDirExt = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputFile));
        ZipFile.ExtractToDirectory(inputFile, tempDirExt);

        return tempDirExt;
    }

    public static string GetEditableXmlPath(string extractedDir, string inputFile)
    {
        string extension = Path.GetExtension(inputFile);
        
        if (extension.Equals(".docx"))
            return Path.Combine(extractedDir, "word", "document.xml");
        
        if (extension.Equals(".odg"))
            return Path.Combine(extractedDir, "content.xml");

        throw new InvalidOperationException($"Nieprawidłowe rozszerzenie {extension}");
    }

    public static List<ReplacementPair> ReadReplacementsFromExcel(string placeholderFile)
    {
        List<ReplacementPair> replacementPairs = new List<ReplacementPair>();
        
        using XLWorkbook workbook = new XLWorkbook(placeholderFile);
        IXLWorksheet worksheet = workbook.Worksheets.FirstOrDefault() ??
                                 throw new InvalidOperationException("Plik excel nie zawiera żadnego arkusza!");

        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
        {
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

        return replacementPairs;
    }

    public static void ReplaceTextInFile(string xmlPath, List<ReplacementPair> replacementPairs)
    {
        XDocument doc = XDocument.Load(xmlPath);
        XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        foreach (XElement textNode in doc.Descendants(w + "t"))
        {
            foreach (ReplacementPair pair in replacementPairs)
            {
                StringComparison stringComparison = pair.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                
                textNode.Value = textNode.Value.Replace(pair.Find, pair.Replace, stringComparison);
            }
        }
        
        doc.Save(xmlPath);
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