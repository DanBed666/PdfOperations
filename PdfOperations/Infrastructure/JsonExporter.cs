using System.Text.Json;

namespace PdfOperations;

public class JsonExporter
{
    public static void SaveToJson(ExtractedValue[] results, string outputPath, ExportLabels? labels = null)
    {
        labels ??= new ExportLabels();
        var records = results.Select(result => new Dictionary<string, object>
        {
            [labels.FileNameProperty] = result.FileName,
            [labels.ValueProperty] = result.DesiredValue,
            [labels.FilePathProperty] = result.FilePath
        });
        string json = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputPath, json);
    }
    
    public static void SaveFieldsToJson(
        FileExtractionResult[] results, string outputPath, ExportLabels? labels = null)
    {
        labels ??= new ExportLabels();
        var records = results.Select(result => new Dictionary<string, object>
        {
            [labels.FilePathProperty] = result.FilePath,
            [labels.FileNameProperty] = result.FileName,
            [labels.FieldsProperty] = result.Fields
        });
        string json = JsonSerializer.Serialize(
            records,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(outputPath, json);
    }
}
