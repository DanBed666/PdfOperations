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
    
    public static string AddDirectory()
    {
        string file = Dialog.SelectDirectory();

        if (file == "")
        {
            Console.WriteLine("Nie wybrano katalogu!");
        }

        return file;
    } 

    public static string [] ReadFile(string input)
    {
        return File.ReadAllLines(input);
    }

    public static void SaveToFile(List<String> found)
    {
        File.WriteAllLines("wyniknowy.txt", found);
    }

    public static string SaveEmptyFile(string input)
    {
        string fileName = Path.GetFileNameWithoutExtension(input);
        string output = $"{fileName}.txt";
        return output;
    }

    public static string SaveExistingFile(string output)
    {
        for (int i = 2; i <= 999999999; i++)
        {
            string outputNew = $"{Path.GetFileNameWithoutExtension(output)}-{i}.txt";

            if (!File.Exists(outputNew))
            {
                output = $"{Path.GetFileNameWithoutExtension(output)}-{i}.txt";
                break;
            }
        }

        return output;
    }
}