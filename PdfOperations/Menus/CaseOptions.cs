namespace PdfOperations;

public class CaseOptions
{
    public static void ExecuteSingleInSingleOut(string filter, Action<string, string> operation)
    {
        Console.WriteLine("Podaj nazwę pdf: ");
        string input = Files.AddFile(filter);

        Console.WriteLine("Wybrano plik: ");
        Console.WriteLine(Path.GetFullPath(input));
                
        Files.ViewFile(input);
        string dir = Files.AddDirectory();

        Console.WriteLine("Podaj nazwę pliku wynikowego: ");
        string output = Console.ReadLine()!;
        
        if (!CheckParams.TryPrepareParams(input, dir, output, out string finalDir, out string finalOutput))
        {
            return;
        }
        
        try
        {
            finalOutput = Path.Combine(finalDir, finalOutput);
            Console.WriteLine("Trwa konwersja...");
            operation(input, finalOutput);
            Console.WriteLine("Operacja zakończona pomyślnie!");

            Console.WriteLine("Zapisano w: ");
            Console.WriteLine(Path.GetFullPath(finalOutput));
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
        string input = Files.AddFile(filter);

        Console.WriteLine("Wybrano plik: ");
        Console.WriteLine(Path.GetFullPath(input));
                
        Files.ViewFile(input);
        string dir = Files.AddDirectory();

        Console.WriteLine("Podaj nazwę pliku wynikowego: ");
        string output = Console.ReadLine()!;
        
        if (!CheckParams.TryPrepareParams(input, dir, output, out string finalDir, out string finalOutput))
        {
            return;
        }
        
        try
        {
            finalOutput = Path.Combine(finalDir, finalOutput);
            Console.WriteLine("Trwa konwersja...");
            operation(input, finalOutput);
            Console.WriteLine("Operacja zakończona pomyślnie!");

            Console.WriteLine("Zapisano w: ");
            Console.WriteLine(Path.GetFullPath(finalOutput));
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
    
            Console.WriteLine("Wybrano plik: ");
            Console.WriteLine(Path.GetFullPath(input));
                    
            Files.ViewFile(input);
            string dir = Files.AddDirectory();
    
            Console.WriteLine("Podaj nazwę pliku wynikowego: ");
            string output = Console.ReadLine()!;
            
            if (!CheckParams.TryPrepareParams(input, dir, output, out string finalDir, out string finalOutput))
            {
                return;
            }
            
            try
            {
                finalOutput = Path.Combine(finalDir, finalOutput);
                Console.WriteLine("Trwa konwersja...");
                operation(input, finalOutput);
                Console.WriteLine("Operacja zakończona pomyślnie!");
    
                Console.WriteLine("Zapisano w: ");
                Console.WriteLine(Path.GetFullPath(finalOutput));
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
    
            Console.WriteLine("Wybrano plik: ");
            Console.WriteLine(Path.GetFullPath(input));
                    
            Files.ViewFile(input);
            string dir = Files.AddDirectory();
    
            Console.WriteLine("Podaj nazwę pliku wynikowego: ");
            string output = Console.ReadLine()!;
            
            if (!CheckParams.TryPrepareParams(input, dir, output, out string finalDir, out string finalOutput))
            {
                return;
            }
            
            try
            {
                finalOutput = Path.Combine(finalDir, finalOutput);
                Console.WriteLine("Trwa konwersja...");
                operation(input, finalOutput);
                Console.WriteLine("Operacja zakończona pomyślnie!");
    
                Console.WriteLine("Zapisano w: ");
                Console.WriteLine(Path.GetFullPath(finalOutput));
                Files.ViewFile(finalOutput);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd: {e.Message}");
            }
        }
        
    public static void ExecuteFun(string filter, Action<string, string> operation)
        {
            Console.WriteLine("Podaj nazwę pdf: ");
            string input = Files.AddFile(filter);
    
            Console.WriteLine("Wybrano plik: ");
            Console.WriteLine(Path.GetFullPath(input));
                    
            Files.ViewFile(input);
            string dir = Files.AddDirectory();
    
            Console.WriteLine("Podaj nazwę pliku wynikowego: ");
            string output = Console.ReadLine()!;
            
            if (!CheckParams.TryPrepareParams(input, dir, output, out string finalDir, out string finalOutput))
            {
                return;
            }
            
            try
            {
                finalOutput = Path.Combine(finalDir, finalOutput);
                Console.WriteLine("Trwa konwersja...");
                operation(input, finalOutput);
                Console.WriteLine("Operacja zakończona pomyślnie!");
    
                Console.WriteLine("Zapisano w: ");
                Console.WriteLine(Path.GetFullPath(finalOutput));
                Files.ViewFile(finalOutput);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd: {e.Message}");
            }
        }
}