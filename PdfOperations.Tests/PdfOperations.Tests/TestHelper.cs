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
    
    public static TestInput PrepareMultipleInputsFormat(string [] inputs, string extension)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationInput input = SetOperationInput(inputFiles, format: extension.Replace(".", ""));
        OperationContext context = SetOperationContext();

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
        };

        return testInput;
    }

    public static TestInput PrepareMultipleInputs(string [] inputs, string extension)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationInput input = SetOperationInput(inputFiles);
        OperationContext context = SetOperationContext();
        OperationDefinition operation = SetOperationDefinition(extension);

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
            Operation = operation
        };

        return testInput;
    }
    
    public static TestInput PrepareInputWithOutputFormat(string [] inputs, string extension, string output)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationDefinition operation = SetOperationDefinition(extension);
        OperationInput input = SetOperationInput(inputFiles, output: output);
        OperationContext context = SetOperationContext();

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
            Operation = operation
        };

        return testInput;
    }
    
    public static TestInput PrepareInputWithOutput(string [] inputs, string output)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationInput input = SetOperationInput(inputFiles, output: output);
        OperationContext context = SetOperationContext();

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
        };

        return testInput;
    }
    
    public static TestInput PrepareMultipleInputsWithDir(string [] inputs, string extension, string dir)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationDefinition operation = SetOperationDefinition(extension);
        OperationInput input = SetOperationInput(inputFiles, dir: dir);
        OperationContext context = SetOperationContext();

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
            Operation = operation
        };

        return testInput;
    }
    
    public static TestInput PrepareMultipleInputsWithDirOutput(string [] inputs, string output, string dir)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationInput input = SetOperationInput(inputFiles, output: output, dir: dir);
        OperationContext context = SetOperationContext();

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
        };

        return testInput;
    }
    
    public static TestInput PrepareMultipleInputsLibre(string [] inputs, string extension, string dir)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationInput input = SetOperationInput(inputFiles, dir: dir, format: extension.Replace(".", ""));
        OperationContext context = SetOperationContext();

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
        };

        return testInput;
    }
    
    public static TestInput PrepareMultipleInputsSearch(string [] inputs, string phrase, string extension, int before, int after)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationDefinition operation = SetOperationDefinition(extension);
        OperationInput input = SetOperationInput(inputFiles, phrase: phrase, before: before, after: after);
        OperationContext context = SetOperationContext();

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
            Operation = operation
        };

        return testInput;
    }

    public static TestInput PrepareMultiplePathsWithoutContext(string [] inputs, string output, string extension)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationInput input = SetOperationInput(inputFiles, output: output);
        OperationDefinition operation = SetOperationDefinition(extension: extension);
        
        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Operation = operation
        };

        return testInput;
    }
    
    public static TestInput PrepareMultipleInputsPages(string [] inputs, string extension, string pages)
    {
        string[] inputFiles = SetInputPaths(inputs);
        OperationDefinition operation = SetOperationDefinition(extension);
        OperationInput input = SetOperationInput(inputFiles, pages: pages);
        OperationContext context = SetOperationContext();

        TestInput testInput = new TestInput()
        {
            InputFiles = inputFiles,
            Input = input,
            Context = context,
            Operation = operation,
        };

        return testInput;
    }
}