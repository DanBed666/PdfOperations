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

    public static void AssertForOneFile(string file, string format)
    {
        Assert.IsTrue(File.Exists(file));
        Assert.AreEqual(format, Path.GetExtension(file));
        Assert.IsGreaterThan(0, new FileInfo(file).Length);
    }
}