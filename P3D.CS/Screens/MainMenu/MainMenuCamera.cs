using Microsoft.Xna.Framework;

namespace P3D;

public class MainMenuCamera : Camera
{
    public MainMenuCamera() : base("MainMenu")
    {
        Position = new Vector3(13, 2, 14);
        Speed = 0.0008f;

        Yaw = (float)(Core.Random.NextDouble() * MathHelper.TwoPi);
        Pitch = -0.2f;

        View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45.0f),
            Core.GraphicsDevice.Viewport.AspectRatio,
            0.01f, 16f);

        Update();
    }

    public override void Update()
    {
        Ray = CreateRay();

        Yaw -= Speed;
        while (Yaw <= 0)
        {
            Yaw = MathHelper.TwoPi;
        }

        UpdateMatrices();
        UpdateFrustrum();
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

    private void UpdateFrustrum()
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
        Vector3 lookAt = Position + transformed;
        View = Matrix.CreateLookAt(Position, lookAt, Vector3.Up);
    }
}
