using Microsoft.Xna.Framework;

namespace P3D;

public class NewGameCamera : OverworldCamera
{
    public NewGameCamera(Vector3 startPosition, float startYaw, float startPitch)
    {
        Name = "New Game";
        Core.Player.StartThirdPerson = false;
        Position = startPosition;
        Speed = 0.0008f;

        Yaw = startYaw;
        Pitch = startPitch;

        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 transformed = Vector3.Transform(new Vector3(0, 0, 0), rotation);
        Vector3 lookAt = new Vector3(Position.X, Position.Y, Position.Z) + transformed;

        View = Matrix.CreateLookAt(Position, lookAt, Vector3.Up);
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45.0f),
            Core.GraphicsDevice.Viewport.AspectRatio,
            0.01f, 16f);
        FOV = 45f;

        CreateProjectionMatrix();
        _canToggleThirdPerson = false;
    }

    private void CreateProjectionMatrix()
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(FOV),
            Core.GraphicsDevice.Viewport.AspectRatio,
            0.01f, FarPlane);
    }

    public override void Update()
    {
        Ray = CreateRay();
        UpdateViewMatrix();
        UpdateMatrices();
        UpdateFrustum();
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

    public void UpdateMatrices()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 transformed = Vector3.Transform(new Vector3(0, 0, -1), rotation);
        Vector3 lookAt = Position + transformed;
        View = Matrix.CreateLookAt(Position, lookAt, Vector3.Up);
    }
}
