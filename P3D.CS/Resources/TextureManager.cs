using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public static class TextureManager
{
    public static Texture2D? DefaultTexture { get; private set; }

    public static Dictionary<String, Texture2D> TextureList { get; } = [];
    public static Dictionary<KeyValuePair<int, Rectangle>, Texture2D> TextureRectList { get; } = [];

    private static String ToOsPath(String root, String key, String ext = "") =>
        Path.Combine(root, key.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)) + ext;

    public static void InitializeTextures()
    {
        DefaultTexture = LoadDirect("GUI\\no_texture.png");
    }

    public static Texture2D LoadDirect(String textureFile)
    {
        using Stream stream = File.Open(ToOsPath(Path.Combine(GameController.GamePath, "Content"), textureFile), FileMode.Open);
        return Texture2D.FromStream(Core.GraphicsDevice, stream);
    }

    public static Texture2D GetTexture(String name)
    {
        ContentManager cContent = ContentPackManager.GetContentManager(name, ".xnb,.png");
        String tKey = cContent.RootDirectory + "\\" + name + ",FULL_IMAGE";

        if (TextureList.ContainsKey(tKey) == false)
        {
            Texture2D? t = null;
            String contentRoot = Path.Combine(GameController.GamePath, cContent.RootDirectory);
            String xnbPath = ToOsPath(contentRoot, name, ".xnb");
            String pngPath = ToOsPath(contentRoot, name, ".png");

            if (File.Exists(xnbPath) == false)
            {
                if (File.Exists(pngPath) == true)
                {
                    try
                    {
                        using Stream stream = File.Open(pngPath, FileMode.OpenOrCreate);
                        t = Texture2D.FromStream(Core.GraphicsDevice, stream);
                    }
                    catch (Exception)
                    {
                        Logger.Log(Logger.LogTypes.ErrorMessage, "Something went wrong while XNA tried to load a texture. Return default.");
                        return DefaultTexture!;
                    }
                }
                else
                {
                    Logger.Log(Logger.LogTypes.ErrorMessage, "TextureManager.vb: Texture \"" + ToOsPath(contentRoot, name) + "\" was not found!");
                    return DefaultTexture!;
                }
            }
            else
            {
                t = cContent.Load<Texture2D>(name);
            }

            TextureList.Add(tKey, ApplyEffect(TextureRectangle(t!, new Rectangle(0, 0, t!.Width, t.Height), 1)));
            cContent.Unload();
        }

        return TextureList[tKey];
    }

    public static Texture2D GetTexture(String name, Rectangle r, String texturePath)
    {
        TextureSource tSource = ContentPackManager.GetTextureReplacement(texturePath + name, r);
        ContentManager cContent = ContentPackManager.GetContentManager(tSource.TexturePath, ".xnb,.png");
        float resolution = ContentPackManager.GetTextureResolution(texturePath + name);

        String tKey = cContent.RootDirectory + "\\" + tSource.TexturePath + "," +
            tSource.TextureRectangle.X + "," + tSource.TextureRectangle.Y + "," +
            tSource.TextureRectangle.Width + "," + tSource.TextureRectangle.Height + "," + resolution;

        if (TextureList.ContainsKey(tKey) == false)
        {
            Texture2D? t = null;
            bool doApplyEffect = true;
            String baseKey = cContent.RootDirectory + "\\" + tSource.TexturePath;

            if (TextureList.ContainsKey(baseKey) == true)
            {
                t = TextureList[baseKey];
                doApplyEffect = false;
            }
            else
            {
                String contentRoot2 = Path.Combine(GameController.GamePath, cContent.RootDirectory);
                String xnbPath = ToOsPath(contentRoot2, tSource.TexturePath, ".xnb");
                String pngPath = ToOsPath(contentRoot2, tSource.TexturePath, ".png");

                if (File.Exists(xnbPath) == false)
                {
                    if (File.Exists(pngPath) == true)
                    {
                        try
                        {
                            using Stream stream = File.Open(pngPath, FileMode.OpenOrCreate);
                            t = Texture2D.FromStream(Core.GraphicsDevice, stream);
                        }
                        catch (Exception)
                        {
                            Logger.Log(Logger.LogTypes.ErrorMessage, "Something went wrong while XNA tried to load a texture. Return default.");
                            return DefaultTexture!;
                        }
                    }
                    else
                    {
                        Logger.Log(Logger.LogTypes.ErrorMessage, "TextureManager.vb: Texture \"" + ToOsPath(contentRoot2, texturePath + name) + "\" was not found!");
                        return DefaultTexture!;
                    }
                }
                else
                {
                    t = cContent.Load<Texture2D>(tSource.TexturePath);
                }

                if (TextureList.ContainsKey(baseKey) == false)
                    TextureList.Add(baseKey, ApplyEffect(t!.Copy()));
            }

            if (doApplyEffect == true)
            {
                if (TextureList.ContainsKey(tKey) == false)
                    TextureList.Add(tKey, ApplyEffect(TextureRectangle(t!, tSource.TextureRectangle, resolution)));
            }
            else
            {
                if (TextureList.ContainsKey(tKey) == false)
                    TextureList.Add(tKey, TextureRectangle(t!, tSource.TextureRectangle, resolution));
            }

            cContent.Unload();
        }

        return TextureList[tKey];
    }

    public static Texture2D GetTexture(String name, Rectangle r) => GetTexture(name, r, "Textures\\");

    public static Texture2D GetTexture(Texture2D source, Rectangle rectangle, float factor = 1)
    {
        KeyValuePair<int, Rectangle> key = new KeyValuePair<int, Rectangle>(source.GetHashCode(), rectangle);
        if (TextureRectList.TryGetValue(key, out Texture2D? cached) == true)
            return cached;

        Texture2D tex = TextureRectangle(source, rectangle, factor);
        TextureRectList.Add(key, tex);
        return tex;
    }

    public static Texture2D GetTexture(Texture2D source, Rectangle rectangle) => GetTexture(source, rectangle, 1);

    private static Texture2D ApplyEffect(Texture2D t)
    {
        if (GameController.Hacker == true)
        {
            Texture2D newT = new Texture2D(Core.GraphicsDevice, t.Width, t.Height);
            Color[] oldC = new Color[t.Width * t.Height];
            t.GetData(oldC);
            Color[] newC = new Color[oldC.Length];
            for (int i = 0; i < oldC.Length; i++)
                newC[i] = oldC[i].Invert();
            newT.SetData(newC);
            return newT;
        }
        return t;
    }

    private static Texture2D TextureRectangle(Texture2D texture, Rectangle rectangle, float factor = 1)
    {
        if (rectangle == Rectangle.Empty)
            return texture;

        rectangle = new Rectangle(
            (int)(rectangle.X * factor), (int)(rectangle.Y * factor),
            (int)(rectangle.Width * factor), (int)(rectangle.Height * factor));

        Rectangle tRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        if (tRectangle.Contains(rectangle) == false)
        {
            Logger.Log(Logger.LogTypes.ErrorMessage, "TextureManager.vb: The rectangle for a texture was out of bounds!");
            return DefaultTexture!;
        }

        Color[] data = new Color[rectangle.Width * rectangle.Height];
        texture.GetData(0, rectangle, data, 0, rectangle.Width * rectangle.Height);

        Texture2D newTex = new Texture2D(Core.GraphicsDevice, rectangle.Width, rectangle.Height);
        newTex.SetData(data);
        return newTex;
    }

    public static bool TextureExist(String name)
    {
        ContentManager cContent = ContentPackManager.GetContentManager(name, ".xnb,.png");
        String contentRoot = Path.Combine(GameController.GamePath, cContent.RootDirectory);
        if (File.Exists(ToOsPath(contentRoot, name, ".xnb")) == true)
            return true;
        if (File.Exists(ToOsPath(contentRoot, name, ".png")) == true)
            return true;
        return false;
    }
}
