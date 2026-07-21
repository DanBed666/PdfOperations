namespace PdfOperations;

public class Files
{
    public static string AddFile(string filter)
    {
        string file = Dialog.SelectFile(filter);
        return file;
    } 
    
    public static bool CheckFileOrDir(string file)
    {
        if (string.IsNullOrEmpty(file))
        {
            Console.WriteLine("Brak plików!");
            return false;
        }
        
        return true;
    }
    
    public static string [] AddFiles(string filter)
    {
        string [] files = Dialog.SelectFiles(filter);
        return files;
    }
    
    public static bool CheckFiles(string [] files)
    {
        if (files.Length == 0)
        {
            Console.WriteLine("Brak plików!");
            return false;
        }
        
        return true;
    }
    
    public static string AddDirectory()
    {
        Console.WriteLine("Czy chcesz dodać plik do folderu (T/N)");
        string opt = Console.ReadLine()!;
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

    public static string GetAvailableFileName(string output)
    {
        string? dir = Path.GetDirectoryName(output);
        string name = Path.GetFileNameWithoutExtension(output);
        string ext = Path.GetExtension(output);
        int i = 2;
        
        while (File.Exists(output))
        {
            output = Path.Combine(dir ?? "", $"{name}-{i}{ext}");
            i++;
        }

        Console.WriteLine($"Plik już istnieje! Utworzono nowy plik {output}!");
        return output;
    }
    
    public static void ViewFile(string path)
    {
        Console.WriteLine("Czy chcesz zrobić podgląd pliku (T/N)");
        string opt = Console.ReadLine()!;
        //ReadInput options
        
        if (opt.ToLower().Equals("t"))
            RunClass.RunFile(path);
    }

    public static bool ViewFileChoosen(string path)
    {
        bool choosen = false;
        Console.WriteLine("Czy chcesz zrobić podgląd pliku (T/N)");
        string opt = Console.ReadLine()!;
        //ReadInput options
        
        if (opt.ToLower().Equals("t"))
        {
            RunClass.RunFile(path);
            choosen = ChoosenFile("Czy to właściwy plik? (T / N)");
        }
        else
        {
            choosen = ChoosenFile("KOntynuować z tym plikiem? (T / N)");
        }
        
        return choosen;
    }

    public static bool ChoosenFile(string message)
    {
        Console.WriteLine(message);
        string opt = Console.ReadLine()!;
        //ReadInput options

        if (opt.ToLower().Equals("t"))
        {
            return true;
        }

        return false;
    }

    public static void SaveInFolder(string file)
    {
        string folder = Path.Combine(AppContext.BaseDirectory, "output");
        
        Console.WriteLine("Czy chesz zapisać w folderze (T/N)");
        string opt = Console.ReadLine()!;
        //ReadInput options but not used

        if (opt.ToLower().Equals("t"))
            folder = AddDirectory();

        string path = Path.Combine(folder, file);
        Directory.CreateDirectory(path);
    }

    public static string GetDefaultDirectory()
    {
        string dirDefault = Path.Combine(AppContext.BaseDirectory, "output");
        
        if (!Directory.Exists(dirDefault))
        {
            Directory.CreateDirectory(dirDefault);
        }

        return dirDefault;
    }
    
    public static string GetDefaultOutputFile(string input)
    {
        string fileName = Path.GetFileNameWithoutExtension(input);
        string ext = Path.GetExtension(input);
        string output = $"{fileName}{ext}";
        Console.WriteLine($"Nie podano nazwy pliku! Utworzono nowy plik {output}!");
        return output;
    }

    public static void MultipleConv(string [] files, string outputDir, string outputFile, 
        string ext, Action<string, string> operation)
    {
        int i = 0;

        if (files.Length >= 2)
        {
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string newOutput = Path.Combine(outputDir, $"{i}_{name}{ext}");
                operation(file, newOutput);
                i++;
            }
        }
        else
        {
            string newOutput = Path.Combine(outputDir, outputFile);
            operation(files[0], newOutput);
        }
    }
}