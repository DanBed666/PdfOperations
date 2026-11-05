namespace PdfOperations.Tests;

[TestClass]
public class SearchExtractionTests
{
    [TestMethod]
    [DataRow("NIP: 6666666666,REGON:2137084275972", "NIP", "6666666666")]
    [DataRow("NIP: 6666666666,REGON:2137084275972", "REGON", "2137084275972")]
    [DataRow("Cena: 12,50; regon : 123", "Cena", "12,50")]
    [DataRow("Firma:\nAlfa, Beta Sp. z o.o., REGON:123", "Firma", "Alfa, Beta Sp. z o.o.")]
    [DataRow("Firma: Alfa Sp. z o.o.", "Firma", "Alfa Sp. z o.o.")]
    [DataRow("Firma: REGIONREGON: test, Inne: 5", "Firma", "REGIONREGON: test")]
    [DataRow("Firma: REGON:123", "Firma", "")]
    public void StopsAtNextSelectedLabelWithoutSplittingPunctuation(string text, string phrase, string expected)
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, text);
            string[] fields = ["NIP", "REGON", "Cena", "Firma"];
            var result = Search.GetFoundLines(path, phrase, 0, 0, path, knownPhrases: fields);
            Assert.AreEqual(expected, result.Hits.Single().Value);
            var whole = Search.GetFoundLines(path, phrase, 0, 0, path, saveWholeLine: true, knownPhrases: fields);
            Assert.AreEqual(text.Split('\n')[0], whole.Hits.Single().Value);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    [DataRow("NIP: 0001010808, REGON:2137084275972", "NIP", "0001010808")]
    [DataRow("Kod: 001,Data ważności: jutro", "Kod", "001")]
    [DataRow("Cena: 12,50", "Cena", "12,50")]
    [DataRow("Cena: 12,50, waluta: PLN", "Cena", "12,50")]
    [DataRow("Firma:\nAlfa, Beta Sp. z o.o., Nr sprawy: 5", "Firma", "Alfa, Beta Sp. z o.o.")]
    [DataRow("Opis: Alfa, Beta, https://example.com", "Opis", "Alfa, Beta, https://example.com")]
    [DataRow("Kod: 001, REGON", "Kod", "001, REGON")]
    [DataRow("NIp: 8700000008, REGON\r\n850000151,", "NIP", "8700000008")]
    [DataRow("Ref:\n00120, XYZ\n007", "Ref", "00120")]
    [DataRow("Firma: Alfa, BETA\n007,", "Firma", "Alfa, BETA")]
    [DataRow("Cena: 12,50\n007,", "Cena", "12,50")]
    [DataRow("Ref: 00120, XYZ\nopis", "Ref", "00120, XYZ")]
    public void DetectsCommaSeparatedFieldsWhenOnlyOnePhraseIsSelected(string text, string phrase, string expected)
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, text);
            var found = Search.GetFoundLines(path, phrase, 0, 0, path);
            Assert.AreEqual(expected, found.Hits.Single().Value);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void MarksRelativeLinesInReportButPreservesSourceValue()
    {
        string dir = Directory.CreateTempSubdirectory("SearchOffsets-").FullName;
        try
        {
            string source = Path.Combine(dir, "source.txt");
            string report = Path.Combine(dir, "report.txt");
            File.WriteAllText(source, "Company details\nCode:\n[+1] 007");
            var found = Search.GetFoundLines(source, "Code", 2, 2, source);
            Files.SaveToFile(found, report);

            CollectionAssert.AreEqual(new[] { "[-1] Company details", "[0] Code:", "[+1] [+1] 007" },
                File.ReadAllLines(report).Where(line => line.StartsWith('[')).ToArray());
            var value = Search.GetExtractedValues(new OperationInput { PhraseToFind = "Code" }, report).Single();
            Assert.AreEqual("[+1] 007", value.DesiredValue);
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(dir);
        }
    }

    [TestMethod]
    public void ExtractsFullCompanyNameFromSameLine()
    {
        ExtractedValue[] results = Extract("Company", 2,
            ("company.pdf", "Company: Studio Graphic ABC"));

        Assert.HasCount(1, results);
        Assert.AreEqual("company.pdf", results[0].FileName);
        Assert.AreEqual("Studio Graphic ABC", results[0].DesiredValue);
    }

    [TestMethod]
    public void PreservesColonsInsideValue()
    {
        ExtractedValue[] results = Extract("Hours", 2,
            ("hours.pdf", "Hours: 08:30 - 16:00"));

        Assert.HasCount(1, results);
        Assert.AreEqual("08:30 - 16:00", results[0].DesiredValue);
    }

    [TestMethod]
    [DataRow("Company:\nStudio ABC")]
    [DataRow("Company:\n\n   \nStudio ABC")]
    [DataRow("Company\n\nStudio ABC")]
    public void ReadsNextNonemptyLine(string text)
    {
        ExtractedValue[] results = Extract("Company", 5, ("company.pdf", text));

        Assert.HasCount(1, results);
        Assert.AreEqual("Studio ABC", results[0].DesiredValue);
    }

    [TestMethod]
    public void KeepsSourceNamesForMixedFormats()
    {
        ExtractedValue[] results = Extract("Company", 2,
            ("first.PDF", "Company: Alpha"),
            ("second.txt", "Company: Beta"),
            ("third.png", "Company: Gamma"),
            ("fourth.jpeg", "Company: Delta"));

        CollectionAssert.AreEqual(
            new[] { "first.PDF", "second.txt", "third.png", "fourth.jpeg" },
            results.Select(result => result.FileName).ToArray());
        CollectionAssert.AreEqual(new[] { "Alpha", "Beta", "Gamma", "Delta" },
            results.Select(result => result.DesiredValue).ToArray());
    }

    [TestMethod]
    public void MissingValueDoesNotReadNextResultHeader()
    {
        ExtractedValue[] results = Extract("Company", 2,
            ("empty.pdf", "Company:"),
            ("next.txt", "Company: Beta"));

        Assert.HasCount(2, results);
        Assert.AreEqual("", results[0].DesiredValue);
        Assert.AreEqual("Beta", results[1].DesiredValue);
    }

    [TestMethod]
    public void ValueOutsideReportContextIsNotRead()
    {
        ExtractedValue[] results = Extract("Company", 0,
            ("company.pdf", "Company:\nStudio ABC"));

        Assert.HasCount(1, results);
        Assert.AreEqual("", results[0].DesiredValue);
    }

    [TestMethod]
    public void MatchesLabelIgnoringCaseWhitespaceAndTrailingColon()
    {
        ExtractedValue[] results = Extract(" company: ", 2,
            ("company.pdf", "   COMPANY: Studio ABC   "));

        Assert.HasCount(1, results);
        Assert.AreEqual("Studio ABC", results[0].DesiredValue);
    }

    [TestMethod]
    public void DoesNotTreatLongerLabelAsExactMatch()
    {
        ExtractedValue[] results = Extract("Company", 2,
            ("company.pdf", "Company registration: 00123"));

        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void NoMatchesReturnsEmptyArray()
    {
        ExtractedValue[] results = Extract("Company", 2,
            ("company.pdf", "Address: Main Street 12"));

        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void EmptyPhraseIsRejectedBeforeReadingReport()
    {
        Assert.Throws<ArgumentException>(() => Search.GetExtractedValues(
            new OperationInput { PhraseToFind = " : " }, "unused.txt"));
    }

    private static ExtractedValue[] Extract(string phrase, int after,
        params (string Name, string Text)[] sources)
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsExtractionTest-").FullName;
        try
        {
            string reportPath = Path.Combine(directory, "report.txt");
            foreach (var source in sources)
            {
                string textPath = Path.Combine(directory, "working.txt");
                File.WriteAllText(textPath, source.Text);
                SearchResult found = Search.GetFoundLines(textPath, phrase, 0, after,
                    Path.Combine(directory, source.Name));
                Files.SaveToFile(found, reportPath);
            }

            return Search.GetExtractedValues(
                new OperationInput { PhraseToFind = phrase, After = after }, reportPath);
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }
    
    [TestMethod]
    public void SearchesAllFieldsAndKeepsFilesSeparate()
    {
        string directory = Directory.CreateTempSubdirectory("MultiSearchTest-").FullName;
        try
        {
            string firstFolder = Directory.CreateDirectory(
                Path.Combine(directory, "first")).FullName;
            string secondFolder = Directory.CreateDirectory(
                Path.Combine(directory, "second")).FullName;

            string first = Path.Combine(firstFolder, "document.txt");
            string second = Path.Combine(secondFolder, "document.txt");

            File.WriteAllText(first,
                "NIP:\nNazwisko: Kowalski\n" +
                "Telefon: 123456789\nTelefon: 123456789\nTelefon: 123456789\n" +
                "Telefon: 987654321\nTelefon: 555666777");

            File.WriteAllText(second,
                "NIP: 0012345678\nNazwisko: Nowak\nTelefon:\n\n000111222");

            var input = new OperationInput
            {
                InputFiles = [first, second],
                PhrasesToFind = ["NIP", "Nazwisko", "Telefon", "Adres"],
                Output = "report.txt",
                Before = 0,
                After = 20
            };
            var context = new OperationContext { TempDir = directory };

            for (int i = 0; i < input.InputFiles.Length; i++)
            {
                string workingPath = Path.Combine(directory, $"working_{i}.txt");
                File.Copy(input.InputFiles[i], workingPath);

                Search.SearchTempTextFiles(input, context, new FileJob
                {
                    InputFile = input.InputFiles[i],
                    TempPath = workingPath
                });

                Assert.IsFalse(File.Exists(workingPath));
                Assert.IsTrue(File.Exists(input.InputFiles[i]));
            }

            var results = Search.GetFileExtractionResults(
                input, Path.Combine(directory, input.Output));

            Assert.HasCount(2, results);
            var firstResult = results.Single(result => result.FilePath == first);
            var secondResult = results.Single(result => result.FilePath == second);

            Assert.HasCount(0, firstResult.Fields["NIP"]);
            CollectionAssert.AreEqual(new[] { "Kowalski" }, firstResult.Fields["Nazwisko"]);
            CollectionAssert.AreEqual(
                new[] { "123456789", "987654321", "555666777" },
                firstResult.Fields["Telefon"]);
            CollectionAssert.AreEqual(new[] { "0012345678" }, secondResult.Fields["NIP"]);
            CollectionAssert.AreEqual(new[] { "Nowak" }, secondResult.Fields["Nazwisko"]);
            CollectionAssert.AreEqual(new[] { "000111222" }, secondResult.Fields["Telefon"]);
            Assert.IsTrue(results.All(result => result.Fields["Adres"].Length == 0));
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    public void ExportsMatchingLinesWithLocationsAndSummarizesUniqueValues()
    {
        string dir = Directory.CreateTempSubdirectory("SearchHitExport-").FullName;
        try
        {
            string first = Path.Combine(dir, "first.txt");
            string second = Path.Combine(dir, "second.txt");
            File.WriteAllText(first, "Code: 007\nCode:\n\n007\fCompany based in Toronto");
            File.WriteAllText(second, "Code:\nBased in London");
            var input = new OperationInput
            {
                InputFiles = [first, second], Output = "report.txt",
                PhrasesToFind = ["Code", "based in"],
                SearchFields = [new("Code", false), new("based in", true)]
            };
            var hits = new List<SearchHit>();
            foreach (string path in input.InputFiles)
            {
                string working = path + ".tmp";
                File.Copy(path, working);
                hits.AddRange(Search.SearchTempTextFiles(input, new OperationContext { TempDir = dir },
                    new FileJob { InputFile = path, TempPath = working }, CancellationToken.None));
            }

            Assert.HasCount(5, hits);
            Assert.AreEqual(new SearchHit(first, 1, 2, "Code", "007"), hits[1]);
            Assert.AreEqual(new SearchHit(first, 2, 1, "based in", "Company based in Toronto"), hits[2]);
            Assert.AreEqual(second, hits[4].FilePath);
            Assert.AreEqual("", hits[3].Value);
            var summary = Search.SummarizeHits(input, hits);
            CollectionAssert.AreEqual(new[] { "007" }, summary[0].Fields["Code"]);
            CollectionAssert.AreEqual(new[] { "Based in London" }, summary[1].Fields["based in"]);

            var labels = new ExportLabels { Page = "Strona", Line = "Linia", Phrase = "Fraza" };
            string xlsx = Path.Combine(dir, "hits.xlsx");
            string json = Path.Combine(dir, "hits.json");
            ExcelExporter.SaveHitsToExcel(hits, xlsx, labels);
            JsonExporter.SaveHitsToJson(hits, json, labels);
            using var book = new ClosedXML.Excel.XLWorkbook(xlsx);
            var sheet = book.Worksheet(labels.WorksheetName);
            Assert.AreEqual(6, sheet.LastRowUsed()!.RowNumber());
            Assert.AreEqual("Linia", sheet.Cell(1, 4).GetString());
            using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(json));
            var records = document.RootElement;
            Assert.AreEqual(hits.Count, records.GetArrayLength());
            for (int i = 0; i < hits.Count; i++)
            {
                Assert.AreEqual(hits[i].Value, sheet.Cell(i + 2, 6).GetString());
                Assert.AreEqual(hits[i].Line, sheet.Cell(i + 2, 4).GetValue<int>());
                Assert.AreEqual(hits[i].Value, records[i].GetProperty(labels.ValueProperty).GetString());
                Assert.AreEqual(hits[i].Page, records[i].GetProperty("Strona").GetInt32());
                Assert.AreEqual(hits[i].FilePath, records[i].GetProperty(labels.FilePathProperty).GetString());
            }
        }
        finally
        {
            TemporaryDirectoryCleanup.Delete(dir);
        }
    }
}
