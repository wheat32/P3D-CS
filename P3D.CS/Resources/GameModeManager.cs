namespace P3D;

// TODO Phase 7: full GameModeManager port
public static partial class GameModeManager
{
    public static int GameModeCount => _gameModes.Count;
    public static GameMode? ActiveGameMode => _gameModes.Count > 0 ? _gameModes[0] : null;

    private static List<GameMode> _gameModes = [];

    public static void LoadGameModes() { }
    public static void SetGameModePointer(String gamemodeName) { }
    public static String GetMapPath(String filename) => System.IO.Path.Combine("maps", filename);
    public static String GetPokeFilePath(String filename) => System.IO.Path.Combine("maps", filename);
    public static String GetContentFilePath(String filename, String category) => filename;
    public static String GetScriptPath(String filename) => System.IO.Path.Combine("scripts", filename);
    public static bool ContentFileExists(String path) => System.IO.File.Exists(path);
}

public class GameMode
{
    public static String DefaultLocalizationsPath = @"\Content\Localization\";
    public String DirectoryName = "";
    public String ContentPath = @"\Content\";
    public String LocalizationsPath => DefaultLocalizationsPath;
    public bool IsDefaultGamemode { get; set; } = true;
}
