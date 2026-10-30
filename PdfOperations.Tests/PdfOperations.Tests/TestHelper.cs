using System.IO.Compression;
using System.Xml.Linq;

namespace PdfOperations.Tests;

public class TestHelper
{
    public static string TestDir = Path.Combine(AppContext.BaseDirectory, "TestData");

    public static string [] SetInputPaths(string [] inputs)
    {
        for (int i = 0; i <inputs.Length; i++)
        {
            inputs[i] = Path.Combine(TestDir, inputs[i]);
        }

        return inputs;
    }
    
    public static string SetInputPath(string input)
    {
        return Path.Combine(TestDir, input);
    }
    
    public static string ReadOutputXml(string tempDir, string extension)
    {
        string outputFile = Directory.GetFiles(tempDir, "*" + extension).Single();

        using ZipArchive archive = ZipFile.OpenRead(outputFile);

        string entryName = extension == ".docx"
            ? "word/document.xml"
            : "content.xml";

        ZipArchiveEntry? entry = archive.GetEntry(entryName);
        Assert.IsNotNull(entry, $"Missing XML entry: {entryName}");

        using StreamReader reader = new(entry.Open());
        return reader.ReadToEnd();
    }
    
    public static string ReadDocxText(string filePath)
    {
        using ZipArchive zipArchive = ZipFile.OpenRead(filePath);

        ZipArchiveEntry entry = zipArchive.GetEntry("word/document.xml") 
            ?? throw new InvalidDataException($"Missing word/document.xml in: {filePath}");
        
        using Stream stream = entry.Open();
        XDocument document = XDocument.Load(stream);
        
        XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        
        return string.Join(
            Environment.NewLine,
            document.Descendants(w + "p").Select(paragraph =>
                string.Concat(paragraph.Descendants()
                    .Where(element =>
                        element.Name == w + "t" ||
                        element.Name == w + "tab" ||
                        element.Name == w + "br" ||
                        element.Name == w + "cr")
                    .Select(element =>
                        element.Name == w + "t" ? element.Value :
                        element.Name == w + "tab" ? "\t" :
                        Environment.NewLine))));
    }

    public static string ReadTextText(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public static string ReadPdfText(string filePath, bool keepPageBreaks = false)
    {
        string tempDir = Files.PrepareTempDir();

        try
        {
            FileJob fileJob = new()
            {
                InputFile = filePath,
                TempPath = Path.Combine(tempDir, "content.txt")
            };

            if (keepPageBreaks)
                Convert.PdfToTxtWithPageBreaks(fileJob);
            else
                Convert.PdfToTxt(fileJob);

            return ReadTextText(fileJob.TempPath);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    public static string[] ReadPdfPageTexts(string filePath)
    {
        // pdftotext ends each page, including the last one, with a form feed.
        string text = ReadPdfText(filePath, keepPageBreaks: true);
        string[] pages = text.Split('\f');

        if (pages.Length > 1 && string.IsNullOrWhiteSpace(pages[^1]))
            pages = pages[..^1];

        return pages.Select(page => page.Trim()).ToArray();
    }
}
