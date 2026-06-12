using Microsoft.Xna.Framework.Graphics;

namespace P3D;

// TODO Phase 7: full BaseModel port
public class BaseModel
{
    public static readonly BaseModel BillModel = new BaseModel();
    public static readonly BaseModel FloorModel = new BaseModel();
    public static readonly BaseModel BlockModel = new BaseModel();

    public int ID;

    public static BaseModel GetModelByID(int id)
    {
        if (id == 0)
        {
            return FloorModel;
        }
        return BillModel;
    }
    public VertexBuffer? VertexBuffer { get; protected set; }

    protected void Setup(VertexPositionNormalTexture[] vertexData)
    {
        VertexBuffer = new VertexBuffer(Core.GraphicsDevice,
            typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.WriteOnly);
        VertexBuffer.SetData(vertexData);
    }

    public virtual void Draw(Entity entity, Texture2D[] textures)
    {
        // TODO Phase 7: implement full draw with BasicEffect
    }

    protected void ApplyTexture(Texture2D texture)
    {
        if (Screen.Effect != null)
        {
            Screen.Effect.Texture = texture;
        }
        foreach (EffectPass pass in Screen.Effect!.CurrentTechnique.Passes)
        {
            pass.Apply();
        }
    }
}
