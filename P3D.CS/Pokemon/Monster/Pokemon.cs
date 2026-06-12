using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using P3D.Items;

namespace P3D;

public partial class Pokemon
{
    // Constants

    private static readonly (int Min, int Max) IV_RANGE = (0, 31);
    private static readonly (int Min, int Max) EV_RANGE = (0, 255);
    private const int EV_TOTAL_MAX = 510;
    private const int EV_SINGLE_MAX = 252;
    private static readonly (int Min, int Max) FRIENDSHIP_RANGE = (0, 255);
    private const int MAX_PARTY_MOVES = 4;
    private const int TEXTURE_CACHE_SIZE = 11;
    private const int SHEDINJA_NUMBER = 292;
    private const int ID_VALUE_LENGTH = 11;
    private const int LUXURY_BALL_ID = 174;
    private const float PITCH_LOW_HP = -0.4f;
    private const float PITCH_VERY_LOW_HP = -0.8f;
    private const float PITCH_FAINTED = -1.0f;
    private const int HP_LOW_THRESHOLD = 50;
    private const int HP_VERY_LOW_THRESHOLD = 15;

    public static readonly int[] LEGENDARIES =
    [
        144, 145, 146, 150, 151, 243, 244, 245, 249, 250, 251,
        377, 378, 379, 380, 381, 382, 383, 384, 385, 386,
        480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490,
        491, 492, 493, 494, 638, 639, 640, 641, 642, 643, 644,
        645, 646, 647, 648, 649, 716, 717, 718, 719, 720, 721,
        772, 773, 785, 786, 787, 788, 789, 790, 791, 792, 800,
        801, 802, 807, 808, 809, 888, 889, 890, 891, 892, 893,
        894, 895, 896, 897, 898, 905, 1001, 1002, 1003, 1004,
        1007, 1008, 1014, 1015, 1016, 1017, 1024, 1025
    ];

    public static readonly List<int> Legendaries = [144, 145, 146, 150, 151, 243, 244, 245, 249, 250, 251, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649];

    public static int MasterShinyRate
    {
        get
        {
            int shinyRate = 4096;
            // TODO Phase 3: mystery event multiplier and ShinyCharm logic
            return shinyRate;
        }
    }

    public static int GetMasterShinyRate(bool adjusted = true) => MasterShinyRate;

    // Events

    public event EventHandler? TexturesCleared;

    // Enums

    public enum ExperienceTypes { Fast, MediumFast, MediumSlow, Slow }

    public enum EggGroups
    {
        Monster, Water1, Water2, Water3, Bug, Flying, Field, Fairy,
        Grass, Undiscovered, HumanLike, Mineral, Amorphous, Ditto,
        Dragon, GenderUnknown, None
    }

    public enum Genders { Male, Female, Genderless }

    public enum StatusProblems { None, Burn, Freeze, Paralyzed, Poison, BadPoison, Sleep, Fainted }

    public enum VolatileStatus { Confusion, Flinch, Infatuation, Trapped }

    public enum Natures
    {
        Hardy, Lonely, Brave, Adamant, Naughty, Bold, Docile, Relaxed,
        Impish, Lax, Timid, Hasty, Serious, Jolly, Naive, Modest,
        Mild, Quiet, Bashful, Rash, Calm, Gentle, Sassy, Careful, Quirky
    }

    public enum FriendShipCauses
    {
        Walking, LevelUp, Fainting, EnergyPowder, HealPowder,
        EnergyRoot, RevivalHerb, Trading, Vitamin, EVBerry
    }

    // Properties (definition data)

    public List<int> abilityTag = [];

    public String AnimationName => PokemonForms.GetAnimationName(this);

    private String _additionalData = String.Empty;
    public String AdditionalData
    {
        get => _additionalData;
        set
        {
            if (_additionalData != value)
            {
                _additionalData = value;
                ReloadDefinitions();
                ClearTextures();
            }
        }
    }

    public int Number { get; set; }
    public ExperienceTypes ExperienceType { get; set; }
    public int BaseExperience { get; set; }
    public String Name { get; set; } = String.Empty;
    public int CatchRate { get; set; }
    public int BaseFriendship { get; set; }
    public int BaseEggSteps { get; set; }
    public EggGroups EggGroup1 { get; set; }
    public EggGroups EggGroup2 { get; set; }
    public decimal IsMale { get; set; }
    public bool IsGenderless { get; set; }
    public String Devolution { get; set; } = "0";
    public bool CanLearnAllMachines { get; set; }
    public bool CanSwim { get; set; }
    public bool CanFly { get; set; }
    public String EggPokemon { get; set; } = "0";
    public int TradeValue { get; set; } = 10;
    public bool CanBreed { get; set; } = true;

    // Type properties

    public Element Type1
    {
        get
        {
            // TODO Phase 3: item type override via PokemonForms.GetTypeAdditionFromItem
            return field;
        }
        set;
    } = new Element(Element.Types.Normal);

    public Element Type2 { get; set; } = new Element(Element.Types.Blank);

    // Definition collections

    public Dictionary<Item, int> startItems = [];
    public Dictionary<int, List<BattleSystem.Attack>> attackLearns = [];
    public List<int> eggMoves = [];
    public List<BattleSystem.Attack> tutorAttacks = [];
    public List<EvolutionCondition> evolutionConditions = [];
    public List<Ability> newAbilities = [];
    public Ability? hiddenAbility;
    public List<int> machines = [];
    public PokedexEntry? pokedexEntry;
    public SoundEffect? cry;
    public Dictionary<int, String> wildItems = [];
    public String regionalForms = String.Empty;
    public List<String> dexForms = [];
    public List<String> evolutionLines = [];

    // Saved stats

    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SpAttack { get; set; }
    public int SpDefense { get; set; }
    public int Speed { get; set; }

