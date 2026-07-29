namespace PdfOperations;

public class Search
{
    public static void SearchPicture(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.inputFile, Path.ChangeExtension(file.inputFile, null), "-l", "pol"]);
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(file), file.tempPath);
    }

    public static void SearchPdf(InputClass file)
    {
        string tool = ToolPaths.ToolPathsDict[Tool.PdfToText];
        List<string> arguments = new List<string>();

        arguments.AddRange([file.inputFile, file.tempPath]);
        RunClass.Run(tool, arguments);
        Files.SaveToFile(SearchNewTxt(file), file.tempPath);
    }
    
    public static List<List<string>> SearchNewTxt(InputClass file)
    {
        List<List<string>> found = new();
        string[] test = Files.ReadFile(file.tempPath);

        for (int i = 0; i < test.Length; i++)
        {
            if (test[i].Contains(file.phrase.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                List<string> lines = new List<string>();
                
                for (int k = file.before; k <= file.after; k++)
                {
                    int idx = i + k;
                    
                    if (idx >= 0 && idx < test.Length)
                    {
                        lines.Add(test[idx]);
                    }
                }
                
                lines.Add("------------------------------------");
                found.Add(lines);
            }
        }

        if (found.Count == 0)
        {
            List<string> lines = new List<string>();
            lines.Add("Nie znaleziono podanej frazy w pliku!");
            found.Add(lines);
        }
        
        return found;
    }
}