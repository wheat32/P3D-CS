using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using P3D.Security;

namespace P3D;

public class Player : HashSecureBase
{
    private const String DEFAULT_NAME = "<player.name>";
    private const String DEFAULT_OT = "00000";
    private const String DEFAULT_REST_PLACE = "yourroom.dat";
    private const String DEFAULT_REST_POSITION = "1,0.1,3";
    private const String DEFAULT_START_MAP = "barktown.dat";
    private const String DEFAULT_SKIN = "Hilbert";
    private const String DEFAULT_RIVAL_SKIN = "Silver";
    private const String DEFAULT_GAMEMODE = "Kolben";
    private const String DEFAULT_EMBLEM_BG = "standard";
    private const int DEFAULT_GTS_STARS = 8;
    private const int DEFAULT_BOX_AMOUNT = 10;
    private const int DEFAULT_ROTATION_SPEED = 12;
    private const float DEFAULT_START_FOV = 60.0f;
    private const float EMBLEM_DISPLAY_DURATION = 35.0f;
    private const float EMBLEM_SLIDE_THRESHOLD = 6.4f;
    private const float EMBLEM_SLIDE_SPEED = 8.0f;
    private const int EMBLEM_TARGET_OFFSET = 512;
    private static readonly (int Min, int Max) OT_VALUE_RANGE = (0, 999999);
    private static readonly (float Min, float Max) FOV_RANGE = (1f, 179f);

    private static readonly String[] SAVE_FILE_NAMES =
    [
        "Apricorns", "Berries", "Box", "Daycare", "HallOfFame",
        "ItemData", "Items", "NPC", "Options", "Party", "Player",
        "Pokedex", "Register", "RoamingPokemon", "SecretBase"
    ];

    // Secure properties

    public String Name
    {
        get
        {
            Assert("_name", field);
            return field;
        }
        set
        {
            Assert("_name", field, value);
            field = value;
        }
    } = DEFAULT_NAME;

    public String OT
    {
        get
        {
            Assert("_ot", field);
            return field;
        }
        set
        {
            Assert("_ot", field, value);
            field = value;
        }
    } = DEFAULT_OT;

    public bool IsFlying { get; set; }

    public bool SandBoxMode
    {
        get
        {
            Assert("_sandboxmode", field);
            return field;
        }
        set
        {
            Assert("_sandboxmode", field, value);
            field = value;
        }
    }

    public bool IsGameJoltSave
    {
        get
        {
            Assert("_isgamejoltsave", field);
            return field;
        }
        set
        {
            Assert("_isgamejoltsave", field, value);
            field = value;
        }
    }

    public String EmblemBackground
    {
        get
        {
            Assert("_emblembackground", field);
            return field;
        }
        set
        {
            Assert("_emblembackground", field, value);
            field = value;
        }
    } = DEFAULT_EMBLEM_BG;

    // Simple properties

    public String RivalName { get; set; } = "???";
    public String RivalSkin { get; set; } = DEFAULT_RIVAL_SKIN;
    public String Gender { get; set; } = "Male";
    public bool Male
    {
        get => Gender != "Female";
        set => Gender = value == true ? "Male" : "Female";
    }
    public int Money { get; set; }
    public int Points { get; set; }
    public int BP { get; set; }
    public int Coins { get; set; }
    public bool HasPokedex { get; set; }
    public bool HasPokegear { get; set; }
    public String LastRestPlace { get; set; } = DEFAULT_REST_PLACE;
    public String LastRestPlacePosition { get; set; } = DEFAULT_REST_POSITION;
    public String LastSavePlace { get; set; } = DEFAULT_REST_PLACE;
    public String LastSavePlacePosition { get; set; } = DEFAULT_REST_POSITION;
    public int RepelSteps { get; set; }
    public String ScriptDelayItems { get; set; } = String.Empty;
    public int ScriptDelaySteps { get; set; }
    public bool ScriptDelayDisplaySteps { get; set; }
    public String SaveCreated { get; set; } = "Pre 0.21";
    public int DaycareSteps { get; set; }
    public int PoisonSteps { get; set; }
    public String GameMode { get; set; } = DEFAULT_GAMEMODE;
    public String Skin { get; set; } = DEFAULT_SKIN;
    public String VisitedMaps { get; set; } = String.Empty;
    public int GTSStars { get; set; } = DEFAULT_GTS_STARS;
    public String RegisterData { get; set; } = String.Empty;
    public String BerryData { get; set; } = String.Empty;
    public String PokedexData { get; set; } = String.Empty;
    public String ItemData { get; set; } = String.Empty;
    public String BoxData { get; set; } = String.Empty;
    public String NPCData { get; set; } = String.Empty;
    public String ApricornData { get; set; } = String.Empty;
    public String SecretBaseData { get; set; } = String.Empty;
    public String DaycareData { get; set; } = String.Empty;
    public String HallOfFameData { get; set; } = String.Empty;
    public String RoamingPokemonData { get; set; } = String.Empty;
    public String HistoryData { get; set; } = String.Empty;

