using Microsoft.Xna.Framework;

namespace P3D.Screens.UI;

public static class ColorProvider
{
    public static bool IsGameJolt => Core.Player?.IsGameJoltSave ?? false;

    public static Color GradientColor(bool isGameJolt, int alpha)
    {
        return isGameJolt == true
            ? new Color(0, 0, 0, alpha)
            : new Color(0, 0, 0, alpha);
    }

    public static Color MainColor(bool isGameJolt, int alpha = 255)
    {
        return isGameJolt == true
            ? new Color(30, 30, 60, alpha)
            : new Color(30, 30, 30, alpha);
    }

    public static Color AccentColor(bool isGameJolt, int alpha = 255)
    {
        return isGameJolt == true
            ? new Color(204, 255, 0, alpha)
            : new Color(0, 140, 220, alpha);
    }
}
