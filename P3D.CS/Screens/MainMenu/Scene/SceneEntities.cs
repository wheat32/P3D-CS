using GameDevCommon;
using GameDevCommon.Rendering;
using GameDevCommon.Rendering.Composers;
using GameDevCommon.Rendering.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Screens.MainMenu.Scene;

public class Clouds : MainMenuEntity
{
    public Clouds() : base(new Vector3(0, -13, 0)) { }

    public override void LoadContent()
    {
        Texture = P3D.TextureManager.LoadDirect(@"GUI\MainMenu\clouds.png");
        base.LoadContent();
    }

    public override void Update()
    {
        Position.X += 0.2f;
        if (Position.X >= 32.0f)
            Position.X = 0f;
        CreateWorld();
    }

    protected override void CreateGeometry()
    {
        var vertices = RectangleComposer.Create(320, 22, new TextureRectangle(0, 0, 10, 1));
        VertexTransformer.Rotate(vertices, new Vector3(MathHelper.PiOver2, 0, 0));
        Geometry.AddVertices(vertices);
    }
}

public class Ground : MainMenuEntity
{
    public Ground() : base(new Vector3(0, -13, 0)) { }

    public override void LoadContent()
    {
        Texture = P3D.TextureManager.LoadDirect(@"GUI\MainMenu\ground.png");
        base.LoadContent();
    }

    public override void Update()
    {
        Position.X += 0.2f;
        if (Position.X >= 32.0f)
            Position.X = 0f;
        Position.Y = -50;
        CreateWorld();
    }

    protected override void CreateGeometry()
    {
        var vertices = RectangleComposer.Create(320, 96, new TextureRectangle(0, 0, 10, 1));
        VertexTransformer.Rotate(vertices, new Vector3(MathHelper.PiOver2, 0, 0));
        Geometry.AddVertices(vertices);
    }
}

public class HoOhParticle : MainMenuEntity
{
    private static Texture2D? _particleSource;
    private static readonly Dictionary<int, Texture2D> _particleTextures = new();
    private readonly int _texVariant;

    public HoOhParticle(Vector3 position, int texVariant) : base(position)
    {
        _texVariant = texVariant;
        IsOpaque = false;
    }

    public override void LoadContent()
    {
        if (_particleSource == null)
        {
            _particleSource = P3D.TextureManager.LoadDirect(@"GUI\MainMenu\hoohParticles.png");
            for (int i = 0; i < 4; i++)
            {
                var colors = new Color[5 * 5];
                _particleSource.GetData(0, new Rectangle(i * 5, 0, 5, 5), colors, 0, colors.Length);
                var tex = new Texture2D(GameInstanceProvider.Instance.GraphicsDevice, 5, 5);
                tex.SetData(colors);
                _particleTextures[i] = tex;
            }
        }
        Texture = _particleTextures[_texVariant];
        base.LoadContent();
    }

    public override void Update()
    {
        Position.X += 1;
        Alpha -= 0.02f;
        if (Alpha <= 0f)
            ToBeRemoved = true;
        CreateWorld();
    }

    protected override void CreateGeometry()
    {
        var vertices = RectangleComposer.Create(4, 4);
        VertexTransformer.Rotate(vertices, new Vector3(MathHelper.PiOver2, 0, 0));
        Geometry.AddVertices(vertices);
    }
}

public class HoOh : MainMenuEntity
{
    private static readonly Random _random = new();
    private readonly List<MainMenuEntity> _entities;
    private Texture2D[]? _textures;
    private int _textureIndex;
    private int _animationDelay = 10;

    public HoOh(List<MainMenuEntity> entities) : base(new Vector3(0, -13, 1))
    {
        Rotation = new Vector3(0, -0.2f, 0);
        _entities = entities;
    }

    public override void LoadContent()
    {
        var t = P3D.TextureManager.LoadDirect(@"GUI\MainMenu\hooh.png");
        var colors = new Color[80 * 48];
        var textures = new List<Texture2D>();
        for (int x = 0; x <= 4; x++)
        {
            t.GetData(0, new Rectangle(x * 80, 0, 80, 48), colors, 0, colors.Length);
            var tex = new Texture2D(GameInstanceProvider.Instance.GraphicsDevice, 80, 48);
            tex.SetData(colors);
            textures.Add(tex);
        }
        _textures = textures.ToArray();
        Texture = _textures[_textureIndex];
        base.LoadContent();
    }

