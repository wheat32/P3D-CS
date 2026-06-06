namespace P3D.GameJolt;

// TODO Phase 8: full GameJolt port

public class GamejoltSave
{
    public String GameJoltID = "";
    public String Gender = "Male";
    public String Emblem = "";
    public int Points;
    public bool DownloadFailed;
    public bool DownloadFinished;
    public Object? DownloadedSprite;

    // Save file data fields (paralleling local .dat files)
    public String Player = "";
    public String Party = "";
    public String Items = "";
    public String Berries = "";
    public String Apricorns = "";
    public String Daycare = "";
    public String Pokedex = "";
    public String Register = "";
    public String ItemData = "";
    public String Box = "";
    public String NPC = "";
    public String HallOfFame = "";
    public String SecretBase = "";
    public String RoamingPokemon = "";
    public String Statistics = "";
    public String Options = "";
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
    public static int GetPlayerLevel(int points) => 0;
    public static String GetPlayerSpriteFile(int level, String id, String gender) => "Hilbert";
    public static void Draw(String username, String id, int points, String gender,
                             String emblem, Microsoft.Xna.Framework.Vector2 position,
                             int scale, Object? sprite) { }
    public static void GetAchievedEmblems() { }
}

public static class API
{
    public static bool LoggedIn;
    public static String username = "";

    public class JoltValue
    {
        public String Value = "";
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
