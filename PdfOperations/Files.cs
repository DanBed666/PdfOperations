namespace PdfOperations;

public class Files
{
    public static string AddFile()
    {
        string file = Dialog.SelectFile();

        if (file == "")
        {
            Console.WriteLine("Nie wybrano pliku!");
        }

        return file;
    } 
    
    public static string [] AddFiles()
    {
        string [] files = Dialog.SelectFiles();

        if (files.Length == 0)
        {
            Console.WriteLine("Nie wybrano pliku!");
            return [];
        }

        return files;
    }
}