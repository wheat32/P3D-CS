using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class MapPreviewCamera : Camera
{
    public float oldX, oldY;

    public MapPreviewCamera() : base("MapPreview")
    {
        Position = MapPreviewScreen.MapViewModePosition;

        RotationSpeed = 0.002F;
        FOV = 65;

        Yaw = 0.0F;
        if ((int)Yaw == (int)MathHelper.TwoPi) Yaw = 0;
        Pitch = -0.4432F;
        Speed = 0.2F;

        Turning = false;

        View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), Core.GraphicsDevice.Viewport.AspectRatio, 0.01f, FarPlane);

        UpdateMatrices();
        UpdateFrustum();
        Ray = CreateRay();
        ResetCursor();
    }

    public override void Update()
    {
        Ray = CreateRay();

        ChangeSpeed();
        TurnCamera();
        MoveCamera();

        UpdateFrustum();
        UpdateMatrices();
        ResetCursor();
    }

    private void ChangeSpeed()
    {
        if (Controls.Up(false, false, true, false, false, false) == true) { Speed += 0.01F; RotationSpeed += 0.0001F; }
        if (Controls.Down(false, false, true, false, false, false) == true) { Speed -= 0.01F; RotationSpeed -= 0.0001F; }
        Speed = Math.Clamp(Speed, 0.0F, 1.0F);
        RotationSpeed = Math.Clamp(RotationSpeed, 0.001F, 0.007F);
    }

    private void TurnCamera()
    {
        MouseState mState = Mouse.GetState();
        GamePadState gState = GamePad.GetState(PlayerIndex.One);

        float dx = mState.X - oldX;
        if (gState.ThumbSticks.Right.X != 0.0F && Core.GameOptions.GamePadEnabled == true)
            dx = gState.ThumbSticks.Right.X * 50.0F;

        float dy = mState.Y - oldY;
        if (gState.ThumbSticks.Right.Y != 0.0F && Core.GameOptions.GamePadEnabled == true)
            dy = gState.ThumbSticks.Right.Y * 40.0F * -1.0F;

        Yaw += -RotationSpeed * 0.75F * dx;

        while (Yaw > MathHelper.TwoPi) Yaw -= MathHelper.TwoPi;
        while (Yaw < 0) Yaw += MathHelper.TwoPi;

        Pitch += -RotationSpeed * dy;
        Pitch = MathHelper.Clamp(Pitch, -1.5f, 1.5f);
    }

    private void MoveCamera()
    {
        if (Controls.Up(false, false, false, true, true, false) == true)
        {
            Matrix rotM = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0.0F);
            Position += Vector3.Transform(Vector3.Forward, rotM) * Speed;
        }
        if (Controls.Down(false, false, false, true, true, false) == true)
        {
            Matrix rotM = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0.0F);
            Position += Vector3.Transform(Vector3.Backward, rotM) * Speed;
        }
        if (Controls.Left(false, false, false, true, true, false) == true)
        {
            Matrix rotM = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0.0F);
            Position += Vector3.Transform(Vector3.Left, rotM) * Speed;
        }
        if (Controls.Right(false, false, false, true, true, false) == true)
        {
            Matrix rotM = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0.0F);
            Position += Vector3.Transform(Vector3.Right, rotM) * Speed;
        }
    }

    public void ResetCursor()
    {
        if (GameController.IsActiveWindow() == true)
        {
            Mouse.SetPosition(Core.windowSize.Width / 2, Core.windowSize.Height / 2);
            oldX = Core.windowSize.Width / 2;
            oldY = Core.windowSize.Height / 2;
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
        int centerX = Core.windowSize.Width / 2;
        int centerY = Core.windowSize.Height / 2;

        Vector3 nearSource = new Vector3(centerX, centerY, 0);
        Vector3 farSource = new Vector3(centerX, centerY, 1);

        Vector3 nearPoint = Core.GraphicsDevice.Viewport.Unproject(nearSource, Projection, View, Matrix.Identity);
        Vector3 farPoint = Core.GraphicsDevice.Viewport.Unproject(farSource, Projection, View, Matrix.Identity);

        Vector3 direction = farPoint - nearPoint;
        direction.Normalize();

        return new Ray(nearPoint, direction);
    }
}