    // Collection fields

    public List<Pokemon> Pokemons = [];
    public List<Pokedex> Pokedexes = [];
    public PlayerInventory Inventory = new PlayerInventory();
    public List<int> Badges = [];
    public TimeSpan PlayTime;
    public DateTime GameStart = DateTime.Now;
    public Vector3 LastPokemonPosition = new Vector3(999, 999, 999);
    public List<String> PokeFiles = [];
    public List<String> EarnedAchievements = [];
    public List<int> PokegearModules = [];
    public List<String> PhoneContacts = [];
    public List<Items.MailItem.MailData> Mails = [];
    public List<int> Trophies = [];

    // Non-secure fields

    public int ShowBattleAnimations = 1;
    public int BoxAmount = DEFAULT_BOX_AMOUNT;
    public bool DiagonalMovement;
    public int DifficultyMode;
    public int BattleStyle;
    public bool ShowModelsInBattle = true;
    public String TempSurfSkin = DEFAULT_SKIN;
    public String TempRideSkin = String.Empty;
    public String Statistics = String.Empty;
    public bool CheckForTrainersLater;
    public List<String> UsedItemsToCheckScriptDelayFor = [];

    public Vector3 StartPosition = new Vector3(14, 0.1f, 10);
    public float StartRotation;
    public bool StartFreeCameraMode;
    public String StartMap = DEFAULT_START_MAP;
    public float StartFOV = DEFAULT_START_FOV;
    public int StartRotationSpeed = DEFAULT_ROTATION_SPEED;
    public bool StartThirdPerson;
    public bool StartSurfing;
    public bool StartRiding;
    public int SurfPokemon;
    public bool EnableExpAll;

    public String FilePrefix = "nilllzz";
    public String NewFilePrefix = String.Empty;
    public bool AutosaveUsed;
    public bool loadedSave;

    public PlayerTemp PlayerTemp = new PlayerTemp();

    public bool RunMode = true;
    public bool RunToggled;
    public bool DoWalkAnimation = true;

    // Step-event private state

    public bool stepEventStartedTrainer;
    private bool _stepEventEggHatched;
    private bool _stepEventRepelMessage;

    // Level-up display state

    private int _lastLevel;
    private float _displayEmblemDelay;
    private int _emblemPositionX;

    // Temp struct

    public static class Temp
    {
        public static int PokemonScreenIndex;
        public static int PokemonStatusPageIndex;
        public static int BagIndex;
        public static int[] BagPageIndex = [ 0, 0, 0, 0, 0, 0, 0, 0 ];
        public static int[] BagItemIndex = [ 0, 0, 0, 0, 0, 0, 0, 0 ];
        public static int BagSelectIndex;
        public static int MenuIndex;
        public static int PokedexIndex;
        public static int PokemonSummaryPageIndex;
        public static int PCBoxIndex;
        public static Vector2 StorageSystemCursorPosition = new Vector2(1, 0);
        public static StorageSystemScreen.SelectionModes PCSelectionType = StorageSystemScreen.SelectionModes.SingleMove;
        public static int OptionScreenIndex;
        public static bool[] MapSwitch = new bool[4];
        public static Vector3 LastPosition;
        public static bool IsInBattle;
        public static Vector3 BeforeBattlePosition = Vector3.Zero;
        public static String BeforeBattleLevelFile = "yourroom.dat";
        public static int BeforeBattleFacing;
        public static int PokedexModeIndex;
        public static int PokedexHabitatIndex;
        public static int PokegearPage;
        public static int LastPokegearPage;
        public static int LastCall = 32;
        public static int LastUsedRepel = -1;
        public static int MapSteps;
        public static int HallOfFameIndex;
        public static bool PCBoxChooseMode;
        public static decimal RadioStation;
    }

