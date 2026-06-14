using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Screens.MainMenu;

// TODO Phase 9: full 3D entity rendering port (replaces GameDevCommon.Rendering.Base3DObject)
public class MainMenuEntity
{
    public Vector3 Position;
    public Vector3 Rotation;
    public bool ToBeRemoved;
    public float Alpha = 1.0f;
    public bool IsOpaque = true;
    public Texture2D? Texture;

    public MainMenuEntity(Vector3 position)
    {
        Position = position;
        Rotation = Vector3.Zero;
    }

    public virtual void LoadContent() { }
    public virtual void Update() { }
    public virtual void Dispose() { }
}
