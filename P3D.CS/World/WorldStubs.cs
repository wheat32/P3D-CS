using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Items;

namespace P3D;

// TODO Phase 4: full Particle port
public class Particle : Entity
{
    public enum Behaviors { Default, Rising, LeftToRight, Sinking }

    public float MoveSpeed { get; set; }
    public float Opacity { get; set; } = 1.0f;
    public float Destination { get; set; }
    public Behaviors Behavior { get; set; } = Behaviors.Default;

    public Particle(Vector3 position, Texture2D[] textures, int[] textureIndices,
                    int rotation, Vector3 scale, Object model, Vector3 color)
    {
    }

    public void MoveWithCamera(Vector3 diff) { }
}

// TODO Phase 4: full OwnPlayer port
public class OwnPlayer : NPC
{
    public String SkinName { get; set; } = String.Empty;
    public bool UsingGameJoltTexture { get; set; }
    public bool DoAnimation;

    public OwnPlayer(float x, float y, float z, Texture2D[] textures, String skin,
                     int facing, int moveType, String script, String name, int id)
    {
    }

    public void SetTexture(String skin, bool useGameJolt) { }
    public new void UpdateEntity() { }
}

// TODO Phase 4: full OverworldPokemon port
public class OverworldPokemon : Entity
{
    public bool warped;
    public String PokemonID { get; set; } = String.Empty;
    public Pokemon? PokemonReference { get; set; }
    public float MoveSpeed;

    public OverworldPokemon(float x, float y, float z) { }
    public void ChangeRotation() { }
    public bool IsVisible() => Visible;
}

// TODO Phase 8: full NetworkPokemon port
public class NetworkPokemon : Entity
{
}

public class Shader
{
    public Vector3 Position;
    public Vector3 Size;
    public Vector3 shaderColor;
    public bool StopOnContact;
    public bool HasBeenApplied = false;
    public bool DisableWhenNoLighting = false;

    public Shader(Vector3 position, Vector3 size, Vector3 shaderColor,
                  bool stopOnContact, bool disableWhenNoLighting = false)
    {
        Position = position;
        Size = size;
        this.shaderColor = shaderColor;
        StopOnContact = stopOnContact;
        DisableWhenNoLighting = disableWhenNoLighting;
    }

    public void ApplyShader(Entity[] entities)
    {
        for (int x = 0; x < (int)Size.X; x++)
        {
            for (int z = 0; z < (int)Size.Z; z++)
            {
                foreach (Entity e in entities)
                {
                    if (StopOnContact == true)
                    {
                        if ((int)e.Position.X == x + Position.X &&
                            (int)e.Position.Z == z + Position.Z &&
                            e.Position.Y <= Position.Y)
                        {
                            e.Shaders.Add(shaderColor);
                            e.ShadersDisableWhenNoLighting.Add(DisableWhenNoLighting);
                        }
                    }
                    else
                    {
                        if ((int)e.Position.X == x + Position.X &&
                            (int)e.Position.Z == z + Position.Z &&
                            e.Position.Y <= Position.Y + Size.Y &&
                            e.Position.Y >= Position.Y)
                        {
                            e.Shaders.Add(shaderColor);
                            e.ShadersDisableWhenNoLighting.Add(DisableWhenNoLighting);
                        }
                    }
                }
            }
        }
        HasBeenApplied = true;
    }
}

// TODO Phase 4: full RoamingPokemon port
public class RoamingPokemon
{
    public String WorldID = String.Empty;
    public String ScriptPath = String.Empty;

    public static void ShiftRoamingPokemon(int shift) { }
    public static String RemoveRoamingPokemon(RoamingPokemon storage) => String.Empty;
    public static String ReplaceRoamingPokemon(RoamingPokemon storage) => String.Empty;
}

// TODO Phase 4: full PokemonEncounterDataStruct port
public struct PokemonEncounterDataStruct
{
    public Vector3 Position;
    public bool EncounteredPokemon;
    public Spawner.EncounterMethods Method;
    public String PokeFile;
}

// TODO Phase 4: full WarpDataStruct port
public struct WarpDataStruct
{
    public String WarpDestination;
    public Vector3 WarpPosition;
    public bool DoWarpInNextTick;
    public int WarpRotations;
    public float CorrectCameraYaw;
    public bool IsWarpBlock;
    public String WarpSound;
}

public class OffsetMap
{
    public String Identifier { get; private set; } = String.Empty;
    public String MapName { get; private set; } = String.Empty;
    public bool Loaded { get; private set; } = false;
    public List<Entity>? Entities { get; private set; }
    public List<Entity>? Floors { get; private set; }

    public OffsetMap(String mapName)
    {
        MapName = mapName;
        Identifier = mapName + "|" +
            Screen.Level.World.CurrentMapWeather + "|" +
            World.GetCurrentRegionWeather() + "|" +
            World.GetTime() + "|" +
            World.CurrentSeason;
    }

    public void LoadMap(Vector3 offset)
    {
        Loaded = true;
    }

    public void ApplyToLevel(Level level) { }
}