    // EV stats

    public int EVHP { get; set { field = value; CalculateStats(); } }
    public int EVAttack { get; set { field = value; CalculateStats(); } }
    public int EVDefense { get; set { field = value; CalculateStats(); } }
    public int EVSpAttack { get; set { field = value; CalculateStats(); } }
    public int EVSpDefense { get; set { field = value; CalculateStats(); } }
    public int EVSpeed { get; set { field = value; CalculateStats(); } }

    // GiveEV stats

    public int GiveEVHP { get; set; }
    public int GiveEVAttack { get; set; }
    public int GiveEVDefense { get; set; }
    public int GiveEVSpAttack { get; set; }
    public int GiveEVSpDefense { get; set; }
    public int GiveEVSpeed { get; set; }

    // IV stats

    public int IVHP { get; set; }
    public int IVAttack { get; set; }
    public int IVDefense { get; set; }
    public int IVSpAttack { get; set; }
    public int IVSpDefense { get; set; }
    public int IVSpeed { get; set; }

    // Base stats

    public int baseHP;
    public int baseAttack;
    public int baseDefense;
    public int baseSpAttack;
    public int baseSpDefense;
    public int baseSpeed;

    // Save-instance fields

    public Item? Item
    {
        get;
        set { field = value; ClearTextures(); }
    }

    public List<BattleSystem.Attack> attacks = [];
    public String? abilitySlot;

    public Ability? Ability
    {
        get
        {
            if (abilitySlot != null && StringHelper.IsNumeric(abilitySlot) == false)
            {
                switch (abilitySlot.ToUpper())
                {
                    case "A":
                        return newAbilities.Count > 0 ? newAbilities[0] : field;

                    case "B":
                        return newAbilities.Count > 1 && newAbilities[1] != null
                            ? newAbilities[1]
                            : newAbilities.Count > 0 ? newAbilities[0] : field;

                    case "C":
                        return newAbilities.Count > 2 && newAbilities[2] != null
                            ? newAbilities[2]
                            : newAbilities.Count > 0 ? newAbilities[0] : field;

                    case "H":
                        return HasHiddenAbility == true && hiddenAbility != null
                            ? hiddenAbility
                            : newAbilities.Count > 0 ? newAbilities[0] : field;
                }
            }
            else if (abilitySlot != null && StringHelper.IsNumeric(abilitySlot) == true)
            {
                return P3D.Ability.GetAbilityByID(int.Parse(abilitySlot));
            }
            return field;
        }
        set;
    }

    public Item? catchBall = Item.GetItemByID("5");

    public int Experience { get; set; }
    public Genders Gender { get; set; }
    public int EggSteps { get; set; }
    public String NickName { get; set; } = String.Empty;
    public int Level { get; set; }
    public String OT { get; set; } = "00000";
    public StatusProblems Status { get; set; } = StatusProblems.None;
    public Natures Nature { get; set; }

    public String CatchLocation
    {
        get
        {
            if (field.StartsWith("<system.token(") == true &&
                field.EndsWith(")>") == true)
            {
                return Localization.GetString(field[14..^2]);
            }
            return field;
        }
        set;
    } = "an unknown place";

    public String CatchTrainerName
    {
        get
        {
            if (field.StartsWith("<system.token(") == true &&
                field.EndsWith(")>") == true)
            {
                return Localization.GetString(field[14..^2]);
            }
            return field;
        }
        set;
    } = "???";

    public String CatchMethod
    {
        get
        {
            if (field.StartsWith("<system.token(") == true &&
                field.EndsWith(")>") == true)
            {
                return Localization.GetString(field[14..^2]);
            }
            return field;
        }
        set;
    } = "Somehow obtained at";

    public int Friendship { get; set; }
    public bool IsShiny { get; set; }
    public String IndividualValue { get; set; } = String.Empty;

    // Temp battle state

    private readonly List<VolatileStatus> _volatiles = [];

    public bool HasVolatileStatus(VolatileStatus vs) => _volatiles.Contains(vs);
    public void AddVolatileStatus(VolatileStatus vs) { if (_volatiles.Contains(vs) == false) _volatiles.Add(vs); }
    public void RemoveVolatileStatus(VolatileStatus vs) { _volatiles.Remove(vs); }
    public void ClearAllVolatiles() => _volatiles.Clear();

    public int statAttack;
    public int statDefense;
    public int statSpAttack;
    public int statSpDefense;
    public int statSpeed;
    public int accuracy;
    public int evasion;
    public bool hasLeveledUp;
    public int sleepTurns = -1;
    public int confusionTurns = -1;
    public BattleSystem.Attack? lastHitByMove;
    public int lastDamageReceived;
    public int lastHitPhysical = -1;

    public void ResetTemp()
    {
        _volatiles.Clear();
        foreach (BattleSystem.Attack attack in attacks)
        {
            if (attack.disabled > 0)
            {
                attack.disabled = 0;
            }
        }
        statAttack = 0;
        statDefense = 0;
        statSpAttack = 0;
        statSpDefense = 0;
        statSpeed = 0;
        accuracy = 0;
        evasion = 0;
        if (_originalNumber > -1)
        {
            Number = _originalNumber;
            _originalNumber = -1;
        }
        if (OriginalStats[0] > -1) { Attack = OriginalStats[0]; OriginalStats[0] = -1; }
        if (OriginalStats[1] > -1) { Defense = OriginalStats[1]; OriginalStats[1] = -1; }
        if (OriginalStats[2] > -1) { SpAttack = OriginalStats[2]; OriginalStats[2] = -1; }
        if (OriginalStats[3] > -1) { SpDefense = OriginalStats[3]; OriginalStats[3] = -1; }
        if (OriginalStats[4] > -1) { Speed = OriginalStats[4]; OriginalStats[4] = -1; }
        if (_originalShiny == 0) { IsShiny = false; _originalShiny = -1; }
        else if (_originalShiny == 1) { IsShiny = true; _originalShiny = -1; }
        if (_originalMoves != null)
        {
            attacks.Clear();
            attacks.AddRange(_originalMoves);
            _originalMoves = null;
        }
        if (OriginalAbility != null) Ability = OriginalAbility;
        if (OriginalAbilitySlot != null) abilitySlot = OriginalAbilitySlot;
        IsTransformed = false;
        CalculateStats();
    }

