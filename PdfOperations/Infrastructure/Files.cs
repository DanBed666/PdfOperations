namespace PdfOperations;

public class Files
{
    
    public static string [] AddFiles(string filter)
    {
        string [] files = Dialog.SelectFiles(filter);
        return files;
    }
    
    public static string AddDirectory()
    {
        Console.WriteLine("Czy chcesz dodać plik do folderu (T/N)");
        string opt = ReadInput.ReadOption();
        //ReadInput options
        
        string dir = "";
        
        if (opt.ToLower().Equals("t"))
            dir = Dialog.SelectDirectory();
        else
            Console.WriteLine("Dodano plik do folderu domyślnego");
        
        return dir;
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
    
    public static void ViewFile(string path)
    {
        Console.WriteLine("Czy chcesz zrobić podgląd pliku (T/N)");
        string opt = ReadInput.ReadOption();
        //ReadInput options
        
        if (opt.ToLower().Equals("t"))
            RunClass.RunFile(path);
    }
    
    public static void PreparePathLibre(InputClass file)
    {
        int i = 1;
        
        foreach (string f in file.inputFiles)
        {
            file.outputPath = Path.Combine(file.dir, new FileInfo(f).Name);
            Console.WriteLine(file.outputPath);

            while (File.Exists(file.outputPath))
            {
                string fileName = Path.GetFileNameWithoutExtension(file.outputPath) + $"_{i}" + Path.GetExtension(file.outputPath);
                file.outputPath = Path.Combine(file.dir, fileName);
                Console.WriteLine(file.outputPath);
                i++;
            }

            File.Move(f, file.outputPath);
            i = 1;
        }
    }
}