using Microsoft.Xna.Framework;

namespace P3D;

public class GameOptions
{
    private const int DEFAULT_RENDER_DISTANCE = 3;
    private const float DEFAULT_WINDOW_WIDTH = 1200f;
    private const float DEFAULT_WINDOW_HEIGHT = 680f;
    private const int MIN_WINDOW_DIM = 1;
    private const int MAX_WINDOW_DIM = 4096;

    public int RenderDistance = DEFAULT_RENDER_DISTANCE;
    public int ShowDebug;
    public bool ShowGUI = true;
    public int GraphicStyle = 1;
    public int LoadOffsetMaps = 1;
    public String[] ContentPackNames = [];
    public bool ViewBobbing = true;
    public bool LightingEnabled = true;
    public bool GamePadEnabled = true;
    public Vector2 GamePadInvertRightStick = new Vector2(0, 0);
    public bool StartedOfflineGame;
    public Vector2 WindowSize = new Vector2(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
    public int MaxOffsetLevel;
    public bool UpdateDisabled;
    public List<String> Extras = [];

    private int _interfaceScale;
    public int InterfaceScale
    {
        get => _interfaceScale;
        set => _interfaceScale = value;
    }

    // VB.NET stores booleans as integers ("0" = false, "-1"/"1" = true) in option files.
    private static bool ParseBool(String value)
    {
        if (int.TryParse(value, out int intVal))
        {
            return intVal != 0;
        }
        return bool.Parse(value);
    }

    public void LoadOptions()
    {
        KeyBindings.CreateKeySave(false);

        String saveDir = Path.Combine(GameController.GamePath, "Save");
        Directory.CreateDirectory(saveDir);

        String optionsPath = Path.Combine(saveDir, "options.dat");
        if (File.Exists(optionsPath) == false)
        {
            CreateOptions();
        }

        String[] lines = File.ReadAllText(optionsPath).SplitAtNewline();

        bool languageFound = false;

        foreach (String line in lines)
        {
            if (String.IsNullOrEmpty(line) == true)
            {
                continue;
            }

            String name = line.GetSplit(0, "|");
            String value = line.GetSplit(1, "|");

            switch (name.ToLower())
            {
                case "volume":
                    MusicManager.MasterVolume = int.Parse(value) / 100f;
                    SoundManager.Volume = int.Parse(value) / 100f;
                    break;

                case "music":
                    MusicManager.MasterVolume = int.Parse(value) / 100f;
                    break;

                case "sound":
                    SoundManager.Volume = int.Parse(value) / 100f;
                    break;

                case "muted":
                    SoundManager.Muted = ParseBool(value);
                    MusicManager.Muted = ParseBool(value);
                    break;

                case "renderdistance":
                    RenderDistance = int.Parse(value);
                    break;

                case "showdebug":
                    ShowDebug = int.Parse(value);
                    break;

                case "showboundingboxes":
                    Entity.drawViewBox = ParseBool(value);
                    break;

                case "showdebugconsole":
                    Logger.DisplayLog = ParseBool(value);
                    break;

                case "showgui":
                    ShowGUI = ParseBool(value);
                    break;

                case "graphicstyle":
                    GraphicStyle = int.Parse(value);
                    break;

                case "loadoffsetmaps":
                    LoadOffsetMaps = int.Parse(value);
                    break;

                case "language":
                    languageFound = true;
                    Localization.Load(value);
                    break;

                case "contentpack":
                case "contentpacks":
                    ContentPackManager.CreateContentPackFolder();
                    if (String.IsNullOrEmpty(value) == false)
                    {
                        ContentPackNames = value.Split(',');
                        List<String> valid = [];
                        foreach (String c in ContentPackNames)
                        {
                            String packPath = Path.Combine(GameController.GamePath, "ContentPacks", c);
                            if (Directory.Exists(packPath) == false)
                            {
                                continue;
                            }
                            ContentPackManager.Load(Path.Combine(packPath, "exceptions.dat"));
                            valid.Add(c);
                        }
                        ContentPackNames = valid.ToArray();
                    }
                    break;

                case "viewbobbing":
                    ViewBobbing = ParseBool(value);
                    break;

                case "lightingenabled":
                case "lightningenabled":
                    LightingEnabled = ParseBool(value);
                    break;

                case "gamepadenabled":
                    GamePadEnabled = ParseBool(value);
                    break;

                case "gamepadinvertrightstick":
                    GamePadInvertRightStick = new Vector2(
                        Math.Abs(int.Parse(value.GetSplit(0, ",")).Clamp(0, 1)),
                        Math.Abs(int.Parse(value.GetSplit(1, ",")).Clamp(0, 1)));
                    break;

                case "startedofflinegame":
                    StartedOfflineGame = ParseBool(value);
                    break;

                case "prefermultisampling":
                    Core.GraphicsManager.PreferMultiSampling = ParseBool(value);
                    break;

                case "windowsize":
                    if (value.Contains(',') == true)
                    {
                        String[] parts = value.Split(',');
                        if (StringHelper.IsNumeric(parts[0]) == true)
                        {
                            WindowSize.X = float.Parse(
                                parts[0].Replace(".", GameController.DecSeparator))
                                .Clamp(MIN_WINDOW_DIM, MAX_WINDOW_DIM);
                        }
                        if (StringHelper.IsNumeric(parts[1]) == true)
                        {
                            WindowSize.Y = float.Parse(
                                parts[1].Replace(".", GameController.DecSeparator))
                                .Clamp(MIN_WINDOW_DIM, MAX_WINDOW_DIM);
                        }
                    }
                    break;

                case "maxoffsetlevel":
                    MaxOffsetLevel = int.Parse(value);
                    break;

                case "extras":
                    if (String.IsNullOrEmpty(value) == false)
                    {
                        Extras = value.Split(';').ToList();
                    }
                    break;

                case "updatedisabled":
                    UpdateDisabled = ParseBool(value);
                    break;

                case "interfacescale":
                    InterfaceScale = int.Parse(value);
                    break;

                default:
                    break;
            }
        }

        if (languageFound == false)
        {
            Localization.Load("en");
        }
    }

    public void SaveOptions()
    {
        if (MapPreviewScreen.MapViewMode == true)
        {
            return;
        }

        String contentPacks = String.Join(",", ContentPackNames);

        if (Core.windowSize.Width == 0)
        {
            Core.windowSize.Width = (int)DEFAULT_WINDOW_WIDTH;
        }
        if (Core.windowSize.Height == 0)
        {
            Core.windowSize.Height = (int)DEFAULT_WINDOW_HEIGHT;
        }

        String data = String.Join(Environment.NewLine, new[]
        {
            $"Music|{(int)(MusicManager.MasterVolume * 100)}",
            $"Sound|{(int)(SoundManager.Volume * 100)}",
            $"Muted|{MusicManager.Muted.ToNumberString()}",
            $"RenderDistance|{RenderDistance}",
            $"ShowDebug|{ShowDebug}",
            $"ShowBoundingBoxes|{Entity.drawViewBox.ToNumberString()}",
            $"ShowDebugConsole|{Logger.DisplayLog.ToNumberString()}",
            $"ShowGUI|{ShowGUI.ToNumberString()}",
            $"GraphicStyle|{GraphicStyle}",
            $"LoadOffsetMaps|{LoadOffsetMaps}",
            $"Language|{Localization.LanguageSuffix}",
            $"ViewBobbing|{ViewBobbing.ToNumberString()}",
            $"GamePadEnabled|{GamePadEnabled.ToNumberString()}",
            $"GamePadInvertRightStick|{GamePadInvertRightStick.X},{GamePadInvertRightStick.Y}",
            $"LightingEnabled|{LightingEnabled.ToNumberString()}",
            $"StartedOfflineGame|{StartedOfflineGame.ToNumberString()}",
            $"PreferMultiSampling|{Core.GraphicsManager.PreferMultiSampling.ToNumberString()}",
            $"ContentPacks|{contentPacks}",
            $"WindowSize|{Core.windowSize.Width},{Core.windowSize.Height}",
            $"MaxOffsetLevel|{MaxOffsetLevel}",
            $"UpdateDisabled|{UpdateDisabled.ToNumberString()}",
            $"InterfaceScale|{InterfaceScale}",
            $"Extras|{String.Join(";", Extras)}"
        });

        File.WriteAllText(Path.Combine(GameController.GamePath, "Save", "options.dat"), data);
        KeyBindings.SaveKeys();

        Logger.Debug("---Options saved---");
    }

    private void CreateOptions()
    {
        String s = String.Join(Environment.NewLine, new[]
        {
            "Music|50", "Sound|50", "Muted|0",
            $"RenderDistance|{DEFAULT_RENDER_DISTANCE}",
            "ShowDebug|0", "ShowBoundingBoxes|0", "ShowDebugConsole|0",
            "ShowGUI|1", "GraphicStyle|1", "LoadOffsetMaps|1", "Language|en",
            "ViewBobbing|1", "GamePadEnabled|1", "GamePadInvertRightStick|0,0",
            "LightingEnabled|1", "StartedOfflineGame|0", "PreferMultiSampling|1",
            "ContentPacks|",
            $"WindowSize|{(int)DEFAULT_WINDOW_WIDTH},{(int)DEFAULT_WINDOW_HEIGHT}",
            "MaxOffsetLevel|0", "UpdateDisabled|0", "InterfaceScale|0", "Extras|"
        });

        File.WriteAllText(Path.Combine(GameController.GamePath, "Save", "options.dat"), s);
    }
}