    private void ResetTemp()
    {
        Temp.PokemonScreenIndex = 0;
        Temp.PokemonStatusPageIndex = 0;
        Temp.BagIndex = 0;
        Temp.BagPageIndex = [ 0, 0, 0, 0, 0, 0, 0, 0 ];
        Temp.BagItemIndex = [ 0, 0, 0, 0, 0, 0, 0, 0 ];
        Temp.BagSelectIndex = 0;
        Temp.MenuIndex = 0;
        Temp.PokedexIndex = 0;
        Temp.PCBoxIndex = 0;
        Temp.OptionScreenIndex = 0;
        Temp.IsInBattle = false;
        for (int i = 0; i < 4; i++)
        {
            Temp.MapSwitch[i] = true;
        }
        Temp.PokedexModeIndex = 0;
        Temp.PokedexHabitatIndex = 0;
        Temp.PokegearPage = 0;
        Temp.LastCall = 32;
        Temp.LastUsedRepel = -1;
        Temp.MapSteps = 0;
        Temp.HallOfFameIndex = 0;
        Temp.PCBoxChooseMode = false;
        Temp.StorageSystemCursorPosition = new Vector2(1, 0);
        Temp.RadioStation = 0m;
    }

    // Load

    public void LoadGame(String filePrefix)
    {
        // TODO Phase 12: wire up full LoadGame dependencies
        FilePrefix = filePrefix;
        PokeFiles.Clear();
        GameMode = DEFAULT_GAMEMODE;
        LoadPlayer();
        LoadPokedex();
        LoadParty();
        LoadItems();
        LoadBerries();
        LoadApricorns();
        LoadDaycare();
        LoadOptions();
        LoadRegister();
        LoadItemData();
        LoadBoxData();
        LoadNPCData();
        LoadHallOfFameData();
        LoadSecretBaseData();
        LoadRoamingPokemonData();
        LoadStatistics();
        PlayerTemp.Reset();
        ResetTemp();
        GameStart = DateTime.Now;
        loadedSave = true;
    }

