using System.CodeDom.Compiler;
using System.Text;

namespace PdfOperations;

public class Search
{
    public static void SearchPicture(OperationInput input, OperationContext context, FileJob file)
    {
        foreach (string f in Directory.GetFiles(context.TempDir))
        {
            string originalInput = Files.FindOriginalFileForTemp(f, file.InputFiles);
            Files.SaveToFile(SearchNewTxt(f, originalInput, input.PhraseToFind, input.Before, input.After), file.TempPath);
            File.Delete(f);
        }
    }

    public static void SearchPdf(OperationInput input, OperationContext context, FileJob file)
    {
        foreach (string f in Directory.GetFiles(context.TempDir))
        {
            string originalInput = Files.FindOriginalFileForTemp(f, file.InputFiles);
            Files.SaveToFile(SearchNewTxt(f, originalInput, input.PhraseToFind, input.Before, input.After), file.TempPath);
            File.Delete(f);
        }
    }
    
    public static List<List<string>> SearchNewTxt(string tempFile, string inputPath, string phrase, int before, int after)
    {
        List<List<string>> found = new();
        string[] test = Files.ReadFile(tempFile);

        for (int i = 0; i < test.Length; i++)
        {
            if (test[i].Contains(phrase.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                List<string> lines = new List<string>();
                
                lines.Add(inputPath);
                lines.Add("\n");
                
                for (int k = before; k <= after; k++)
                {
                    int idx = i + k;
                    
                    if (idx >= 0 && idx < test.Length)
                    {
                        lines.Add(test[idx]);
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