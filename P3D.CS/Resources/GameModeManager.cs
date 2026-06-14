using Microsoft.Xna.Framework;

namespace P3D;

public static partial class GameModeManager
{
    private static List<GameMode> _gameModes = [];
    private static int _gameModePointer = 0;

    public static bool Initialized = false;
    public static int ForceWaterSpeed = -1;

    public static int GameModeCount => _gameModes.Count;

    public static GameMode? ActiveGameMode
    {
        get
        {
            if (_gameModes.Count - 1 >= _gameModePointer)
                return _gameModes[_gameModePointer];
            return null;
        }
    }

    public static void LoadGameModes()
    {
        _gameModes.Clear();
        _gameModePointer = 0;

        CreateKolbenMode();

        foreach (String gameModefolder in Directory.GetDirectories(BuildPath("GameModes")))
        {
            if (File.Exists(Path.Combine(gameModefolder, "GameMode.dat")) == true)
                AddGameMode(gameModefolder);
        }

        SetGameModePointer("Kolben");
        Initialized = true;
    }

    public static GameMode? GetGameMode(String gameModedirectory)
    {
        foreach (GameMode gameMode in _gameModes)
        {
            if (gameMode.DirectoryName == gameModedirectory)
                return gameMode;
        }
        return null;
    }

    public static void CreateGameModesFolder()
    {
        if (Directory.Exists(BuildPath("GameModes")) == false)
            Directory.CreateDirectory(BuildPath("GameModes"));
    }

    public static void SetGameModePointer(String gameModeDirectoryName)
    {
        for (int i = 0; i < _gameModes.Count; i++)
        {
            if (_gameModes[i].DirectoryName == gameModeDirectoryName)
            {
                _gameModePointer = i;
                Logger.Debug("---Set pointer to \"" + gameModeDirectoryName + "\"!---");
                return;
            }
        }
        Logger.Debug("Couldn't find the GameMode \"" + gameModeDirectoryName + "\"!");
    }

    public static GameMode[] GetAllGameModes() => _gameModes.ToArray();

