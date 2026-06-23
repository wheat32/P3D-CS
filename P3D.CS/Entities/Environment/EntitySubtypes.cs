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

    // Called when the player steps off the upper floor into the stair opening
    // (no floor at their Y, stair tile exists one unit below).
    // Adds an extra movement step and sets a Y=-0.5 component so the camera
    // descends diagonally (the hypotenuse) over two tiles to the landing level.
    public override void WalkOntoFunction()
    {
        if (Screen.Camera == null) return;
        Screen.Camera.Move(1);
        Vector3 pm = Screen.Camera.PlannedMovement;
        // -0.5 per tile × 2 tiles = -1 total — matches the 1-unit drop to the landing.
        Screen.Camera.PlannedMovement = new Vector3(pm.X, -0.5f, pm.Z);
    }

    // Called when the player walks into the stair tile from the landing side.
    // Adds an extra movement step and a +Y component so the camera ascends
    // diagonally over two tiles, mirroring the descent path.
    public override bool WalkAgainstFunction()
    {
        if (Screen.Camera != null)
        {
            Screen.Camera.Move(1);
            Vector3 pm = Screen.Camera.PlannedMovement;
            // +0.5 per tile × 2 tiles = +1 total — matches the 1-unit rise.
            Screen.Camera.PlannedMovement = new Vector3(pm.X, 0.5f, pm.Z);
        }
        return false;
    }
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

    public override void Render()
    {
        if (Model == null)
        {
            Draw(BaseModel, Textures, false);
        }
        else
        {
            UpdateModel();
            Draw(BaseModel, Textures, true, Model);
        }
    }

    public override bool WalkAgainstFunction() => Warp(false);

    public bool Warp(bool mapViewMode)
    {
        if (IsValidLink(AdditionalValue) == true && ScriptBlock.TriggeredScriptBlock == false)
        {
            String destination = AdditionalValue.GetSplit(0);
            int separatorCount = AdditionalValue.CountSeperators(",");

            if (separatorCount >= 5)
            {
                if (AdditionalValue.GetSplit(5, ",").Equals(String.Empty) == false)
                {
                    List<int> validRotations = [];
                    foreach (String element in AdditionalValue.GetSplit(5, ",").Split('|'))
                    {
                        validRotations.Add(int.Parse(element));
                    }
                    if (validRotations.Contains(Screen.Camera!.GetPlayerFacingDirection()) == false)
                    {
                        return true;
                    }
                }
            }

            String warpSoundName = "Warp_Exit";
            if (separatorCount >= 6)
            {
                warpSoundName = int.Parse(AdditionalValue.GetSplit(6)) switch
                {
                    0 => "Warp_Exit",
                    1 => "Warp_Door",
                    2 => "Warp_Ladder",
                    3 => String.Empty,
                    _ => "Warp_Exit"
                };
            }

            String[] destParts = destination.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            String[] gmParts = GameModeManager.ActiveGameMode!.MapPath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            String gameModeMapPath = Path.Combine([GameController.GamePath, ..gmParts, ..destParts]);
            String defaultMapPath = Path.Combine([GameController.GamePath, "Content", "Data", "maps", ..destParts]);

            if (File.Exists(gameModeMapPath) == true || File.Exists(defaultMapPath) == true)
            {
                if (mapViewMode == false)
                {
                    Screen.Level!.WarpData.WarpDestination = AdditionalValue.GetSplit(0);
                    Screen.Level.WarpData.WarpPosition = new Vector3(
                        float.Parse(AdditionalValue.GetSplit(1)),
                        float.Parse(AdditionalValue.GetSplit(2).InsertDecSeparator()),
                        float.Parse(AdditionalValue.GetSplit(3)));
                    Screen.Level.WarpData.WarpRotations = int.Parse(AdditionalValue.GetSplit(4));
                    Screen.Level.WarpData.DoWarpInNextTick = true;
                    Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera!.Yaw;
                    Screen.Level.WarpData.IsWarpBlock = true;

                    if (GameModeManager.ContentFileExists(@"Sounds\" + warpSoundName + ".wav") == true ||
                        GameModeManager.ContentFileExists(@"Sounds\" + warpSoundName + ".xnb") == true)
                    {
                        Screen.Level.WarpData.WarpSound = warpSoundName;
                    }
                    else if (warpSoundName.Equals(String.Empty) == true)
                    {
                        Screen.Level.WarpData.WarpSound = null!;
                    }
                    else
                    {
                        Screen.Level.WarpData.WarpSound = "Warp_Exit";
                    }

                    Logger.Debug("Lock Camera");
                    ((OverworldCamera)Screen.Camera!).YawLocked = true;
                }
                else
                {
                    Screen.Level = new Level();
                    Screen.Level.Load(AdditionalValue.GetSplit(0));
                    Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);

                    Screen.Camera!.Position = new Vector3(
                        float.Parse(AdditionalValue.GetSplit(1)),
                        float.Parse(AdditionalValue.GetSplit(2).InsertDecSeparator()),
                        float.Parse(AdditionalValue.GetSplit(3)));
                }
            }
            else
            {
                CallError("Map file \"" + GameModeManager.ActiveGameMode.MapPath + destination + "\" does not exist.");
            }
        }

        return false;
    }

    public static bool IsValidLink(String link)
    {
        if (link.Equals(String.Empty) == true)
        {
            CallError("Link is empty.");
            return false;
        }

        if (link.Contains(",") == false)
        {
            CallError("Link does not contain seperators or has wrong seperators.");
            return false;
        }

        if (link.CountSeperators(",") < 4)
        {
            CallError("Not enough or too much arguments to resolve the link.");
            return false;
        }

        String destination = link.GetSplit(0);
        if (destination.EndsWith(".dat") == false)
        {
            CallError("Destination file is not a valid map file.");
            return false;
        }

        String x = link.GetSplit(1);
        String y = link.GetSplit(2);
        String z = link.GetSplit(3);
        String l = link.GetSplit(4);

        if (StringHelper.IsNumeric(x) == true && StringHelper.IsNumeric(y) == true &&
            StringHelper.IsNumeric(z) == true && StringHelper.IsNumeric(l) == true)
        {
            return true;
        }

        CallError("Position values are not numeric.");
        return false;
    }

    private static void CallError(String message)
    {
        Logger.Log(Logger.LogTypes.ErrorMessage, "WarpBlock.vb: Invalid warp! More information:" + message);
    }
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
    public static Dictionary<String, Texture2D> WaterTexturesTemp { get; } = [];

    private String _waterTextureName = String.Empty;
    private Animation? _waterAnimation;
    private Rectangle _currentRectangle = new Rectangle(0, 0, 0, 0);

    public override void Initialize()
    {
        base.Initialize();

        _waterAnimation = new Animation(TextureManager.GetTexture(@"Textures\Routes"), 1, 3, 16, 16,
            GameModeManager.ActiveGameMode.WaterSpeed, 15, 0);

        CreateWaterTextureTemp();
        ChangeTexture();
    }

    public static void ClearAnimationResources() => WaterTexturesTemp.Clear();

    public static void AddDefaultWaterAnimationResources()
    {
        WaterTexturesTemp["_0"] = TextureManager.GetTexture("Routes", new Rectangle(0, 220, 20, 20));
        WaterTexturesTemp["_1"] = TextureManager.GetTexture("Routes", new Rectangle(20, 220, 20, 20));
        WaterTexturesTemp["_2"] = TextureManager.GetTexture("Routes", new Rectangle(40, 220, 20, 20));
        WaterTexturesTemp["_3"] = TextureManager.GetTexture("Routes", new Rectangle(60, 220, 20, 20));
        WaterTexturesTemp["_4"] = TextureManager.GetTexture("Routes", new Rectangle(80, 220, 20, 20));
        WaterTexturesTemp["_5"] = TextureManager.GetTexture("Routes", new Rectangle(100, 220, 20, 20));
        WaterTexturesTemp["_6"] = TextureManager.GetTexture("Routes", new Rectangle(120, 220, 20, 20));
        WaterTexturesTemp["_7"] = TextureManager.GetTexture("Routes", new Rectangle(140, 220, 20, 20));
        WaterTexturesTemp["_8"] = TextureManager.GetTexture("Routes", new Rectangle(160, 220, 20, 20));
        WaterTexturesTemp["_9"] = TextureManager.GetTexture("Routes", new Rectangle(180, 220, 20, 20));
        WaterTexturesTemp["_10"] = TextureManager.GetTexture("Routes", new Rectangle(200, 220, 20, 20));
        WaterTexturesTemp["_11"] = TextureManager.GetTexture("Routes", new Rectangle(220, 220, 20, 20));
    }

    private void CreateWaterTextureTemp()
    {
        List<String> textureData = AdditionalValue.Split(',').ToList();
        if (textureData.Count >= 5)
        {
            bool rotationOffsetVertical = false; // False = Horizontal, True = Vertical
            if (textureData.Count >= 6)
                rotationOffsetVertical = Convert.ToBoolean(textureData[5]);

            Rectangle r = new Rectangle(int.Parse(textureData[1]), int.Parse(textureData[2]),
                                         int.Parse(textureData[3]), int.Parse(textureData[4]));
            String texturePath = textureData[0];
            _waterTextureName = AdditionalValue;

            if (rotationOffsetVertical == true)
            {
                if (WaterTexturesTemp.ContainsKey(AdditionalValue + "_0") == false)
                {
                    WaterTexturesTemp[AdditionalValue + "_0"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_1"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_2"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 2, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_3"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X, r.Y + r.Height, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_4"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width, r.Y + r.Height, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_5"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 2, r.Y + r.Height, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_6"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X, r.Y + r.Height * 2, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_7"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width, r.Y + r.Height * 2, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_8"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 2, r.Y + r.Height * 2, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_9"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X, r.Y + r.Height * 3, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_10"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width, r.Y + r.Height * 3, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_11"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 2, r.Y + r.Height * 3, r.Width, r.Height));
                }
            }
            else
            {
                if (WaterTexturesTemp.ContainsKey(AdditionalValue + "_0") == false)
                {
                    WaterTexturesTemp[AdditionalValue + "_0"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_1"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_2"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 2, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_3"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 3, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_4"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 4, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_5"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 5, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_6"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 6, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_7"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 7, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_8"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 8, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_9"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 9, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_10"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 10, r.Y, r.Width, r.Height));
                    WaterTexturesTemp[AdditionalValue + "_11"] = TextureManager.GetTexture(texturePath, new Rectangle(r.X + r.Width * 11, r.Y, r.Width, r.Height));
                }
            }
        }
        else
        {
            if (WaterTexturesTemp.ContainsKey("_0") == false)
            {
                AddDefaultWaterAnimationResources();
            }
        }
    }

    public override void ClickFunction()
    {
        if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
        {
            // Surf() not yet ported (Phase 12)
        }
    }

    public override void UpdateEntity()
    {
        if (Core.GameOptions.GraphicStyle == 1)
        {
            if (_waterAnimation != null)
            {
                _waterAnimation.Update(0.01f);
                if (_currentRectangle != _waterAnimation.TextureRectangle)
                {
                    ChangeTexture();
                    _currentRectangle = _waterAnimation.TextureRectangle;
                }
            }
        }
        base.UpdateEntity();
    }

    private void ChangeTexture()
    {
        if (WaterTexturesTemp.Count == 0 ||
            WaterTexturesTemp.ContainsKey("_0") == false ||
            WaterTexturesTemp.ContainsKey(_waterTextureName + "_0") == false)
        {
            ClearAnimationResources();
            AddDefaultWaterAnimationResources();
            CreateWaterTextureTemp();
        }

        if (_waterAnimation == null || Textures.Length == 0) return;

        int col = _waterAnimation.CurrentColumn;
        if (Rotation.Y == 0f || Rotation.Y == MathHelper.TwoPi)
        {
            if (col == 0) Textures[0] = WaterTexturesTemp[_waterTextureName + "_0"];
            else if (col == 1) Textures[0] = WaterTexturesTemp[_waterTextureName + "_1"];
            else if (col == 2) Textures[0] = WaterTexturesTemp[_waterTextureName + "_2"];
        }
        else if (Rotation.Y == MathHelper.PiOver2)
        {
            if (col == 0) Textures[0] = WaterTexturesTemp[_waterTextureName + "_3"];
            else if (col == 1) Textures[0] = WaterTexturesTemp[_waterTextureName + "_4"];
            else if (col == 2) Textures[0] = WaterTexturesTemp[_waterTextureName + "_5"];
        }
        else if (Rotation.Y == MathHelper.Pi)
        {
            if (col == 0) Textures[0] = WaterTexturesTemp[_waterTextureName + "_6"];
            else if (col == 1) Textures[0] = WaterTexturesTemp[_waterTextureName + "_7"];
            else if (col == 2) Textures[0] = WaterTexturesTemp[_waterTextureName + "_8"];
        }
        else if (Rotation.Y == MathHelper.Pi * 1.5f)
        {
            if (col == 0) Textures[0] = WaterTexturesTemp[_waterTextureName + "_9"];
            else if (col == 1) Textures[0] = WaterTexturesTemp[_waterTextureName + "_10"];
            else if (col == 2) Textures[0] = WaterTexturesTemp[_waterTextureName + "_11"];
        }
    }

    public override void Render()
    {
        bool setRasterizerState = BaseModel != null && BaseModel.ID != 0;
        if (Model == null)
        {
            Draw(BaseModel, Textures, setRasterizerState);
        }
        else
        {
            UpdateModel();
            Draw(BaseModel, Textures, true, Model);
        }
    }
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

    public new void Initialize()
    {
        ScriptID = AdditionalValue;
        base.Initialize();
    }

    public int GetActivationID() => ActionValue;

    public override void ClickFunction()
    {
        if (Core.CurrentScreen.Identification != Screen.Identifications.OverworldScreen) return;
        OverworldScreen os = (OverworldScreen)Core.CurrentScreen;
        if (os.ActionScript.IsReady == false || TriggeredScriptBlock == true) return;

        // ActionValue 1 = interact trigger (script file); 2 = interact trigger (raw text)
        if (ActionValue == 1 || ActionValue == 2)
        {
            TriggeredScriptBlock = true;
            int inputType = ActionValue == 2 ? 1 : 0;
            os.ActionScript.StartScript(AdditionalValue, inputType, scriptTrigger: "ScriptBlockInteract");
        }
    }

    public override bool WalkIntoFunction()
    {
        if (Core.CurrentScreen.Identification != Screen.Identifications.OverworldScreen) return false;
        OverworldScreen os = (OverworldScreen)Core.CurrentScreen;
        if (os.ActionScript.IsReady == false || TriggeredScriptBlock == true) return false;

        // ActionValue 0 and 4 = walk-on trigger (script file)
        if (ActionValue == 0 || ActionValue == 4)
        {
            TriggeredScriptBlock = true;
            os.ActionScript.StartScript(AdditionalValue, 0, scriptTrigger: "ScriptBlockWalkOn");
        }
        return false;
    }
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

    private const float STANDARD_SPEED = 0.04f;
    private const float ANIMATION_DELAY_LENGTH = 1.1f;

    private Texture2D? _fullTexture;
    private Vector2 _frameSize;
    private Rectangle _lastFrameRect = Rectangle.Empty;
    private int _animationX = 1;
    private int _animationXOffset = 0;
    private float _animationDelay = ANIMATION_DELAY_LENGTH;

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
        faceRotation = rotation;
        FaceDirection = rotation;
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
        NeedsUpdate = true;
        CreateWorldEveryFrame = true;
        DropUpdateUnlessDrawn = false;
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

        _fullTexture = TextureManager.GetTexture(texturePath + TextureID);

        if (Movement == Movements.Pokeball)
        {
            _frameSize = new Vector2(_fullTexture.Width, _fullTexture.Height);
        }
        else if (_fullTexture.Width == _fullTexture.Height / 2)
        {
            _frameSize = new Vector2(_fullTexture.Width / 2, _fullTexture.Height / 4);
        }
        else if (_fullTexture.Width == _fullTexture.Height)
        {
            _frameSize = new Vector2(_fullTexture.Width / 4, _fullTexture.Height / 4);
        }
        else
        {
            _frameSize = new Vector2(_fullTexture.Width / 3, _fullTexture.Height / 4);
        }

        _lastFrameRect = Rectangle.Empty;
        UpdateSpriteFrame();
    }

    private void UpdateSpriteFrame()
    {
        if (_fullTexture == null) return;

        int cameraRotation = GetCameraRotation();
        int spriteIndex = faceRotation - cameraRotation;
        if (spriteIndex < 0) spriteIndex += 4;
        if (spriteIndex > 3) spriteIndex -= 4;

        int x = 0;
        if (Moved > 0f || AnimateIdle)
            x = (int)_frameSize.X * GetAnimationX();

        if (Movement == Movements.Pokeball)
        {
            spriteIndex = 0;
            x = 0;
        }

        int y = (int)(_frameSize.Y * spriteIndex);

        int fw = (int)_frameSize.X;
        int fh = (int)_frameSize.Y;
        if (x + fw > _fullTexture.Width) fw = _fullTexture.Width - x;
        if (y + fh > _fullTexture.Height) fh = _fullTexture.Height - y;

        Rectangle frameRect = new Rectangle(x, y, fw, fh);
        if (frameRect != _lastFrameRect)
        {
            _lastFrameRect = frameRect;
            Textures = [TextureManager.GetTexture(_fullTexture, frameRect, 1)];
        }
    }

    private int GetCameraRotation()
    {
        if (Screen.Camera == null) return 0;
        float yaw = Screen.Camera.Yaw;
        while (yaw < 0) yaw += MathHelper.TwoPi;
        while (yaw >= MathHelper.TwoPi) yaw -= MathHelper.TwoPi;
        if (yaw < MathHelper.PiOver4) return 0;
        if (yaw < MathHelper.Pi - MathHelper.PiOver4) return 1;
        if (yaw < MathHelper.Pi + MathHelper.PiOver4) return 2;
        if (yaw < MathHelper.TwoPi - MathHelper.PiOver4) return 3;
        return 0;
    }

    private int GetAnimationX()
    {
        if (_fullTexture == null) return 0;
        if (_fullTexture.Width == _fullTexture.Height / 2)
            return _animationX switch { 1 => 0, 2 => 1, 3 => 0, _ => 1 };
        if (_fullTexture.Width == _fullTexture.Height)
            return _animationX switch { 1 => 0, 2 => 1, 3 => 2, _ => 3 };
        return _animationX switch { 1 => 0, 2 => 1, 3 => 0, _ => 2 };
    }

    public float TurningDelay = 2.0f;
    public int StraightDirection = -1;

    public override void Update()
    {
        base.Update();
        NPCMovement();
    }

    // Faithful port of VB NPC.NPCMovement().
    // Exits immediately when an OverworldScreen script is running so that
    // autonomous AI does not fire during cutscenes — same guard as VB.
    private void NPCMovement()
    {
        Screen s = Core.CurrentScreen;
        while (s.PreScreen != null && s.Identification != Screen.Identifications.OverworldScreen)
            s = s.PreScreen;
        if (s.Identification == Screen.Identifications.OverworldScreen &&
            ((OverworldScreen)s).ActionScript.IsReady == false)
            return;

        switch (Movement)
        {
            case Movements.Still:
            case Movements.Pokeball:
                break;

            case Movements.Turning:
                TurningDelay -= 0.1f;
                if (TurningDelay <= 0.0f)
                {
                    TurningDelay = 3.0f;
                    faceRotation = (faceRotation + 1) % 4;
                    FaceDirection = faceRotation;
                }
                break;

            case Movements.Looking:
                if (Moved == 0.0f && Core.Random.Next(0, 220) == 0)
                {
                    int newRot = faceRotation;
                    while (newRot == faceRotation)
                        newRot = Core.Random.Next(0, 4);
                    faceRotation = newRot;
                    FaceDirection = faceRotation;
                }
                break;

            case Movements.FacePlayer:
                if (Moved == 0.0f && Screen.Camera != null)
                {
                    // Only update when on the same row or column as the player
                    if (Screen.Camera.Position.X == Position.X || Screen.Camera.Position.Z == Position.Z)
                    {
                        if (Position.X < Screen.Camera.Position.X) faceRotation = 3;
                        else if (Position.X > Screen.Camera.Position.X) faceRotation = 1;
                        if (Position.Z < Screen.Camera.Position.Z) faceRotation = 2;
                        else if (Position.Z > Screen.Camera.Position.Z) faceRotation = 0;
                        FaceDirection = faceRotation;
                    }
                }
                break;

            case Movements.Walk:
            {
                if (Moved != 0.0f || Core.Random.Next(0, 129) != 0) break;

                int newRotation = faceRotation;
                if (Core.Random.Next(0, 3) == 0)
                {
                    // 1/3 chance to pick a fresh direction different from current
                    while (newRotation == faceRotation)
                        newRotation = Core.Random.Next(0, 4);
                }

                bool canMove = false;
                List<int> canRotate = [];
                int startRotation = newRotation;

                do
                {
                    bool inRect = InMoveRectangle(NPCStepPosition(newRotation));
                    if (inRect) { canMove = true; break; }
                    if (!canRotate.Contains(newRotation) && newRotation != faceRotation)
                        canRotate.Add(newRotation);
                    newRotation = (newRotation + 1) % 4;
                }
                while (newRotation != startRotation);

                faceRotation = newRotation;
                FaceDirection = faceRotation;
                if (canMove) { CanMove = true; Moved = 1.0f; }
                else if (canRotate.Count > 0)
                {
                    faceRotation = canRotate[Core.Random.Next(0, canRotate.Count)];
                    FaceDirection = faceRotation;
                }
                break;
            }

            case Movements.Straight:
            {
                if (Moved != 0.0f) break;
                if (!InMoveRectangle(NPCStepPosition(faceRotation)))
                {
                    faceRotation = (faceRotation + 2) % 4;
                    FaceDirection = faceRotation;
                }
                if (InMoveRectangle(NPCStepPosition(faceRotation)) || MoveRectangles.Count == 0)
                {
                    CanMove = true;
                    Moved = 1.0f;
                }
                break;
            }
        }
    }

    private Vector3 NPCStepPosition(int dir) => dir switch
    {
        0 => new Vector3(Position.X,       Position.Y, Position.Z - 1f),
        1 => new Vector3(Position.X - 1f,  Position.Y, Position.Z),
        2 => new Vector3(Position.X,       Position.Y, Position.Z + 1f),
        3 => new Vector3(Position.X + 1f,  Position.Y, Position.Z),
        _ => Position,
    };

    private bool InMoveRectangle(Vector3 pos)
    {
        foreach (Rectangle r in MoveRectangles)
            if (r.Contains(new Microsoft.Xna.Framework.Point((int)pos.X, (int)pos.Z)))
                return true;
        return false;
    }

    public override void UpdateEntity()
    {
        // Billboard NPCs rotate to always face the camera
        if (Model == null && Screen.Camera != null)
        {
            Rotation = new Vector3(Rotation.X, Screen.Camera.Yaw, Rotation.Z);
        }

        bool wasMoving = Moved > 0f;
        base.UpdateEntity();

        if (Moved > 0f)
        {
            _animationDelay -= 0.13f * (Math.Abs(Speed) / STANDARD_SPEED);
            if (_animationDelay <= 0f)
            {
                _animationDelay = ANIMATION_DELAY_LENGTH;
                if (++_animationX > 4) _animationX = 1;
            }
        }
        else if (wasMoving)
        {
            if (Movement == Movements.Straight && _fullTexture != null && _fullTexture.Width != _fullTexture.Height)
                _animationXOffset = _animationXOffset == 0 ? 2 : 0;
            else
                _animationXOffset = 0;
            _animationX = 1 + _animationXOffset;
            _animationDelay = ANIMATION_DELAY_LENGTH;
        }
        else if (AnimateIdle)
        {
            _animationDelay -= 0.1f;
            if (_animationDelay <= 0f)
            {
                _animationDelay = ANIMATION_DELAY_LENGTH;
                if (++_animationX > 4) _animationX = 1;
            }
        }
    }

    public override void Render()
    {
        if (Model == null)
        {
            UpdateSpriteFrame();
            var prevDepth = Core.GraphicsDevice.DepthStencilState;
            Core.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Draw(BaseModel, Textures, setRasterizerState: true, model: null);
            Core.GraphicsDevice.DepthStencilState = prevDepth;
        }
        else
        {
            UpdateModel();
            Draw(BaseModel, Textures, setRasterizerState: true, model: Model);
        }
    }

    public static void AddNPCData(String data) { }
    public static void RemoveNPCData(String id) { }
}

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
    private int _framesRemaining = 50;

    public MessageBulb(Microsoft.Xna.Framework.Vector3 position, NotificationTypes notificationType)
    {
        Position = position;
        NotificationType = notificationType;
        EntityID = "MessageBulb";
    }

    public override void Update()
    {
        if (_framesRemaining > 0)
        {
            _framesRemaining--;
            if (_framesRemaining == 0)
                CanBeRemoved = true;
        }
    }
}
