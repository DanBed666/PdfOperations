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
}