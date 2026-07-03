namespace PdfOperations.Tests;

[TestClass]
public class TestSetup
{
    [AssemblyInitialize]
    public static void ConfigureTools(TestContext testContext)
    {
        ToolPaths.ToolsDir = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "..",
            "PdfOperations", "bin", "Debug", "net10.0-windows", "tools"));
        
        Assert.IsTrue(Directory.Exists(ToolPaths.ToolsDir),  "Directory does not exist");
    }
}