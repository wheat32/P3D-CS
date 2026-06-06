using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public static class Lighting
{
    private static Texture2D _lightingColorTexture = TextureManager.GetTexture(@"SkyDomeResource\LightingColors");
    private static Texture2D _fogColorTexture = TextureManager.GetTexture(@"SkyDomeResource\FogColors");

    // TextureType: 0 = Directional light, 1 = Ambient light, 2 = Fog color
    public static Vector3 GetEnvironmentColor(int textureType)
    {
        Texture2D colorTexture;
        Vector3 environmentColor = Vector3.Zero;

        int x = 0;
        int y = 0;

        switch (textureType)
        {
            case 0:
                x = Screen.Level.DayTime - 1;
                if (x >= 0 && x <= 3)
                {
                    colorTexture = _lightingColorTexture;
                    Color[] colorData = new Color[1];
                    colorTexture.GetData(0, new Rectangle(x, 0, 1, 1), colorData, 0, 1);

                    Color[] darkOrBrightData = new Color[1];
                    colorTexture.GetData(0, new Rectangle(x, 1, 1, 1), darkOrBrightData, 0, 1);
                    environmentColor = colorData[0].ToVector3();
                    if (darkOrBrightData[0] == Color.Black)
                    {
                        environmentColor = Vector3.Zero - environmentColor;
                    }
                }
                break;
            case 1:
                x = Screen.Level.DayTime - 1;
                if (x >= 0 && x <= 3)
                {
                    colorTexture = _lightingColorTexture;
                    Color[] colorData = new Color[1];
                    colorTexture.GetData(0, new Rectangle(x, 2, 1, 1), colorData, 0, 1);

                    Color[] darkOrBrightData = new Color[1];
                    colorTexture.GetData(0, new Rectangle(x, 3, 1, 1), darkOrBrightData, 0, 1);
                    environmentColor = colorData[0].ToVector3();
                    if (darkOrBrightData[0] == Color.Black)
                    {
                        environmentColor = Vector3.Zero - environmentColor;
                    }
                }
                break;
            case 2:
                colorTexture = _fogColorTexture;
                switch (Screen.Level.EnvironmentType)
                {
                    case 0:
                        x = Screen.Level.DayTime - 1;
                        if (x > 2)
                        {
                            x = 0;
                            y += 1;
                        }
                        break;
                    case 1:
                        x = 1;
                        y = 1;
                        break;
                    case 2:
                        x = 2;
                        y = 1;
                        break;
                    case 3:
                        x = 0;
                        y = 2;
                        break;
                    case 4:
                        x = 1;
                        y = 2;
                        break;
                    case 5:
                        x = 2;
                        y = 2;
                        break;
                }
                Color[] fogColorData = new Color[1];
                colorTexture.GetData(0, new Rectangle(x, y, 1, 1), fogColorData, 0, 1);
                environmentColor = fogColorData[0].ToVector3();
                break;
        }
        return environmentColor;
    }

    public static void UpdateLighting(ref BasicEffect refEffect, bool forceLighting = false)
    {
        if (Core.GameOptions.LightingEnabled == true || forceLighting == true)
        {
            refEffect.LightingEnabled = true;
            refEffect.PreferPerPixelLighting = true;
            refEffect.SpecularPower = 2000.0f;

            switch (GetLightingType())
            {
                case 0:
                    refEffect.AmbientLightColor = GetEnvironmentColor(1);
                    refEffect.DirectionalLight0.DiffuseColor = GetEnvironmentColor(0);
                    refEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f));
                    refEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f);
                    refEffect.DirectionalLight0.Enabled = true;
                    break;
                case 1:
                    refEffect.AmbientLightColor = GetEnvironmentColor(1);
                    refEffect.DirectionalLight0.DiffuseColor = GetEnvironmentColor(0);
                    refEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1.0f, 0.0f, 1.0f));
                    refEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f);
                    refEffect.DirectionalLight0.Enabled = true;
                    break;
                case 2:
                    refEffect.AmbientLightColor = GetEnvironmentColor(1);
                    refEffect.DirectionalLight0.DiffuseColor = GetEnvironmentColor(0);
                    refEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1.0f, 0.0f, 1.0f));
                    refEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f);
                    refEffect.DirectionalLight0.Enabled = true;
                    break;
                case 3:
                    refEffect.AmbientLightColor = GetEnvironmentColor(1);
                    refEffect.DirectionalLight0.DiffuseColor = GetEnvironmentColor(0);
                    refEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f));
                    refEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f);
                    refEffect.DirectionalLight0.Enabled = true;
                    break;
                default:
                    refEffect.LightingEnabled = false;
                    break;
            }
        }
        else
        {
            refEffect.LightingEnabled = false;
        }
    }

    public static void UpdateLighting(ref BasicEffectWithAlphaTest refEffect, bool forceLighting = false)
    {
        if (Core.GameOptions.LightingEnabled == true || forceLighting == true)
        {
            refEffect.LightingEnabled = true;
            refEffect.PreferPerPixelLighting = true;
            refEffect.SpecularPower = 2000.0f;

            switch (GetLightingType())
            {
                case 0:
                    refEffect.AmbientLightColor = GetEnvironmentColor(1);
                    refEffect.DirectionalLight0.DiffuseColor = GetEnvironmentColor(0);
                    refEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f));
                    refEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f);
                    refEffect.DirectionalLight0.Enabled = true;
                    break;
                case 1:
                    refEffect.AmbientLightColor = GetEnvironmentColor(1);
                    refEffect.DirectionalLight0.DiffuseColor = GetEnvironmentColor(0);
                    refEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1.0f, 0.0f, 1.0f));
                    refEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f);
                    refEffect.DirectionalLight0.Enabled = true;
                    break;
                case 2:
                    refEffect.AmbientLightColor = GetEnvironmentColor(1);
                    refEffect.DirectionalLight0.DiffuseColor = GetEnvironmentColor(0);
                    refEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1.0f, 0.0f, 1.0f));
                    refEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f);
                    refEffect.DirectionalLight0.Enabled = true;
                    break;
                case 3:
                    refEffect.AmbientLightColor = GetEnvironmentColor(1);
                    refEffect.DirectionalLight0.DiffuseColor = GetEnvironmentColor(0);
                    refEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f));
                    refEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f);
                    refEffect.DirectionalLight0.Enabled = true;
                    break;
                default:
                    refEffect.LightingEnabled = false;
                    break;
            }
        }
        else
        {
            refEffect.LightingEnabled = false;
        }
    }

    public static int GetLightingType()
    {
        int lightType = (int)World.GetTime();

        if (Screen.Level.EnvironmentType == (int)World.EnvironmentTypes.Outside)
        {
            switch (Screen.Level.DayTime)
            {
                case 1:
                    lightType = 0;
                    break;
                case 2:
                    lightType = 1;
                    break;
                case 3:
                    lightType = 2;
                    break;
                case 4:
                    lightType = 3;
                    break;
            }
        }
        if (Screen.Level.LightingType == 1)
        {
            lightType = 99;
        }
        if (Screen.Level.LightingType > 1 && Screen.Level.LightingType < 6)
        {
            lightType = Screen.Level.LightingType - 2;
        }
        if ((Screen.Level.LightingType == 6 && Screen.Level.EnvironmentType == 1) ||
            Screen.Level.LightingType > 6)
        {
            lightType = 99;
        }

        return lightType;
    }
}
