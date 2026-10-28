using ClosedXML.Excel;

namespace PdfOperations;

public class ExcelExporter
{
    public static void SaveToExcel(ExtractedValue[] results, string outputPath, ExportLabels? labels = null)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(labels?.WorksheetName ?? "Sheet1");
        labels ??= new ExportLabels();
        
        sheet.Cell(1, 1).Value = labels.FileNameHeader;
        sheet.Cell(1, 2).Value = labels.ValueHeader;
        
        sheet.Column(2).Style.NumberFormat.Format = "@";

        for (int i = 0; i < results.Length; i++)
        {
            sheet.Cell(1 + i + 1, 1).Value = results[i].FileName;
            sheet.Cell(1 + i + 1, 2).Value = results[i].DesiredValue;
        }
        
        sheet.Row(1).Style.Font.Bold = true;
        sheet.SheetView.FreezeRows(1);
        sheet.Columns(1, 2).AdjustToContents();
        
        workbook.SaveAs(outputPath);
    }
    
    public static void SaveFieldsToExcel(
        FileExtractionResult[] results,
        string[] fields,
        string outputPath,
        ExportLabels? labels = null)
    {
        labels ??= new ExportLabels();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(labels.WorksheetName);

        sheet.Cell(1, 1).Value = labels.FileNameHeader;
        sheet.Cell(1, 2).Value = labels.FilePathHeader;

        for (int column = 0; column < fields.Length; column++)
            sheet.Cell(1, column + 3).Value = fields[column];

        sheet.Columns(1, fields.Length + 2).Style.NumberFormat.Format = "@";

        for (int row = 0; row < results.Length; row++)
        {
            FileExtractionResult result = results[row];

            sheet.Cell(row + 2, 1).Value = result.FileName;
            sheet.Cell(row + 2, 2).Value = result.FilePath;

            for (int column = 0; column < fields.Length; column++)
            {
                string[] values = result.Fields.TryGetValue(
                    fields[column], out var found) ? found : [];

                var cell = sheet.Cell(row + 2, column + 3);
                cell.Value = string.Join("\n", values);
                cell.Style.Alignment.WrapText = true;
            }
        }

        sheet.Row(1).Style.Font.Bold = true;
        sheet.SheetView.FreezeRows(1);
        sheet.Columns(1, fields.Length + 2).AdjustToContents(10, 60);

        workbook.SaveAs(outputPath);
    }
}
