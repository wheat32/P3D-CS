using Microsoft.Xna.Framework.Input;

namespace P3D.ScriptVersion2
{
    public static partial class ScriptComparer
    {
        private static int Int(Object o) => ScriptConversion.ToInteger(o);
        private static float Sng(Object o) => ScriptConversion.ToSingle(o);
        private static double Dbl(Object o) => ScriptConversion.ToDouble(o);

        private static Object DoPokemon(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "id": case "number": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Number;
                case "data": return Core.Player.Pokemons[Int(argument.GetSplit(0))].GetSaveData().Replace(",", "§").Replace("[", "«").Replace("]", "»");
                case "gender": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Gender;
                case "genderchance":
                {
                    String dexID = argument.GetSplit(0); String dexAD = String.Empty;
                    if (dexID.Contains("_") == true) { dexAD = PokemonForms.GetAdditionalValueFromDataFile(dexID); dexID = dexID.GetSplit(0, "_"); }
                    return Pokemon.GetPokemonByID(Int(dexID), dexAD).IsMale;
                }
                case "level": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Level;
                case "hasfullhp": { Pokemon p = Core.Player.Pokemons[Int(argument.GetSplit(0))]; return ReturnBoolean(p.HP == p.MaxHP); }
                case "hp": return Core.Player.Pokemons[Int(argument.GetSplit(0))].HP;
                case "atk": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Attack;
                case "def": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Defense;
                case "spatk": return Core.Player.Pokemons[Int(argument.GetSplit(0))].SpAttack;
                case "spdef": return Core.Player.Pokemons[Int(argument.GetSplit(0))].SpDefense;
                case "speed": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Speed;
                case "maxhp": return Core.Player.Pokemons[Int(argument.GetSplit(0))].MaxHP;
                case "isegg": return ReturnBoolean(Core.Player.Pokemons[Int(argument.GetSplit(0))].IsEgg);
                case "additionaldata": return Core.Player.Pokemons[Int(argument.GetSplit(0))].AdditionalData;
                case "nickname": { Pokemon p = Core.Player.Pokemons[Int(argument.GetSplit(0))]; return p.NickName.Equals("") == false ? p.NickName : p.GetDisplayName(); }
                case "hasnickname": return Core.Player.Pokemons[Int(argument.GetSplit(0))].NickName.Equals("");
                case "name": return Core.Player.Pokemons[Int(argument.GetSplit(0))].GetName();
                case "originalname": return Core.Player.Pokemons[Int(argument.GetSplit(0))].OriginalName;
                case "ot": return Core.Player.Pokemons[Int(argument.GetSplit(0))].OT;
                case "trainer": return Core.Player.Pokemons[Int(argument.GetSplit(0))].CatchTrainerName;
                case "itemid":
                {
                    Pokemon p = Core.Player.Pokemons[Int(argument.GetSplit(0))];
                    if (p.Item == null) { return 0; }
                    return p.Item.IsGameModeItem == true ? (Object)p.Item.gmID : p.Item.ID.ToString();
                }
                case "friendship": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Friendship.ToString();
                case "itemname": case "item":
                {
                    Pokemon p = Core.Player.Pokemons[Int(argument.GetSplit(0))];
                    return p.Item == null ? (Object)"" : p.Item.Name;
                }
                case "catchball": return Core.Player.Pokemons[Int(argument.GetSplit(0))].catchBall.ID;
                case "catchmethod": return Core.Player.Pokemons[Int(argument.GetSplit(0))].CatchMethod;
                case "catchlocation": return Core.Player.Pokemons[Int(argument.GetSplit(0))].CatchLocation;
                case "hasattackinparty":
                {
                    int atkID = Int(argument.GetSplit(0)); int i = 0;
                    foreach (Pokemon p in Core.Player.Pokemons)
                    {
                        foreach (BattleSystem.Attack a in p.Attacks) { if (a.ID == atkID) { return i; } }
                        i++;
                    }
                    return "-1";
                }
                case "hasattack":
                {
                    int index = Int(argument.GetSplit(0)); int atkID = Int(argument.GetSplit(1)); bool has = false;
                    foreach (BattleSystem.Attack a in Core.Player.Pokemons[index].Attacks) { if (a.ID == atkID) { has = true; break; } }
                    return ReturnBoolean(has);
                }
                case "countattacks": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Attacks.Count;
                case "attackname": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Attacks[Int(argument.GetSplit(1))].Name;
                case "levelattacks":
                {
                    int pi = Int(argument.GetSplit(0)); int maxLevel = Core.Player.Pokemons[pi].Level;
                    if (argument.Split(',').Length > 1 && argument.GetSplit(1).ToLower().Equals("-1") == false) { maxLevel = Int(argument.GetSplit(1)); }
                    String levelMoves = String.Empty;
                    foreach (int level in Core.Player.Pokemons[pi].attackLearns.Keys)
                    {
                        if (level <= maxLevel)
                        {
                            foreach (BattleSystem.Attack a in Core.Player.Pokemons[pi].attackLearns[level])
                            { levelMoves = levelMoves.Equals("") == true ? a.ID.ToString() : levelMoves + "," + a.ID.ToString(); }
                        }
                    }
                    return levelMoves;
                }
                case "canlearnattack":
                {
                    int pi = Int(argument.GetSplit(0)); BattleSystem.Attack la = BattleSystem.Attack.GetAttackByID(Int(argument.GetSplit(1))); bool canLearn = false;
                    for (int i = 0; i < Core.Player.Pokemons[pi].attackLearns.Count; i++)
                    {
                        List<BattleSystem.Attack> aList = Core.Player.Pokemons[pi].attackLearns.Values.ElementAt(i);
                        foreach (BattleSystem.Attack a in aList) { if (a.ID == la.ID) { canLearn = true; } }
                    }
                    foreach (int egg in Core.Player.Pokemons[pi].eggMoves) { if (egg == la.ID) { canLearn = true; } }
                    foreach (int tm in Core.Player.Pokemons[pi].machines) { if (tm == la.ID) { canLearn = true; } }
                    return ReturnBoolean(canLearn);
                }
                case "isshiny": return ReturnBoolean(Core.Player.Pokemons[Int(argument.GetSplit(0))].IsShiny);
                case "nature": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Nature.ToString();
                case "ownpokemon": { Pokemon p = Core.Player.Pokemons[Int(argument.GetSplit(0))]; return ReturnBoolean(p.OT.Equals(Core.Player.OT) == true); }
                case "islegendary": return ReturnBoolean(Pokemon.Legendaries.Contains(Core.Player.Pokemons[Int(argument.GetSplit(0))].Number));
                case "freeplaceinparty": return ReturnBoolean(Core.Player.Pokemons.Count < 6);
                case "nopokemon": return ReturnBoolean(Core.Player.Pokemons.Count == 0);
                case "count": return Core.Player.Pokemons.Count;
                case "countbattle": { int c = 0; foreach (Pokemon p in Core.Player.Pokemons) { if (p.IsEgg == false && p.HP > 0 && p.Status != Pokemon.StatusProblems.Fainted) { c++; } } return c; }
                case "has": { bool has = false; int pid = Int(argument.GetSplit(0)); foreach (Pokemon p in Core.Player.Pokemons) { if (p.Number == pid) { has = true; break; } } return ReturnBoolean(has); }
                case "selected": return PartyScreen.Selected;
                case "selectedmove": return ChooseAttackScreen.Selected;
                case "hasegg": { bool hasEgg = false; foreach (Pokemon p in Core.Player.Pokemons) { if (p.IsEgg == true) { hasEgg = true; break; } } return ReturnBoolean(hasEgg); }
                case "maxpartylevel": { int ml = 0; foreach (Pokemon p in Core.Player.Pokemons) { if (ml < p.Level) { ml = p.Level; } } return ml; }
                case "evhp": return Core.Player.Pokemons[Int(argument.GetSplit(0))].EVHP;
                case "evatk": return Core.Player.Pokemons[Int(argument.GetSplit(0))].EVAttack;
                case "evdef": return Core.Player.Pokemons[Int(argument.GetSplit(0))].EVDefense;
                case "evspatk": return Core.Player.Pokemons[Int(argument.GetSplit(0))].EVSpAttack;
                case "evspdef": return Core.Player.Pokemons[Int(argument.GetSplit(0))].EVSpDefense;
                case "evspeed": return Core.Player.Pokemons[Int(argument.GetSplit(0))].EVSpeed;
                case "ivhp": return Core.Player.Pokemons[Int(argument.GetSplit(0))].IVHP;
                case "ivatk": return Core.Player.Pokemons[Int(argument.GetSplit(0))].IVAttack;
                case "ivdef": return Core.Player.Pokemons[Int(argument.GetSplit(0))].IVDefense;
                case "ivspatk": return Core.Player.Pokemons[Int(argument.GetSplit(0))].IVSpAttack;
                case "ivspdef": return Core.Player.Pokemons[Int(argument.GetSplit(0))].IVSpDefense;
                case "ivspeed": return Core.Player.Pokemons[Int(argument.GetSplit(0))].IVSpeed;
                case "itemdata": { int i = Int(argument); return Core.Player.Pokemons[i].Item != null ? (Object)(Core.Player.Pokemons[i].Item!.AdditionalData) : ""; }
                case "mailsendername": { int i = Int(argument); if (Core.Player.Pokemons[i].Item != null && Core.Player.Pokemons[i].Item!.IsMail == true) { return Core.Player.Pokemons[i].Item!.AdditionalData.GetSplit(1, "\\,"); } return ""; }
                case "mailsenderot": { int i = Int(argument); if (Core.Player.Pokemons[i].Item != null && Core.Player.Pokemons[i].Item!.IsMail == true) { return Core.Player.Pokemons[i].Item!.AdditionalData.GetSplit(6, "\\,"); } return ""; }
                case "counthalloffame": return HallOfFameScreen.GetHallOfFameCount();
                case "learnedtutormove": return ReturnBoolean(TeachMovesScreen.LearnedMove);
                case "totalexp": return Core.Player.Pokemons[Int(argument.GetSplit(0))].Experience;
                case "needexp": { Pokemon p = Core.Player.Pokemons[Int(argument.GetSplit(0))]; return p.NeedExperience(p.Level + 1) - p.Experience; }
                case "currentexp": { Pokemon p = Core.Player.Pokemons[Int(argument.GetSplit(0))]; return p.Experience - p.NeedExperience(p.Level); }
                case "generatefrontier":
                {
                    int level = Int(argument.GetSplit(0)).Clamp(1, Int(GameModeManager.GetGameRuleValue("MaxLevel", "100")));
                    int pokClass = Int(argument.GetSplit(1)); List<int>? idPreset = null;
                    if (argument.CountSeperators(",") > 1)
                    {
                        for (int i = 2; i <= argument.CountSeperators(","); i++)
                        {
                            String s = argument.GetSplit(i);
                            if (s.Contains("-") == true) { int min = Int(s.Remove(s.IndexOf("-"))); int max = Int(s.Remove(0, s.IndexOf("-") + 1)); if (idPreset == null) { idPreset = []; } for (int c = min; c <= max; c++) { idPreset.Add(c); } }
                            else { if (idPreset == null) { idPreset = []; } idPreset.Add(Int(argument.GetSplit(i))); }
                        }
                    }
                    Pokemon? fp = FrontierSpawner.GetPokemon(level, pokClass, idPreset);
                    return fp != null ? (Object)(fp.GetSaveData().Replace(",", "§").Replace("[", "«").Replace("]", "»")) : DefaultNull;
                }
                case "spawnwild":
                {
                    Pokemon? sp = Spawner.GetPokemon(Screen.Level.LevelFile!, (Spawner.EncounterMethods)Int(argument));
                    return sp != null ? (Object)(sp.GetSaveData().Replace(",", "§").Replace("[", "«").Replace("]", "»")) : DefaultNull;
                }
                case "spawn":
                {
                    Pokemon p = Pokemon.GetPokemonByID(Int(argument.GetSplit(0))); p.Generate(Int(argument.GetSplit(1)), true);
                    return p.GetSaveData().Replace(",", "§").Replace("[", "«").Replace("]", "»");
                }
                case "otmatch":
                {
                    int maxDigits = 0; String maxName = "[EMPTY]"; int maxID = 0;
                    String checkOT = argument.GetSplit(0); while (checkOT.Length < 5) { checkOT = "0" + checkOT; }
                    char[] checkDigits = [checkOT[0], checkOT[1], checkOT[2], checkOT[3], checkOT[4]];
                    List<Pokemon> ps = StorageSystemScreen.GetAllBoxPokemon();
                    foreach (Pokemon p in Core.Player.Pokemons) { ps.Add(p); }
                    ps = ps.ToArray().Randomize().ToList();
                    foreach (Pokemon p in ps)
                    {
                        int currentCount = 0; String pOT = p.OT; while (pOT.Length < 5) { pOT = "0" + pOT; }
                        char[] pDigits = [pOT[0], pOT[1], pOT[2], pOT[3], pOT[4]];
                        for (int i = 4; i >= 0; i--) { if (pDigits[i] == checkDigits[i]) { currentCount++; } else { break; } }
                        if (currentCount > maxDigits) { maxDigits = currentCount; maxName = p.GetDisplayName(); maxID = p.Number; }
                    }
                    String arg = argument.Split(',')[1];
                    switch (arg.ToLower()) { case "has": return ReturnBoolean(maxDigits > 0); case "id": case "number": return maxID; case "name": return maxName; case "maxhits": return maxDigits; }
                    return "INVALID ARGUMENT";
                }
                case "randomot": { String n = Core.Random.Next(0, 100000).ToString(); while (n.Length < 5) { n = "0" + n; } return n; }
                case "status": return Core.Player.Pokemons[Int(argument)].Status.ToString();
                case "canevolve":
                {
                    String[] args = argument.Split(','); String triggerStr = args.Length > 1 ? args[1] : "level"; String evArg = args.Length > 2 ? args[2] : "";
                    EvolutionCondition.EvolutionTrigger trigger = EvolutionCondition.EvolutionTrigger.LevelUp;
                    switch (triggerStr) { case "level": case "levelup": case "level up": case "level-up": trigger = EvolutionCondition.EvolutionTrigger.LevelUp; break; case "none": trigger = EvolutionCondition.EvolutionTrigger.None; break; case "itemuse": case "item use": case "item": case "item-use": trigger = EvolutionCondition.EvolutionTrigger.ItemUse; break; case "trade": case "trading": trigger = EvolutionCondition.EvolutionTrigger.Trading; break; }
                    return ReturnBoolean(Core.Player.Pokemons[Int(args[0])].CanEvolve(trigger, evArg));
                }
                case "type1": { Pokemon p = Core.Player.Pokemons[Int(argument)]; return p.Type1.IsGameModeElement == false ? (Object)p.Type1.Type.ToString() : p.Type1.gmOriginalName; }
                case "type2": { Pokemon p = Core.Player.Pokemons[Int(argument)]; return p.Type2.IsGameModeElement == false ? (Object)p.Type2.Type.ToString() : p.Type2.gmOriginalName; }
                case "istype": { String[] args = argument.Split(','); return ReturnBoolean(Core.Player.Pokemons[Int(args[0])].IsType(BattleSystem.GameModeElementLoader.GetElementByName(args[1]).Type)); }
                case "ability": return Core.Player.Pokemons[Int(argument)].Ability.ID;
                case "abilityslot": return Core.Player.Pokemons[Int(argument)].AbilitySlot ?? "";
                case "displayname": return Core.Player.Pokemons[Int(argument.GetSplit(0))].GetDisplayName();
                case "menusprite":
                {
                    int index = Int(argument.GetSplit(0)); Pokemon p = Core.Player.Pokemons[index];
                    Microsoft.Xna.Framework.Vector2 v = PokemonForms.GetMenuImagePositionVec(p); Size s = PokemonForms.GetMenuImageSize(p); String sheet = PokemonForms.GetSheetName(p);
                    int shinypos = p.IsShiny == true ? 512 : 0;
                    return "GUI\\PokemonMenu\\" + sheet + "|" + ((int)v.X * 32 + shinypos) + "|" + ((int)v.Y * 32) + "|" + s.Width + "|" + s.Height;
                }
                case "getsteps": { int i = Int(argument.GetSplit(0)); return Core.Player.Pokemons.Count - 1 >= i ? (Object)Core.Player.Pokemons[i].EggSteps : DefaultNull; }
                case "mastershinyrate": { bool adjusted = argument.Equals("") == false ? ScriptConversion.ToBoolean(argument) : true; return Pokemon.GetMasterShinyRate(adjusted); }
                case "isroaming": { foreach (String line in Core.Player.RoamingPokemonData.SplitAtNewline()) { if (line.StartsWith(argument) == true) { return ReturnBoolean(true); } } return ReturnBoolean(false); }
                case "fullyhealed":
                {
                    bool isFullyHealed = true;
                    if (argument.Equals("") == true)
                    {
                        foreach (Pokemon pokemon in Core.Player.Pokemons)
                        {
                            for (int d = 0; d < pokemon.Attacks.Count; d++) { if (pokemon.Attacks[d].CurrentPP < pokemon.Attacks[d].MaxPP) { isFullyHealed = false; } }
                            if (pokemon.HP < pokemon.MaxHP) { isFullyHealed = false; }
                            if (pokemon.Status != Pokemon.StatusProblems.None) { isFullyHealed = false; }
                        }
                    }
                    else
                    {
                        Pokemon pokemon = Core.Player.Pokemons[Int(argument)];
                        for (int d = 0; d < pokemon.Attacks.Count; d++) { if (pokemon.Attacks[d].CurrentPP < pokemon.Attacks[d].MaxPP) { isFullyHealed = false; } }
                        if (pokemon.HP < pokemon.MaxHP) { isFullyHealed = false; }
                        if (pokemon.Status != Pokemon.StatusProblems.None) { isFullyHealed = false; }
                    }
                    return ReturnBoolean(isFullyHealed);
                }
            }
            return DefaultNull;
        }