    public Ability? normalAbility = new Abilities.Stench();
    public String normalAbilitySlot = "A";

    public void LoadAltAbility()
    {
        normalAbility = OriginalAbility;
        normalAbilitySlot = OriginalAbilitySlot ?? "A";
        if (newAbilities.Count > 0)
        {
            Ability = newAbilities[0];
        }
        abilitySlot = "A";
        SetOriginalAbility();
    }

    public void RestoreAbility()
    {
        Ability = normalAbility;
        abilitySlot = normalAbilitySlot;
        SetOriginalAbility();
    }

    // Original stats (for Transform)

    private int _originalNumber = -1;
    private int _originalShiny = -1;
    private List<BattleSystem.Attack>? _originalMoves;

    public Element? OriginalType1 { get; set; }
    public Element? OriginalType2 { get; set; }

    public int OriginalNumber
    {
        get => _originalNumber;
        set { if (_originalNumber == -1) _originalNumber = value; }
    }

    public int OriginalShiny
    {
        get => _originalShiny;
        set { if (_originalShiny == -1) _originalShiny = value; }
    }

    public int[] OriginalStats { get; set; } = [ -1, -1, -1, -1, -1 ];

    public Ability? OriginalAbility { get; private set; }
    public String? OriginalAbilitySlot { get; private set; }

    public void SetOriginalAbility()
    {
        OriginalAbility = Ability;
        OriginalAbilitySlot = abilitySlot;
    }

    public Item? OriginalItem { get; set; }

    public List<BattleSystem.Attack>? OriginalMoves
    {
        get => _originalMoves;
        set { if (_originalMoves == null) _originalMoves = value; }
    }

    public bool IsTransformed { get; set; }

    // Texture cache

    private readonly List<Texture2D?> _textures = [];

    public void ClearTextures()
    {
        _textures.Clear();
        for (int i = 0; i < TEXTURE_CACHE_SIZE; i++)
        {
            _textures.Add(null);
        }
        TexturesCleared?.Invoke(this, EventArgs.Empty);
    }

    // Constructors

    private Pokemon()
    {
        ClearTextures();
    }

    public static Pokemon GetPokemonByID(int number)
    {
        return GetPokemonByID(number, "");
    }

    public static Pokemon GetPokemonByID(int number, String additionalData,
                                          bool preventFormGeneration = false)
    {
        Pokemon p = new Pokemon();
        p.LoadDefinitions(number, additionalData);
        if (preventFormGeneration == false)
        {
            p.AdditionalData = additionalData;
        }
        else
        {
            p._additionalData = additionalData;
        }
        return p;
    }

    public static bool PokemonDataExists(String dataID)
    {
        return File.Exists(GameModeManager.GetPokemonDataFilePath(dataID + ".dat"));
    }

    public static Pokemon GetPokemonByData(String inputData)
    {
        Dictionary<String, String> tags = ParseTags(inputData);

        String additionalData = String.Empty;
        if (tags.ContainsKey("AdditionalData") == true)
        {
            additionalData = ScriptVersion2.ScriptCommander.Parse(tags["AdditionalData"]).ToString() ?? "";
        }

        int pokemonID = 10;
        if (tags.ContainsKey("Pokemon") == true)
        {
            pokemonID = ScriptConversion.ToInteger(
                ScriptVersion2.ScriptCommander.Parse(tags["Pokemon"]));
        }

        Pokemon p = GetPokemonByID(pokemonID, additionalData);
        p.LoadData(inputData);
        return p;
    }

    private static Dictionary<String, String> ParseTags(String data)
    {
        Dictionary<String, String> tags = [];
        String[] parts = data.Replace("§", ",").Replace("«", "[").Replace("»", "]")
                             .Split('}');
        foreach (String tag in parts)
        {
            if (tag.Contains('{') == false || tag.Contains('[') == false)
            {
                continue;
            }
            try
            {
                String tagName = tag[(tag.IndexOf('{') + 2)..];
                if (tagName.Contains('"'))
                {
                    tagName = tagName[..tagName.IndexOf('"')];
                }
                String tagContent = tag[(tag.IndexOf('[') + 1)..];
                if (tagContent.Contains(']'))
                {
                    tagContent = tagContent[..tagContent.IndexOf(']')];
                }
                if (tags.ContainsKey(tagName) == false)
                {
                    tags.Add(tagName, tagContent);
                }
            }
            catch { }
        }
        return tags;
    }

    public void ReloadDefinitions()
    {
        attackLearns.Clear();
        LoadDefinitions(Number, _additionalData);
        ClearTextures();
    }

