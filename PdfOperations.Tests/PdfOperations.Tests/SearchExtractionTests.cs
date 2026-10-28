namespace PdfOperations.Tests;

[TestClass]
public class SearchExtractionTests
{
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
}
