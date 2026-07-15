using System.Diagnostics;

namespace PdfOperations;

public class RunClass
{
    public static void Run(string exe, List<string> arguments)
    {
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
        
        using Process process = Process.Start(info)!;
        process.WaitForExit();
        
        string error = process.StandardError.ReadToEnd();
        
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Operacja zakończona z kodem {process.ExitCode}: {error}");
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
    
    public static void RunFilesDraw(string [] files, string app)
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
    
    public static string RunWithOutput(string exe, params  string[] arguments)
    {
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
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Operacja zakończona z kodem {process.ExitCode}: {error}");
        }
        
        return output;
    }
}