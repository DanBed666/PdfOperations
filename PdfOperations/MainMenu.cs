namespace PdfOperations;

public class MainMenu
{
    public static void MainMenuF()
    {
        while (true)
        {
            Console.WriteLine("1. Konwersja");
            Console.WriteLine("2. Strony");
            Console.WriteLine("3. Rozdzielanie");
            Console.WriteLine("4. Informacje");
            Console.WriteLine("5. Wyszukiwanie");
            Console.WriteLine("0. Wyjscie");
            Console.WriteLine("Wpisz opcje: ");
            string znak = Console.ReadLine()!;
            
            switch (znak)
            {
                case "1":
                    AddOpsOptions.ConvertOptions();
                    break;    
                case "2":
                    AddOpsOptions.PagesOptions();
                    break; 
                case "3":
                    AddOpsOptions.DivideOptions();
                    break; 
                case "4":
                    AddOpsOptions.InfoOptions();
                    break; 
                case "5":
                    AddOpsOptions.SearchOptions();
                    break; 
                case "0":
                    Environment.Exit(0);
                    break; 
                default:
                    Console.WriteLine("Nie ma takiej opcji!");
                    break; 
            }
        }
    }
}