    public void LoadDefinitions(int number, String additionalData)
    {
        String path = PokemonForms.GetPokemonDataFile(number, additionalData);
        if (String.IsNullOrEmpty(path) == true || File.Exists(path) == false)
        {
            return;
        }

        newAbilities.Clear();
        String[] lines = File.ReadAllLines(path);

        foreach (String line in lines)
        {
            String varName = line.GetSplit(0, "|");
            String value = line.GetSplit(1, "|");

            switch (varName.ToLower())
            {
                case "name": Name = value; break;
                case "number": Number = int.Parse(value); break;
                case "baseexperience": BaseExperience = int.Parse(value); break;
                case "experiencetype":
                    ExperienceType = int.Parse(value) switch
                    {
                        0 => ExperienceTypes.Fast,
                        1 => ExperienceTypes.MediumFast,
                        2 => ExperienceTypes.MediumSlow,
                        3 => ExperienceTypes.Slow,
                        _ => ExperienceTypes.MediumFast
                    };
                    break;
                case "type1": Type1 = BattleSystem.GameModeElementLoader.GetElementByName(value); break;
                case "type2": Type2 = BattleSystem.GameModeElementLoader.GetElementByName(value); break;
                case "catchrate": CatchRate = int.Parse(value); break;
                case "basefriendship": BaseFriendship = int.Parse(value); break;
                case "egggroup1": EggGroup1 = ConvertIDToEggGroup(value); break;
                case "egggroup2": EggGroup2 = ConvertIDToEggGroup(value); break;
                case "baseeggsteps": BaseEggSteps = int.Parse(value); break;
                case "ismale":
                    IsMale = decimal.Parse(value.Replace(".", ",").Replace(",", GameController.DecSeparator));
                    break;
                case "isgenderless": IsGenderless = bool.Parse(value); break;
                case "devolution": Devolution = value; break;
                // TODO Phase 3: remaining definition fields (HP, types, abilities, etc.)
                default: break;
            }
        }
    }

    public void LoadData(String inputData)
    {
        // TODO Phase 3: full LoadData implementation using ScriptVersion2/ScriptCommander
        Dictionary<String, String> tags = ParseTags(inputData);
        bool loadedHP = false;
        bool loadedAttacks = false;
        bool loadedIVs = false;
        bool loadedAbility = false;
        bool loadedGender = false;
        bool loadedNature = false;
        bool loadedFriendship = false;
        bool loadedShiny = false;
        bool loadedEXP = false;

        foreach (KeyValuePair<String, String> kv in tags)
        {
            String tagValue = kv.Value;
            switch (kv.Key.ToLower())
            {
                case "pokemon": Number = ScriptConversion.ToInteger(ScriptVersion2.ScriptCommander.Parse(tagValue)); break;
                case "experience": Experience = ScriptConversion.ToInteger(ScriptVersion2.ScriptCommander.Parse(tagValue)); loadedEXP = true; break;
                case "gender":
                {
                    int g = ScriptConversion.ToInteger(ScriptVersion2.ScriptCommander.Parse(tagValue));
                    Gender = g == 0 ? Genders.Male : g == 1 ? Genders.Female : Genders.Genderless;
                    loadedGender = true;
                    break;
                }
                case "eggsteps": EggSteps = ScriptConversion.ToInteger(ScriptVersion2.ScriptCommander.Parse(tagValue)); break;
                case "nickname": NickName = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? ""; break;
                case "level": Level = ScriptConversion.ToInteger(ScriptVersion2.ScriptCommander.Parse(tagValue)); break;
                case "ot": OT = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? "00000"; break;
                case "status":
                {
                    Status = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() switch
                    {
                        "BRN" => StatusProblems.Burn,
                        "PSN" => StatusProblems.Poison,
                        "PRZ" => StatusProblems.Paralyzed,
                        "SLP" => StatusProblems.Sleep,
                        "FNT" => StatusProblems.Fainted,
                        "FRZ" => StatusProblems.Freeze,
                        "BPSN" => StatusProblems.BadPoison,
                        _ => StatusProblems.None
                    };
                    break;
                }
                case "nature":
                    Nature = ConvertIDToNature(ScriptConversion.ToInteger(ScriptVersion2.ScriptCommander.Parse(tagValue)));
                    loadedNature = true;
                    break;
                case "catchlocation": CatchLocation = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? ""; break;
                case "catchtrainer": CatchTrainerName = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? ""; break;
                case "catchball": catchBall = Item.GetItemByID(ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? "5"); break;
                case "catchmethod": CatchMethod = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? ""; break;
                case "friendship": Friendship = ScriptConversion.ToInteger(ScriptVersion2.ScriptCommander.Parse(tagValue)); loadedFriendship = true; break;
                case "isshiny": IsShiny = bool.Parse(ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? "false"); loadedShiny = true; break;
                case "attack1":
                case "attack2":
                case "attack3":
                case "attack4":
                {
                    BattleSystem.Attack? attack = BattleSystem.Attack.ConvertStringToAttack(ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? "");
                    if (attack != null) attacks.Add(attack);
                    loadedAttacks = true;
                    break;
                }
                case "hp": case "stats":
                {
                    HP = ScriptConversion.ToInteger(ScriptVersion2.ScriptCommander.Parse(tagValue));
                    loadedHP = true;
                    break;
                }
                case "fps": case "evs":
                {
                    String[] evs = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString()!.Split(',');
                    if (evs.Length >= 6)
                    {
                        EVHP = int.Parse(evs[0]).Clamp(EV_RANGE.Min, EV_RANGE.Max);
                        EVAttack = int.Parse(evs[1]).Clamp(EV_RANGE.Min, EV_RANGE.Max);
                        EVDefense = int.Parse(evs[2]).Clamp(EV_RANGE.Min, EV_RANGE.Max);
                        EVSpAttack = int.Parse(evs[3]).Clamp(EV_RANGE.Min, EV_RANGE.Max);
                        EVSpDefense = int.Parse(evs[4]).Clamp(EV_RANGE.Min, EV_RANGE.Max);
                        EVSpeed = int.Parse(evs[5]).Clamp(EV_RANGE.Min, EV_RANGE.Max);
                    }
                    break;
                }
                case "dvs": case "ivs":
                {
                    String[] ivs = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString()!.Split(',');
                    if (ivs.Length >= 6)
                    {
                        IVHP = int.Parse(ivs[0]);
                        IVAttack = int.Parse(ivs[1]);
                        IVDefense = int.Parse(ivs[2]);
                        IVSpAttack = int.Parse(ivs[3]);
                        IVSpDefense = int.Parse(ivs[4]);
                        IVSpeed = int.Parse(ivs[5]);
                        loadedIVs = true;
                    }
                    break;
                }
                case "additionaldata": _additionalData = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? ""; break;
                case "idvalue": IndividualValue = ScriptVersion2.ScriptCommander.Parse(tagValue).ToString() ?? ""; break;
                default: break;
            }
        }

        if (String.IsNullOrEmpty(IndividualValue) == true)
        {
            GenerateIndividualValue();
        }

        CalculateStats();

        Pokemon reference = GetPokemonByID(Number, _additionalData);
        reference.Generate(Level, true);

        if (loadedEXP == false) Experience = reference.Experience;
        if (loadedAttacks == false) attacks = reference.attacks;
        if (loadedIVs == false)
        {
            IVHP = reference.IVHP; IVAttack = reference.IVAttack;
            IVDefense = reference.IVDefense; IVSpAttack = reference.IVSpAttack;
            IVSpDefense = reference.IVSpDefense; IVSpeed = reference.IVSpeed;
        }
        if (loadedAbility == false)
        {
            Ability = reference.Ability; abilitySlot = reference.abilitySlot;
            SetOriginalAbility(); normalAbility = Ability; normalAbilitySlot = abilitySlot ?? "A";
        }
        if (loadedGender == false) Gender = reference.Gender;
        if (loadedNature == false) Nature = reference.Nature;
        if (loadedFriendship == false) Friendship = reference.Friendship;
        if (loadedShiny == false) IsShiny = reference.IsShiny;
        HP = loadedHP == false ? MaxHP : HP.Clamp(0, MaxHP);
    }

