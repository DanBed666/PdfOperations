namespace PdfOperations.Tests;

public class TestHelper
{
    public static string TestDir = Path.Combine(AppContext.BaseDirectory, "TestData");

    public static string [] SetInputPaths(string [] inputs)
    {
        for (int i = 0; i <inputs.Length; i++)
        {
            inputs[i] = Path.Combine(TestDir, inputs[i]);
        }

        return inputs;
    }
    
    public static string SetInputPath(string input)
    {
        return Path.Combine(TestDir, input);
    }
}