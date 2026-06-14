using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class FloorModel : BaseModel
{
    public FloorModel()
    {
        ID = 0;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
        ]);
    }
}

public class BillModel : BaseModel
{
    public BillModel()
    {
        ID = 3;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)),
        ]);
    }
}

public class WallModel : BaseModel
{
    public WallModel()
    {
        ID = 20;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Backward, new Vector2(0, 1)),
        ]);
    }
}

public class CeilingModel : BaseModel
{
    public CeilingModel()
    {
        ID = 21;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
        ]);
    }
}

public class CrossModel : BaseModel
{
    public CrossModel()
    {
        ID = 13;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, -0.5f), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0, -0.5f, -0.5f), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, -0.5f), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0.5f), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0.5f), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0.5f), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)),
        ]);
    }
}

public class DoubleFloorModel : BaseModel
{
    public DoubleFloorModel()
    {
        ID = 14;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
        ]);
    }
}

public class BlockModel : BaseModel
{
    public BlockModel()
    {
        ID = 1;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
        ]);
    }
}

public class SlideModel : BaseModel
{
    public SlideModel()
    {
        ID = 2;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
        ]);
    }
}

public class CornerModel : BaseModel
{
    public CornerModel()
    {
        ID = 5;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
        ]);
    }
}

public class InsideCornerModel : BaseModel
{
    public InsideCornerModel()
    {
        ID = 6;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
        ]);
    }
}

public class CubeModel : BaseModel
{
    public CubeModel()
    {
        ID = 12;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Down, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Down, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Down, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Down, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Down, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Down, new Vector2(0, 1)),
        ]);
    }
}

public class DiagonalWallModel : BaseModel
{
    public DiagonalWallModel()
    {
        ID = 17;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Backward, new Vector2(0, 1)),
        ]);
    }
}

public class HalfDiagonalWallModel : BaseModel
{
    public HalfDiagonalWallModel()
    {
        ID = 18;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Backward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Backward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)),
        ]);
    }
}

public class PyramidModel : BaseModel
{
    public PyramidModel()
    {
        ID = 15;
        Setup([
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(0.5f, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0)),
            new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), new Vector3(0, 0, -1), new Vector2(0.5f, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
        ]);
    }
}

public class StairsModel : BaseModel
{
    public StairsModel()
    {
        ID = 16;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0.5f)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)),
        ]);
    }
}

public class CliffModel : BaseModel
{
    public CliffModel()
    {
        ID = 9;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(1, 0)),
        ]);
    }
}

public class CliffInsideModel : BaseModel
{
    public CliffInsideModel()
    {
        ID = 10;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
        ]);
    }
}

public class CliffCornerModel : BaseModel
{
    public CliffCornerModel()
    {
        ID = 11;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.25f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, 0.25f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.25f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, 0.25f), new Vector3(0, 0, -1), new Vector2(0, 1)),
        ]);
    }
}

public class SignModel : BaseModel
{
    public SignModel()
    {
        ID = 4;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.1f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.1f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.1f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.1f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.1f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.1f), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.1f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.1f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.1f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.1f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.1f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.1f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.1f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.1f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.1f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.1f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.1f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.1f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.1f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.1f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.1f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.1f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.1f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.1f), new Vector3(0, 0, -1), new Vector2(1, 0)),
        ]);
    }
}

public class StepModel : BaseModel
{
    public StepModel()
    {
        ID = 7;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
        ]);
    }
}

public class InsideStepModel : BaseModel
{
    public InsideStepModel()
    {
        ID = 8;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
        ]);
    }
}

public class OutsideStepModel : BaseModel
{
    public OutsideStepModel()
    {
        ID = 19;
        Setup([
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
            new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
        ]);
    }
}
