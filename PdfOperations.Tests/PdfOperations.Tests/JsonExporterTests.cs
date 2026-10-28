using System.Text.Json;

namespace PdfOperations.Tests;

[TestClass]
public class JsonExporterTests
{
    [TestMethod]
    public void SavesValuesWithoutChangingText()
    {
        string directory = Directory.CreateTempSubdirectory("JsonTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "results.json");
            ExtractedValue[] expected =
            [
                new() { FileName = "number.pdf", DesiredValue = "001234567" },
                new() { FileName = "firma.txt", DesiredValue = "Żółta Łódź" },
                new() { FileName = "hours.png", DesiredValue = "08:30 - 16:00" },
                new() { FileName = "quote.pdf", DesiredValue = "Firma \"ABC\"" },
                new() { FileName = "empty.pdf", DesiredValue = "" }
            ];

            JsonExporter.SaveToJson(expected, path);

            string json = File.ReadAllText(path);
            ExtractedValue[]? actual =
                JsonSerializer.Deserialize<ExtractedValue[]>(json);

            Assert.IsNotNull(actual);
            Assert.HasCount(expected.Length, actual);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].FileName, actual[i].FileName);
                Assert.AreEqual(expected[i].DesiredValue, actual[i].DesiredValue);
            }
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    public void EmptyResultsProduceEmptyJsonArray()
    {
        string directory = Directory.CreateTempSubdirectory("JsonTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "empty.json");
            JsonExporter.SaveToJson([], path);

            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));

            Assert.AreEqual(JsonValueKind.Array, document.RootElement.ValueKind);
            Assert.AreEqual(0, document.RootElement.GetArrayLength());
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }
}