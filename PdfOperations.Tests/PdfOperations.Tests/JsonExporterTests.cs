using System.Text.Json;

namespace PdfOperations.Tests;

[TestClass]
public class JsonExporterTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void SavesLocalizedKeysWithoutTranslatingFieldsOrValues(bool empty)
    {
        string directory = Directory.CreateTempSubdirectory("JsonLocalizedTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "results.json");
            var result = new FileExtractionResult
            {
                FilePath = Path.Combine(directory, "document.pdf"),
                Fields = new()
                {
                    ["Company name"] = ["Żółta Łódź", "Studio ABC"],
                    ["FileName"] = ["00123"],
                    ["Pola"] = []
                }
            };
            JsonExporter.SaveFieldsToJson(empty ? [] : [result], path,
                new ExportLabels
                {
                    FileNameProperty = "NazwaPliku",
                    FilePathProperty = "SciezkaPliku",
                    FieldsProperty = "Pola"
                });

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            Assert.AreEqual(JsonValueKind.Array, document.RootElement.ValueKind);
            Assert.AreEqual(empty ? 0 : 1, document.RootElement.GetArrayLength());
            if (!empty)
            {
                var record = document.RootElement[0];
                Assert.AreEqual(3, record.EnumerateObject().Count());
                Assert.AreEqual("document.pdf", record.GetProperty("NazwaPliku").GetString());
                Assert.AreEqual(result.FilePath, record.GetProperty("SciezkaPliku").GetString());
                Assert.IsFalse(record.TryGetProperty("Fields", out _));
                var fields = record.GetProperty("Pola");
                Assert.AreEqual(3, fields.EnumerateObject().Count());
                CollectionAssert.AreEqual(new[] { "Żółta Łódź", "Studio ABC" },
                    fields.GetProperty("Company name").EnumerateArray().Select(v => v.GetString()).ToArray());
                Assert.AreEqual("00123", fields.GetProperty("FileName")[0].GetString());
                Assert.AreEqual(0, fields.GetProperty("Pola").GetArrayLength());
            }
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

    [TestMethod]
    public void SavesLocalizedSingleValueKeys()
    {
        string directory = Directory.CreateTempSubdirectory("JsonLocalizedTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "results.json");
            string sourcePath = Path.Combine(directory, "document.pdf");
            JsonExporter.SaveToJson(
                [new() { FileName = "document.pdf", FilePath = sourcePath, DesiredValue = "00123" }],
                path, new ExportLabels
                {
                    FileNameProperty = "NazwaPliku",
                    FilePathProperty = "SciezkaPliku",
                    ValueProperty = "Wartosc"
                });

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            var record = document.RootElement[0];
            Assert.AreEqual(3, record.EnumerateObject().Count());
            Assert.AreEqual("document.pdf", record.GetProperty("NazwaPliku").GetString());
            Assert.AreEqual(sourcePath, record.GetProperty("SciezkaPliku").GetString());
            Assert.AreEqual("00123", record.GetProperty("Wartosc").GetString());
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

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
    
    [TestMethod]
    public void SavesMultipleFieldsAsArrays()
    {
        string directory = Directory.CreateTempSubdirectory("JsonFieldsTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "results.json");
            var result = new FileExtractionResult
            {
                FilePath = Path.Combine(directory, "document.pdf"),
                Fields = new()
                {
                    ["NIP"] = ["0012345678"],
                    ["Telefon"] = ["000111222", "333444555"],
                    ["Adres"] = []
                }
            };
            JsonExporter.SaveFieldsToJson([result], path);

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            Assert.AreEqual(1, document.RootElement.GetArrayLength());
            var record = document.RootElement[0];
            Assert.AreEqual(result.FilePath, record.GetProperty("FilePath").GetString());
            Assert.AreEqual("document.pdf", record.GetProperty("FileName").GetString());
            var fields = record.GetProperty("Fields");
            CollectionAssert.AreEqual(new[] { "000111222", "333444555" },
                fields.GetProperty("Telefon").EnumerateArray().Select(v => v.GetString()).ToArray());
            Assert.AreEqual(1, fields.GetProperty("NIP").GetArrayLength());
            Assert.AreEqual("0012345678", fields.GetProperty("NIP")[0].GetString());
            Assert.AreEqual(0, fields.GetProperty("Adres").GetArrayLength());
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }
}