    private void LoadPlayer()
    {
        String[] data = IsGameJoltSave == true
            ? Core.GameJoltSave.Player.SplitAtNewline()
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Player.dat"))
                .SplitAtNewline();

        foreach (String line in data)
        {
            if (String.IsNullOrEmpty(line) == true || line.Contains('|') == false)
            {
                Logger.Log(Logger.LogTypes.Warning,
                    $"Player.cs: The line \"{line}\" is either empty or does not conform to player.dat file rules.");
                continue;
            }

            String id = line[..line.IndexOf('|')];
            String value = line[(line.IndexOf('|') + 1)..];

            switch (id.ToLower())
            {
                case "name":
                    Name = value;
                    break;

                case "position":
                {
                    String[] v = value.Split(',');
                    StartPosition = new Vector3(
                        float.Parse(v[0].Replace(".", GameController.DecSeparator)),
                        float.Parse(v[1].Replace(".", GameController.DecSeparator)),
                        float.Parse(v[2].Replace(".", GameController.DecSeparator)));
                    break;
                }

                case "lastpokemonposition":
                {
                    String[] v = value.Split(',');
                    LastPokemonPosition = new Vector3(
                        float.Parse(v[0].Replace(".", GameController.DecSeparator)),
                        float.Parse(v[1].Replace(".", GameController.DecSeparator)),
                        float.Parse(v[2].Replace(".", GameController.DecSeparator)));
                    break;
                }

                case "mapfile":
                    StartMap = value;
                    break;

                case "rivalname":
                    RivalName = value;
                    break;

                case "rivalskin":
                    RivalSkin = value;
                    break;

                case "money":
                    Money = int.Parse(value);
                    break;

                case "badges":
                    Badges.Clear();
                    if (value.Equals("0") == false)
                    {
                        foreach (String b in value.Split(','))
                        {
                            Badges.Add(int.Parse(b));
                        }
                    }
                    break;

                case "rotation":
                    StartRotation = float.Parse(value.Replace(".", GameController.DecSeparator));
                    break;

                case "gender":
                    Gender = value.ToLower() is "male" or "0" ? "Male"
                           : value.ToLower() is "female" or "1" ? "Female"
                           : "Other";
                    break;

                case "playtime":
                {
                    String[] dd = value.Split(',');
                    PlayTime = dd.Length >= 4
                        ? new TimeSpan(int.Parse(dd[3]), int.Parse(dd[0]), int.Parse(dd[1]), int.Parse(dd[2]))
                        : new TimeSpan(int.Parse(dd[0]), int.Parse(dd[1]), int.Parse(dd[2]));
                    break;
                }

                case "ot":
                    OT = int.Parse(value).Clamp(OT_VALUE_RANGE.Min, OT_VALUE_RANGE.Max).ToString();
                    break;

                case "points":
                    Points = int.Parse(value);
                    break;

                case "haspokedex":
                    HasPokedex = bool.Parse(value);
                    break;

                case "haspokegear":
                    HasPokegear = bool.Parse(value);
                    break;

                case "fov":
                    StartFOV = float.Parse(value.Replace(".", GameController.DecSeparator))
                                    .Clamp(FOV_RANGE.Min, FOV_RANGE.Max);
                    break;

                case "freecamera":
                    StartFreeCameraMode = bool.Parse(value);
                    break;

                case "thirdperson":
                    StartThirdPerson = bool.Parse(value);
                    break;

                case "skin":
                    Skin = value;
                    break;

                case "battleanimations":
                    ShowBattleAnimations = int.Parse(value);
                    break;

                case "runmode":
                    RunMode = bool.Parse(value);
                    break;

                case "runtoggled":
                    RunToggled = bool.Parse(value);
                    break;

                case "boxamount":
                    BoxAmount = int.Parse(value);
                    break;

                case "lastrestplace":
                    LastRestPlace = value;
                    break;

                case "lastrestplaceposition":
                    LastRestPlacePosition = value;
                    break;

                case "diagonalmovement":
                    DiagonalMovement = GameController.IS_DEBUG_ACTIVE == true && bool.Parse(value);
                    break;

                case "repelsteps":
                    RepelSteps = int.Parse(value);
                    break;

                case "scriptdelayitems":
                    ScriptDelayItems = value;
                    break;

                case "scriptdelaysteps":
                    ScriptDelaySteps = int.Parse(value);
                    break;

                case "scriptdelaydisplaysteps":
                    ScriptDelayDisplaySteps = bool.Parse(value);
                    break;

                case "lastsaveplace":
                    LastSavePlace = value;
                    break;

                case "lastsaveplaceposition":
                    LastSavePlacePosition = value;
                    break;

                case "difficulty":
                    DifficultyMode = int.Parse(value);
                    break;

                case "battlestyle":
                    BattleStyle = int.Parse(value);
                    break;

                case "savecreated":
                    SaveCreated = value;
                    break;

                case "autosave":
                    if (IsGameJoltSave == false)
                    {
                        NewFilePrefix = value;
                        AutosaveUsed = true;
                    }
                    break;

                case "daycaresteps":
                    DaycareSteps = int.Parse(value);
                    break;

                case "gamemode":
                    GameMode = value;
                    break;

                case "pokefiles":
                    if (String.IsNullOrEmpty(value) == false)
                    {
                        foreach (String pf in value.ToLower().Split(','))
                        {
                            PokeFiles.Add(pf);
                        }
                    }
                    break;

                case "visitedmaps":
                    VisitedMaps = value;
                    break;

                case "tempsurfskin":
                    TempSurfSkin = value;
                    break;

                case "surfing":
                    StartSurfing = bool.Parse(value);
                    break;

                case "bp":
                    BP = int.Parse(value);
                    break;

                case "coins":
                    Coins = int.Parse(value);
                    break;

                case "gtsstars":
                    GTSStars = int.Parse(value);
                    break;

                case "showmodels":
                    ShowModelsInBattle = bool.Parse(value);
                    break;

                case "sandboxmode":
                    SandBoxMode = bool.Parse(value);
                    break;

                case "earnedachievements":
                    if (String.IsNullOrEmpty(value) == false)
                    {
                        EarnedAchievements = [.. value.ToLower().Split(',')];
                    }
                    break;

                case "expall":
                    EnableExpAll = bool.Parse(value);
                    break;

                default:
                    break;
            }
        }

        GameStart = DateTime.Now;
    }

