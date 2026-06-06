using Microsoft.Xna.Framework.Graphics;

namespace P3D;

// TODO Phase 7: full TextureManager port
public static class TextureManager
{
    public static Dictionary<String, Texture2D> TextureList { get; } = [];
    public static Texture2D? DefaultTexture { get; private set; }

    public static void InitializeTextures()
    {
        DefaultTexture = new Texture2D(Core.GraphicsDevice, 1, 1);
        DefaultTexture.SetData([ Microsoft.Xna.Framework.Color.White ]);
    }

    public static Texture2D GetTexture(String name)
    {
        TextureList.TryGetValue(name.ToLower(), out Texture2D? t);
        return t ?? DefaultTexture!;
    }

    public static Texture2D GetTexture(String name, Microsoft.Xna.Framework.Rectangle region, String suffix)
    {
        return GetTexture(name);
    }
}
