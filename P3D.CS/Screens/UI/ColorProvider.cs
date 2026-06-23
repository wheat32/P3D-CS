using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Screens.UI;

public static class ColorProvider
{
    public static bool IsGameJolt => Core.Player?.IsGameJoltSave ?? false;

    // Matches VB's GetInterfaceColor: reads a single pixel from the InterfaceColors
    // texture at (colorType, row) where row=0 is normal mode, row=1 is GameJolt mode.
    // colorType: 0=Gradient, 1=Main, 2=Light, 3=Accent
    public static Color GetInterfaceColor(int colorType, bool isGameJolt)
    {
        int row = isGameJolt == true ? 1 : 0;
        Texture2D? texture = TextureManager.GetTexture(@"GUI\Menus\InterfaceColors");
        if (texture != null)
        {
            Color[] data = new Color[1];
            texture.GetData(0, new Rectangle(colorType, row, 1, 1), data, 0, 1);
            return data[0];
        }
        // Fallback values matching the InterfaceColors.png pixel data
        return colorType switch
        {
            0 => isGameJolt == true ? new Color(45, 45, 45)  : new Color(99, 204, 255),
            1 => isGameJolt == true ? new Color(39, 39, 39)  : new Color(84, 198, 216),
            2 => isGameJolt == true ? new Color(70, 70, 70)  : new Color(111, 249, 255),
            _ => isGameJolt == true ? new Color(204, 255, 0) : new Color(3, 155, 229),
        };
    }

    public static Color GradientColor(bool isGameJolt, int alpha)
    {
        Color c = GetInterfaceColor(0, isGameJolt);
        return new Color(c.R, c.G, c.B, alpha);
    }

    public static Color MainColor(bool isGameJolt, int alpha = 255)
    {
        Color c = GetInterfaceColor(1, isGameJolt);
        return new Color(c.R, c.G, c.B, alpha);
    }

    public static Color AccentColor(bool isGameJolt, int alpha = 255)
    {
        Color c = GetInterfaceColor(3, isGameJolt);
        return new Color(c.R, c.G, c.B, alpha);
    }
}
