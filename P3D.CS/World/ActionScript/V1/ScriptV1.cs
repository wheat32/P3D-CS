using Microsoft.Xna.Framework;

namespace P3D;

public class ScriptV1
{
    public enum ScriptTypes : int
    {
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
        Level = 33,
        SwitchWhen = 34,
        SwitchEndWhen = 35,
        SwitchIf = 36,
        SwitchThen = 37,
        SwitchElse = 38,
        SwitchEndIf = 39,
        SwitchEnd = 40,
    }

    public ScriptTypes ScriptType = ScriptTypes.Text;
    public String Value = String.Empty;
    public bool started;
    public bool IsReady;
    public bool CanContinue;

    public void Initialize(String line)
    {
        if (line.StartsWith("@") == true)
        {
            line = line.Remove(0, 1);

            String script = line;
            String command = String.Empty;

            if (line.Contains(":") == true)
            {
                script = line.Remove(line.IndexOf(':'));
                command = line.Remove(0, line.IndexOf(':') + 1);
            }

            switch (script)
            {
                case "Move":
                    if (command.StartsWith("Async,") == true)
                    {
                        command = command.Remove(0, 6);
                        ScriptType = ScriptTypes.MoveAsync;
                    }
                    else if (command.StartsWith("Player,") == true)
                    {
                        command = command.Remove(0, 7);
                        ScriptType = ScriptTypes.MovePlayer;
                    }
                    else
                    {
                        ScriptType = ScriptTypes.Move;
                    }
                    Value = command;
                    break;
                case "Turn":
                    if (command.StartsWith("Player,") == true)
                    {
                        command = command.Remove(0, 7);
                        ScriptType = ScriptTypes.TurnPlayer;
                    }
                    else
                    {
                        ScriptType = ScriptTypes.Turn;
                    }
                    Value = command;
                    break;
                case "Warp":
                    if (command.StartsWith("Player,") == true)
                    {
                        command = command.Remove(0, 7);
                        ScriptType = ScriptTypes.WarpPlayer;
                    }
                    else
                    {
                        ScriptType = ScriptTypes.Warp;
                    }
                    Value = command;
                    break;
                case "Heal":
                    ScriptType = ScriptTypes.Heal;
                    Value = command;
                    break;
                case "ViewPokemonImage":
                    ScriptType = ScriptTypes.ViewPokemonImage;
                    Value = command;
                    break;
                case "GiveItem":
                    ScriptType = ScriptTypes.GiveItem;
                    Value = command;
                    break;
                case "RemoveItem":
                    ScriptType = ScriptTypes.RemoveItem;
                    Value = command;
                    break;
                case "GetBadge":
                    ScriptType = ScriptTypes.GetBadge;
                    Value = command;
                    break;
                case "Action":
                    ScriptType = ScriptTypes.Action;
                    Value = command;
                    break;
                case "Music":
                    ScriptType = ScriptTypes.Music;
                    Value = command;
                    break;
                case "Sound":
                    ScriptType = ScriptTypes.Sound;
                    Value = command;
                    break;
                case "Text":
                    ScriptType = ScriptTypes.Text;
                    Value = command;
                    break;
                case "Options":
                    ScriptType = ScriptTypes.Options;
                    Value = command;
                    break;
                case "Wait":
                    ScriptType = ScriptTypes.Wait;
                    Value = command;
                    break;
                case "Register":
                    ScriptType = ScriptTypes.Register;
                    Value = command;
                    break;
                case "Unregister":
                    ScriptType = ScriptTypes.Unregister;
                    Value = command;
                    break;
                case "NPC":
                    ScriptType = ScriptTypes.NPC;
                    Value = command;
                    break;
                case "Achievement":
                    ScriptType = ScriptTypes.Achievement;
                    Value = command;
                    break;
                case "Trainer":
                    ScriptType = ScriptTypes.Trainer;
                    Value = command;
                    break;
                case "Battle":
                    ScriptType = ScriptTypes.Battle;
                    Value = command;
                    break;
                case "Script":
                    ScriptType = ScriptTypes.Script;
                    Value = command;
                    break;
                case "Bulb":
                case "MessageBulb":
                    ScriptType = ScriptTypes.MessageBulb;
                    Value = command;
                    break;
                case "Camera":
                    ScriptType = ScriptTypes.Camera;
                    Value = command;
                    break;
                case "Pokemon":
                    ScriptType = ScriptTypes.Pokemon;
                    Value = command;
                    break;
                case "Player":
                    ScriptType = ScriptTypes.Player;
                    Value = command;
                    break;
                case "Entity":
                    ScriptType = ScriptTypes.Entity;
                    Value = command;
                    break;
                case "Environment":
                    ScriptType = ScriptTypes.Environment;
                    Value = command;
                    break;
                case "Level":
                    ScriptType = ScriptTypes.Level;
                    Value = command;
                    break;
            }
        }
        else if (line.StartsWith(":") == true)
        {
            line = line.Remove(0, 1);

            String script = String.Empty;
            String command = String.Empty;

            if (line.Contains(":") == true)
            {
                script = line.Remove(line.IndexOf(':'));
                command = line.Remove(0, line.IndexOf(':') + 1);
            }
            else
            {
                script = line;
                command = String.Empty;
            }

            switch (script)
            {
                case "if":
                    ScriptType = ScriptTypes.SwitchIf;
                    Value = command;
                    break;
                case "when":
                    ScriptType = ScriptTypes.SwitchWhen;
                    Value = command;
                    break;
                case "then":
                    ScriptType = ScriptTypes.SwitchThen;
                    break;
                case "else":
                    ScriptType = ScriptTypes.SwitchElse;
                    break;
                case "endif":
                    ScriptType = ScriptTypes.SwitchEndIf;
                    break;
                case "endwhen":
                    ScriptType = ScriptTypes.SwitchEndWhen;
                    break;
                case "end":
                    ScriptType = ScriptTypes.SwitchEnd;
                    break;
                case "select":
                    ScriptType = ScriptTypes.SelectCase;
                    Value = command;
                    break;
            }
        }
    }

