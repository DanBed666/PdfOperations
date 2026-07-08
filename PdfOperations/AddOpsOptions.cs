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
                
                if (input9.Length == 0)
                    break;
                
                Console.WriteLine("Podaj format dla konwersji: ");
                string format = Console.ReadLine()!;
                string extension = "";

                if (Enum.TryParse(format, ignoreCase: true, out FileExtension ext))
                {
                    extension = ToFileFormat.ToFormatFile(ext);
                }
                else
                {
                    Console.WriteLine("Nieprawidłowy format pliku!");
                    break;
                }
                
                Console.WriteLine("Podaj katalog docelowy: ");
                string dir = Files.AddDirectory();
                
                if (CheckParams.checkParams(input9))
                {
                    try
                    {
                        Console.WriteLine("Trwa konwersja...");
                        Convert.FileToPdf(input9, extension, dir);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
                }

                break;  
            
            case "2":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input = Files.AddFile(pdfFiles);
                
                if (input.Length == 0)
                    break;
                
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output2 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input))
                {
                    try
                    {
                        Console.WriteLine("Trwa konwersja...");
                        Convert.PdfToPict(input, output2);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
                }

                break; 
            
            case "3":
                Console.WriteLine("Podaj nazwę obrazu: ");
                string [] input8 = Files.AddFiles(pictFiles);
                
                if (input8.Length == 0)
                    break;
                
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output8 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input8))
                {
                    try
                    {
                        Console.WriteLine("Trwa konwersja...");
                        Convert.PictToPdf(input8, output8);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
                }
                
                break; 
            
            case "4":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input88 = Files.AddFile(pdfFiles);
                
                if (input88.Length == 0)
                    break;
                
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input88))
                {
                    try
                    {
                        Console.WriteLine("Trwa konwersja...");
                        Convert.PdfToTxt(input88, output);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
                }
                
                break;
            
            case "5":
                Console.WriteLine("Podaj nazwę obrazu: ");
                string input5 = Files.AddFile(pictFiles);
                
                if (input5.Length == 0)
                    break;
                
                Console.WriteLine("Podaj nazwę pliku wynikowego pdf: ");
                string output5 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input5))
                {
                    try
                    {
                        Console.WriteLine("Trwa konwersja...");
                        Convert.PictToTxt(input5, output5);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
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
                
                if (input.Length == 0)
                    break;
                
                Console.WriteLine("Podaj strony (np. 3-6 lub 2-5, 7): ");
                string pages = Console.ReadLine()!;
                Console.WriteLine("Podaj nazwę pliku wynikowego pdf: ");
                string output = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input))
                {
                    try
                    {
                        Console.WriteLine("Trwa konwersja...");
                        Pages.CreateWithPages(input, pages, output);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
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
                
                if (input88.Length == 0)
                    break;
                
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input88))
                {
                    try
                    {
                        Console.WriteLine("Trwa konwersja...");
                        Divide.ManyToOne(input88, output);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
                }
                
                break;
            
            case "2":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input8 = Files.AddFile(pdfFiles);
                
                if (input8.Length == 0)
                    break;
                
                Console.WriteLine("Podaj nazwę pliku wynikowego: ");
                string output8 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input8))
                {
                    try
                    {
                        Console.WriteLine("Trwa konwersja...");
                        Divide.OneToMany(input8, output8);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
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
                
                if (input.Length == 0)
                    break;
                
                Console.WriteLine("Podaj hasło do wyszukiwania: ");
                string phrase = Console.ReadLine()!;
                
                Console.WriteLine("Podaj nazwe pliku do zapisu: ");
                string output = Console.ReadLine()!;

                if (CheckParams.checkParams(input))
                {
                    try
                    {
                        Console.WriteLine("Trwa wyszukiwanie...");
                        Search.SearchPicture(input, phrase, output);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
                }
                
                break;
            
            case "2":
                Console.WriteLine("Podaj nazwę pdf: ");
                string input2 = Files.AddFile(pdfFiles);
                
                if (input2.Length == 0)
                    break;
                
                Console.WriteLine("Podaj hasło do wyszukiwania: ");
                string phrase2 = Console.ReadLine()!;
                
                Console.WriteLine("Podaj nazwe pliku do zapisu: ");
                string output2 = Console.ReadLine()!;
                
                if (CheckParams.checkParams(input2))
                {
                    try
                    {
                        Console.WriteLine("Trwa wyszukiwanie...");
                        Search.SearchPdf(input2, phrase2, output2);
                        Console.WriteLine("Operacja zakończona pomyślnie!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Wystąpił błąd: {e.Message}");
                    }
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
}