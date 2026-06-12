using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;

namespace P3D.BattleSystem;

public class BattleCamera : Camera
{
    private const float DEFAULT_SPEED = 0.04f;
    private const float DEFAULT_ROTATION_SPEED = 0.008f;
    private const float DEFAULT_FOV = 60.0f;
    private const float DEFAULT_YAW = (float)(Math.PI / 4);
    private const float DEFAULT_PITCH = -0.6f;
    private const float SPEED_STEP = 0.005f;

    public Vector3 TargetPosition;
    public bool TargetMode = true;

    public float TargetYaw = DEFAULT_YAW;
    public float TargetSpeed = DEFAULT_SPEED;
    public float TargetRotationSpeed = DEFAULT_ROTATION_SPEED;
    public float TargetPitch = DEFAULT_PITCH;

    public bool IsReady
    {
        get
        {
            return Position.Equals(TargetPosition) &&
                   TargetSpeed.Equals(Speed) &&
                   TargetYaw.Equals(Yaw) &&
                   TargetPitch.Equals(Pitch) &&
                   TargetRotationSpeed.Equals(RotationSpeed);
        }
    }

    public new Vector3 CPosition => Position + GetBattleMapOffset();

    public BattleCamera() : base("BattleV2")
    {
        Position = new Vector3(10, 10, 14);
        RotationSpeed = DEFAULT_ROTATION_SPEED;
        FOV = DEFAULT_FOV;
        Speed = DEFAULT_SPEED;
        TargetSpeed = DEFAULT_SPEED;
        Yaw = DEFAULT_YAW;
        TargetYaw = DEFAULT_YAW;
        Pitch = DEFAULT_PITCH;
        TargetPitch = DEFAULT_PITCH;

        View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(FOV),
            Core.GraphicsDevice.Viewport.AspectRatio,
            0.01f,
            FarPlane);

        UpdateMatrices();
        UpdateFrustum();
        Ray = CreateRay();
    }

    public override void Update()
    {
        Ray = CreateRay();
        UpdateCamera();
        UpdateMatrices();
        UpdateFrustum();
    }

    public void UpdateCamera()
    {
        if (TargetMode == false)
        {
            return;
        }

        if (Position.Equals(TargetPosition) == false)
        {
            float moveX = 0.0f;
            float moveY = 0.0f;
            float moveZ = 0.0f;

            if (Position.X < TargetPosition.X)
            {
                moveX = Speed;
                if (Position.X + moveX > TargetPosition.X)
                {
                    moveX = TargetPosition.X - Position.X;
                }
            }
            if (Position.X > TargetPosition.X)
            {
                moveX = -Speed;
                if (Position.X + moveX < TargetPosition.X)
                {
                    moveX = TargetPosition.X - Position.X;
                }
            }

            if (Position.Y < TargetPosition.Y)
            {
                moveY = Speed;
                if (Position.Y + moveY > TargetPosition.Y)
                {
                    moveY = TargetPosition.Y - Position.Y;
                }
            }
            if (Position.Y > TargetPosition.Y)
            {
                moveY = -Speed;
                if (Position.Y + moveY < TargetPosition.Y)
                {
                    moveY = TargetPosition.Y - Position.Y;
                }
            }

            if (Position.Z < TargetPosition.Z)
            {
                moveZ = Speed;
                if (Position.Z + moveZ > TargetPosition.Z)
                {
                    moveZ = TargetPosition.Z - Position.Z;
                }
            }
            if (Position.Z > TargetPosition.Z)
            {
                moveZ = -Speed;
                if (Position.Z + moveZ < TargetPosition.Z)
                {
                    moveZ = TargetPosition.Z - Position.Z;
                }
            }

            Position = new Vector3(Position.X + moveX, Position.Y + moveY, Position.Z + moveZ);
        }

        if (TargetYaw.Equals(Yaw) == false)
        {
            if (Yaw < TargetYaw)
            {
                Yaw += RotationSpeed;
                if (Yaw > TargetYaw)
                {
                    Yaw = TargetYaw;
                }
            }
            if (Yaw > TargetYaw)
            {
                Yaw -= RotationSpeed;
                if (Yaw < TargetYaw)
                {
                    Yaw = TargetYaw;
                }
            }
        }

        if (TargetPitch.Equals(Pitch) == false)
        {
            if (Pitch < TargetPitch)
            {
                Pitch += RotationSpeed;
                if (Pitch > TargetPitch)
                {
                    Pitch = TargetPitch;
                }
            }
            if (Pitch > TargetPitch)
            {
                Pitch -= RotationSpeed;
                if (Pitch < TargetPitch)
                {
                    Pitch = TargetPitch;
                }
            }
        }

        if (TargetSpeed.Equals(Speed) == false)
        {
            if (Speed < TargetSpeed)
            {
                Speed += SPEED_STEP;
                if (Speed > TargetSpeed)
                {
                    Speed = TargetSpeed;
                }
            }
            if (Speed > TargetSpeed)
            {
                Speed -= SPEED_STEP;
                if (Speed < TargetSpeed)
                {
                    Speed = TargetSpeed;
                }
            }
        }

        if (TargetRotationSpeed.Equals(RotationSpeed) == false)
        {
            if (RotationSpeed < TargetRotationSpeed)
            {
                RotationSpeed += SPEED_STEP;
                if (RotationSpeed > TargetRotationSpeed)
                {
                    RotationSpeed = TargetRotationSpeed;
                }
            }
            if (RotationSpeed > TargetRotationSpeed)
            {
                RotationSpeed -= SPEED_STEP;
                if (RotationSpeed < TargetRotationSpeed)
                {
                    RotationSpeed = TargetRotationSpeed;
                }
            }
        }
    }

    public void UpdateFrustum()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 fPosition = new Vector3(Position.X, Position.Y, Position.Z) + GetBattleMapOffset();
        Vector3 transformed = Vector3.Transform(new Vector3(0, 0, -1), rotation);
        Vector3 lookAt = fPosition + transformed;

        BoundingFrustum = new BoundingFrustum(
            Matrix.CreateLookAt(fPosition, lookAt, Vector3.Up) * Projection);
    }

    public void UpdateMatrices()
    {
        Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
        Vector3 transformed = Vector3.Transform(new Vector3(0, 0, -1), rotation);
        Vector3 lookAt = new Vector3(Position.X, Position.Y, Position.Z) +
                         GetBattleMapOffset() + transformed;

        View = Matrix.CreateLookAt(Position + GetBattleMapOffset(), lookAt, Vector3.Up);
    }

    public Ray CreateRay()
    {
        int centerX = (int)(Core.windowSize.Width / 2);
        int centerY = (int)(Core.windowSize.Height / 2);

        Vector3 nearSource = new Vector3(centerX, centerY, 0);
        Vector3 farSource = new Vector3(centerX, centerY, 1);

        Vector3 nearPoint = Core.GraphicsDevice.Viewport.Unproject(
            nearSource, Projection, View, Matrix.Identity);
        Vector3 farPoint = Core.GraphicsDevice.Viewport.Unproject(
            farSource, Projection, View, Matrix.Identity);

        Vector3 direction = farPoint - nearPoint;
        direction.Normalize();

        return new Ray(nearPoint, direction);
    }

    private static Vector3 GetBattleMapOffset()
    {
        return BattleScreen.BattleMapOffset;
    }
}