    public void Update()
    {
        switch (ScriptType)
        {
            case ScriptTypes.Text:
                DoText();
                break;
            case ScriptTypes.Wait:
                DoWait();
                break;
            case ScriptTypes.Move:
                Move();
                break;
            case ScriptTypes.MoveAsync:
                MoveAsync();
                break;
            case ScriptTypes.MovePlayer:
                MovePlayer();
                break;
            case ScriptTypes.Register:
                Register();
                break;
            case ScriptTypes.Unregister:
                Unregister();
                break;
            case ScriptTypes.Turn:
                Turn();
                break;
            case ScriptTypes.TurnPlayer:
                TurnPlayer();
                break;
            case ScriptTypes.Warp:
                Warp();
                break;
            case ScriptTypes.WarpPlayer:
                WarpPlayer();
                break;
            case ScriptTypes.Heal:
                Heal();
                break;
            case ScriptTypes.Action:
                DoAction();
                break;
            case ScriptTypes.Music:
                DoMusic();
                break;
            case ScriptTypes.Sound:
                DoSound();
                break;
            case ScriptTypes.ViewPokemonImage:
                ViewPokemonImage();
                break;
            case ScriptTypes.NPC:
                DoNPC();
                break;
            case ScriptTypes.Achievement:
                GetAchievement();
                break;
            case ScriptTypes.GiveItem:
                GiveItem();
                break;
            case ScriptTypes.RemoveItem:
                RemoveItem();
                break;
            case ScriptTypes.Trainer:
                DoTrainerBattle();
                break;
            case ScriptTypes.Battle:
                DoBattle();
                break;
            case ScriptTypes.Script:
                DoScript();
                break;
            case ScriptTypes.MessageBulb:
                DoMessageBulb();
                break;
            case ScriptTypes.Camera:
                DoCamera();
                break;
            case ScriptTypes.GetBadge:
                GetBadge();
                break;
            case ScriptTypes.Pokemon:
                DoPokemon();
                break;
            case ScriptTypes.Player:
                DoPlayer();
                break;
            case ScriptTypes.Entity:
                DoEntity();
                break;
            case ScriptTypes.Environment:
                DoEnvironment();
                break;
            case ScriptTypes.Level:
                DoLevel();
                break;

            case ScriptTypes.SwitchIf:
                DoIf();
                break;
            case ScriptTypes.SwitchThen:
                IsReady = true;
                break;
            case ScriptTypes.SwitchElse:
            {
                OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                oS.ActionScript.ChooseIf(true);
                IsReady = true;
                break;
            }
            case ScriptTypes.SwitchEndIf:
            {
                OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                oS.ActionScript.ChooseIf(true);
                IsReady = true;
                break;
            }
            case ScriptTypes.SwitchWhen:
                if (ActionScript.CSL().WaitingEndWhen[ActionScript.CSL().WhenIndex] == true)
                {
                    OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                    oS.ActionScript.Switch("");
                }
                IsReady = true;
                break;
            case ScriptTypes.SwitchEndWhen:
            {
                OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                oS.ActionScript.Switch("");
                IsReady = true;
                break;
            }
            case ScriptTypes.SwitchEnd:
                EndScript();
                break;
            case ScriptTypes.Options:
                DoOptions();
                break;
            case ScriptTypes.SelectCase:
                DoSelect();
                break;
        }
    }

    public void EndScript()
    {
        ActionScript.ScriptLevelIndex -= 1;
        if (ActionScript.ScriptLevelIndex == -1)
        {
            OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
            oS.ActionScript.Scripts.Clear();
            oS.ActionScript.reDelay = 1.0f;
            IsReady = true;
            Screen.TextBox.reDelay = 1.0f;
            ActionScript.TempInputDirection = -1;
        }
    }

    private void Register()
    {
        ActionScript.RegisterID(Value);
        IsReady = true;
    }

    private void Unregister()
    {
        ActionScript.UnregisterID(Value);
        IsReady = true;
    }

    private void GetAchievement()
    {
        String indiciesData = Value.GetSplit(0, "|");
        indiciesData = indiciesData.Remove(0, 1);
        indiciesData = indiciesData.Remove(indiciesData.Length - 1, 1);
        String[] stringIndicies = indiciesData.Split(',');
        List<int> indicies = [];
        for (int i = 0; i <= stringIndicies.Length - 1; i++)
        {
            indicies.Add(int.Parse(stringIndicies[i]));
        }

        IsReady = true;
    }

    private void DoScript()
    {
        if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
        {
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(Value, 0);
        }
    }

    private void DoCamera()
    {
        OverworldCamera c = (OverworldCamera)Screen.Camera;

        if (c.ThirdPerson == true)
        {
            String action = Value.GetSplit(0);
            switch (action.ToLower())
            {
                case "set":
                {
                    float x = float.Parse(Value.GetSplit(1).Replace(".", GameController.DecSeparator));
                    float y = float.Parse(Value.GetSplit(2).Replace(".", GameController.DecSeparator));
                    float z = float.Parse(Value.GetSplit(3).Replace(".", GameController.DecSeparator));
                    float yaw = float.Parse(Value.GetSplit(4).Replace(".", GameController.DecSeparator));
                    float pitch = float.Parse(Value.GetSplit(5).Replace(".", GameController.DecSeparator));
                    c.ThirdPersonOffset = new Vector3(x, y, z);
                    c.Yaw = yaw;
                    c.Pitch = pitch;
                    break;
                }
                case "reset":
                    c.ThirdPersonOffset = new Vector3(0.0f, 0.3f, 1.5f);
                    break;
                case "yaw":
                {
                    float yaw = float.Parse(Value.GetSplit(1).Replace(",", ".").Replace(".", GameController.DecSeparator));
                    c.Yaw = yaw;
                    break;
                }
                case "pitch":
                {
                    float pitch = float.Parse(Value.GetSplit(1).Replace(",", ".").Replace(".", GameController.DecSeparator));
                    c.Pitch = pitch;
                    break;
                }
                case "position":
                {
                    float x = float.Parse(Value.GetSplit(1).Replace(".", GameController.DecSeparator));
                    float y = float.Parse(Value.GetSplit(2).Replace(".", GameController.DecSeparator));
                    float z = float.Parse(Value.GetSplit(3).Replace(".", GameController.DecSeparator));
                    c.ThirdPersonOffset = new Vector3(x, y, z);
                    break;
                }
                case "x":
                {
                    float x = float.Parse(Value.GetSplit(1).Replace(".", GameController.DecSeparator));
                    Vector3 offset = c.ThirdPersonOffset;
                    offset.X = x;
                    c.ThirdPersonOffset = offset;
                    break;
                }
                case "y":
                {
                    float y = float.Parse(Value.GetSplit(1).Replace(".", GameController.DecSeparator));
                    Vector3 offset = c.ThirdPersonOffset;
                    offset.Y = y;
                    c.ThirdPersonOffset = offset;
                    break;
                }
                case "z":
                {
                    float z = float.Parse(Value.GetSplit(1).Replace(".", GameController.DecSeparator));
                    Vector3 offset = c.ThirdPersonOffset;
                    offset.Z = z;
                    c.ThirdPersonOffset = offset;
                    break;
                }
            }

            c.UpdateThirdPersonCamera();
            c.UpdateFrustum();
            c.UpdateViewMatrix();
            Screen.Level.UpdateEntities();
            Screen.Level.Entities = Screen.Level.Entities.OrderByDescending(e => e.CameraDistance).ToList();
            Screen.Level.UpdateEntities();
        }

        IsReady = true;
    }

    private void DoText()
    {
        Screen.TextBox.reDelay = 0.0f;
        Screen.TextBox.Show(Value, []);
        IsReady = true;
    }

    private void DoSelect()
    {
        String condition = String.Empty;
        String check = Value;

        if (Value.Contains("(") == true && Value.Contains(")") == true)
        {
            condition = Value.Remove(0, Value.IndexOf('(') + 1);
            condition = condition.Remove(condition.LastIndexOf(')'));
            check = Value.Remove(Value.IndexOf('('));
        }

        switch (check.ToLower())
        {
            case "random":
                check = Core.Random.Next(1, int.Parse(condition) + 1).ToString();
                break;
        }

        OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;

        ActionScript.CSL().WhenIndex += 1;

        oS.ActionScript.Switch(check);

        IsReady = true;
    }

    private void DoOptions()
    {
        Screen.TextBox.Showing = true;
        String[] options = Value.Split(',');

        for (int i = 0; i <= options.Length - 1; i++)
        {
            if (i <= options.Length - 1)
            {
                String flag = options[i];
                bool removeFlag = false;

                switch (flag)
                {
                    case "[TEXT=FALSE]":
                        removeFlag = true;
                        Screen.TextBox.Showing = false;
                        break;
                }

                if (removeFlag == true)
                {
                    List<String> l = [..options];
                    l.RemoveAt(i);
                    options = [..l];
                    i -= 1;
                }
            }
        }
        Screen.ChooseBox.Show(options, 0, true);

        ActionScript.CSL().WhenIndex += 1;

        IsReady = true;
    }

