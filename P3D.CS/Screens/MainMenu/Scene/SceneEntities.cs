using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Screens.MainMenu.Scene;

// TODO Phase 9: full 3D scene entity rendering port (requires GameDevCommon geometry / shader replacement)

public class Clouds : MainMenuEntity
{
    public Clouds() : base(new Vector3(0, -13, 0)) { }
    public override void Update() { Position.X += 0.2f; if (Position.X >= 32.0f) Position.X = 0f; }
}

public class Ground : MainMenuEntity
{
    public Ground() : base(new Vector3(0, -13, 0)) { }
    public override void Update() { Position.X += 0.2f; if (Position.X >= 32.0f) Position.X = 0f; Position.Y = -50; }
}

public class HoOhParticle : MainMenuEntity
{
    public HoOhParticle(Vector3 position, int texVariant) : base(position) { IsOpaque = false; }
    public override void Update() { Position.X += 1; Alpha -= 0.02f; if (Alpha <= 0f) ToBeRemoved = true; }
}

public class HoOh : MainMenuEntity
{
    private List<MainMenuEntity> _entities;
    private static Random _random = new Random();
    private int _animationDelay = 10;

    public HoOh(List<MainMenuEntity> entities) : base(new Vector3(0, -13, 1))
    {
        Rotation = new Vector3(0, -0.2f, 0);
        _entities = entities;
    }

    public override void Update()
    {
        _animationDelay -= 1;
        if (_animationDelay == 0)
        {
            _animationDelay = 8;
            for (int i = 0; i <= _random.Next(1, 4); i++)
            {
                HoOhParticle particle = new HoOhParticle(
                    Position + new Vector3(6, 0, 0) + new Vector3(_random.Next(-3, 4), _random.Next(-3, 4), 0), _random.Next(0, 4));
                particle.LoadContent();
                _entities.Add(particle);
            }
        }
    }
}

public class LugiaParticle : MainMenuEntity
{
    private float _phase;
    public LugiaParticle(Vector3 position) : base(position) { IsOpaque = false; }
    public override void Update() { _phase += 0.4f; Position.X += 1.2f; Position.Y += (float)Math.Sin(_phase); Alpha -= 0.018f; if (Alpha <= 0f) ToBeRemoved = true; }
}

public class Lugia : MainMenuEntity
{
    private List<MainMenuEntity> _entities;
    private static Random _random = new Random();
    private int _animationDelay = 10;

    public Lugia(List<MainMenuEntity> entities) : base(new Vector3(0, -13, 1))
    {
        Rotation = new Vector3(0, -0.2f, 0);
        _entities = entities;
    }

    public override void Update()
    {
        _animationDelay -= 1;
        if (_animationDelay == 0)
        {
            _animationDelay = 12;
            for (int i = 0; i <= _random.Next(1, 4); i++)
            {
                LugiaParticle particle = new LugiaParticle(Position + new Vector3(10, -5, 0));
                particle.LoadContent();
                _entities.Add(particle);
            }
        }
    }
}

public class MainMenuCamera
{
    public float Pitch;
    public float FOV = 45;
    public Microsoft.Xna.Framework.Vector3 Position;
    public float Yaw;

    public MainMenuCamera()
    {
        Pitch = 0;
        FOV = 45;
        Position = new Microsoft.Xna.Framework.Vector3(0, 0, 100);
    }

    public void Update() { Yaw = -0.1f; }
}
