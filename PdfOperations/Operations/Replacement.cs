using System.IO.Compression;
using System.Text.RegularExpressions;

namespace PdfOperations;

public class Replacement
{
    public static void ReplaceTextWithPlaceholders(FileJob file, OperationInput input, OperationContext context)
    {
        string tempDir = Path.Combine(context.TempDir, Path.GetFileNameWithoutExtension(file.InputFile));
        string extension = Path.GetExtension(file.InputFile);
        string path = "";

        ZipFile.ExtractToDirectory(file.InputFile, tempDir);
        string [] plcLines = File.ReadAllLines(input.PlaceholderFile);
        Dictionary<string, string> placeholders = new Dictionary<string, string>();
        
        if (extension.Equals(".docx"))
            path = Path.Combine(tempDir, "word", "document.xml");
        else if (extension.Equals(".odg"))
            path = Path.Combine(tempDir, "content.xml");
        else
            Console.WriteLine("Nieprawidłowe rozszerzenie!");
        
        string text = File.ReadAllText(path);
        
        string key = "";
        string value = "";

        foreach (string line in plcLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            if (line.StartsWith("Find:"))
                key = line.Substring("Find: ".Length).Trim();

            if (line.StartsWith("Replace:"))
            {
                value = line.Substring("Replace: ".Length).Trim();
                if (!string.IsNullOrEmpty(key))
                    placeholders[key] = value;
            } 
        }

        foreach (KeyValuePair<string, string> placeholder in placeholders)
        {
            text = text.Replace(placeholder.Key, placeholder.Value);
        }
        
        File.WriteAllText(path, text);

        if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            UpdateDocxDates(tempDir);
        
        if (string.IsNullOrWhiteSpace(input.Output) && input.InputFiles.Length > 1)
            ZipFile.CreateFromDirectory(tempDir, file.TempPath + extension);
        else
            ZipFile.CreateFromDirectory(tempDir, file.TempPath);
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