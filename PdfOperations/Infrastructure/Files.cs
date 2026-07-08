namespace PdfOperations;

public class Files
{
    public static string AddFile(string filter)
    {
        string file = Dialog.SelectFile(filter);

        if (string.IsNullOrEmpty(file))
        {
            Console.WriteLine("Nie wybrano pliku!");
        }

        return file;
    } 
    
    public static string [] AddFiles(string filter)
    {
        string [] files = Dialog.SelectFiles(filter);

        if (files.Length == 0)
        {
            Console.WriteLine("Nie wybrano pliku!");
            return [];
        }

        return files;
    }
    
    public static string AddDirectory()
    {
        string file = Dialog.SelectDirectory();

        if (string.IsNullOrEmpty(file))
        {
            Console.WriteLine("Nie wybrano katalogu!");
        }

        return file;
    } 

    public static string [] ReadFile(string input)
    {
        return File.ReadAllLines(input);
    }

    public static void SaveToFile(List<List<string>> found, string output)
    {
        List<string> outputLines = new();
        
        foreach (List<String> lista in found)
        {
            outputLines.AddRange(lista);
        }
        
        File.WriteAllLines(output, outputLines);
    }

    public static string SaveEmptyFile(string input, string ext)
    {
        string fileName = Path.GetFileNameWithoutExtension(input);
        string output = $"{fileName}{ext}";
        Console.WriteLine($"Nie podano nazwy pliku! Utworzono nowy plik {output}!");
        return output;
    }

    public static string SaveExistingFile(string output, string ext)
    {
        for (int i = 2; i <= 999999999; i++)
        {
            string outputNew = $"{Path.GetFileNameWithoutExtension(output)}-{i}{ext}";

            if (!File.Exists(outputNew))
            {
                output = $"{Path.GetFileNameWithoutExtension(output)}-{i}{ext}";
                break;
            }
        }

        Console.WriteLine($"Plik już istnieje! Utworzono nowy plik {output}!");
        return output;
    }
}