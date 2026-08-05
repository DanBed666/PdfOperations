namespace PdfOperations.Tests;

public class FileTestHelper
{
    public static void CreateFiles()
    {
        string tempDir = TestHelper.CreateTempFolder();
        InputClass input = new InputClass();
        OperationDefinition ope =  new OperationDefinition();
        int length = 3;
        string[] files = new string[length];
        string fileName = "test.pdf";

        ope.Extension = ".pdf";

        for (int i = 0; i < length; i++)
        {
            files[i] = Path.Combine(TestHelper.dir, $"{Path.GetFileNameWithoutExtension(fileName)}_{i + 1}{ope.Extension}");
        }

        input.inputFiles = files;
        input.dir = tempDir;
    }
}