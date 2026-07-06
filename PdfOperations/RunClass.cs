using System.Diagnostics;

namespace PdfOperations;

public class RunClass
{
    public static void Run(string exe, params  string[] arguments)
    {
        var info = new ProcessStartInfo
        {
            FileName = exe,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        foreach (string argument in arguments)
        {
            info.ArgumentList.Add(argument);
        }
        
        using Process process = Process.Start(info)!;
        process.WaitForExit();
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
        
        return output;
    }
}