using System.IO;
using Microsoft.Xna.Framework;
using P3D;
using P3D.Items;
using P3D.ScriptVersion2;

namespace P3D.BattleSystem;

public class Trainer
{
    public int AILevel;
    public List<Attack> SignatureMoves = [];
    public List<Pokemon> Pokemons = [];
    public String TrainerType = "Youngster";
    public String TrainerType2 = "Youngster";
    public String Name = "Joey";
    public String Name2 = "Joey";
    public int Money = 84;
    public String SpriteName = "14";
    public String SpriteName2 = "14";
    public String Region = "Johto";
    public String Music = "Trainer";
    public String TrainerFile = String.Empty;
    public bool DoubleTrainer;
    public List<Item> Items = [];
    public int Gender = -1;
    public int IntroType = 10;
    public String GameJoltID = String.Empty;

    public String VSImageOrigin = "VSIntro";
    public Vector2 VSImagePosition = new Vector2(0, 0);
    public Size VSImageSize = new Size(64, 64);
    public Vector2 BarImagePosition = new Vector2(0, 0);

    public String OutroMessage = "TRAINER_DEFAULT_MESSAGE";
    public String OutroMessage2 = "TRAINER_DEFAULT_MESSAGE";
    public String IntroMessage = "TRAINER_DEFAULT_MESSAGE";
    public String DefeatMessage = "TRAINER_DEFAULT_MESSAGE";

    public static int FrontierTrainer = -1;

    public String BattleStartMessage = String.Empty;
    public Dictionary<int, String> BigDamageOwnMessage = [];
    public Dictionary<int, String> BigDamageOppMessage = [];
    public Dictionary<int, String> FaintedOwnMessage = [];
    public Dictionary<int, String> FaintedOppMessage = [];
    public Dictionary<int, String> RecallOwnMessage = [];
    public Dictionary<int, String> RecallOppMessage = [];
    public Dictionary<int, String> SendOutXOwnMessage = [];
    public Dictionary<int, String> SendOutXOppMessage = [];
    public String SendOutLastOwnMessage = String.Empty;
    public String SendOutLastOppMessage = String.Empty;
    public String PlayerLossMessage = String.Empty;

    private String _iniMusic = String.Empty;
    private String _defeatMusic = String.Empty;
    private String _battleMusic = String.Empty;
    private String _inSightMusic = "trainer_encounter";

    public int CountUseablePokemon
    {
        get
        {
            int count = 0;
            foreach (Pokemon p in Pokemons)
            {
                if (p.HP > 0 && p.Status.Equals(Pokemon.StatusProblems.Fainted) == false)
                {
                    count += 1;
                }
            }
            return count;
        }
    }

    public bool IsBeaten()
    {
        return ActionScript.IsRegistered("trainer_" + TrainerFile);
    }

    public static bool IsBeaten(String checkTrainerFile)
    {
        return ActionScript.IsRegistered("trainer_" + checkTrainerFile);
    }