    private void DoIf()
    {
        String condition = String.Empty;
        String check = Value;

        if (Value.Contains("(") == true && Value.Contains(")") == true)
        {
            condition = Value.Remove(0, Value.IndexOf('(') + 1);
            condition = condition.Remove(condition.LastIndexOf(')'));
            check = Value.Remove(Value.IndexOf('('));
        }

        OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;

        bool t = false;
        bool inverse = false;

        if (check.StartsWith("not ") == true)
        {
            check = check.Remove(0, 4);
            inverse = true;
        }

        switch (check.ToLower())
        {
            case "register":
                t = ActionScript.IsRegistered(condition);
                break;
            case "daytime":
                if ((World.DayTimes)int.Parse(condition) == World.GetTime())
                {
                    t = true;
                }
                break;
            case "freeplaceinparty":
                if (Core.Player.Pokemons.Count < 6)
                {
                    t = true;
                }
                break;
            case "season":
                if (int.Parse(condition) == (int)World.CurrentSeason)
                {
                    t = true;
                }
                break;
            case "nopokemon":
                if (Core.Player.Pokemons.Count == 0)
                {
                    t = true;
                }
                break;
            case "countpokemon":
                if (Core.Player.Pokemons.Count == int.Parse(condition))
                {
                    t = true;
                }
                break;
            case "day":
                if (int.Parse(condition) == (int)DateTime.Now.DayOfWeek)
                {
                    t = true;
                }
                break;
            case "aurora":
                if (condition.Equals("0") == true)
                {
                    t = World.IsAurora == false;
                }
                else
                {
                    t = World.IsAurora;
                }
                break;
            case "random":
                if (Core.Random.Next(0, int.Parse(condition)) == 0)
                {
                    t = true;
                }
                break;
            case "position":
            {
                String[] positionValues = condition.Split(',');
                Vector3 checkPosition = new Vector3(
                    (int)Screen.Camera.Position.X,
                    (int)Screen.Camera.Position.Y,
                    (int)Screen.Camera.Position.Z);

                if (positionValues[0].ToLower().Equals("player") == false)
                {
                    int targetID = int.Parse(positionValues[0]);
                    checkPosition = Screen.Level.GetNPC(targetID).Position;
                }

                Vector3 p = new Vector3(
                    float.Parse(positionValues[1]),
                    float.Parse(positionValues[2]),
                    float.Parse(positionValues[3]));

                t = p == checkPosition;
                break;
            }
            case "weather":
                t = (World.Weathers)int.Parse(condition) == Screen.Level.World.CurrentMapWeather;
                break;
            case "regionweather":
                t = int.Parse(condition) == (int)World.GetCurrentRegionWeather();
                break;
            case "hasbadge":
                t = Core.Player.Badges.Contains(int.Parse(condition));
                break;
            case "hasitem":
                t = Core.Player.Inventory.GetItemAmount(condition) > 0;
                break;
            case "haspokemon":
                foreach (Pokemon p in Core.Player.Pokemons)
                {
                    if (p.Number == int.Parse(condition))
                    {
                        t = true;
                        break;
                    }
                }
                break;
        }

        if (inverse == true)
        {
            t = t == false;
        }

        ActionScript.CSL().WaitingEndIf[ActionScript.CSL().IfIndex + 1] = false;
        ActionScript.CSL().CanTriggerElse[ActionScript.CSL().IfIndex + 1] = false;

        oS.ActionScript.ChooseIf(t);

        IsReady = true;
    }

    private void DoWait()
    {
        if (int.Parse(Value) > 0)
        {
            Value = (int.Parse(Value) - 1).ToString();
        }
        if (int.Parse(Value) <= 0)
        {
            IsReady = true;
        }
    }

