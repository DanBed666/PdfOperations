namespace PdfOperations;

public class Files
{
    public static string AddFile(string filter)
    {
        string file = Dialog.SelectFile(filter);

        if (file == "")
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

    public static string [] ReadFile(string input)
    {
        return File.ReadAllLines(input);
    }

    public static void SaveToFile(List<String> found)
    {
        File.WriteAllLines("wyniknowy.txt", found);
    }
}