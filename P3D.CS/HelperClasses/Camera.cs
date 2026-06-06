using Microsoft.Xna.Framework;

namespace P3D;

public class Camera
{
    private const float NEAR_PLANE_DISTANCE = 0.01f;
    private const float DIRECTION_NORTH_UPPER = 0.25f;
    private const float DIRECTION_NORTH_LOWER = 1.75f;
    private const float DIRECTION_WEST_UPPER = 0.75f;
    private const float DIRECTION_SOUTH_UPPER = 1.25f;
    private const float DIRECTION_EAST_UPPER = 1.75f;

    public BoundingFrustum BoundingFrustum { get; set; } = new BoundingFrustum(Matrix.Identity);
    public Matrix View { get; set; }
    public Matrix Projection { get; set; }
    public Vector3 Position { get; set; }

    public float Yaw { get; set; }
    public float Pitch { get; set; }

    protected Vector3 _plannedMovement;
    protected bool _setPlannedMovement;

    public Vector3 PlannedMovement
    {
        get => _plannedMovement;
        set
        {
            _plannedMovement = value;
            _setPlannedMovement = value != Vector3.Zero;
        }
    }

    public void AddToPlannedMovement(Vector3 v)
    {
        _plannedMovement += v;
    }

    public Ray Ray { get; set; } = new Ray();

    public bool Turning { get; set; }

    public float Speed { get; set; } = 0.04f;
    public float RotationSpeed { get; set; } = 0.003f;

    public float FarPlane { get; set; } = 30f;
    public float FOV { get; set; } = 45f;

    public String Name { get; set; }

    public Camera(String name)
    {
        Name = name;
    }

    public virtual void Update()
    {
        throw new NotImplementedException();
    }

    public virtual void Turn(int turns, bool forceCameraTurn = false, bool doPlayerTurn = true)
    {
        throw new NotImplementedException();
    }

    public virtual void InstantTurn(int turns)
    {
        throw new NotImplementedException();
    }

    public int GetFacingDirection()
    {
        if (Yaw <= MathHelper.Pi * DIRECTION_NORTH_UPPER ||
            Yaw > MathHelper.Pi * DIRECTION_NORTH_LOWER)
        {
            return 0;
        }
        if (Yaw <= MathHelper.Pi * DIRECTION_WEST_UPPER &&
            Yaw > MathHelper.Pi * DIRECTION_NORTH_UPPER)
        {
            return 1;
        }
        if (Yaw <= MathHelper.Pi * DIRECTION_SOUTH_UPPER &&
            Yaw > MathHelper.Pi * DIRECTION_WEST_UPPER)
        {
            return 2;
        }
        if (Yaw <= MathHelper.Pi * DIRECTION_EAST_UPPER &&
            Yaw > MathHelper.Pi * DIRECTION_SOUTH_UPPER)
        {
            return 3;
        }
        return 0;
    }

    public virtual int GetPlayerFacingDirection()
    {
        return GetFacingDirection();
    }

    public virtual Vector3 GetForwardMovedPosition()
    {
        throw new NotImplementedException();
    }

    public virtual Vector3 GetMoveDirection()
    {
        throw new NotImplementedException();
    }

    public virtual void Move(float steps)
    {
        throw new NotImplementedException();
    }

    public virtual void StopMovement()
    {
        throw new NotImplementedException();
    }

    public virtual bool IsMoving => false;

    public void CreateNewProjection(float newFOV)
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(newFOV),
            Core.GraphicsDevice.Viewport.AspectRatio,
            NEAR_PLANE_DISTANCE,
            FarPlane);
        FOV = newFOV;
    }

    public virtual Vector3 CPosition => Position;
}
