namespace PdfOperations;

public static class LibreOfficeDiskGuard
{
    public const long MaxTemporaryBytes = 2L * 1024 * 1024 * 1024;
    public const long MinimumFreeBytes = 2L * 1024 * 1024 * 1024;

    public static void Check(string[] directories,
        long maxTemporaryBytes = MaxTemporaryBytes,
        long minimumFreeBytes = MinimumFreeBytes)
    {
        foreach (string root in directories
                     .Select(path => Path.GetPathRoot(Path.GetFullPath(path))!)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (new DriveInfo(root).AvailableFreeSpace < minimumFreeBytes)
                throw new IOException(
                    "Not enough free space for LibreOffice conversion.");
        }

        long total = 0;
        var pending = new Stack<string>(directories);
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = false,
            IgnoreInaccessible = false,
            AttributesToSkip = FileAttributes.ReparsePoint
        };

        while (pending.Count > 0)
        {
            string directory = pending.Pop();
            FileSystemInfo[] entries;

            try
            {
                entries = new DirectoryInfo(directory)
                    .GetFileSystemInfos("*", options);
            }
            catch (DirectoryNotFoundException)
            {
                continue;
            }

            foreach (FileSystemInfo entry in entries)
            {
                if (entry is DirectoryInfo subdirectory)
                {
                    pending.Push(subdirectory.FullName);
                    continue;
                }

                if (entry is not FileInfo file)
                    continue;

                try
                {
                    file.Refresh();
                    total += file.Length;
                }
                catch (IOException ex) when (
                    ex is FileNotFoundException or DirectoryNotFoundException)
                {
                    continue;
                }

                if (total > maxTemporaryBytes)
                    throw new IOException(
                        "LibreOffice temporary files exceeded the size limit.");
            }
        }
    }
}