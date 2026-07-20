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
            {
                // File to pdf
                CaseOptions.ExecuteManyInDirOutFormat(allFiles, Convert.FileToPdf);
                break;
            }
            case "2":
            {
                // Pdf to picture (1 file)
                //CaseOptions.ExecuteSingleInManyOut(pdfFiles, Convert.PdfToPict);
                //break;
                
                // Multiple pdfs to picture
                CaseOptions.ExecuteManyInManyOut(pdfFiles, Convert.PdfsToPict);
                break;
            }
            case "3":
            {
                // Picture to pdf
                CaseOptions.ExecuteManyInSingleOut(pictFiles, Convert.PictToPdf);
                break;
            }
            case "4":
            {
                // Pdf to txt
                //CaseOptions.ExecuteSingleInSingleOut(pdfFiles, Convert.PdfToTxt);
                CaseOptions.ExecuteManyInManyOut(pdfFiles, Convert.PdfsToTxt);
                break;
            }
            case "5":
            {
                // Picture to txt
                //CaseOptions.ExecuteSingleInSingleOut(pictFiles, Convert.PictToTxt);
                CaseOptions.ExecuteManyInManyOut(pictFiles, Convert.PictsToTxt);
                break;
            }
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
            {
                // Create pdf with selected pages
                CaseOptions.ExecuteSingleInSingleOutPages(pdfFiles, Pages.CreateWithPages);
                break;
            }
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
            {
                // Unite many to one
                CaseOptions.ExecuteManyInSingleOut(pdfFiles, Divide.ManyToOne);
                break;
            }
            case "2":
            {
                // Separate one to many
                CaseOptions.ExecuteSingleInManyOut(pdfFiles, Divide.OneToMany);
                break;
            }
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
                //Show info
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
            {
                // Search phrase in picture
                CaseOptions.ExecuteSingleInSingleOutSearch(pictFiles, Search.SearchPicture);
                break;
            }
            case "2":
            {
                // Search phrase in pdf
                CaseOptions.ExecuteSingleInSingleOutSearch(pdfFiles, Search.SearchPdf);
                break;
            }
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
    
    public static void OpenOptions()
    {
        Console.WriteLine("1. Otwieranie wi3lu plików");
        Console.WriteLine("2. Otwieranie wilu plików z wybraną aplikacją");
        Console.WriteLine("0. Wyjscie");
        Console.WriteLine("Wpisz opcje: ");
        string znak = Console.ReadLine();

        switch (znak)
        {
            case "1":
            {
                // Open multiple files
                CaseOptions.ExecuteManyRun(allFiles);
                break;
            }
            case "2":
            {
                // Open multiple files
                CaseOptions.ExecuteManyRunApp(pdfFiles);
                break;
            }
            case "0":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("nic");
                break;
        }
    }
}