    private void LoadOptions()
    {
        String[] data = IsGameJoltSave == true
            ? Core.GameJoltSave.Options.SplitAtNewline()
            : File.ReadAllLines(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Options.dat"));

        foreach (String line in data)
        {
            if (line.Contains('|') == false)
            {
                continue;
            }
            String id = line[..line.IndexOf('|')];
            String value = line[(line.IndexOf('|') + 1)..];
            switch (id.ToLower())
            {
                case "fov":
                    StartFOV = float.Parse(value.Replace(".", GameController.DecSeparator))
                                    .Clamp(FOV_RANGE.Min, FOV_RANGE.Max);
                    break;

                case "textspeed":
                    TextBox.TextSpeed = int.Parse(value);
                    break;

                case "mousespeed":
                    StartRotationSpeed = int.Parse(value);
                    break;

                default:
                    break;
            }
        }
    }

    private void LoadItems()
    {
        Inventory.Clear();
        Mails.Clear();
        String data = IsGameJoltSave == true
            ? Core.GameJoltSave.Items
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Items.dat"));
        if (String.IsNullOrEmpty(data) == true)
        {
            return;
        }
        foreach (String itemDat in data.SplitAtNewline())
        {
            if (String.IsNullOrEmpty(itemDat) == true)
            {
                continue;
            }
            if (itemDat.StartsWith("{") == true && itemDat.EndsWith("}") == true && itemDat.Contains('|') == true)
            {
                String inner = itemDat[1..^1];
                String itemID = inner[..inner.IndexOf('|')];
                int amount = int.Parse(inner[(inner.IndexOf('|') + 1)..]);
                Inventory.AddItem(itemID, amount);
            }
            else if (itemDat.StartsWith("Mail|") == true)
            {
                Mails.Add(Items.MailItem.GetMailDataFromString(itemDat[5..]));
            }
        }
    }

    private void LoadBerries()
    {
        BerryData = IsGameJoltSave == true
            ? Core.GameJoltSave.Berries
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Berries.dat"));
    }

    private void LoadApricorns()
    {
        ApricornData = IsGameJoltSave == true
            ? Core.GameJoltSave.Apricorns
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Apricorns.dat"));
    }

    private void LoadDaycare()
    {
        DaycareData = String.Empty;
        if (IsGameJoltSave == true)
        {
            DaycareData = Core.GameJoltSave.Daycare;
        }
        else
        {
            String path = Path.Combine(AppPaths.SaveDir, FilePrefix, "Daycare.dat");
            if (File.Exists(path) == true)
            {
                DaycareData = File.ReadAllText(path);
            }
        }
    }

    private void LoadPokedex()
    {
        PokedexData = IsGameJoltSave == true
            ? Core.GameJoltSave.Pokedex
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Pokedex.dat"));
        if (String.IsNullOrEmpty(PokedexData) == true)
        {
            PokedexData = Pokedex.NewPokedex();
        }
    }

    private void LoadRegister()
    {
        RegisterData = IsGameJoltSave == true
            ? Core.GameJoltSave.Register
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Register.dat"));
    }

    private void LoadItemData()
    {
        ItemData = IsGameJoltSave == true
            ? Core.GameJoltSave.ItemData
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "ItemData.dat"));
    }

    private void LoadBoxData()
    {
        BoxData = IsGameJoltSave == true
            ? Core.GameJoltSave.Box
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Box.dat"));
    }

    private void LoadNPCData()
    {
        NPCData = IsGameJoltSave == true
            ? Core.GameJoltSave.NPC
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "NPC.dat"));
    }

    private void LoadHallOfFameData()
    {
        HallOfFameData = IsGameJoltSave == true
            ? Core.GameJoltSave.HallOfFame
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "HallOfFame.dat"));
    }

    private void LoadSecretBaseData()
    {
        SecretBaseData = IsGameJoltSave == true
            ? Core.GameJoltSave.SecretBase
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "SecretBase.dat"));
    }

    private void LoadRoamingPokemonData()
    {
        RoamingPokemonData = IsGameJoltSave == true
            ? Core.GameJoltSave.RoamingPokemon
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "RoamingPokemon.dat"));
    }

    private void LoadStatistics()
    {
        Statistics = IsGameJoltSave == true
            ? Core.GameJoltSave.Statistics
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Statistics.dat"));
    }

    private void LoadParty()
    {
        Pokemons.Clear();
        String data = IsGameJoltSave == true
            ? Core.GameJoltSave.Party
            : File.ReadAllText(
                Path.Combine(AppPaths.SaveDir, FilePrefix, "Party.dat"));
        foreach (String line in data.SplitAtNewline())
        {
            if (line.StartsWith("{") == false) continue;
            Pokemon p = Pokemon.GetPokemonByData(line);
            if (p != null)
            {
                Pokemons.Add(p);
            }
        }
    }

    public void SaveGame(bool isAutosave = false)
    {
        if (IsGameJoltSave == true) return;

        String saveDir = Path.Combine(AppPaths.SaveDir, FilePrefix);
        Directory.CreateDirectory(saveDir);

        File.WriteAllText(Path.Combine(saveDir, "Player.dat"), GetPlayerData(false));
        File.WriteAllText(Path.Combine(saveDir, "Options.dat"), GetOptionsData());

        System.Text.StringBuilder partySb = new();
        foreach (Pokemon p in Pokemons)
            partySb.AppendLine(p.GetSaveData());
        File.WriteAllText(Path.Combine(saveDir, "Party.dat"), partySb.ToString());

        System.Text.StringBuilder itemsSb = new();
        foreach (PlayerInventory.ItemContainer slot in Inventory)
            itemsSb.AppendLine("{" + slot.ItemID + "|" + slot.Amount + "}");
        foreach (Items.MailItem.MailData mail in Mails)
            itemsSb.AppendLine("Mail|" + Items.MailItem.GetStringFromMail(mail));
        File.WriteAllText(Path.Combine(saveDir, "Items.dat"), itemsSb.ToString());

        File.WriteAllText(Path.Combine(saveDir, "Berries.dat"), BerryData);
        File.WriteAllText(Path.Combine(saveDir, "Apricorns.dat"), ApricornData);
        File.WriteAllText(Path.Combine(saveDir, "Box.dat"), BoxData);
        File.WriteAllText(Path.Combine(saveDir, "Daycare.dat"), DaycareData);
        File.WriteAllText(Path.Combine(saveDir, "HallOfFame.dat"), HallOfFameData);
        File.WriteAllText(Path.Combine(saveDir, "ItemData.dat"), ItemData);
        File.WriteAllText(Path.Combine(saveDir, "NPC.dat"), NPCData);
        File.WriteAllText(Path.Combine(saveDir, "Pokedex.dat"), PokedexData);
        File.WriteAllText(Path.Combine(saveDir, "Register.dat"), RegisterData);
        File.WriteAllText(Path.Combine(saveDir, "RoamingPokemon.dat"), RoamingPokemonData);
        File.WriteAllText(Path.Combine(saveDir, "SecretBase.dat"), SecretBaseData);
    }

    public String GetPlayerData(bool online)
    {
        System.Text.StringBuilder sb = new();
        sb.AppendLine("Name|" + Name);
        sb.AppendLine("Position|" + StartPosition.X.ToString("G", System.Globalization.CultureInfo.InvariantCulture) + "," +
                                    StartPosition.Y.ToString("G", System.Globalization.CultureInfo.InvariantCulture) + "," +
                                    StartPosition.Z.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
        sb.AppendLine("LastPokemonPosition|" + LastPokemonPosition.X.ToString("G", System.Globalization.CultureInfo.InvariantCulture) + "," +
                                               LastPokemonPosition.Y.ToString("G", System.Globalization.CultureInfo.InvariantCulture) + "," +
                                               LastPokemonPosition.Z.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
        sb.AppendLine("MapFile|" + StartMap);
        sb.AppendLine("RivalName|" + RivalName);
        sb.AppendLine("RivalSkin|" + RivalSkin);
        sb.AppendLine("Money|" + Money);
        sb.AppendLine("Badges|" + String.Join(",", Badges));
        sb.AppendLine("Rotation|" + StartRotation.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
        sb.AppendLine("Gender|" + Gender);
        TimeSpan elapsed = PlayTime + (DateTime.Now - GameStart);
        sb.AppendLine("Playtime|" + elapsed.Days + "," + elapsed.Hours + "," + elapsed.Minutes + "," + elapsed.Seconds);
        sb.AppendLine("OT|" + OT);
        sb.AppendLine("Points|" + Points);
        sb.AppendLine("HasPokedex|" + HasPokedex);
        sb.AppendLine("HasPokegear|" + HasPokegear);
        sb.AppendLine("FOV|" + StartFOV.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
        sb.AppendLine("FreeCamera|" + StartFreeCameraMode);
        sb.AppendLine("ThirdPerson|" + StartThirdPerson);
        sb.AppendLine("Skin|" + Skin);
        sb.AppendLine("BattleAnimations|" + ShowBattleAnimations);
        sb.AppendLine("RunMode|" + RunMode);
        sb.AppendLine("RunToggled|" + RunToggled);
        sb.AppendLine("BoxAmount|" + BoxAmount);
        sb.AppendLine("LastRestPlace|" + LastRestPlace);
        sb.AppendLine("LastRestPlacePosition|" + LastRestPlacePosition);
        sb.AppendLine("RepelSteps|" + RepelSteps);
        if (String.IsNullOrEmpty(ScriptDelayItems) == false)
        {
            sb.AppendLine("ScriptDelayItems|" + ScriptDelayItems);
        }
        sb.AppendLine("ScriptDelaySteps|" + ScriptDelaySteps);
        sb.AppendLine("ScriptDelayDisplaySteps|" + ScriptDelayDisplaySteps);
        sb.AppendLine("LastSavePlace|" + LastSavePlace);
        sb.AppendLine("LastSavePlacePosition|" + LastSavePlacePosition);
        sb.AppendLine("Difficulty|" + DifficultyMode);
        sb.AppendLine("BattleStyle|" + BattleStyle);
        sb.AppendLine("SaveCreated|" + SaveCreated);
        if (String.IsNullOrEmpty(NewFilePrefix) == false)
        {
            sb.AppendLine("Autosave|" + NewFilePrefix);
        }
        sb.AppendLine("DaycareSteps|" + DaycareSteps);
        sb.AppendLine("GameMode|" + GameMode);
        if (PokeFiles.Count > 0)
        {
            sb.AppendLine("PokeFiles|" + String.Join(",", PokeFiles));
        }
        sb.AppendLine("VisitedMaps|" + VisitedMaps);
        sb.AppendLine("TempSurfSkin|" + TempSurfSkin);
        sb.AppendLine("Surfing|" + StartSurfing);
        sb.AppendLine("BP|" + BP);
        sb.AppendLine("Coins|" + Coins);
        sb.AppendLine("GTSStars|" + GTSStars);
        sb.AppendLine("ShowModels|" + ShowModelsInBattle);
        sb.AppendLine("SandboxMode|" + SandBoxMode);
        if (EarnedAchievements.Count > 0)
        {
            sb.AppendLine("EarnedAchievements|" + String.Join(",", EarnedAchievements));
        }
        sb.AppendLine("ExpAll|" + EnableExpAll);
        return sb.ToString();
    }

    public String GetOptionsData()
    {
        System.Text.StringBuilder sb = new();
        sb.AppendLine("FOV|" + StartFOV.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
        sb.AppendLine("TextSpeed|" + TextBox.TextSpeed);
        sb.AppendLine("MouseSpeed|" + StartRotationSpeed);
        return sb.ToString();
    }

    // Utility methods

    public static bool IsSaveGameFolder(String folder)
    {
        if (Directory.Exists(folder) == false)
        {
            return false;
        }
        foreach (String file in SAVE_FILE_NAMES)
        {
            if (File.Exists(Path.Combine(folder, file + ".dat")) == false)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsRunning()
    {
        if (RunMode == true)
        {
            return RunToggled;
        }
        if (KeyBoardHandler.KeyDown(Keys.LeftShift) == true ||
            ControllerHandler.ButtonDown(Buttons.B) == true)
        {
            if (Screen.Level?.Riding == false &&
                Screen.Level?.Surfing == false &&
                Inventory.HasRunningShoes == true)
            {
                return true;
            }
        }
        return false;
    }

    public void TakeStep(int steps) { }

    public void AddVisitedMap(String mapFile)
    {
        List<String> maps = VisitedMaps.Split(',').ToList();
        if (maps.Contains(mapFile) == false)
        {
            maps.Add(mapFile);
            VisitedMaps = String.Join(",", maps);
        }
    }

    public Pokemon? GetWalkPokemon()
    {
        if (Pokemons.Count == 0) return null;
        for (int i = 0; i < Pokemons.Count; i++)
        {
            if (Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Pokemons[i].IsEgg == false)
            {
                return Pokemons[i];
            }
        }
        return null;
    }

    // VB accessed this as a property (no parens); expose as property for C# compatibility
    public int CountFightablePokemon
    {
        get
        {
            int count = 0;
            foreach (Pokemon p in Pokemons)
            {
                if (p.HP > 0 && p.Status != Pokemon.StatusProblems.Fainted && p.IsEgg == false)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public bool CanCatchPokemon()
    {
        if (Pokemons.Count < 6)
            return true;
        foreach (StorageSystemScreen.Box box in StorageSystemScreen.LoadBoxes())
        {
            if (box.pokemon.Count < 30)
                return true;
        }
        return false;
    }

    public void ResetNewLevel()
    {
        _lastLevel = 0;
        _displayEmblemDelay = 0f;
        _emblemPositionX = Core.windowSize.Width;
    }

    public void DrawLevelUp()
    {
        // TODO Phase 9: wire up full GameJolt emblem draw
        if (IsGameJoltSave == false)
        {
            return;
        }

        if (_displayEmblemDelay > 0f)
        {
            _displayEmblemDelay -= 0.1f;
            if (_displayEmblemDelay <= EMBLEM_SLIDE_THRESHOLD)
            {
                if (_emblemPositionX < Core.windowSize.Width)
                {
                    _emblemPositionX += (int)EMBLEM_SLIDE_SPEED;
                }
            }
            else
            {
                if (_emblemPositionX > Core.windowSize.Width - EMBLEM_TARGET_OFFSET)
                {
                    _emblemPositionX -= (int)EMBLEM_SLIDE_SPEED;
                }
            }

            if (_displayEmblemDelay <= 0f)
            {
                _displayEmblemDelay = 0f;
                _emblemPositionX = Core.windowSize.Width;
            }
        }
    }

    public void Unload()
    {
        Pokemons.Clear();
        Pokedexes.Clear();
        Inventory.Clear();
        Badges.Clear();
        PokeFiles.Clear();
        EarnedAchievements.Clear();
        PokegearModules.Clear();
        PhoneContacts.Clear();
        Mails.Clear();
        Trophies.Clear();

        Name = DEFAULT_NAME;
        RivalName = String.Empty;
        RivalSkin = String.Empty;
        Money = 0;
        PlayTime = TimeSpan.Zero;
        GameStart = DateTime.Now;
        OT = DEFAULT_OT;
        Points = 0;
        BP = 0;
        Coins = 0;
        HasPokedex = false;
        HasPokegear = false;
        ShowBattleAnimations = 1;
        BoxAmount = DEFAULT_BOX_AMOUNT;
        LastRestPlace = DEFAULT_REST_PLACE;
        LastRestPlacePosition = DEFAULT_REST_POSITION;
        LastSavePlace = DEFAULT_REST_PLACE;
        LastSavePlacePosition = DEFAULT_REST_POSITION;
        DiagonalMovement = false;
        RepelSteps = 0;
        DifficultyMode = 0;
        BattleStyle = 0;
        ShowModelsInBattle = true;
        SaveCreated = "Pre 0.21";
        LastPokemonPosition = new Vector3(999, 999, 999);
        DaycareSteps = 0;
        GameMode = DEFAULT_GAMEMODE;
        VisitedMaps = String.Empty;
        TempSurfSkin = DEFAULT_SKIN;
        TempRideSkin = String.Empty;
        GTSStars = DEFAULT_GTS_STARS;
        SandBoxMode = false;
        Statistics = String.Empty;
        StartPosition = new Vector3(14, 0.1f, 10);
        StartRotation = 0;
        StartFreeCameraMode = false;
        StartMap = DEFAULT_START_MAP;
        StartFOV = DEFAULT_START_FOV;
        StartRotationSpeed = DEFAULT_ROTATION_SPEED;
        StartThirdPerson = false;
        StartSurfing = false;
        StartRiding = false;
        Skin = DEFAULT_SKIN;

        RegisterData = String.Empty;
        BerryData = String.Empty;
        PokedexData = String.Empty;
        ItemData = String.Empty;
        BoxData = String.Empty;
        NPCData = String.Empty;
        ApricornData = String.Empty;
        SecretBaseData = String.Empty;
        DaycareData = String.Empty;
        HallOfFameData = String.Empty;
        RoamingPokemonData = String.Empty;
        UsedItemsToCheckScriptDelayFor.Clear();

        FilePrefix = "nilllzz";
        NewFilePrefix = String.Empty;
        AutosaveUsed = false;
        loadedSave = false;
        IsGameJoltSave = false;
        EmblemBackground = DEFAULT_EMBLEM_BG;

        ResetNewLevel();
    }

    // Step events (TODO Phase 12)

    public void HealParty()
    {
        foreach (Pokemon p in Pokemons)
        {
            p.HP = p.MaxHP;
            p.Status = Pokemon.StatusProblems.None;
            foreach (BattleSystem.Attack atk in p.Attacks)
            {
                atk.CurrentPP = atk.MaxPP;
            }
        }
    }

    public void HealParty(int[] members)
    {
        foreach (int idx in members)
        {
            if (idx >= 0 && idx < Pokemons.Count)
            {
                Pokemon p = Pokemons[idx];
                p.HP = p.MaxHP;
                p.Status = Pokemon.StatusProblems.None;
                foreach (BattleSystem.Attack atk in p.Attacks)
                {
                    atk.CurrentPP = atk.MaxPP;
                }
            }
        }
    }

    public void StepEvent(int stepAmount)
    {
        // TODO Phase 12: implement full step event system
    }

    public void CheckItemCountScriptDelay(String itemID)
    {
        // TODO Phase 12: implement
    }

    public void AddPoints(int amount, String reason)
    {
        // TODO Phase 9: add mystery event multiplier and full GameJolt handling
        Points += amount;
    }
}