    // Serialization

    public String GetHallOfFameData()
    {
        int saveGender = Gender == Genders.Female ? 1 : IsGenderless == true ? 2 : 0;
        String shiny = IsShiny == true ? "1" : "0";
        return $"{{\"Pokemon\"[{Number}]}}" +
               $"{{\"Gender\"[{saveGender}]}}" +
               $"{{\"NickName\"[{NickName}]}}" +
               $"{{\"Level\"[{Level}]}}" +
               $"{{\"OT\"[{OT}]}}" +
               $"{{\"CatchTrainer\"[{CatchTrainerName}]}}" +
               $"{{\"isShiny\"[{shiny}]}}" +
               $"{{\"AdditionalData\"[{_additionalData}]}}" +
               $"{{\"IDValue\"[{IndividualValue}]}}";
    }

    public String GetSaveData()
    {
        int saveGender = Gender == Genders.Female ? 1 : IsGenderless == true ? 2 : 0;
        String saveStatus = Status switch
        {
            StatusProblems.Burn => "BRN",
            StatusProblems.Poison => "PSN",
            StatusProblems.Paralyzed => "PRZ",
            StatusProblems.Sleep => "SLP",
            StatusProblems.Fainted => "FNT",
            StatusProblems.Freeze => "FRZ",
            StatusProblems.BadPoison => "BPSN",
            _ => ""
        };

        String a1 = attacks.Count > 0 && attacks[0] != null ? attacks[0].ToString() : "";
        String a2 = attacks.Count > 1 && attacks[1] != null ? attacks[1].ToString() : "";
        String a3 = attacks.Count > 2 && attacks[2] != null ? attacks[2].ToString() : "";
        String a4 = attacks.Count > 3 && attacks[3] != null ? attacks[3].ToString() : "";

        String itemID = "0";
        String itemData = String.Empty;
        if (Item != null)
        {
            itemID = Item.IsGameModeItem == true ? Item.gmID : Item.ID.ToString();
            itemData = Item.AdditionalData;
        }

        String evSave = $"{EVHP},{EVAttack},{EVDefense},{EVSpAttack},{EVSpDefense},{EVSpeed}";
        String ivSave = $"{IVHP},{IVAttack},{IVDefense},{IVSpAttack},{IVSpDefense},{IVSpeed}";
        String shiny = IsShiny == true ? "1" : "0";

        return $"{{\"Pokemon\"[{Number}]}}" +
               $"{{\"OriginalNumber\"[{OriginalNumber}]}}" +
               $"{{\"Experience\"[{Experience}]}}" +
               $"{{\"Gender\"[{saveGender}]}}" +
               $"{{\"EggSteps\"[{EggSteps}]}}" +
               $"{{\"Item\"[{itemID}]}}" +
               $"{{\"ItemData\"[{itemData}]}}" +
               $"{{\"NickName\"[{NickName}]}}" +
               $"{{\"Level\"[{Level}]}}" +
               $"{{\"OT\"[{OT}]}}" +
               $"{{\"Ability\"[{abilitySlot}]}}" +
               $"{{\"Status\"[{saveStatus}]}}" +
               $"{{\"Nature\"[{Nature}]}}" +
               $"{{\"CatchLocation\"[{CatchLocation}]}}" +
               $"{{\"CatchTrainer\"[{CatchTrainerName}]}}" +
               $"{{\"CatchBall\"[{catchBall?.ID ?? 5}]}}" +
               $"{{\"CatchMethod\"[{CatchMethod}]}}" +
               $"{{\"Friendship\"[{Friendship}]}}" +
               $"{{\"isShiny\"[{shiny}]}}" +
               $"{{\"Attack1\"[{a1}]}}" +
               $"{{\"Attack2\"[{a2}]}}" +
               $"{{\"Attack3\"[{a3}]}}" +
               $"{{\"Attack4\"[{a4}]}}" +
               $"{{\"HP\"[{HP}]}}" +
               $"{{\"EVs\"[{evSave}]}}" +
               $"{{\"IVs\"[{ivSave}]}}" +
               $"{{\"AdditionalData\"[{_additionalData}]}}" +
               $"{{\"IDValue\"[{IndividualValue}]}}";
    }

