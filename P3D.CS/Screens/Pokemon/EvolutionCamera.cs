using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;

namespace P3D;

public class EvolutionCamera : Camera
{
    public float OldX, OldY;

    private float _scrollSpeed = 0.0F;
    private int _multi = 1;

    public EvolutionCamera() : base("Evolution")
    {
        Position = new Vector3(0, 1.86F, 3.3F);
        RotationSpeed = (float)(Core.Player.StartRotationSpeed / 10000);
        FOV = 65;
        Yaw = 0.0F;
        if ((int)Yaw == (int)MathHelper.TwoPi)
            Yaw = 0;
        Pitch = -0.4432F;
        Turning = false;

        View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), Core.GraphicsDevice.Viewport.AspectRatio, 0.01F, FarPlane);

        UpdateMatrices();
        UpdateFrustum();
        Ray = CreateRay();
        ResetCursor();
    }

    public override void Update()
    {
        Ray = CreateRay();
        Pitch = MathHelper.Clamp(Pitch, -1.5F, 1.5F);
        ScrollThirdPerson();
        UpdateFrustum();
        UpdateMatrices();
        ResetCursor();
    }

    private void ScrollThirdPerson()
    {
        if (Controls.Down(true, false, true, false, false) == true)
        {
            if (_scrollSpeed == 0.0F || _multi != 1)
                _scrollSpeed = 0.01F;
            _multi = 1;
            _scrollSpeed += _scrollSpeed.Clamp(0, 0.01F);
        }

        if (Controls.Up(true, false, true, false, false) == true)
        {
            if (_scrollSpeed == 0.0F || _multi != -1)
                _scrollSpeed = 0.01F;
            _multi = -1;
            _scrollSpeed += _scrollSpeed.Clamp(0, 0.01F);
        }

        _scrollSpeed = _scrollSpeed.Clamp(0, 0.08F);

        if (_scrollSpeed > 0.0F)
        {
            Vector3 pos = Position;
            pos.Y += _scrollSpeed * _multi;
            pos.Z += _scrollSpeed * _multi;
            pos.Y = pos.Y.Clamp(1.0F, 4.7F);
            pos.Z = pos.Z.Clamp(1.0F, 6.0F);
            Position = pos;

            _scrollSpeed -= 0.001F;
            if (_scrollSpeed <= 0.0F)
                _scrollSpeed = 0.0F;
        }
    }

    public void ResetCursor()
    {
        if (GameController.IsActiveWindow() == true)
        {
            Mouse.SetPosition((int)(Core.windowSize.Width / 2), (int)(Core.windowSize.Height / 2));
            OldX = (int)(Core.windowSize.Width / 2);
            OldY = (int)(Core.windowSize.Height / 2);
        }
    }

    public void UpdateFrustum()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 fPosition = new Vector3(Position.X, Position.Y, Position.Z);
        Vector3 transformed = Vector3.Transform(new Vector3(0, 0, -1), rotation);
        Vector3 lookAt = fPosition + transformed;
        BoundingFrustum = new BoundingFrustum(Matrix.CreateLookAt(fPosition, lookAt, Vector3.Up) * Projection);
    }

    public void UpdateMatrices()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 transformed = Vector3.Transform(new Vector3(0, 0, -1), rotation);
        Vector3 lookAt = new Vector3(Position.X, Position.Y, Position.Z) + transformed;
        View = Matrix.CreateLookAt(Position, lookAt, Vector3.Up);
    }

    public Ray CreateRay()
    {
        int centerX = (int)(Core.windowSize.Width / 2);
        int centerY = (int)(Core.windowSize.Height / 2);

        Vector3 nearSource = new Vector3(centerX, centerY, 0);
        Vector3 farSource = new Vector3(centerX, centerY, 1);

        Vector3 nearPoint = Core.GraphicsDevice.Viewport.Unproject(nearSource, Projection, View, Matrix.Identity);
        Vector3 farPoint = Core.GraphicsDevice.Viewport.Unproject(farSource, Projection, View, Matrix.Identity);

        Vector3 direction = farPoint - nearPoint;
        direction.Normalize();

        return new Ray(nearPoint, direction);
    }
}
