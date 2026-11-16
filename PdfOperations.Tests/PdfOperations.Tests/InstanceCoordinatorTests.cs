using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using PdfOperations.Gui.Services;

namespace PdfOperations.Tests;

[TestClass]
public sealed class InstanceCoordinatorTests
{
    private string root = "";
    private string AppRoot => Path.Combine(root, "app");
    private string StateRoot => Path.Combine(root, "user-data");
    private InstanceCoordinator Open(string? appRoot = null, Func<bool>? running = null,
        TimeSpan? wait = null) => new(appRoot ?? AppRoot, StateRoot,
            running ?? (() => false), wait ?? TimeSpan.Zero);

    [TestInitialize]
    public void Initialize()
    {
        root = Path.Combine(Path.GetTempPath(), "PdfOperationsInstances-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(AppRoot);
    }

    [TestCleanup]
    public void Cleanup() => TemporaryDirectoryCleanup.Delete(root);

    [TestMethod]
    public void MultipleInstancesBlockApplyUntilOtherInstancesClose()
    {
        using var first = Open();
        bool called = false;
        using (var second = Open())
        {
            Assert.IsFalse(first.TryApply(() => called = true));
            Assert.IsFalse(second.TryApply(() => called = true));
            Assert.IsFalse(called);
        }
        Assert.IsTrue(first.TryApply(() => called = true));
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void ApplyFailureRestoresLeaseAndAllowsRetry()
    {
        using var first = Open();
        var failure = new InvalidOperationException("Simulated updater failure.");
        var actual = Assert.Throws<InvalidOperationException>(() => first.TryApply(() => throw failure));
        Assert.AreSame(failure, actual);
        using (var second = Open())
            Assert.IsFalse(second.TryApply(() => Assert.Fail("First instance lost its lease.")));
        Assert.IsTrue(first.TryApply(() => { }));
    }

    [TestMethod]
    public void DifferentCopiesCanUpdateIndependently()
    {
        using var first = Open(Path.Combine(root, "first"));
        using var second = Open(Path.Combine(root, "second"));
        Assert.IsTrue(first.TryApply(() => { }));
        Assert.IsTrue(second.TryApply(() => { }));
    }

    [TestMethod]
    public void EquivalentPathsShareCoordination()
    {
        using var first = Open();
        using var second = Open(AppRoot.ToUpperInvariant() + Path.DirectorySeparatorChar);
        Assert.IsFalse(first.TryApply(() => Assert.Fail("Equivalent paths bypassed the lease.")));
    }

    [TestMethod]
    public void StartupIsBlockedWhileApplyHoldsGate()
    {
        using var instance = Open();
        Assert.IsTrue(instance.TryApply(() =>
            Assert.Throws<IOException>(() => { using var other = Open(); })));
        using var after = Open();
    }

    [TestMethod]
    public void OldPendingMarkersDoNotBlockStartupOrRequireDeletion()
    {
        string directory = Path.Combine(AppRoot, ".update-coordination");
        Directory.CreateDirectory(directory);
        string marker = Path.Combine(directory, "pending-version.txt");
        foreach (string content in new[] { "1.0.1", "", "invalid" })
        {
            File.WriteAllText(marker, content);
            using var instance = Open();
            Assert.IsTrue(instance.TryApply(() => { }));
            Assert.AreEqual(content, File.ReadAllText(marker));
        }
    }

    [TestMethod]
    public void CoordinationLivesOutsideApplicationDirectory()
    {
        using var instance = Open();
        string directory = InstanceCoordinator.GetCoordinationDirectory(AppRoot, StateRoot);
        Assert.IsTrue(directory.StartsWith(StateRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(File.Exists(Path.Combine(directory, "instances.lock")));
        Assert.IsEmpty(Directory.GetFileSystemEntries(AppRoot));
    }

    [TestMethod]
    public void StartupAndCoordinationWorkWithReadOnlyApplicationDirectory()
    {
        var directory = new DirectoryInfo(AppRoot);
        var original = directory.GetAccessControl();
        var restricted = directory.GetAccessControl();
        restricted.AddAccessRule(new FileSystemAccessRule(WindowsIdentity.GetCurrent().User!,
            FileSystemRights.Write, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
            PropagationFlags.None, AccessControlType.Deny));
        bool permissionsChanged = false;
        try
        {
            try
            {
                directory.SetAccessControl(restricted);
                permissionsChanged = true;
            }
            catch (UnauthorizedAccessException)
            {
                Assert.Inconclusive("This environment does not allow setting a test directory ACL.");
            }
            Assert.Throws<UnauthorizedAccessException>(() => File.WriteAllText(Path.Combine(AppRoot, "probe"), "x"));
            using var instance = Open();
            Assert.IsTrue(instance.TryApply(() => { }));
            Assert.IsEmpty(Directory.GetFileSystemEntries(AppRoot));
        }
        finally
        {
            if (permissionsChanged) directory.SetAccessControl(original);
        }
    }

    [TestMethod]
    public void CoordinationWorksForProtectedApplicationRootWithoutWritingThere()
    {
        // System32 is a real protected directory; coordination must only write to StateRoot.
        using var instance = Open(Environment.SystemDirectory);
        Assert.IsTrue(instance.TryApply(() => { }));
        string directory = InstanceCoordinator.GetCoordinationDirectory(Environment.SystemDirectory, StateRoot);
        Assert.IsTrue(File.Exists(Path.Combine(directory, "instances.lock")));
    }

    [TestMethod]
    public void StartupWaitsUntilUpdaterFinishes()
    {
        int checks = 0;
        using var instance = Open(running: () => ++checks < 3, wait: TimeSpan.FromSeconds(2));
        Assert.AreEqual(3, checks);
    }

    [TestMethod]
    public void UpdaterCheckFailureDoesNotReleaseApplicationLease()
    {
        bool fail = false;
        using var first = Open(running: () => fail ? throw new InvalidOperationException("Unavailable") : false);
        fail = true;
        Assert.Throws<InvalidOperationException>(() => first.TryApply(() => Assert.Fail()));
        using var second = Open();
        Assert.IsFalse(second.TryApply(() => Assert.Fail("Lease was lost.")));
    }

    [TestMethod]
    public void KilledUpdaterDoesNotLeavePermanentStartupBlock()
    {
        string executable = CreateUpdaterExecutable(AppRoot);
        Process? updater = null;
        try
        {
            using (var application = Open(running: () => InstanceCoordinator.IsUpdaterRunning(executable)))
                Assert.IsTrue(application.TryApply(() => updater = StartWaitingProcess(executable)));
            Assert.IsNotNull(updater);
            Assert.IsTrue(InstanceCoordinator.IsUpdaterRunning(executable));
            Assert.Throws<InvalidOperationException>(() =>
            {
                using var blocked = Open(running: () => InstanceCoordinator.IsUpdaterRunning(executable));
            });
            updater.Kill(entireProcessTree: true);
            Assert.IsTrue(updater.WaitForExit(10000));
            using var recovered = Open(running: () => InstanceCoordinator.IsUpdaterRunning(executable));
            Assert.IsTrue(recovered.TryApply(() => { }));
            Assert.IsFalse(Directory.Exists(Path.Combine(AppRoot, ".update-coordination")));
        }
        finally
        {
            if (updater is not null)
            {
                StopProcess(updater);
                updater.Dispose();
            }
        }
    }

    [TestMethod]
    public void SameNamedUpdaterOfAnotherCopyDoesNotBlockStartup()
    {
        string executable = CreateUpdaterExecutable(Path.Combine(root, "other-copy"));
        using var updater = StartWaitingProcess(executable);
        try
        {
            string expected = Path.Combine(AppRoot, Path.GetFileName(executable));
            Assert.IsFalse(InstanceCoordinator.IsUpdaterRunning(expected));
            using var instance = Open(running: () => InstanceCoordinator.IsUpdaterRunning(expected));
            Assert.IsTrue(instance.TryApply(() => { }));
        }
        finally { StopProcess(updater); }
    }

    [TestMethod]
    public void DisposedInstanceCannotApply()
    {
        var instance = Open();
        instance.Dispose();
        instance.Dispose();
        Assert.Throws<ObjectDisposedException>(() => instance.TryApply(() => { }));
    }

    [TestMethod]
    public void InvalidArgumentsDoNotReleaseLease()
    {
        using var first = Open();
        Assert.Throws<ArgumentNullException>(() => first.TryApply(null!));
        using var second = Open();
        Assert.IsFalse(second.TryApply(() => Assert.Fail("First instance lost its lease.")));
    }

    private static string CreateUpdaterExecutable(string directory)
    {
        Directory.CreateDirectory(directory);
        string executable = Path.Combine(directory, "UpdaterFixture.exe");
        File.Copy(Path.Combine(Environment.SystemDirectory, "cmd.exe"), executable);
        return executable;
    }

    private static Process StartWaitingProcess(string executable)
    {
        var info = new ProcessStartInfo(executable)
        {
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true
        };
        info.ArgumentList.Add("/d");
        info.ArgumentList.Add("/c");
        info.ArgumentList.Add("echo READY & set /p wait=");
        var process = Process.Start(info)!;
        try
        {
            string? ready = process.StandardOutput.ReadLineAsync().WaitAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
            Assert.AreEqual("READY", ready?.Trim());
            return process;
        }
        catch
        {
            StopProcess(process);
            process.Dispose();
            throw;
        }
    }

    private static void StopProcess(Process process)
    {
        if (!process.HasExited) process.Kill(entireProcessTree: true);
        Assert.IsTrue(process.WaitForExit(10000));
    }
}