    public bool HasBattlePokemon()
    {
        foreach (Pokemon pokemon in Pokemons)
        {
            if (pokemon.Status.Equals(Pokemon.StatusProblems.Fainted) == false && pokemon.HP > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void TrainerItemUse(int itemID)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].ID.Equals(itemID) == true)
            {
                Items.RemoveAt(i);
                return;
            }
        }
    }

    public Trainer()
    {
    }

    public Trainer(String trainerFile)
    {
        TrainerFile = trainerFile;

        String path = GameModeManager.GetScriptPath("Trainer\\" + trainerFile + ".trainer");
        Security.FileValidation.CheckFileValid(path, false, "Trainer.cs");

        String[] data = File.ReadAllLines(path);

        if ("[TRAINER FORMAT]".Equals(data[0]) == true)
        {
            LoadTrainer(data);
        }
        else
        {
            LoadTrainerLegacy(data);
        }
    }

    private void LoadTrainerLegacy(String[] data)
    {
        List<String> newData = [];
        List<String> sevenData = [.. data[7].Split('|')];

        newData.Add("Name|" + data[2]);
        newData.Add("TrainerClass|" + data[1]);
        newData.Add("Money|" + data[0]);
        newData.Add("IntroMessage|" + data[3]);
        newData.Add("OutroMessage|" + data[4]);
        newData.Add("DefeatMessage|" + data[5]);
        newData.Add("TextureID|" + data[6]);
        newData.Add("Region|" + sevenData[0]);

        Region = sevenData[0];
        Music = sevenData[1];

        newData.Add("IniMusic|" + GetIniMusicName());
        newData.Add("DefeatMusic|" + GetDefeatMusic());
        newData.Add("BattleMusic|" + GetBattleMusicName());

        newData.Add("Pokemon1|" + data[8].Remove(0, 2));
        newData.Add("Pokemon2|" + data[9].Remove(0, 2));
        newData.Add("Pokemon3|" + data[10].Remove(0, 2));
        newData.Add("Pokemon4|" + data[11].Remove(0, 2));
        newData.Add("Pokemon5|" + data[12].Remove(0, 2));
        newData.Add("Pokemon6|" + data[13].Remove(0, 2));

        if (data.Length > 14)
        {
            newData.Add("Items|" + data[14]);
        }
        if (data.Length > 15)
        {
            newData.Add("AI|" + data[15]);
        }
        if (data.Length > 16)
        {
            newData.Add("Gender|" + data[16]);
        }

        String sequenceData = "Blue,Blue";
        if (sevenData.Count.Equals(3) == true)
        {
            sequenceData = sevenData[2] + ",Blue";
        }
        else if (sevenData.Count > 3)
        {
            sequenceData = sevenData[2] + "," + sevenData[3];
        }
        newData.Add("IntroSequence|" + sequenceData);

        Logger.Log(Logger.LogTypes.Warning, "Trainer.cs: Converted legacy trainer file! Generated new trainer data:");
        Logger.Log(Logger.LogTypes.Message, newData.ToArray().ArrayToString());

        LoadTrainer([.. newData]);
    }

    private void LoadTrainer(String[] data)
    {
        List<String> pokeLines = [];
        int isDoubleTrainerValid = 0;
        String vsdata = "blue";
        String bardata = "blue";

        foreach (String line in data)
        {
            if (line.Contains('|') == false)
            {
                continue;
            }

            String pointer = line.Remove(line.IndexOf('|'));
            String value = line.Remove(0, line.IndexOf('|') + 1);

            switch (pointer.ToLower())
            {
                case "name":
                    Name = ScriptCommander.Parse(value).ToString();
                    if (Name.Contains(',') == true)
                    {
                        Name2 = Name.GetSplit(1);
                        Name = Name.GetSplit(0);
                        if (String.IsNullOrEmpty(BattleStartMessage) == true)
                        {
                            BattleStartMessage = "<Battle.TrainerName(0)> and <Battle.TrainerName(1)> want to battle!";
                        }
                        isDoubleTrainerValid += 1;
                    }
                    break;

                case "trainerclass":
                    TrainerType = ScriptCommander.Parse(value).ToString();
                    if (TrainerType.Contains(',') == true)
                    {
                        TrainerType2 = TrainerType.GetSplit(1);
                        TrainerType = TrainerType.GetSplit(0);
                        isDoubleTrainerValid += 1;
                    }
                    break;

                case "money":
                    Money = (int)ScriptConversion.ToInteger(ScriptCommander.Parse(value).ToString());
                    break;

                case "intromessage":
                    IntroMessage = ScriptCommander.Parse(value).ToString();
                    break;

                case "outromessage":
                    OutroMessage = value;
                    if (OutroMessage.Contains('|') == true)
                    {
                        OutroMessage2 = OutroMessage.GetSplit(1, "|");
                        OutroMessage = OutroMessage.GetSplit(0, "|");
                        isDoubleTrainerValid += 1;
                    }
                    break;

                case "defeatmessage":
                    DefeatMessage = ScriptCommander.Parse(value).ToString();
                    break;

                case "battlestartmessage":
                    BattleStartMessage = value;
                    break;

                case "bigdamageownmessage":
                {
                    int instance = 1;
                    if (value.Contains('|') == true)
                    {
                        instance = (int)ScriptConversion.ToInteger(value.GetSplit(1, "|"));
                    }
                    BigDamageOwnMessage.Add(instance, value.GetSplit(0, "|"));
                    break;
                }

                case "bigdamageoppmessage":
                {
                    int instance = 1;
                    if (value.Contains('|') == true)
                    {
                        instance = (int)ScriptConversion.ToInteger(value.GetSplit(1, "|"));
                    }
                    BigDamageOppMessage.Add(instance, value.GetSplit(0, "|"));
                    break;
                }

                case "faintedownmessage":
                {
                    int instance = 1;
                    if (value.Contains('|') == true)
                    {
                        instance = (int)ScriptConversion.ToInteger(value.GetSplit(1, "|"));
                    }
                    FaintedOwnMessage.Add(instance, value.GetSplit(0, "|"));
                    break;
                }

                case "faintedoppmessage":
                {
                    int instance = 1;
                    if (value.Contains('|') == true)
                    {
                        instance = (int)ScriptConversion.ToInteger(value.GetSplit(1, "|"));
                    }
                    FaintedOppMessage.Add(instance, value.GetSplit(0, "|"));
                    break;
                }

                case "recallownmessage":
                {
                    int instance = 1;
                    if (value.Contains('|') == true)
                    {
                        instance = (int)ScriptConversion.ToInteger(value.GetSplit(1, "|"));
                    }
                    RecallOwnMessage.Add(instance, value.GetSplit(0, "|"));
                    break;
                }

                case "recalloppmessage":
                {
                    int instance = 1;
                    if (value.Contains('|') == true)
                    {
                        instance = (int)ScriptConversion.ToInteger(value.GetSplit(1, "|"));
                    }
                    RecallOppMessage.Add(instance, value.GetSplit(0, "|"));
                    break;
                }

                case "sendoutownmessage":
                {
                    int partyID = 0;
                    if (value.Contains('|') == true)
                    {
                        partyID = (int)ScriptConversion.ToInteger(value.GetSplit(1, "|"));
                    }
                    SendOutXOwnMessage.Add(partyID, value.GetSplit(0, "|"));
                    break;
                }

                case "sendoutoppmessage":
                {
                    int partyID = 0;
                    if (value.Contains('|') == true)
                    {
                        partyID = (int)ScriptConversion.ToInteger(value.GetSplit(1, "|"));
                    }
                    SendOutXOppMessage.Add(partyID, value.GetSplit(0, "|"));
                    break;
                }

                case "sendoutlastownmessage":
                    SendOutLastOwnMessage = value;
                    break;

                case "sendoutlastoppmessage":
                    SendOutLastOppMessage = value;
                    break;

                case "playerlossmessage":
                    PlayerLossMessage = value;
                    break;

                case "textureid":
                    SpriteName = ScriptCommander.Parse(value).ToString();
                    if (SpriteName.Contains(',') == true)
                    {
                        SpriteName2 = SpriteName.GetSplit(1);
                        SpriteName = SpriteName.GetSplit(0);
                        isDoubleTrainerValid += 1;
                    }
                    break;

                case "region":
                    Region = ScriptCommander.Parse(value).ToString();
                    break;

                case "inimusic":
                    _iniMusic = ScriptCommander.Parse(value).ToString();
                    break;

                case "defeatmusic":
                    _defeatMusic = ScriptCommander.Parse(value).ToString();
                    break;

                case "battlemusic":
                    _battleMusic = ScriptCommander.Parse(value).ToString();
                    break;

                case "insightmusic":
                    _inSightMusic = ScriptCommander.Parse(value).ToString();
                    break;

                case "pokemon1":
                case "pokemon2":
                case "pokemon3":
                case "pokemon4":
                case "pokemon5":
                case "pokemon6":
                    if (String.IsNullOrEmpty(value) == false)
                    {
                        pokeLines.Add(value);
                    }
                    break;

                case "items":
                    if (String.IsNullOrEmpty(value) == false)
                    {
                        String[] itemData = ScriptCommander.Parse(value).ToString().Split(',');
                        foreach (String itemID in itemData)
                        {
                            Items.Add(Item.GetItemByID(itemID));
                        }
                    }
                    break;

                case "gender":
                    Gender = (int)MathHelper.Clamp(
                        (int)ScriptConversion.ToInteger(ScriptCommander.Parse(value).ToString()),
                        -1, 1);
                    break;

                case "ai":
                    AILevel = (int)ScriptConversion.ToInteger(ScriptCommander.Parse(value).ToString());
                    break;

                case "introsequence":
                    value = ScriptCommander.Parse(value).ToString();
                    if (value.Contains(',') == true)
                    {
                        vsdata = value.GetSplit(0);
                        bardata = value.GetSplit(1);
                    }
                    else
                    {
                        vsdata = value;
                    }
                    break;

                case "introtype":
                    IntroType = (int)ScriptConversion.ToInteger(ScriptCommander.Parse(value).ToString());
                    break;
            }
        }

        if (String.IsNullOrEmpty(BattleStartMessage) == true)
        {
            BattleStartMessage = "<Battle.TrainerName(0)> wants to battle!";
        }

        int maxLevelCap = (int)ScriptConversion.ToInteger(
            GameModeManager.GetGameRuleValue("MaxLevel", "100"));

        foreach (String pokeLine in pokeLines)
        {
            String pokeData = pokeLine.GetSplit(1, "|");
            if (String.IsNullOrEmpty(pokeData) == true)
            {
                continue;
            }

            if (ScriptCommander.Parse(pokeData).ToString().StartsWith("{") == true)
            {
                pokeData = ScriptCommander.Parse(pokeData).ToString()
                    .Replace("§", ",").Replace("«", "[").Replace("»", "]");
            }

            if (pokeData.StartsWith("{") == true && pokeData.EndsWith("}") == true)
            {
                Pokemon p = Pokemon.GetPokemonByData(pokeData);
                int level = p.Level;
                int addLevel = ComputeAddLevel(level);
                if (level + addLevel > maxLevelCap)
                {
                    addLevel = maxLevelCap - level;
                }
                if (addLevel < 0)
                {
                    addLevel = 0;
                }

                int targetLevel = level + addLevel;
                while (targetLevel > p.Level)
                {
                    p.LevelUp(false);
                    p.Experience = p.NeedExperience(p.Level);
                }
                p.HP = p.MaxHP;
                Pokemons.Add(p);
            }
            else
            {
                String firstPart = String.Empty;
                String secondPart = String.Empty;
                bool endedFirstPart = false;
                String readData = pokeData;
                bool openTag = false;

                while (readData.Length > 0)
                {
                    switch (readData[0].ToString())
                    {
                        case "<":
                            openTag = true;
                            break;
                        case ">":
                            openTag = false;
                            break;
                        case ",":
                            if (openTag == false)
                            {
                                endedFirstPart = true;
                            }
                            break;
                    }

                    if (readData[0].ToString().Equals(",") == false || openTag == true)
                    {
                        if (endedFirstPart == true)
                        {
                            secondPart += readData[0].ToString();
                        }
                        else
                        {
                            firstPart += readData[0].ToString();
                        }
                    }

                    readData = readData.Remove(0, 1);
                }

                String pk = ScriptCommander.Parse(firstPart).ToString();
                int levelVal = (int)ScriptConversion.ToInteger(ScriptCommander.Parse(secondPart).ToString());

                int id = (int)ScriptConversion.ToInteger(pk.Split('_')[0]);
                String ad = String.Empty;
                if (pk.Contains('_') == true)
                {
                    ad = pk.Split('_')[1];
                }

                int addLevel = ComputeAddLevel(levelVal);
                if (levelVal + addLevel > maxLevelCap)
                {
                    addLevel = maxLevelCap - levelVal;
                }
                if (addLevel < 0)
                {
                    addLevel = 0;
                }
                levelVal += addLevel;
                if (levelVal > maxLevelCap)
                {
                    levelVal = maxLevelCap;
                }

                Pokemon? p;
                if (FrontierTrainer > -1)
                {
                    p = FrontierSpawner.GetPokemon(levelVal, FrontierTrainer, null);
                }
                else if (String.IsNullOrEmpty(ad) == false)
                {
                    p = Pokemon.GetPokemonByID(id, ad);
                    p.Generate(levelVal, true, ad);
                }
                else
                {
                    p = Pokemon.GetPokemonByID(id);
                    p.Generate(levelVal, true);
                }

                if (p.IsGenderless == false)
                {
                    switch (Gender)
                    {
                        case 0:
                            if (p.IsMale > 0.0m)
                            {
                                p.Gender = Pokemon.Genders.Male;
                            }
                            break;

                        case 1:
                            if (p.IsMale < 100.0m)
                            {
                                p.Gender = Pokemon.Genders.Female;
                            }
                            break;
                    }
                }

                p.IsShiny = false;
                Pokemons.Add(p);
            }
        }

        if (isDoubleTrainerValid.Equals(4) == true)
        {
            DoubleTrainer = true;
        }

        SetIniImage(vsdata, bardata);
        FrontierTrainer = -1;
    }

    private int ComputeAddLevel(int level)
    {
        int diffMode = Core.Player.DifficultyMode;
        String multiplierStr;
        if (diffMode.Equals(0) == true)
        {
            multiplierStr = GameModeManager.GetGameRuleValue("LevelMultiplier", "1.0");
        }
        else if (diffMode.Equals(1) == true)
        {
            multiplierStr = GameModeManager.GetGameRuleValue("LevelMultiplier", "1.1");
        }
        else
        {
            multiplierStr = GameModeManager.GetGameRuleValue("LevelMultiplier", "1.2");
        }
        float multiplier = (float)ScriptConversion.ToSingle(multiplierStr.InsertDecSeparator());
        return (int)Math.Ceiling(level * multiplier - level);
    }

    private static readonly (Vector2 Pos, String Key)[] VS_IMAGE_TABLE =
    [
        (new Vector2(0, 0), "blue"),    (new Vector2(0, 0), "0"),
        (new Vector2(1, 0), "orange"),  (new Vector2(1, 0), "1"),
        (new Vector2(0, 1), "lightgreen"), (new Vector2(0, 1), "2"),
        (new Vector2(1, 1), "gray"),    (new Vector2(1, 1), "3"),
        (new Vector2(0, 2), "violet"),  (new Vector2(0, 2), "4"),
        (new Vector2(1, 2), "green"),   (new Vector2(1, 2), "5"),
        (new Vector2(0, 3), "yellow"),  (new Vector2(0, 3), "6"),
        (new Vector2(1, 3), "brown"),   (new Vector2(1, 3), "7"),
        (new Vector2(0, 4), "lightblue"), (new Vector2(0, 4), "8"),
        (new Vector2(1, 4), "lightgray"), (new Vector2(1, 4), "9"),
        (new Vector2(0, 5), "red"),     (new Vector2(0, 5), "10"),
        (new Vector2(1, 5), "empty"),   (new Vector2(1, 5), "11"),
    ];

    private void SetIniImage(String vsType, String barType)
    {
        VSImagePosition = ResolveImagePosition(vsType.ToLower(), ref VSImageOrigin, ref VSImageSize);
        BarImagePosition = ResolveImagePosition(barType.ToLower(), ref VSImageOrigin, ref VSImageSize);
    }

    private static Vector2 ResolveImagePosition(String key, ref String origin, ref Size size)
    {
        if ("battlefrontier".Equals(key) == true)
        {
            origin = "battlefrontier";
            size = new Size(275, 275);
            return new Vector2(0, 0);
        }

        foreach ((Vector2 pos, String tableKey) in VS_IMAGE_TABLE)
        {
            if (tableKey.Equals(key) == true)
            {
                return pos;
            }
        }

        if (StringHelper.IsNumeric(key) == true)
        {
            int idx = (int)ScriptConversion.ToInteger(key);
            if (idx > 11)
            {
                int x = idx;
                int y = 0;
                while (x > 1)
                {
                    x -= 2;
                    y += 1;
                }
                return new Vector2(x, y);
            }
        }

        return new Vector2(0, 0);
    }

    public String GetIniMusicName()
    {
        if (String.IsNullOrEmpty(_iniMusic) == false)
        {
            return _iniMusic;
        }

        String middle = "trainer";
        switch (Music.ToLower())
        {
            case "rival":
                middle = "rival";
                break;
            case "leader":
                middle = "leader";
                break;
            case "rocket":
                middle = "rocket";
                break;
        }

        return Region + "_" + middle + "_intro";
    }

    public String GetDefeatMusic()
    {
        if (String.IsNullOrEmpty(_defeatMusic) == false)
        {
            return _defeatMusic;
        }

        String pre = "trainer";
        if ("leader".Equals(Music.ToLower()) == true)
        {
            pre = "leader";
        }

        return pre + "_defeat";
    }

    public String GetBattleMusicName()
    {
        if (String.IsNullOrEmpty(_battleMusic) == false)
        {
            return _battleMusic;
        }
        return Region.ToLower() + "_" + Music.ToLower();
    }

    public String GetInSightMusic()
    {
        return _inSightMusic;
    }
}
