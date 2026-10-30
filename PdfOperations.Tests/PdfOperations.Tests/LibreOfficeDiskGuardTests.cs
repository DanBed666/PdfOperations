using System.Diagnostics;

namespace PdfOperations.Tests;

[TestClass]
public class LibreOfficeDiskGuardTests
{
    [TestMethod]
    public void EmptyDirectoryIsAllowed()
    {
        WithDirectory(root => LibreOfficeDiskGuard.Check([root], 0, 0));
    }

    [TestMethod]
    public void MissingDirectoryIsAllowed()
    {
        WithDirectory(root =>
            LibreOfficeDiskGuard.Check([Path.Combine(root, "missing")], 0, 0));
    }

    [TestMethod]
    public void DeletedSubdirectoryDoesNotPreventCheckingRemainingFiles()
    {
        WithDirectory(root =>
        {
            string deleted = Directory.CreateDirectory(Path.Combine(root, "deleted")).FullName;
            Directory.Delete(deleted);
            File.WriteAllBytes(Path.Combine(root, "remaining.tmp"), new byte[11]);
            Assert.Throws<IOException>(() => LibreOfficeDiskGuard.Check([deleted, root], 10, 0));
        });
    }

    [TestMethod]
    public void ExactSizeLimitIncludesNestedFiles()
    {
        WithDirectory(root =>
        {
            string nested = Directory.CreateDirectory(Path.Combine(root, "nested")).FullName;
            File.WriteAllBytes(Path.Combine(root, "first.tmp"), new byte[4]);
            File.WriteAllBytes(Path.Combine(nested, "second.tmp"), new byte[6]);
            LibreOfficeDiskGuard.Check([root], 10, 0);
        });
    }

    [TestMethod]
    public void CombinedNestedSizeExceedingLimitIsRejected()
    {
        WithDirectory(root =>
        {
            string nested = Directory.CreateDirectory(Path.Combine(root, "nested")).FullName;
            File.WriteAllBytes(Path.Combine(root, "first.tmp"), new byte[4]);
            File.WriteAllBytes(Path.Combine(nested, "second.tmp"), new byte[7]);
            Assert.Throws<IOException>(() => LibreOfficeDiskGuard.Check([root], 10, 0));
        });
    }

    [TestMethod]
    public void LimitIsSharedBetweenMonitoredDirectories()
    {
        WithDirectory(root =>
        {
            string work = Directory.CreateDirectory(Path.Combine(root, "work")).FullName;
            string output = Directory.CreateDirectory(Path.Combine(root, "output")).FullName;
            File.WriteAllBytes(Path.Combine(work, "profile.tmp"), new byte[6]);
            File.WriteAllBytes(Path.Combine(output, "result.txt"), new byte[5]);
            Assert.Throws<IOException>(() => LibreOfficeDiskGuard.Check([work, output], 10, 0));
        });
    }

    [TestMethod]
    public void InsufficientFreeSpaceIsRejectedWithoutFillingDisk()
    {
        WithDirectory(root => Assert.Throws<IOException>(() =>
            LibreOfficeDiskGuard.Check([root], long.MaxValue, long.MaxValue)));
    }

    [TestMethod]
    public void GuardFailurePreventsProcessStart()
    {
        var expected = new IOException("Test disk limit exceeded.");
        IOException actual = Assert.Throws<IOException>(() =>
            RunClass.RunGuarded("missing-program.exe", [], TimeSpan.FromSeconds(5),
                CancellationToken.None, null, () => throw expected));
        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void SizeLimitStopsRunningProcessAndRemovesTemporaryFiles()
    {
        RequireWindows();
        string root = Directory.CreateTempSubdirectory("DiskGuardTest-").FullName;
        int checks = 0;
        IOException? expected = null;
        var watch = Stopwatch.StartNew();
        try
        {
            IOException actual = Assert.Throws<IOException>(() =>
                TemporaryDirectoryCleanup.Run(root, () =>
                    RunClass.RunGuarded(Shell, WaitingArguments(), TimeSpan.FromSeconds(30),
                        CancellationToken.None, root, () =>
                        {
                            // The first check runs before launch; the second runs while waiting.
                            if (++checks == 2)
                                File.WriteAllBytes(Path.Combine(root, "growing.tmp"), new byte[11]);
                            try { LibreOfficeDiskGuard.Check([root], 10, 0); }
                            catch (IOException ex) { expected = ex; throw; }
                        })));
            Assert.IsNotNull(expected);
            Assert.AreSame(expected, actual);
            Assert.IsNull(actual.Data[TemporaryDirectoryCleanup.ErrorDataKey]);
            Assert.IsFalse(Directory.Exists(root));
            Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(10));
        }
        finally { TemporaryDirectoryCleanup.Delete(root); }
    }

