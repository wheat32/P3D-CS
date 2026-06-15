using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class BaseModel
{
    public static readonly FloorModel FloorModel = new FloorModel();
    public static readonly BlockModel BlockModel = new BlockModel();
    public static readonly SlideModel SlideModel = new SlideModel();
    public static readonly BillModel BillModel = new BillModel();
    public static readonly SignModel SignModel = new SignModel();
    public static readonly CornerModel CornerModel = new CornerModel();
    public static readonly InsideCornerModel InsideCornerModel = new InsideCornerModel();
    public static readonly StepModel StepModel = new StepModel();
    public static readonly InsideStepModel InsideStepModel = new InsideStepModel();
    public static readonly CliffModel CliffModel = new CliffModel();
    public static readonly CliffInsideModel CliffInsideModel = new CliffInsideModel();
    public static readonly CliffCornerModel CliffCornerModel = new CliffCornerModel();
    public static readonly CubeModel CubeModel = new CubeModel();
    public static readonly CrossModel CrossModel = new CrossModel();
    public static readonly DoubleFloorModel DoubleFloorModel = new DoubleFloorModel();
    public static readonly PyramidModel PyramidModel = new PyramidModel();
    public static readonly StairsModel StairsModel = new StairsModel();
    public static readonly DiagonalWallModel DiagonalWallModel = new DiagonalWallModel();
    public static readonly HalfDiagonalWallModel HalfDiagonalWallModel = new HalfDiagonalWallModel();
    public static readonly OutsideStepModel OutsideStepModel = new OutsideStepModel();
    public static readonly WallModel WallModel = new WallModel();
    public static readonly CeilingModel CeilingModel = new CeilingModel();

    public int ID = 0;
    public VertexBuffer? VertexBuffer;

    protected void Setup(VertexPositionNormalTexture[] vertexData)
    {
        VertexBuffer = new VertexBuffer(Core.GraphicsDevice,
            typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.WriteOnly);
        VertexBuffer.SetData(vertexData);
    }

    public void Draw(Entity entity, Texture2D[] textures)
    {
        Vector3 effectDiffuseColor = Screen.Effect!.DiffuseColor;

        Screen.Effect.World = entity.World;
        Screen.Effect.TextureEnabled = true;
        Screen.Effect.Alpha = entity.Opacity;
        Screen.Effect.DiffuseColor = effectDiffuseColor * entity.Shader * entity.Color;

        if (Screen.Level!.IsDark == true || Screen.Level.LightingType == 6)
            Screen.Effect.DiffuseColor *= new Vector3(0.5f, 0.5f, 0.5f);

        Core.GraphicsDevice.SetVertexBuffer(VertexBuffer);

        int faceCount = VertexBuffer!.VertexCount / 3;
        if (faceCount > entity.TextureIndex.Length)
        {
            int[] newTextureIndex = new int[faceCount + 1];
            for (int i = 0; i <= faceCount; i++)
            {
                if (entity.TextureIndex.Length - 1 >= i)
                    newTextureIndex[i] = entity.TextureIndex[i];
                else
                    newTextureIndex[i] = 0;
            }
            entity.TextureIndex = newTextureIndex;
        }

        bool isEqual = true;
        if (entity.HasEqualTextures == -1)
        {
            entity.HasEqualTextures = 1;
            int contains = entity.TextureIndex[0];
            for (int index = 1; index < entity.TextureIndex.Length; index++)
            {
                if (contains != entity.TextureIndex[index])
                {
                    entity.HasEqualTextures = 0;
                    break;
                }
            }
        }
        if (entity.HasEqualTextures == 0)
            isEqual = false;

        if (isEqual == true)
        {
            if (entity.TextureIndex[0] > -1 && textures[entity.TextureIndex[0]] != null)
            {
                ApplyTexture(textures[entity.TextureIndex[0]]);
                Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, faceCount);
                DebugDisplay.DrawnVertices += faceCount;
            }
        }
        else
        {
            for (int i = 0; i < VertexBuffer.VertexCount - 2; i += 3)
            {
                int face = i / 3;
                if (entity.TextureIndex[face] > -1 && textures[entity.TextureIndex[face]] != null)
                {
                    ApplyTexture(textures[entity.TextureIndex[face]]);
                    Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, i, 1);
                    DebugDisplay.DrawnVertices += 1;
                }
            }
        }

        Screen.Effect.DiffuseColor = effectDiffuseColor;
        if (DebugDisplay.MaxDistance < entity.CameraDistance)
            DebugDisplay.MaxDistance = (int)entity.CameraDistance;
    }

    private void ApplyTexture(Texture2D texture)
    {
        Screen.Effect!.Texture = texture;
        Screen.Effect.CurrentTechnique.Passes[0].Apply();
    }

    public static BaseModel GetModelByID(int id) => id switch
    {
        0 => FloorModel,
        1 => BlockModel,
        2 => SlideModel,
        3 => BillModel,
        4 => SignModel,
        5 => CornerModel,
        6 => InsideCornerModel,
        7 => StepModel,
        8 => InsideStepModel,
        9 => CliffModel,
        10 => CliffInsideModel,
        11 => CliffCornerModel,
        12 => CubeModel,
        13 => CrossModel,
        14 => DoubleFloorModel,
        15 => PyramidModel,
        16 => StairsModel,
        17 => DiagonalWallModel,
        18 => HalfDiagonalWallModel,
        19 => OutsideStepModel,
        20 => WallModel,
        21 => CeilingModel,
        _ => BlockModel,
    };
}
