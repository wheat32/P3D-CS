using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace P3D;

public static class ContentPackManager
{
    private static Dictionary<TextureSource, TextureSource> _textureReplacements = [];
    private static Dictionary<String, bool> _filesExist = [];
    private static Dictionary<String, float> _textureResolutions = [];
    public static Dictionary<TextureSource, TextureSource> ScriptTextureReplacements { get; } = [];

    public static Dictionary<String, float> PokeModelScale { get; } = [];
    public static Dictionary<String, Vector3> PokeModelRotation { get; } = [];

    public static void Load(String contentPackFile, bool isScriptContent = false)
    {
        if (Directory.Exists(Path.Combine(GameController.GamePath, "ContentPacks")) == false) return;
        if (File.Exists(contentPackFile) == false) return;

        String[] lines = File.ReadAllLines(contentPackFile);
        foreach (String line in lines)
        {
            switch (line.GetSplit(0, "|").ToLower())
            {
                case "waterspeed":
                    if (isScriptContent == false)
                        GameModeManager.ForceWaterSpeed = int.Parse(line.GetSplit(1, "|"));
                    break;
                case "pokemodelscale":
                    if (isScriptContent == false)
                    {
                        String folderName = Path.GetDirectoryName(
                            Path.GetRelativePath(GameController.GamePath, contentPackFile))
                            ?.ToLower().Replace(Path.DirectorySeparatorChar, '\\') ?? String.Empty;
                        if (PokeModelScale.ContainsKey(folderName) == false)
                            PokeModelScale.Add(folderName, float.Parse(line.GetSplit(1, "|").Replace(".", GameController.DecSeparator)));
                    }
                    break;
                case "pokemodelrotation":
                    if (isScriptContent == false)
                    {
                        String folderName = Path.GetDirectoryName(
                            Path.GetRelativePath(GameController.GamePath, contentPackFile))
                            ?.ToLower().Replace(Path.DirectorySeparatorChar, '\\') ?? String.Empty;
                        if (PokeModelRotation.ContainsKey(folderName) == false)
                        {
                            String rotPart = line.GetSplit(1, "|");
                            PokeModelRotation.Add(folderName, new Vector3(
                                float.Parse(rotPart.GetSplit(0).Replace(".", GameController.DecSeparator)),
                                0,
                                float.Parse(rotPart.GetSplit(1).Replace(".", GameController.DecSeparator))));
                        }
                    }
                    break;
                default:
                    switch (line.CountSplits("|"))
                    {
                        case 2:
                        {
                            String textureName = ScriptVersion2.ScriptCommander.Parse(line.GetSplit(0, "|")).ToString()!;
                            float resolution = float.Parse(line.GetSplit(1, "|").Replace(".", GameController.DecSeparator));
                            if (isScriptContent == false && _textureResolutions.ContainsKey(textureName) == false)
                                _textureResolutions.Add(textureName, resolution);
                            break;
                        }
                        case 4:
                        {
                            String oldName = ScriptVersion2.ScriptCommander.Parse(line.GetSplit(0, "|")).ToString()!;
                            String newName = ScriptVersion2.ScriptCommander.Parse(line.GetSplit(2, "|")).ToString()!;
                            String oRS = line.GetSplit(1, "|");
                            String nRS = line.GetSplit(3, "|");
                            TextureSource oldSrc = new TextureSource(oldName, new Rectangle(
                                int.Parse(oRS.GetSplit(0)), int.Parse(oRS.GetSplit(1)),
                                int.Parse(oRS.GetSplit(2)), int.Parse(oRS.GetSplit(3))));
                            TextureSource newSrc = new TextureSource(newName, new Rectangle(
                                int.Parse(nRS.GetSplit(0)), int.Parse(nRS.GetSplit(1)),
                                int.Parse(nRS.GetSplit(2)), int.Parse(nRS.GetSplit(3))));
                            if (isScriptContent == false)
                            {
                                if (_textureReplacements.ContainsKey(oldSrc) == false)
                                    _textureReplacements.Add(oldSrc, newSrc);
                            }
                            else
                            {
                                if (ScriptTextureReplacements.ContainsKey(oldSrc) == false)
                                    ScriptTextureReplacements.Add(oldSrc, newSrc);
                            }
                            break;
                        }
                    }
                    break;
            }
        }
    }

