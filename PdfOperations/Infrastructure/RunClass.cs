using System.Diagnostics;

namespace PdfOperations;

public class RunClass
{
    public static void Run(string exe, List<string> arguments, params int [] allowedExitCodes)
    {
        Run(exe, arguments, Timeout.InfiniteTimeSpan, allowedExitCodes);
    }

    public static void Run(string exe, List<string> arguments, TimeSpan timeout,
        params int[] allowedExitCodes)
    {
        ValidateTimeout(timeout);
        var info = new ProcessStartInfo
        {
            FileName = exe,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        foreach (string argument in arguments)
        {
            info.ArgumentList.Add(argument);
        }
        
        Console.WriteLine(Messages.RunningCommand);
        Console.WriteLine($"{info.FileName} {string.Join(" ",  info.ArgumentList)}");
        
        using Process process = Process.Start(info)!;
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        CompleteProcess(process, timeout, errorTask);
        string error = errorTask.GetAwaiter().GetResult();

        if (allowedExitCodes.Length == 0)
            allowedExitCodes = [0];
        
        if (!allowedExitCodes.Contains(process.ExitCode))
        {
            throw new InvalidOperationException($"{Messages.OperationFailed} {process.ExitCode}: {error}");
        }
    }

    public static void RunFile(string path)
    {
        var run = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true,
        };
        
        using Process process = Process.Start(run)!;
        //process.WaitForExit();
    }
    
    public static void RunFiles(string [] files)
    {
        foreach (string file in files)
        {
            var run = new ProcessStartInfo
            {
                FileName = file,
                UseShellExecute = true,
            };
            
            using Process process = Process.Start(run)!;
            //process.WaitForExit();
        }
    }
    
    public static void RunFiles(string [] files, string app)
    {
        foreach (string file in files)
        {
            var run = new ProcessStartInfo
            {
                FileName = app,
                Arguments = $"\"{file}\"",
                UseShellExecute = true,
            };
            
            using Process process = Process.Start(run)!;
            //process.WaitForExit();
        }
    }
    
    public static void RunFilesDraw(string [] files)
    {
        string soffice = Path.Combine(
            AppContext.BaseDirectory,
            "tools",
            "libreoffice",
            "program",
            "soffice.exe"
        );

        string profile = Path.Combine(AppContext.BaseDirectory, "lo-profile");
        Directory.CreateDirectory(profile);
        string profileUri = new Uri(profile).AbsoluteUri;

        foreach (string file in files)
        {
            var psi = new ProcessStartInfo
            {
                FileName = soffice,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            psi.ArgumentList.Add($"-env:UserInstallation={profileUri}");
            psi.ArgumentList.Add("--draw");
            psi.ArgumentList.Add(file);

            Process.Start(psi);
        }
    }
    
    public static string RunWithOutput(string exe, params string[] arguments)
    {
        return RunWithOutput(exe, Timeout.InfiniteTimeSpan, arguments);
    }

    public static string RunWithOutput(string exe, TimeSpan timeout, params string[] arguments)
    {
        ValidateTimeout(timeout);
        var info = new ProcessStartInfo
        {
            FileName = exe,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (string argument in arguments)
        {
            info.ArgumentList.Add(argument);
        }
        
        using Process process = Process.Start(info)!;
        Console.WriteLine($"{info.FileName} {string.Join(" ",  info.ArgumentList)}");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        CompleteProcess(process, timeout, outputTask, errorTask);
        string output = outputTask.GetAwaiter().GetResult();
        string error = errorTask.GetAwaiter().GetResult();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"{Messages.OperationFailed} {process.ExitCode}: {error}");
        }
        
        return output;
    }

    private static void ValidateTimeout(TimeSpan timeout)
    {
        if (timeout != Timeout.InfiniteTimeSpan
            && (timeout <= TimeSpan.Zero || timeout.TotalMilliseconds > int.MaxValue))
            throw new ArgumentOutOfRangeException(nameof(timeout));
    }

    private static void CompleteProcess(Process process, TimeSpan timeout, params Task[] readTasks)
    {
        // Drain redirected streams concurrently and include them in the same deadline.
        Task completion = Task.WhenAll([process.WaitForExitAsync(), ..readTasks]);
        try
        {
            completion.WaitAsync(timeout).GetAwaiter().GetResult();
        }
        catch (TimeoutException)
        {
            List<Exception> cleanupErrors = [];
            // Observe stream failures even if process termination completes after this method.
            _ = completion.ContinueWith(task => { _ = task.Exception; },
                CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException) when (process.HasExited)
            {
                // The process exited between the timeout and Kill.
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or AggregateException)
            {
                cleanupErrors.Add(ex);
            }

            // Waiting only for the launcher does not wait for its descendants' output pipes.
            try
            {
                completion.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            }
            catch (Exception ex) when (ex is TimeoutException or IOException or InvalidOperationException)
            {
                cleanupErrors.Add(ex);
            }
            throw new TimeoutException(
                $"Process '{Path.GetFileName(process.StartInfo.FileName)}' exceeded the time limit of {timeout.TotalSeconds:0.###} seconds.",
                cleanupErrors.Count == 0 ? null
                    : new AggregateException("Process cleanup did not finish cleanly.", cleanupErrors));
        }
    }
}
