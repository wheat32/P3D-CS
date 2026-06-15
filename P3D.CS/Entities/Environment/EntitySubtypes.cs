using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

// TODO Phase 12: full entity subtype ports

public class AnimatedBlock : Entity
{
    public void Initialize(List<List<int>>? animationData = null) => base.Initialize();
    public static void ClearAnimationResources() { }
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
    public void Warp(bool instant) { }
}

public class Floor : Entity
{
    public bool IsIce;
    public bool hasSnow;
    public bool hasSand;

    public Floor() { }

    public Floor(float x, float y, float z,
                 Texture2D[] textures, int[] textureIndex,
                 bool collision, int rotation, Vector3 scale,
                 BaseModel model, int action, String additionalValue,
                 bool visible, Vector3 shader,
                 bool hasSnow, bool hasIce, bool hasSand)
    {
        Position = new Vector3(x, y, z);
        Textures = textures;
        Collision = collision;
        Scale = scale;
        BaseModel = model;
        ActionValue = action;
        AdditionalValue = additionalValue;
        Visible = visible;
        this.hasSnow = hasSnow;
        IsIce = hasIce;
        this.hasSand = hasSand;
        Initialize(hasSnow, hasSand, hasIce);
    }

    public static void ClearFloorTemp() { }

    public void SetRotation(int rotation) { }

    public void Initialize(bool hasSnowVal, bool hasSandVal, bool hasIceVal) => base.Initialize();

    public int GetIceFloors() => 1;
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
    public static void ClearAnimationResources() { }
    public static void AddDefaultWaterAnimationResources() { }
}

public class Grass : Entity
{
    public new void Initialize() => base.Initialize();
    public static List<Entity> GetGrassTilesAroundPlayer(float radius) => [];
}

public class BerryPlant : Entity
{
    public new void Initialize() => base.Initialize();

    public void Initialize(int berryID, int stage, String plantDate, String harvestData, bool drenched)
    {
        base.Initialize();
    }
}

public class LoamySoil : Entity
{
    public new void Initialize() => base.Initialize();
}

public class ItemObject : Entity
{
    public new void Initialize() => base.Initialize();
    public bool IsHiddenItem() => false;
}

public class ScriptBlock : Entity
{
    public static bool TriggeredScriptBlock;
    public String ScriptID { get; set; } = String.Empty;

    public new void Initialize() => base.Initialize();
    public int GetActivationID() => 0;
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
    public static void ClearAnimationResources() { }
    public static void AddDefaultWaterAnimationResources() { }
}

public class Whirlpool : Entity
{
    public static bool LoadedWaterTemp = false;

    public new void Initialize() => base.Initialize();
}

public class StrengthTrigger : Entity
{
    public new void Initialize() => base.Initialize();
}

public class ModelEntity : Entity
{
    public new void Initialize() => base.Initialize();
    public void LoadModel(String modelPath) { }
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
    public enum Movements
    {
        Still = 0,
        Looking = 1,
        FacePlayer = 2,
        Walk = 3,
        Straight = 4,
        Turning = 5,
        Pokeball = 6
    }

    public bool IsTrainer;
    public int NPCID { get; set; }
    public List<Pokemon> Pokemons { get; } = [];
    public int faceRotation;
    public bool isDancing;
    public String TextureID { get; set; } = String.Empty;
    public bool MoveAsync;
    public float MoveY;
    public bool AnimateIdle;
    public List<Rectangle> MoveRectangles { get; set; } = [];
    public Movements Movement { get; set; } = Movements.Still;
    public String Name { get; set; } = String.Empty;

    public bool CheckInSight() => false;
    public bool InCameraFocus() => false;

    /// <summary>Number of Pokémon that can still battle (not fainted, not egg).</summary>
    public int CountUseablePokemon
    {
        get
        {
            int count = 0;
            for (int i = 0; i < Pokemons.Count; i++)
            {
                if (Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Pokemons[i].IsEgg == false)
                {
                    count++;
                }
            }
            return count;
        }
    }

    // Parameters match Entity.GetNewEntity's NPC call order:
    // textureID, rotation, name, id, animateIdle, movement, moveRectangles
    public void Initialize(String textureID, int rotation, String name, int id,
                           bool animateIdle, String movement, List<Rectangle> moveRectangles)
    {
        NPCID = id;
        Name = name;
        TextureID = textureID;
        AnimateIdle = animateIdle;
        Rotation = Entity.GetRotationFromInteger(rotation);
        Movement = movement.ToLower() switch
        {
            "walk" => Movements.Walk,
            "straight" => Movements.Straight,
            "looking" => Movements.Looking,
            "facerotation" => Movements.Turning,
            _ => Movements.Still
        };
        MoveRectangles = moveRectangles;
        base.Initialize();
        SetupSprite(textureID, "", false);
    }

    public void SetupSprite(String textureID, String extra, bool update)
    {
        TextureID = textureID;

        String texturePath = @"Textures\NPC\";
        if (TextureID.StartsWith("[POKEMON|N]"))
        {
            TextureID = TextureID.Remove(0, 11);
            texturePath = @"Pokemon\Overworld\Normal\";
        }
        else if (TextureID.StartsWith("[POKEMON|S]"))
        {
            TextureID = TextureID.Remove(0, 11);
            texturePath = @"Pokemon\Overworld\Shiny\";
        }

        if (TextureID.Equals("<player.skin>", StringComparison.OrdinalIgnoreCase))
            TextureID = Core.Player.Skin;
        else if (TextureID.Equals("<rival.skin>", StringComparison.OrdinalIgnoreCase))
            TextureID = Core.Player.RivalSkin;

        if (textureID.StartsWith(@"Pokemon\Overworld\") || textureID.StartsWith(@"Pokemon\Battle\"))
            texturePath = "";

        Texture2D fullTexture = TextureManager.GetTexture(texturePath + TextureID);

        Microsoft.Xna.Framework.Vector2 frameSize;
        if (Movement == Movements.Pokeball)
        {
            frameSize = new Microsoft.Xna.Framework.Vector2(fullTexture.Width, fullTexture.Height);
        }
        else if (fullTexture.Width == fullTexture.Height / 2)
        {
            frameSize = new Microsoft.Xna.Framework.Vector2(fullTexture.Width / 2, fullTexture.Height / 4);
        }
        else if (fullTexture.Width == fullTexture.Height)
        {
            frameSize = new Microsoft.Xna.Framework.Vector2(fullTexture.Width / 4, fullTexture.Height / 4);
        }
        else
        {
            frameSize = new Microsoft.Xna.Framework.Vector2(fullTexture.Width / 3, fullTexture.Height / 4);
        }

        // Use the first frame (standing, facing down)
        Rectangle frameRect = new Rectangle(0, 0, (int)frameSize.X, (int)frameSize.Y);
        Textures = [TextureManager.GetTexture(fullTexture, frameRect, 1)];
    }

    public static void AddNPCData(String data) { }
    public static void RemoveNPCData(String id) { }
}

// TODO Phase 12: full MessageBulb port
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

    public MessageBulb(Microsoft.Xna.Framework.Vector3 position, NotificationTypes notificationType)
    {
        Position = position;
        NotificationType = notificationType;
        EntityID = "MessageBulb";
    }
}
