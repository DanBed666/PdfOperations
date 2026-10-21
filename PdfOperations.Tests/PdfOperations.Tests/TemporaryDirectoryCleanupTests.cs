namespace PdfOperations.Tests;

[TestClass]
public class TemporaryDirectoryCleanupTests
{
    [TestMethod]
    public void RunRemovesNestedFilesAfterSuccess()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        try
        {
            TemporaryDirectoryCleanup.Run(directory, () =>
            {
                string nested = Directory.CreateDirectory(Path.Combine(directory, "extracted")).FullName;
                File.WriteAllText(Path.Combine(nested, "document.xml"), "test");
            });
            Assert.IsFalse(Directory.Exists(directory));
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

    [TestMethod]
    public void RunPreservesOperationErrorAndRemovesPartialOutput()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        var original = new TimeoutException("Conversion timed out.");
        try
        {
            TimeoutException actual = Assert.Throws<TimeoutException>(() =>
                TemporaryDirectoryCleanup.Run(directory, () =>
                {
                    File.WriteAllText(Path.Combine(directory, "partial.tmp"), "partial");
                    throw original;
                }));
            Assert.AreSame(original, actual);
            Assert.IsFalse(Directory.Exists(directory));
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

    [TestMethod]
    public void RunStopsProcessBeforeDeletingDirectory()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        FileStream? locked = null;
        try
        {
            TemporaryDirectoryCleanup.Run(directory,
                () => locked = File.Open(Path.Combine(directory, "locked.tmp"), FileMode.Create,
                    FileAccess.ReadWrite, FileShare.None),
                () => locked!.Dispose());
            Assert.IsFalse(Directory.Exists(directory));
        }
        finally
        {
            locked?.Dispose();
            TemporaryDirectoryCleanup.Delete(directory);
        }
    }

    [TestMethod]
    public void RunDeletesDirectoryEvenWhenStoppingProcessFails()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        var stopError = new InvalidOperationException("Cannot stop worker.");
        try
        {
            InvalidOperationException actual = Assert.Throws<InvalidOperationException>(() =>
                TemporaryDirectoryCleanup.Run(directory, () => { }, () => throw stopError));
            Assert.AreSame(stopError, actual);
            Assert.IsFalse(Directory.Exists(directory));
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

    [TestMethod]
    public void RunKeepsOriginalErrorWhenStoppingProcessFails()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        var original = new TimeoutException("Conversion timed out.");
        try
        {
            TimeoutException actual = Assert.Throws<TimeoutException>(() =>
                TemporaryDirectoryCleanup.Run(directory, () => throw original,
                    () => throw new InvalidOperationException("Cannot stop worker.")));
            Assert.AreSame(original, actual);
            StringAssert.Contains((string)actual.Data[TemporaryDirectoryCleanup.ErrorDataKey]!, "Cannot stop worker.");
            Assert.IsFalse(Directory.Exists(directory));
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

    [TestMethod]
    public void RunPreservesAllCleanupErrors()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        var original = new TimeoutException("Conversion timed out.");
        original.Data[TemporaryDirectoryCleanup.ErrorDataKey] = "Earlier cleanup failure.";
        try
        {
            using var locked = File.Open(Path.Combine(directory, "locked.tmp"),
                FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            TimeoutException actual = Assert.Throws<TimeoutException>(() =>
                TemporaryDirectoryCleanup.Run(directory, () => throw original,
                    () => throw new InvalidOperationException("Cannot stop worker.")));
            Assert.AreSame(original, actual);
            string details = (string)actual.Data[TemporaryDirectoryCleanup.ErrorDataKey]!;
            StringAssert.Contains(details, "Earlier cleanup failure.");
            StringAssert.Contains(details, "Cannot stop worker.");
            StringAssert.Contains(details, directory);
        }
        finally { TemporaryDirectoryCleanup.Delete(directory); }
    }

    [TestMethod]
    public void RemovesTemporaryDirectory()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        File.WriteAllText(Path.Combine(directory, "test.tmp"), "test");
        TemporaryDirectoryCleanup.Delete(directory);
        Assert.IsFalse(Directory.Exists(directory));
    }

    [TestMethod]
    public void MissingDirectoryIsAlreadyClean()
    {
        TemporaryDirectoryCleanup.Delete(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
    }

    [TestMethod]
    public void RetriesUntilFileIsUnlocked()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        using var locked = File.Open(Path.Combine(directory, "locked.tmp"),
            FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        Task release = Task.Run(() =>
        {
            Thread.Sleep(350);
            locked.Dispose();
        });
        try
        {
            TemporaryDirectoryCleanup.Delete(directory);
            Assert.IsFalse(Directory.Exists(directory));
        }
        finally
        {
            release.GetAwaiter().GetResult();
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    public void LockedFileDoesNotReplaceOriginalTimeout()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        var original = new TimeoutException("Expected conversion timeout.");
        try
        {
            using var locked = File.Open(Path.Combine(directory, "locked.tmp"),
                FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            TimeoutException actual = Assert.Throws<TimeoutException>(() =>
            {
                try { throw original; }
                finally { TemporaryDirectoryCleanup.Delete(directory, original); }
            });
            Assert.AreSame(original, actual);
            StringAssert.Contains((string)actual.Data[TemporaryDirectoryCleanup.ErrorDataKey]!, directory);
            Assert.IsTrue(Directory.Exists(directory));
        }
        finally { Directory.Delete(directory, true); }
    }

    [TestMethod]
    public void CleanupFailureWithoutOriginalErrorIsReported()
    {
        string directory = Directory.CreateTempSubdirectory("PdfOperationsCleanupTest-").FullName;
        try
        {
            using var locked = File.Open(Path.Combine(directory, "locked.tmp"),
                FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            IOException error = Assert.Throws<IOException>(() => TemporaryDirectoryCleanup.Delete(directory));
            StringAssert.Contains(error.Message, directory);
            Assert.IsNotNull(error.InnerException);
        }
        finally { Directory.Delete(directory, true); }
    }
}
