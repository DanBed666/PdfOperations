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

            int? znak = UserInput.ReadRequiredInt(Messages.ChooseOption);

            if (znak is null)
            {
                Console.WriteLine(Messages.InvalidOption);
                continue;
            }

            if (!OperationPaths.OperationDefinitions.TryGetValue(znak.Value, out var value))
            {
                Console.WriteLine(Messages.InvalidOption);
                continue;
            }

            try
            {
                if (znak >= 1 && znak <= 15)
                {
                    OperationInput? operationInput = ExecuteCaseOperations8.InputOpe(value);

                    if (operationInput is null)
                        continue;

                    Execute8.ExecuteOpe(operationInput, value);
                }
                else if (znak >= 16 && znak <= 17)
                {
                    Execute8.ExecuteRunApp(value);
                }
                else if (znak == 18)
                {
                    Help.ShowHelp();
                }
                else if (znak == 19)
                {
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine(Messages.InvalidOption);
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd: {e.Message}");
            }
        }
    }
}