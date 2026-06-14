namespace P3D;

// TODO Phase 12: full Pokedex port
public class Pokedex
{
    public int Obtained { get; set; }
    public int Seen { get; set; }
    public String Name { get; set; } = String.Empty;
    public int Count { get; set; } = 0;
    public int OriginalCount { get; set; } = 0;
    public bool IncludeExternalPokemon { get; set; } = false;
    public Dictionary<int, String> PokemonList { get; set; } = [];

    public static bool AutoDetect = true;
    public static int PokemonMaxCount = 0;

    public static String NewPokedex() => "";
    public static String ChangeEntry(String data, String id, int status) => data;
    public static String ChangeEntry(String data, String id, int status, bool forceChange) => data;
    public static void Load() { }
    public static int CountEntries(String data, int[] statuses) => 0;
    public static int GetEntryType(String data, String id) => 0;
    public static int HasAnyForm(int number) => 0;

    public bool IsActivated { get; set; }
    public bool HasPokemon(String dexID, bool includeExtra) => false;
    public int GetPlace(String dexID) => 0;
}
