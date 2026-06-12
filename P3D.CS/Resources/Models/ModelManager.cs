using Microsoft.Xna.Framework.Graphics;

namespace P3D;

// TODO Phase 7: full ModelManager port
public static class ModelManager
{
    public const float MODELSCALE = 1.0f;

    public static bool ModelExist(String path) => false;
    public static Model? GetModel(String path) => null;
    public static float PokeModelScale(String path) => 1.0f;
    public static Microsoft.Xna.Framework.Vector3 PokeModelRotation(String path) => Microsoft.Xna.Framework.Vector3.Zero;
}
