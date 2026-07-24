namespace PdfOperations;

public class MainMenu
{
    public static void MainMenuF()
    {
        while (true)
        {
            for (int i = 1; i <= OperationPaths.OperationDefinitions.Count; i++)
            {
                Console.WriteLine($"[{i}] {OperationPaths.OperationDefinitions[i].Name}");
            }
            
            Console.WriteLine("Wpisz opcje: ");
            Int32.TryParse(Console.ReadLine(), out int znak);

            OperationPaths.OperationDefinitions.TryGetValue(znak, out var value);

            if (znak >= 1 && znak <= 13)
                ExecuteCaseOperations.InputOpe(value!);
            else if (znak >= 14 && znak <= 15)
                value!.RunOperationAction(value);
            else if (znak == 16)
                Environment.Exit(0);
            else
                Console.WriteLine("Niepoprawna opcja!");
        }
    }
}