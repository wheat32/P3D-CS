using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class Entity : BaseEntity
{
    public static bool MakeShake;
    public static bool drawViewBox;

    private const float NEAR_OPACITY = 0.3f;
    private const float OPACITY_THRESHOLD = 0.5f;
    private const float NEAR_OPACITY_DISTANCE = 0.3f;
    private const float CLOSE_OPACITY_CLAMP = 0.3f;
    private const float SHAKE_SCALE = 100f;
    private const float SHAKE_RANGE = 3f;
    private const float DARK_SHADER = 0.5f;
    private const int LIGHTING_TYPE_DARK = 6;

    private static RasterizerState? _newRasterizerState;
    private static RasterizerState? _oldRasterizerState;

    public int ID = -1;
    public String EntityID = "";
    public String MapOrigin = "";
    public bool IsOffsetMapContent;
    public Vector3 Offset = Vector3.Zero;
    public Vector3 Position;
    public Vector3 Rotation = Vector3.Zero;
    public Vector3 Scale = Vector3.One;
    public Texture2D[] Textures = [];
    public int[] TextureIndex = [];
    public int ActionValue;
    public String AdditionalValue = "";
    public String ModelPath = "";
    public bool Visible = true;
    public Vector3 Shader = Vector3.One;
    public List<Vector3> Shaders = [];
    public List<bool> ShadersDisableWhenNoLighting = [];
    public Vector3 Color = Vector3.One;
    public float CameraDistanceDelta;
    public String SeasonColorTexture = "";
    public int FaceDirection;
    public float Moved;
    public float Speed = 0.04f;
    public bool CanMove;
    public bool IsDancing;
    public float Opacity = 1.0f;
    public BoundingBox BoundingBox;
    public Vector3 BoundingBoxScale = new Vector3(1.25f);
    public BoundingBox ViewBox;
    public Vector3 ViewBoxScale = Vector3.One;
    public float CameraDistance;
    public Matrix World;
    public bool CreatedWorld;
    public bool CreateWorldEveryFrame;
    public bool Collision = true;
    public bool CanBeRemoved;
    public bool NeedsUpdate;
    public BaseModel? BaseModel;
    public Model? Model;
    public int HasEqualTextures = -1;
    public bool _visibleLastFrame;
    public bool _occluded;

    private float _normalOpacity = 1.0f;
    public float NormalOpacity
    {
        get => _normalOpacity;
        set { Opacity = value; _normalOpacity = value; }
    }

    private Vector3 _boundingPositionCreated = new Vector3(1110);
    private Vector3 _boundingRotationCreated = new Vector3(-1);
    private bool _drawnLastFrame = true;
    protected bool DropUpdateUnlessDrawn = true;
    private int _cachedVertexCount = -1;
    private Vector3 _tempCenterVector = Vector3.Zero;

    public Entity() : base(EntityTypes.Entity) { }

    public Entity(float x, float y, float z, String entityID, Texture2D[] textures,
                  int[] textureIndex, bool collision, int rotation, Vector3 scale,
                  BaseModel baseModel, int actionValue, String additionalValue,
                  Vector3 shader, String modelPath = "")
        : base(EntityTypes.Entity)
    {
        Position = new Vector3(x, y, z);
        EntityID = entityID;
        Textures = textures;
        TextureIndex = textureIndex;
        Collision = collision;
        Rotation = GetRotationFromInteger(rotation);
        Scale = scale;
        BaseModel = baseModel;
        ModelPath = modelPath;
        ActionValue = actionValue;
        AdditionalValue = additionalValue;
        Shader = shader;
        Initialize();
    }

    public virtual void Initialize()
    {
        int rotInt = GetRotationFromVector(Rotation);
        if (rotInt % 2 == 1)
        {
            ViewBox = new BoundingBox(
                Vector3.Transform(new Vector3(-(Scale.Z / 2), -(Scale.Y / 2), -(Scale.X / 2)),
                    Matrix.CreateScale(ViewBoxScale) * Matrix.CreateTranslation(Position)),
                Vector3.Transform(new Vector3(Scale.Z / 2, Scale.Y / 2, Scale.X / 2),
                    Matrix.CreateScale(ViewBoxScale) * Matrix.CreateTranslation(Position)));
        }
        else
        {
            ViewBox = new BoundingBox(
                Vector3.Transform(new Vector3(-(Scale.X / 2), -(Scale.Y / 2), -(Scale.Z / 2)),
                    Matrix.CreateScale(ViewBoxScale) * Matrix.CreateTranslation(Position)),
                Vector3.Transform(new Vector3(Scale.X / 2, Scale.Y / 2, Scale.Z / 2),
                    Matrix.CreateScale(ViewBoxScale) * Matrix.CreateTranslation(Position)));
        }

        BoundingBox = new BoundingBox(
            Vector3.Transform(new Vector3(-0.5f),
                Matrix.CreateScale(BoundingBoxScale) * Matrix.CreateTranslation(Position)),
            Vector3.Transform(new Vector3(0.5f),
                Matrix.CreateScale(BoundingBoxScale) * Matrix.CreateTranslation(Position)));

        _boundingPositionCreated = Position;
        _boundingRotationCreated = Rotation;

        if (_newRasterizerState == null)
        {
            _newRasterizerState = new RasterizerState { CullMode = CullMode.None };
            _oldRasterizerState = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace };
        }

        LoadSeasonTextures();
        UpdateEntity();
    }

    public static Entity GetNewEntity(String entityID, Vector3 position,
        Texture2D[] textures, int[] textureIndex, bool collision, Vector3 rotation,
        Vector3 scale, BaseModel? baseModel, int actionValue, String additionalValue,
        bool visible, Vector3 shader, int id, String mapOrigin, String seasonColorTexture,
        Vector3 offset, Object[]? parameters = null, float opacity = 1.0f,
        List<List<int>>? animationData = null, float cameraDistanceDelta = 0f,
        String modelPath = "")
    {
        Entity newEnt = new Entity();
        Entity props = new Entity();
        props.EntityID = entityID;
        props.Position = position;
        props.Textures = textures;
        props.TextureIndex = textureIndex;
        props.Collision = collision;
        props.Rotation = rotation;
        props.Scale = scale;
        props.BaseModel = baseModel;
        props.ModelPath = modelPath;
        props.ActionValue = actionValue;
        props.AdditionalValue = additionalValue;
        props.Visible = visible;
        props.Shader = shader;
        props.ID = id;
        props.MapOrigin = mapOrigin;
        props.SeasonColorTexture = seasonColorTexture;
        props.Offset = offset;
        props.NormalOpacity = opacity;
        props.CameraDistanceDelta = cameraDistanceDelta;
        // TODO Phase 4: ModelManager.ModelExist / GetModel when ModelManager is ported

        switch (entityID.ToLower())
        {
            case "animatedblock":      newEnt = new AnimatedBlock();      SetProperties(ref newEnt, props); ((AnimatedBlock)newEnt).Initialize(animationData); break;
            case "wallblock":          newEnt = new WallBlock();          SetProperties(ref newEnt, props); ((WallBlock)newEnt).Initialize(); break;
            case "cube":
            case "allsidesobject":     newEnt = new AllSidesObject();     SetProperties(ref newEnt, props); ((AllSidesObject)newEnt).Initialize(); break;
            case "slideblock":         newEnt = new SlideBlock();         SetProperties(ref newEnt, props); ((SlideBlock)newEnt).Initialize(); break;
            case "halfslideblock":     newEnt = new HalfSlideBlock();     SetProperties(ref newEnt, props); ((HalfSlideBlock)newEnt).Initialize(); break;
            case "wallbill":           newEnt = new WallBill();           SetProperties(ref newEnt, props); ((WallBill)newEnt).Initialize(); break;
            case "signblock":          newEnt = new SignBlock();          SetProperties(ref newEnt, props); ((SignBlock)newEnt).Initialize(); break;
            case "warpblock":          newEnt = new WarpBlock();          SetProperties(ref newEnt, props); ((WarpBlock)newEnt).Initialize(); break;
            case "floor":              newEnt = new Floor();              SetProperties(ref newEnt, props); ((Floor)newEnt).Initialize(true, false, true); break;
            case "step":               newEnt = new StepBlock();         SetProperties(ref newEnt, props); ((StepBlock)newEnt).Initialize(); break;
            case "cuttree":            newEnt = new CutDownTree();        SetProperties(ref newEnt, props); ((CutDownTree)newEnt).Initialize(); break;
            case "water":              newEnt = new Water();              SetProperties(ref newEnt, props); ((Water)newEnt).Initialize(); break;
            case "grass":              newEnt = new Grass();              SetProperties(ref newEnt, props); ((Grass)newEnt).Initialize(); break;
            case "berryplant":         newEnt = new BerryPlant();         SetProperties(ref newEnt, props); ((BerryPlant)newEnt).Initialize(); break;
            case "loamysoil":          newEnt = new LoamySoil();          SetProperties(ref newEnt, props); ((LoamySoil)newEnt).Initialize(); break;
            case "itemobject":         newEnt = new ItemObject();         SetProperties(ref newEnt, props); ((ItemObject)newEnt).Initialize(); break;
            case "scriptblock":        newEnt = new ScriptBlock();        SetProperties(ref newEnt, props); ((ScriptBlock)newEnt).Initialize(); break;
            case "turningsign":        newEnt = new TurningSign();        SetProperties(ref newEnt, props); ((TurningSign)newEnt).Initialize(); break;
            case "apricornplant":      newEnt = new ApricornPlant();      SetProperties(ref newEnt, props); ((ApricornPlant)newEnt).Initialize(); break;
            case "headbutttree":       newEnt = new HeadbuttTree();       SetProperties(ref newEnt, props); ((HeadbuttTree)newEnt).Initialize(); break;
            case "smashrock":          newEnt = new SmashRock();          SetProperties(ref newEnt, props); ((SmashRock)newEnt).Initialize(); break;
            case "strengthrock":       newEnt = new StrengthRock();       SetProperties(ref newEnt, props); ((StrengthRock)newEnt).Initialize(); break;
            case "npc":
                newEnt = new NPC();
                SetProperties(ref newEnt, props);
                if (parameters != null && parameters.Length >= 7)
                {
                    ((NPC)newEnt).Initialize(
                        (String)parameters[0], (int)parameters[1],
                        (String)parameters[2], (int)parameters[3],
                        (bool)parameters[4], (String)parameters[5],
                        (List<Rectangle>)parameters[6]);
                }
                break;
            case "waterfall":          newEnt = new Waterfall();          SetProperties(ref newEnt, props); ((Waterfall)newEnt).Initialize(); break;
            case "whirlpool":          newEnt = new Whirlpool();          SetProperties(ref newEnt, props); ((Whirlpool)newEnt).Initialize(); break;
            case "strengthtrigger":    newEnt = new StrengthTrigger();    SetProperties(ref newEnt, props); ((StrengthTrigger)newEnt).Initialize(); break;
            case "modelentity":        newEnt = new ModelEntity();        SetProperties(ref newEnt, props); ((ModelEntity)newEnt).Initialize(); break;
            case "rotationtile":       newEnt = new RotationTile();       SetProperties(ref newEnt, props); ((RotationTile)newEnt).Initialize(); break;
            case "divetile":           newEnt = new DiveTile();           SetProperties(ref newEnt, props); ((DiveTile)newEnt).Initialize(); break;
            case "rockclimbentity":    newEnt = new RockClimbEntity();    SetProperties(ref newEnt, props); ((RockClimbEntity)newEnt).Initialize(); break;
            case "holeblock":          newEnt = new HoleBlock();          SetProperties(ref newEnt, props); ((HoleBlock)newEnt).Initialize(); break;

            default:
                break;
        }

        return newEnt;
    }

    internal static void SetProperties(ref Entity target, Entity src)
    {
        target.EntityID = src.EntityID;
        target.Position = src.Position;
        target.Textures = src.Textures;
        target.TextureIndex = src.TextureIndex;
        target.Collision = src.Collision;
        target.Rotation = src.Rotation;
        target.Scale = src.Scale;
        target.BaseModel = src.BaseModel;
        target.ModelPath = src.ModelPath;
        target.Model = src.Model;
        target.ActionValue = src.ActionValue;
        target.AdditionalValue = src.AdditionalValue;
        target.Visible = src.Visible;
        target.Shader = src.Shader;
        target.ID = src.ID;
        target.MapOrigin = src.MapOrigin;
        target.SeasonColorTexture = src.SeasonColorTexture;
        target.Offset = src.Offset;
        target.NormalOpacity = src.Opacity;
        target.CameraDistanceDelta = src.CameraDistanceDelta;
    }

    public static Vector3 GetRotationFromInteger(int i)
    {
        return i switch
        {
            0 => new Vector3(0, 0, 0),
            1 => new Vector3(0, MathHelper.PiOver2, 0),
            2 => new Vector3(0, MathHelper.Pi, 0),
            3 => new Vector3(0, MathHelper.Pi * 1.5f, 0),
            _ => Vector3.Zero
        };
    }

    public static int GetRotationFromVector(Vector3 v)
    {
        if (v.Y == 0) return 0;
        if (v.Y == MathHelper.PiOver2) return 1;
        if (v.Y == MathHelper.Pi) return 2;
        if (v.Y == MathHelper.Pi * 1.5f) return 3;
        return 0;
    }

    internal void LoadSeasonTextures()
    {
        if (String.IsNullOrEmpty(SeasonColorTexture) == true)
        {
            return;
        }
        // TODO Phase 7: World.GetSeasonTexture when World is fully ported
    }

    public virtual void Update() { }

    public void UpdateModel()
    {
        if (Model == null)
        {
            return;
        }
        ViewBox = new BoundingBox(
            Vector3.Transform(new Vector3(-1, -1, -1),
                Matrix.CreateScale(ViewBoxScale) * Matrix.CreateTranslation(Position)),
            Vector3.Transform(new Vector3(1, 1, 1),
                Matrix.CreateScale(ViewBoxScale) * Matrix.CreateTranslation(Position)));
        ApplyEffect();
    }

    public virtual void OpacityCheck()
    {
        if (CameraDistance > 10.0f)
        {
            Opacity = _normalOpacity;
            return;
        }

        String[] notNames = [ "Floor", "OwnPlayer", "Water", "Whirlpool", "Particle",
                               "OverworldPokemon", "ItemObject", "NetworkPokemon", "NetworkPlayer" ];

        if ("Overworld".Equals(Screen.Camera?.Name) && notNames.Contains(EntityID) == false)
        {
            Opacity = _normalOpacity;
            if (Screen.Camera is OverworldCamera oc && oc.ThirdPerson == true)
            {
                float? result = Screen.Camera.Ray.Intersects(BoundingBox);
                if (result.HasValue == true)
                {
                    if (result.Value < NEAR_OPACITY_DISTANCE + (oc.ThirdPersonOffset.Z - 1.5f))
                    {
                        Opacity = _normalOpacity - OPACITY_THRESHOLD;
                        if (Opacity < CLOSE_OPACITY_CLAMP)
                        {
                            Opacity = CLOSE_OPACITY_CLAMP;
                        }
                    }
                }
            }
        }
    }

    protected virtual Vector3 GetCameraDistanceCenterPoint()
    {
        return Position + GetCenter();
    }

    protected virtual float CalculateCameraDistance(Vector3 cameraPosition)
    {
        return Vector3.Distance(GetCameraDistanceCenterPoint(), cameraPosition) + CameraDistanceDelta;
    }

    public virtual void UpdateEntity()
    {
        Vector3 cameraPosition = Screen.Camera?.Position ?? Vector3.Zero;
        bool actionScriptActive = false;

        if (Core.CurrentScreen != null)
        {
            cameraPosition = Screen.Camera?.CPosition ?? Vector3.Zero;
            if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
            {
                actionScriptActive = ((OverworldScreen)Core.CurrentScreen).ActionScript.IsReady == false;
            }
        }

        CameraDistance = CalculateCameraDistance(cameraPosition);

        if (DropUpdateUnlessDrawn == true && _drawnLastFrame == false &&
            Visible == true && actionScriptActive == false)
        {
            return;
        }

        if (Moved > 0f && CanMove == true)
        {
            Moved -= Speed;
            Vector3 movement = FaceDirection switch
            {
                0 => new Vector3(0, 0, -1),
                1 => new Vector3(-1, 0, 0),
                2 => new Vector3(0, 0, 1),
                3 => new Vector3(1, 0, 0),
                _ => Vector3.Zero
            };
            movement *= Speed;
            Position += movement;
            CreatedWorld = false;

            if (Moved <= 0f)
            {
                Moved = 0f;
                Position.X = (int)Position.X;
                Position.Z = (int)Position.Z;
            }
        }

        if (IsOffsetMapContent == false)
        {
            OpacityCheck();
        }

        if (CreatedWorld == false || CreateWorldEveryFrame == true)
        {
            World = Matrix.CreateScale(Scale) *
                    Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                    Matrix.CreateTranslation(Position);
            CreatedWorld = true;
        }

        if (CameraDistance < (Screen.Camera?.FarPlane ?? 60f) * 2)
        {
            if (Position != _boundingPositionCreated)
            {
                float dx = _boundingPositionCreated.X - Position.X;
                float dy = _boundingPositionCreated.Y - Position.Y;
                float dz = _boundingPositionCreated.Z - Position.Z;

                ViewBox.Min -= new Vector3(dx, dy, dz);
                ViewBox.Max -= new Vector3(dx, dy, dz);
                BoundingBox.Min -= new Vector3(dx, dy, dz);
                BoundingBox.Max -= new Vector3(dx, dy, dz);

                _boundingPositionCreated = Position;
            }
        }

        if (MakeShake == true)
        {
            if (Core.Random.Next(0, 2) == 0)
            {
                float jitter() => (Core.Random.Next(1, 6) - SHAKE_RANGE) / SHAKE_SCALE;
                Rotation += new Vector3(jitter(), jitter(), jitter());
                Position += new Vector3(jitter(), jitter(), jitter());
                Scale += new Vector3(jitter(), jitter(), jitter());
                CreatedWorld = false;
            }
        }

        // TODO Phase 4: lighting updates when Lighting class is ported
        Shader = Vector3.One;

        for (int s = 0; s < Shaders.Count; s++)
        {
            if (ShadersDisableWhenNoLighting[s] == false || Core.GameOptions.LightingEnabled == true)
            {
                Shader *= Shaders[s];
            }
        }
    }

    private Vector3 GetCenter()
    {
        if (CreatedWorld == false || CreateWorldEveryFrame == true)
        {
            Vector3 v = Vector3.Zero;
            if (BaseModel != null)
            {
                switch (BaseModel.ID)
                {
                    case 0:
                    case 9:
                    case 10:
                    case 11:
                        v.Y -= 0.5f;
                        break;
                }
            }
            _tempCenterVector = v;
        }
        return _tempCenterVector;
    }

    public virtual void Draw(BaseModel? baseModel, Texture2D[] textures,
                              bool setRasterizerState, Model? model = null)
    {
        if (Visible == false)
        {
            _drawnLastFrame = false;
            return;
        }

        if (model != null)
        {
            Core.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            model.Draw(World, Screen.Camera!.View, Screen.Camera.Projection);
            Core.GraphicsDevice.SamplerStates[0] = Core.Sampler;
            if (drawViewBox == true)
            {
                BoundingBoxRenderer.Render(ViewBox, Core.GraphicsDevice,
                    Screen.Camera.View, Screen.Camera.Projection, Microsoft.Xna.Framework.Color.Red);
            }
        }
        else
        {
            if (IsInFieldOfView() == true)
            {
                if (setRasterizerState == true)
                {
                    Core.GraphicsDevice.RasterizerState = _newRasterizerState!;
                }
                baseModel?.Draw(this, textures);
                if (setRasterizerState == true)
                {
                    Core.GraphicsDevice.RasterizerState = _oldRasterizerState!;
                }
                _drawnLastFrame = true;

                if (EntityID.Equals("Floor") == false && EntityID.Equals("Water") == false && drawViewBox == true)
                {
                    BoundingBoxRenderer.Render(ViewBox, Core.GraphicsDevice,
                        Screen.Camera!.View, Screen.Camera.Projection,
                        Microsoft.Xna.Framework.Color.LightCoral);
                }
            }
            else
            {
                _drawnLastFrame = false;
            }
        }
    }

    public virtual void Render()
    {
        UpdateModel();
    }

    public virtual void ClickFunction() { }
    public virtual bool WalkAgainstFunction() => true;
    public virtual bool WalkIntoFunction() => false;
    public virtual void WalkOntoFunction() { }
    public virtual void ResultFunction(int result) { }
    public virtual bool LetPlayerMove() => true;

    public bool IsInFieldOfView()
    {
        if (Screen.Camera?.BoundingFrustum.Contains(ViewBox) != ContainmentType.Disjoint)
        {
            _visibleLastFrame = true;
            return true;
        }
        _visibleLastFrame = false;
        return false;
    }

    public int VertexCount
    {
        get
        {
            if (_cachedVertexCount != -1)
            {
                return _cachedVertexCount;
            }
            if (BaseModel != null)
            {
                int faceCount = BaseModel.VertexBuffer?.VertexCount / 3 ?? 0;
                int count = 0;
                for (int i = 0; i < TextureIndex.Length && i < faceCount; i++)
                {
                    if (TextureIndex[i] > -1)
                    {
                        count++;
                    }
                }
                _cachedVertexCount = count;
            }
            else
            {
                _cachedVertexCount = 0;
            }
            return _cachedVertexCount;
        }
    }

    protected Entity? GetEntity(List<Entity> list, Vector3 position, bool intComparison, Type[] validTypes)
    {
        foreach (Entity e in list.Where(e => validTypes.Contains(e.GetType())))
        {
            if (intComparison == true)
            {
                if ((int)e.Position.X == (int)position.X &&
                    (int)e.Position.Y == (int)position.Y &&
                    (int)e.Position.Z == (int)position.Z)
                {
                    return e;
                }
            }
            else
            {
                if (e.Position == position)
                {
                    return e;
                }
            }
        }
        return null;
    }

    public void ApplyEffect()
    {
        if (Model == null || Screen.Effect == null)
        {
            return;
        }
        foreach (ModelMesh mesh in Model.Meshes)
        {
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                if (part.Effect is BasicEffect be)
                {
                    be.Alpha = Opacity;
                    be.DiffuseColor = Screen.Effect.DiffuseColor * Shader * Color;
                    be.FogEnabled = true;
                    be.FogColor = Screen.Effect.FogColor;
                    be.FogEnd = Screen.Effect.FogEnd;
                    be.FogStart = Screen.Effect.FogStart;
                }
            }
        }
    }
}
