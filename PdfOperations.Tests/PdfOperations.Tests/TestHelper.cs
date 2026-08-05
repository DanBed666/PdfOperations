namespace PdfOperations.Tests;

public class TestHelper
{
    public static string dir = Path.Combine(AppContext.BaseDirectory, "TestData");

    public static string CreateTempFolder()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }
    
    public static List<InputClass> PrepareFilesToFiles(string fileName, string format, int count, out string tempDir, string phrase = "", int before = 0, int after = 0)
    {
        tempDir = CreateTempFolder();
        List<InputClass> testInputs = new List<InputClass>();

        for (int i = 0; i < count; i++)
        {
            InputClass testInput = new InputClass();
            string file = Path.Combine(dir, fileName);
            
            testInput.inputFile = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + $"_{i + 1}" + Path.GetExtension(file));
            testInput.tempPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(testInput.inputFile) + format);

            if (!string.IsNullOrEmpty(phrase))
            {
                testInput.phrase = phrase;
                testInput.before = before;
                testInput.after = after;
            }

            testInputs.Add(testInput);
        }

        return testInputs;
    }
    
    public static InputClass PrepareFilesToSingle(string fileName, string format, int count, out string tempDir)
    {
        tempDir = CreateTempFolder();
        string [] inputs = new string[count];
        InputClass testInput = new InputClass();

        for (int i = 0; i < count; i++)
        {
            string file = Path.Combine(dir, fileName);

            file = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + $"_{i + 1}" + Path.GetExtension(file));
            inputs[i] = file;
        }
        
        testInput.inputFiles = inputs;
        testInput.tempPath = Path.Combine(tempDir, $"output{format}");

        return testInput;
    }
    
    public static InputClass PrepareFilesLibre(string fileName, string format, int count, out string tempDir)
    {
        tempDir = CreateTempFolder();
        string [] inputs = new string[count];
        InputClass testInput = new InputClass();

        for (int i = 0; i < count; i++)
        {
            string file = Path.Combine(dir, fileName);

            file = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + $"_{i + 1}" + Path.GetExtension(file));
            inputs[i] = file;
        }

        testInput.inputFiles = inputs;
        testInput.tempDir = tempDir;
        testInput.format = format.Replace(".", "");

        return testInput;
    }
    
    public static void AssertForOneFile(string file, string format)
    {
        Assert.IsTrue(File.Exists(file));
        Assert.AreEqual(format, Path.GetExtension(file));
        Assert.IsGreaterThan(0, new FileInfo(file).Length);
    }
}