    public override String ToString() => GetSaveData();

    // Generation

    public void Generate(int newLevel, bool setParameters, String opAdData = "xXx")
    {
        Level = 0;

        if (setParameters == true)
        {
            GenerateIndividualValue();
            if (opAdData.Equals("xXx"))
            {
                _additionalData = PokemonForms.GetInitialAdditionalData(this);
            }
            else
            {
                _additionalData = opAdData;
            }

            Nature = (Natures)Core.Random.Next(0, 25);

            // Synchronize ability check
            if (Core.Player.Pokemons.Count > 0 &&
                "synchronize".Equals(Core.Player.Pokemons[0].Ability?.Name.ToLower()))
            {
                Nature = Core.Player.Pokemons[0].Nature;
            }

            // Assign ability
            if (newAbilities.Count > 0)
            {
                int abilityIndex = Core.Random.Next(0, 2);
                if (abilityIndex == 0 || newAbilities.Count == 1)
                {
                    Ability = P3D.Ability.GetAbilityByID(newAbilities[0].ID);
                    abilitySlot = "A";
                }
                else
                {
                    Ability = P3D.Ability.GetAbilityByID(
                        newAbilities.Count > 1 ? newAbilities[1].ID : newAbilities[0].ID);
                    abilitySlot = "B";
                }
                SetOriginalAbility();
            }

            if (Core.Random.Next(0, MasterShinyRate) == 0)
            {
                IsShiny = true;
            }

            if (IsGenderless == true)
            {
                Gender = Genders.Genderless;
            }
            else
            {
                Gender = Core.Random.Next(1, 101) > (int)IsMale
                    ? Genders.Female
                    : Genders.Male;
            }

            IVHP = Core.Random.Next(IV_RANGE.Min, IV_RANGE.Max + 1);
            IVAttack = Core.Random.Next(IV_RANGE.Min, IV_RANGE.Max + 1);
            IVDefense = Core.Random.Next(IV_RANGE.Min, IV_RANGE.Max + 1);
            IVSpAttack = Core.Random.Next(IV_RANGE.Min, IV_RANGE.Max + 1);
            IVSpDefense = Core.Random.Next(IV_RANGE.Min, IV_RANGE.Max + 1);
            IVSpeed = Core.Random.Next(IV_RANGE.Min, IV_RANGE.Max + 1);

            Friendship = BaseFriendship;
        }

        while (newLevel > Level)
        {
            LevelUp(false);
            Experience = NeedExperience(Level);
        }

        // Build moveset from learn list
        List<BattleSystem.Attack> canLearn = [];
        foreach (KeyValuePair<int, List<BattleSystem.Attack>> entry in attackLearns)
        {
            if (entry.Key <= Level)
            {
                foreach (BattleSystem.Attack attack in entry.Value)
                {
                    bool has = false;
                    foreach (BattleSystem.Attack m in attacks)
                    {
                        if (m.ID == attack.ID) { has = true; break; }
                    }
                    foreach (BattleSystem.Attack m in canLearn)
                    {
                        if (m.ID == attack.ID) { has = true; break; }
                    }
                    if (has == false) canLearn.Add(attack);
                }
            }
        }

        if (canLearn.Count > 0)
        {
            attacks.Clear();
            int startIndex = Math.Max(0, canLearn.Count - MAX_PARTY_MOVES);
            for (int i = startIndex; i < canLearn.Count; i++)
            {
                attacks.Add(canLearn[i]);
            }
        }

        HP = MaxHP;
    }

    // Converters

    public static EggGroups ConvertIDToEggGroup(String id)
    {
        return id.ToLower() switch
        {
            "monster" => EggGroups.Monster,
            "water1" => EggGroups.Water1,
            "water2" => EggGroups.Water2,
            "water3" => EggGroups.Water3,
            "bug" => EggGroups.Bug,
            "flying" => EggGroups.Flying,
            "field" => EggGroups.Field,
            "fairy" => EggGroups.Fairy,
            "grass" => EggGroups.Grass,
            "undiscovered" => EggGroups.Undiscovered,
            "humanlike" => EggGroups.HumanLike,
            "mineral" => EggGroups.Mineral,
            "amorphous" => EggGroups.Amorphous,
            "ditto" => EggGroups.Ditto,
            "dragon" => EggGroups.Dragon,
            "genderunknown" => EggGroups.GenderUnknown,
            _ => EggGroups.None
        };
    }

    public static Natures ConvertIDToNature(int id)
    {
        return id >= 0 && id <= 24 ? (Natures)id : Natures.Hardy;
    }

    private void GenerateIndividualValue()
    {
        const String chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        String s = String.Empty;
        for (int i = 0; i < ID_VALUE_LENGTH; i++)
        {
            s += chars[Core.Random.Next(0, chars.Length)];
        }
        IndividualValue = s;
    }

    // Display name

    public String GetDisplayName(bool getFormName = true)
    {
        if (EggSteps > 0)
        {
            return Localization.GetString("pokemon_name_Egg", "Egg");
        }
        if (String.IsNullOrEmpty(NickName) == false)
        {
            return NickName;
        }
        if (getFormName == true)
        {
            String formName = PokemonForms.GetFormName(this);
            if (String.IsNullOrEmpty(formName) == false)
            {
                return Localization.TokenExists("pokemon_name_" + formName) == true
                    ? Localization.GetString("pokemon_name_" + formName)
                    : formName;
            }
        }
        return Localization.TokenExists("pokemon_name_" + Name) == true
            ? Localization.GetString("pokemon_name_" + Name)
            : Name;
    }