    public static TextureSource GetTextureReplacement(String texturePath, Rectangle r)
    {
        // Lowercase only for comparison — never for the returned path, since Linux
        // filesystems are case-sensitive and the actual files use mixed case.
        TextureSource src = new TextureSource(texturePath.ToLower(), r);

        if (ScriptTextureReplacements.Count > 0)
        {
            foreach (TextureSource key in ScriptTextureReplacements.Keys)
            {
                if (key.IsEqual(src) == true)
                {
                    TextureSource scriptReplacement = ScriptTextureReplacements[key];
                    foreach (TextureSource replKey in _textureReplacements.Keys)
                    {
                        if (replKey.IsEqual(scriptReplacement) == true)
                            return _textureReplacements[replKey];
                    }
                    return scriptReplacement;
                }
            }
        }

        foreach (TextureSource key in _textureReplacements.Keys)
        {
            if (key.IsEqual(src) == true)
                return _textureReplacements[key];
        }

        return new TextureSource(texturePath, r);
    }

    public static float GetTextureResolution(String textureName)
    {
        foreach (KeyValuePair<String, float> kv in _textureResolutions)
        {
            if (kv.Key.ToLower() == textureName.ToLower())
                return kv.Value;
        }
        return 1;
    }

    public static ContentManager GetContentManager(String file, String fileEndings)
    {
        if (Core.GameOptions.ContentPackNames.Count() > 0)
        {
            foreach (String c in Core.GameOptions.ContentPackNames)
            {
                String contentPath = "ContentPacks\\" + c;
                foreach (String ending in fileEndings.Split(','))
                {
                    String fullPath = Path.Combine(GameController.GamePath,
                        contentPath.Replace('\\', Path.DirectorySeparatorChar),
                        file.Replace('\\', Path.DirectorySeparatorChar) + ending);
                    if (_filesExist.ContainsKey(fullPath) == false)
                        _filesExist.Add(fullPath, File.Exists(fullPath));
                    if (_filesExist[fullPath] == true)
                        return new ContentManager(Core.GameInstance.Services, contentPath);
                }
            }
        }

        GameMode gameMode = GameModeManager.ActiveGameMode;
        if (gameMode.ContentPath != "\\Content\\" && gameMode.ContentPath != String.Empty)
        {
            String[] gmParts = gameMode.ContentPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            String gameModePath = Path.Combine([GameController.GamePath, ..gmParts]);
            String rootDirectory = String.Join("\\", gmParts);
            foreach (String ending in fileEndings.Split(','))
            {
                if (File.Exists(Path.Combine(gameModePath, file.Replace('\\', Path.DirectorySeparatorChar) + ending)) == true)
                    return new ContentManager(Core.GameInstance.Services, rootDirectory);
            }
        }

        return new ContentManager(Core.GameInstance.Services, "Content");
    }

    public static void CreateContentPackFolder()
    {
        if (Directory.Exists(Path.Combine(GameController.GamePath, "ContentPacks")) == false)
            Directory.CreateDirectory(Path.Combine(GameController.GamePath, "ContentPacks"));
    }

    public static String[] GetContentPackInfo(String contentPackName)
    {
        String infoPath = Path.Combine(GameController.GamePath, "ContentPacks", contentPackName, "info.dat");
        if (File.Exists(infoPath) == false)
        {
            String s = "1.00" + Environment.NewLine + "Pokémon3D" + Environment.NewLine + "[Add information here!]";
            File.WriteAllText(infoPath, s);
        }
        return File.ReadAllLines(infoPath);
    }

    public static void Clear()
    {
        _textureReplacements.Clear();
        _textureResolutions.Clear();
        _filesExist.Clear();
        PokeModelScale.Clear();
        PokeModelRotation.Clear();
        MusicManager.Clear();
        SoundManager.Clear();
        ModelManager.Clear();
        TextureManager.TextureList.Clear();
        TextureManager.TextureRectList.Clear();
        Water.ClearAnimationResources();
        Whirlpool.LoadedWaterTemp = false;
        Waterfall.ClearAnimationResources();
        AnimatedBlock.ClearAnimationResources();
        Logger.Debug("---Cleared ContentPackManager---");
    }
}

public class TextureSource
{
    public String TexturePath { get; set; } = String.Empty;
    public Rectangle TextureRectangle { get; set; }

    public TextureSource(String texturePath, Rectangle textureRectangle)
    {
        TexturePath = texturePath;
        TextureRectangle = textureRectangle;
    }

    public String GetString() =>
        TexturePath + "," + TextureRectangle.X + "," + TextureRectangle.Y + "," + TextureRectangle.Width + "," + TextureRectangle.Height;

    public bool IsEqual(TextureSource other) =>
        TexturePath.ToLower() == other.TexturePath.ToLower() && TextureRectangle == other.TextureRectangle;

    public bool IsEqual(String texturePath, Rectangle textureRectangle) =>
        IsEqual(new TextureSource(texturePath, textureRectangle));
}
