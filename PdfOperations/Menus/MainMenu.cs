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

            Console.WriteLine(Messages.ChooseOption);
            Int32.TryParse(Console.ReadLine(), out int znak);

            OperationPaths.OperationDefinitions.TryGetValue(znak, out var value);

            try
            {
                if (znak >= 1 && znak <= 15)
                {
                    OperationInput operationInput = ExecuteCaseOperations.InputOpe(value!);

                    if (operationInput == null)
                        continue;

                    Execute.ExecuteOpe(operationInput, value!);
                }
                else if (znak >= 16 && znak <= 17)
                {
                    Execute.ExecuteRunApp(value!);
                }
                else if (znak == 18)
                    Environment.Exit(0);
                else
                    Console.WriteLine(Messages.InvalidOption);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}