using System.CodeDom.Compiler;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;

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
    
    public static OperationInput SetOperationInput(string [] inputFiles, string output = "", string pages = "", string dir = "", 
        string format = "", string phrase = "", int before = 0, int after = 0)
    {
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = inputFiles,
            Output = output,
            Pages = pages,
            Dir = dir,
            Format = format,
            PhraseToFind = phrase,
            Before = before,
            After = after
        };

        return operationInput;
    }

    public static OperationContext SetOperationContext()
    {
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        return operationContext;
    }

    public static FileJob SetFileJob(string temp, string final)
    {
        FileJob fileJob = new FileJob()
        {
            TempPath = temp,
            FinalPath = final
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