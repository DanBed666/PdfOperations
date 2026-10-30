using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace PdfOperations;

public static class WordConversionWorker
{
    private const string Command = "--word-to-pdf-worker";

    public static void ConvertFile(FileJob file)
    {
        ConvertFile(file, CancellationToken.None);
    }

    public static void ConvertFile(FileJob file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string input = Path.GetFullPath(file.InputFile);
        string output = Path.GetFullPath(file.TempPath);
        if (!File.Exists(input))
            throw new FileNotFoundException("Input document was not found.", input);
        if (string.Equals(input, output, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The PDF output must not replace the input document.");

        string stateDirectory = Directory.CreateTempSubdirectory("PdfOperationsWord-").FullName;
        TemporaryDirectoryCleanup.Run(stateDirectory, () =>
        {
            (string executable, List<string> arguments) = GetWorkerCommand();
            arguments.AddRange([Command, input, output, stateDirectory]);
            try
            {
                RunClass.Run(executable, arguments, OperationTimeouts.WordToPdf, cancellationToken);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested
                && File.Exists(Path.Combine(stateDirectory, "word-starting"))
                && !File.Exists(Path.Combine(stateDirectory, "word-process.json")))
            {
                ex.Data[TemporaryDirectoryCleanup.ErrorDataKey] =
                    "Word startup was cancelled before its process could be identified. " +
                    "The worker was stopped, but an unidentified Word process may still be running.";
                throw;
            }
            catch (TimeoutException ex) when (!File.Exists(Path.Combine(stateDirectory, "word-process.json")))
            {
                throw new TimeoutException(
                    "Word startup exceeded its time limit before a dedicated Word process could be identified. " +
                    "The worker was stopped; unidentified Word processes were left untouched.", ex);
            }
            catch (InvalidOperationException ex) when (File.Exists(Path.Combine(stateDirectory, "error.txt")))
            {
                throw new InvalidOperationException(
                    File.ReadAllText(Path.Combine(stateDirectory, "error.txt")), ex);
            }
            Convert.ValidatePdfOutput(output);
        }, () => StopRegisteredWord(stateDirectory));
    }

    public static bool TryRun(string[] args, out int exitCode)
    {
        exitCode = 0;
        if (args.Length == 0 || args[0] != Command)
            return false;
        if (args.Length != 4)
        {
            Console.Error.WriteLine("Expected input, output and worker state directory.");
            exitCode = 1;
            return true;
        }

        try
        {
            ConvertInWorker(args[1], args[2], args[3]);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            try { File.WriteAllText(Path.Combine(args[3], "error.txt"), ex.ToString()); }
            catch (IOException error) { Console.Error.WriteLine(error.Message); }
            catch (UnauthorizedAccessException error) { Console.Error.WriteLine(error.Message); }
            exitCode = 1;
        }
        return true;
    }

    private static (string Executable, List<string> Arguments) GetWorkerCommand()
    {
        // Self-hosting also works when the GUI is published as a single executable.
        Assembly? entry = Assembly.GetEntryAssembly();
        string? name = entry?.GetName().Name;
        if (name is "PdfOperations" or "PdfOperations.Gui")
        {
            string host = Environment.ProcessPath
                ?? throw new InvalidOperationException("Cannot locate the application executable.");
            if (Path.GetFileNameWithoutExtension(host).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
                return (host, [entry!.Location]);
            return (host, []);
        }

        // Project references copy the console apphost to the test output directory.
        string consoleHost = Path.Combine(AppContext.BaseDirectory, "PdfOperations.exe");
        if (!File.Exists(consoleHost))
            throw new FileNotFoundException("Cannot locate the Word conversion worker.", consoleHost);
        return (consoleHost, []);
    }

    private static void ConvertInWorker(string input, string output, string stateDirectory)
    {
        if (!File.Exists(input))
            throw new FileNotFoundException("Input document was not found.", input);
        Type wordType = Type.GetTypeFromProgID("Word.Application")
            ?? throw new InvalidOperationException("Microsoft Word is not installed or cannot be started.");
        HashSet<int> existingWordIds = [];
        foreach (Process process in Process.GetProcessesByName("WINWORD"))
        {
            using (process)
                existingWordIds.Add(process.Id);
        }

        dynamic? word = null;
        dynamic? documents = null;
        dynamic? document = null;
        bool ownsWord = false;
        try
        {
            File.WriteAllText(Path.Combine(stateDirectory, "word-starting"), "Starting Word");
            word = Activator.CreateInstance(wordType)!;
            ownsWord = true;
            documents = word.Documents;
            // A blank document provides a window handle before opening the user's file.
            document = documents.Add();
            RegisterWord(document, existingWordIds, stateDirectory, ref ownsWord);
            document.Close(false);
            ReleaseCom(document);
            document = null;

            word.Visible = false;
            word.DisplayAlerts = 0;
            word.AutomationSecurity = 3;
            document = documents.Open(input, ConfirmConversions: false, ReadOnly: true,
                AddToRecentFiles: false, Visible: false);
            document.ExportAsFixedFormat(output, 17);
        }
        finally
        {
            try
            {
                if (document is not null)
                    document.Close(false);
            }
            finally
            {
                try
                {
                    if (ownsWord)
                        word!.Quit(false);
                }
                finally
                {
                    ReleaseCom(document);
                    ReleaseCom(documents);
                    ReleaseCom(word);
                }
            }
        }
    }

    private static void RegisterWord(dynamic document, HashSet<int> existingIds, string stateDirectory,
        ref bool ownsWord)
    {
        dynamic windows = document.Windows;
        dynamic? window = null;
        try
        {
            window = windows[1];
            uint thread = GetWindowThreadProcessId(new IntPtr((int)window.Hwnd), out uint id);
            if (existingIds.Contains((int)id))
            {
                ownsWord = false;
                throw new InvalidOperationException("Word reused an existing process; conversion was stopped.");
            }
            if (thread == 0 || id == 0)
                throw new InvalidOperationException("Cannot identify a dedicated Word process.");
            using Process process = Process.GetProcessById((int)id);
            if (!process.ProcessName.Equals("WINWORD", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Unexpected Word process identity.");
            var identity = new WordProcessIdentity(process.Id, process.StartTime.ToUniversalTime().Ticks);
            string path = Path.Combine(stateDirectory, "word-process.json");
            File.WriteAllText(path + ".tmp", JsonSerializer.Serialize(identity));
            File.Move(path + ".tmp", path);
        }
        finally
        {
            ReleaseCom(window);
            ReleaseCom(windows);
        }
    }

    private static void StopRegisteredWord(string stateDirectory)
    {
        string path = Path.Combine(stateDirectory, "word-process.json");
        if (!File.Exists(path))
            return;
        WordProcessIdentity identity = JsonSerializer.Deserialize<WordProcessIdentity>(File.ReadAllText(path))
            ?? throw new InvalidOperationException("Invalid Word process identity.");
        Process process;
        try { process = Process.GetProcessById(identity.Id); }
        catch (ArgumentException) { return; }
        using (process)
        {
            try
            {
                // Check start time as well as PID to avoid killing a reused process ID.
                if (process.HasExited
                    || !process.ProcessName.Equals("WINWORD", StringComparison.OrdinalIgnoreCase)
                    || process.StartTime.ToUniversalTime().Ticks != identity.StartTimeUtcTicks)
                    return;
                process.Kill();
                if (!process.WaitForExit(5000))
                    throw new InvalidOperationException("The Word conversion process could not be stopped.");
            }
            catch (InvalidOperationException) when (process.HasExited) { }
        }
    }

    private static void ReleaseCom(object? value)
    {
        if (value is not null && Marshal.IsComObject(value))
            Marshal.FinalReleaseComObject(value);
    }

    private sealed record WordProcessIdentity(int Id, long StartTimeUtcTicks);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
}
