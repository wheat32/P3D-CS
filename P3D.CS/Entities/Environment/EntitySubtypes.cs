using Microsoft.Xna.Framework;

namespace P3D;

// TODO Phase 4: full entity subtype ports

public class AnimatedBlock : Entity
{
    public void Initialize(List<List<int>>? animationData = null) => base.Initialize();
}

public class WallBlock : Entity
{
    public new void Initialize() => base.Initialize();
}

public class AllSidesObject : Entity
{
    public new void Initialize() => base.Initialize();
}

public class SlideBlock : Entity
{
    public new void Initialize() => base.Initialize();
}

public class HalfSlideBlock : Entity
{
    public new void Initialize() => base.Initialize();
}

public class WallBill : Entity
{
    public new void Initialize() => base.Initialize();
}

public class SignBlock : Entity
{
    public new void Initialize() => base.Initialize();
}

public class WarpBlock : Entity
{
    public new void Initialize() => base.Initialize();
}

public class Floor : Entity
{
    public bool IsIce;

    public void Initialize(bool a, bool b, bool c) => base.Initialize();
}

public class StepBlock : Entity
{
    public new void Initialize() => base.Initialize();
}

public class CutDownTree : Entity
{
    public new void Initialize() => base.Initialize();
}

public class Water : Entity
{
    public new void Initialize() => base.Initialize();
}

public class Grass : Entity
{
    public new void Initialize() => base.Initialize();
}

public class BerryPlant : Entity
{
    public new void Initialize() => base.Initialize();
}

public class LoamySoil : Entity
{
    public new void Initialize() => base.Initialize();
}

public class ItemObject : Entity
{
    public new void Initialize() => base.Initialize();
}

public class ScriptBlock : Entity
{
    public static bool TriggeredScriptBlock;

    public new void Initialize() => base.Initialize();
}

public class TurningSign : Entity
{
    public new void Initialize() => base.Initialize();
}

public class ApricornPlant : Entity
{
    public new void Initialize() => base.Initialize();
}

public class HeadbuttTree : Entity
{
    public new void Initialize() => base.Initialize();
}

public class SmashRock : Entity
{
    public new void Initialize() => base.Initialize();
    public static void Load() { }
}

public class StrengthRock : Entity
{
    public new void Initialize() => base.Initialize();
}

public class Waterfall : Entity
{
    public new void Initialize() => base.Initialize();
}

public class Whirlpool : Entity
{
    public new void Initialize() => base.Initialize();
}

public class StrengthTrigger : Entity
{
    public new void Initialize() => base.Initialize();
}

public class ModelEntity : Entity
{
    public new void Initialize() => base.Initialize();
}

public class RotationTile : Entity
{
    public new void Initialize() => base.Initialize();
}

public class DiveTile : Entity
{
    public new void Initialize() => base.Initialize();
}

public class RockClimbEntity : Entity
{
    public new void Initialize() => base.Initialize();
}

public class HoleBlock : Entity
{
    public new void Initialize() => base.Initialize();
}

public class NPC : Entity
{
    public bool IsTrainer;
    public List<Pokemon> Pokemons { get; } = [];

    /// <summary>Number of Pokémon that can still battle (not fainted, not egg).</summary>
    public int CountUseablePokemon
    {
        get
        {
            int count = 0;
            for (int i = 0; i < Pokemons.Count; i++)
            {
                if (Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Pokemons[i].IsEgg == false)
                    count++;
            }
            return count;
        }
    }

    public void Initialize(String name, int id, String facing, int moveType,
                           bool interact, String script, List<Rectangle> collisions)
    {
        base.Initialize();
    }
}

// TODO Phase 4: full MessageBulb port
public class MessageBulb : Entity
{
    public enum NotificationTypes
    {
        Waiting = 0,
        Exclamation = 1,
        Shouting = 2,
        Question = 3,
        Note = 4,
        Heart = 5,
        Unhappy = 6,
        Happy = 7,
        Friendly = 8,
        Poisoned = 9,
        Battle = 10,
        Wink = 11,
        AFK = 12,
        Angry = 13,
        CatFace = 14,
        Unsure = 15
    }

    public NotificationTypes NotificationType;
}