    private void DoAction()
    {
        String valueLower = Value.ToLower();
        if (valueLower.Equals("storagesystem") == true)
        {
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new StorageSystemScreen(Core.CurrentScreen), Microsoft.Xna.Framework.Color.Black, false));
        }
        else if (valueLower.Equals("apricornkurt") == true)
        {
            Core.SetScreen(new ApricornScreen(Core.CurrentScreen, "Kurt"));
        }
        else if (valueLower.StartsWith("trade(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            String storeData = Value.GetSplit(0);
            bool canBuy = bool.Parse(Value.GetSplit(1));
            bool canSell = bool.Parse(Value.GetSplit(2));

            String currencyIndicator = "P";
            if (Value.CountSplits() > 3)
            {
                currencyIndicator = Value.GetSplit(3);
            }

            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new TradeScreen(Core.CurrentScreen, storeData, canBuy, canSell, currencyIndicator, ""), Microsoft.Xna.Framework.Color.Black, false));
        }
        else if (valueLower.StartsWith("getpokemon(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            int commas = 0;
            foreach (char c in Value)
            {
                if (c == ',')
                {
                    commas += 1;
                }
            }

            int pokemonID = int.Parse(Value.GetSplit(0));
            int level = int.Parse(Value.GetSplit(1));

            String catchMethod = "random reason";
            if (commas > 1)
            {
                catchMethod = Value.GetSplit(2);
            }

            Items.Item catchBall = Items.Item.GetItemByID("1")!;
            if (commas > 2)
            {
                catchBall = Items.Item.GetItemByID(Value.GetSplit(3))!;
            }

            String catchLocation = Screen.Level.MapName;
            if (commas > 3)
            {
                catchLocation = Value.GetSplit(4);
            }

            bool isEgg = false;
            if (commas > 4)
            {
                isEgg = bool.Parse(Value.GetSplit(5));
            }

            String catchTrainer = Core.Player.Name;
            if (commas > 5 && Value.GetSplit(6).Equals("<playername>") == false && Value.GetSplit(6).Equals("<player.name>") == false)
            {
                catchTrainer = Value.GetSplit(6);
            }

            Pokemon pokemon = Pokemon.GetPokemonByID(pokemonID);
            pokemon.Generate(level, true);

            pokemon.CatchTrainerName = catchTrainer;
            pokemon.OT = Core.Player.OT;
            pokemon.CatchLocation = catchLocation;
            pokemon.catchBall = catchBall;
            pokemon.CatchMethod = catchMethod;

            if (isEgg == true)
            {
                pokemon.EggSteps = 1;
                pokemon.SetCatchInfos(Items.Item.GetItemByID("5")!, Localization.GetString("CatchMethod_Obtained", "Obtained at"));
            }
            else
            {
                pokemon.EggSteps = 0;
            }

            Core.Player.Pokemons.Add(pokemon);

            int pokedexType = 2;
            if (pokemon.IsShiny == true)
            {
                pokedexType = 3;
            }

            String dexID = PokemonForms.GetPokemonDataFileName(pokemon.Number, pokemon.AdditionalData);
            if (dexID.Contains("_") == false)
            {
                if (PokemonForms.GetAdditionalDataForms(pokemon.Number) != null &&
                    PokemonForms.GetAdditionalDataForms(pokemon.Number)!.Contains(pokemon.AdditionalData) == true)
                {
                    dexID = pokemon.Number + ";" + pokemon.AdditionalData;
                }
                else
                {
                    dexID = pokemon.Number.ToString();
                }
            }

            Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, pokedexType);
        }
        else if (valueLower.StartsWith("townmap,") == true)
        {
            String startRegion = Value.GetSplit(1);
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, startRegion, ["view"]), Microsoft.Xna.Framework.Color.Black, false));
        }
        else if (valueLower.Equals("opendonation") == true)
        {
            Core.SetScreen(new DonationScreen(Core.CurrentScreen));
        }
        else if (valueLower.Equals("receivepokedex") == true)
        {
            Core.Player.HasPokedex = true;
            foreach (Pokemon p in Core.Player.Pokemons)
            {
                int i = 2;
                if (p.IsShiny == true)
                {
                    i = 3;
                }
                String dexID = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);
                Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, i);
            }
        }
        else if (valueLower.Equals("receivepokegear") == true)
        {
            Core.Player.HasPokegear = true;
        }
        else if (valueLower.StartsWith("renamepokemon(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            String index = Value;
            bool renameOTcheck = false;
            bool canRename = true;

            if (Value.Contains(",") == true)
            {
                index = Value.GetSplit(0);
                renameOTcheck = bool.Parse(Value.GetSplit(1));
            }

            int pokemonIndex = 0;
            if (StringHelper.IsNumeric(index) == true)
            {
                pokemonIndex = int.Parse(index);
            }
            else
            {
                if (index.ToLower().Equals("last") == true)
                {
                    pokemonIndex = Core.Player.Pokemons.Count - 1;
                }
            }

            if (renameOTcheck == true)
            {
                if (Core.Player.Pokemons[pokemonIndex].OT.Equals(Core.Player.OT) == false)
                {
                    canRename = false;
                }
            }

            if (canRename == true)
            {
                Core.SetScreen(new NameObjectScreen(Core.CurrentScreen, Core.Player.Pokemons[pokemonIndex]));
            }
            else
            {
                Screen.TextBox.Show("I cannot rename this~Pokémon because the~OT is different!*Did you receive it in~a trade or something?", [], true, false);
            }
        }
        else if (valueLower.Equals("renamerival") == true)
        {
            Core.SetScreen(new NameObjectScreen(Core.CurrentScreen, TextureManager.GetTexture(@"NPC\4", new Rectangle(0, 64, 32, 32), ""), false, false, "rival", "Silver", Script.NameRival));
        }
        else if (valueLower.StartsWith("playcry(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            Pokemon p = Pokemon.GetPokemonByID(int.Parse(Value));
            p.PlayCry();
        }
        else if (valueLower.StartsWith("showoopokemon(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            Screen.Level.OverworldPokemon.Visible = bool.Parse(Value);
        }
        else if (valueLower.Equals("togglethirdperson") == true)
        {
            if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
            {
                OverworldCamera c = (OverworldCamera)Screen.Camera;
                c.SetThirdPerson(c.ThirdPerson == false, false);
                c.UpdateFrustum();
                c.UpdateViewMatrix();
                Screen.Level.UpdateEntities();
                Screen.Level.Entities = Screen.Level.Entities.OrderByDescending(e => e.CameraDistance).ToList();
                Screen.Level.UpdateEntities();
            }
        }
        else if (valueLower.Equals("activatethirdperson") == true)
        {
            if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
            {
                OverworldCamera c = (OverworldCamera)Screen.Camera;
                c.SetThirdPerson(true, false);
                c.UpdateFrustum();
                c.UpdateViewMatrix();
                Screen.Level.UpdateEntities();
                Screen.Level.Entities = Screen.Level.Entities.OrderByDescending(e => e.CameraDistance).ToList();
                Screen.Level.UpdateEntities();
            }
        }
        else if (valueLower.Equals("deactivatethirdperson") == true)
        {
            if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
            {
                OverworldCamera c = (OverworldCamera)Screen.Camera;
                c.SetThirdPerson(false, false);
                c.UpdateFrustum();
                c.UpdateViewMatrix();
                Screen.Level.UpdateEntities();
                Screen.Level.Entities = Screen.Level.Entities.OrderByDescending(e => e.CameraDistance).ToList();
                Screen.Level.UpdateEntities();
            }
        }
        else if (valueLower.StartsWith("setfont(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            switch (Value.ToLower())
            {
                case "standard":
                    Screen.TextBox.TextFont = FontManager.GetFontContainer("textfont");
                    break;
                case "unown":
                    Screen.TextBox.TextFont = FontManager.GetFontContainer("unown");
                    break;
            }
        }
        else if (valueLower.StartsWith("setrenderdistance(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            switch (Value.ToLower())
            {
                case "0":
                case "tiny":
                    Core.GameOptions.RenderDistance = 0;
                    break;
                case "1":
                case "small":
                    Core.GameOptions.RenderDistance = 1;
                    break;
                case "2":
                case "normal":
                    Core.GameOptions.RenderDistance = 2;
                    break;
                case "3":
                case "far":
                    Core.GameOptions.RenderDistance = 3;
                    break;
                case "4":
                case "extreme":
                    Core.GameOptions.RenderDistance = 4;
                    break;
            }

            Screen.Level.World = new World(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
        }
        else if (valueLower.StartsWith("wearskin(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            String textureID = Value;
            Logger.Debug(textureID);
            Screen.Level.OwnPlayer.Textures = [P3D.TextureManager.GetTexture(@"Textures\NPC\" + textureID)];
            Screen.Level.OwnPlayer.SkinName = textureID;
            Screen.Level.OwnPlayer.UpdateEntity();
        }
        else if (valueLower.Equals("toggledarkness") == true)
        {
            Screen.Level.IsDark = Screen.Level.IsDark == false;
        }
        else if (valueLower.StartsWith("globalhub") == true || valueLower.StartsWith("friendhub") == true)
        {
            if ((GameJolt.API.LoggedIn == true && Core.Player.IsGameJoltSave == true) || GameController.IS_DEBUG_ACTIVE == true)
            {
                if (GameJolt.LogInScreen.UserBanned(Core.GameJoltSave.GameJoltID) == false)
                {
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new GameJolt.GTSMainScreen(Core.CurrentScreen), Microsoft.Xna.Framework.Color.Black, false));
                }
                else
                {
                    Screen.TextBox.Show("This GameJolt account~(" + Core.GameJoltSave.GameJoltID + ") is banned~from the GTS!", [], false, false, Microsoft.Xna.Framework.Color.Red);
                }
            }
            else
            {
                Screen.TextBox.Show("You are not using~your GameJolt profile.*Please connect to GameJolt~and switch to the GameJolt~profile to enable the GTS.*You can do this by going~back to the main menu~and choosing \"Play online\".", [], false, false, Microsoft.Xna.Framework.Color.Red);
            }
        }
        else if (valueLower.StartsWith("gamejoltlogin") == true)
        {
            Core.SetScreen(new GameJolt.LogInScreen(Core.CurrentScreen));
        }
        else if (valueLower.StartsWith("readpokemon(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            Pokemon p = Core.Player.Pokemons[int.Parse(Value)];

            String message = "Hm... I see your~" + p.GetDisplayName();
            String addMessage = "~is very stable with~";

            if (p.EVAttack > p.EVDefense && p.EVAttack > p.EVHP && p.EVAttack > p.EVSpAttack && p.EVAttack > p.EVSpDefense && p.EVAttack > p.EVSpeed)
            {
                addMessage += "performing physical moves.";
            }
            if (p.EVDefense > p.EVAttack && p.EVDefense > p.EVHP && p.EVDefense > p.EVSpAttack && p.EVDefense > p.EVSpDefense && p.EVDefense > p.EVSpeed)
            {
                addMessage += "taking hits.";
            }
            if (p.EVHP > p.EVAttack && p.EVHP > p.EVDefense && p.EVHP > p.EVSpAttack && p.EVHP > p.EVSpDefense && p.EVHP > p.EVSpeed)
            {
                addMessage += "taking damage.";
            }
            if (p.EVSpAttack > p.EVAttack && p.EVSpAttack > p.EVDefense && p.EVSpAttack > p.EVHP && p.EVSpAttack > p.EVSpDefense && p.EVSpAttack > p.EVSpeed)
            {
                addMessage += "performing complex strategies.";
            }
            if (p.EVSpDefense > p.EVAttack && p.EVSpDefense > p.EVDefense && p.EVSpDefense > p.EVHP && p.EVSpDefense > p.EVSpAttack && p.EVSpDefense > p.EVSpeed)
            {
                addMessage += "breaking strategies.";
            }
            if (p.EVSpeed > p.EVAttack && p.EVSpeed > p.EVDefense && p.EVSpeed > p.EVHP && p.EVSpeed > p.EVSpAttack && p.EVSpeed > p.EVSpDefense)
            {
                addMessage += "speeding the others out.";
            }

            if (addMessage.Equals("~is very stable with~") == true)
            {
                addMessage = "~is very well balanced.";
            }

            message += addMessage;
            message += "*...~...*What that means?~I am not sure...";

            Screen.TextBox.Show(message, [], false, false);
        }
        else if (valueLower.StartsWith("achieveemblem(") == true)
        {
            Value = Value.Remove(0, Value.IndexOf('(') + 1);
            Value = Value.Remove(Value.Length - 1, 1);

            GameJolt.Emblem.AchieveEmblem(Value);
        }

        IsReady = true;
    }

    private void DoMusic()
    {
        MusicManager.Play(Value, true);

        if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
        {
            Screen.Level.MusicLoop = Value;
        }

        IsReady = true;
    }

    private void DoSound()
    {
        String sound = Value;
        bool stopMusic = false;

        if (Value.Contains(",") == true)
        {
            sound = Value.GetSplit(0);
            stopMusic = bool.Parse(Value.GetSplit(1));
        }

        SoundManager.PlaySound(sound, stopMusic);

        IsReady = true;
    }

    private void DoMessageBulb()
    {
        if (started == false)
        {
            started = true;
            String[] data = Value.Split('|');

            int id = int.Parse(data[0]);
            Vector3 position = new Vector3(
                float.Parse(data[1].Replace(".", GameController.DecSeparator)),
                float.Parse(data[2].Replace(".", GameController.DecSeparator)),
                float.Parse(data[3].Replace(".", GameController.DecSeparator)));

            MessageBulb.NotificationTypes noType;
            switch (id)
            {
                case 0: noType = MessageBulb.NotificationTypes.Waiting; break;
                case 1: noType = MessageBulb.NotificationTypes.Exclamation; break;
                case 2: noType = MessageBulb.NotificationTypes.Shouting; break;
                case 3: noType = MessageBulb.NotificationTypes.Question; break;
                case 4: noType = MessageBulb.NotificationTypes.Note; break;
                case 5: noType = MessageBulb.NotificationTypes.Heart; break;
                case 6: noType = MessageBulb.NotificationTypes.Unhappy; break;
                case 7: noType = MessageBulb.NotificationTypes.Happy; break;
                case 8: noType = MessageBulb.NotificationTypes.Friendly; break;
                case 9: noType = MessageBulb.NotificationTypes.Poisoned; break;
                default: noType = MessageBulb.NotificationTypes.Exclamation; break;
            }

            Screen.Level.Entities.Add(new MessageBulb(position, noType));
        }

        bool contains = false;
        Screen.Level.Entities = Screen.Level.Entities.OrderByDescending(e => e.CameraDistance).ToList();
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.Equals("MessageBulb") == true)
            {
                e.Update();
                contains = true;
            }
        }
        if (contains == false)
        {
            IsReady = true;
        }
        else
        {
            for (int i = 0; i <= Screen.Level.Entities.Count - 1; i++)
            {
                if (i <= Screen.Level.Entities.Count - 1)
                {
                    if (Screen.Level.Entities[i].CanBeRemoved == true)
                    {
                        Screen.Level.Entities.RemoveAt(i);
                        i -= 1;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void DoBattle()
    {
        String actionValue = Value.GetSplit(0);
        switch (actionValue.ToLower())
        {
            case "trainer":
            {
                String id = Value.GetSplit(1);
                BattleSystem.Trainer t = new BattleSystem.Trainer(id);

                if (Value.CountSeperators(",") > 1)
                {
                    foreach (String v in Value.Split(','))
                    {
                        switch (v)
                        {
                            case "generate_pokemon_tower":
                            {
                                int level = 0;
                                foreach (Pokemon p in Core.Player.Pokemons)
                                {
                                    if (p.Level > level)
                                    {
                                        level = p.Level;
                                    }
                                }

                                String levelStr = level.ToString();
                                while (levelStr[levelStr.Length - 1] != '0')
                                {
                                    level += 1;
                                    levelStr = level.ToString();
                                }
                                break;
                            }
                        }
                    }
                }

                BattleSystem.BattleScreen b = new BattleSystem.BattleScreen(t, Core.CurrentScreen, 0);
                Core.SetScreen(new BattleIntroScreen(Core.CurrentScreen, b, t, t.GetIniMusicName(), t.IntroType));
                break;
            }
            case "wild":
            {
                int id = int.Parse(Value.GetSplit(1));
                int level = int.Parse(Value.GetSplit(2));

                Pokemon p = Pokemon.GetPokemonByID(id);
                p.Generate(level, true);

                String dexID = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);
                Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 1);

                BattleSystem.BattleScreen b = new BattleSystem.BattleScreen(p, Core.CurrentScreen, Spawner.EncounterMethods.Land);
                Core.SetScreen(new BattleIntroScreen(Core.CurrentScreen, b, Core.Random.Next(0, 10)));
                break;
            }
        }
        IsReady = true;
    }

    private void DoTrainerBattle()
    {
        BattleSystem.Trainer t = new BattleSystem.Trainer(Value);
        if (t.IsBeaten() == false)
        {
            if (started == false)
            {
                ((OverworldScreen)Core.CurrentScreen).TrainerEncountered = true;
                MusicManager.Play(t.GetInSightMusic(), true);
                if (t.IntroMessage.Equals("") == false)
                {
                    Screen.TextBox.reDelay = 0.0f;
                    Screen.TextBox.Show(t.IntroMessage, []);
                }
                started = true;
            }

            if (Screen.TextBox.Showing == false)
            {
                ((OverworldScreen)Core.CurrentScreen).TrainerEncountered = false;

                BattleSystem.BattleScreen b = new BattleSystem.BattleScreen(new BattleSystem.Trainer(Value), Core.CurrentScreen, 0);
                Core.SetScreen(new BattleIntroScreen(Core.CurrentScreen, b, t, t.GetIniMusicName(), t.IntroType));
            }
        }
        else
        {
            Screen.TextBox.reDelay = 0.0f;
            Screen.TextBox.Show(t.DefeatMessage, []);

            IsReady = true;
        }

        if (Screen.TextBox.Showing == false)
        {
            IsReady = true;
        }
    }

    private void DoPokemon()
    {
        String command = Value;
        String argument = String.Empty;

        if (command.Contains("(") == true && command.EndsWith(")") == true)
        {
            argument = command.Remove(0, command.IndexOf('(') + 1);
            argument = argument.Remove(argument.Length - 1, 1);
            command = command.Remove(command.IndexOf('('));
        }

        switch (command.ToLower())
        {
            case "cry":
            {
                int pokemonID = int.Parse(argument);
                Pokemon p = Pokemon.GetPokemonByID(pokemonID);
                p.PlayCry();
                break;
            }
            case "remove":
            {
                int index = int.Parse(argument);
                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Logger.Debug("Remove Pokémon (" + Core.Player.Pokemons[index].GetDisplayName() + ") at index " + index);
                    Core.Player.Pokemons.RemoveAt(index);
                }
                break;
            }
            case "add":
            {
                int commas = 0;
                foreach (char c in argument)
                {
                    if (c == ',')
                    {
                        commas += 1;
                    }
                }

                int pokemonID = int.Parse(argument.GetSplit(0));
                int level = int.Parse(argument.GetSplit(1));

                String catchMethod = "random reason";
                if (commas > 1)
                {
                    catchMethod = argument.GetSplit(2);
                }

                Items.Item catchBall = Items.Item.GetItemByID("1")!;
                if (commas > 2)
                {
                    catchBall = Items.Item.GetItemByID(argument.GetSplit(3))!;
                }

                String catchLocation = Screen.Level.MapName;
                if (commas > 3)
                {
                    catchLocation = argument.GetSplit(4);
                }

                bool isEgg = false;
                if (commas > 4)
                {
                    isEgg = bool.Parse(argument.GetSplit(5));
                }

                String catchTrainer = Core.Player.Name;
                if (commas > 5 && argument.GetSplit(6).Equals("<playername>") == false)
                {
                    catchTrainer = argument.GetSplit(6);
                }

                Pokemon pokemon = Pokemon.GetPokemonByID(pokemonID);
                pokemon.Generate(level, true);

                pokemon.CatchTrainerName = catchTrainer;
                pokemon.OT = Core.Player.OT;
                pokemon.CatchLocation = catchLocation;
                pokemon.catchBall = catchBall;
                pokemon.CatchMethod = catchMethod;

                if (isEgg == true)
                {
                    pokemon.EggSteps = 1;
                    pokemon.SetCatchInfos(Items.Item.GetItemByID("5")!, Localization.GetString("CatchMethod_Obtained", "Obtained at"));
                }
                else
                {
                    pokemon.EggSteps = 0;
                }

                Core.Player.Pokemons.Add(pokemon);

                int pokedexType = 2;
                if (pokemon.IsShiny == true)
                {
                    pokedexType = 3;
                }

                String dexID = PokemonForms.GetPokemonDataFileName(pokemon.Number, pokemon.AdditionalData);
                if (dexID.Contains("_") == false)
                {
                    if (PokemonForms.GetAdditionalDataForms(pokemon.Number) != null &&
                        PokemonForms.GetAdditionalDataForms(pokemon.Number)!.Contains(pokemon.AdditionalData) == true)
                    {
                        dexID = pokemon.Number + ";" + pokemon.AdditionalData;
                    }
                    else
                    {
                        dexID = pokemon.Number.ToString();
                    }
                }

                Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, pokedexType);
                break;
            }
            case "setadditionalvalue":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                String additionalValue = argument.GetSplit(1, ",");

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Core.Player.Pokemons[index].AdditionalData = additionalValue;
                }
                break;
            }
            case "setnickname":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                String nickName = argument.GetSplit(1, ",");

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Core.Player.Pokemons[index].NickName = nickName;
                }
                break;
            }
            case "setstat":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                String stat = argument.GetSplit(1, ",");
                int statValue = int.Parse(argument.GetSplit(2, ","));

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Pokemon p = Core.Player.Pokemons[index];
                    switch (stat.ToLower())
                    {
                        case "maxhp":
                        case "hp":
                            p.MaxHP = statValue;
                            break;
                        case "chp":
                            p.HP = statValue;
                            break;
                        case "atk":
                        case "attack":
                            p.Attack = statValue;
                            break;
                        case "def":
                        case "defense":
                            p.Defense = statValue;
                            break;
                        case "spatk":
                        case "specialattack":
                        case "spattack":
                            p.SpAttack = statValue;
                            break;
                        case "spdef":
                        case "specialdefense":
                        case "spdefense":
                            p.SpDefense = statValue;
                            break;
                        case "speed":
                            p.Speed = statValue;
                            break;
                    }
                }
                break;
            }
            case "clear":
                Core.Player.Pokemons.Clear();
                break;
            case "removeattack":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                int attackIndex = int.Parse(argument.GetSplit(1, ","));

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Pokemon p = Core.Player.Pokemons[index];
                    if (p.Attacks.Count - 1 >= attackIndex)
                    {
                        p.Attacks.RemoveAt(attackIndex);
                    }
                }
                break;
            }
            case "clearattacks":
            {
                int index = int.Parse(argument);
                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Core.Player.Pokemons[index].Attacks.Clear();
                }
                break;
            }
            case "addattack":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                int attackID = int.Parse(argument.GetSplit(1, ","));

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Pokemon p = Core.Player.Pokemons[index];
                    if (p.Attacks.Count < 4)
                    {
                        BattleSystem.Attack newAttack = BattleSystem.Attack.GetAttackByID(attackID);
                        p.Attacks.Add(newAttack);
                    }
                }
                break;
            }
            case "setshiny":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                bool isShiny = bool.Parse(argument.GetSplit(1, ","));

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Core.Player.Pokemons[index].IsShiny = isShiny;
                }
                break;
            }
            case "changelevel":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                int newLevel = int.Parse(argument.GetSplit(1, ","));

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Core.Player.Pokemons[index].Level = newLevel;
                }
                break;
            }
            case "gainexp":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                int exp = int.Parse(argument.GetSplit(1, ","));

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Core.Player.Pokemons[index].Experience += exp;
                }
                break;
            }
            case "setnature":
            {
                int index = int.Parse(argument.GetSplit(0, ","));
                Pokemon.Natures nature = Pokemon.ConvertIDToNature(int.Parse(argument.GetSplit(1, ",")));

                if (Core.Player.Pokemons.Count - 1 >= index)
                {
                    Core.Player.Pokemons[index].Nature = nature;
                }
                break;
            }
            case "npctrade":
            {
                String[] splits = argument.Split('|');
                Script.SaveNPCTrade = splits;

                PartyScreen partyScreen = new PartyScreen(Core.CurrentScreen, Items.Item.GetItemByID("5")!, index => { Script.DoNPCTrade(index); return true; }, "Choose trade Pokémon", true);
                partyScreen.ExitedSub = Script.ExitedNPCTrade;
                Core.SetScreen(partyScreen);
                break;
            }
            case "hide":
                Screen.Level.OverworldPokemon.Visible = false;
                break;
        }

        IsReady = true;
    }

    private void DoNPC()
    {
        String command = Value;
        String argument = String.Empty;

        if (command.Contains("(") == true && command.EndsWith(")") == true)
        {
            argument = command.Remove(0, command.IndexOf('(') + 1);
            argument = argument.Remove(argument.Length - 1, 1);
            command = command.Remove(command.IndexOf('('));
        }

        switch (command.ToLower())
        {
            case "remove":
            {
                Entity targetNPC = Screen.Level.GetNPC(int.Parse(argument));
                Screen.Level.Entities.Remove(targetNPC);
                IsReady = true;
                break;
            }
            case "position":
            case "warp":
            {
                NPC targetNPC = Screen.Level.GetNPC(int.Parse(argument.GetSplit(0)));
                String[] positionData = argument.Split(',');
                targetNPC.Position = new Vector3(
                    float.Parse(positionData[1].Replace(".", GameController.DecSeparator)),
                    float.Parse(positionData[2].Replace(".", GameController.DecSeparator)),
                    float.Parse(positionData[3].Replace(".", GameController.DecSeparator)));
                targetNPC.CreatedWorld = false;
                IsReady = true;
                break;
            }
            case "register":
                NPC.AddNPCData(argument);
                IsReady = true;
                break;
            case "unregister":
                NPC.RemoveNPCData(argument);
                IsReady = true;
                break;
            case "wearskin":
            {
                String textureID = argument.GetSplit(1);
                NPC targetNPC = Screen.Level.GetNPC(int.Parse(argument.GetSplit(0)));
                targetNPC.SetupSprite(textureID, "", false);
                IsReady = true;
                break;
            }
            case "move":
            {
                NPC targetNPC = Screen.Level.GetNPC(int.Parse(argument.GetSplit(0)));
                int steps = int.Parse(argument.GetSplit(1));

                Screen.Level.UpdateEntities();
                if (started == false)
                {
                    targetNPC.Moved += steps;
                    started = true;
                }
                else
                {
                    if (targetNPC.Moved <= 0.0f)
                    {
                        IsReady = true;
                    }
                }
                break;
            }
            case "turn":
            {
                NPC targetNPC = Screen.Level.GetNPC(int.Parse(argument.GetSplit(0)));
                targetNPC.faceRotation = int.Parse(argument.GetSplit(1));
                targetNPC.Update();
                targetNPC.UpdateEntity();
                IsReady = true;
                break;
            }
            default:
                IsReady = true;
                break;
        }
    }

    private void DoPlayer()
    {
        String command = Value;
        String argument = String.Empty;

        if (command.Contains("(") == true && command.EndsWith(")") == true)
        {
            argument = command.Remove(0, command.IndexOf('(') + 1);
            argument = argument.Remove(argument.Length - 1, 1);
            command = command.Remove(command.IndexOf('('));
        }

        switch (command.ToLower())
        {
            case "wearskin":
                Screen.Level.OwnPlayer.SetTexture(argument, false);
                Screen.Level.OwnPlayer.UpdateEntity();
                IsReady = true;
                break;
            case "move":
                if (started == false)
                {
                    Screen.Camera.Move(float.Parse(argument));
                    started = true;
                    Screen.Level.OverworldPokemon.Visible = false;
                }
                else
                {
                    Screen.Level.UpdateEntities();
                    Screen.Camera.Update();
                    if (Screen.Camera.IsMoving == false)
                    {
                        IsReady = true;
                    }
                }
                break;
            case "turn":
                if (started == false)
                {
                    Screen.Camera.Turn(int.Parse(argument));
                    started = true;
                    Screen.Level.OverworldPokemon.Visible = false;
                }
                else
                {
                    Screen.Camera.Update();
                    Screen.Level.UpdateEntities();
                    if (Screen.Camera.Turning == false)
                    {
                        IsReady = true;
                    }
                }
                break;
            case "turnto":
                if (started == false)
                {
                    int turns = int.Parse(argument) - Screen.Camera.GetPlayerFacingDirection();
                    if (turns < 0)
                    {
                        turns = turns + 4;
                    }

                    if (turns > 0)
                    {
                        Screen.Camera.Turn(turns);
                        started = true;
                        Screen.Level.OverworldPokemon.Visible = false;
                    }
                    else
                    {
                        IsReady = true;
                    }
                }
                else
                {
                    Screen.Camera.Update();
                    Screen.Level.UpdateEntities();
                    if (Screen.Camera.Turning == false)
                    {
                        IsReady = true;
                    }
                }
                break;
            case "warp":
            {
                int commas = 0;
                foreach (char c in argument)
                {
                    if (c == ',')
                    {
                        commas += 1;
                    }
                }

                switch (commas)
                {
                    case 4:
                        Screen.Level.WarpData.WarpDestination = argument.GetSplit(0);
                        Screen.Level.WarpData.WarpPosition = new Vector3(
                            float.Parse(argument.GetSplit(1)),
                            float.Parse(argument.GetSplit(2).Replace(".", GameController.DecSeparator)),
                            float.Parse(argument.GetSplit(3)));
                        Screen.Level.WarpData.WarpRotations = int.Parse(argument.GetSplit(4));
                        Screen.Level.WarpData.DoWarpInNextTick = true;
                        Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                        break;
                    case 2:
                        Screen.Camera.Position = new Vector3(
                            float.Parse(argument.GetSplit(0)),
                            float.Parse(argument.GetSplit(1).Replace(".", GameController.DecSeparator)),
                            float.Parse(argument.GetSplit(2)));
                        break;
                }

                Screen.Level.OverworldPokemon.warped = true;
                Screen.Level.OverworldPokemon.Visible = false;
                IsReady = true;
                break;
            }
            case "stopmovement":
                Screen.Camera.StopMovement();
                IsReady = true;
                break;
            case "money":
                Core.Player.Money += int.Parse(argument);
                IsReady = true;
                break;
            case "setmovement":
            {
                String[] movements = argument.Split(',');
                Screen.Camera.PlannedMovement = new Vector3(
                    int.Parse(movements[0]),
                    int.Parse(movements[1]),
                    int.Parse(movements[2]));
                IsReady = true;
                break;
            }
            default:
                IsReady = true;
                break;
        }
    }

    private void DoEntity()
    {
        String command = Value;
        String argument = String.Empty;

        if (command.Contains("(") == true && command.EndsWith(")") == true)
        {
            argument = command.Remove(0, command.IndexOf('(') + 1);
            argument = argument.Remove(argument.Length - 1, 1);
            command = command.Remove(command.IndexOf('('));
        }

        int entID = int.Parse(argument.GetSplit(0));
        Entity? ent = Screen.Level.GetEntity(entID);

        if (ent != null)
        {
            switch (command.ToLower())
            {
                case "warp":
                {
                    List<String> positionList = [..argument.Split(',')];
                    Vector3 newPosition = new Vector3(
                        float.Parse(positionList[1].Replace(".", GameController.DecSeparator)),
                        float.Parse(positionList[2].Replace(".", GameController.DecSeparator)),
                        float.Parse(positionList[3].Replace(".", GameController.DecSeparator)));
                    ent.Position = newPosition;
                    ent.CreatedWorld = false;
                    break;
                }
                case "scale":
                {
                    List<String> scaleList = [..argument.Split(',')];
                    Vector3 newScale = new Vector3(
                        float.Parse(scaleList[1].Replace(".", GameController.DecSeparator)),
                        float.Parse(scaleList[2].Replace(".", GameController.DecSeparator)),
                        float.Parse(scaleList[3].Replace(".", GameController.DecSeparator)));
                    ent.Scale = newScale;
                    ent.CreatedWorld = false;
                    break;
                }
                case "remove":
                    ent.CanBeRemoved = true;
                    break;
                case "setid":
                    ent.ID = int.Parse(argument.GetSplit(1));
                    break;
                case "opacity":
                    ent.NormalOpacity = int.Parse(argument.GetSplit(1)) / 100.0f;
                    break;
                case "visible":
                    ent.Visible = bool.Parse(argument.GetSplit(1));
                    break;
                case "setadditionalvalue":
                    ent.AdditionalValue = argument.GetSplit(1);
                    break;
                case "collision":
                    ent.Collision = bool.Parse(argument.GetSplit(1));
                    break;
            }
        }

        IsReady = true;
    }

    private void DoEnvironment()
    {
        String command = Value;
        String argument = String.Empty;

        if (command.Contains("(") == true && command.EndsWith(")") == true)
        {
            argument = command.Remove(0, command.IndexOf('(') + 1);
            argument = argument.Remove(argument.Length - 1, 1);
            command = command.Remove(command.IndexOf('('));
        }

        switch (argument.ToLower())
        {
            case "changeweathertype":
                Screen.Level.WeatherType = int.Parse(argument);
                Screen.Level.World = new World(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
                break;
            case "changeenvironmenttype":
                Screen.Level.EnvironmentType = int.Parse(argument);
                Screen.Level.World = new World(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
                break;
            case "canfly":
                Screen.Level.CanFly = bool.Parse(argument);
                break;
            case "candig":
                Screen.Level.CanDig = bool.Parse(argument);
                break;
            case "canteleport":
                Screen.Level.CanTeleport = bool.Parse(argument);
                break;
            case "wildpokemongrass":
                Screen.Level.WildPokemonGrass = bool.Parse(argument);
                break;
            case "wildpokemonwater":
                Screen.Level.WildPokemonWater = bool.Parse(argument);
                break;
            case "wildpokemoneverywhere":
                Screen.Level.WildPokemonFloor = bool.Parse(argument);
                break;
            case "isdark":
                Screen.Level.IsDark = bool.Parse(argument);
                break;
            case "resetwalkedsteps":
                Screen.Level.WalkedSteps = 0;
                break;
        }

        IsReady = true;
    }

    private void DoLevel()
    {
        String command = Value;
        String argument = String.Empty;

        if (command.Contains("(") == true && command.EndsWith(")") == true)
        {
            argument = command.Remove(0, command.IndexOf('(') + 1);
            argument = argument.Remove(argument.Length - 1, 1);
            command = command.Remove(command.IndexOf('('));
        }

        switch (command.ToLower())
        {
            case "update":
                Screen.Level.Update();
                Screen.Level.UpdateEntities();
                Screen.Camera.Update();
                break;
        }

        IsReady = true;
    }

    private void ViewPokemonImage()
    {
        int pokemonID = int.Parse(Value.GetSplit(0));
        bool shiny = bool.Parse(Value.GetSplit(1));
        bool front = bool.Parse(Value.GetSplit(2));

        Screen.PokemonImageView.Show(pokemonID.ToString(), shiny, front);
        IsReady = true;
    }

    private void Move()
    {
        int targetID = int.Parse(Value.GetSplit(0));
        NPC targetNPC = Screen.Level.GetNPC(targetID);
        int moved = int.Parse(Value.GetSplit(1));

        Screen.Level.UpdateEntities();
        if (started == false)
        {
            targetNPC.Moved += moved;
            started = true;
        }
        else
        {
            if (targetNPC.Moved <= 0.0f)
            {
                IsReady = true;
            }
        }
    }

    private void MoveAsync()
    {
        int targetID = int.Parse(Value.GetSplit(0));
        NPC targetNPC = Screen.Level.GetNPC(targetID);
        int moved = int.Parse(Value.GetSplit(1));

        targetNPC.Moved += moved;
        started = true;
        IsReady = true;
    }

    private void MovePlayer()
    {
        if (started == false)
        {
            Screen.Camera.Move(float.Parse(Value));
            started = true;
            Screen.Level.OverworldPokemon.Visible = false;
        }
        else
        {
            Screen.Level.UpdateEntities();
            Screen.Camera.Update();
            if (Screen.Camera.IsMoving == false)
            {
                IsReady = true;
            }
        }
    }

    private void Turn()
    {
        int targetID = int.Parse(Value.GetSplit(0));
        NPC targetNPC = Screen.Level.GetNPC(targetID);

        targetNPC.faceRotation = int.Parse(Value.GetSplit(1));
        targetNPC.Update();
        targetNPC.UpdateEntity();
        IsReady = true;
    }

    private void Warp()
    {
        int targetID = int.Parse(Value.GetSplit(0));
        NPC targetNPC = Screen.Level.GetNPC(targetID);

        Vector3 targetPosition = new Vector3(
            int.Parse(Value.GetSplit(1)),
            int.Parse(Value.GetSplit(2)),
            int.Parse(Value.GetSplit(3)));
        targetNPC.Position = targetPosition;
        Logger.Debug(targetNPC.Position.ToString());
        targetNPC.Update();

        IsReady = true;
    }

    private void WarpPlayer()
    {
        int commas = 0;
        foreach (char c in Value)
        {
            if (c == ',')
            {
                commas += 1;
            }
        }

        switch (commas)
        {
            case 4:
                Screen.Level.WarpData.WarpDestination = Value.GetSplit(0);
                Screen.Level.WarpData.WarpPosition = new Vector3(
                    float.Parse(Value.GetSplit(1)),
                    float.Parse(Value.GetSplit(2).Replace(".", GameController.DecSeparator)),
                    float.Parse(Value.GetSplit(3)));
                Screen.Level.WarpData.WarpRotations = int.Parse(Value.GetSplit(4));
                Screen.Level.WarpData.DoWarpInNextTick = true;
                Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                break;
            case 2:
                Screen.Camera.Position = new Vector3(
                    float.Parse(Value.GetSplit(0)),
                    float.Parse(Value.GetSplit(1).Replace(".", GameController.DecSeparator)),
                    float.Parse(Value.GetSplit(2)));
                break;
        }

        Screen.Level.OverworldPokemon.Visible = false;

        IsReady = true;
    }

    private void Heal()
    {
        if (Value.Equals("") == true)
        {
            Core.Player.HealParty();
        }
        else
        {
            String[] data = Value.Split(',');
            List<int> members = [];
            foreach (String member in data)
            {
                members.Add(int.Parse(member));
            }
            Core.Player.HealParty([..members]);
        }

        IsReady = true;
    }

    private void TurnPlayer()
    {
        if (started == false)
        {
            Screen.Camera.Turn(int.Parse(Value));
            started = true;
            Screen.Level.OverworldPokemon.Visible = false;
        }
        else
        {
            Screen.Camera.Update();
            Screen.Level.UpdateEntities();
            if (Screen.Camera.Turning == false)
            {
                IsReady = true;
            }
        }
    }

    private void GiveItem()
    {
        String itemID = Value.GetSplit(0);
        Items.Item? item = Items.Item.GetItemByID(itemID);

        int amount = int.Parse(Value.GetSplit(1));

        String message;
        if (amount == 1)
        {
            message = "Received the~" + item?.OneLineName() + ".*" + Core.Player.Name + " stored it in the~" + item?.ItemType.ToString() + " pocket.";
        }
        else
        {
            message = "Received " + amount + "~" + item?.OneLinePluralName() + ".*" + Core.Player.Name + " stored them~in the " + item?.ItemType.ToString() + " pocket.";
        }

        Core.Player.Inventory.AddItem(itemID, amount);
        SoundManager.PlaySound("Receive_Item", true);

        Screen.TextBox.reDelay = 0.0f;
        Screen.TextBox.Show(message, []);

        IsReady = true;
    }

    private void RemoveItem()
    {
        String itemID = Value.GetSplit(0);
        Items.Item? item = Items.Item.GetItemByID(itemID);

        int amount = int.Parse(Value.GetSplit(1));

        String message;
        if (amount == 1)
        {
            message = "<player.name> handed over the~" + item?.OneLineName() + "!";
        }
        else
        {
            message = "<player.name> handed over the~" + item?.OneLinePluralName() + "!";
        }

        Core.Player.Inventory.RemoveItem(itemID, amount);

        Screen.TextBox.reDelay = 0.0f;
        Screen.TextBox.Show(message, []);

        IsReady = true;
    }

    private void GetBadge()
    {
        if (StringHelper.IsNumeric(Value) == true)
        {
            if (Core.Player.Badges.Contains(int.Parse(Value)) == false)
            {
                Core.Player.Badges.Add(int.Parse(Value));
                SoundManager.PlaySound("badge_acquired", true);
                Screen.TextBox.Show(Core.Player.Name + " received the~" + Badge.GetBadgeName(int.Parse(Value)) + "badge.", [], false, false);
                Core.Player.AddPoints(10, "Got a badge (V1 script!).");
            }
        }
        else
        {
            throw new Exception("Invalid argument exception");
        }

        IsReady = true;
    }
}
