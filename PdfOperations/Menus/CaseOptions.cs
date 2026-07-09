namespace PdfOperations;

public class CaseOptions
{
    public static string [] GetData(string filter)
    {
        string [] data = new string [3];
        
        Console.WriteLine("Podaj nazwę pdf: ");
        string input = Files.AddFile(filter);
        data[0] = input;

        Console.WriteLine("Wybrano plik: ");
        Console.WriteLine(Path.GetFullPath(input));
                
        Files.ViewFile(input);
        string dir = Files.AddDirectory();
        data[1] = dir;

        Console.WriteLine("Podaj nazwę pliku wynikowego: ");
        string output = Console.ReadLine()!;
        data[2] = output;

        return data;
    }
    public static void Execute(string [] data, Action<string, string> operation)
    {
        string input = data[0];
        string dir = data[1];
        string output = data[2];
        
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