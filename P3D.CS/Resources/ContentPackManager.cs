using Microsoft.Xna.Framework.Content;

namespace P3D;

// TODO Phase 7: full ContentPackManager port
public static class ContentPackManager
{
    public static void CreateContentPackFolder() { }
    public static void Load(String exceptionsPath) { }
    public static ContentManager GetContentManager(String path, String extensions)
    {
        return Core.Content;
    }
}
