namespace P3D;

// TODO Phase 3: full Pokedex port
public class Pokedex
{
    public int Obtained { get; set; }
    public int Seen { get; set; }

    public static bool AutoDetect = true;

    public static String NewPokedex() => "";
    public static String ChangeEntry(String data, String id, int status) => data;
    public static String ChangeEntry(String data, String id, int status, bool forceChange) => data;
    public static void Load() { }
    public static int CountEntries(String data, int[] statuses) => 0;
    public static int GetEntryType(String data, String id) => 0;
}
