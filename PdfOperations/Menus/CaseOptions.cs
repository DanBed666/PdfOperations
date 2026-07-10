namespace PdfOperations;

public class CaseOptions
{
    public static void ExecuteSingleInSingleOut(string filter, Action<string, string> operation)
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
                    
            Files.ViewFile(input);
            string dir = Files.AddDirectory();
    
            Console.WriteLine("Podaj nazwę pliku wynikowego: ");
            string output = Console.ReadLine()!;
            
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
                    
            Files.ViewFile(input);
            string dir = Files.AddDirectory();
    
            Console.WriteLine("Podaj nazwę pliku wynikowego: ");
            string output = Console.ReadLine()!;
            
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
        
    public static void ExecuteManyInSingleOutFormat(string filter, Action<string [], string, string> operation)
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
                //Files.ViewFile(finalOutput);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd: {e.Message}");
            }
        }
}