    public String GetName(bool getFormName = true)
    {
        if (getFormName == true)
        {
            String formName = PokemonForms.GetFormName(this);
            if (String.IsNullOrEmpty(formName) == false)
            {
                return Localization.TokenExists("pokemon_name_" + formName) == true
                    ? Localization.GetString("pokemon_name_" + formName)
                    : formName;
            }
        }
        return Localization.TokenExists("pokemon_name_" + Name) == true
            ? Localization.GetString("pokemon_name_" + Name)
            : Name;
    }

    public String OriginalName
    {
        get
        {
            String formName = PokemonForms.GetFormName(this);
            return String.IsNullOrEmpty(formName) == false ? formName : Name;
        }
        set => Name = value;
    }

    // Stats

    public void GetExperience(int exp, bool learnRandomAttack)
    {
        Experience += exp;
        while (Experience >= NeedExperience(Level + 1))
        {
            LevelUp(learnRandomAttack);
        }
        int maxLevel = ScriptConversion.ToInteger(GameModeManager.GetGameRuleValue("MaxLevel", "100"));
        Level = Level.Clamp(1, maxLevel);
    }

    public void LevelUp(bool learnRandomAttack)
    {
        Level++;
        int prevMaxHP = MaxHP;
        CalculateStats();
        int diff = MaxHP - prevMaxHP;
        if (diff > 0)
        {
            Heal(diff);
        }
        if (learnRandomAttack == true)
        {
            LearnAttack(Level);
        }
    }

    public void CalculateStatsBarSpeed()
    {
        if (IsTransformed == false)
        {
            MaxHP = CalcStatus(Level, true, baseHP, EVHP, IVHP, "HP");
            Attack = CalcStatus(Level, false, baseAttack, EVAttack, IVAttack, "Attack");
            Defense = CalcStatus(Level, false, baseDefense, EVDefense, IVDefense, "Defense");
            SpAttack = CalcStatus(Level, false, baseSpAttack, EVSpAttack, IVSpAttack, "SpAttack");
            SpDefense = CalcStatus(Level, false, baseSpDefense, EVSpDefense, IVSpDefense, "SpDefense");
        }
    }

    public void CalculateStats()
    {
        CalculateStatsBarSpeed();
        if (IsTransformed == false)
        {
            Speed = CalcStatus(Level, false, baseSpeed, EVSpeed, IVSpeed, "Speed");
        }
    }

    private int CalcStatus(int calcLevel, bool doHP, int baseStat, int evStat, int ivStat, String statName)
    {
        if (doHP == true)
        {
            if (Number == SHEDINJA_NUMBER)
            {
                return 1;
            }
            return (int)Math.Floor(((ivStat + 2 * baseStat + evStat / 4.0 + 100) * calcLevel / 100.0) + 10);
        }
        return (int)Math.Floor((((ivStat + 2 * baseStat + evStat / 4.0) * calcLevel / 100.0) + 5) *
                               P3D.Nature.GetMultiplier(this.Nature, statName));
    }

    public void LearnAttack(int learnLevel)
    {
        if (attackLearns.ContainsKey(learnLevel) == false)
        {
            return;
        }
        List<BattleSystem.Attack> aList = attackLearns[learnLevel];
        BattleSystem.Attack a = aList.Count > 1
            ? aList[Core.Random.Next(0, aList.Count - 1)]
            : aList[0];

        foreach (BattleSystem.Attack la in attacks)
        {
            if (la.ID == a.ID) return;
        }

        attacks.Add(a);
        if (attacks.Count == MAX_PARTY_MOVES + 1)
        {
            attacks.RemoveAt(Core.Random.Next(0, MAX_PARTY_MOVES + 1));
        }
    }

    public int NeedExperience(int expLevel)
    {
        int n = expLevel;
        int i = ExperienceType switch
        {
            ExperienceTypes.Fast => (int)(4 * n * n * n / 5.0),
            ExperienceTypes.MediumFast => n * n * n,
            ExperienceTypes.MediumSlow => (int)(6 * n * n * n / 5.0 - 15 * n * n + 100 * n - 140),
            ExperienceTypes.Slow => (int)(5 * n * n * n / 4.0),
            _ => n * n * n
        };
        return Math.Max(0, i);
    }

    public int CountPP()
    {
        int total = 0;
        foreach (BattleSystem.Attack attack in attacks)
        {
            total += attack.currentPP;
        }
        return total;
    }

    public void FullRestore()
    {
        Status = StatusProblems.None;
        Heal(MaxHP);
        _volatiles.Clear();
        foreach (BattleSystem.Attack attack in attacks)
        {
            attack.currentPP = attack.maxPP;
        }
    }

    public void Heal(int healHP)
    {
        HP = (HP + healHP).Clamp(0, MaxHP);
    }

    public void ChangeFriendShip(FriendShipCauses cause)
    {
        int add = 0;
        switch (cause)
        {
            case FriendShipCauses.Walking:
                add = 1;
                break;

            case FriendShipCauses.LevelUp:
                add = Friendship <= 99 ? 5 : Friendship <= 199 ? 3 : 2;
                break;

            case FriendShipCauses.Fainting:
                Friendship--;
                return;

            case FriendShipCauses.EnergyPowder:
            case FriendShipCauses.HealPowder:
                add = Friendship <= 99 ? -5 : Friendship <= 199 ? -5 : -10;
                break;

            case FriendShipCauses.EnergyRoot:
                add = Friendship <= 99 ? -10 : Friendship <= 199 ? -10 : -15;
                break;

            case FriendShipCauses.RevivalHerb:
                add = Friendship <= 99 ? -15 : Friendship <= 199 ? -15 : -20;
                break;

            case FriendShipCauses.Trading:
                Friendship = BaseFriendship;
                return;

            case FriendShipCauses.Vitamin:
                add = Friendship <= 99 ? 5 : Friendship <= 199 ? 3 : 2;
                break;

            case FriendShipCauses.EVBerry:
                add = Friendship <= 99 ? 10 : Friendship <= 199 ? 5 : 2;
                break;
        }

        if (add > 0)
        {
            if (catchBall.ID == LUXURY_BALL_ID) add++;
            if ("soothe bell".Equals(Item?.OriginalName.ToLower())) add *= 2;
        }

        Friendship = (Friendship + add).Clamp(FRIENDSHIP_RANGE.Min, FRIENDSHIP_RANGE.Max);
    }

