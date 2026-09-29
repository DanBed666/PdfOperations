namespace PdfOperations.Tests;

[TestClass]
public class ExecuteTests
{
    [TestMethod]
    public void MoveFilesAndCreateConflictsTestZeroFiles()
    {
        string finalDir = "";
        
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["plik_test_1.txt", "plik_test_2.txt", "plik_test_3.txt"]),
            Dir = "final_dir_name"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        //Prepare temp and final dir

        try
        {
            foreach (string file in operationInput.InputFiles)
            {
                File.Copy(file, Files.PrepareTempPath(operationContext.TempDir, file, operationDefinition.Extension));
            }

            finalDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + $"_{operationInput.Dir}");
            Directory.CreateDirectory(finalDir);

            Dictionary<string, string> conflicts =
                Execute.MoveNewFilesAndCollectConflicts(operationContext.TempDir, finalDir);

            string[] finalDirArray = Directory.GetFiles(finalDir).Select(file => Path.GetFileName(file)).ToArray();

            Assert.Contains("plik_test_1.txt", finalDirArray);
            Assert.Contains("plik_test_2.txt", finalDirArray);
            Assert.Contains("plik_test_3.txt", finalDirArray);
            Assert.HasCount(3, finalDirArray);
            Assert.HasCount(0, conflicts);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
            
