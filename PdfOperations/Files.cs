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
}