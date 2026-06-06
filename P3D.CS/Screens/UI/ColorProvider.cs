using Microsoft.Xna.Framework;

namespace P3D.Screens.UI;

// TODO Phase 6: full ColorProvider port
public static class ColorProvider
{
    public static bool IsGameJolt => Core.Player?.IsGameJoltSave ?? false;

    public static Color GradientColor(bool isGameJolt, int alpha)
    {
        return isGameJolt == true
            ? new Color(0, 0, 0, alpha)
            : new Color(0, 0, 0, alpha);
    }
}
