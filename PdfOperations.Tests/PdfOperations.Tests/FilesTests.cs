using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;

namespace PdfOperations.Tests;

[TestClass]
public class FilesTests
{
    [TestMethod]
    public void GetDefaultDirTest()
    {
        string defDir = Path.Combine(AppContext.BaseDirectory, "output");
        
        if (Directory.Exists(defDir))
            Directory.Delete(defDir, true);
        
        string dir = Files8.GetDefaultDirectory();
        
        Assert.IsTrue(Directory.Exists(dir));
    }
    
    [TestMethod]
    public void PrepareTempDirTest()
    {
        string tempDir = Files8.PrepareTempDir();
        Assert.IsTrue(Directory.Exists(tempDir));
    }
    
    [TestMethod]
    public void PrepareTempPathMultipleTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".jpg";
        List<string> tempPaths = new List<string>();

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles);
        OperationContext context = TestHelper8.SetOperationContext();
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);

        foreach (string inp in input.InputFiles)
        {
            string path = Files8.PrepareTempPathMultiple(context.TempDir, inp, operation.Extension);
            Console.WriteLine(path);
            tempPaths.Add(path);
        }

        foreach (string path in tempPaths)
        {
            Assert.IsNotNull(path);
        }
    }
    
    [TestMethod]
    public void PrepareTempPathMultipleToSingleTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf"};
        string output = "test.pdf";

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);
        OperationContext context = TestHelper8.SetOperationContext();

        string path = Files8.PrepareTempPathSingle(context.TempDir, input.Output);
        Console.WriteLine(path);
        
        Assert.IsNotNull(path);
    }
    
    [TestMethod]
    public void PrepareTempPathSingleTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.txt";

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);
        OperationContext context = TestHelper8.SetOperationContext();

        string path = Files8.PrepareTempPathSingle(context.TempDir, input.Output);
        Console.WriteLine(path);
        
        Assert.IsNotNull(path);
    }
    
    [TestMethod]
    public void PrepareFinalPathMultipleTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(".txt");
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, dir: dir);
        OperationContext context = TestHelper8.SetOperationContext();

        foreach (string file in inputFiles)
        {
            string tempPath = Files8.PrepareTempPathMultiple(context.TempDir, file, operation.Extension);
            string path = Files8.PrepareFinalOutputPath(input.Dir, tempPath);
            Console.WriteLine(path);
            Assert.IsNotNull(path);
        }
    }
    
    [TestMethod]
    public void PrepareFinalPathTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.pdf";
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output, dir: dir);
        OperationContext context = TestHelper8.SetOperationContext();
        
        string tempPath = Files8.PrepareTempPathSingle(context.TempDir, input.Output);
        string path = Files8.PrepareFinalOutputPath(input.Dir, tempPath);
        Console.WriteLine(path);
        
        Assert.IsNotNull(path);
    }
    
    [TestMethod]
    public void FindNotExistingTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        string[] tempPaths = new string[inputs.Length];
        List<FileJob> fileJobs = new List<FileJob>();
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, dir: dir);
        OperationContext context = TestHelper8.SetOperationContext();
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        
        for (int i = 0; i < input.InputFiles.Length; i++)
        {
            string path = Files8.PrepareTempPathMultiple(context.TempDir, input.InputFiles[i], operation.Extension);
            tempPaths[i] = path;

            File.Copy(input.InputFiles[i], tempPaths[i]);
            string pathFin = Files8.PrepareFinalOutputPath(input.Dir, tempPaths[i]);
            FileJob fileJob = TestHelper8.SetFileJob(tempPaths[i], pathFin);

            if (i == 1)
            {
                File.Copy(fileJob.TempPath, fileJob.FinalPath);
                Console.WriteLine(fileJob.FinalPath);
                Assert.IsNotNull(fileJob.FinalPath);
            }

            fileJobs.Add(fileJob);
            Assert.HasCount(3, fileJobs);
        }

        Dictionary <string, string> exist = Files8.MoveNewFilesAndReturnConflicts(fileJobs);

        foreach (KeyValuePair<string, string> item in exist)
        {
            Console.WriteLine(item.Key);
            Console.WriteLine(item.Value);
        }
    }
    
    [TestMethod]
    public void OverwriteTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        string[] tempPaths = new string[inputs.Length];
        List<FileJob> fileJobs = new List<FileJob>();
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, dir: dir);
        OperationContext context = TestHelper8.SetOperationContext();
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        
        for (int i = 0; i < input.InputFiles.Length; i++)
        {
            string path = Files8.PrepareTempPathMultiple(context.TempDir, input.InputFiles[i], operation.Extension);
            tempPaths[i] = path;

            File.Copy(input.InputFiles[i], tempPaths[i]);
            string pathFin = Files8.PrepareFinalOutputPath(input.Dir, tempPaths[i]);
            FileJob fileJob = TestHelper8.SetFileJob(tempPaths[i], pathFin);

            if (i == 1)
            {
                File.Copy(fileJob.TempPath, fileJob.FinalPath);
                Console.WriteLine(fileJob.FinalPath);
                Assert.IsNotNull(fileJob.FinalPath);
            }

            fileJobs.Add(fileJob);
            Assert.HasCount(3, fileJobs);
        }
        
        Dictionary <string, string> exist = Files8.MoveNewFilesAndReturnConflicts(fileJobs);
        Files8.OverWriteFile(exist);
    }
    
    [TestMethod]
    public void CheckFileFormatTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.pdf";

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);

        bool check = CheckParams.CheckFileFormat(input.Output, out string format);
        Console.WriteLine(format);
        Console.WriteLine(check);
    }
    
    [TestMethod]
    public void CheckFormatTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.pdf";

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);
        
        bool check = CheckParams.CheckFormat(input.Output);
        Console.WriteLine(check);
    }
    
    [TestMethod]
    public void FixFormatExistTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.pdf";
        string extension = ".jpg";

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension: extension);

        CheckParams.FixFormatExist(operation.Extension, input, input.Output);
        Console.WriteLine(input.Output);
    }
    
    [TestMethod]
    public void FixFormatNotExistTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2";
        string extension = ".pdf";

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, output: output);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension: extension);

        CheckParams.FixFormatNotExist(operation.Extension, input, input.Output);
        Console.WriteLine(input.Output);
    }
    
    [TestMethod]
    public void SetFileJobTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, dir: dir);
        OperationContext context = TestHelper8.SetOperationContext();
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        
        FileJob fileJob = ExecutionBuilder.SetFileJob(input, context, operation);
        Console.WriteLine(JsonSerializer.Serialize(fileJob, new JsonSerializerOptions {WriteIndented = true}));
    }
    
    [TestMethod]
    public void SetFileJobListTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, dir: dir);
        OperationContext context = TestHelper8.SetOperationContext();
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);

        foreach (FileJob fileJob in fileJobList)
        {
            Console.WriteLine(JsonSerializer.Serialize(fileJob, new JsonSerializerOptions {WriteIndented = true}));
        }
    }
    
    [TestMethod]
    public void SetFileJobLibreTest()
        {
            string [] inputs = new [] {"word_1.docx", "word_2.docx", "word_3.docx"};
            string extension = ".pdf";
            
            string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            
            string[] inputFiles = TestHelper8.SetInputPaths(inputs);
            OperationInput input = TestHelper8.SetOperationInput(inputFiles, dir: dir, format: "pdf");
            OperationContext context = TestHelper8.SetOperationContext();
            
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobLibre(input, context);
    
            foreach (FileJob fileJob in fileJobList)
            {
                Console.WriteLine(JsonSerializer.Serialize(fileJob, new JsonSerializerOptions {WriteIndented = true}));
            }
        }
    
    [TestMethod]
    public void SetDirContext()
    {
        OperationContext context = ExecutionBuilder.SetOperationContext();
        Console.WriteLine(context.TempDir);
    }
    
    [TestMethod]
    public void SetUniqueNameTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        string[] tempPaths = new string[inputs.Length];
        List<FileJob> fileJobs = new List<FileJob>();
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, dir: dir);
        OperationContext context = TestHelper8.SetOperationContext();
        OperationDefinition operation = TestHelper8.SetOperationDefinition(extension);
        
        for (int i = 0; i < input.InputFiles.Length; i++)
        {
            string path = Files8.PrepareTempPathMultiple(context.TempDir, input.InputFiles[i], operation.Extension);
            //Console.WriteLine(path);
            tempPaths[i] = path;

            File.Copy(input.InputFiles[i], tempPaths[i]);
            string pathFin = Files8.PrepareFinalOutputPath(input.Dir, tempPaths[i]);
            FileJob fileJob = TestHelper8.SetFileJob(tempPaths[i], pathFin);

            if (i == 1)
            {
                File.Copy(fileJob.TempPath, fileJob.FinalPath);
                Console.WriteLine(fileJob.FinalPath);
            }

            fileJobs.Add(fileJob);
        }
        
        Dictionary <string, string> exist = Files8.MoveNewFilesAndReturnConflicts(fileJobs);
        Files8.SaveWithUniqueFileName(extension, exist);
    }

    [TestMethod]
    public void ErrorLoggerTest()
    {
        var ex = new Exception("Testowy błąd Loggera");
        ErrorLogger.Log(ex);
    }

    [TestMethod]
    public void SaveFileTest()
    {
        string[] inputs = new[] { "search_1.txt", "search_2.txt", "search_3.txt" };
        string format = ".txt";
        int count = 3;
        string phrase = "hydraulika";
        List<List<string>> allFound = new List<List<string>>();

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(format);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles, phrase: phrase, before: -2, after: 2);
        OperationContext context = TestHelper8.SetOperationContext();

        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);

        foreach (FileJob fileJob in fileJobList)
        {
            File.Copy(fileJob.InputFile, fileJob.TempPath);
        }

        try
        {
            foreach (FileJob fileJob in fileJobList)
            {
                List<List<string>> result =
                    Search.SearchNewTxt(fileJob.TempPath, input.PhraseToFind, input.Before, input.After);
                allFound.AddRange(result);
                Files8.SaveToFile(result, Path.Combine(context.TempDir, "output.txt"));
            }
        }
        finally
        {
            //if (Directory.Exists(context.TempDir))
            //Directory.Delete(context.TempDir, true);
        }
        
        Assert.IsTrue(Path.Exists(Path.Combine(context.TempDir, "output.txt")));
        Assert.IsGreaterThan(0, new FileInfo(Path.Combine(context.TempDir, "output.txt")).Length);
    }
    
    [TestMethod]
    public void ReadFileTest()
    {
        string[] inputs = new[] { "search_1.txt", "search_2.txt", "search_3.txt" };
        string format = ".txt";
        int count = 3;
        string[] text;

        string[] inputFiles = TestHelper8.SetInputPaths(inputs);
        OperationDefinition operation = TestHelper8.SetOperationDefinition(format);
        OperationInput input = TestHelper8.SetOperationInput(inputFiles);
        OperationContext context = TestHelper8.SetOperationContext();
        
        List<FileJob> fileJobList = ExecutionBuilder.SetFileJobList(input, context, operation);

        foreach (FileJob fileJob in fileJobList)
        {
            File.Copy(fileJob.InputFile, fileJob.TempPath);
            text = Files8.ReadFile(fileJob.TempPath);
            Assert.IsNotNull(text);

            foreach (string t in text)
            {
                Console.WriteLine(t);
            }
        }
    }

    [TestMethod]
    public void NormalizeExtensionTest()
    {
        string extension = ".pdf";
        string extension2 = "jpg";
        string res = CheckParams.NormalizeExtension(extension);
        string res2 = CheckParams.NormalizeExtension(extension2);
        
        Console.WriteLine(res + " " + res2);
        
        Assert.AreEqual(".pdf", res);
        Assert.AreEqual(".jpg", res2);
    }
}