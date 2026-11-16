namespace PdfOperations.Gui.Services;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public sealed class InstanceCoordinator : IDisposable
{
    private readonly string gatePath;
    private readonly string leasePath;
    private readonly Func<bool> isUpdaterRunning;
    private readonly TimeSpan updaterWait;
    private FileStream? lease;
    private bool disposed;

    public static InstanceCoordinator? Current { get; private set; }

    public InstanceCoordinator(string applicationRoot, string? updaterExecutable = null)
        : this(applicationRoot,
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            () => IsUpdaterRunning(string.IsNullOrWhiteSpace(updaterExecutable)
                ? Path.Combine(applicationRoot, "Update.exe") : updaterExecutable),
            TimeSpan.FromSeconds(10))
    {
    }

    internal InstanceCoordinator(string applicationRoot, string stateRoot,
        Func<bool> isUpdaterRunning, TimeSpan updaterWait)
    {
        ArgumentNullException.ThrowIfNull(isUpdaterRunning);
        if (updaterWait < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(updaterWait));
        this.isUpdaterRunning = isUpdaterRunning;
        this.updaterWait = updaterWait;
        string directory = GetCoordinationDirectory(applicationRoot, stateRoot);
        Directory.CreateDirectory(directory);
        gatePath = Path.Combine(directory, "gate.lock");
        leasePath = Path.Combine(directory, "instances.lock");

        using var gate = EnterGate();
        WaitForUpdater();
        if (!File.Exists(leasePath))
        {
            using var created = new FileStream(leasePath, FileMode.CreateNew,
                FileAccess.Write, FileShare.None);
        }
        lease = OpenSharedLease();
    }

    internal static string GetCoordinationDirectory(string applicationRoot, string stateRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(stateRoot);
        string identity = Path.TrimEndingDirectorySeparator(Path.GetFullPath(applicationRoot))
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).ToUpperInvariant();
        string key = System.Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)));
        return Path.Combine(stateRoot, "PdfOperations", "UpdateCoordination", key);
    }

    public static InstanceCoordinator? OpenApplication(string root, string? updaterExecutable = null)
    {
        if (Current is not null)
            throw new InvalidOperationException("Application instance already registered.");
        try
        {
            Current = new InstanceCoordinator(root, updaterExecutable);
            return Current;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                                       or InvalidOperationException or Win32Exception)
        {
            MessageBox(IntPtr.Zero,
                "Nie mo\u017cna teraz uruchomi\u0107 programu. / Cannot start the application now.\n\n" +
                ex.Message, "PdfOperations", 0x30);
            return null;
        }
    }

    // Call on the UI thread after checking operations. Velopack starts Update.exe before exiting.
    public bool TryApply(Action applyAndExit)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentNullException.ThrowIfNull(applyAndExit);
        using var gate = EnterGate();
        WaitForUpdater();
        lease!.Dispose();
        lease = null;
        FileStream? exclusive = null;
        try
        {
            try
            {
                exclusive = new FileStream(leasePath, FileMode.Open,
                    FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex) when (IsSharingViolation(ex))
            {
                return false;
            }

            // No persistent marker: OS locks and the live updater protect the handoff.
            applyAndExit();
            return true;
        }
        finally
        {
            exclusive?.Dispose();
            try { lease = OpenSharedLease(); }
            catch (Exception ex)
            {
                // Continuing without a lease would hide a live instance from other updaters.
                Environment.FailFast("Could not restore the application instance lease.", ex);
            }
        }
    }

    private void WaitForUpdater()
    {
        var timer = Stopwatch.StartNew();
        while (isUpdaterRunning())
        {
            if (timer.Elapsed >= updaterWait)
                throw new InvalidOperationException(
                    "Aktualizacja tej kopii programu nadal trwa. Spr\u00f3buj ponownie po jej zako\u0144czeniu. / " +
                    "This copy is still being updated. Try again when the updater finishes.");
            Thread.Sleep(50);
        }
    }

    internal static bool IsUpdaterRunning(string executablePath)
    {
        string expected = Path.GetFullPath(executablePath);
        Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(expected));
        try
        {
            foreach (var process in processes)
            {
                try
                {
                    if (process.HasExited) continue;
                    string? actual = process.MainModule?.FileName;
                    if (actual is null)
                    {
                        if (process.HasExited) continue;
                        throw new InvalidOperationException("Cannot determine the updater executable path.");
                    }
                    if (string.Equals(Path.GetFullPath(actual), expected, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
                {
                    // A process can disappear between enumeration and reading its executable path.
                    if (process.HasExited) continue;
                    throw new InvalidOperationException(
                        "Nie mo\u017cna sprawdzi\u0107 procesu aktualizatora. / Cannot inspect the updater process.", ex);
                }
            }
            return false;
        }
        finally
        {
            foreach (var process in processes) process.Dispose();
        }
    }

    private FileStream OpenSharedLease() =>
        new(leasePath, FileMode.Open, FileAccess.Read, FileShare.Read);

    private FileStream EnterGate()
    {
        var timer = Stopwatch.StartNew();
        while (true)
        {
            try
            {
                return new FileStream(gatePath, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex) when (IsSharingViolation(ex) && timer.ElapsedMilliseconds < 2000)
            {
                Thread.Sleep(25);
            }
        }
    }

    private static bool IsSharingViolation(IOException ex) =>
        (ex.HResult & 0xFFFF) is 32 or 33;

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        lease?.Dispose();
        if (ReferenceEquals(Current, this)) Current = null;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MessageBoxW")]
    private static extern int MessageBox(IntPtr owner, string text, string caption, uint type);
}
