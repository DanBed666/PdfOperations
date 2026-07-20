namespace PdfOperations;

public class CaseOptions
{
    public static void ExecuteSingleInSingleOut(string filter, Action<string, string> operation)
    {
        bool choosenFile = false;
        string input = "";

        while (!choosenFile)
        {
            Console.WriteLine("Podaj nazwę pdf: ");
            input = Files.AddFile(filter);

            if (!Files.CheckFileOrDir(input))
            {
                return;
            }

            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(input)}");

            choosenFile = Files.ViewFileChoosen(input);
        }

        string dir = Files.AddDirectory();

        Console.WriteLine("Podaj nazwę pliku wynikowego: ");
        string output = Console.ReadLine()!;
        //ReadInput output
        
        if (!CheckParams.TryPrepareOutputParams(input, dir, output, out string finalDir, out string finalOutput))
        {
            return;
        }
        
        try
        {
            Console.WriteLine("Trwa konwersja...");
            operation(input, finalOutput);
            Console.WriteLine("Operacja zakończona pomyślnie!");

            Console.WriteLine($"Zapisano w: {Path.GetFullPath(finalOutput)}");
            Files.ViewFile(finalOutput);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
        }
    }
    
    public static void ExecuteSingleInManyOut(string filter, Action<string, string> operation)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string input = Files.AddFile(filter);

        if (!Files.CheckFileOrDir(input))
        {
            return;
        }

        Console.WriteLine($"Wybrano plik: {Path.GetFullPath(input)}");
                
        Files.ViewFile(input);
        string dir = Files.AddDirectory();

        Console.WriteLine("Podaj nazwę pliku wynikowego: ");
        string output = Console.ReadLine()!;
        //ReadInput output
        
        if (!CheckParams.TryPrepareOutputParamsMultiple(input, dir, output, out string finalDir, out string finalOutput))
        {
            return;
        }
        
        try
        {
            Console.WriteLine("Trwa konwersja...");
            operation(input, finalOutput);
            Console.WriteLine("Operacja zakończona pomyślnie!");

            Console.WriteLine($"Zapisano w: {Path.GetFullPath(finalOutput)}");
            Files.ViewFile(finalOutput);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
        }
    }
    
    public static void ExecuteManyInSingleOut(string filter, Action<string [], string> operation)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(filter);
        
        if (!Files.CheckFiles(input))
        {
            return;
        }

        foreach (string file in input)
        {
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
        }

        if (input.Length == 1)
            Files.ViewFile(input[0]);
        
        string dir = Files.AddDirectory();

        Console.WriteLine("Podaj nazwę pliku wynikowego: ");
        string output = Console.ReadLine()!;
        //ReadInput output
        
        if (!CheckParams.TryPrepareOutputParams(input, dir, output, out string finalDir, out string finalOutput))
        {
            return;
        }
        
        try
        {
            Console.WriteLine("Trwa konwersja...");
            operation(input, finalOutput);
            Console.WriteLine("Operacja zakończona pomyślnie!");

            Console.WriteLine($"Zapisano w: {Path.GetFullPath(finalOutput)}");
            Files.ViewFile(finalOutput);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
        }
    }
    
    public static void ExecuteManyInManyOut(string filter, Action<string [], string> operation)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(filter);
        
        if (!Files.CheckFiles(input))
        {
            return;
        }

        foreach (string file in input)
        {
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
        }

        if (input.Length == 1)
            Files.ViewFile(input[0]);
        
        string dir = Files.AddDirectory();

        if (!CheckParams.TryPrepareOutputOnlyDirLight(input, dir, out string finalDir))
        {
            return;
        }
        
        try
        {
            Console.WriteLine("Trwa konwersja...");
            operation(input, finalDir);
            Console.WriteLine("Operacja zakończona pomyślnie!");

            Console.WriteLine($"Zapisano w: {Path.GetFullPath(finalDir)}");
            Files.ViewFile(finalDir);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
        }
    }
    
    public static void ExecuteSingleInSingleOutPages(string filter, Action<string, string, string> operation)
        {
            Console.WriteLine("Podaj nazwę pdf: ");
            string input = Files.AddFile(filter);
            
            if (!Files.CheckFileOrDir(input))
            {
                return;
            }
    
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(input)}");
            
            Console.WriteLine("Podaj strony: ");
            string pages = Console.ReadLine()!;
            //ReadInput pages
                    
            Files.ViewFile(input);
            string dir = Files.AddDirectory();
    
            Console.WriteLine("Podaj nazwę pliku wynikowego: ");
            string output = Console.ReadLine()!;
            //ReadInput output
            
            if (!CheckParams.TryPrepareOutputParamsArg(input, pages, "Brak stron!", dir,  
                    output, out string finalDir, out string finalOutput))
            {
                return;
            }
            
            try
            {
                Console.WriteLine("Trwa konwersja...");
                operation(input, pages, finalOutput);
                Console.WriteLine("Operacja zakończona pomyślnie!");
    
                Console.WriteLine($"Zapisano w: {Path.GetFullPath(finalOutput)}");
                Files.ViewFile(finalOutput);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd: {e.Message}");
            }
        }
        
    public static void ExecuteSingleInSingleOutSearch(string filter, Action<string, string, string> operation)
        {
            Console.WriteLine("Podaj nazwę pdf: ");
            string input = Files.AddFile(filter);
            
            if (!Files.CheckFileOrDir(input))
            {
                return;
            }
    
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(input)}");
            
            Console.WriteLine("Podaj frazę: ");
            string phrase = Console.ReadLine()!;
            //ReadInput phrase
                    
            Files.ViewFile(input);
            string dir = Files.AddDirectory();
    
            Console.WriteLine("Podaj nazwę pliku wynikowego: ");
            string output = Console.ReadLine()!;
            //ReadInput output
            
            if (!CheckParams.TryPrepareOutputParamsArg(input, phrase, "Brak frazy!", dir,  
                    output, out string finalDir, out string finalOutput))
            {
                return;
            }
            
            try
            {
                Console.WriteLine("Trwa konwersja...");
                operation(input, phrase, finalOutput);
                Console.WriteLine("Operacja zakończona pomyślnie!");
    
                Console.WriteLine($"Zapisano w: {Path.GetFullPath(finalOutput)}");
                Files.ViewFile(finalOutput);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd: {e.Message}");
            }
        }
    
    public static void ExecuteManyInSingleOutSearch(string filter, Action<string[], string, string> operation)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(filter);

        foreach (string file in input)
        {
            Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
        }

        Console.WriteLine("Podaj frazę: ");
        string phrase = Console.ReadLine()!;
        //ReadInput phrase
                    
        //Files.ViewFile(input);
        string dir = Files.AddDirectory();
    
        //Console.WriteLine("Podaj nazwę pliku wynikowego: ");
        //string output = Console.ReadLine()!;
        //ReadInput output
            
        if (!CheckParams.TryPrepareOutputSearch(input, phrase, "Brak frazy!", dir, out string finalDir))
        {
            return;
        }
            
        try
        {
            Console.WriteLine("Trwa konwersja...");
            operation(input, phrase, finalDir);
            Console.WriteLine("Operacja zakończona pomyślnie!");
    
            Console.WriteLine($"Zapisano w: {Path.GetFullPath(finalDir)}");
            Files.ViewFile(finalDir);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Wystąpił błąd: {e.Message}");
        }
    }
        
    public static void ExecuteManyInDirOutFormat(string filter, Action<string [], string, string> operation)
        {
            Console.WriteLine("Podaj nazwę pdf: ");
            string [] input = Files.AddFiles(filter);
            
            if (!Files.CheckFiles(input))
            {
                return;
            }

            foreach (string file in input)
            {
                Console.WriteLine($"Wybrano plik: {Path.GetFullPath(file)}");
            }

            Console.WriteLine("Podaj format: ");
            string format = Console.ReadLine()!;
            //ReadInput format
                    
            if (input.Length == 1)
                Files.ViewFile(input[0]);
            
            string dir = Files.AddDirectory();
            Console.WriteLine(dir);
            
            if (!CheckParams.TryPrepareOutputOnlyDir(input, format, "Brak formatu!", dir, out string finalDir))
            {
                return;
            }
            
            try
            {
                Console.WriteLine("Trwa konwersja...");
                operation(input, format, finalDir);
                Console.WriteLine("Operacja zakończona pomyślnie!");
    
                Console.WriteLine($"Zapisano w: {Path.GetFullPath(finalDir)}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd: {e.Message}");
            }
        }
    
    public static void ExecuteManyRun(string filter)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(filter);
        
        RunClass.RunFiles(input);
    }
    
    public static void ExecuteManyRunApp(string filter)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string [] input = Files.AddFiles(filter);

        string app = Console.ReadLine();
        string appConv = "";

        if (app.Equals("w"))
        {
            appConv = "winword.exe";
            RunClass.RunFiles(input, appConv);
        }
        else if (app.Equals("d"))
        {
            string dir = AppContext.BaseDirectory;

            string soffice = Path.Combine(
                dir,
                "tools",
                "libreoffice",
                "program",
                "soffice.exe"
            );

            appConv = soffice;
            RunClass.RunFilesDraw(input, appConv);
        }
    }
}