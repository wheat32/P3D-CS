namespace P3D;

public class Script
{
    public enum ScriptTypes : int
    {
        // V1 types:
        Move = 0,
        MoveAsync = 1,
        MovePlayer = 2,
        Turn = 3,
        TurnPlayer = 4,
        Warp = 5,
        WarpPlayer = 6,
        Heal = 7,
        ViewPokemonImage = 8,
        GiveItem = 9,
        RemoveItem = 10,
        GetBadge = 11,
        Pokemon = 12,
        NPC = 13,
        Player = 14,
        Text = 15,
        Options = 16,
        SelectCase = 17,
        Wait = 18,
        Camera = 19,
        Battle = 20,
        Script = 21,
        Trainer = 22,
        Achievement = 23,
        Action = 24,
        Music = 25,
        Sound = 26,
        Register = 27,
        Unregister = 28,
        MessageBulb = 29,
        Entity = 30,
        Environment = 31,
        Value = 32,
        Level = 33,
        SwitchWhen = 34,
        SwitchEndWhen = 35,
        SwitchIf = 36,
        SwitchThen = 37,
        SwitchElse = 38,
        SwitchEndIf = 39,
        SwitchEnd = 40,

        // V2 types:
        Command = 100,
        @if = 101,
        when = 102,
        then = 103,
        @else = 104,
        endif = 105,
        end = 106,
        select = 107,
        endwhen = 108,
    }

    public ScriptV1 ScriptV1 = new ScriptV1();
    public ScriptV2 ScriptV2 = new ScriptV2();

    public String ScriptLine = "";
    public int Level;

