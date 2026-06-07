using Microsoft.Xna.Framework;

namespace P3D.ScriptVersion2
{
    public static partial class ScriptCommander
    {
        private static void DoPokemon(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "cry":
                {
                    String pokID = argument.GetSplit(0);
                    String pokAD = "xXx";
                    if (pokID.Contains("_") == true) { pokAD = PokemonForms.GetAdditionalValueFromDataFile(pokID); pokID = pokID.GetSplit(0, "_"); }
                    if (pokID.Contains(";") == true) { pokAD = argument.GetSplit(0).GetSplit(1, ";"); pokID = argument.GetSplit(0).GetSplit(0, ";"); }
                    Pokemon p = Pokemon.GetPokemonByID(Int(pokID), pokAD, true);
                    p.PlayCry();
                    break;
                }
                case "remove":
                {
                    int index = Int(argument);
                    if (Core.Player.Pokemons.Count - 1 >= index)
                    {
                        Logger.Debug("Remove Pokémon (" + Core.Player.Pokemons[index].GetDisplayName() + ") at index " + index);
                        Core.Player.Pokemons.RemoveAt(index);
                    }
                    break;
                }
                case "add":
                {
                    if (argument.StartsWith("{") == true || (argument.Length > 1 && argument.Remove(0, 1).StartsWith(",{") == true))
                    {
                        int insertIndex = Core.Player.Pokemons.Count;
                        if (argument.Length > 1 && argument.Remove(0, 1).StartsWith(",{") == true) { insertIndex = Int(argument.GetSplit(0)); }
                        argument = argument.Remove(0, argument.IndexOf("{"));
                        Pokemon p = Pokemon.GetPokemonByData(argument.Replace("§", ",").Replace("«", "[").Replace("»", "]"));
                        Core.Player.Pokemons.Insert(insertIndex, p);
                        int dexType = p.IsShiny == true ? 3 : 2;
                        if (p.IsEgg == false) { Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true), dexType); }
                    }
                    else
                    {
                        int commas = 0; foreach (char c in argument) { if (c == ',') { commas++; } }
                        String pokID = argument.GetSplit(0); String pokAD = "xXx";
                        if (pokID.Contains("_") == true) { pokAD = PokemonForms.GetAdditionalValueFromDataFile(pokID); pokID = pokID.GetSplit(0, "_"); }
                        else if (pokID.Contains(";") == true) { pokAD = argument.GetSplit(0).GetSplit(1, ";"); pokID = argument.GetSplit(0).GetSplit(0, ";"); }
                        int level = Int(argument.GetSplit(1));
                        String catchMethod = Localization.GetString("CatchMethod_Empty", "Somehow obtained at");
                        if (commas > 1) { catchMethod = ScriptCommander.Parse(argument.GetSplit(2)).ToString() ?? catchMethod; }
                        Items.Item catchBall = Items.Item.GetItemByID("5")!;
                        if (commas > 2) { catchBall = Items.Item.GetItemByID(argument.GetSplit(3))!; }
                        String catchLocation = Localization.GetString("Places_" + Screen.Level.MapName, Screen.Level.MapName);
                        if (commas > 3) { catchLocation = ScriptCommander.Parse(argument.GetSplit(4)).ToString() ?? catchLocation; }
                        bool isEgg = false;
                        if (commas > 4) { isEgg = ScriptConversion.ToBoolean(argument.GetSplit(5)); }
                        String catchTrainer = Core.Player.Name;
                        if (commas > 5 && argument.GetSplit(6).Equals("<playername>") == false && argument.GetSplit(6).Equals("<player.name>") == false)
                        { catchTrainer = ScriptCommander.Parse(argument.GetSplit(6)).ToString() ?? catchTrainer; }
                        String heldItem = "0";
                        if (commas > 6) { heldItem = argument.GetSplit(7); }
                        bool isShiny = Core.Random.Next(0, Pokemon.MasterShinyRate + 1) == 0;
                        if (commas > 7) { isShiny = ScriptConversion.ToBoolean(argument.GetSplit(8)); }
                        Pokemon pokemon = Pokemon.GetPokemonByID(Int(pokID), pokAD);
                        pokemon.Generate(level, true, pokAD);
                        pokemon.CatchTrainerName = catchTrainer; pokemon.OT = Core.Player.OT;
                        pokemon.CatchLocation = catchLocation; pokemon.catchBall = catchBall; pokemon.CatchMethod = catchMethod;
                        if (isEgg == true) { pokemon.EggSteps = 1; pokemon.SetCatchInfos(Items.Item.GetItemByID("5")!, Localization.GetString("CatchMethod_Obtained", "Obtained at")); }
                        else { pokemon.EggSteps = 0; }
                        if (heldItem.Equals("0") == false) { pokemon.Item = Items.Item.GetItemByID(heldItem); }
                        pokemon.IsShiny = isShiny;
                        Core.Player.Pokemons.Add(pokemon);
                        int pdType = pokemon.IsShiny == true ? 3 : 2;
                        if (pokemon.IsEgg == false) { Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, PokemonForms.GetPokemonDataFileName(pokemon.Number, pokemon.AdditionalData, true), pdType); }
                    }
                    break;
                }
                case "setadditionalvalue": case "setadditionaldata":
                {
                    int index = Int(argument.GetSplit(0, ",")); String ad = argument.GetSplit(1, ",");
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].AdditionalData = ad; }
                    break;
                }
                case "setnickname":
                {
                    int index = Int(argument.GetSplit(0, ",")); String nick = argument.GetSplit(1, ",");
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].NickName = nick; }
                    break;
                }
                case "setstat":
                {
                    int index = Int(argument.GetSplit(0, ",")); String stat = argument.GetSplit(1, ","); int statValue = Int(argument.GetSplit(2, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index)
                    {
                        Pokemon p = Core.Player.Pokemons[index];
                        switch (stat.ToLower())
                        {
                            case "maxhp": case "hp": p.MaxHP = statValue; break;
                            case "chp": p.HP = statValue; break;
                            case "atk": case "attack": p.Attack = statValue; break;
                            case "def": case "defense": p.Defense = statValue; break;
                            case "spatk": case "specialattack": case "spattack": p.SpAttack = statValue; break;
                            case "spdef": case "specialdefense": case "spdefense": p.SpDefense = statValue; break;
                            case "speed": p.Speed = statValue; break;
                        }
                    }
                    break;
                }
                case "clear": Core.Player.Pokemons.Clear(); break;
                case "removeattack":
                {
                    int index = Int(argument.GetSplit(0, ",")); int atkIndex = Int(argument.GetSplit(1, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index && Core.Player.Pokemons[index].Attacks.Count - 1 >= atkIndex)
                        Core.Player.Pokemons[index].Attacks.RemoveAt(atkIndex);
                    break;
                }
                case "removeattackid":
                {
                    int index = Int(argument.GetSplit(0, ",")); int atkId = Int(argument.GetSplit(1, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index)
                    {
                        Pokemon p = Core.Player.Pokemons[index];
                        for (int a = 0; a < p.Attacks.Count; a++) { if (p.Attacks[a].ID == atkId) { p.Attacks.RemoveAt(a); break; } }
                    }
                    break;
                }
                case "clearattacks":
                { int index = Int(argument); if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].Attacks.Clear(); } break; }
                case "addattack":
                {
                    int index = Int(argument.GetSplit(0, ",")); int atkID = Int(argument.GetSplit(1, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index && Core.Player.Pokemons[index].Attacks.Count < 4)
                        Core.Player.Pokemons[index].Attacks.Add(BattleSystem.Attack.GetAttackByID(atkID));
                    break;
                }
                case "setshiny":
                {
                    int index = Int(argument.GetSplit(0, ",")); bool shiny = ScriptConversion.ToBoolean(argument.GetSplit(1, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].IsShiny = shiny; }
                    break;
                }
                case "setshinyall":
                { bool shiny = ScriptConversion.ToBoolean(argument.GetSplit(0, ",")); for (int i = 0; i < Core.Player.Pokemons.Count; i++) { Core.Player.Pokemons[i].IsShiny = shiny; } break; }
                case "changelevel":
                {
                    int index = Int(argument.GetSplit(0, ",")); int newLevel = Int(argument.GetSplit(1, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].Level = newLevel; }
                    break;
                }
                case "gainexp":
                {
                    int index = Int(argument.GetSplit(0, ",")); int exp = Int(argument.GetSplit(1, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].Experience += exp; }
                    break;
                }
                case "setnature":
                {
                    int index = Int(argument.GetSplit(0, ",")); Pokemon.Natures nature = Pokemon.ConvertIDToNature(Int(argument.GetSplit(1, ",")));
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].Nature = nature; }
                    break;
                }
                case "npctrade":
                {
                    Script.SaveNPCTrade = argument.Split('|');
                    PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, Items.Item.GetItemByID("5")!, null,
                        Localization.GetString("trade_screen_ChoosePokemonForTrade", "Choose Pokémon for Trade"), true)
                        { Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection, CanExit = true };
                    selScreen.SelectedObject += Script.DoNPCTradeHandler;
                    Core.SetScreen(selScreen);
                    ((PartyScreen)Core.CurrentScreen).ExitedSub = Script.ExitedNPCTrade;
                    CanContinue = false;
                    break;
                }
                case "rename":
                {
                    String index = argument; bool renameOTcheck = false; bool canRename = true;
                    if (argument.Contains(",") == true) { index = argument.GetSplit(0); renameOTcheck = ScriptConversion.ToBoolean(argument.GetSplit(1)); }
                    int pokemonIndex = index.ToLower().Equals("last") == true ? Core.Player.Pokemons.Count - 1 : Int(index);
                    if (renameOTcheck == true && Core.Player.Pokemons[pokemonIndex].OT.Equals(Core.Player.OT) == true) { canRename = false; }
                    if (Core.Player.Pokemons[pokemonIndex].IsEgg == false)
                    {
                        if (canRename == true) { Core.SetScreen(new NameObjectScreen(Core.CurrentScreen, Core.Player.Pokemons[pokemonIndex])); }
                        else { Screen.TextBox.Show("I cannot rename this~Pokémon because the~OT is different!*Did you receive it in~a trade or something?"); }
                    }
                    else { Screen.TextBox.Show("I cannot rename~this egg..."); }
                    CanContinue = false;
                    break;
                }
                case "read":
                {
                    Pokemon p = Core.Player.Pokemons[Int(argument)];
                    String message = "Hm... I see your~" + p.GetDisplayName();
                    String addMessage = "~is very stable with~";
                    if (p.EVAttack > p.EVDefense && p.EVAttack > p.EVHP && p.EVAttack > p.EVSpAttack && p.EVAttack > p.EVSpDefense && p.EVAttack > p.EVSpeed) addMessage += "performing physical moves.";
                    else if (p.EVDefense > p.EVAttack && p.EVDefense > p.EVHP && p.EVDefense > p.EVSpAttack && p.EVDefense > p.EVSpDefense && p.EVDefense > p.EVSpeed) addMessage += "taking hits.";
                    else if (p.EVHP > p.EVAttack && p.EVHP > p.EVDefense && p.EVHP > p.EVSpAttack && p.EVHP > p.EVSpDefense && p.EVHP > p.EVSpeed) addMessage += "taking damage.";
                    else if (p.EVSpAttack > p.EVAttack && p.EVSpAttack > p.EVDefense && p.EVSpAttack > p.EVHP && p.EVSpAttack > p.EVSpDefense && p.EVSpAttack > p.EVSpeed) addMessage += "performing complex strategies.";
                    else if (p.EVSpDefense > p.EVAttack && p.EVSpDefense > p.EVDefense && p.EVSpDefense > p.EVHP && p.EVSpDefense > p.EVSpAttack && p.EVSpDefense > p.EVSpeed) addMessage += "breaking strategies.";
                    else if (p.EVSpeed > p.EVAttack && p.EVSpeed > p.EVDefense && p.EVSpeed > p.EVHP && p.EVSpeed > p.EVSpAttack && p.EVSpeed > p.EVSpDefense) addMessage += "speeding the others out.";
                    if (addMessage.Equals("~is very stable with~") == true) { addMessage = "~is very well balanced."; }
                    Screen.TextBox.Show(message + addMessage + "*...~...*What that means?~I am not sure...", [], false, false);
                    CanContinue = false;
                    break;
                }
                case "heal":
                    if (argument.Equals("") == true) { Core.Player.HealParty(); }
                    else if (argument.Contains(",") == true)
                    {
                        String[] data = argument.Split(','); List<int> members = [];
                        foreach (String m in data) { members.Add(Int(m)); }
                        Core.Player.HealParty(members.ToArray());
                    }
                    else { Core.Player.HealParty([Int(argument)]); }
                    break;
                case "setfriendship": { int i = Int(argument.GetSplit(0)); int amt = Int(argument.GetSplit(1)); Core.Player.Pokemons[i].Friendship = (int)MathHelper.Clamp(amt, 0, 255); break; }
                case "addfriendship": { int i = Int(argument.GetSplit(0)); int amt = Int(argument.GetSplit(1)); Core.Player.Pokemons[i].Friendship = (int)MathHelper.Clamp(Core.Player.Pokemons[i].Friendship + amt, 0, 255); break; }
                case "removefriendship": { int i = Int(argument.GetSplit(0)); int amt = Int(argument.GetSplit(1)); Core.Player.Pokemons[i].Friendship = (int)MathHelper.Clamp(Core.Player.Pokemons[i].Friendship - amt, 0, 255); break; }
                case "select":
                {
                    bool canExit = false; bool canChooseEgg = true; bool canChooseFainted = true; int canLearnAttack = -1;
                    String selectBtnText = Localization.GetString("global_select", "Select");
                    if (argument.Equals("") == false)
                    {
                        String[] data = argument.Split(',');
                        if (data.Length > 0) { canExit = ScriptConversion.ToBoolean(data[0]); }
                        if (data.Length > 1) { canChooseFainted = ScriptConversion.ToBoolean(data[1]); }
                        if (data.Length > 2) { canChooseEgg = ScriptConversion.ToBoolean(data[2]); }
                        if (data.Length > 3) { canLearnAttack = Int(data[3]); }
                        if (data.Length > 4) { selectBtnText = data[4]; }
                    }
                    PartyScreen selScreen;
                    if (canLearnAttack != -1)
                    {
                        selScreen = new PartyScreen(Core.CurrentScreen, Items.Item.GetItemByID("5")!, null,
                            Localization.GetString("global_Learn", "Learn") + " " + BattleSystem.Attack.GetAttackByID(canLearnAttack).Name,
                            canExit, canChooseFainted, canChooseEgg)
                            { Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection, CanExit = canExit, SelectButtonText = selectBtnText };
                        selScreen.SetupLearnAttack(BattleSystem.Attack.GetAttackByID(canLearnAttack), 2, null);
                    }
                    else
                    {
                        selScreen = new PartyScreen(Core.CurrentScreen, Items.Item.GetItemByID("5")!, null,
                            Localization.GetString("party_screen_ChoosePokemon", "Choose Pokémon"),
                            canExit, canChooseFainted, canChooseEgg)
                            { Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection, CanExit = canExit, SelectButtonText = selectBtnText };
                    }
                    Core.SetScreen(selScreen);
                    CanContinue = false;
                    break;
                }
                case "selectmove":
                {
                    int index = 0; bool canHMMove = true; bool canExit = false;
                    if (argument.Contains(",") == true)
                    {
                        List<String> args = argument.Split(',').ToList();
                        for (int i = 0; i < args.Count; i++)
                        {
                            switch (i) { case 0: index = Int(args[i]); break; case 1: canHMMove = ScriptConversion.ToBoolean(args[i]); break; case 2: canExit = ScriptConversion.ToBoolean(args[i]); break; }
                        }
                    }
                    else { index = Int(argument); }
                    Core.SetScreen(new ChooseAttackScreen(Core.CurrentScreen, Core.Player.Pokemons[index], canHMMove, canExit, null));
                    CanContinue = false;
                    break;
                }
                case "calcstats": { int index = Int(argument); Core.Player.Pokemons[index].CalculateStats(); break; }
                case "learnattack":
                {
                    int index = Int(argument.GetSplit(0)); int atkID = Int(argument.GetSplit(1));
                    Core.SetScreen(new LearnAttackScreen(Core.CurrentScreen, Core.Player.Pokemons[index], BattleSystem.Attack.GetAttackByID(atkID)));
                    CanContinue = false;
                    break;
                }
                case "setgender":
                {
                    int index = Int(argument.GetSplit(0, ",")); int gender = Int(argument.GetSplit(1, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index && gender >= 0 && gender <= 2)
                        Core.Player.Pokemons[index].Gender = (Pokemon.Genders)gender;
                    break;
                }
                case "setability":
                {
                    int index = Int(argument.GetSplit(0, ","));
                    if (StringHelper.IsNumeric(argument.GetSplit(1, ",")) == true)
                    {
                        if (Core.Player.Pokemons.Count - 1 >= index)
                        {
                            Core.Player.Pokemons[index].Ability = Ability.GetAbilityByID(Int(argument.GetSplit(1, ",")));
                            for (int a = 0; a < Core.Player.Pokemons[index].newAbilities.Count; a++)
                            {
                                if (Core.Player.Pokemons[index].Ability.ID == Core.Player.Pokemons[index].newAbilities[a].ID)
                                {
                                    Core.Player.Pokemons[index].AbilitySlot = a == 0 ? "A" : a == 1 ? "B" : a == 2 ? "C" : argument.GetSplit(1, ",");
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        String slot = argument.GetSplit(1, ",").ToLower();
                        switch (slot)
                        {
                            case "a": Core.Player.Pokemons[index].Ability = Core.Player.Pokemons[index].newAbilities[0]; Core.Player.Pokemons[index].AbilitySlot = "A"; break;
                            case "b":
                                Core.Player.Pokemons[index].Ability = Core.Player.Pokemons[index].newAbilities.Count > 1 && Core.Player.Pokemons[index].newAbilities[1] != null
                                    ? Core.Player.Pokemons[index].newAbilities[1] : Core.Player.Pokemons[index].newAbilities[0];
                                Core.Player.Pokemons[index].AbilitySlot = "B"; break;
                            case "c":
                                Core.Player.Pokemons[index].Ability = Core.Player.Pokemons[index].newAbilities.Count > 2 && Core.Player.Pokemons[index].newAbilities[2] != null
                                    ? Core.Player.Pokemons[index].newAbilities[2] : Core.Player.Pokemons[index].newAbilities[0];
                                Core.Player.Pokemons[index].AbilitySlot = "C"; break;
                            case "h":
                                Core.Player.Pokemons[index].Ability = Core.Player.Pokemons[index].HasHiddenAbility == true
                                    ? Core.Player.Pokemons[index].hiddenAbility : Core.Player.Pokemons[index].newAbilities[0];
                                Core.Player.Pokemons[index].AbilitySlot = "H"; break;
                            default:
                            {
                                int ni = Core.Random.Next(0, 2);
                                Core.Player.Pokemons[index].Ability = ni == 0 ? Core.Player.Pokemons[index].newAbilities[0]
                                    : (Core.Player.Pokemons[index].newAbilities.Count > 1 && Core.Player.Pokemons[index].newAbilities[1] != null
                                        ? Core.Player.Pokemons[index].newAbilities[1] : Core.Player.Pokemons[index].newAbilities[0]);
                                Core.Player.Pokemons[index].AbilitySlot = ni == 0 ? "A" : "B";
                                break;
                            }
                        }
                    }
                    break;
                }
                case "addev": case "setev": case "setallevs":
                {
                    int index = Int(argument.GetSplit(0, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index)
                    {
                        Pokemon p = Core.Player.Pokemons[index];
                        if (command.ToLower().Equals("setallevs") == true)
                        {
                            p.EVHP = Int(argument.GetSplit(1, ",")).Clamp(0, 255); p.EVAttack = Int(argument.GetSplit(2, ",")).Clamp(0, 255);
                            p.EVDefense = Int(argument.GetSplit(3, ",")).Clamp(0, 255); p.EVSpAttack = Int(argument.GetSplit(4, ",")).Clamp(0, 255);
                            p.EVSpDefense = Int(argument.GetSplit(5, ",")).Clamp(0, 255); p.EVSpeed = Int(argument.GetSplit(6, ",")).Clamp(0, 255);
                        }
                        else
                        {
                            String ev = argument.GetSplit(1, ","); int evValue = Int(argument.GetSplit(2, ","));
                            if (command.ToLower().Equals("addev") == true)
                            {
                                int total = p.EVHP + p.EVAttack + p.EVDefense + p.EVSpAttack + p.EVSpDefense + p.EVSpeed;
                                if (total + evValue > 510) { evValue = 510 - total; }
                                switch (ev.ToLower())
                                {
                                    case "hp": if (p.EVHP + evValue > 255) { evValue = 255 - p.EVHP; } p.EVHP += evValue; break;
                                    case "atk": case "attack": if (p.EVAttack + evValue > 255) { evValue = 255 - p.EVAttack; } p.EVAttack += evValue; break;
                                    case "def": case "defense": if (p.EVDefense + evValue > 255) { evValue = 255 - p.EVDefense; } p.EVDefense += evValue; break;
                                    case "spatk": case "specialattack": case "spattack": if (p.EVSpAttack + evValue > 255) { evValue = 255 - p.EVSpAttack; } p.EVSpAttack += evValue; break;
                                    case "spdef": case "specialdefense": case "spdefense": if (p.EVSpDefense + evValue > 255) { evValue = 255 - p.EVSpDefense; } p.EVSpDefense += evValue; break;
                                    case "speed": if (p.EVSpeed + evValue > 255) { evValue = 255 - p.EVSpeed; } p.EVSpeed += evValue; break;
                                }
                            }
                            else
                            {
                                switch (ev.ToLower())
                                {
                                    case "hp": p.EVHP = evValue; break; case "atk": case "attack": p.EVAttack = evValue; break;
                                    case "def": case "defense": p.EVDefense = evValue; break;
                                    case "spatk": case "specialattack": case "spattack": p.EVSpAttack = evValue; break;
                                    case "spdef": case "specialdefense": case "spdefense": p.EVSpDefense = evValue; break;
                                    case "speed": p.EVSpeed = evValue; break;
                                }
                            }
                        }
                    }
                    break;
                }
                case "setiv": case "setallivs":
                {
                    int index = Int(argument.GetSplit(0, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index)
                    {
                        Pokemon p = Core.Player.Pokemons[index];
                        if (command.ToLower().Equals("setallivs") == true)
                        {
                            p.IVHP = Int(argument.GetSplit(1, ",")).Clamp(0, 31); p.IVAttack = Int(argument.GetSplit(2, ",")).Clamp(0, 31);
                            p.IVDefense = Int(argument.GetSplit(3, ",")).Clamp(0, 31); p.IVSpAttack = Int(argument.GetSplit(4, ",")).Clamp(0, 31);
                            p.IVSpDefense = Int(argument.GetSplit(5, ",")).Clamp(0, 31); p.IVSpeed = Int(argument.GetSplit(6, ",")).Clamp(0, 31);
                        }
                        else
                        {
                            String dv = argument.GetSplit(1, ","); int dvValue = Int(argument.GetSplit(2, ","));
                            switch (dv.ToLower())
                            {
                                case "hp": p.IVHP = dvValue; break; case "atk": case "attack": p.IVAttack = dvValue; break;
                                case "def": case "defense": p.IVDefense = dvValue; break;
                                case "spatk": case "specialattack": case "spattack": p.IVSpAttack = dvValue; break;
                                case "spdef": case "specialdefense": case "spdefense": p.IVSpDefense = dvValue; break;
                                case "speed": p.IVSpeed = dvValue; break;
                            }
                        }
                    }
                    break;
                }
                case "registerhalloffame":
                {
                    int count = -1; String newHallOfFameData = "";
                    if (Core.Player.HallOfFameData.Equals("") == false)
                    {
                        String[] data = Core.Player.HallOfFameData.SplitAtNewline();
                        foreach (String l in data) { int id = int.Parse(l.Remove(l.IndexOf(","))); if (id > count) { count = id; } }
                        foreach (String l in data) { int id = int.Parse(l.Remove(l.IndexOf(","))); if (id > (count - 19) || id == 0) { newHallOfFameData += l + Environment.NewLine; } }
                    }
                    count += 1;
                    String time = TimeHelpers.GetDisplayTime(TimeHelpers.GetCurrentPlayTime(), true);
                    String newData = Core.Player.IsGameJoltSave == true
                        ? count + ",(" + Core.Player.Name + "|" + time + "|" + Core.GameJoltSave.Points + "|" + Core.Player.OT + "|" + Core.Player.Skin + ")"
                        : count + ",(" + Core.Player.Name + "|" + time + "|" + Core.Player.Points + "|" + Core.Player.OT + "|" + Core.Player.Skin + ")";
                    foreach (Pokemon p in Core.Player.Pokemons)
                    {
                        if (p.IsEgg == false) { newData += Environment.NewLine + count + "," + p.GetHallOfFameData(); }
                    }
                    Core.Player.HallOfFameData = newHallOfFameData + newData;
                    break;
                }
                case "setot":
                {
                    int index = Int(argument.GetSplit(0, ",")); String ot = argument.GetSplit(1, ",");
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].OT = ot; }
                    break;
                }
                case "setitem":
                {
                    int index = Int(argument.GetSplit(0, ","));
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].Item = Items.Item.GetItemByID(argument.GetSplit(1, ",")); }
                    break;
                }
                case "removeitem":
                { int index = Int(argument.GetSplit(0, ",")); Core.Player.Pokemons[index].Item = null; break; }
                case "setitemdata":
                {
                    int index = Int(argument.GetSplit(0, ",")); String itemData = argument.Remove(0, argument.IndexOf(",") + 1);
                    if (Core.Player.Pokemons.Count - 1 >= index && Core.Player.Pokemons[index].Item != null)
                        Core.Player.Pokemons[index].Item!.AdditionalData = itemData;
                    break;
                }
                case "setcatchtrainer":
                {
                    int index = Int(argument.GetSplit(0, ",")); String trainer = argument.GetSplit(1, ",");
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].CatchTrainerName = trainer; }
                    break;
                }
                case "setcatchball":
                {
                    int index = Int(argument.GetSplit(0, ",")); String ballID = argument.GetSplit(1, ",");
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].catchBall = Items.Item.GetItemByID(ballID)!; }
                    break;
                }
                case "setcatchmethod":
                {
                    int index = Int(argument.GetSplit(0, ",")); String method = argument.GetSplit(1, ",");
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].CatchMethod = method; }
                    break;
                }
                case "setcatchplace": case "setcatchlocation":
                {
                    int index = Int(argument.GetSplit(0, ",")); String place = argument.GetSplit(1, ",");
                    if (Localization.TokenExists("Places_" + Screen.Level.MapName) == true) { place = Localization.GetString("Places_" + Screen.Level.MapName, Screen.Level.MapName); }
                    if (Core.Player.Pokemons.Count - 1 >= index) { Core.Player.Pokemons[index].CatchLocation = place; }
                    break;
                }
                case "newroaming":
                {
                    String[] data = argument.Split(',');
                    String pokID = data[1]; String pokAD = "xXx";
                    if (pokID.Contains("_") == true) { pokAD = PokemonForms.GetAdditionalValueFromDataFile(data[1]); pokID = data[1].GetSplit(0, "_"); }
                    else if (pokID.Contains(";") == true) { pokAD = data[1].GetSplit(1, ";"); pokID = data[1].GetSplit(0, ";"); }
                    Pokemon p = Pokemon.GetPokemonByID(Int(pokID), pokAD); p.Generate(Int(data[2]), true, pokAD);
                    if (data.Length > 6 && data[6].Equals("") == false && data[6].Equals("-1") == false) { p.IsShiny = ScriptConversion.ToBoolean(data[6]); }
                    String scriptPath = data.Length > 7 && data[7].Equals("") == false ? data[7] : "";
                    if (Core.Player.RoamingPokemonData.Equals("") == false) { Core.Player.RoamingPokemonData += Environment.NewLine; }
                    Core.Player.RoamingPokemonData += data[0] + "|" + data[1] + "|" + data[2] + "|" + data[3] + "|" + data[4] + "|" + data[5] + "|" + p.IsShiny + "|" + p.GetSaveData() + "|" + scriptPath;
                    break;
                }
                case "evolve":
                {
                    String[] args = argument.Split(','); String triggerStr = args.Length > 1 ? args[1] : "level"; String evolutionArg = args.Length > 2 ? args[2] : "";
                    EvolutionCondition.EvolutionTrigger trigger = EvolutionCondition.EvolutionTrigger.LevelUp;
                    switch (triggerStr) { case "level": case "levelup": case "level up": case "level-up": trigger = EvolutionCondition.EvolutionTrigger.LevelUp; break; case "none": trigger = EvolutionCondition.EvolutionTrigger.None; break; case "itemuse": case "item use": case "item": case "item-use": trigger = EvolutionCondition.EvolutionTrigger.ItemUse; break; case "trade": case "trading": trigger = EvolutionCondition.EvolutionTrigger.Trading; break; }
                    Pokemon p = Core.Player.Pokemons[Int(args[0]).Clamp(0, Core.Player.Pokemons.Count - 1)];
                    if (p.CanEvolve(trigger, evolutionArg) == true)
                    {
                        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new EvolutionScreen(Core.CurrentScreen, [Int(args[0])], evolutionArg, trigger, false), Color.Black, false));
                        CanContinue = false;
                    }
                    break;
                }
                case "levelup":
                {
                    String[] args = argument.Split(','); int amount = args.Length > 1 ? Int(args[1]) : 1;
                    Pokemon p = Core.Player.Pokemons[Int(args[0]).Clamp(0, Core.Player.Pokemons.Count - 1)];
                    int originalLevel = p.Level; int maxLevel = Int(GameModeManager.GetGameRuleValue("MaxLevel", "100"));
                    if (originalLevel < maxLevel && p.IsEgg == false)
                    {
                        if (originalLevel + amount > maxLevel) { amount = maxLevel - originalLevel; }
                        p.Level += amount;
                        List<BattleSystem.Attack> attackLearnList = [];
                        if (amount > 0)
                        {
                            for (int i = 1; i <= amount; i++)
                            {
                                if (p.attackLearns.ContainsKey(originalLevel + i) == true)
                                {
                                    foreach (BattleSystem.Attack a in p.attackLearns[originalLevel + i])
                                    { if (attackLearnList.Contains(a) == false && p.KnowsMove(a) == false) { attackLearnList.Add(a); } }
                                }
                            }
                        }
                        String s = "version=2" + Environment.NewLine;
                        if (amount > 0) { s += "@text.show(" + Localization.GetString("level_up_PokemonReachedLevel", "[POKEMONNAME] reached~level [LEVELNUMBER]!").Replace("[POKEMONNAME]", p.GetDisplayName()).Replace("[LEVELNUMBER]", p.Level.ToString()) + ")" + Environment.NewLine; }
                        int currentMaxHP = p.MaxHP; p.CalculateStats();
                        int hpDiff = p.MaxHP - currentMaxHP; if (hpDiff > 0) { p.Heal(hpDiff); }
                        if (p.CanEvolve(EvolutionCondition.EvolutionTrigger.LevelUp, "") == true) { s += "@pokemon.evolve(" + Int(args[0]) + ")" + Environment.NewLine; }
                        foreach (BattleSystem.Attack a in attackLearnList)
                        {
                            if (p.Attacks.Count < 4) { s += "@text.show(" + Localization.GetString("learn_move_PokemonLearnedMove", "[POKEMONNAME] learned~[MOVENAME]!").Replace("[POKEMONNAME]", p.GetDisplayName()).Replace("[MOVENAME]", a.Name) + ")" + Environment.NewLine; p.Attacks.Add(a); PlayerStatistics.Track("Moves learned", 1); }
                            else { s += "@pokemon.learnattack(" + Int(args[0]) + "," + a.ID + ")" + Environment.NewLine; }
                        }
                        s += ":end";
                        if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
                        { ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2, false); }
                    }
                    break;
                }
                case "reload": { int i = Int(argument); if (Core.Player.Pokemons.Count - 1 >= i) { Core.Player.Pokemons[i].ReloadDefinitions(); Core.Player.Pokemons[i].CalculateStats(); } break; }
                case "reloadall": for (int i = 0; i < Core.Player.Pokemons.Count; i++) { Core.Player.Pokemons[i].ReloadDefinitions(); Core.Player.Pokemons[i].CalculateStats(); } break;
                case "megaevolve":
                {
                    Pokemon p = Core.Player.Pokemons[Int(argument)];
                    if (p.Item != null && p.Item.IsGameModeItem == false)
                    { switch (p.Item.ID) { case 516: case 529: p.AdditionalData = "mega_x"; break; case 517: case 530: p.AdditionalData = "mega_y"; break; default: p.AdditionalData = "mega"; break; } }
                    else { p.AdditionalData = "mega"; }
                    p.ReloadDefinitions(); p.CalculateStats(); p.LoadAltAbility();
                    break;
                }
                case "megaevolveall":
                    for (int i = 0; i < Core.Player.Pokemons.Count; i++)
                    {
                        Pokemon p = Core.Player.Pokemons[i];
                        if (p.Item != null && p.Item.IsGameModeItem == false)
                        { switch (p.Item.ID) { case 516: case 529: p.AdditionalData = "mega_x"; break; case 517: case 530: p.AdditionalData = "mega_y"; break; default: p.AdditionalData = "mega"; break; } }
                        else { p.AdditionalData = "mega"; }
                        p.ReloadDefinitions(); p.CalculateStats(); p.LoadAltAbility();
                    }
                    break;
                case "clone":
                { int i = Int(argument); if (Core.Player.Pokemons.Count - 1 >= i && Core.Player.Pokemons.Count < 6) { Core.Player.Pokemons.Add(Core.Player.Pokemons[i]); } break; }
                case "sendtostorage":
                {
                    String[] data = argument.Split(',');
                    int pi = Int(data[0]);
                    if (Core.Player.Pokemons.Count - 1 >= pi)
                    {
                        if (data.Length == 1) { StorageSystemScreen.DepositPokemon(Core.Player.Pokemons[pi]); }
                        else { StorageSystemScreen.DepositPokemon(Core.Player.Pokemons[pi], Int(data[1])); }
                        Core.Player.Pokemons.RemoveAt(pi);
                    }
                    break;
                }
                case "addtostorage":
                {
                    if (argument.StartsWith("{") == true || (argument.IndexOf(",") >= 0 && argument.Remove(0, argument.IndexOf(",")).StartsWith(",{") == true))
                    {
                        int insertIndex = -1;
                        if (argument.IndexOf(",") >= 0 && argument.Remove(0, argument.IndexOf(",")).StartsWith(",{") == true) { insertIndex = Int(argument.GetSplit(0)); }
                        argument = argument.Remove(0, argument.IndexOf("{"));
                        Pokemon p = Pokemon.GetPokemonByData(argument.Replace("§", ",").Replace("«", "[").Replace("»", "]"));
                        StorageSystemScreen.DepositPokemon(p, insertIndex);
                        if (p.IsEgg == false) { Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true), p.IsShiny == true ? 3 : 2); }
                    }
                    else
                    {
                        int commas = 0; foreach (char c in argument) { if (c == ',') { commas++; } }
                        String pokID = argument.GetSplit(0); String pokAD = "xXx";
                        if (pokID.Contains("_") == true) { pokAD = PokemonForms.GetAdditionalValueFromDataFile(pokID); pokID = pokID.GetSplit(0, "_"); }
                        else if (pokID.Contains(";") == true) { pokAD = argument.GetSplit(0).GetSplit(1, ";"); pokID = argument.GetSplit(0).GetSplit(0, ";"); }
                        int level = Int(argument.GetSplit(1));
                        String catchMethod = "random reason"; if (commas > 1) { catchMethod = ScriptCommander.Parse(argument.GetSplit(2)).ToString() ?? catchMethod; }
                        Items.Item catchBall = Items.Item.GetItemByID("5")!; if (commas > 2) { catchBall = Items.Item.GetItemByID(argument.GetSplit(3))!; }
                        String catchLocation = Screen.Level.MapName; if (commas > 3) { catchLocation = ScriptCommander.Parse(argument.GetSplit(4)).ToString() ?? catchLocation; }
                        bool isEgg = false; if (commas > 4) { isEgg = ScriptConversion.ToBoolean(argument.GetSplit(5)); }
                        String catchTrainer = Core.Player.Name;
                        if (commas > 5 && argument.GetSplit(6).Equals("<playername>") == false && argument.GetSplit(6).Equals("<player.name>") == false) { catchTrainer = ScriptCommander.Parse(argument.GetSplit(6)).ToString() ?? catchTrainer; }
                        String heldItem = "0"; if (commas > 6) { heldItem = argument.GetSplit(7); }
                        bool isShiny = Core.Random.Next(0, Pokemon.MasterShinyRate + 1) == 0; if (commas > 7) { isShiny = ScriptConversion.ToBoolean(argument.GetSplit(8)); }
                        Pokemon pokemon = Pokemon.GetPokemonByID(Int(pokID), pokAD); pokemon.Generate(level, true, pokAD);
                        pokemon.CatchTrainerName = catchTrainer; pokemon.OT = Core.Player.OT; pokemon.CatchLocation = catchLocation; pokemon.catchBall = catchBall; pokemon.CatchMethod = catchMethod;
                        if (isEgg == true) { pokemon.EggSteps = 1; pokemon.SetCatchInfos(Items.Item.GetItemByID("5")!, Localization.GetString("CatchMethod_Obtained", "Obtained at")); } else { pokemon.EggSteps = 0; }
                        if (heldItem.Equals("0") == false) { pokemon.Item = Items.Item.GetItemByID(heldItem); }
                        pokemon.IsShiny = isShiny;
                        StorageSystemScreen.DepositPokemon(pokemon);
                        if (pokemon.IsEgg == false) { Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, PokemonForms.GetPokemonDataFileName(pokemon.Number, pokemon.AdditionalData, true), pokemon.IsShiny == true ? 3 : 2); }
                    }
                    break;
                }
                case "addsteps": { int i = Int(argument.GetSplit(0, ",")); int steps = Int(argument.GetSplit(1, ",")); if (Core.Player.Pokemons.Count - 1 >= i) { Core.Player.Pokemons[i].EggSteps += steps; } break; }
                case "setsteps": { int i = Int(argument.GetSplit(0, ",")); int steps = Int(argument.GetSplit(1, ",")); if (Core.Player.Pokemons.Count - 1 >= i) { Core.Player.Pokemons[i].EggSteps = steps; } break; }
                case "hatch":
                {
                    int index = Int(argument.GetSplit(0, ",")); bool canRename = true; String msg = "";
                    if (argument.Split(',').Length > 1) { canRename = ScriptConversion.ToBoolean(argument.GetSplit(1, ",")); if (argument.Split(',').Length > 2) { msg = argument.GetSplit(2, ","); } }
                    Pokemon? pokemon = null;
                    if (Core.Player.Pokemons.Count - 1 >= index) { pokemon = Core.Player.Pokemons[index]; Core.Player.Pokemons.Remove(pokemon); }
                    if (pokemon != null && pokemon.IsEgg == true)
                    {
                        Screen.TextBox.Show("Huh?");
                        Core.SetScreen(new TransitionScreen((OverworldScreen)Core.CurrentScreen, new HatchEggScreen((OverworldScreen)Core.CurrentScreen, [pokemon], canRename, msg), Color.White, false));
                        CanContinue = false;
                    }
                    break;
                }
                case "setstatus":
                {
                    int index = Int(argument.GetSplit(0, ",")); int status = -1;
                    switch (argument.GetSplit(1, ","))
                    {
                        case "brn": status = (int)Pokemon.StatusProblems.Burn; break;
                        case "frz": status = (int)Pokemon.StatusProblems.Freeze; break;
                        case "prz": status = (int)Pokemon.StatusProblems.Paralyzed; break;
                        case "psn": status = (int)Pokemon.StatusProblems.Poison; break;
                        case "bpsn": status = (int)Pokemon.StatusProblems.BadPoison; break;
                        case "slp": status = (int)Pokemon.StatusProblems.Sleep; break;
                        case "fnt": status = (int)Pokemon.StatusProblems.Fainted; break;
                        default: status = (int)Pokemon.StatusProblems.None; break;
                    }
                    if (status != -1 && Core.Player.Pokemons.Count - 1 >= index)
                    {
                        Core.Player.Pokemons[index].Status = (Pokemon.StatusProblems)status;
                        if (status == (int)Pokemon.StatusProblems.Fainted) { Core.Player.Pokemons[index].HP = 0; }
                    }
                    break;
                }
            }
            IsReady = true;
        }
    }
}
