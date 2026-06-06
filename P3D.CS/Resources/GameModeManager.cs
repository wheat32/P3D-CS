namespace P3D;

// TODO Phase 7: full GameModeManager port
public static partial class GameModeManager
{
    public static int GameModeCount => _gameModes.Count;
    public static GameMode? ActiveGameMode => _gameModes.Count > 0 ? _gameModes[0] : null;

    private static List<GameMode> _gameModes = [];

    public static void LoadGameModes() { }
}

public class GameMode
{
    public static String DefaultLocalizationsPath = @"\Content\Localization\";
    public String DirectoryName = "";
    public String LocalizationsPath => DefaultLocalizationsPath;
}
