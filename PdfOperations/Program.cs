namespace PdfOperations;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

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
                    Console.WriteLine(1);
                    break;    
                case "2":
                    Console.WriteLine(2);
                    break; 
                case "3":
                    Console.WriteLine(3);
                    break; 
                case "4":
                    Console.WriteLine(4);
                    break; 
                case "5":
                    Console.WriteLine(5);
                    break; 
                case "0":
                    Console.WriteLine(0);
                    break; 
                default:
                    Console.WriteLine("default");
                    break; 
            }
        }
    }
}