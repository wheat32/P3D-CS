namespace P3D;

public static class DebugFileWatcher
{
    private static readonly List<String> _changedFiles = [];
    private static FileSystemWatcher? _watcher;
    private static bool _isWatching;

    public static void TriggerReload()
    {
        lock (_changedFiles)
        {
            String projectPath = GetProjectPath();
            String targetPath = AppDomain.CurrentDomain.BaseDirectory;

            foreach (String changedFile in _changedFiles)
            {
                String relativeFile = changedFile[(projectPath.Length + 1)..];
                if (File.Exists(relativeFile) == false)
                {
                    continue;
                }
                String targetFile = Path.Combine(targetPath, relativeFile);
                File.Copy(changedFile, targetFile, true);
            }

            _changedFiles.Clear();
        }
    }

    private static String GetProjectPath()
    {
        return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent!.Parent!.FullName;
    }

    public static void StartWatching()
    {
        if (_isWatching == true)
        {
            return;
        }
        _isWatching = true;

        String projectPath = GetProjectPath();
        String contentPath = Path.Combine(projectPath, "Content");

        _watcher = new FileSystemWatcher
        {
            Path = contentPath,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess,
            IncludeSubdirectories = true
        };
        _watcher.Changed += OnChanged;
        _watcher.EnableRaisingEvents = true;
    }

    private static void OnChanged(Object source, FileSystemEventArgs e)
    {
        lock (_changedFiles)
        {
            String file = e.FullPath;
            if (File.Exists(file) == true)
            {
                if (_changedFiles.Contains(file) == false)
                {
                    Logger.Debug("File changed: " + file);
                    _changedFiles.Add(file);
                }
            }
            else if (file.EndsWith("~") == true)
            {
                String? dir = Path.GetDirectoryName(file);
                if (dir == null)
                {
                    return;
                }
                Logger.Debug($"Single file can't be watched. Watch folder \"{dir}\" instead.");
                foreach (String dirFile in Directory.GetFiles(dir))
                {
                    if (_changedFiles.Contains(dirFile) == false)
                    {
                        Logger.Debug("File changed: " + dirFile);
                        _changedFiles.Add(dirFile);
                    }
                }
            }
        }
    }
}
