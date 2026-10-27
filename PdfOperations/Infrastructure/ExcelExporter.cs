using ClosedXML.Excel;

namespace PdfOperations;

public class ExcelExporter
{
    public static void SaveToExcel(ExtractedValue[] results, string outputPath)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Sheet1");
        
        sheet.Cell(1, 1).Value = "File name";
        sheet.Cell(1, 2).Value = "Value";
        
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
}