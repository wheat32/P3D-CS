using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class SecretBase
{
    private const int BASE_X_MIN = -1;
    private const int BASE_X_MAX = 10;
    private const int BASE_Z_MIN = -1;
    private const int BASE_Z_MAX = 10;
    private const int TEXTURE_FRAME_SIZE = 16;

    public enum BaseTypes
    {
        Grass,
        Desert,
        Mountain,
        Dirt,
        Cave,
    }

    public BaseTypes BaseType { get; set; } = BaseTypes.Grass;

    public SecretBase()
    {
    }

    public void LoadSecretBaseFromStore(Level level)
    {
        List<Entity> entities = [];
        List<Entity> floors = [];

        for (int x = BASE_X_MIN; x <= BASE_X_MAX; x++)
        {
            for (int z = BASE_Z_MIN; z <= BASE_Z_MAX; z++)
            {
                if (x == BASE_X_MIN || x == BASE_X_MAX || z == BASE_Z_MIN || z == BASE_Z_MAX)
                {
                    Entity wallBlock = Entity.GetNewEntity(
                        "WallBlock",
                        new Vector3(x, 0, z),
                        new Texture2D[] { GetBaseTexture(1, BaseTypes.Grass), GetBaseTexture(2, BaseTypes.Grass) },
                        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                        true,
                        Vector3.Zero,
                        Vector3.One,
                        BaseModel.BlockModel,
                        0,
                        String.Empty,
                        true,
                        Vector3.One,
                        -1,
                        String.Empty,
                        String.Empty,
                        Vector3.Zero);
                    entities.Add(wallBlock);
                }

                Entity newFloor = new Floor(
                    x, 0, z,
                    new Texture2D[] { GetBaseTexture(0, BaseTypes.Grass) },
                    new int[] { 0, 0 },
                    false, 0, Vector3.One, BaseModel.FloorModel, 0,
                    String.Empty, true, Vector3.One, false, false, false);
                floors.Add(newFloor);
            }
        }

        entities = entities.OrderByDescending(e => e.CameraDistance).ToList();
        floors = floors.OrderByDescending(f => f.CameraDistance).ToList();

        level.Entities = entities;
        level.Floors = floors;
    }

    public void GenerateSecretBase()
    {
    }

    public void SaveBaseToStore(Level level)
    {
    }

    public static Texture2D GetBaseTexture(int baseObject, BaseTypes baseType)
    {
        int x = baseObject * TEXTURE_FRAME_SIZE;
        int y = 0;

        switch (baseType)
        {
            case BaseTypes.Desert:
                y = TEXTURE_FRAME_SIZE;
                break;
            case BaseTypes.Mountain:
                y = TEXTURE_FRAME_SIZE * 2;
                break;
            case BaseTypes.Dirt:
                y = TEXTURE_FRAME_SIZE * 3;
                break;
            case BaseTypes.Cave:
                y = TEXTURE_FRAME_SIZE * 4;
                break;
        }

        return TextureManager.GetTexture(@"Textures\SecretBase", new Rectangle(x, y, TEXTURE_FRAME_SIZE, TEXTURE_FRAME_SIZE), String.Empty);
    }
}
