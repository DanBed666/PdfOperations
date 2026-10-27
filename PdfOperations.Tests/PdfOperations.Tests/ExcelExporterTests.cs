using ClosedXML.Excel;

namespace PdfOperations.Tests;

[TestClass]
public class ExcelExporterTests
{
    [TestMethod]
    public void SavesHeadersAndValuesAsText()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsExcelTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "results.xlsx");
            ExcelExporter.SaveToExcel(
            [
                new() { FileName = "number.pdf", DesiredValue = "001234567" },
                new() { FileName = "company.txt", DesiredValue = "Studio Graphic ABC" },
                new() { FileName = "hours.png", DesiredValue = "08:30 - 16:00" },
                new() { FileName = "literal.pdf", DesiredValue = "=1+1" },
                new() { FileName = "empty.pdf", DesiredValue = "" }
            ], path);

            using var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheet(1);
            Assert.AreEqual("File name", sheet.Cell(1, 1).GetString());
            Assert.AreEqual("Value", sheet.Cell(1, 2).GetString());
            Assert.AreEqual(6, sheet.LastRowUsed()!.RowNumber());
            Assert.AreEqual("number.pdf", sheet.Cell(2, 1).GetString());
            Assert.AreEqual("001234567", sheet.Cell(2, 2).GetString());
            Assert.AreEqual(XLDataType.Text, sheet.Cell(2, 2).DataType);
            Assert.AreEqual("Studio Graphic ABC", sheet.Cell(3, 2).GetString());
            Assert.AreEqual("08:30 - 16:00", sheet.Cell(4, 2).GetString());
            Assert.AreEqual("=1+1", sheet.Cell(5, 2).GetString());
            Assert.IsFalse(sheet.Cell(5, 2).HasFormula);
            Assert.AreEqual("", sheet.Cell(6, 2).GetString());
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    public void EmptyResultsProduceWorkbookWithHeaders()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsExcelTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "empty.xlsx");
            ExcelExporter.SaveToExcel([], path);

            using var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheet(1);
            Assert.AreEqual(1, sheet.LastRowUsed()!.RowNumber());
            Assert.AreEqual("File name", sheet.Cell(1, 1).GetString());
            Assert.AreEqual("Value", sheet.Cell(1, 2).GetString());
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }
}