    public String Value
    {
        get
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    return ScriptV1.Value;
                case 2:
                    return ScriptV2.Value;
            }
            return "";
        }
        set
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    ScriptV1.Value = value;
                    break;
                case 2:
                    ScriptV2.Value = value;
                    break;
            }
        }
    }

    public ScriptTypes ScriptType
    {
        get
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    return (ScriptTypes)ScriptV1.ScriptType;
                case 2:
                    return (ScriptTypes)ScriptV2.ScriptType;
            }
            return 0;
        }
        set
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    ScriptV1.ScriptType = (ScriptV1.ScriptTypes)value;
                    break;
                case 2:
                    ScriptV2.ScriptType = (ScriptV2.ScriptTypes)value;
                    break;
            }
        }
    }

    public bool started
    {
        get
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    return ScriptV1.started;
                case 2:
                    return ScriptV2.started;
            }
            return false;
        }
        set
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    ScriptV1.started = value;
                    break;
                case 2:
                    ScriptV2.started = value;
                    break;
            }
        }
    }

    public bool IsReady
    {
        get
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    return ScriptV1.IsReady;
                case 2:
                    return ScriptV2.IsReady;
            }
            return false;
        }
        set
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    ScriptV1.IsReady = value;
                    break;
                case 2:
                    ScriptV2.IsReady = value;
                    break;
            }
        }
    }

    public bool CanContinue
    {
        get
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    return ScriptV1.CanContinue;
                case 2:
                    return ScriptV2.CanContinue;
            }
            return false;
        }
        set
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    ScriptV1.CanContinue = value;
                    break;
                case 2:
                    ScriptV2.CanContinue = value;
                    break;
            }
        }
    }

    public Script(String line, int level)
    {
        Level = level;
        ScriptLine = line;

        switch (ActionScript.CSL().ScriptVersion)
        {
            case 1:
                ScriptV1.Initialize(line);
                break;
            case 2:
                if (line.Equals("") == false)
                {
                    ScriptV2.Initialize(line);
                }
                break;
        }
    }

    public void Update()
    {
        if (Level == ActionScript.ScriptLevelIndex)
        {
            switch (ActionScript.CSL().ScriptVersion)
            {
                case 1:
                    ScriptV1.Update();
                    break;
                case 2:
                    ScriptV2.Update();
                    break;
            }
        }
        else
        {
            IsReady = true;
        }
    }

    public static void NameRival(String name)
    {
        Core.Player.RivalName = name;
    }

    public static String[] SaveNPCTrade = new String[17];

    public static void ExitedNPCTrade()
    {
        String message2 = SaveNPCTrade[14];
        Screen.TextBox.Show(message2, [], false, false);
    }

    public static void DoNPCTradeHandler(Object[] parameters)
    {
        DoNPCTrade((int)parameters[0]);
    }

    public static void DoNPCTrade(int pokeIndex)
    {
        Core.SetScreen(Core.CurrentScreen.PreScreen);

        Pokemon ownPokemon = Core.Player.Pokemons[pokeIndex];

        String ownPokeID = "";
        String ownPokeAD = "";
        int ownPokeIndex = -1;

        String oppPokeID = SaveNPCTrade[1];
        String oppPokeAD = "";
        bool ownPreventFormGeneration = false;
        bool oppPreventFormGeneration = false;

        if (SaveNPCTrade[0].Contains(",") == true)
        {
            for (int p = 0; p <= SaveNPCTrade[0].Split(',').Length; p++)
            {
                if (SaveNPCTrade[0].GetSplit(p).Equals(PokemonForms.GetPokemonDataFileName(ownPokemon.Number, ownPokemon.AdditionalData, true)) == true)
                {
                    ownPokeID = SaveNPCTrade[0].GetSplit(p);
                    ownPokeIndex = p;
                    break;
                }
            }
            if (ownPokeID.Equals("") == true)
            {
                ownPokeID = SaveNPCTrade[0].GetSplit(0, ",");
            }
        }
        else
        {
            ownPokeID = SaveNPCTrade[0];
        }

        if (SaveNPCTrade[1].Contains(",") == true && ownPokeIndex != -1)
        {
            oppPokeID = SaveNPCTrade[1].GetSplit(ownPokeIndex);
        }
        else
        {
            oppPokeID = SaveNPCTrade[1];
        }

        if (ownPokeID.Contains("_") == true)
        {
            ownPokeAD = PokemonForms.GetAdditionalValueFromDataFile(ownPokeID);
            ownPokeID = ownPokeID.GetSplit(0, "_");
        }

        if (oppPokeID.Contains("_") == true)
        {
            oppPokeAD = PokemonForms.GetAdditionalValueFromDataFile(oppPokeID);
            oppPokeID = oppPokeID.GetSplit(0, "_");
        }

        if (ownPokeID.Contains(";") == true)
        {
            ownPokeAD = ownPokeID.GetSplit(1, ";");
            ownPokeID = ownPokeID.GetSplit(0, ";");
            ownPreventFormGeneration = true;
        }

        if (oppPokeID.Contains(";") == true)
        {
            oppPokeAD = oppPokeID.GetSplit(1, ";");
            oppPokeID = oppPokeID.GetSplit(0, ";");
            oppPreventFormGeneration = true;
        }

        Pokemon oppPokemon = Pokemon.GetPokemonByID(int.Parse(oppPokeID), oppPokeAD, oppPreventFormGeneration);

        int level = ownPokemon.Level;

        if (StringHelper.IsNumeric(SaveNPCTrade[2]) == true)
        {
            level = ScriptConversion.ToInteger(SaveNPCTrade[2]);
        }

        oppPokemon.Generate(level, true);

        Pokemon.Genders gender;

        if (StringHelper.IsNumeric(SaveNPCTrade[3]) == true)
        {
            int genderID = ScriptConversion.ToInteger(SaveNPCTrade[3]);
            if (genderID == -1)
            {
                if (oppPokemon.IsGenderless == true)
                {
                    genderID = 2;
                }
                else
                {
                    if (oppPokemon.IsMale == 0)
                    {
                        genderID = 1;
                    }
                    else if (oppPokemon.IsMale == 100)
                    {
                        genderID = 0;
                    }
                    else
                    {
                        genderID = Core.Random.Next(0, 2);
                    }
                }
            }

            switch (genderID)
            {
                case 0:
                    gender = Pokemon.Genders.Male;
                    break;
                case 1:
                    gender = Pokemon.Genders.Female;
                    break;
                case 2:
                    gender = Pokemon.Genders.Genderless;
                    break;
                default:
                    if (oppPokemon.IsGenderless == true)
                    {
                        gender = Pokemon.Genders.Genderless;
                    }
                    else
                    {
                        if (oppPokemon.IsMale == 0)
                        {
                            gender = Pokemon.Genders.Female;
                        }
                        else if (oppPokemon.IsMale == 100)
                        {
                            gender = Pokemon.Genders.Male;
                        }
                        else
                        {
                            gender = ownPokemon.Gender;
                        }
                    }
                    break;
            }
        }
        else
        {
            if (oppPokemon.IsGenderless == true)
            {
                gender = Pokemon.Genders.Genderless;
            }
            else
            {
                if (oppPokemon.IsMale == 0)
                {
                    gender = Pokemon.Genders.Female;
                }
                else if (oppPokemon.IsMale == 100)
                {
                    gender = Pokemon.Genders.Male;
                }
                else
                {
                    gender = ownPokemon.Gender;
                }
            }
        }

        oppPokemon.Gender = gender;

        if (SaveNPCTrade[4].Equals("") == false)
        {
            oppPokemon.Attacks.Clear();
            String[] attacks = [SaveNPCTrade[4]];
            if (SaveNPCTrade[4].Contains(",") == true)
            {
                attacks = SaveNPCTrade[4].Split(',');
            }
            foreach (String attackID in attacks)
            {
                if (oppPokemon.Attacks.Count < 4)
                {
                    oppPokemon.Attacks.Add(BattleSystem.Attack.GetAttackByID(ScriptConversion.ToInteger(attackID)));
                }
            }
        }

        if (SaveNPCTrade[5].Equals("") == false)
        {
            oppPokemon.IsShiny = bool.Parse(SaveNPCTrade[5]);
        }

        oppPokemon.OT = SaveNPCTrade[6];
        oppPokemon.CatchTrainerName = SaveNPCTrade[7];
        oppPokemon.catchBall = Items.Item.GetItemByID(SaveNPCTrade[8]);
        oppPokemon.Item = Items.Item.GetItemByID(SaveNPCTrade[9]);
        oppPokemon.CatchLocation = SaveNPCTrade[10];
        oppPokemon.CatchMethod = SaveNPCTrade[11];
        if (SaveNPCTrade[12].Equals("") == false)
        {
            oppPokemon.NickName = SaveNPCTrade[12];
        }

        String message1 = "";
        if (SaveNPCTrade[13].Equals("") == false)
        {
            message1 = SaveNPCTrade[13];
        }
        String message2 = "";
        if (SaveNPCTrade[14].Equals("") == false)
        {
            message2 = SaveNPCTrade[14];
        }

        String register = SaveNPCTrade[15];

        String afterTradeMessage = "";
        if (SaveNPCTrade.Length > 16 && SaveNPCTrade[16].Equals("") == false)
        {
            afterTradeMessage = SaveNPCTrade[16]
                .Replace("[YOURPOKEMON]", ownPokemon.Name)
                .Replace("[THEIRPOKEMON]", oppPokemon.Name);
        }

        if (PokemonForms.GetPokemonDataFileName(int.Parse(ownPokeID), ownPokeAD, true)
            .Equals(PokemonForms.GetPokemonDataFileName(ownPokemon.Number, ownPokemon.AdditionalData, true)) == true)
        {
            Core.Player.Pokemons.RemoveAt(pokeIndex);
            Core.Player.Pokemons.Add(oppPokemon);

            int pokedexType = 2;
            if (oppPokemon.IsShiny == true)
            {
                pokedexType = 3;
            }

            String dexID = PokemonForms.GetPokemonDataFileName(oppPokemon.Number, oppPokemon.AdditionalData);
            if (dexID.Contains("_") == false)
            {
                if (PokemonForms.GetAdditionalDataForms(oppPokemon.Number) != null &&
                    PokemonForms.GetAdditionalDataForms(oppPokemon.Number)!.Contains(oppPokemon.AdditionalData) == true)
                {
                    dexID = oppPokemon.Number + ";" + oppPokemon.AdditionalData;
                }
                else
                {
                    dexID = oppPokemon.Number.ToString();
                }
            }

            Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, pokedexType);

            if (register.Equals("") == false)
            {
                ActionScript.RegisterID(register);
            }

            Core.Player.AddPoints(10, "Traded with NPC.");

            if (message1.Equals("") == false)
            {
                Screen.TextBox.Show(message1, [], false, false);
            }
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new NPCTradeScreen(Core.CurrentScreen, ownPokemon, oppPokemon, oppPokemon.CatchTrainerName, afterTradeMessage), Microsoft.Xna.Framework.Color.Black, false));
        }
        else
        {
            if (message2.Equals("") == false)
            {
                Screen.TextBox.Show(message2, [], false, false);
            }
        }
    }

    public Script Clone()
    {
        return new Script(ScriptLine, Level);
    }

    public static List<String> ParseArguments(String inputString, char separatorChar = ',')
    {
        List<String> arguments = [];
        bool stringDeclaration = false;
        String data = inputString;
        String cArg = "";

        while (data.Length > 0)
        {
            char c = data[0];
            if (c == separatorChar)
            {
                if (stringDeclaration == true)
                {
                    cArg += c.ToString();
                }
                else
                {
                    arguments.Add(cArg);
                    cArg = "";
                }
            }
            else if (c == '"')
            {
                if (stringDeclaration == false)
                {
                    stringDeclaration = true;
                }
                else
                {
                    if (data.Length == 1 || data[1] != data[0])
                    {
                        stringDeclaration = !stringDeclaration;
                    }
                    else if (data.Length > 1 && data[1] == data[0])
                    {
                        if (stringDeclaration == true)
                        {
                            cArg += "\"";
                        }
                        data = data.Remove(0, 1);
                    }
                }
            }
            else
            {
                cArg += c.ToString();
            }

            data = data.Remove(0, 1);
        }

        arguments.Add(cArg);

        return arguments;
    }
}
