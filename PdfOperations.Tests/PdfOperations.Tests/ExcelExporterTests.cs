using ClosedXML.Excel;

namespace PdfOperations.Tests;

[TestClass]
public class ExcelExporterTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void SavesLocalizedFieldHeadersWithoutTranslatingData(bool empty)
    {
        string directory = Directory.CreateTempSubdirectory("ExcelLocalizedTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "results.xlsx");
            var labels = new ExportLabels
            {
                WorksheetName = "Wyniki",
                FileNameHeader = "Nazwa pliku",
                FilePathHeader = "Ścieżka pliku"
            };
            var result = new FileExtractionResult
            {
                FilePath = Path.Combine(directory, "document.pdf"),
                Fields = new()
                {
                    ["Company name"] = ["Żółta Łódź", "Studio ABC"],
                    ["Code"] = ["00123"],
                    ["Missing"] = []
                }
            };

            ExcelExporter.SaveFieldsToExcel(empty ? [] : [result],
                ["Company name", "Code", "Missing"], path, labels);

            using var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheet("Wyniki");
            CollectionAssert.AreEqual(
                new[] { "Nazwa pliku", "Ścieżka pliku", "Company name", "Code", "Missing" },
                sheet.Range("A1:E1").Cells().Select(cell => cell.GetString()).ToArray());
            Assert.AreEqual(empty ? 1 : 2, sheet.LastRowUsed()!.RowNumber());
            if (!empty)
            {
                Assert.AreEqual("document.pdf", sheet.Cell(2, 1).GetString());
                Assert.AreEqual(result.FilePath, sheet.Cell(2, 2).GetString());
                Assert.AreEqual("Żółta Łódź\nStudio ABC", sheet.Cell(2, 3).GetString());
                Assert.IsTrue(sheet.Cell(2, 3).Style.Alignment.WrapText);
                Assert.AreEqual("00123", sheet.Cell(2, 4).GetString());
                Assert.AreEqual(XLDataType.Text, sheet.Cell(2, 4).DataType);
                Assert.AreEqual("", sheet.Cell(2, 5).GetString());
            }
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

    [TestMethod]
    public void SavesLocalizedSingleValueHeaders()
    {
        string directory = Directory.CreateTempSubdirectory("ExcelLocalizedTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "results.xlsx");
            ExcelExporter.SaveToExcel(
                [new() { FileName = "document.pdf", DesiredValue = "00123" }], path,
                new ExportLabels
                {
                    WorksheetName = "Wyniki",
                    FileNameHeader = "Nazwa pliku",
                    ValueHeader = "Wartość"
                });

            using var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheet("Wyniki");
            Assert.AreEqual("Nazwa pliku", sheet.Cell(1, 1).GetString());
            Assert.AreEqual("Wartość", sheet.Cell(1, 2).GetString());
            Assert.AreEqual("00123", sheet.Cell(2, 2).GetString());
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

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
    
    [TestMethod]
    public void SavesMultipleFieldsInRequestedColumnOrder()
    {
        string directory = Directory.CreateTempSubdirectory("ExcelFieldsTest-").FullName;
        try
        {
            string path = Path.Combine(directory, "results.xlsx");
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
            ExcelExporter.SaveFieldsToExcel([result], ["Telefon", "NIP", "Adres"], path);

            using var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheet("Results");
            CollectionAssert.AreEqual(
                new[] { "File name", "File path", "Telefon", "NIP", "Adres" },
                sheet.Range("A1:E1").Cells().Select(cell => cell.GetString()).ToArray());
            Assert.AreEqual(2, sheet.LastRowUsed()!.RowNumber());
            Assert.AreEqual("document.pdf", sheet.Cell(2, 1).GetString());
            Assert.AreEqual(result.FilePath, sheet.Cell(2, 2).GetString());
            Assert.AreEqual("000111222\n333444555", sheet.Cell(2, 3).GetString());
            Assert.IsTrue(sheet.Cell(2, 3).Style.Alignment.WrapText);
            Assert.AreEqual("0012345678", sheet.Cell(2, 4).GetString());
            Assert.AreEqual(XLDataType.Text, sheet.Cell(2, 4).DataType);
            Assert.AreEqual("", sheet.Cell(2, 5).GetString());
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }
}