    public override void Update()
    {
        _animationDelay--;
        if (_animationDelay == 0)
        {
            _animationDelay = 8;
            _textureIndex++;
            if (_textureIndex == _textures!.Length)
                _textureIndex = 0;
            Texture = _textures[_textureIndex];

            for (int i = 0; i <= _random.Next(1, 4); i++)
            {
                var particle = new HoOhParticle(
                    Position + new Vector3(6, GetParticleY() * 10, 0) +
                    new Vector3(_random.Next(-3, 4), _random.Next(-3, 4), 0),
                    _random.Next(0, 4));
                particle.LoadContent();
                _entities.Add(particle);
            }
        }
        CreateWorld();
    }

    protected override void CreateGeometry()
    {
        var vertices = RectangleComposer.Create(37, 22);
        VertexTransformer.Rotate(vertices, new Vector3(MathHelper.PiOver2, 0, 0));
        Geometry.AddVertices(vertices);
    }

    private int GetParticleY() => _textureIndex switch
    {
        0 => 1,
        1 => 0,
        2 => -1,
        3 => 1,
        _ => 1
    };
}

public class LugiaParticle : MainMenuEntity
{
    private static Texture2D? _particleTexture;
    private float _phase;

    public LugiaParticle(Vector3 position) : base(position)
    {
        IsOpaque = false;
    }

    public override void LoadContent()
    {
        if (_particleTexture == null)
            _particleTexture = P3D.TextureManager.LoadDirect(@"GUI\MainMenu\lugiaParticle.png");
        Texture = _particleTexture;
        base.LoadContent();
    }

    public override void Update()
    {
        _phase += 0.4f;
        Position.X += 1.2f;
        Position.Y += (float)Math.Sin(_phase);
        Alpha -= 0.018f;
        if (Alpha <= 0f)
            ToBeRemoved = true;
        CreateWorld();
    }

    protected override void CreateGeometry()
    {
        var vertices = RectangleComposer.Create(8, 8);
        VertexTransformer.Rotate(vertices, new Vector3(MathHelper.PiOver2, 0, 0));
        Geometry.AddVertices(vertices);
    }
}

public class Lugia : MainMenuEntity
{
    private static readonly Random _random = new();
    private static readonly int[] _frames = { 0, 0, 0, 1, 2, 3 };
    private readonly List<MainMenuEntity> _entities;
    private Texture2D[]? _textures;
    private int _textureIndex;
    private int _animationDelay = 10;

    public Lugia(List<MainMenuEntity> entities) : base(new Vector3(0, -13, 1))
    {
        Rotation = new Vector3(0, -0.2f, 0);
        _entities = entities;
    }

    public override void LoadContent()
    {
        var t = P3D.TextureManager.LoadDirect(@"GUI\MainMenu\lugia.png");
        var colors = new Color[80 * 48];
        var textures = new List<Texture2D>();
        for (int x = 0; x <= 3; x++)
        {
            t.GetData(0, new Rectangle(x * 80, 0, 80, 48), colors, 0, colors.Length);
            var tex = new Texture2D(GameInstanceProvider.Instance.GraphicsDevice, 80, 48);
            tex.SetData(colors);
            textures.Add(tex);
        }
        _textures = textures.ToArray();
        Texture = _textures[0];
        base.LoadContent();
    }

    public override void Update()
    {
        _animationDelay--;
        if (_animationDelay == 0)
        {
            _animationDelay = 12;
            _textureIndex++;
            if (_textureIndex == _frames.Length)
                _textureIndex = 0;
            Texture = _textures![_frames[_textureIndex]];

            for (int i = 0; i <= _random.Next(1, 4); i++)
            {
                var particle = new LugiaParticle(Position + new Vector3(10, -5, 0));
                particle.LoadContent();
                _entities.Add(particle);
            }
        }
        CreateWorld();
    }

    protected override void CreateGeometry()
    {
        var vertices = RectangleComposer.Create(37, 22);
        VertexTransformer.Rotate(vertices, new Vector3(MathHelper.PiOver2, 0, 0));
        Geometry.AddVertices(vertices);
    }
}

public class MainMenuCamera : GameDevCommon.Rendering.PerspectiveCamera
{
    public MainMenuCamera()
    {
        Position = new Vector3(0, 0, 100);
        FOV = 45;
        Pitch = 0;
        Yaw = -0.1f;
        CreateProjection();
        CreateView();
    }

    public override void Update()
    {
        Yaw = -0.1f;
        CreateView();
    }
}
