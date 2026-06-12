namespace P3D.GameJolt;

// TODO Phase 8: full GameJolt port

public class GamejoltSave
{
    public String GameJoltID = String.Empty;
    public String Gender = "Male";
    public String Emblem = String.Empty;
    public int Points;
    public bool DownloadFailed;
    public bool DownloadFinished;
    public Object? DownloadedSprite;

    // Save file data fields (paralleling local .dat files)
    public String Player = String.Empty;
    public String Party = String.Empty;
    public String Items = String.Empty;
    public String Berries = String.Empty;
    public String Apricorns = String.Empty;
    public String Daycare = String.Empty;
    public String Pokedex = String.Empty;
    public String Register = String.Empty;
    public String ItemData = String.Empty;
    public String Box = String.Empty;
    public String NPC = String.Empty;
    public String HallOfFame = String.Empty;
    public String SecretBase = String.Empty;
    public String RoamingPokemon = String.Empty;
    public String Statistics = String.Empty;
    public String Options = String.Empty;
}

public static class SessionManager
{
    public static void Update() { }
    public static void Close() { }
}

public static class StaffProfile
{
    public static void SetupStaff() { }
}

public static class Emblem
{
    public static void DrawNewEmblems() { }
    public static void AchieveEmblem(String emblemID) { }
    public static int GetPlayerLevel(int points) => 0;
    public static String GetPlayerSpriteFile(int level, String id, String gender) => "Hilbert";
    public static void Draw(String username, String id, int points, String gender,
                             String emblem, Microsoft.Xna.Framework.Vector2 position,
                             int scale, Object? sprite) { }
    public static void GetAchievedEmblems() { }
}

// TODO Phase 8: full LogInScreen port
public class LogInScreen : P3D.Screen
{
    public LogInScreen(P3D.Screen preScreen) { PreScreen = preScreen; }
    public static bool UserBanned(String gameJoltID) => false;
    public static void KickFromOnlineScreen(P3D.Screen screen) { }
}

// TODO Phase 8: full GTSMainScreen port
public class GTSMainScreen : P3D.Screen
{
    public GTSMainScreen(P3D.Screen preScreen) { PreScreen = preScreen; }
}

public static class API
{
    public static bool LoggedIn;
    public static String username = String.Empty;

    public class JoltValue
    {
        public String Value = String.Empty;
    }

    public static List<JoltValue> HandleData(String result) => [];
}

public static class GameJoltStatistics
{
    public static void Track(String statName, int addition) { }
}

// Stub for the APICall type used in save
public class APICall
{
    public delegate void ResponseHandler(String result);
    public event Action<Exception>? CallFails;

    public APICall(ResponseHandler handler) { }
    public void SetStorageData(String key, String data, bool useUsername) { }
    public void SetStorageData(String[] keys, String[] data, bool[] useUsername) { }
}

// TODO Phase 8: full PokegearScreen port
public class PokegearScreen : P3D.Screen
{
    public enum EntryModes
    {
        MainMenu = 0,
        BattleRequest = 1,
        TradeRequest = 2,
    }

    public class RadioStation
    {
        public String Music { get; set; } = String.Empty;
        public String Name { get; set; } = String.Empty;
    }

    public static String Call_Flag = String.Empty;
    public static int BattleRequestData = -1;
    public static int TradeRequestData = -1;
    public static bool StationCanPlay(RadioStation? station) => false;

    public PokegearScreen(P3D.Screen preScreen, EntryModes mode, int[] args)
    {
        PreScreen = preScreen;
    }
}
