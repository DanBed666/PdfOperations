using System.Text.Json;

namespace PdfOperations;

public class JsonExporter
{
    public static void SaveToJson(ExtractedValue[] results, string outputPath)
    {
        string json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputPath, json);
    }
}