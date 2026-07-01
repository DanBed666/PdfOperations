namespace PdfOperations;

public class AdditionalOptions
{
    public static void AdditionalMenu()
    {
        Console.WriteLine("1. Konwersja");
        Console.WriteLine("2. Strony");
        Console.WriteLine("3. Rozdzielanie");
        Console.WriteLine("4. Informacje");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine();

        switch (znak)
        {
            case "1":
                //Convert();
                break;
            case "2":
                //Pages();
                break;
            case "3":
                //Divide();
                break;
            case "4":
                //Info();
                break;
            case "0":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("nic");
                break;
        }
    }
}