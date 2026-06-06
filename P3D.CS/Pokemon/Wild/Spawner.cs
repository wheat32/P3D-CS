namespace P3D;

// TODO Phase 4: full Spawner port (requires Level system, .poke file loading, roaming encounter logic)
public class Spawner
{
    public enum EncounterMethods
    {
        Land = 0,
        Headbutt = 1,
        Surfing = 2,
        OldRod = 3,
        GoodRod = 31,
        SuperRod = 32,
        Event = 4,
        RockSmash = 5,
    }

    public static Pokemon? GetPokemon(String levelFile, EncounterMethods method,
        bool canEncounterRoaming = true, String inputPokeFile = "") => null;
}