    [TestMethod]
    public void GuardedProcessStillSupportsUserCancellation()
    {
        RequireWindows();
        using var cancellation = new CancellationTokenSource();
        int checks = 0;
        var watch = Stopwatch.StartNew();
        OperationCanceledException actual = Assert.Throws<OperationCanceledException>(() =>
            RunClass.RunGuarded(Shell, WaitingArguments(), TimeSpan.FromSeconds(30),
                cancellation.Token, null, () =>
                {
                    if (++checks == 2)
                        cancellation.Cancel();
                }));
        Assert.AreEqual(cancellation.Token, actual.CancellationToken);
        Assert.IsNull(actual.InnerException);
        Assert.IsTrue(checks >= 2);
        Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(10));
    }

    [TestMethod]
    public void GuardedProcessStillSupportsTimeout()
    {
        RequireWindows();
        int checks = 0;
        var watch = Stopwatch.StartNew();
        TimeoutException actual = Assert.Throws<TimeoutException>(() =>
            RunClass.RunGuarded(Shell, WaitingArguments(), TimeSpan.FromSeconds(1),
                CancellationToken.None, null, () => checks++));
        Assert.IsNull(actual.InnerException);
        Assert.IsTrue(checks >= 1);
        Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(10));
    }

    [TestMethod]
    public void PrivateTempDirectoryIsPassedToChildProcess()
    {
        RequireWindows();
        WithDirectory(root =>
        {
            string originalTemp = Environment.GetEnvironmentVariable("TEMP") ?? "";
            string originalTmp = Environment.GetEnvironmentVariable("TMP") ?? "";
            string privateTemp = Directory.CreateDirectory(Path.Combine(root, "private temp")).FullName;
            string script = "if ($env:TEMP -ne $env:TMP) { exit 9 }; " +
                "[IO.File]::WriteAllText([IO.Path]::Combine($env:TEMP, 'environment.txt'), 'ok')";
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));
            string powershell = Path.Combine(Environment.SystemDirectory,
                "WindowsPowerShell", "v1.0", "powershell.exe");
            RunClass.RunGuarded(powershell,
                ["-NoProfile", "-NonInteractive", "-EncodedCommand", encoded],
                TimeSpan.FromSeconds(10), CancellationToken.None, privateTemp,
                () => LibreOfficeDiskGuard.Check([root], 1024, 0));
            Assert.AreEqual("ok", File.ReadAllText(Path.Combine(privateTemp, "environment.txt")).Trim());
            Assert.AreEqual(originalTemp, Environment.GetEnvironmentVariable("TEMP") ?? "");
            Assert.AreEqual(originalTmp, Environment.GetEnvironmentVariable("TMP") ?? "");
        });
    }

    private static string Shell => Path.Combine(Environment.SystemDirectory, "cmd.exe");

    private static List<string> WaitingArguments() =>
        ["/d", "/c", "ping -n 60 127.0.0.1 > nul"];

    private static void RequireWindows()
    {
        if (!OperatingSystem.IsWindows())
            Assert.Inconclusive("Requires the Windows command interpreter.");
    }

    private static void WithDirectory(Action<string> action)
    {
        string root = Directory.CreateTempSubdirectory("DiskGuardTest-").FullName;
        try { action(root); }
        finally { TemporaryDirectoryCleanup.Delete(root); }
    }
}
