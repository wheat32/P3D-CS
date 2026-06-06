namespace P3D;

// TODO Phase 4: full Level port
public class Level
{
    public bool Riding;
    public bool Surfing;
    public Terrain Terrain { get; set; } = new Terrain();
    public bool WildPokemonGrass;
    public bool WildPokemonFloor;
    public bool WildPokemonWater;
    public bool IsOutside;
    public bool CanFly;
    public bool CanDig;
    public bool CanTeleport;
    public bool ShowOverworldPokemon;
    public String LevelFile = "";
    public String BlackOutScript = "";
    public List<String> DisabledMenus = [];
    public List<Entity> Entities = [];
    public PokemonEncounterData PokemonEncounterData = new PokemonEncounterData();

    public void StartOffsetMapUpdate() { }
    public void StopOffsetMapUpdate() { }
}

public class PokemonEncounterData
{
    public bool EncounteredPokemon;
}

// TODO Phase 4: full Terrain port
public class Terrain
{
    public enum TerrainTypes
    {
        Plain, Underwater, Cave, Rock, Sand, Grass, LongGrass,
        Snow, Mountain, PondWater, Ocean, Puddles, Swamp, Raft,
        Magma, DistortionWorld, Sky, Indoor, Ice,
    }

    public TerrainTypes TerrainType;
    public Terrain? Terrain2 { get; set; }
}
