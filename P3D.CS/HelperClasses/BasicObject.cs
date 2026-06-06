using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

/// <summary>Base class for all graphics components.</summary>
public abstract class BasicObject
{
    public Texture2D? Texture { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Vector2 Position { get; set; }
    public bool Visible { get; set; } = true;
    public bool DisposeReady { get; set; }

    public Size Size
    {
        get => new Size(Width, Height);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    protected BasicObject(Texture2D? texture, int width, int height, Vector2 position)
    {
        Texture = texture;
        Width = width;
        Height = height;
        Position = position;
    }
}
