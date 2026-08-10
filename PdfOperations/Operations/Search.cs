using System.CodeDom.Compiler;
using System.Text;

namespace PdfOperations;

public class Search
{
    public static void SearchPicture(OperationInput input, OperationContext context, FileJob file)
    {
        foreach (string f in Directory.GetFiles(context.TempDir))
        {
            Files8.SaveToFile(SearchNewTxt(f, input), file.FinalPath);
        }
    }

    public static void SearchPdf(OperationInput input, OperationContext context, FileJob file)
    {
        foreach (string f in Directory.GetFiles(context.TempDir))
        {
            Files8.SaveToFile(SearchNewTxt(f, input), file.FinalPath);
        }
    }
    
    public static List<List<string>> SearchNewTxt(string f, OperationInput fileInput)
    {
        List<List<string>> found = new();
        string[] test = Files8.ReadFile(f);

        for (int i = 0; i < test.Length; i++)
        {
            if (test[i].Contains(fileInput.PhraseToFind.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                List<string> lines = new List<string>();
                
                lines.Add(f);
                lines.Add("\n");
                
                for (int k = fileInput.Before; k <= fileInput.After; k++)
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
            lines.Add(f);
            lines.Add("\n");
            lines.Add("Nie znaleziono podanej frazy w pliku!");
            lines.Add("------------------------------------");
            lines.Add("\n");
            found.Add(lines);
        }
        
        return found;
    }
    
    public static void SaveToFile(string file, string output, string f)
    {
        if (!File.Exists(file))
        {
            File.WriteAllText(file, f, new UTF8Encoding(true));
            File.AppendAllText(file, "\n\n");
            File.AppendAllText(file, output);
            File.AppendAllText(file, "-------------------------");
            File.AppendAllText(file, "\n\n");
        }
        else
        {
            File.AppendAllText(file, f, new UTF8Encoding(true));
            File.AppendAllText(file, "\n\n");
            File.AppendAllText(file, output);
            File.AppendAllText(file, "-------------------------");
            File.AppendAllText(file, "\n\n");
        }

        Console.WriteLine(File.Exists(file));
    }
}