    private static String BuildPath(params String[] segments)
    {
        List<String> parts = [GameController.GamePath];
        foreach (String seg in segments)
            parts.AddRange(seg.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries));
        return Path.Combine(parts.ToArray());
    }

    public static bool GameModeExists(String gameModePath)
    {
        foreach (GameMode gameMode in _gameModes)
        {
            if (gameMode.DirectoryName == gameModePath)
                return true;
        }
        return false;
    }

    private static void AddGameMode(String path)
    {
        GameMode newGameMode = new GameMode(Path.Combine(path, "GameMode.dat"));
        if (newGameMode.IsValid == true)
            _gameModes.Add(newGameMode);
    }

    public static void CreateKolbenMode()
    {
        if (Directory.Exists(BuildPath("GameModes", "Kolben")) == true)
            Directory.Delete(BuildPath("GameModes", "Kolben"), true);

        bool doCreateKolbenMode = false;
        if (Directory.Exists(BuildPath("GameModes", "Kolben")) == false)
        {
            doCreateKolbenMode = true;
            Directory.CreateDirectory(BuildPath("GameModes", "Kolben"));
        }
        if (doCreateKolbenMode == false)
        {
            if (File.Exists(BuildPath("GameModes", "Kolben", "GameMode.dat")) == false)
                doCreateKolbenMode = true;
        }

        if (doCreateKolbenMode == true)
        {
            GameMode kolbenMode = GameMode.GetKolbenGameMode();
            kolbenMode.SaveToFile(BuildPath("GameModes", "Kolben", "GameMode.dat"));
        }
    }

    public static List<GameMode.GameRule> GetGameRules() => ActiveGameMode!.GameRules;

    public static String GetGameRuleValue(String ruleName, String defaultValue)
    {
        while (true)
        {
            List<GameMode.GameRule> rules = GetGameRules();
            foreach (GameMode.GameRule rule in rules)
            {
                if (rule.RuleName.ToLower() == ruleName.ToLower())
                    return rule.RuleValue;
            }
            ActiveGameMode!.GameRules.Add(new GameMode.GameRule(ruleName, defaultValue));
        }
    }

    public static String GetMapPath(String levelFile)
    {
        if (ActiveGameMode!.IsDefaultGamemode == true)
            return BuildPath(GameMode.DefaultMapPath, levelFile);

        if (File.Exists(BuildPath(ActiveGameMode.MapPath, levelFile)) == true)
            return BuildPath(ActiveGameMode.MapPath, levelFile);

        if (BuildPath(GameMode.DefaultMapPath, levelFile) != BuildPath(ActiveGameMode.MapPath, levelFile))
            Logger.Log(Logger.LogTypes.Message, "Map file: \"" + ActiveGameMode.MapPath + levelFile + "\" does not exist in the GameMode. The game tries to load the normal file at \"\\maps\\" + levelFile + "\".");

        return BuildPath(GameMode.DefaultMapPath, levelFile);
    }

    public static String GetScriptPath(String scriptFile)
    {
        if (ActiveGameMode!.IsDefaultGamemode == true)
            return BuildPath(GameMode.DefaultScriptPath, scriptFile);

        if (File.Exists(BuildPath(ActiveGameMode.ScriptPath, scriptFile)) == true)
            return BuildPath(ActiveGameMode.ScriptPath, scriptFile);

        if (BuildPath(GameMode.DefaultScriptPath, scriptFile) != BuildPath(ActiveGameMode.ScriptPath, scriptFile))
            Logger.Log(Logger.LogTypes.Message, "Script file: \"" + ActiveGameMode.ScriptPath + scriptFile + "\" does not exist in the GameMode. The game tries to load the normal file at \"\\Scripts\\" + scriptFile + "\".");

        return BuildPath(GameMode.DefaultScriptPath, scriptFile);
    }

    public static String GetPokeFilePath(String pokeFile)
    {
        if (ActiveGameMode!.IsDefaultGamemode == true)
            return BuildPath(GameMode.DefaultPokeFilePath, pokeFile);

        if (File.Exists(BuildPath(ActiveGameMode.PokeFilePath, pokeFile)) == true)
            return BuildPath(ActiveGameMode.PokeFilePath, pokeFile);

        if (BuildPath(GameMode.DefaultPokeFilePath, pokeFile) != BuildPath(ActiveGameMode.PokeFilePath, pokeFile))
            Logger.Log(Logger.LogTypes.Message, "Poke file: \"" + ActiveGameMode.PokeFilePath + pokeFile + "\" does not exist in the GameMode. The game tries to load the normal file at \"\\maps\\poke\\" + pokeFile + "\".");

        return BuildPath(GameMode.DefaultPokeFilePath, pokeFile);
    }

    public static String GetPokemonDataFilePath(String pokemonDataFile)
    {
        if (ActiveGameMode!.IsDefaultGamemode == true)
            return BuildPath(GameMode.DefaultPokemonDataPath, pokemonDataFile);

        if (File.Exists(BuildPath(ActiveGameMode.PokemonDataPath, pokemonDataFile)) == true)
            return BuildPath(ActiveGameMode.PokemonDataPath, pokemonDataFile);

        return BuildPath(GameMode.DefaultPokemonDataPath, pokemonDataFile);
    }

    public static String GetLocalizationsPath(String tokensFile)
    {
        if (File.Exists(BuildPath(ActiveGameMode!.LocalizationsPath, tokensFile)) == true)
            return BuildPath(ActiveGameMode.LocalizationsPath, tokensFile);

        return BuildPath(GameMode.DefaultLocalizationsPath, tokensFile);
    }

    public static String GetContentFilePath(String contentFile)
    {
        if (ActiveGameMode!.IsDefaultGamemode == true)
            return BuildPath(GameMode.DefaultContentPath, contentFile);

        if (File.Exists(BuildPath(ActiveGameMode.ContentPath, contentFile)) == true)
            return BuildPath(ActiveGameMode.ContentPath, contentFile);

        return BuildPath(GameMode.DefaultContentPath, contentFile);
    }

    public static String GetContentFilePath(String contentFile, String category) =>
        GetContentFilePath(contentFile);

    public static bool MapFileExists(String levelFile)
    {
        String path = ActiveGameMode!.IsDefaultGamemode == true
            ? BuildPath(GameMode.DefaultMapPath, levelFile)
            : BuildPath(ActiveGameMode.MapPath, levelFile);
        String defaultPath = BuildPath(GameMode.DefaultMapPath, levelFile);
        return File.Exists(path) || File.Exists(defaultPath);
    }

    public static bool ContentFileExists(String contentFile)
    {
        String path = ActiveGameMode!.IsDefaultGamemode == true
            ? BuildPath(GameMode.DefaultContentPath, contentFile)
            : BuildPath(ActiveGameMode.ContentPath, contentFile);
        String defaultPath = BuildPath(GameMode.DefaultContentPath, contentFile);
        return File.Exists(path) || File.Exists(defaultPath);
    }
}

