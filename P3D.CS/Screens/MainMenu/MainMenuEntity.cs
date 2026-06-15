using GameDevCommon.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Screens.MainMenu;

public abstract class MainMenuEntity : Base3DObject<VertexPositionNormalTexture>
{
    public Vector3 Position;
    public Vector3 Rotation;
    public bool ToBeRemoved;

    protected MainMenuEntity(Vector3 position)
    {
        Position = position;
        Rotation = Vector3.Zero;
    }

    protected override void CreateWorld()
    {
        World = Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z) *
                Matrix.CreateTranslation(Position);
    }
}
