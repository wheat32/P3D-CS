using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class SecretBaseCamera : Camera
{
    private const float DEFAULT_PITCH = -0.4f;
    private const float DEFAULT_FOV = 60f;
    private const float DEFAULT_SPEED = 1f;
    private const float DEFAULT_ROTATION_SPEED = 0.04f;
    private const float INITIAL_X = 3f;
    private const float INITIAL_Y = 2f;
    private const float INITIAL_Z = 3f;

    private int _oldX;
    private int _oldY;

    public SecretBaseCamera() : base("SecretBase")
    {
        Position = new Vector3(INITIAL_X, INITIAL_Y, INITIAL_Z);
        RotationSpeed = DEFAULT_ROTATION_SPEED;
        FOV = DEFAULT_FOV;
        Speed = DEFAULT_SPEED;

        Yaw = 0f;
        Pitch = DEFAULT_PITCH;

        View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(FOV),
            Core.GraphicsDevice.Viewport.AspectRatio,
            0.01f,
            FarPlane);

        UpdateMatrices();
        UpdateFrustum();
        Ray = CreateRay();
        ResetCursor();
    }

    public override void Update()
    {
        Ray = CreateRay();
        MoveCamera();
        UpdateMatrices();
        UpdateFrustum();
        ResetCursor();
    }

    public void MoveCamera()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Controls.Left(true, true, false, true, true, true) == true)
        {
            moveX -= Speed;
        }
        if (Controls.Right(true, true, false, true, true, true) == true)
        {
            moveX += Speed;
        }
        if (Controls.Up(true, true, false, true, true, true) == true)
        {
            moveZ -= Speed;
        }
        if (Controls.Down(true, true, false, true, true, true) == true)
        {
            moveZ += Speed;
        }

        if (Position.X + moveX < 0f)
        {
            moveX = 0f;
            Position = new Vector3(0f, Position.Y, Position.Z);
        }
        if (Position.Z + moveZ < 0f)
        {
            moveZ = 0f;
            Position = new Vector3(Position.X, Position.Y, 0f);
        }

        Position = new Vector3(Position.X + moveX, Position.Y, Position.Z + moveZ);
    }

    public void ResetCursor()
    {
        if (GameController.IsActiveWindow() == true)
        {
            Mouse.SetPosition((int)(Core.windowSize.Width / 2), (int)(Core.windowSize.Height / 2));
            _oldX = (int)(Core.windowSize.Width / 2);
            _oldY = (int)(Core.windowSize.Height / 2);
        }
    }

    public void UpdateFrustum()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 transformed = Vector3.Transform(new Vector3(0f, 0f, -1f), rotation);
        Vector3 lookAt = Position + transformed;
        BoundingFrustum = new BoundingFrustum(Matrix.CreateLookAt(Position, lookAt, Vector3.Up) * Projection);
    }

    public void UpdateMatrices()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 transformed = Vector3.Transform(new Vector3(0f, 0f, -1f), rotation);
        Vector3 lookAt = Position + transformed;
        View = Matrix.CreateLookAt(Position, lookAt, Vector3.Up);
    }

    public Ray CreateRay()
    {
        int centerX = (int)(Core.windowSize.Width / 2);
        int centerY = (int)(Core.windowSize.Height / 2);

        Vector3 nearSource = new Vector3(centerX, centerY, 0f);
        Vector3 farSource = new Vector3(centerX, centerY, 1f);

        Vector3 nearPoint = Core.GraphicsDevice.Viewport.Unproject(nearSource, Projection, View, Matrix.Identity);
        Vector3 farPoint = Core.GraphicsDevice.Viewport.Unproject(farSource, Projection, View, Matrix.Identity);

        Vector3 direction = farPoint - nearPoint;
        direction.Normalize();

        return new Ray(nearPoint, direction);
    }
}
