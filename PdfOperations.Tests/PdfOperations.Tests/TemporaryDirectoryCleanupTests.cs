namespace PdfOperations.Tests;

[TestClass]
public class TemporaryDirectoryCleanupTests
{
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
