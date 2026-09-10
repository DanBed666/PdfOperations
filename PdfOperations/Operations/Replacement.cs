using System.IO.Compression;

namespace PdfOperations;

public class Replacement
{
    public static void ReplaceTextWithPlaceholders(FileJob file, OperationInput input, OperationContext context)
    {
        string tempDir = Path.Combine(context.TempDir, Path.GetFileNameWithoutExtension(file.InputFile));
        string extenstion = Path.GetExtension(file.InputFile);
        string path = "";

        ZipFile.ExtractToDirectory(file.InputFile, tempDir);
        string [] plcLines = File.ReadAllLines(input.PlaceholderFile);
        Dictionary<string, string> placeholders = new Dictionary<string, string>();
        
        if (extenstion.Equals(".docx"))
            path = Path.Combine(tempDir, "word", "document.xml");
        else if (extenstion.Equals(".odg"))
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
        ZipFile.CreateFromDirectory(tempDir, file.TempPath + extenstion.Replace(".", ""));
    }
    
    public static void ReplacePlaceholdersWithText(FileJob file)
    {
        //string tool = ToolPaths.ToolPathsDict[Tool.Qpdf];
        List<string> arguments = new List<string>();
        
        ZipFile.ExtractToDirectory(file.InputFile, file.TempPath);
        //RunClass.Run(tool, arguments);
    }
}