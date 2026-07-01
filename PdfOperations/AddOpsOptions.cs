namespace PdfOperations;

public class AddOpsOptions
{
    public static void ConvertOptions()
    {
        Console.WriteLine("1. Plik na PDF");
        Console.WriteLine("2. Pdf na obraz");
        Console.WriteLine("3. Obraz na tekst");
        Console.WriteLine("4. Pdf na tekst");
        Console.WriteLine("5. Sklej obrazy w pdf");
        Console.WriteLine("6. Menu główne");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine()!;

        switch (znak)
        {
            case "1":
                Convert.FileToPdf();
                break;    
            case "2":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input2 = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output2 = Console.ReadLine()!;
                Convert.PdfToPict(input2, output2);
                break; 
            case "3":
                Console.WriteLine("Podaj nazwę obrazu: ");
                string input8 = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output8 = Console.ReadLine()!;
                Convert.PictToTxt(input8, output8);
                break; 
            case "4":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output = Console.ReadLine()!;
                Convert.PdfToTxt(input, output);
                break;
            case "5":
                Console.WriteLine("Podaj nazwę obrazu: ");
                string input5 = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego pdf: ");
                string output5 = Console.ReadLine()!;
                Convert.PictToPdf(input5, output5);
                break;
            case "6":
                MainMenu.MainMenuF();
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
        Console.WriteLine("1. Utworz pdf z wybranymi stronami");
        Console.WriteLine("2. Menu główne");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine();

        switch (znak)
        {
            case "1":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input = Console.ReadLine()!;
                Console.WriteLine("Podaj strony: ");
                string pages = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego pdf: ");
                string output = Console.ReadLine()!;
                Pages.CreateWithPages(input, pages, output);
                break;
            case "2":
                MainMenu.MainMenuF();
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
        Console.WriteLine("1. Zlaczenie pdfów w jeden");
        Console.WriteLine("2. Rozdzielenie pdfów");
        Console.WriteLine("3. Menu główne");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine();

        switch (znak)
        {
            case "1":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output = Console.ReadLine()!;
                Divide.ManyToOne(input, output);
                break;
            case "2":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input8 = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output8 = Console.ReadLine()!;
                Divide.OneToMany(input8, output8);
                break;
            case "3":
                MainMenu.MainMenuF();
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
        Console.WriteLine("1. Wyswietl info o pdf");
        Console.WriteLine("2. Menu główne");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine();

        switch (znak)
        {
            case "1":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input = Console.ReadLine()!;
                Info.ShowInfo(input);
                break;
            case "2":
                MainMenu.MainMenuF();
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