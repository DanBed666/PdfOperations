namespace PdfOperations;

public class AddOpsOptions
{
    public static void ConvertOptions()
    {
        Console.WriteLine("1. Plik na PDF");
        Console.WriteLine("2. Pdf na obraz");
        Console.WriteLine("3. Obraz na tekst");
        Console.WriteLine("4. Pdf na tekst");
        Console.WriteLine("5. Manu główne");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine()!;

        switch (znak)
        {
            case "1":
                Convert.FileToPdf();
                break;    
            case "2":
                Convert.PdfToPict();
                break; 
            case "3":
                Convert.PictToTxt();
                break; 
            case "4":
                Convert.PdfToText();
                break;
            case "0":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("nic");
                break;
        }
    }
    
    public static void PagesOptions()
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
    
    public static void DivideOptions()
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
    
    public static void InfoOptions()
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