    // Textures

    private Texture2D? GetTexture(int index)
    {
        if (_textures.Count <= index || _textures[index] == null)
        {
            // TODO Phase 7: full texture loading via TextureManager when ported
        }
        return _textures.Count > index ? _textures[index] : null;
    }

    public Texture2D? GetOverworldTexture() =>
        GetTexture(IsShiny == false ? 8 : 9);

    public Texture2D? GetMenuTexture(bool canGetEgg = true) =>
        GetTexture(EggSteps > 0 && canGetEgg == true ? 5 : 4);

    public Texture2D? GetTexture(bool frontView, bool forceShiny = false)
    {
        if (frontView == true)
        {
            return IsEgg == true ? GetTexture(6)
                : (IsShiny == true || forceShiny == true) ? GetTexture(2) : GetTexture(0);
        }
        return IsEgg == true ? GetTexture(7)
            : (IsShiny == true || forceShiny == true) ? GetTexture(3) : GetTexture(1);
    }

    public Tuple<float, float, float, float, float> GetModelProperties()
    {
        return new Tuple<float, float, float, float, float>(0.6f, 0f, 0f, 0f, 0.3f);
    }

    // Queries

    public bool CanEvolve(EvolutionCondition.EvolutionTrigger trigger, String argument)
    {
        return String.IsNullOrEmpty(EvolutionCondition.EvolutionNumber(this, trigger, argument)) == false;
    }

    public String GetEvolutionID(EvolutionCondition.EvolutionTrigger trigger, String argument)
    {
        return EvolutionCondition.EvolutionNumber(this, trigger, argument);
    }

    public void SetCatchInfos(Item ball, String method, String mapName = "")
    {
        String loc = String.IsNullOrEmpty(mapName) == true
            ? Localization.GetString("Places_" + (Screen.Level?.LevelFile ?? ""), Screen.Level?.LevelFile ?? "")
            : Localization.GetString("Places_" + mapName, mapName);
        CatchLocation = loc;
        CatchTrainerName = Core.Player.Name;
        OT = Core.Player.OT;
        CatchMethod = method;
        catchBall = ball;
    }

    public bool IsType(int checkType)
    {
        return (int)Type1.Type == checkType || (int)Type2.Type == checkType;
    }

    // Overload for Element.Types enum (VB passes enum values directly; C# needs explicit cast or overload)
    public bool IsType(Element.Types checkType)
    {
        return Type1.Type == checkType || Type2.Type == checkType;
    }

    public void PlayCry()
    {
        float pitch = 0f;
        if (MaxHP > 0)
        {
            int percent = (int)(Math.Ceiling((double)HP / MaxHP) * 100);
            if (percent <= HP_LOW_THRESHOLD) pitch = PITCH_LOW_HP;
            if (percent <= HP_VERY_LOW_THRESHOLD) pitch = PITCH_VERY_LOW_HP;
            if (percent == 0) pitch = PITCH_FAINTED;
        }
        SoundManager.PlayPokemonCry(Number, pitch, 0f, PokemonForms.GetCrySuffix(this));
    }

    public bool KnowsMove(BattleSystem.Attack move)
    {
        foreach (BattleSystem.Attack a in attacks)
        {
            if (a.ID == move.ID) return true;
        }
        return false;
    }

    public bool IsEgg => EggSteps > 0;

    public void GainEffort(Pokemon defeated)
    {
        int allEV = EVHP + EVAttack + EVDefense + EVSpAttack + EVSpDefense + EVSpeed;
        if (allEV >= EV_TOTAL_MAX)
        {
            return;
        }
        // TODO Phase 5: full EV gain logic with item bonuses
        EVHP = (EVHP + defeated.GiveEVHP).Clamp(EV_RANGE.Min, EV_SINGLE_MAX);
        EVAttack = (EVAttack + defeated.GiveEVAttack).Clamp(EV_RANGE.Min, EV_SINGLE_MAX);
        EVDefense = (EVDefense + defeated.GiveEVDefense).Clamp(EV_RANGE.Min, EV_SINGLE_MAX);
        EVSpAttack = (EVSpAttack + defeated.GiveEVSpAttack).Clamp(EV_RANGE.Min, EV_SINGLE_MAX);
        EVSpDefense = (EVSpDefense + defeated.GiveEVSpDefense).Clamp(EV_RANGE.Min, EV_SINGLE_MAX);
        EVSpeed = (EVSpeed + defeated.GiveEVSpeed).Clamp(EV_RANGE.Min, EV_SINGLE_MAX);
    }

    public bool HasHMMove()
    {
        foreach (BattleSystem.Attack m in attacks)
        {
            if (m.isHMMove == true) return true;
        }
        return false;
    }

    public bool IsFullyEvolved() => evolutionConditions.Count == 0;

    public bool HasHiddenAbility => hiddenAbility != null;

    public bool IsUsingHiddenAbility =>
        HasHiddenAbility == true && hiddenAbility?.ID == Ability?.ID;
}