        private static Object DoOverworldPokemon(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "position":
                {
                    String[] args = argument.Split(',');
                    if (argument.Equals("") == false)
                    {
                        String s = String.Empty;
                        foreach (String a in args)
                        {
                            switch (a) {
                                case "x": s = s.Equals("") == false ? s + "," + Int(Screen.Level.OverworldPokemon.Position.X) : Int(Screen.Level.OverworldPokemon.Position.X).ToString(); break;
                                case "y": s = s.Equals("") == false ? s + "," + Int(Screen.Level.OverworldPokemon.Position.Y) : Int(Screen.Level.OverworldPokemon.Position.Y).ToString(); break;
                                case "z": s = s.Equals("") == false ? s + "," + Int(Screen.Level.OverworldPokemon.Position.Z) : Int(Screen.Level.OverworldPokemon.Position.Z).ToString(); break;
                            }
                        }
                        return s;
                    }
                    return Int(Screen.Level.OverworldPokemon.Position.X) + "," + Int(Screen.Level.OverworldPokemon.Position.Y) + "," + Int(Screen.Level.OverworldPokemon.Position.Z);
                }
                case "visible": return ReturnBoolean(Screen.Level.OverworldPokemon.IsVisible());
                case "id": return Screen.Level.OverworldPokemon.PokemonID;
                case "skin":
                    if (Screen.Level.OverworldPokemon.PokemonReference != null)
                    {
                        String shiny = Screen.Level.OverworldPokemon.PokemonReference.IsShiny == true ? "Shiny" : "Normal";
                        String addition = PokemonForms.GetOverworldAddition(Screen.Level.OverworldPokemon.PokemonReference);
                        return "Pokemon\\Overworld\\" + shiny + "\\" + Screen.Level.OverworldPokemon.PokemonReference.Number + addition;
                    }
                    break;
            }
            return DefaultNull;
        }

        private static Object DoPlayer(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "position":
                {
                    String[] args = argument.Split(',');
                    if (argument.Equals("") == false)
                    {
                        String s = String.Empty;
                        foreach (String a in args)
                        {
                            switch (a)
                            {
                                case "x": s = s.Equals("") == false ? s + "," + Int(Screen.Camera.Position.X) : Int(Screen.Camera.Position.X).ToString(); break;
                                case "y": s = s.Equals("") == false ? s + "," + Int(Screen.Camera.Position.Y) : Int(Screen.Camera.Position.Y).ToString(); break;
                                case "z": s = s.Equals("") == false ? s + "," + Int(Screen.Camera.Position.Z) : Int(Screen.Camera.Position.Z).ToString(); break;
                            }
                        }
                        return s;
                    }
                    return Int(Screen.Camera.Position.X) + "," + Int(Screen.Camera.Position.Y) + "," + Int(Screen.Camera.Position.Z);
                }
                case "hasbadge": return ReturnBoolean(Core.Player.Badges.Contains(Int(argument)));
                case "hasfrontieremblem":
                {
                    String id = String.Empty; bool? checkType = null;
                    if (argument.Equals("") == false)
                    {
                        if (argument.Split(',').Length == 1) { id = argument; }
                        else { id = argument.GetSplit(0, ","); checkType = ScriptConversion.ToBoolean(argument.GetSplit(1, ",")); }
                    }
                    if (checkType.HasValue == true)
                    {
                        return checkType.Value == false ? ReturnBoolean(ActionScript.IsRegistered("frontier_" + id + "_silver")) : ReturnBoolean(ActionScript.IsRegistered("frontier_" + id + "_gold"));
                    }
                    if (ActionScript.IsRegistered("frontier_" + id + "_gold") == true) { return "true"; }
                    return ReturnBoolean(ActionScript.IsRegistered("frontier_" + id + "_silver"));
                }
                case "skin": return Core.Player.Skin;
                case "velocity": return (int)((OverworldCamera)Screen.Camera)._moved;
                case "speed": return Screen.Camera.Speed / 0.04f;
                case "isrunning": return ReturnBoolean(Core.Player.IsRunning());
                case "ismoving": return ReturnBoolean(Screen.Camera.IsMoving);
                case "facing": return Screen.Camera.GetPlayerFacingDirection().ToString();
                case "compass":
                    switch (Screen.Camera.GetPlayerFacingDirection())
                    { case 0: return "north"; case 1: return "west"; case 2: return "south"; case 3: return "east"; }
                    break;
                case "money": return Core.Player.Money.ToString();
                case "name": return Core.Player.Name;
                case "gender": return Core.Player.Gender;
                case "bp": return Core.Player.BP.ToString();
                case "coins": return Core.Player.Coins.ToString();
                case "badges": return Core.Player.Badges.Count;
                case "thirdperson": return ReturnBoolean(((OverworldCamera)Screen.Camera).ThirdPerson);
                case "rival": case "rivalname": return Core.Player.RivalName;
                case "rivalskin": return Core.Player.RivalSkin;
                case "ot": return Core.Player.OT;
                case "gamejoltid": return Core.GameJoltSave.GameJoltID;
                case "haspokedex": return ReturnBoolean(Core.Player.HasPokedex);
                case "haspokegear": return ReturnBoolean(Core.Player.HasPokegear);
                case "isgamejolt":
                    if (argument.Equals("") == false && ScriptConversion.ToBoolean(argument) == true)
                    {
                        return ReturnBoolean(Core.Player.IsGameJoltSave == true && GameJolt.LogInScreen.UserBanned(Core.GameJoltSave.GameJoltID) == false);
                    }
                    return ReturnBoolean(Core.Player.IsGameJoltSave);
                case "lastrestplace": return Core.Player.LastRestPlace + "," + Core.Player.LastRestPlacePosition;
            }
            return DefaultNull;
        }

        private static Object DoEnvironment(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "daytime": return World.GetTime().ToString();
                case "daytimeid": return ((int)World.GetTime()).ToString();
                case "season": return World.CurrentSeason.ToString();
                case "seasonid": return ((int)World.CurrentSeason).ToString();
                case "day": return DateTime.Now.DayOfWeek.ToString();
                case "dayofyear": return DateTime.Now.DayOfYear.ToString();
                case "dayinformation": return DateTime.Now.DayOfWeek.ToString() + "," + World.GetTime().ToString();
                case "week": return World.WeekOfYear.ToString();
                case "hour": return DateTime.Now.Hour.ToString();
                case "year": return DateTime.Now.Year.ToString();
                case "weather": case "mapweather": case "currentmapweather": return Screen.Level.World.CurrentMapWeather.ToString();
                case "weatherid": case "mapweatherid": case "currentmapweatherid": return ((int)Screen.Level.World.CurrentMapWeather).ToString();
                case "regionweather": return World.GetCurrentRegionWeather().ToString();
                case "regionweatherid": return ((int)World.GetCurrentRegionWeather()).ToString();
                case "canfly": return ReturnBoolean(Screen.Level.CanFly);
                case "candig": return ReturnBoolean(Screen.Level.CanDig);
                case "canteleport": return ReturnBoolean(Screen.Level.CanTeleport);
                case "wildpokemongrass": return ReturnBoolean(Screen.Level.WildPokemonGrass);
                case "wildpokemonwater": return ReturnBoolean(Screen.Level.WildPokemonWater);
                case "wildpokemoneverywhere": return ReturnBoolean(Screen.Level.WildPokemonFloor);
                case "isdark": return ReturnBoolean(Screen.Level.IsDark);
                case "region":
                    if (argument.Equals("") == false) { return Screen.Level.CurrentRegion.Split(',')[Int(argument)]; }
                    return Screen.Level.CurrentRegion;
                case "regionalform":
                    if (argument.Equals("") == false) { return Screen.Level.RegionalForm.Split(',')[Int(argument)]; }
                    return Screen.Level.RegionalForm;
                case "graphicstyle": return ReturnBoolean(Core.GameOptions.GraphicStyle == 1);
            }
            return DefaultNull;
        }

        private static Object DoRegister(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "registered": return ReturnBoolean(ActionScript.IsRegistered(argument));
                case "count": return Core.Player.RegisterData.CountSplits(",") + 1;
                case "type":
                {
                    Object[] rc = ActionScript.GetRegisterValue(argument);
                    if (rc[0] == null || rc[1] == null) { Logger.Log(Logger.LogTypes.Warning, "ScriptComparer.cs: (<register." + command + ">) The requested register \"" + argument + "\" doesn't exist."); return DefaultNull; }
                    return (String)rc[1];
                }
                case "value":
                {
                    Object[] rc = ActionScript.GetRegisterValue(argument);
                    if (rc[0] == null || rc[1] == null) { Logger.Log(Logger.LogTypes.Warning, "ScriptComparer.cs: (<register." + command + ">) The requested register \"" + argument + "\" doesn't exist."); return DefaultNull; }
                    String lType = (String)rc[1]; String lValue = (String)rc[0];
                    switch (lType.ToLower())
                    {
                        case "bool": case "boolean": return ReturnBoolean(ScriptConversion.ToBoolean(lValue));
                        case "sng": case "single": return Dbl(lValue);
                        case "int": case "integer": return Int(lValue);
                        case "str": case "string": return lValue;
                        default: Logger.Log(Logger.LogTypes.Warning, "ScriptComparer.cs: (<register." + command + ">) Invalid lType: " + lType + ". Assuming str."); return lValue;
                    }
                }
            }
            return DefaultNull;
        }

        private static Object DoSystem(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "token": return Localization.GetString(argument);
                case "button":
                {
                    Microsoft.Xna.Framework.Input.Keys key = Microsoft.Xna.Framework.Input.Keys.None; String buttonName = argument;
                    switch (buttonName.ToLower())
                    {
                        case "moveforward": key = KeyBindings.ForwardMoveKey; break; case "moveleft": key = KeyBindings.LeftMoveKey; break;
                        case "movebackward": key = KeyBindings.BackwardMoveKey; break; case "moveright": key = KeyBindings.RightMoveKey; break;
                        case "run": key = KeyBindings.RunKey; break; case "openmenu": key = KeyBindings.OpenInventoryKey; break;
                        case "chat": key = KeyBindings.ChatKey; break; case "special": case "phone": case "pokegear": key = KeyBindings.SpecialKey; break;
                        case "muteaudio": case "mutemusic": key = KeyBindings.MuteAudioKey; break;
                        case "cameraleft": case "left": key = KeyBindings.LeftKey; break; case "cameraright": case "right": key = KeyBindings.RightKey; break;
                        case "cameraup": case "up": key = KeyBindings.UpKey; break; case "cameradown": case "down": key = KeyBindings.DownKey; break;
                        case "cameralock": key = KeyBindings.CameraLockKey; break; case "guicontrol": case "hidegui": key = KeyBindings.GUIControlKey; break;
                        case "screenshot": key = KeyBindings.ScreenshotKey; break; case "debugcontrol": key = KeyBindings.DebugKey; break;
                        case "perspectiveswitch": key = KeyBindings.PerspectiveSwitchKey; break; case "fullscreen": key = KeyBindings.FullScreenKey; break;
                        case "enter1": key = KeyBindings.EnterKey1; break; case "enter2": key = KeyBindings.EnterKey2; break;
                        case "back1": key = KeyBindings.BackKey1; break; case "back2": key = KeyBindings.BackKey2; break;
                        case "escape": case "esc": key = KeyBindings.EscapeKey; break; case "onlinestatus": key = KeyBindings.OnlineStatusKey; break;
                        case "lighting": key = KeyBindings.LightKey; break;
                    }
                    if (key != Microsoft.Xna.Framework.Input.Keys.None) { buttonName = Localization.GetString("keyboard_key_" + KeyBindings.GetKeyName(key), KeyBindings.GetKeyName(key)); }
                    return buttonName;
                }
                case "scripttrigger": return ActionScript.ScriptTrigger;
                case "random":
                {
                    int minRange = 0; int maxRange = 1;
                    if (argument.Equals("") == false)
                    {
                        if (argument.Contains(",") == true) { minRange = Int(argument.GetSplit(0)); maxRange = Int(argument.GetSplit(1)); }
                        else if (StringHelper.IsNumeric(argument) == true) { maxRange = Int(argument); }
                    }
                    return Core.Random.Next(minRange, maxRange + 1);
                }
                case "chooserandom":
                {
                    String[] args = argument.Split(','); List<String> chooseList = [];
                    foreach (String a in args)
                    {
                        if (a.Contains("-") == true && ScriptConversion.IsArithmeticExpression(a.GetSplit(0, "-")) == true && ScriptConversion.IsArithmeticExpression(a.GetSplit(1, "-")) == true)
                        {
                            for (int i = Int(a.GetSplit(0, "-")); i <= Int(a.GetSplit(1, "-")); i++) { chooseList.Add(i.ToString()); }
                        }
                        else { chooseList.Add(a); }
                    }
                    if (chooseList.Count > 0) { return chooseList[Core.Random.Next(0, chooseList.Count)]; }
                    break;
                }
                case "unixtimestamp": return (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
                case "dayofyear": return DateTime.Now.DayOfYear;
                case "year": return DateTime.Now.Year;
                case "booltoint": return argument.ToLower().Equals("true") == true ? "1" : "0";
                case "startswith":
                {
                    String[] args = argument.Split(','); String[] checks = args[1].Split(';');
                    foreach (String s in checks) { if (args[0].ToLower().StartsWith(s.ToLower()) == true) { return ReturnBoolean(true); } }
                    return ReturnBoolean(false);
                }
                case "contains":
                {
                    String[] args = argument.Split(','); String[] checks = args[1].Split(';');
                    foreach (String s in checks) { if (args[0].ToLower().Contains(s.ToLower()) == true) { return ReturnBoolean(true); } }
                    return ReturnBoolean(false);
                }
                case "calcint": case "int": return Int(argument);
                case "calcsng": case "sng": return Dbl(argument);
                case "sort":
                {
                    String[] args = argument.Split(','); String sortMode = args[0]; int returnIndex = Int(args[1]); List<String> sortList = [];
                    for (int i = 2; i < args.Length; i++) { sortList.Add(args[i]); }
                    if (sortMode.ToLower().Equals("ascending") == true) { return sortList.OrderBy(x => x).ToList()[returnIndex]; }
                    if (sortMode.ToLower().Equals("descending") == true) { return sortList.OrderByDescending(x => x).ToList()[returnIndex]; }
                    return DefaultNull;
                }
                case "isinsightscript": return ReturnBoolean(ActionScript.IsInSightScript);
                case "lastinput": return InputScreen.LastInput;
                case "return": return ScriptV2.TempReturn;
                case "isint": case "issng": return ReturnBoolean(ScriptConversion.IsArithmeticExpression(argument));
                case "chrw":
                {
                    String[] chars = argument.Split(','); String output = String.Empty;
                    foreach (String c in chars) { if (StringHelper.IsNumeric(c) == true) { output += StringHelper.GetChar(Int(c)); } }
                    return output;
                }
                case "scriptlevel": return ActionScript.ScriptLevelIndex.ToString();
                case "language": return Localization.LanguageSuffix;
                case "fileexists": return ReturnBoolean(GameModeManager.ContentFileExists(argument));
            }
            return DefaultNull;
        }

        private static Object DoNPC(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "position":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC == null) { return DefaultNull; }
                    String[] args = argument.Split(',');
                    if (args.Length > 1)
                    {
                        String s = String.Empty;
                        for (int i = 1; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "x": s = s.Equals("") == false ? s + "," + Int(targetNPC.Position.X) : Int(targetNPC.Position.X).ToString(); break;
                                case "y": s = s.Equals("") == false ? s + "," + Int(targetNPC.Position.Y) : Int(targetNPC.Position.Y).ToString(); break;
                                case "z": s = s.Equals("") == false ? s + "," + Int(targetNPC.Position.Z) : Int(targetNPC.Position.Z).ToString(); break;
                            }
                        }
                        return s;
                    }
                    return Int(targetNPC.Position.X) + "," + Int(targetNPC.Position.Y) + "," + Int(targetNPC.Position.Z);
                }
                case "exists": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return ReturnBoolean(n != null); }
                case "ismoving": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? ReturnBoolean(n.Moved != 0.0f) : (Object)DefaultNull; }
                case "moved": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? (Object)n.Moved.ToString() : DefaultNull; }
                case "skin": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? (Object)n.TextureID : DefaultNull; }
                case "facing": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? (Object)n.faceRotation : DefaultNull; }
                case "id": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? (Object)n.NPCID : DefaultNull; }
                case "name": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? (Object)n.Name : DefaultNull; }
                case "action": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? (Object)n.ActionValue : DefaultNull; }
                case "additionalvalue": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? (Object)n.AdditionalValue : DefaultNull; }
                case "movement": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? (Object)n.Movement.ToString() : DefaultNull; }
                case "hasmoverectangles": { NPC? n = Screen.Level.GetNPC(Int(argument.GetSplit(0))); return n != null ? ReturnBoolean(n.MoveRectangles.Count > 0) : (Object)DefaultNull; }
                case "trainertexture": return new BattleSystem.Trainer(argument).SpriteName;
            }
            return DefaultNull;
        }

        private static Object DoInventory(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "countitem": return Core.Player.Inventory.GetItemAmount(argument.GetSplit(0));
                case "countitems": { int c = 0; for (int i = 0; i < Core.Player.Inventory.Count; i++) { c += Core.Player.Inventory[i].Amount; } return c; }
                case "name":
                {
                    String itemID = argument.GetSplit(0);
                    if (argument.Contains(",") == true)
                    {
                        switch (argument.GetSplit(1).ToLower())
                        {
                            case "p": case "plural": return Items.Item.GetItemByID(itemID)!.OneLinePluralName();
                            case "s": case "singular": return Items.Item.GetItemByID(itemID)!.OneLineName();
                        }
                    }
                    return Items.Item.GetItemByID(itemID)!.Name;
                }
                case "id": { Items.Item? item = Items.Item.GetItemByName(argument); return item != null ? (Object)item.ID : 0; }
                case "juicecolor":
                {
                    String itemID = argument.GetSplit(0);
                    if (Items.Item.GetItemByID(itemID)!.PluralName.ToLower().EndsWith("berries") == true)
                    {
                        return ((Items.Berry)Items.Item.GetItemByID(itemID)!).JuiceColor;
                    }
                    return "black";
                }
                case "juicegroup":
                {
                    String itemID = argument.GetSplit(0);
                    if (Items.Item.GetItemByID(itemID)!.PluralName.ToLower().EndsWith("berries") == true)
                    {
                        return ((Items.Berry)Items.Item.GetItemByID(itemID)!).JuiceGroup;
                    }
                    return 0;
                }
                case "selected": return NewInventoryScreen.SelectedItem;
            }
            return DefaultNull;
        }

        private static Object DoStorage(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "get":
                {
                    String type = argument.Remove(argument.IndexOf(","));
                    String name = argument.Remove(0, argument.IndexOf(",") + 1);
                    return ScriptStorage.GetObject(type, name);
                }
                case "count": return ScriptStorage.Count(argument);
            }
            return DefaultNull;
        }

        private static Object DoPhone(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;

            switch (command.ToLower())
            {
                case "callflag": return GameJolt.PokegearScreen.Call_Flag;
                case "got": return ReturnBoolean(Core.Player.HasPokegear);
            }
            return DefaultNull;
        }

        private static Object DoEntity(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            Entity? ent = Screen.Level.GetEntity(Int(argument.GetSplit(0)));
            if (ent != null)
            {
                switch (command.ToLower())
                {
                    case "visible": return ReturnBoolean(ent.Visible);
                    case "opacity": return ent.Opacity * 100;
                    case "position": return ent.Position.X.ToString().ReplaceDecSeparator() + "," + ent.Position.Y.ToString().ReplaceDecSeparator() + "," + ent.Position.Z.ToString().ReplaceDecSeparator();
                    case "positionx": return ent.Position.X.ToString().ReplaceDecSeparator();
                    case "positiony": return ent.Position.Y.ToString().ReplaceDecSeparator();
                    case "positionz": return ent.Position.Z.ToString().ReplaceDecSeparator();
                    case "rotation": return ent.Rotation.X.ToString() + "," + ent.Rotation.Y.ToString() + "," + ent.Rotation.Z.ToString();
                    case "scale": return ent.Scale.X.ToString().ReplaceDecSeparator() + "," + ent.Scale.Y.ToString().ReplaceDecSeparator() + "," + ent.Scale.Z.ToString().ReplaceDecSeparator();
                    case "additionalvalue": return ent.AdditionalValue;
                    case "collision": return ReturnBoolean(ent.Collision);
                }
            }
            return DefaultNull;
        }

        private static Object DoLevel(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;

            switch (command.ToLower())
            {
                case "mapfile": case "levelfile": return Screen.Level.LevelFile;
                case "filename": return Path.GetFileNameWithoutExtension(Screen.Level.LevelFile);
                case "riding": return ReturnBoolean(Screen.Level.Riding);
                case "surfing": return ReturnBoolean(Screen.Level.Surfing);
                case "musicloop": return Path.GetFileNameWithoutExtension(Screen.Level.MusicLoop);
                case "daytime": return Screen.Level.DayTime;
                case "environmenttype": return Int(Screen.Level.EnvironmentType);
                case "loadoffsetmaps": return ReturnBoolean(Core.GameOptions.LoadOffsetMaps > 0);
            }
            return DefaultNull;
        }

        private static Object DoBattle(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "defeatmessage": return new BattleSystem.Trainer(argument).DefeatMessage;
                case "intromessage": return new BattleSystem.Trainer(argument).IntroMessage;
                case "outromessage": return new BattleSystem.Trainer(argument).OutroMessage;
                case "won": return ReturnBoolean(BattleSystem.Battle.Won);
                case "caught": return ReturnBoolean(BattleSystem.Battle.Caught);
                case "trainername":
                {
                    int index = argument.Equals("") == false ? Int(argument) : 0;
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.BattleScreen) == true)
                    {
                        BattleSystem.BattleScreen bs = (BattleSystem.BattleScreen)Core.CurrentScreen;
                        return index == 1 ? (Object)bs.Trainer.Name2 : bs.Trainer.Name;
                    }
                    break;
                }
                case "pokemonname":
                {
                    bool own = argument.Equals("") == false ? ScriptConversion.ToBoolean(argument) : true;
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.BattleScreen) == true)
                    {
                        BattleSystem.BattleScreen bs = (BattleSystem.BattleScreen)Core.CurrentScreen;
                        return own == true ? (Object)bs.SelfPokemon!.GetDisplayName() : bs.OpponentPokemon!.GetDisplayName();
                    }
                    break;
                }
                case "pokemonid":
                {
                    bool own = argument.Equals("") == false ? ScriptConversion.ToBoolean(argument) : true;
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.BattleScreen) == true)
                    {
                        BattleSystem.BattleScreen bs = (BattleSystem.BattleScreen)Core.CurrentScreen;
                        Pokemon p = own == true ? bs.SelfPokemon! : bs.OpponentPokemon!;
                        return PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);
                    }
                    break;
                }
                case "pokemonitem":
                {
                    bool own = argument.Equals("") == false ? ScriptConversion.ToBoolean(argument) : true;
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.BattleScreen) == true)
                    {
                        BattleSystem.BattleScreen bs = (BattleSystem.BattleScreen)Core.CurrentScreen;
                        Pokemon p = own == true ? bs.SelfPokemon! : bs.OpponentPokemon!;
                        if (p.Item != null) { return p.Item.IsGameModeItem == true ? (Object)p.Item.gmID : p.Item.ID; }
                        return -1;
                    }
                    break;
                }
            }
            return DefaultNull;
        }

        private static Object DoDaycare(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "pokemonid":
                {
                    int dcID = Int(argument.GetSplit(0)); int pi = Int(argument.GetSplit(1));
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dcID.ToString() + "|" + pi.ToString() + "|") == true)
                        { return Pokemon.GetPokemonByData(line.Remove(0, line.IndexOf("{"))).Number; }
                    }
                    return 0;
                }
                case "pokemonname":
                {
                    int dcID = Int(argument.GetSplit(0)); int pi = Int(argument.GetSplit(1));
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dcID.ToString() + "|" + pi.ToString() + "|") == true)
                        { return Pokemon.GetPokemonByData(line.Remove(0, line.IndexOf("{"))).GetDisplayName(); }
                    }
                    return "missingno";
                }
                case "shinyindicator":
                {
                    int dcID = Int(argument.GetSplit(0)); int pi = Int(argument.GetSplit(1));
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dcID.ToString() + "|" + pi.ToString() + "|") == true)
                        { return Pokemon.GetPokemonByData(line.Remove(0, line.IndexOf("{"))).IsShiny == true ? "S" : "N"; }
                    }
                    return "N";
                }
                case "pokemonsprite":
                {
                    int dcID = Int(argument.GetSplit(0)); int pi = Int(argument.GetSplit(1));
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dcID.ToString() + "|" + pi.ToString() + "|") == true)
                        {
                            Pokemon p = Pokemon.GetPokemonByData(line.Remove(0, line.IndexOf("{")));
                            String shiny = p.IsShiny == true ? "S" : "N";
                            return "[POKEMON|" + shiny + "]" + p.Number.ToString() + PokemonForms.GetOverworldAddition(p);
                        }
                    }
                    return "[POKEMON|N]10";
                }
                case "countpokemon":
                {
                    int count = 0;
                    if (Core.Player.DaycareData.Equals("") == false)
                    {
                        int dcID = Int(argument);
                        foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                        { if (line.StartsWith(dcID.ToString() + "|") == true) { count++; } }
                    }
                    return count;
                }
                case "haspokemon":
                {
                    int count = 0;
                    if (Core.Player.DaycareData.Equals("") == false)
                    {
                        int dcID = Int(argument);
                        foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                        { if (line.StartsWith(dcID.ToString() + "|") == true) { count++; } }
                    }
                    return ReturnBoolean(count > 0);
                }
                case "canswim":
                {
                    int dcID = Int(argument.GetSplit(0)); int pi = Int(argument.GetSplit(1));
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dcID.ToString() + "|" + pi.ToString() + "|") == true)
                        { return ReturnBoolean(Pokemon.GetPokemonByData(line.Remove(0, line.IndexOf("{"))).CanSwim); }
                    }
                    return ReturnBoolean(false);
                }
                case "hasegg":
                {
                    int dcID = Int(argument);
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    { if (line.StartsWith(dcID.ToString() + "|Egg|") == true) { return ReturnBoolean(true); } }
                    return ReturnBoolean(false);
                }
                case "grownlevels":
                {
                    int dcID = Int(argument.GetSplit(0)); int pi = Int(argument.GetSplit(1));
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dcID.ToString() + "|" + pi.ToString() + "|") == true)
                        {
                            int startStep = int.Parse(line.Split('|')[2]);
                            Pokemon p = Pokemon.GetPokemonByData(line.Remove(0, line.IndexOf("{")));
                            int startLevel = p.Level; p.GetExperience(Core.Player.DaycareSteps - startStep, true);
                            return p.Level - startLevel;
                        }
                    }
                    break;
                }
                case "currentlevel":
                {
                    int dcID = Int(argument.GetSplit(0)); int pi = Int(argument.GetSplit(1));
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dcID.ToString() + "|" + pi.ToString() + "|") == true)
                        {
                            int startStep = int.Parse(line.Split('|')[2]);
                            Pokemon p = Pokemon.GetPokemonByData(line.Remove(0, line.IndexOf("{")));
                            p.GetExperience(Core.Player.DaycareSteps - startStep, true);
                            return p.Level;
                        }
                    }
                    break;
                }
                case "canbreed":
                {
                    int dcID = Int(argument.GetSplit(0)); bool wm = true;
                    if (argument.Contains(",") == true) { wm = ScriptConversion.ToBoolean(argument.GetSplit(1)); }
                    return Daycare.CanBreed(dcID, wm);
                }
            }
            return DefaultNull;
        }

        private static Object DoRival(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;

            switch (command.ToLower())
            {
                case "name": return Core.Player.RivalName;
                case "skin": return Core.Player.RivalSkin;
            }
            return DefaultNull;
        }

        private static Object DoMath(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "int": return Int(argument);
                case "sng": return Sng(argument);
                case "dbl": return Dbl(argument);
                case "abs": return Math.Abs(Dbl(argument));
                case "ceiling": return Math.Ceiling(Dbl(argument));
                case "floor": return Math.Floor(Dbl(argument));
                case "isint": case "issng": case "isdbl": return ReturnBoolean(ScriptConversion.IsArithmeticExpression(argument));
                case "clamp":
                {
                    String[] args = argument.Split(',');
                    return Dbl(args[0]).Clamp(Dbl(args[1]), Dbl(args[2]));
                }
                case "rollover":
                {
                    String[] args = argument.Split(',');
                    double n = Dbl(args[0]); double min = Dbl(args[1]); double max = Dbl(args[2]);
                    double diff = (max - min) + 1;
                    while (n > max) { n -= diff; }
                    while (n < min) { n += diff; }
                    return n;
                }
            }
            return DefaultNull;
        }

        private static Object DoPokedex(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "caught": return Pokedex.CountEntries(Core.Player.PokedexData, [2, 3]).ToString();
                case "shiny": return Pokedex.CountEntries(Core.Player.PokedexData, [3]).ToString();
                case "seen": return Pokedex.CountEntries(Core.Player.PokedexData, [1]).ToString();
                case "dexcaught": { int di = Int(argument); return Core.Player.Pokedexes[di].Obtained; }
                case "dexseen": { int di = Int(argument); return Core.Player.Pokedexes[di].Seen + Core.Player.Pokedexes[di].Obtained; }
                case "getheight": case "getweight": case "getentry": case "getcolor": case "getspecies": case "getname": case "getability":
                {
                    int id = Int(argument.GetSplit(0, ",").GetSplit(0, "_").GetSplit(0, ";")); String ad = String.Empty;
                    String dexID = argument;
                    if (argument.Contains(";") == true) { if (Pokemon.PokemonDataExists(argument) == false) { dexID = argument.GetSplit(0, ";"); } ad = argument.GetSplit(1, ";"); }
                    else if (argument.Contains("_") == true) { ad = PokemonForms.GetAdditionalValueFromDataFile(argument); }
                    if (Pokemon.PokemonDataExists(dexID) == true)
                    {
                        Pokemon p = Pokemon.GetPokemonByID(id, ad);
                        switch (command.ToLower())
                        {
                            case "getheight": return p.pokedexEntry?.height ?? 0;
                            case "getweight": return p.pokedexEntry?.weight ?? 0;
                            case "getentry":
                            {
                                String formName = PokemonForms.GetFormName(p); if (formName.Equals("") == true) { formName = p.Name; }
                                if (Localization.LanguageSuffix.Equals("en") == false && Localization.TokenExists("pokemon_desc_" + formName) == true)
                                    return Localization.GetString("pokemon_desc_" + formName, p.pokedexEntry?.text ?? "");
                                return p.pokedexEntry?.text ?? "";
                            }
                            case "getcolor": return p.pokedexEntry?.color ?? "";
                            case "getspecies":
                            {
                                if (Localization.LanguageSuffix.Equals("en") == false && Localization.TokenExists("pokemon_species_" + dexID) == true)
                                    return Localization.GetString("pokemon_species_" + dexID, p.pokedexEntry?.species ?? "");
                                return p.pokedexEntry?.species ?? "";
                            }
                            case "getname": return p.GetName();
                            case "getability":
                            {
                                String slot = argument.GetSplit(1);
                                switch (slot)
                                {
                                    case "0": return p.newAbilities[Core.Random.Next(0, p.newAbilities.Count)].ID;
                                    case "1": return p.newAbilities[0].ID;
                                    case "2": return p.newAbilities[p.newAbilities.Count - 1].ID;
                                    case "3": return p.hiddenAbility.ID;
                                }
                                break;
                            }
                        }
                    }
                    break;
                }
                case "pokemoncaught":
                {
                    String dexID = argument; if (argument.Contains(";") == true && Pokemon.PokemonDataExists(argument) == false) { dexID = argument.GetSplit(0, ";"); }
                    if (Pokemon.PokemonDataExists(dexID) == true) { return ReturnBoolean(Pokedex.GetEntryType(Core.Player.PokedexData, dexID) > 1); }
                    break;
                }
                case "pokemonseen":
                {
                    String dexID = argument; if (argument.Contains(";") == true && Pokemon.PokemonDataExists(argument) == false) { dexID = argument.GetSplit(0, ";"); }
                    if (Pokemon.PokemonDataExists(dexID) == true) { return ReturnBoolean(Pokedex.GetEntryType(Core.Player.PokedexData, dexID) > 0); }
                    break;
                }
            }
            return DefaultNull;
        }

        private static Object DoRadio(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;

            switch (command.ToLower())
            {
                case "currentchannel":
                    return Screen.Level.SelectedRadioStation == null ? (Object)"" : Screen.Level.SelectedRadioStation.Name;
            }
            return DefaultNull;
        }

        private static Object DoCamera(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;

            OverworldCamera c = (OverworldCamera)Screen.Camera;
            Microsoft.Xna.Framework.Vector3 position = Core.CurrentScreen.Identification.Equals(Screen.Identifications.NewGameScreen) == true ? c.Position : c.ThirdPersonOffset;

            switch (command.ToLower())
            {
                case "isfixed": return ReturnBoolean(c.Fixed);
                case "x": return position.X.ToString().ReplaceDecSeparator();
                case "y": return position.Y.ToString().ReplaceDecSeparator();
                case "z": return position.Z.ToString().ReplaceDecSeparator();
                case "yaw": return c.Yaw.ToString().ReplaceDecSeparator();
                case "pitch": return c.Pitch.ToString().ReplaceDecSeparator();
                case "thirdperson": return ReturnBoolean(c.ThirdPerson);
            }
            return DefaultNull;
        }

        private static Object DoFileSystem(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "pathsplit":
                {
                    int index = Int(argument.Remove(argument.IndexOf(",")));
                    String folderpath = argument.Remove(0, argument.IndexOf(",") + 1);
                    String[] folderSplits = folderpath.Split('\\');
                    return folderSplits[index];
                }
                case "pathsplitcount": return argument.Split('\\').Length;
                case "pathup": return argument.Contains("\\") == true ? argument.Remove(argument.LastIndexOf("\\")) : (Object)DefaultNull;
            }
            return DefaultNull;
        }

        private static Object DoScreen(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;

            switch (command.ToLower())
            {
                case "selectedskin": return NewNewGameScreen.CharacterSelectionScreen.SelectedSkin;
                case "selectedname": return NewNewGameScreen.CharacterSelectionScreen.SelectedName;
                case "selectedgender": return NewNewGameScreen.CharacterSelectionScreen.SelectedGender;
            }
            return DefaultNull;
        }

        private static Object DoScript(String subClass)
        {
            String command = GetSubClassArgumentPair(subClass).Command;
            String argument = GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "delay":
                {
                    String[] args = argument.Split(',');
                    if (ActionScript.IsRegistered("SCRIPTDELAY_" + args[0]) == true)
                    {
                        Object[] rc = ActionScript.GetRegisterValue("SCRIPTDELAY_" + args[0]);
                        if (rc[0] == null || rc[1] == null) { ActionScript.UnregisterID("SCRIPTDELAY_" + args[0], "str"); ActionScript.UnregisterID("SCRIPTDELAY_" + args[0]); return DefaultNull; }
                        switch (args[1].ToLower())
                        {
                            case "type": return ((String)rc[0]).GetSplit(0, ";");
                            case "script": return ((String)rc[0]).GetSplit(1, ";");
                            case "value":
                            {
                                String delayType = ((String)rc[0]).GetSplit(0, ";");
                                switch (delayType)
                                {
                                    case "steps": return Core.Player.ScriptDelaySteps;
                                    case "itemcount":
                                    {
                                        List<String> itemDelayList = Core.Player.ScriptDelayItems.Split(';').ToList();
                                        foreach (String entry in itemDelayList)
                                        { if (entry.GetSplit(0, ",").Equals(args[0]) == true) { return entry.GetSplit(3); } }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else { return ReturnBoolean(false); }
                    break;
                }
            }
            return DefaultNull;
        }
    }
}