            if (Directory.Exists(finalDir))
                Directory.Delete(finalDir, true);
        }
    }
    
    [TestMethod]
    public void MoveFilesAndCreateConflictsTestWithConflicts()
    {
        string finalDir = "";
        
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["plik_test_1.txt", "plik_test_2.txt", "plik_test_3.txt"]),
            Dir = "final_dir_name"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        //Prepare temp and final dir

        try
        {
            foreach (string file in operationInput.InputFiles)
            {
                File.Copy(file, Files.PrepareTempPath(operationContext.TempDir, file, operationDefinition.Extension));
            }

            finalDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + $"_{operationInput.Dir}");
            Directory.CreateDirectory(finalDir);

            File.WriteAllText(Path.Combine(finalDir, "plik_test_1.txt"), "tekstos");
            File.WriteAllText(Path.Combine(finalDir, "plik_test_2.txt"), "tekstos");
            File.WriteAllText(Path.Combine(finalDir, "pliczek.txt"), "tekstos");

            Dictionary<string, string> conflicts =
                Execute.MoveNewFilesAndCollectConflicts(operationContext.TempDir, finalDir);

            string[] finalDirArray = Directory.GetFiles(finalDir).Select(file => Path.GetFileName(file)).ToArray();

            Assert.Contains("plik_test_1.txt", finalDirArray);
            Assert.Contains("plik_test_2.txt", finalDirArray);
            Assert.Contains("plik_test_3.txt", finalDirArray);
            Assert.Contains("pliczek.txt", finalDirArray);
            Assert.HasCount(4, finalDirArray);
            Assert.HasCount(2, conflicts);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
            
            if (Directory.Exists(finalDir))
                Directory.Delete(finalDir, true);
        }
    }
    
    [TestMethod]
    public void MoveFilesAndCreateConflictsTestWithNoConflicts()
    {
        string finalDir = "";
        
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["plik_test_1.txt", "plik_test_2.txt", "plik_test_3.txt"]),
            Dir = "final_dir_name"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        //Prepare temp and final dir

        try
        {
            foreach (string file in operationInput.InputFiles)
            {
                File.Copy(file, Files.PrepareTempPath(operationContext.TempDir, file, operationDefinition.Extension));
            }

            finalDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + $"_{operationInput.Dir}");
            Directory.CreateDirectory(finalDir);

            File.WriteAllText(Path.Combine(finalDir, "test.txt"), "tekstos");
            File.WriteAllText(Path.Combine(finalDir, "nowy.txt"), "tekstos");
            File.WriteAllText(Path.Combine(finalDir, "pliczek.txt"), "tekstos");

            Dictionary<string, string> conflicts =
                Execute.MoveNewFilesAndCollectConflicts(operationContext.TempDir, finalDir);

            string[] finalDirArray = Directory.GetFiles(finalDir).Select(file => Path.GetFileName(file)).ToArray();

            Assert.Contains("plik_test_1.txt", finalDirArray);
            Assert.Contains("plik_test_2.txt", finalDirArray);
            Assert.Contains("plik_test_3.txt", finalDirArray);
            Assert.Contains("pliczek.txt", finalDirArray);
            Assert.HasCount(6, finalDirArray);
            Assert.HasCount(0, conflicts);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
            
            if (Directory.Exists(finalDir))
                Directory.Delete(finalDir, true);
        }
    }

    [TestMethod]
    public void MoveConflictsTestOverWrite()
    {
        string finalDir = "";
        
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["plik_test_1.txt", "plik_test_2.txt", "plik_test_3.txt"]),
            Dir = "final_dir_name"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        //Prepare temp and final dir

        try
        {
            foreach (string file in operationInput.InputFiles)
            {
                File.Copy(file, Files.PrepareTempPath(operationContext.TempDir, file, operationDefinition.Extension));
            }

            finalDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + $"_{operationInput.Dir}");
            Directory.CreateDirectory(finalDir);

            File.WriteAllText(Path.Combine(finalDir, "plik_test_2.txt"), "wczesniejszy 2");
            File.WriteAllText(Path.Combine(finalDir, "plik_test_3.txt"), "wczesniejszy 3");
            File.WriteAllText(Path.Combine(finalDir, "pliczek.txt"), "tekstos");

            Dictionary<string, string> conflicts =
                Execute.MoveNewFilesAndCollectConflicts(operationContext.TempDir, finalDir);
            Execute.MoveConflicts(conflicts, true);

            string[] finalDirArray = Directory.GetFiles(finalDir).Select(file => Path.GetFileName(file)).ToArray();

            Assert.Contains("plik_test_1.txt", finalDirArray);
            Assert.Contains("plik_test_2.txt", finalDirArray);
            Assert.Contains("plik_test_3.txt", finalDirArray);
            Assert.HasCount(4, finalDirArray);

            string text = File.ReadAllText(Path.Combine(finalDir, "plik_test_2.txt"));
            string text2 = File.ReadAllText(Path.Combine(finalDir, "plik_test_3.txt"));

            Assert.Contains("nowy", text);
            Assert.Contains("nowy", text2);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
            
            if (Directory.Exists(finalDir))
                Directory.Delete(finalDir, true);
        }
    }
    
    [TestMethod]
    public void MoveConflictsTestNoOverWrite()
    {
        string finalDir = "";
        
        OperationInput operationInput = new OperationInput()
        {
            InputFiles = TestHelper.SetInputPaths(["plik_test_1.txt", "plik_test_2.txt", "plik_test_3.txt"]),
            Dir = "final_dir_name"
        };
        
        OperationDefinition operationDefinition = new OperationDefinition()
        {
            Extension = ".txt"
        };
        
        OperationContext operationContext = new OperationContext()
        {
            TempDir = Files.PrepareTempDir()
        };
        
        //Prepare temp and final dir

        try
        {
            foreach (string file in operationInput.InputFiles)
            {
                File.Copy(file, Files.PrepareTempPath(operationContext.TempDir, file, operationDefinition.Extension));
            }

            finalDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + $"_{operationInput.Dir}");
            Directory.CreateDirectory(finalDir);

            File.WriteAllText(Path.Combine(finalDir, "plik_test_2.txt"), "wczesniejszy 2");
            File.WriteAllText(Path.Combine(finalDir, "plik_test_3.txt"), "wczesniejszy 3");
            File.WriteAllText(Path.Combine(finalDir, "pliczek.txt"), "tekstos");

            Dictionary<string, string> conflicts =
                Execute.MoveNewFilesAndCollectConflicts(operationContext.TempDir, finalDir);
            Execute.MoveConflicts(conflicts, false);

            string[] finalDirArray = Directory.GetFiles(finalDir).Select(file => Path.GetFileName(file)).ToArray();

            Assert.Contains("plik_test_1.txt", finalDirArray);
            Assert.Contains("plik_test_2.txt", finalDirArray);
            Assert.Contains("plik_test_2_1.txt", finalDirArray);
            Assert.Contains("plik_test_3.txt", finalDirArray);
            Assert.Contains("plik_test_3_1.txt", finalDirArray);
            Assert.HasCount(6, finalDirArray);

            string text = File.ReadAllText(Path.Combine(finalDir, "plik_test_2.txt"));
            string text2 = File.ReadAllText(Path.Combine(finalDir, "plik_test_3.txt"));
            string text3 = File.ReadAllText(Path.Combine(finalDir, "plik_test_2_1.txt"));
            string text4 = File.ReadAllText(Path.Combine(finalDir, "plik_test_3_1.txt"));

            Assert.Contains("wczesniejszy", text);
            Assert.Contains("wczesniejszy", text2);
            Assert.Contains("nowy", text3);
            Assert.Contains("nowy", text4);
        }
        finally
        {
            if (Directory.Exists(operationContext.TempDir))
                Directory.Delete(operationContext.TempDir, true);
            
            if (Directory.Exists(finalDir))
                Directory.Delete(finalDir, true);
        }
    }
}