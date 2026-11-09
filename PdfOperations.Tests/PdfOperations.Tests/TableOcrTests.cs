using System.Drawing;
using System.Drawing.Imaging;

namespace PdfOperations.Tests;

[TestClass]
public class TableOcrTests
{
    [TestMethod]
    public void ReadsTableCellsInOrderBetweenOrdinaryParagraphs()
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string input = Path.Combine(directory, "table.png");
            DrawTable(input);
            string output = Path.Combine(directory, "table.txt");
            Convert.PictToTxtWithTables(new FileJob { InputFile = input, TempPath = output });

            string[] lines = File.ReadAllLines(output).Where(line => line.Length > 0).ToArray();
            CollectionAssert.AreEqual(new[]
            {
                "RAPORT TESTOWY",
                "Kod\tKwota\tStatus",
                "ALFA\t120,50\tNOWY",
                "BETA\t350,00\tGOTOWE",
                "KONIEC RAPORTU"
            }, lines);
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    public void ImageWithoutTableProducesIdenticalPlainOcr()
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string input = TestHelper.SetInputPath("ocr_1.jpg");
            string ordinary = Path.Combine(directory, "ordinary.txt");
            string automatic = Path.Combine(directory, "automatic.txt");
            Convert.PictToTxt(new FileJob { InputFile = input, TempPath = ordinary });
            Convert.PictToTxtWithTables(new FileJob { InputFile = input, TempPath = automatic });
            Assert.Contains("OCR IMAGE 1", File.ReadAllText(automatic));
            CollectionAssert.AreEqual(File.ReadAllBytes(ordinary), File.ReadAllBytes(automatic));
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    public void RecoversSpacedLettersWithoutChangingSurroundingText()
    {
        string directory = Files.PrepareTempDir();
        try
        {
            string input = TestHelper.SetInputPath("ocr-spaced-letters.png");
            string ordinary = Path.Combine(directory, "ordinary.txt");
            string improved = Path.Combine(directory, "improved.txt");
            Convert.PictToTxt(new FileJob { InputFile = input, TempPath = ordinary });
            Convert.PictToTxtWithTables(new FileJob { InputFile = input, TempPath = improved });

            const string expected = "ą ć ę ł ń ó ś ź ż — Ą Ć Ę Ł Ń Ó Ś Ź Ż.";
            string[] before = File.ReadAllLines(ordinary).Where(l => l.Length > 0).ToArray();
            string[] after = File.ReadAllLines(improved).Where(l => l.Length > 0).ToArray();
            Assert.Contains(expected, after);
            string[] beforeTable = before.TakeWhile(l => l != "ZESTAWIENIE KONTROLNE").ToArray();
            string previous = beforeTable.Single(l => l.Contains('—'));
            CollectionAssert.AreEqual(beforeTable.Select(l => l == previous ? expected : l).ToArray(),
                after.TakeWhile(l => l != "ZESTAWIENIE KONTROLNE").ToArray());
            CollectionAssert.AreEqual(before.SkipWhile(l => l != "INFORMACJE DO WERYFIKACJI").ToArray(),
                after.SkipWhile(l => l != "INFORMACJE DO WERYFIKACJI").ToArray());
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    public void CancelledOcrDoesNotStartRecognition()
    {
        Assert.Throws<OperationCanceledException>(() =>
            Convert.PictToTxtWithTables(new FileJob(), new CancellationToken(true)));
    }

    private static void DrawTable(string path)
    {
        using var page = new Bitmap(1000, 570);
        using var graphics = Graphics.FromImage(page);
        using var font = new Font("Arial", 30, FontStyle.Regular, GraphicsUnit.Pixel);
        using var rule = new Pen(Color.SlateGray, 2);
        using var headerRule = new Pen(Color.White, 2);
        graphics.Clear(Color.White);
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        graphics.DrawString("RAPORT TESTOWY", font, Brushes.Black, 52, 42);
        graphics.FillRectangle(Brushes.MidnightBlue, 50, 150, 900, 70);
        int[] edges = [50, 360, 650, 950];
        foreach (int x in edges)
        {
            graphics.DrawLine(headerRule, x, 150, x, 219);
            graphics.DrawLine(rule, x, 220, x, 389);
        }
        string[][] rows = [["Kod", "Kwota", "Status"], ["ALFA", "120,50", "NOWY"], ["BETA", "350,00", "GOTOWE"]];
        int[] tops = [168, 240, 325];
        for (int r = 0; r < rows.Length; r++)
            for (int c = 0; c < rows[r].Length; c++)
                graphics.DrawString(rows[r][c], font, r == 0 ? Brushes.White : Brushes.Black, edges[c] + 18, tops[r]);
        graphics.DrawString("KONIEC RAPORTU", font, Brushes.Black, 52, 450);
        page.Save(path, ImageFormat.Png);
    }
}
