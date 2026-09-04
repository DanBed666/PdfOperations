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
        
        string dir = Files.GetDefaultDirectory();
        
        Assert.IsTrue(Directory.Exists(dir));
        Assert.AreEqual(defDir, dir);
    }
    
    [TestMethod]
    public void PrepareTempDirTest()
    {
        string tempDir = Files.PrepareTempDir();
        
        Assert.IsTrue(Directory.Exists(tempDir));
        StringAssert.StartsWith(tempDir, Path.GetTempPath());
    }
    
    [TestMethod]
    public void PrepareTempPathMultipleTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".jpg";
        List<string> tempPaths = new List<string>();

        TestInput testInput = TestHelper.PrepareMultipleInputs(inputs, extension);
        
        try
        {
            foreach (string inp in testInput.Input.InputFiles)
            {
                string path = Files.PrepareTempPathMultiple(testInput.Context.TempDir, inp, testInput.Operation.Extension);
                tempPaths.Add(path);
            }

            Assert.HasCount(3, tempPaths);

            foreach (string path in tempPaths)
            {
                Assert.IsNotNull(path);
                Assert.AreEqual(extension, Path.GetExtension(path));
                StringAssert.StartsWith(path, testInput.Context.TempDir);
            }
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PrepareTempPathMultipleToSingleTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf"};
        string output = "test.pdf";

        TestInput testInput = TestHelper.PrepareInputWithOutput(inputs, output);

        try
        {
            string path = Files.PrepareTempPathSingle(testInput.Context.TempDir, testInput.Input.Output);
            Assert.IsNotNull(path);
            Assert.AreEqual(Path.Combine(testInput.Context.TempDir, output), path);
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PrepareTempPathSingleTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.txt";

        TestInput testInput = TestHelper.PrepareInputWithOutput(inputs, output);
        
        try
        {
            string path = Files.PrepareTempPathSingle(testInput.Context.TempDir, testInput.Input.Output);
            Assert.IsNotNull(path);
            Assert.AreEqual(Path.Combine(testInput.Context.TempDir, output), path);
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void PrepareFinalPathMultipleTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".txt";
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);

        TestInput testInput = TestHelper.PrepareMultipleInputsWithDir(inputs, extension, dir);
        
        try
        {
            foreach (string file in testInput.InputFiles)
            {
                string tempPath = Files.PrepareTempPathMultiple(testInput.Context.TempDir, file, testInput.Operation.Extension);
                string path = Files.PrepareFinalOutputPath(testInput.Input.Dir, tempPath);

                Assert.IsNotNull(path);
                Assert.AreEqual(testInput.Operation.Extension, Path.GetExtension(path));
                StringAssert.StartsWith(path, dir);
            }
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
            
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
    
    [TestMethod]
    public void PrepareFinalPathTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string output = "test2.pdf";
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);

        TestInput testInput = TestHelper.PrepareMultipleInputsWithDirOutput(inputs, output, dir);
        
        try
        {
            string tempPath = Files.PrepareTempPathSingle(testInput.Context.TempDir, testInput.Input.Output);
            string path = Files.PrepareFinalOutputPath(testInput.Input.Dir, tempPath);

            Assert.IsNotNull(path);
            Assert.AreEqual(Path.GetFileName(tempPath), Path.GetFileName(path));
            Assert.AreEqual(Path.Combine(dir, output), path);
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
            
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
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
        
        TestInput testInput = TestHelper.PrepareMultipleInputsWithDir(inputs, extension, dir);

        try
        {
            for (int i = 0; i < testInput.Input.InputFiles.Length; i++)
            {
                string path = Files.PrepareTempPathMultiple(testInput.Context.TempDir, testInput.Input.InputFiles[i],
                    testInput.Operation.Extension);
                tempPaths[i] = path;

                File.Copy(testInput.Input.InputFiles[i], tempPaths[i]);
                string pathFin = Files.PrepareFinalOutputPath(testInput.Input.Dir, tempPaths[i]);
                FileJob fileJob = TestHelper.SetFileJob(tempPaths[i], pathFin);

                if (i == 1)
                {
                    File.Copy(fileJob.TempPath, fileJob.FinalPath);
                    Assert.IsNotNull(fileJob.FinalPath);
                }

                fileJobs.Add(fileJob);
            }

            Assert.HasCount(3, fileJobs);

            Dictionary<string, string> exist =
                Files.MoveNewFilesAndReturnConflicts(testInput.Input.Dir, testInput.Context.TempDir);

            Assert.HasCount(1, exist);
            Assert.IsTrue(File.Exists(fileJobs[0].FinalPath));
            Assert.IsTrue(File.Exists(fileJobs[1].TempPath));
            Assert.IsTrue(File.Exists(fileJobs[2].FinalPath));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
            
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
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
        
        TestInput testInput = TestHelper.PrepareMultipleInputsWithDir(inputs, extension, dir);

        try
        {
            for (int i = 0; i < testInput.Input.InputFiles.Length; i++)
            {
                string path = Files.PrepareTempPathMultiple(testInput.Context.TempDir, testInput.Input.InputFiles[i],
                    testInput.Operation.Extension);
                tempPaths[i] = path;

                File.Copy(testInput.Input.InputFiles[i], tempPaths[i]);
                string pathFin = Files.PrepareFinalOutputPath(testInput.Input.Dir, tempPaths[i]);
                FileJob fileJob = TestHelper.SetFileJob(tempPaths[i], pathFin);

                if (i == 1)
                {
                    File.Copy(fileJob.TempPath, fileJob.FinalPath);
                    Assert.IsNotNull(fileJob.FinalPath);
                }

                fileJobs.Add(fileJob);
            }

            Assert.HasCount(3, fileJobs);

            Dictionary<string, string> exist =
                Files.MoveNewFilesAndReturnConflicts(testInput.Input.Dir, testInput.Context.TempDir);
            Files.OverWriteFile(exist);

            Assert.HasCount(1, exist);
            Assert.IsTrue(File.Exists(fileJobs[0].FinalPath));
            Assert.IsTrue(File.Exists(fileJobs[1].FinalPath));
            Assert.IsTrue(File.Exists(fileJobs[2].FinalPath));
            Assert.IsFalse(File.Exists(fileJobs[0].TempPath));
            Assert.IsFalse(File.Exists(fileJobs[1].TempPath));
            Assert.IsFalse(File.Exists(fileJobs[2].TempPath));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
            
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
    
    [TestMethod]
    public void SetFileJobTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        TestInput testInput = TestHelper.PrepareMultipleInputsWithDir(inputs, extension, dir);

        try
        {

            FileJob fileJob = ExecutionBuilder.SetFileJob(testInput.Input, testInput.Context, testInput.Operation);

            Assert.IsNotNull(fileJob);
            Assert.AreEqual(Path.Combine(testInput.Context.TempDir, "default.pdf"), fileJob.TempPath);
            Assert.AreEqual(Path.Combine(dir, "default.pdf"), fileJob.FinalPath);
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
            
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
    
    [TestMethod]
    public void SetFileJobListTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        
        TestInput testInput = TestHelper.PrepareMultipleInputsWithDir(inputs, extension, dir);

        try
        {
            List<FileJob> fileJobList =
                ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);

            Assert.HasCount(3, fileJobList);
            Assert.IsTrue(fileJobList.All(fj => fj.TempPath.StartsWith(testInput.Context.TempDir)));
            Assert.IsTrue(fileJobList.All(fj => fj.FinalPath.StartsWith(dir)));
            Assert.IsTrue(fileJobList.All(fj => Path.GetExtension(fj.TempPath) == extension));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
            
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
    
    [TestMethod]
    public void SetFileJobLibreTest()
    {
        string [] inputs = new [] {"word_1.docx", "word_2.docx", "word_3.docx"};
        string extension = ".pdf";
        
        string dir = Path.Combine(Path.GetTempPath(), "Folderos" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);

        TestInput testInput = TestHelper.PrepareMultipleInputsLibre(inputs, extension, dir);

        try
        {
            List<FileJob> fileJobList = ExecutionBuilder.SetFileJobLibre(testInput.Input, testInput.Context);

            Assert.HasCount(3, fileJobList);
            Assert.IsTrue(fileJobList.All(fj => fj.TempPath.StartsWith(testInput.Context.TempDir)));
            Assert.IsTrue(fileJobList.All(fj => fj.FinalPath.StartsWith(dir)));
            Assert.IsTrue(fileJobList.All(fj => Path.GetExtension(fj.TempPath) == extension));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
            
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
    
    [TestMethod]
    public void SetDirContext()
    {
        OperationContext context = ExecutionBuilder.SetOperationContext();
        Assert.IsTrue(Directory.Exists(context.TempDir));
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
        
        TestInput testInput = TestHelper.PrepareMultipleInputsWithDir(inputs, extension, dir);

        try
        {

            for (int i = 0; i < testInput.Input.InputFiles.Length; i++)
            {
                string path = Files.PrepareTempPathMultiple(testInput.Context.TempDir, testInput.Input.InputFiles[i],
                    testInput.Operation.Extension);
                tempPaths[i] = path;

                File.Copy(testInput.Input.InputFiles[i], tempPaths[i]);
                string pathFin = Files.PrepareFinalOutputPath(testInput.Input.Dir, tempPaths[i]);
                FileJob fileJob = TestHelper.SetFileJob(tempPaths[i], pathFin);

                if (i == 1)
                {
                    File.Copy(fileJob.TempPath, fileJob.FinalPath);
                }

                fileJobs.Add(fileJob);
            }

            Dictionary<string, string> exist =
                Files.MoveNewFilesAndReturnConflicts(testInput.Input.Dir, testInput.Context.TempDir);
            Files.SaveWithUniqueFileName(extension, exist);

            Assert.HasCount(1, exist);
            Assert.IsFalse(File.Exists(fileJobs[1].TempPath));
            Assert.IsTrue(Directory.GetFiles(dir).Any(f => Path.GetFileName(f).StartsWith("ocr_test_2_")));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
            
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }

    [TestMethod]
    public void ErrorLoggerTest()
    {
        string message = "Testowy błąd Loggera";
        var ex = new Exception(message);
        ErrorLogger.Log(ex);
        
        string path = Path.Combine(AppContext.BaseDirectory, "logs", "error_log.txt");
        Assert.IsTrue(File.Exists(path));
        StringAssert.Contains( File.ReadAllText(path), message);
    }

    [TestMethod]
    public void SaveFileTest()
    {
        string[] inputs = new[] { "search_1.txt", "search_2.txt", "search_3.txt" };
        string format = ".txt";
        int count = 3;
        string phrase = "hydraulika";
        List<List<string>> allFound = new List<List<string>>();

        TestInput testInput = TestHelper.PrepareMultipleInputsSearch(inputs, phrase, format, -2, 2);

        try
        {
            List<FileJob> fileJobList =
                ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);

            foreach (FileJob fileJob in fileJobList)
            {
                File.Copy(fileJob.InputFile, fileJob.TempPath);
            }

            foreach (FileJob fileJob in fileJobList)
            {
                string originalFile = Files.FindOriginalFileForTemp(fileJob.TempPath, fileJob.InputFiles);
                List<List<string>> result =
                    Search.SearchNewTxt(fileJob.TempPath, originalFile, testInput.Input.PhraseToFind, testInput.Input.Before,
                        testInput.Input.After);
                allFound.AddRange(result);
                Files.SaveToFile(result, Path.Combine(testInput.Context.TempDir, "output.txt"));
            }

            Assert.IsTrue(Path.Exists(Path.Combine(testInput.Context.TempDir, "output.txt")));
            Assert.IsGreaterThan(0, new FileInfo(Path.Combine(testInput.Context.TempDir, "output.txt")).Length);
            string saved = File.ReadAllText(Path.Combine(testInput.Context.TempDir, "output.txt"));
            Assert.IsTrue(saved.Contains("hydraulika", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void ReadFileTest()
    {
        string[] inputs = new[] { "search_1.txt", "search_2.txt", "search_3.txt" };
        string extension = ".txt";
        int count = 3;
        string[] text;

        TestInput testInput = TestHelper.PrepareMultipleInputs(inputs, extension);

        try
        {
            List<FileJob> fileJobList =
                ExecutionBuilder.SetFileJobList(testInput.Input, testInput.Context, testInput.Operation);

            foreach (FileJob fileJob in fileJobList)
            {
                File.Copy(fileJob.InputFile, fileJob.TempPath);
                text = Files.ReadFile(fileJob.TempPath);
                
                Assert.IsNotNull(text);
                Assert.IsGreaterThan(0, text.Length);
                Assert.IsTrue(text.Any(line => line.Contains("hydraulika", StringComparison.OrdinalIgnoreCase)));
            }
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
    
    [TestMethod]
    public void FindOriginalFileForTempTest()
    {
        string [] inputs = new [] {"ocr_test_1.pdf", "ocr_test_2.pdf", "ocr_test_3.pdf"};
        string extension = ".pdf";
        List<string> originalFiles = new List<string>();

        TestInput testInput = TestHelper.PrepareMultipleInputs(inputs, extension);

        try
        {
            foreach (string inputFile in testInput.InputFiles)
            {
                string tempPath = Path.Combine(testInput.Context.TempDir, Path.GetFileName(inputFile));
                File.Copy(inputFile, tempPath);
            }

            foreach (string file in Directory.GetFiles(testInput.Context.TempDir))
            {
                string originalFile = Files.FindOriginalFileForTemp(file, testInput.InputFiles);
                Assert.IsNotNull(originalFile);
                originalFiles.Add(originalFile);
            }

            Assert.HasCount(3, originalFiles);
            Assert.IsTrue(originalFiles.All(file => testInput.InputFiles.Contains(file)));
        }
        finally
        {
            if (Directory.Exists(testInput.Context.TempDir))
                Directory.Delete(testInput.Context.TempDir, true);
        }
    }
}