public class GameMode
{
    public const String DefaultContentPath = @"\Content\";
    public const String DefaultMapPath = @"\Content\Data\maps\";
    public const String DefaultScriptPath = @"\Content\Data\Scripts\";
    public const String DefaultPokeFilePath = @"\Content\Data\maps\poke\";
    public const String DefaultPokemonDataPath = @"\Content\Pokemon\Data\";
    public const String DefaultLocalizationsPath = @"\Content\Localization\";

    private bool _loaded = false;
    private String _usedFileName = String.Empty;

    private String _name = String.Empty;
    private String _description = String.Empty;
    private String _version = String.Empty;
    private String _author = String.Empty;
    private String _mapPath = String.Empty;
    private String _scriptPath = String.Empty;
    private String _pokeFilePath = String.Empty;
    private String _pokemonDataPath = String.Empty;
    private String _localizationsPath = String.Empty;
    private String _contentPath = @"\Content\";
    private List<GameRule> _gameRules = [];
    private List<GameRule> _hardGameRules = [];
    private List<GameRule> _superHardGameRules = [];
    private int _waterSpeed = 4;
    private int _masterShinyRate = 4096;

    private String _startMap = String.Empty;
    private Vector3 _startPosition = new Vector3(14, 0.1f, 10);
    private float _startPitch = -0.2f;
    private float _startRotation = 0.0f;
    private String _startLocationName = String.Empty;
    private String _startDialogue = String.Empty;
    private Color _startColor = new Color(59, 123, 165);
    private String _pokemonAppear = String.Empty;
    private String _introMusic = String.Empty;
    private String _introType = String.Empty;
    private List<Color> _skinColors = [];
    private List<String> _skinFiles = [];
    private List<String> _skinNames = [];
    private List<String> _skinGenders = [];
    private int[] _pokemonRange = [1, 252];
    private float _pokeModelScale = 1.0f;
    private Vector3 _pokeModelRotation = Vector3.Zero;

    public bool IsValid
    {
        get
        {
            if (_loaded == true)
            {
                if (Name.ToLower() == "pokemon 3d" && DirectoryName.ToLower() != "kolben")
                {
                    Logger.Log(Logger.LogTypes.Message, "Unofficial GameMode with the name \"Pokemon 3D\" exists (in folder: \"" + DirectoryName + "\")!");
                    return false;
                }
                return true;
            }
            return false;
        }
    }

