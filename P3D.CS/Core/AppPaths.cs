using System.Runtime.InteropServices;

namespace P3D;

/// <summary>
/// Platform-aware base directories for user-writable data.
/// On Linux, follows the XDG Base Directory Specification so that Flatpak and AppImage
/// both write to the correct per-user directories instead of next to the executable.
///
/// XDG mapping:
///   SaveDir        → $XDG_DATA_HOME/Pokemon3D/saves    (default ~/.local/share/Pokemon3D/saves)
///   ConfigDir      → $XDG_CONFIG_HOME/Pokemon3D        (default ~/.config/Pokemon3D)
///   CacheDir       → $XDG_CACHE_HOME/Pokemon3D         (default ~/.cache/Pokemon3D)
///   ScreenshotsDir → $XDG_DATA_HOME/Pokemon3D/screenshots
///
/// On Windows and macOS the original behaviour (paths relative to GamePath) is preserved.
/// </summary>
public static class AppPaths
{
    private const String AppName = "Pokemon3D";

    /// <summary>Per-save-slot game data root.</summary>
    public static String SaveDir { get; } = ResolveSaveDir();

    /// <summary>Global settings: options.dat, Keyboard.dat.</summary>
    public static String ConfigDir { get; } = ResolveConfigDir();

    /// <summary>Logs, crash reports, temp files, PvP debug logs.</summary>
    public static String CacheDir { get; } = ResolveCacheDir();

    /// <summary>Screenshots directory.</summary>
    public static String ScreenshotsDir { get; } = ResolveScreenshotsDir();

    private static String ResolveSaveDir()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            String xdgData = Environment.GetEnvironmentVariable("XDG_DATA_HOME")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
            return Path.Combine(xdgData, AppName, "saves");
        }
        return Path.Combine(GameController.GamePath, "Save");
    }

    private static String ResolveConfigDir()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            String xdgConfig = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
            return Path.Combine(xdgConfig, AppName);
        }
        return Path.Combine(GameController.GamePath, "Save");
    }

    private static String ResolveCacheDir()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            String xdgCache = Environment.GetEnvironmentVariable("XDG_CACHE_HOME")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache");
            return Path.Combine(xdgCache, AppName);
        }
        return GameController.GamePath;
    }

    private static String ResolveScreenshotsDir()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            String xdgData = Environment.GetEnvironmentVariable("XDG_DATA_HOME")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
            return Path.Combine(xdgData, AppName, "screenshots");
        }
        return Path.Combine(GameController.GamePath, "screenshots");
    }
}
