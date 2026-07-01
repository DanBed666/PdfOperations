namespace PdfOperations;

class Program
{
    static void MainMenu()
    {
        while (true)
        {
            Console.WriteLine("1. Plik na Pdf");
            Console.WriteLine("2. Pdf na obraz");
            Console.WriteLine("3. Obraz na tekst");
            Console.WriteLine("4. Pdf na text");
            Console.WriteLine("5. Menu glowne");
            Console.WriteLine("0. Wyjscie");
            Console.WriteLine("Wpisz opcje: ");
            string znak = Console.ReadLine();
            
            switch (znak)
            {
                case "1":
                    OperationsMenu.FileToPdf();
                    break;    
                case "2":
                    OperationsMenu.PdfToPict();
                    break; 
                case "3":
                    OperationsMenu.PictToTxt();
                    break; 
                case "4":
                    OperationsMenu.PdfToText();
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
    
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        MainMenu();
    }
}