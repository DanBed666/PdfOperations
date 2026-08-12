namespace PdfOperations.Tests;

public class TestHelper8
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
    
    public static OperationDefinition SetOperationDefinition(string extension)
    {
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = extension
        };

        return operationDefinition;
    }
    
    public static OperationInput SetOperationInput(string [] inputFiles, string output = "", string pages = "")
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = inputFiles,
            Output = output,
            Pages = pages
        };

        return operationInput;
    }

    public static OperationContext SetOperationContext()
    {
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))
        };
        
        return operationContext;
    }

    public static FileJob SetFileJob()
    {
        FileJob fileJob = new FileJob()
        {

        };
        
        return fileJob;
    }
    
    public static void AssertForOneFile(string file, string format)
    {
        Assert.IsTrue(File.Exists(file));
        Assert.AreEqual(format, Path.GetExtension(file));
        Assert.IsGreaterThan(0, new FileInfo(file).Length);
    }
}