    public String DirectoryName
    {
        get
        {
            if (_usedFileName != String.Empty)
                return System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(_usedFileName) ?? String.Empty);
            return String.Empty;
        }
    }

    public String Path
    {
        get
        {
            if (_usedFileName != String.Empty)
            {
                String dir = System.IO.Path.GetDirectoryName(_usedFileName) ?? String.Empty;
                return dir.Length > 0 ? dir + System.IO.Path.DirectorySeparatorChar : String.Empty;
            }
            return String.Empty;
        }
    }

    public bool IsDefaultGamemode => Name == "Kolben";

    public String Name
    {
        get => _name;
        set => _name = value;
    }

    public String Description
    {
        get => _description;
        set => _description = value;
    }

    public String Version
    {
        get => _version;
        set => _version = value;
    }

    public String Author
    {
        get => _author;
        set => _author = value;
    }

    public String MapPath
    {
        get => _mapPath.Replace("$Mode", "\\GameModes\\" + DirectoryName);
        set => _mapPath = value;
    }

    public String ScriptPath
    {
        get => _scriptPath.Replace("$Mode", "\\GameModes\\" + DirectoryName);
        set => _scriptPath = value;
    }

    public String PokeFilePath
    {
        get => _pokeFilePath.Replace("$Mode", "\\GameModes\\" + DirectoryName);
        set => _pokeFilePath = value;
    }

    public String PokemonDataPath
    {
        get => _pokemonDataPath.Replace("$Mode", "\\GameModes\\" + DirectoryName);
        set => _pokemonDataPath = value;
    }

    public String ContentPath
    {
        get => _contentPath.Replace("$Mode", "\\GameModes\\" + DirectoryName);
        set => _contentPath = value;
    }

    public String LocalizationsPath
    {
        get => _localizationsPath.Replace("$Mode", "\\GameModes\\" + DirectoryName);
        set => _localizationsPath = value;
    }

    public int WaterSpeed
    {
        get
        {
            if (GameModeManager.ForceWaterSpeed != -1)
                return GameModeManager.ForceWaterSpeed;
            return _waterSpeed;
        }
        set => _waterSpeed = value;
    }

    public int MasterShinyRate
    {
        get => _masterShinyRate;
        set => _masterShinyRate = value;
    }

    public List<GameRule> GameRules
    {
        get
        {
            return Core.Player.DifficultyMode switch
            {
                0 => _gameRules,
                1 => _hardGameRules,
                2 => _superHardGameRules,
                _ => _gameRules,
            };
        }
        set
        {
            switch (Core.Player.DifficultyMode)
            {
                case 0: _gameRules = value; break;
                case 1: _hardGameRules = value; break;
                case 2: _superHardGameRules = value; break;
                default:
                    _gameRules = value;
                    _hardGameRules = value;
                    _superHardGameRules = value;
                    break;
            }
        }
    }

    public String StartMap
    {
        get => _startMap;
        set => _startMap = value;
    }

    public Vector3 StartPosition
    {
        get => _startPosition;
        set => _startPosition = value;
    }

    public float StartPitch
    {
        get => _startPitch;
        set => _startPitch = value;
    }

    public float StartRotation
    {
        get => _startRotation;
        set => _startRotation = value;
    }

    public String StartLocationName
    {
        get => _startLocationName;
        set => _startLocationName = value;
    }

    public String StartDialogue
    {
        get => _startDialogue;
        set => _startDialogue = value;
    }

    public Color StartColor
    {
        get => _startColor;
        set => _startColor = value;
    }

    public String PokemonAppear
    {
        get => _pokemonAppear;
        set
        {
            _pokemonAppear = value;
            if (StringHelper.IsNumeric(value) == true && int.Parse(value) == 0)
            {
                _pokemonRange = [1, 252];
            }
            else if (value.Contains("-") == true)
            {
                int v1 = int.Parse(value.GetSplit(0, "-"));
                int v2 = int.Parse(value.GetSplit(1, "-")) + 1;
                _pokemonRange = [v1, v2];
            }
            else if (StringHelper.IsNumeric(value) == true)
            {
                int v = int.Parse(value);
                _pokemonRange = [v, v + 1];
            }
        }
    }

    public int[] PokemonRange => _pokemonRange;

    public float PokeModelScale => _pokeModelScale;
    public Vector3 PokeModelRotation => _pokeModelRotation;

    public String IntroMusic
    {
        get => _introMusic;
        set => _introMusic = value;
    }

    public String IntroType
    {
        get => _introType;
        set => _introType = value;
    }

    public List<Color> SkinColors
    {
        get => _skinColors;
        set => _skinColors = value;
    }

    public List<String> SkinFiles
    {
        get => _skinFiles;
        set => _skinFiles = value;
    }

    public List<String> SkinNames
    {
        get => _skinNames;
        set => _skinNames = value;
    }

    public List<String> SkinGenders
    {
        get => _skinGenders;
        set => _skinGenders = value;
    }

    public String StartScript { get; set; } = String.Empty;

    public List<String> ContentPackNames { get; set; } = [];

    public GameMode(String fileName)
    {
        Load(fileName);
    }

    public GameMode(String name, String description, String version, String author,
        String mapPath, String scriptPath, String pokeFilePath, String pokemonDataPath,
        String contentPath, String localizationsPath,
        List<GameRule> gameRules, List<GameRule> hardGameRules, List<GameRule> superHardGameRules,
        String startMap, Vector3 startPosition, float startRotation, String startLocationName,
        String startDialogue, Color startColor, String pokemonAppear, String introMusic,
        String introType, List<Color> skinColors, List<String> skinFiles, List<String> skinNames,
        List<String> skinGenders, float startPitch, int waterSpeed = 4, int masterShinyRate = 4096,
        float pokeModelScale = 1.0f, Vector3 pokeModelRotation = default)
    {
        _name = name;
        _description = description;
        _version = version;
        _author = author;
        _mapPath = mapPath;
        _scriptPath = scriptPath;
        _pokeFilePath = pokeFilePath;
        _pokemonDataPath = pokemonDataPath;
        _contentPath = contentPath;
        _localizationsPath = localizationsPath;
        _gameRules = gameRules;
        _hardGameRules = hardGameRules;
        _superHardGameRules = superHardGameRules;
        _waterSpeed = waterSpeed;
        _masterShinyRate = masterShinyRate;
        _pokeModelScale = pokeModelScale;
        if (pokeModelRotation != default)
            _pokeModelRotation = pokeModelRotation;
        _startMap = startMap;
        _startPosition = startPosition;
        _startRotation = startRotation;
        _startPitch = startPitch;
        _startLocationName = startLocationName;
        _startDialogue = startDialogue;
        _startColor = startColor;
        _introMusic = introMusic;
        _introType = introType;
        _skinColors = skinColors;
        _skinFiles = skinFiles;
        _skinNames = skinNames;
        _skinGenders = skinGenders;
        PokemonAppear = pokemonAppear;
        _loaded = true;
    }

    private void Load(String fileName)
    {
        if (File.Exists(fileName) == false) return;

        String[] data = File.ReadAllLines(fileName);
        foreach (String line in data)
        {
            if (line != String.Empty && line.Contains("|") == true)
            {
                String pointer = line.Remove(line.IndexOf("|"));
                String value = line.Remove(0, line.IndexOf("|") + 1);

                switch (pointer.ToLower())
                {
                    case "name": _name = value; break;
                    case "description": _description = value; break;
                    case "version": _version = value; break;
                    case "author": _author = value; break;
                    case "mappath": _mapPath = value; break;
                    case "scriptpath": _scriptPath = value; break;
                    case "pokefilepath": _pokeFilePath = value; break;
                    case "pokemondatapath": _pokemonDataPath = value; break;
                    case "contentpath": _contentPath = value; break;
                    case "localizationspath": _localizationsPath = value; break;
                    case "waterspeed": _waterSpeed = int.Parse(value); break;
                    case "shinyrate": _masterShinyRate = int.Parse(value); break;
                    case "startmap": _startMap = value; break;
                    case "startrotation":
                    case "startyaw":
                        _startRotation = float.Parse(value.Replace(".", GameController.DecSeparator)); break;
                    case "startpitch":
                        _startPitch = float.Parse(value.Replace(".", GameController.DecSeparator)); break;
                    case "startscript": StartScript = value; break;
                    case "startlocationname": _startLocationName = value; break;
                    case "startdialogue": _startDialogue = value; break;
                    case "pokemonappear": PokemonAppear = value; break;
                    case "pokemodelscale":
                        _pokeModelScale = float.Parse(value.Replace(".", GameController.DecSeparator)); break;
                    case "intromusic": _introMusic = value; break;
                    case "introtype": _introType = value; break;
                    case "startposition":
                    {
                        String[] parts = value.Split(',');
                        if (parts.Length >= 3)
                        {
                            _startPosition = new Vector3(
                                float.Parse(parts[0].Replace(".", GameController.DecSeparator)),
                                float.Parse(parts[1].Replace(".", GameController.DecSeparator)),
                                float.Parse(parts[2].Replace(".", GameController.DecSeparator)));
                        }
                        else
                        {
                            _startPosition = Vector3.Zero;
                        }
                        break;
                    }
                    case "startcolor":
                    {
                        if (value != String.Empty && value.CountSplits(",") == 3)
                        {
                            String[] c = value.Split(',');
                            _startColor = new Color(int.Parse(c[0]), int.Parse(c[1]), int.Parse(c[2]));
                        }
                        else
                        {
                            _startColor = new Color(59, 123, 165);
                        }
                        break;
                    }
                    case "pokemodelrotation":
                    {
                        String[] parts = value.Split(',');
                        float x = float.Parse(parts[0].Replace(".", GameController.DecSeparator));
                        float y = 0.0f;
                        float z = 0.0f;
                        if (parts.Length > 2)
                        {
                            y = float.Parse(parts[1].Replace(".", GameController.DecSeparator));
                            z = float.Parse(parts[2].Replace(".", GameController.DecSeparator));
                        }
                        else if (parts.Length > 1)
                        {
                            z = float.Parse(parts[1].Replace(".", GameController.DecSeparator));
                        }
                        _pokeModelRotation = new Vector3(x, y, z);
                        break;
                    }
                    case "gamerules":
                        if (value != String.Empty && value.Contains("(") && value.Contains(")") && value.Contains("|") == true)
                        {
                            foreach (String rule in value.Split(')'))
                            {
                                if (rule.StartsWith("(") == true)
                                {
                                    String r = rule.Remove(0, 1);
                                    GameRule gr = new GameRule(r.GetSplit(0, "|"), r.GetSplit(1, "|"));
                                    _gameRules.Add(gr);
                                    _hardGameRules.Add(new GameRule(r.GetSplit(0, "|"), r.GetSplit(1, "|")));
                                    _superHardGameRules.Add(new GameRule(r.GetSplit(0, "|"), r.GetSplit(1, "|")));
                                }
                            }
                        }
                        break;
                    case "hardgamerules":
                        if (value != String.Empty && value.Contains("(") && value.Contains(")") && value.Contains("|") == true)
                        {
                            foreach (String rule in value.Split(')'))
                            {
                                if (rule.StartsWith("(") == true)
                                {
                                    String r = rule.Remove(0, 1);
                                    for (int i = _hardGameRules.Count - 1; i >= 0; i--)
                                    {
                                        if (i < _hardGameRules.Count && _hardGameRules[i].RuleName.ToLower() == r.GetSplit(0, "|").ToLower())
                                            _hardGameRules.RemoveAt(i);
                                    }
                                    for (int i = _superHardGameRules.Count - 1; i >= 0; i--)
                                    {
                                        if (i < _superHardGameRules.Count && _superHardGameRules[i].RuleName.ToLower() == r.GetSplit(0, "|").ToLower())
                                            _superHardGameRules.RemoveAt(i);
                                    }
                                    _hardGameRules.Add(new GameRule(r.GetSplit(0, "|"), r.GetSplit(1, "|")));
                                    _superHardGameRules.Add(new GameRule(r.GetSplit(0, "|"), r.GetSplit(1, "|")));
                                }
                            }
                        }
                        break;
                    case "superhardgamerules":
                        if (value != String.Empty && value.Contains("(") && value.Contains(")") && value.Contains("|") == true)
                        {
                            foreach (String rule in value.Split(')'))
                            {
                                if (rule.StartsWith("(") == true)
                                {
                                    String r = rule.Remove(0, 1);
                                    for (int i = _superHardGameRules.Count - 1; i >= 0; i--)
                                    {
                                        if (i < _superHardGameRules.Count && _superHardGameRules[i].RuleName.ToLower() == r.GetSplit(0, "|").ToLower())
                                            _superHardGameRules.RemoveAt(i);
                                    }
                                    _superHardGameRules.Add(new GameRule(r.GetSplit(0, "|"), r.GetSplit(1, "|")));
                                }
                            }
                        }
                        break;
                    case "skincolors":
                    {
                        List<Color> l = [];
                        foreach (String color in value.Split(','))
                        {
                            l.Add(new Color(int.Parse(color.GetSplit(0, ";")), int.Parse(color.GetSplit(1, ";")), int.Parse(color.GetSplit(2, ";"))));
                        }
                        if (l.Count > 0) _skinColors = l;
                        break;
                    }
                    case "skinfiles":
                    {
                        List<String> l = [.. value.Split(',')];
                        if (l.Count > 0) _skinFiles = l;
                        break;
                    }
                    case "skinnames":
                    {
                        List<String> l = [.. value.Split(',')];
                        if (l.Count > 0) _skinNames = l;
                        break;
                    }
                    case "skingenders":
                    {
                        List<String> l = [.. value.Split(',')];
                        if (l.Count > 0) _skinGenders = l;
                        break;
                    }
                }
            }
        }

        _loaded = true;
        _usedFileName = fileName;
    }

    public void Reload() => Load(_usedFileName);
    public void Reload(String fileName) => Load(fileName);

    public void SaveToFile(String file)
    {
        String s = "Name|" + _name + Environment.NewLine +
            "Description|" + _description + Environment.NewLine +
            "Version|" + _version + Environment.NewLine +
            "Author|" + _author + Environment.NewLine +
            "MapPath|" + _mapPath + Environment.NewLine +
            "ScriptPath|" + _scriptPath + Environment.NewLine +
            "PokeFilePath|" + _pokeFilePath + Environment.NewLine +
            "PokemonDataPath|" + _pokemonDataPath + Environment.NewLine +
            "ContentPath|" + _contentPath + Environment.NewLine +
            "LocalizationsPath|" + _localizationsPath + Environment.NewLine +
            "WaterSpeed|" + _waterSpeed + Environment.NewLine;

        String gameRuleString = "GameRules|";
        foreach (GameRule rule in _gameRules)
            gameRuleString += "(" + rule.RuleName + "|" + rule.RuleValue + ")";
        s += gameRuleString + Environment.NewLine;

        String hardGameRuleString = "HardGameRules|";
        foreach (GameRule rule in _hardGameRules)
            hardGameRuleString += "(" + rule.RuleName + "|" + rule.RuleValue + ")";

        String superHardGameRuleString = "SuperHardGameRules|";
        foreach (GameRule rule in _superHardGameRules)
            superHardGameRuleString += "(" + rule.RuleName + "|" + rule.RuleValue + ")";

        s += hardGameRuleString + Environment.NewLine +
            superHardGameRuleString + Environment.NewLine +
            "StartMap|" + _startMap + Environment.NewLine +
            "StartPosition|" + _startPosition.X.ToString().Replace(GameController.DecSeparator, ".") + "," +
                _startPosition.Y.ToString().Replace(GameController.DecSeparator, ".") + "," +
                _startPosition.Z.ToString().Replace(GameController.DecSeparator, ".") + Environment.NewLine +
            "StartPitch|" + _startPitch.ToString().Replace(GameController.DecSeparator, ".") + Environment.NewLine +
            "StartRotation|" + _startRotation.ToString().Replace(GameController.DecSeparator, ".") + Environment.NewLine +
            "StartScript|" + StartScript + Environment.NewLine +
            "StartLocationName|" + _startLocationName + Environment.NewLine +
            "StartDialogue|" + _startDialogue + Environment.NewLine +
            "StartColor|" + _startColor.R + "," + _startColor.G + "," + _startColor.B + Environment.NewLine +
            "PokemonAppear|" + _pokemonAppear + Environment.NewLine +
            "IntroMusic|" + _introMusic + Environment.NewLine +
            "IntroType|" + _introType + Environment.NewLine;

        String skinColorsString = "SkinColors|";
        for (int i = 0; i < _skinColors.Count; i++)
        {
            if (i > 0) skinColorsString += ",";
            skinColorsString += _skinColors[i].R + ";" + _skinColors[i].G + ";" + _skinColors[i].B;
        }
        s += skinColorsString + Environment.NewLine;

        String skinFilesString = "SkinFiles|";
        for (int i = 0; i < _skinFiles.Count; i++)
        {
            if (i > 0) skinFilesString += ",";
            skinFilesString += _skinFiles[i];
        }
        s += skinFilesString + Environment.NewLine;

        String skinNamesString = "SkinNames|";
        for (int i = 0; i < _skinNames.Count; i++)
        {
            if (i > 0) skinNamesString += ",";
            skinNamesString += _skinNames[i];
        }
        s += skinNamesString + Environment.NewLine;

        String skinGendersString = "SkinGenders|";
        for (int i = 0; i < _skinGenders.Count; i++)
        {
            if (i > 0) skinGendersString += ",";
            skinGendersString += _skinGenders[i];
        }
        s += skinGendersString;

        String? folder = System.IO.Path.GetDirectoryName(file);
        if (folder != null && Directory.Exists(folder) == false)
            Directory.CreateDirectory(folder);

        File.WriteAllText(file, s);
    }

    public static GameMode GetKolbenGameMode()
    {
        List<Color> skinColors = [new Color(245, 26, 33), new Color(26, 245, 48), new Color(255, 208, 16), new Color(248, 176, 32), new Color(248, 216, 88), new Color(152, 27, 8), new Color(8, 46, 152), new Color(8, 143, 152), new Color(148, 52, 145), new Color(56, 88, 200), new Color(216, 96, 112), new Color(56, 88, 152), new Color(239, 90, 156)];
        List<String> skinFiles = ["Red", "Green", "Yellow", "Ethan", "Lyra", "Brendan", "May", "Lucas", "Dawn", "Nate", "Rosa", "Hilbert", "Hilda"];
        List<String> skinNames = ["Red", "Green", "Yellow", "Ethan", "Lyra", "Brendan", "May", "Lucas", "Dawn", "Nate", "Rosa", "Hilbert", "Hilda"];
        List<String> skinGenders = ["Male", "Female", "Female", "Male", "Female", "Male", "Female", "Male", "Female", "Male", "Female", "Male", "Female"];

        GameMode gameMode = new GameMode("Kolben", "The normal GameMode.", GameController.GAME_VERSION, "Kolben Games",
            @"\Content\Data\maps\", @"\Content\Data\Scripts\", @"\Content\Data\maps\poke\", @"\Content\Pokemon\Data\",
            @"\Content\", @"\Content\Localization\",
            [], [], [],
            @"newgame\intro0.dat", new Vector3(6, 3, 7), 0.0f, "Your Room",
            String.Empty, new Color(59, 123, 165), "0", "welcome", "1",
            skinColors, skinFiles, skinNames, skinGenders, -0.3f, 4);

        gameMode.StartScript = "startscript\\main";

        List<GameRule> gameRules =
        [
            new GameRule("MaxLevel", "100"),
            new GameRule("OnlyCaptureFirst", "0"),
            new GameRule("ForceRename", "0"),
            new GameRule("DeathInsteadOfFaint", "0"),
            new GameRule("CanUseHealItems", "1"),
            new GameRule("Difficulty", "0"),
            new GameRule("LockDifficulty", "0"),
            new GameRule("GameOverAt0Pokemon", "0"),
            new GameRule("CanGetAchievements", "1"),
            new GameRule("ShowFollowPokemon", "1"),
            new GameRule("RandomFollowItemPickup", "1"),
            new GameRule("OverworldPoison", "0"),
            new GameRule("SavingDisabled", "0"),
            new GameRule("SingleUseTM", "0"),
            new GameRule("CanForgetHM", "0"),
            new GameRule("CoinCaseCap", "0"),
            new GameRule("GainExpAfterCatch", "1"),
            new GameRule("ShinyRate", "4096"),
            new GameRule("LevelMultiplier", "1.0"),
        ];
        gameMode._gameRules = gameRules;

        gameMode._hardGameRules = [new GameRule("OverworldPoison", "1"), new GameRule("LevelMultiplier", "1.1")];
        gameMode._superHardGameRules = [new GameRule("LevelMultiplier", "1.2")];

        return gameMode;
    }

    public class GameRule
    {
        private String _ruleName = "EMPTY";
        private String _ruleValue = "EMPTY";

        public GameRule(String name, String value)
        {
            _ruleName = name;
            _ruleValue = value;
        }

        public String RuleName
        {
            get => _ruleName;
            set => _ruleName = value;
        }

        public String RuleValue
        {
            get => _ruleValue;
            set => _ruleValue = value;
        }
    }
}
