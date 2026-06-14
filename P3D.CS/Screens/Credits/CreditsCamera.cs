using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class CreditsCamera : Camera
{
    public Vector3 Target;

    public bool IsReady
    {
        get
        {
            if (new Vector3((int)Position.X, (int)Position.Y, (int)Position.Z) ==
                new Vector3((int)Target.X, (int)Target.Y, (int)Target.Z))
                return true;
            return false;
        }
    }

    public CreditsCamera() : base("Credits")
    {
        Position = new Vector3(0, 2, 0);
        RotationSpeed = 0.04f;
        FOV = 60.0f;
        Speed = 1.0f;

        Yaw = (float)(Math.PI / 4);
        Pitch = -0.2f;

        View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), Core.GraphicsDevice.Viewport.AspectRatio, 0.01f, FarPlane);

        UpdateMatrices();
        UpdateFrustum();
        CreateRay();
    }

    public override void Update()
    {
        Ray = CreateRay();
        MoveCamera();
        UpdateMatrices();
        UpdateFrustum();
    }

    public void MoveCamera()
    {
        if (IsReady == true)
        {
            Position = Target;
        }
        else
        {
            float moveX = 0.0f;
            float moveY = 0.0f;
            float moveZ = 0.0f;

            if (Position.X < Target.X)
            {
                moveX = Speed;
                if (Position.X + moveX > Target.X)
                    moveX = Target.X - Position.X;
            }
            if (Position.X > Target.X)
            {
                moveX = -Speed;
                if (Position.X + moveX < Target.X)
                    moveX = Position.X - Target.X;
            }

            if (Position.Y < Target.Y)
            {
                moveY = Speed;
                if (Position.Y + moveY > Target.Y)
                    moveY = Target.Y - Position.Y;
            }
            if (Position.Y > Target.Y)
            {
                moveY = -Speed;
                if (Position.Y + moveY < Target.Y)
                    moveY = Position.Y - Target.Y;
            }

            if (Position.Z < Target.Z)
            {
                moveZ = Speed;
                if (Position.Z + moveZ > Target.Z)
                    moveZ = Target.Z - Position.Z;
            }
            if (Position.Z > Target.Z)
            {
                moveZ = -Speed;
                if (Position.Z + moveZ < Target.Z)
                    moveZ = Position.Z - Target.Z;
            }

            Position = new Vector3(Position.X + moveX, Position.Y + moveY, Position.Z + moveZ);
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
