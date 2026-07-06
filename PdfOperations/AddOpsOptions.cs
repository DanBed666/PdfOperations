namespace PdfOperations;

public class AddOpsOptions
{
    public static string allFiles = "Wszystkie pliki (*.*)|*.*";
    public static string pdfFiles = "Pliki PDF (*.pdf*)|*.pdf*";
    public static string pictFiles = "Pliki obrazów (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
    
    public static void ConvertOptions()
    {
        Console.WriteLine("1. Plik na PDF");
        Console.WriteLine("2. Pdf na obraz");
        Console.WriteLine("3. Sklej obrazy w pdf");
        Console.WriteLine("4. Pdf na tekst");
        Console.WriteLine("5. Obraz na tekst");
        Console.WriteLine("6. Menu główne");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine()!;

        switch (znak)
        {
            case "1":
                Console.WriteLine("Podaj nazwę pliku: ");
                string [] input9 = Files.AddFiles(allFiles);
                Console.WriteLine("Podaj katalog docelowy: ");
                string dir = Files.AddDirectory();

                if (CheckParams.checkParams(input9, dir))
                {
                    Convert.FileToPdf(input9, dir);
                }

                break;  
            
            case "2":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input = Files.AddFile(pdfFiles);
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output2 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input, output2))
                {
                    Convert.PdfToPict(input, output2);
                }

                break; 
            
            case "3":
                Console.WriteLine("Podaj nazwę obrazu: ");
                string [] input8 = Files.AddFiles(pictFiles);
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output8 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input8, output8))
                {
                    Convert.PictToPdf(input8, output8);
                }
                
                break; 
            
            case "4":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input88 = Files.AddFile(pdfFiles);
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input88, output))
                {
                    Convert.PdfToTxt(input88, output);
                }
                
                break;
            
            case "5":
                Console.WriteLine("Podaj nazwę obrazu: ");
                string input5 = Files.AddFile(pictFiles);
                Console.WriteLine("Podaj nazwę pliku wynikowego pdf: ");
                string output5 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input5, output5))
                {
                    Convert.PictToTxt(input5, output5);
                }
                
                break;
            
            case "6":
                MainMenu.MainMenuF();
                break;
            case "0":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Nic nie wybrano!");
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
                string input = Files.AddFile(pdfFiles);
                Console.WriteLine("Podaj strony (np. 3-6): ");
                string pages = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego pdf: ");
                string output = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input, pages, output))
                {
                    Pages.CreateWithPages(input, pages, output);
                }
                
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
                string[] input88 = Files.AddFiles(pdfFiles);
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input88, output))
                {
                    Divide.ManyToOne(input88, output);
                }
                
                break;
            
            case "2":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input8 = Files.AddFile(pdfFiles);
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output8 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input8, output8))
                {
                    Divide.OneToMany(input8, output8);
                }
                
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
                Info.ShowInfo(Files.AddFile(pdfFiles));
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
    
    public static void SearchOptions()
    {
        Console.WriteLine("1. Wyszukaj wyraz w obrazie");
        Console.WriteLine("2. Wyszukaj wyraz w pdf");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine();

        switch (znak)
        {
            case "1":
                Console.WriteLine("Podaj nazwę obrazu: ");
                string input = Files.AddFile(pictFiles);
                Console.WriteLine("Podaj hasło do wyszukiwania: ");
                string phrase = Console.ReadLine()!;
                Search.SearchPicture(input, phrase);
                break;
            case "2":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input2 = Files.AddFile(pdfFiles);
                Console.WriteLine("Podaj hasło do wyszukiwania: ");
                string phrase2 = Console.ReadLine()!;
                Search.SearchPdf(input2, phrase2);
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
}