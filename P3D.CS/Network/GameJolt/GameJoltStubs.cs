namespace P3D.GameJolt;

// TODO Phase 9: full GameJolt port

public class GamejoltSave
{
    public String GameJoltID = String.Empty;
    public String Gender = "Male";
    public String Emblem = String.Empty;
    public int Points;
    public bool DownloadFailed;
    public bool DownloadFinished;
    public Object? DownloadedSprite;
    public String Friends = String.Empty;

    public const int SAVEFILECOUNT = 16;
    public const int EXTRADATADOWNLOADCOUNT = 4;
    public const int VERSION = 1;

    public List<String> AchievedEmblems { get; } = [];

    public void ResetSave() { }
    public void DownloadSave(String gameJoltID, bool mainSave) { }
    public int DownloadProgress => 0;
    public int TotalDownloadItems => 0;

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

public class Emblem
{
    public Emblem(String name, String gameJoltID, int points, String gender, String emblem)
    {
        Username = name;
        GameJoltID = gameJoltID;
        Points = points;
    }
    public Emblem(String gameJoltID, int dummy) { GameJoltID = gameJoltID; }
    public Emblem(String username, String publicKeys, bool autoLoad) { Username = username; }

    public String Username = String.Empty;
    public String GameJoltID = String.Empty;
    public int Points;
    public bool startedLoading;
    public bool DoneLoading;
    public Microsoft.Xna.Framework.Graphics.Texture2D SpriteTexture =>
        TextureManager.GetTexture("Textures\\NPC\\" + Core.Player.Skin);

    public void StartLoading(String username) { startedLoading = true; }
    public void Draw(Microsoft.Xna.Framework.Vector2 position, float scale) { }

    public static void DrawNewEmblems() { }
    public static void AchieveEmblem(String emblemID) { }
    public static bool HasDownloadedSprite(String gameJoltID) => false;
    public static Microsoft.Xna.Framework.Graphics.Texture2D? GetOnlineSprite(String gameJoltID) => null;
    public static int GetPlayerLevel(int points) => 0;
    public static String GetPlayerSpriteFile(int level, String id, String gender) => "Hilbert";
    public static void Draw(String username, String id, int points, String gender,
                             String emblem, Microsoft.Xna.Framework.Vector2 position,
                             float scale, Object? sprite) { }
    public static Microsoft.Xna.Framework.Graphics.Texture2D? GetPlayerSprite(int level, String id, String gender) => null;
    public static void GetAchievedEmblems() { }
    public static void ClearOnlineSpriteCache() { }
    public static int GetPointsForLevel(int level) => level * 1000;
    public static Microsoft.Xna.Framework.Graphics.Texture2D? GetEmblemBackgroundTexture(String emblem) => null;
}

// TODO Phase 9: full LogInScreen port
public class LogInScreen : P3D.Screen
{
    public static String LoadedGameJoltID = String.Empty;

    public LogInScreen(P3D.Screen preScreen) { PreScreen = preScreen; }
    public static bool UserBanned(String gameJoltID) => false;
    public static void KickFromOnlineScreen(P3D.Screen screen) { }
    public static String GetBanReasonByID(String id) => String.Empty;
    public static String BanReasonIDForUser(String gameJoltID) => String.Empty;
}

// TODO Phase 9: full GTSMainScreen port
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
        public String Name = String.Empty;
        public String Value = String.Empty;
    }

    public static List<JoltValue> HandleData(String result) => [];
}

public static class GameJoltStatistics
{
    public static void Track(String statName, int addition) { }
    public static void GetStatisticValue(String statName, Action<String> callback) { }
}

// Stub for the APICall type used in save
public class APICall
{
    public delegate void ResponseHandler(String result);
    public event Action<Exception>? CallFails;

    public APICall(ResponseHandler handler) { }
    public void SetStorageData(String key, String data, bool useUsername) { }
    public void SetStorageData(String[] keys, String[] data, bool[] useUsername) { }
    public void GetKeys(bool matchUsername, String pattern) { }
    public void FetchTable(int limit, String tableID) { }
    public void FetchUserRank(String tableID, int score) { }
    public void FetchUserdataByID(String userID) { }
    public void FetchFriendList(String gameJoltID) { }
}

// PokegearScreen is fully ported in Screens/PokegearScreen.cs
