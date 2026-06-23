using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.ScriptVersion2
{
    public static partial class ScriptCommander
    {
        private static void DoPlayer(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "receivepokedex":
                    Core.Player.HasPokedex = true;
                    foreach (Pokemon p in Core.Player.Pokemons)
                    {
                        int i = p.IsShiny == true ? 3 : 2;
                        String dexID = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);
                        Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, i);
                    }
                    IsReady = true;
                    break;
                case "receivepokegear":
                    Core.Player.HasPokegear = true;
                    IsReady = true;
                    break;
                case "renamerival":
                {
                    if (GameModeManager.ActiveGameMode.IsDefaultGamemode == true && Core.Player.RivalSkin.Equals("4") == false)
                    {
                        Core.Player.RivalSkin = "4";
                    }
                    if (Core.Player.RivalSkin.Equals("") == true) { Core.Player.RivalSkin = "4"; }
                    Texture2D rivalTexture2D = TextureManager.GetTexture("Textures\\NPC\\" + Core.Player.RivalSkin);
                    Size rivalFrameSize;
                    if (rivalTexture2D.Width == rivalTexture2D.Height / 2)
                        rivalFrameSize = new Size(rivalTexture2D.Width / 2, rivalTexture2D.Height / 4);
                    else if (rivalTexture2D.Width == rivalTexture2D.Height)
                        rivalFrameSize = new Size(rivalTexture2D.Width / 4, rivalTexture2D.Height / 4);
                    else
                        rivalFrameSize = new Size(rivalTexture2D.Width / 3, rivalTexture2D.Height / 4);
                    Rectangle rivalRect = new Rectangle(0, rivalFrameSize.Height * 2, rivalFrameSize.Width, rivalFrameSize.Height);
                    Texture2D rivalSprite = TextureManager.GetTexture(rivalTexture2D, rivalRect);
                    Core.SetScreen(new NameObjectScreen(Core.CurrentScreen, rivalSprite, false, false, Core.Player.RivalName, "???", Script.NameRival));
                    IsReady = true;
                    CanContinue = false;
                    break;
                }
                case "setrivalskin":
                    Core.Player.RivalSkin = argument;
                    IsReady = true;
                    break;
                case "wearskin":
                    Screen.Level.OwnPlayer!.SetTexture(argument, false);
                    Screen.Level.OwnPlayer!.UpdateEntity();
                    IsReady = true;
                    break;
                case "setskin":
                    Core.Player.Skin = argument;
                    Screen.Level.OwnPlayer!.SetTexture(argument, false);
                    Screen.Level.OwnPlayer!.UpdateEntity();
                    IsReady = true;
                    break;
                case "setspeed":
                    Screen.Camera.Speed = Sng(argument) * 0.04f;
                    IsReady = true;
                    break;
                case "resetspeed":
                    Screen.Camera.Speed = 0.04f;
                    IsReady = true;
                    break;
                case "move":
                    if (Started == false)
                    {
                        Screen.Camera.Move(Sng(argument));
                        Started = true;
                        Screen.Level.OverworldPokemon.Visible = false;
                    }
                    else
                    {
                        Screen.Level.UpdateEntities();
                        Screen.Camera.Update();
                        if (Screen.Camera.IsMoving == false) { IsReady = true; Screen.Level.OverworldPokemon.Visible = false; }
                    }
                    break;
                case "moveasnyc": case "moveasync":
                    Screen.Camera.Move(Sng(argument));
                    IsReady = true;
                    Screen.Level.OverworldPokemon.Visible = false;
                    break;
                case "turn":
                {
                    int turnAmount = Int(argument.GetSplit(0, ","));
                    bool forceCameraTurn = argument.Split(',').Length > 1 ? ScriptConversion.ToBoolean(argument.GetSplit(1, ",")) : false;
                    if (Started == false)
                    {
                        Screen.Camera.Turn(turnAmount, forceCameraTurn);
                        Started = true;
                        Screen.Level.OverworldPokemon.Visible = false;
                    }
                    else
                    {
                        Screen.Camera.Update();
                        Screen.Level.UpdateEntities();
                        if (Screen.Camera.Turning == false) { IsReady = true; }
                    }
                    break;
                }
                case "dance":
                    Screen.Level.OwnPlayer!.isDancing = true;
                    if (Started == false)
                    {
                        Screen.Camera.Move(Sng(argument));
                        Started = true;
                        Screen.Level.OverworldPokemon.Visible = false;
                    }
                    else
                    {
                        Screen.Level.UpdateEntities();
                        Screen.Camera.Update();
                        if (Screen.Camera.IsMoving == false) { IsReady = true; Screen.Level.OverworldPokemon.Visible = false; }
                    }
                    break;
                case "turnasync":
                {
                    int turnAmount = Int(argument.GetSplit(0, ","));
                    bool forceCameraTurn = argument.Split(',').Length > 1 ? ScriptConversion.ToBoolean(argument.GetSplit(1, ",")) : false;
                    Screen.Camera.Turn(turnAmount, forceCameraTurn);
                    IsReady = true;
                    Screen.Level.OverworldPokemon.Visible = false;
                    break;
                }
                case "turntoasync":
                {
                    bool forceCameraTurn = argument.Split(',').Length > 1 ? ScriptConversion.ToBoolean(argument.GetSplit(1, ",")) : false;
                    int turns = Int(argument.GetSplit(0, ",")) - Screen.Camera.GetPlayerFacingDirection();
                    if (turns < 0) { turns += 4; }
                    if (turns > 0) { Screen.Camera.Turn(turns, forceCameraTurn); Screen.Level.OverworldPokemon.Visible = false; }
                    IsReady = true;
                    break;
                }
                case "turnto":
                {
                    bool forceCameraTurn = argument.Split(',').Length > 1 ? ScriptConversion.ToBoolean(argument.GetSplit(1, ",")) : false;
                    if (Started == false)
                    {
                        int turns = Int(argument.GetSplit(0, ",")) - Screen.Camera.GetPlayerFacingDirection();
                        if (turns < 0) { turns += 4; }
                        if (turns > 0 || forceCameraTurn == true)
                        {
                            Screen.Camera.Turn(turns, forceCameraTurn);
                            Started = true;
                            Screen.Level.OverworldPokemon.Visible = false;
                        }
                        else { IsReady = true; }
                    }
                    else
                    {
                        Screen.Camera.Update();
                        Screen.Level.UpdateEntities();
                        if (Screen.Camera.Turning == false) { IsReady = true; }
                    }
                    break;
                }
                case "warp":
                {
                    int commas = 0;
                    foreach (char c in argument) { if (c == ',') { commas++; } }
                    Vector3 cPosition = Screen.Camera.Position;
                    switch (commas)
                    {
                        case 5:
                            Screen.Level.WarpData.WarpDestination = argument.GetSplit(0);
                            Screen.Level.WarpData.WarpPosition = new Vector3(
                                Sng(argument.GetSplit(1).Replace("~", cPosition.X.ToString()).Replace(".", GameController.DecSeparator)),
                                Sng(argument.GetSplit(2).Replace("~", cPosition.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                Sng(argument.GetSplit(3).Replace("~", cPosition.Z.ToString()).Replace(".", GameController.DecSeparator)));
                            Screen.Level.WarpData.WarpRotations = Int(argument.GetSplit(4));
                            Screen.Level.WarpData.WarpSound = GetWarpSound(Int(argument.GetSplit(5)));
                            Screen.Level.WarpData.DoWarpInNextTick = true;
                            Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                            break;
                        case 4:
                            if (argument.GetSplit(0).Contains(".dat") == false)
                            {
                                Screen.Level.WarpData.WarpDestination = Screen.Level.LevelFile;
                                Screen.Level.WarpData.WarpPosition = new Vector3(
                                    Sng(argument.GetSplit(0).Replace("~", cPosition.X.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(argument.GetSplit(1).Replace("~", cPosition.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(argument.GetSplit(2).Replace("~", cPosition.Z.ToString()).Replace(".", GameController.DecSeparator)));
                                Screen.Level.WarpData.WarpRotations = Int(argument.GetSplit(3));
                                Screen.Level.WarpData.WarpSound = GetWarpSound(Int(argument.GetSplit(4)));
                                Screen.Level.WarpData.DoWarpInNextTick = true;
                                Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                            }
                            else
                            {
                                Screen.Level.WarpData.WarpDestination = argument.GetSplit(0);
                                Screen.Level.WarpData.WarpPosition = new Vector3(
                                    Sng(argument.GetSplit(1).Replace("~", cPosition.X.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(argument.GetSplit(2).Replace("~", cPosition.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(argument.GetSplit(3).Replace("~", cPosition.Z.ToString()).Replace(".", GameController.DecSeparator)));
                                Screen.Level.WarpData.WarpRotations = Int(argument.GetSplit(4));
                                Screen.Level.WarpData.WarpSound = "Warp_Exit";
                                Screen.Level.WarpData.DoWarpInNextTick = true;
                                Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                            }
                            break;
                        case 3:
                            if (argument.GetSplit(0).Contains(".dat") == false)
                            {
                                Screen.Level.WarpData.WarpDestination = Screen.Level.LevelFile;
                                Screen.Level.WarpData.WarpPosition = new Vector3(
                                    Sng(argument.GetSplit(0).Replace("~", cPosition.X.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(argument.GetSplit(1).Replace("~", cPosition.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(argument.GetSplit(2).Replace("~", cPosition.Z.ToString()).Replace(".", GameController.DecSeparator)));
                                Screen.Level.WarpData.WarpRotations = 0;
                                Screen.Level.WarpData.WarpSound = GetWarpSound(Int(argument.GetSplit(3)));
                                Screen.Level.WarpData.DoWarpInNextTick = true;
                                Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                            }
                            else
                            {
                                Screen.Level.WarpData.WarpDestination = argument.GetSplit(0);
                                Screen.Level.WarpData.WarpPosition = new Vector3(
                                    Sng(argument.GetSplit(1).Replace("~", cPosition.X.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(argument.GetSplit(2).Replace("~", cPosition.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(argument.GetSplit(3).Replace("~", cPosition.Z.ToString()).Replace(".", GameController.DecSeparator)));
                                Screen.Level.WarpData.WarpRotations = 0;
                                Screen.Level.WarpData.WarpSound = "Warp_Exit";
                                Screen.Level.WarpData.DoWarpInNextTick = true;
                                Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                            }
                            break;
                        case 2:
                            Screen.Camera.Position = new Vector3(
                                Sng(argument.GetSplit(0).Replace("~", cPosition.X.ToString()).Replace(".", GameController.DecSeparator)),
                                Sng(argument.GetSplit(1).Replace("~", cPosition.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                Sng(argument.GetSplit(2).Replace("~", cPosition.Z.ToString()).Replace(".", GameController.DecSeparator)));
                            break;
                        case 1:
                            if (argument.GetSplit(0).Contains(".dat") == true)
                            {
                                Screen.Level.WarpData.WarpDestination = argument.GetSplit(0);
                                Screen.Level.WarpData.WarpPosition = Screen.Camera.Position;
                                Screen.Level.WarpData.WarpRotations = 0;
                                Screen.Level.WarpData.WarpSound = GetWarpSound(Int(argument.GetSplit(1)));
                                Screen.Level.WarpData.DoWarpInNextTick = true;
                                Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                            }
                            break;
                        case 0:
                            Screen.Level.WarpData.WarpDestination = argument;
                            Screen.Level.WarpData.WarpPosition = Screen.Camera.Position;
                            Screen.Level.WarpData.WarpRotations = 0;
                            Screen.Level.WarpData.WarpSound = "Warp_Exit";
                            Screen.Level.WarpData.DoWarpInNextTick = true;
                            Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                            break;
                    }
                    Screen.Level.OverworldPokemon.warped = true;
                    Screen.Level.OverworldPokemon.Visible = false;
                    IsReady = true;
                    break;
                }
                case "stopmovement": Screen.Camera.StopMovement(); IsReady = true; break;
                case "preventmovement": ((OverworldCamera)Screen.Camera).PreventMovement = true; IsReady = true; break;
                case "allowmovement":
                    if (((OverworldCamera)Screen.Camera)._moved == 0.0f)
                    {
                        ((OverworldCamera)Screen.Camera).PreventMovement = false;
                        IsReady = true;
                    }
                    break;
                case "money": case "addmoney":
                    Core.Player.Money += Int(argument);
                    if (Core.Player.Money < 0) { Core.Player.Money = 0; }
                    IsReady = true;
                    break;
                case "removemoney":
                    Core.Player.Money -= Int(argument);
                    if (Core.Player.Money < 0) { Core.Player.Money = 0; }
                    IsReady = true;
                    break;
                case "setmovement":
                {
                    String[] movements = argument.Split(',');
                    Screen.Camera.PlannedMovement = new Vector3(Int(movements[0]), Sng(movements[1]), Int(movements[2]));
                    IsReady = true;
                    break;
                }
                case "resetmovement":
                    Screen.Camera.PlannedMovement = Vector3.Zero;
                    IsReady = true;
                    break;
                case "getbadge":
                    if (StringHelper.IsNumeric(argument) == true)
                    {
                        if (Core.Player.Badges.Contains(Int(argument)) == false)
                        {
                            Core.Player.Badges.Add(Int(argument));
                            SoundManager.PlaySound("Receive_Badge", true);
                            Screen.TextBox.TextColor = TextBox.PlayerColor;
                            Screen.TextBox.Show(Core.Player.Name + " received the~" + Badge.GetBadgeName(Int(argument)) + " Badge.", [], false, false);
                            Core.Player.AddPoints(10, "Got a badge.");
                        }
                    }
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "removebadge":
                    if (StringHelper.IsNumeric(argument) == true && Core.Player.Badges.Contains(Int(argument)) == true)
                    {
                        Core.Player.Badges.Remove(Int(argument));
                    }
                    IsReady = true;
                    break;
                case "addbadge":
                    if (StringHelper.IsNumeric(argument) == true && Core.Player.Badges.Contains(Int(argument)) == false)
                    {
                        Core.Player.Badges.Add(Int(argument));
                    }
                    IsReady = true;
                    break;
                case "addfrontieremblem":
                    if (argument.Split(',').Length == 1)
                    {
                        ActionScript.RegisterID("frontier_" + argument + "_gold");
                    }
                    else
                    {
                        if (ScriptConversion.ToBoolean(argument.GetSplit(1, ",")) == false)
                            ActionScript.RegisterID("frontier_" + argument.GetSplit(0, ",") + "_silver");
                        else
                            ActionScript.RegisterID("frontier_" + argument.GetSplit(0, ",") + "_gold");
                    }
                    IsReady = true;
                    break;
                case "removefrontieremblem":
                    if (argument.Split(',').Length == 1)
                    {
                        ActionScript.UnregisterID("frontier_" + argument + "_gold");
                        ActionScript.UnregisterID("frontier_" + argument + "_silver");
                    }
                    else
                    {
                        if (ScriptConversion.ToBoolean(argument.GetSplit(1, ",")) == false)
                            ActionScript.UnregisterID("frontier_" + argument.GetSplit(0, ",") + "_silver");
                        else
                            ActionScript.UnregisterID("frontier_" + argument.GetSplit(0, ",") + "_gold");
                    }
                    IsReady = true;
                    break;
                case "achieveemblem":
                    GameJolt.Emblem.AchieveEmblem(argument);
                    IsReady = true;
                    break;
                case "addbp":
                {
                    int bp = Int(argument);
                    Core.Player.BP += bp;
                    if (bp > 0) { PlayerStatistics.Track("Obtained BP", bp); }
                    IsReady = true;
                    break;
                }
                case "removebp":
                    Core.Player.BP -= Int(argument);
                    if (Core.Player.BP < 0) { Core.Player.BP = 0; }
                    IsReady = true;
                    break;
                case "addcoins": case "coins":
                {
                    int coins = Int(argument);
                    int coinCap = Int(GameModeManager.GetGameRuleValue("CoinCaseCap", "0"));
                    if (coinCap > 0 && Core.Player.Coins + coins > coinCap) { coins = coinCap - Core.Player.Coins; }
                    Core.Player.Coins += coins;
                    if (coins > 0) { PlayerStatistics.Track("Obtained Coins", coins); }
                    IsReady = true;
                    break;
                }
                case "removecoins":
                    Core.Player.Coins -= Int(argument);
                    if (Core.Player.Coins < 0) { Core.Player.Coins = 0; }
                    IsReady = true;
                    break;
                case "showrod":
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
                    {
                        OverworldScreen.DrawRodID = Int(argument);
                    }
                    IsReady = true;
                    break;
                case "hiderod":
                    OverworldScreen.DrawRodID = -1;
                    IsReady = true;
                    break;
                case "save":
                    Core.Player.SaveGame(false);
                    IsReady = true;
                    break;
                case "setname": Core.Player.Name = argument; IsReady = true; break;
                case "setrivalname": Core.Player.RivalName = argument; IsReady = true; break;
                case "showbattleanimations": Core.Player.ShowBattleAnimations = Int(argument); IsReady = true; break;
                case "setdifficulty": Core.Player.DifficultyMode = Int(argument).Clamp(0, 2); IsReady = true; break;
                case "setgender":
                    switch (argument)
                    {
                        case "0": case "Male": case "male": Core.Player.Gender = "Male"; break;
                        case "1": case "Female": case "female": Core.Player.Gender = "Female"; break;
                        default: Core.Player.Gender = "Other"; break;
                    }
                    IsReady = true;
                    break;
                case "setopacity":
                {
                    float newOpacity = Sng(argument.Replace("~", Screen.Level.OwnPlayer!.Opacity.ToString().Replace(".", GameController.DecSeparator)));
                    Screen.Level.OwnPlayer!.Opacity = newOpacity;
                    IsReady = true;
                    break;
                }
                case "dowalkanimation":
                    Core.Player.DoWalkAnimation = ScriptConversion.ToBoolean(argument);
                    IsReady = true;
                    break;
                case "removeitemdata":
                {
                    String[] ids = Core.Player.ItemData.Split(',');
                    String levelPath = argument.GetSplit(0, ",");
                    String levelItemIndex = argument.GetSplit(1, ",");
                    String key = (levelPath.ToLower() + "|" + levelItemIndex).ToLower();
                    if (ids.Contains(key) == true)
                    {
                        if (Core.Player.ItemData.Split(',').Length == 1) { Core.Player.ItemData = String.Empty; }
                        else { Core.Player.ItemData = Core.Player.ItemData.Replace("," + key, ""); }
                    }
                    IsReady = true;
                    break;
                }
                case "quitgame":
                    VoltorbFlip.VoltorbFlipScreen.CurrentLevel = 1;
                    VoltorbFlip.VoltorbFlipScreen.PreviousLevel = 1;
                    VoltorbFlip.VoltorbFlipScreen.ConsecutiveWins = 0;
                    VoltorbFlip.VoltorbFlipScreen.TotalFlips = 0;
                    VoltorbFlip.VoltorbFlipScreen.CurrentCoins = 0;
                    VoltorbFlip.VoltorbFlipScreen.TotalCoins = -1;
                    if (JoinServerScreen.Online == true) { Core.ServersManager.ServerConnection.Disconnect(); }
                    World.setDaytime = -1;
                    World.setSeason = -1;
                    Chat.ClearChat();
                    ScriptStorage.Clear();
                    GameModeManager.SetGameModePointer("Kolben");
                    Localization.LocalizationTokens.Clear();
                    Localization.LoadTokenFile(GameMode.DefaultLocalizationsPath, false);
                    Core.OffsetMaps.Clear();
                    TextureManager.TextureList.Clear();
                    TextureManager.TextureRectList.Clear();
                    Whirlpool.LoadedWaterTemp = false;
                    Core.Player.RunToggled = false;
                    if (argument.Equals("") == false)
                    {
                        if (ScriptConversion.ToBoolean(argument) == true)
                            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new PressStartScreen(), Color.Black, false, 15));
                        else
                            Core.SetScreen(new PressStartScreen());
                    }
                    else { Core.SetScreen(new PressStartScreen()); }
                    Core.Player.loadedSave = false;
                    break;
                default:
                    IsReady = true;
                    break;
            }
        }

        private static String GetWarpSound(int soundData)
        {
            switch (soundData)
            {
                case 0: return "Warp_Exit";
                case 1: return "Warp_Door";
                case 2: return "Warp_Ladder";
                case 3: return "";
                default: return "Warp_Exit";
            }
        }

        private static void DoBattle(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "starttrainer":
                {
                    if (Core.Player.Pokemons.Count > 0)
                    {
                        BattleSystem.Trainer t = new BattleSystem.Trainer(argument);
                        if (t.IsBeaten() == false)
                        {
                            if (Started == false)
                            {
                                ((OverworldScreen)Core.CurrentScreen).TrainerEncountered = true;
                                if (t.GetInSightMusic().Equals("") == false && t.GetInSightMusic().Equals("nomusic") == false)
                                {
                                    MusicManager.Play(t.GetInSightMusic(), true, 0.0f);
                                }
                                if (t.IntroMessage.Equals("") == false)
                                {
                                    Screen.TextBox.reDelay = 0.0f;
                                    Screen.TextBox.Show(t.IntroMessage, []);
                                }
                                Started = true;
                            }
                            if (Screen.TextBox.Showing == false)
                            {
                                ((OverworldScreen)Core.CurrentScreen).TrainerEncountered = false;
                                int method = Screen.Level.Surfing == true ? 2 : 0;
                                BattleSystem.BattleScreen b = new BattleSystem.BattleScreen(new BattleSystem.Trainer(argument), Core.CurrentScreen, method);
                                Core.SetScreen(new BattleIntroScreen(Core.CurrentScreen, b, t, t.GetIniMusicName(), t.IntroType));
                            }
                        }
                        else
                        {
                            Screen.TextBox.reDelay = 0.0f;
                            Screen.TextBox.Show(t.DefeatMessage, []);
                            IsReady = true;
                        }
                        if (Screen.TextBox.Showing == false) { IsReady = true; }
                        CanContinue = false;
                    }
                    else
                    {
                        StartBlackout();
                    }
                    break;
                }
                case "trainer":
                {
                    if (Core.Player.Pokemons.Count > 0)
                    {
                        String id = argument;
                        if (argument.CountSeperators(",") > 0) { id = argument.GetSplit(0); }
                        int method = Screen.Level.Surfing == true ? 2 : 0;
                        BattleSystem.BattleScreen b = new BattleSystem.BattleScreen(new BattleSystem.Trainer(id), Core.CurrentScreen, method);
                        Core.SetScreen(new BattleIntroScreen(Core.CurrentScreen, b, new BattleSystem.Trainer(id), new BattleSystem.Trainer(id).GetIniMusicName(), new BattleSystem.Trainer(id).IntroType));
                        IsReady = true;
                        CanContinue = false;
                    }
                    else { StartBlackout(); }
                    break;
                }
                case "wild":
                {
                    if (Core.Player.Pokemons.Count > 0)
                    {
                        Pokemon? p = null;
                        String musicLoop = String.Empty;
                        int introType = Core.Random.Next(0, 10);
                        int method = Screen.Level.Surfing == true ? 2 : 0;

                        if (argument.StartsWith("{") == true && argument.Contains("}") == true)
                        {
                            if (argument.EndsWith("}") == true)
                            {
                                p = Pokemon.GetPokemonByData(argument);
                            }
                            else
                            {
                                String pokemonData = argument.Remove(argument.LastIndexOf("}") + 1);
                                p = Pokemon.GetPokemonByData(pokemonData);
                                argument = argument.Remove(0, argument.LastIndexOf("}") + 1);
                                if (argument.Length > 1 && argument.StartsWith(",") == true)
                                {
                                    argument = argument.Remove(0, 1);
                                    String[] args = argument.Split(',');
                                    for (int i = 0; i < args.Length; i++)
                                    {
                                        if (i == 0) { musicLoop = args[i]; }
                                        else if (i == 1 && args[i].Equals("") == false) { introType = Int(args[i]); }
                                    }
                                }
                            }
                        }
                        else if (argument.Length > 0)
                        {
                            String pokID = argument.GetSplit(0);
                            String pokAD = "xXx";
                            if (pokID.Contains("_") == true) { pokAD = PokemonForms.GetAdditionalValueFromDataFile(pokID); pokID = pokID.GetSplit(0, "_"); }
                            else if (pokID.Contains(";") == true) { pokAD = pokID.GetSplit(1, ";"); pokID = pokID.GetSplit(0, ";"); }
                            int level = Int(argument.GetSplit(1));
                            if (pokAD.Equals("xXx") == false) { p = Pokemon.GetPokemonByID(Int(pokID), pokAD); p!.Generate(level, true, pokAD); }
                            else { p = Pokemon.GetPokemonByID(Int(pokID)); p!.Generate(level, true); }
                            String[] args = argument.Split(',');
                            for (int i = 0; i < args.Length; i++)
                            {
                                switch (i)
                                {
                                    case 2: if (args[i].Equals("-1") == false) { p.IsShiny = ScriptConversion.ToBoolean(args[i]); } break;
                                    case 3: musicLoop = args[i]; break;
                                    case 4: if (args[i].Equals("") == false) { introType = Int(args[i]); } break;
                                    case 5:
                                        if (args[i].Equals("") == false)
                                        {
                                            switch (Int(args[i])) { case 0: p.Gender = Pokemon.Genders.Male; break; case 1: p.Gender = Pokemon.Genders.Female; break; case 2: p.Gender = Pokemon.Genders.Genderless; break; }
                                        }
                                        break;
                                }
                            }
                        }

                        if (p != null)
                        {
                            String dexID = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);
                            Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 1);
                            BattleSystem.BattleScreen b = new BattleSystem.BattleScreen(p, Core.CurrentScreen, (Spawner.EncounterMethods)method);
                            Core.SetScreen(new BattleIntroScreen(Core.CurrentScreen, b, introType, musicLoop));
                        }
                        IsReady = true;
                        CanContinue = false;
                    }
                    else { StartBlackout(); }
                    break;
                }
                case "setvar":
                {
                    String varname = argument.GetSplit(0);
                    String varvalue = argument.GetSplit(1);
                    switch (varname.ToLower())
                    {
                        case "canrun": BattleSystem.BattleScreen.CanRun = ScriptConversion.ToBoolean(varvalue); break;
                        case "canalwaysrun": BattleSystem.BattleScreen.CanAlwaysRun = ScriptConversion.ToBoolean(varvalue); break;
                        case "cancatch": BattleSystem.BattleScreen.CanCatch = ScriptConversion.ToBoolean(varvalue); break;
                        case "canblackout": BattleSystem.BattleScreen.CanBlackout = ScriptConversion.ToBoolean(varvalue); break;
                        case "canreceiveexp": BattleSystem.BattleScreen.CanReceiveEXP = ScriptConversion.ToBoolean(varvalue); break;
                        case "canuseitems": BattleSystem.BattleScreen.CanUseItems = ScriptConversion.ToBoolean(varvalue); break;
                        case "frontiertrainer": BattleSystem.Trainer.FrontierTrainer = Int(varvalue); break;
                        case "divebattle": BattleSystem.BattleScreen.DiveBattle = ScriptConversion.ToBoolean(varvalue); break;
                        case "inversebattle": BattleSystem.BattleScreen.IsInverseBattle = ScriptConversion.ToBoolean(varvalue); break;
                        case "custombattlemusic": BattleSystem.BattleScreen.CustomBattleMusic = varvalue; break;
                        case "hiddenabilitychance": Screen.Level.HiddenAbilityChance = Int(varvalue); break;
                        case "cangainlosemoney": BattleSystem.BattleScreen.CanGainLoseMoney = ScriptConversion.ToBoolean(varvalue); break;
                    }
                    IsReady = true;
                    break;
                }
                case "resetvars":
                    BattleSystem.BattleScreen.ResetVars();
                    IsReady = true;
                    break;
                default:
                    IsReady = true;
                    break;
            }
        }

        private static void StartBlackout()
        {
            if (Screen.Level.BlackOutScript.Equals("") == false)
            {
                ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(Screen.Level.BlackOutScript, 0, false);
            }
            else
            {
                Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new BlackOutScreen(Core.CurrentScreen), Color.Black, false));
            }
        }

        private static void DoEntity(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "showmessagebulb":
                {
                    if (Started == false)
                    {
                        Started = true;
                        String[] data = argument.Split('|');
                        int id = Int(data[0]);
                        Vector3 pos = new Vector3(
                            Sng(data[1].Replace(".", GameController.DecSeparator)),
                            Sng(data[2].Replace(".", GameController.DecSeparator)),
                            Sng(data[3].Replace(".", GameController.DecSeparator)));
                        MessageBulb.NotificationTypes noType = id >= 0 && id <= 15
                            ? (MessageBulb.NotificationTypes)id
                            : MessageBulb.NotificationTypes.Exclamation;
                        Screen.Level.Entities.Add(new MessageBulb(pos, noType));
                    }
                    bool contains = false;
                    Screen.Level.Entities = Screen.Level.Entities.OrderByDescending(e => e.CameraDistance).ToList();
                    foreach (Entity e in Screen.Level.Entities)
                    {
                        if (e.EntityID.Equals("MessageBulb") == true) { e.Update(); contains = true; }
                    }
                    if (contains == false) { IsReady = true; }
                    else
                    {
                        for (int i = 0; i < Screen.Level.Entities.Count; i++)
                        {
                            if (i <= Screen.Level.Entities.Count - 1 && Screen.Level.Entities[i].CanBeRemoved == true)
                            {
                                Screen.Level.Entities.RemoveAt(i);
                                i -= 1;
                            }
                        }
                    }
                    break;
                }
                default:
                {
                    int entID = Int(argument.GetSplit(0));
                    IEnumerable<Entity> ents = Screen.Level.Entities.Where(e => e.ID == entID);
                    foreach (Entity ent in ents)
                    {
                        switch (command.ToLower())
                        {
                            case "warp":
                            {
                                List<String> pl = argument.Split(',').ToList();
                                ent.Position = new Vector3(
                                    Sng(pl[1].Replace("~", ent.Position.X.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(pl[2].Replace("~", ent.Position.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(pl[3].Replace("~", ent.Position.Z.ToString()).Replace(".", GameController.DecSeparator)));
                                ent.CreatedWorld = false;
                                break;
                            }
                            case "setscale":
                            {
                                List<String> sl = argument.Split(',').ToList();
                                ent.Scale = new Vector3(
                                    Sng(sl[1].Replace("~", ent.Position.X.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(sl[2].Replace("~", ent.Position.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(sl[3].Replace("~", ent.Position.Z.ToString()).Replace(".", GameController.DecSeparator)));
                                if (ent.ModelPath.Equals("") == false) { ent.Scale *= ModelManager.MODELSCALE; }
                                ent.CreatedWorld = false;
                                break;
                            }
                            case "remove": ent.CanBeRemoved = true; break;
                            case "setid": ent.ID = Int(argument.GetSplit(1)); break;
                            case "setopacity": ent.NormalOpacity = Sng(Int(argument.GetSplit(1)) / 100); break;
                            case "setvisible": ent.Visible = ScriptConversion.ToBoolean(argument.GetSplit(1)); break;
                            case "setadditionalvalue": ent.AdditionalValue = argument.GetSplit(1); break;
                            case "setaction": ent.ActionValue = Int(argument.GetSplit(1)); break;
                            case "setcollision": ent.Collision = ScriptConversion.ToBoolean(argument.GetSplit(1)); break;
                            case "settexture":
                            {
                                int textureID = Int(argument.GetSplit(1));
                                String textureData = argument.Remove(0, argument.IndexOf("[") + 1);
                                textureData = textureData.Remove(textureData.Length - 1, 1);
                                String[] ti = textureData.Split(',');
                                ent.Textures[textureID] = TextureManager.GetTexture(ti[0], new Rectangle(Int(ti[1]), Int(ti[2]), Int(ti[3]), Int(ti[4])), "");
                                break;
                            }
                            case "setmodelpath":
                                ent.ModelPath = argument.GetSplit(1);
                                ent.Model = ModelManager.GetModel(ent.ModelPath);
                                break;
                            case "addtoposition":
                            {
                                List<String> pl = argument.Split(',').ToList();
                                ent.Position += new Vector3(
                                    Sng(pl[1].Replace("~", ent.Position.X.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(pl[2].Replace("~", ent.Position.Y.ToString()).Replace(".", GameController.DecSeparator)),
                                    Sng(pl[3].Replace("~", ent.Position.Z.ToString()).Replace(".", GameController.DecSeparator)));
                                ent.CreatedWorld = false;
                                CanContinue = false;
                                break;
                            }
                        }
                    }
                    IsReady = true;
                    break;
                }
            }
        }

        private static void DoCamera(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            OverworldCamera c = (OverworldCamera)Screen.Camera;
            bool doCameraUpdate = true;
            Vector3 position = Core.CurrentScreen.Identification.Equals(Screen.Identifications.NewGameScreen) == true
                ? c.Position : c.ThirdPersonOffset;

            switch (command.ToLower())
            {
                case "set":
                    position = new Vector3(
                        Sng(argument.GetSplit(0).Replace("~", position.X.ToString()).Replace(".", GameController.DecSeparator)),
                        Sng(argument.GetSplit(1).Replace("~", position.Y.ToString()).Replace(".", GameController.DecSeparator)),
                        Sng(argument.GetSplit(2).Replace("~", position.Z.ToString()).Replace(".", GameController.DecSeparator)));
                    c.Yaw = Sng(argument.GetSplit(3).Replace(".", GameController.DecSeparator));
                    c.Pitch = Sng(argument.GetSplit(4).Replace(".", GameController.DecSeparator));
                    break;
                case "reset":
                    position = new Vector3(0.0f, 0.3f, 1.5f);
                    if (argument.Equals("") == false) { doCameraUpdate = ScriptConversion.ToBoolean(argument); }
                    break;
                case "setyaw":
                    c.Yaw = Sng(argument.Replace(",", ".").Replace(".", GameController.DecSeparator));
                    break;
                case "setpitch":
                    c.Pitch = Sng(argument.Replace("~", c.Pitch.ToString()).Replace(",", ".").Replace(".", GameController.DecSeparator));
                    break;
                case "setposition":
                    position = new Vector3(
                        Sng(argument.GetSplit(0).Replace("~", position.X.ToString()).Replace(".", GameController.DecSeparator)),
                        Sng(argument.GetSplit(1).Replace("~", position.Y.ToString()).Replace(".", GameController.DecSeparator)),
                        Sng(argument.GetSplit(2).Replace("~", position.Z.ToString()).Replace(".", GameController.DecSeparator)));
                    break;
                case "setx": position.X = Sng(argument.Replace("~", position.X.ToString()).Replace(".", GameController.DecSeparator)); break;
                case "sety": position.Y = Sng(argument.Replace("~", position.Y.ToString()).Replace(".", GameController.DecSeparator)); break;
                case "setz": position.Z = Sng(argument.Replace("~", position.Z.ToString()).Replace(".", GameController.DecSeparator)); break;
                case "togglethirdperson":
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true) { c.SetThirdPerson(!c.ThirdPerson, false); }
                    if (argument.Equals("") == false) { doCameraUpdate = ScriptConversion.ToBoolean(argument); }
                    break;
                case "activatethirdperson":
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true) { c.SetThirdPerson(true, false); }
                    if (argument.Equals("") == false) { doCameraUpdate = ScriptConversion.ToBoolean(argument); }
                    break;
                case "deactivethirdperson": case "deactivatethirdperson":
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true) { c.SetThirdPerson(false, false); }
                    if (argument.Equals("") == false) { doCameraUpdate = ScriptConversion.ToBoolean(argument); }
                    break;
                case "setthirdperson":
                {
                    bool tp = ScriptConversion.ToBoolean(argument.GetSplit(0));
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true) { c.SetThirdPerson(tp, false); }
                    if (argument.CountSplits() > 1) { doCameraUpdate = ScriptConversion.ToBoolean(argument.GetSplit(1)); }
                    break;
                }
                case "fix":
                    c.Fixed = true;
                    if (argument.Equals("") == false) { doCameraUpdate = ScriptConversion.ToBoolean(argument); }
                    break;
                case "defix":
                    c.Fixed = false;
                    if (argument.Equals("") == false) { doCameraUpdate = ScriptConversion.ToBoolean(argument); }
                    break;
                case "togglefix":
                    c.Fixed = !c.Fixed;
                    if (argument.Equals("") == false) { doCameraUpdate = ScriptConversion.ToBoolean(argument); }
                    break;
                case "update":
                    doCameraUpdate = true;
                    CanContinue = false;
                    break;
                case "setfocus":
                {
                    OverworldCamera.CameraFocusTypes focusType = OverworldCamera.CameraFocusTypes.Player;
                    switch (argument.GetSplit(0).ToLower())
                    {
                        case "player": focusType = OverworldCamera.CameraFocusTypes.Player; break;
                        case "npc": focusType = OverworldCamera.CameraFocusTypes.NPC; break;
                        case "entity": focusType = OverworldCamera.CameraFocusTypes.Entity; break;
                    }
                    c.SetupFocus(focusType, Int(argument.GetSplit(1)));
                    break;
                }
                case "setfocustype":
                    switch (argument.ToLower())
                    {
                        case "player": c.CameraFocusType = OverworldCamera.CameraFocusTypes.Player; break;
                        case "npc": c.CameraFocusType = OverworldCamera.CameraFocusTypes.NPC; break;
                        case "entity": c.CameraFocusType = OverworldCamera.CameraFocusTypes.Entity; break;
                    }
                    break;
                case "setfocusid": c.CameraFocusID = Int(argument); break;
                case "resetfocus": c.CameraFocusType = OverworldCamera.CameraFocusTypes.Player; c.CameraFocusID = -1; break;
                case "settoplayerfacing":
                    c.Yaw = Screen.Camera.GetPlayerFacingDirection() * Microsoft.Xna.Framework.MathHelper.PiOver2;
                    break;
            }

            if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.NewGameScreen) == true)
                c.Position = position;
            else
                c.ThirdPersonOffset = position;

            c.UpdateThirdPersonCamera();
            if (doCameraUpdate == true)
            {
                c.UpdateFrustum();
                c.UpdateViewMatrix();
                Screen.Level.Entities = Screen.Level.Entities.OrderByDescending(e => e.CameraDistance).ToList();
                Screen.Level.UpdateEntities();
            }
            IsReady = true;
        }

        private static void DoItem(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "give":
                {
                    int amount = 1;
                    String itemID = argument;
                    if (argument.Contains(",") == true) { amount = Int(argument.GetSplit(1)); itemID = argument.GetSplit(0); }
                    Items.Item? item = (ScriptConversion.IsArithmeticExpression(itemID) == false && itemID.StartsWith("gm") == false)
                        ? Items.Item.GetItemByName(itemID) : Items.Item.GetItemByID(itemID);
                    if (item != null)
                    {
                        String pid = item.IsGameModeItem == true ? item.gmID : item.ID.ToString();
                        int cap = (int)item.MaxStack - Core.Player.Inventory.GetItemAmount(pid);
                        if (item.MaxStack < Core.Player.Inventory.GetItemAmount(pid) + amount) { amount = cap.Clamp(0, 999); }
                        Core.Player.Inventory.AddItem(pid, amount);
                        Core.Player.CheckItemCountScriptDelay(pid);
                    }
                    break;
                }
                case "remove":
                {
                    int amount = 1;
                    String itemID = argument;
                    bool showMessage = true;
                    if (argument.Contains(",") == true)
                    {
                        amount = Int(argument.GetSplit(1));
                        itemID = argument.GetSplit(0);
                        if (argument.CountSeperators(",") >= 2) { showMessage = ScriptConversion.ToBoolean(argument.GetSplit(2)); }
                    }
                    Items.Item item = Items.Item.GetItemByID(itemID)!;
                    String pid = item.IsGameModeItem == true ? item.gmID : item.ID.ToString();
                    Core.Player.Inventory.RemoveItem(pid, amount);
                    if (showMessage == true)
                    {
                        String msg;
                        String space = String.Empty;
                        if (amount == 1)
                        {
                            String t = Localization.GetString("item_handed_over_single", "<Player.Name> handed over the~").Replace("<player.name>", Core.Player.Name);
                            if (t.EndsWith("~") == false) { space = " "; }
                            msg = t + space + item.OneLineName() + "!";
                        }
                        else
                        {
                            String t = Localization.GetString("item_handed_over_multiple", "<Player.Name> handed over the~").Replace("<player.name>", Core.Player.Name);
                            if (t.EndsWith("~") == false) { space = " "; }
                            msg = t + space + item.OneLinePluralName() + "!";
                        }
                        Screen.TextBox.reDelay = 0.0f;
                        Screen.TextBox.TextColor = TextBox.PlayerColor;
                        Screen.TextBox.Show(msg, []);
                        CanContinue = false;
                    }
                    Core.Player.CheckItemCountScriptDelay(pid);
                    break;
                }
                case "clearitem":
                    if (argument.Equals("") == false)
                    {
                        int amt = Core.Player.Inventory.GetItemAmount(argument);
                        if (amt > 0) { Core.Player.Inventory.RemoveItem(argument, amt); }
                        Core.Player.CheckItemCountScriptDelay(argument);
                    }
                    else { Core.Player.Inventory.Clear(); }
                    break;
                case "messagegive":
                {
                    String itemID = argument.GetSplit(0);
                    Items.Item? item = (ScriptConversion.IsArithmeticExpression(itemID) == false && itemID.StartsWith("gm") == false)
                        ? Items.Item.GetItemByName(itemID) : Items.Item.GetItemByID(itemID);
                    int amount = Int(argument.GetSplit(1));
                    if (item != null)
                    {
                        String receiveStr = Localization.GetString("item_received_single", "Received the~") + item.OneLineName();
                        if (item.ItemType == Items.ItemTypes.Machines)
                        {
                            if (amount > 1)
                            {
                                String moveName = item.IsGameModeItem == true ? ((Items.GameModeItem)item).gmTeachMove.Name : ((Items.TechMachine)item).Attack.Name;
                                receiveStr = Localization.GetString("item_received_multiple", "Received") + " " + amount + "~" + item.OneLinePluralName() + " " + moveName + ".*";
                            }
                            else
                            {
                                String moveName = item.IsGameModeItem == true ? ((Items.GameModeItem)item).gmTeachMove.Name : ((Items.TechMachine)item).Attack.Name;
                                receiveStr += " " + moveName + ".*";
                            }
                        }
                        else
                        {
                            if (amount > 1) { receiveStr = Localization.GetString("item_received_multiple", "Received") + " " + amount + "~" + item.OneLinePluralName() + ".*"; }
                            else { receiveStr += ".*"; }
                        }
                        SoundManager.PlaySound(item.OriginalName.Contains("HM") == true ? "Receive_HM" : "Receive_Item", true);
                        Screen.TextBox.reDelay = 0.0f;
                        Screen.TextBox.TextColor = TextBox.PlayerColor;
                        Screen.TextBox.Show(receiveStr + Core.Player.Inventory.GetMessageReceive(item, amount), []);
                        CanContinue = false;
                    }
                    break;
                }
                case "repel":
                {
                    int itemID = Int(argument);
                    int steps = 0;
                    switch (itemID) { case 20: steps = 100; break; case 42: steps = 200; break; case 43: steps = 250; break; }
                    Core.Player.RepelSteps += steps;
                    break;
                }
                case "use":
                    if (Core.Player.Inventory.GetItemAmount(argument) > 0)
                    {
                        Items.Item.GetItemByID(argument)!.Use();
                        Core.Player.CheckItemCountScriptDelay(argument);
                    }
                    break;
                case "select":
                {
                    NewInventoryScreen inventory;
                    if (argument.Equals("") == false)
                    {
                        String[] data = argument.Split(',');
                        List<int> pageNumbers = [];
                        if (data.Length > 0)
                        {
                            String[] typeData = data[0].Split(';');
                            if (typeData.Length == 0 || data[0].Equals("") == true) { pageNumbers = [0, 1, 2, 3, 4, 5, 6, 7]; }
                            else
                            {
                                bool allPages = false;
                                foreach (String td in typeData)
                                {
                                    if (td.Contains("-1") == true) { pageNumbers = [0, 1, 2, 3, 4, 5, 6, 7]; allPages = true; break; }
                                    switch (td.ToLower())
                                    {
                                        case "standard": case "0": pageNumbers.Add(0); break;
                                        case "medicine": case "1": pageNumbers.Add(1); break;
                                        case "plants": case "2": pageNumbers.Add(2); break;
                                        case "balls": case "pokeballs": case "3": pageNumbers.Add(3); break;
                                        case "machines": case "4": pageNumbers.Add(4); break;
                                        case "keyitems": case "5": pageNumbers.Add(5); break;
                                        case "mail": case "6": pageNumbers.Add(6); break;
                                        case "battleitems": case "7": pageNumbers.Add(7); break;
                                    }
                                }
                                _ = allPages;
                            }
                        }
                        List<String> allowedItems = [];
                        if (data.Length > 1)
                        {
                            String[] itemData = data[1].Split(';');
                            if (itemData.Length > 0 && data[1].Equals("") == false)
                            {
                                bool allItems = false;
                                foreach (String id in itemData)
                                {
                                    if (id.Contains("-1") == true) { allowedItems = ["-1"]; allItems = true; break; }
                                    if (id.Contains("-") == true)
                                    {
                                        String[] mm = id.Split('-');
                                        if (mm[0].StartsWith("gm") == true && mm[1].StartsWith("gm") == true)
                                        {
                                            for (int n = Int(mm[0].Remove(0, 2)); n <= Int(mm[1].Remove(0, 2)); n++) { allowedItems.Add("gm" + n); }
                                        }
                                        else { for (int n = Int(mm[0]); n <= Int(mm[1]); n++) { allowedItems.Add(n.ToString()); } }
                                    }
                                    else { allowedItems.Add(id); }
                                }
                                _ = allItems;
                            }
                            else { allowedItems = ["-1"]; }
                        }
                        else { allowedItems = ["-1"]; }
                        inventory = new NewInventoryScreen(Core.CurrentScreen, pageNumbers.ToArray(), null, allowedItems, true);
                    }
                    else { inventory = new NewInventoryScreen(Core.CurrentScreen, new List<String>(), true); }
                    if (inventory != null)
                    {
                        Core.SetScreen(inventory);
                        if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.InventoryScreen) == false) { IsReady = true; }
                    }
                    CanContinue = false;
                    break;
                }
            }
            IsReady = true;
        }

        private static void DoNPC(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "remove":
                {
                    Entity? target = Screen.Level.GetNPC(Int(argument));
                    if (target != null) { Screen.Level.Entities.Remove(target); }
                    IsReady = true;
                    break;
                }
                case "position": case "warp":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null)
                    {
                        String[] pd = argument.Split(',');
                        targetNPC.Position = new Vector3(
                            Sng(pd[1].Replace("~", targetNPC.Position.X.ToString()).Replace(".", GameController.DecSeparator)),
                            Sng(pd[2].Replace("~", targetNPC.Position.Y.ToString()).Replace(".", GameController.DecSeparator)),
                            Sng(pd[3].Replace("~", targetNPC.Position.Z.ToString()).Replace(".", GameController.DecSeparator)));
                        targetNPC.CreatedWorld = false;
                        if (targetNPC.InCameraFocus() == true) { Screen.Camera.Update(); }
                    }
                    IsReady = true;
                    break;
                }
                case "addtoposition":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null)
                    {
                        String[] pd = argument.Split(',');
                        targetNPC.Position += new Vector3(
                            Sng(pd[1].Replace("~", targetNPC.Position.X.ToString()).Replace(".", GameController.DecSeparator)),
                            Sng(pd[2].Replace("~", targetNPC.Position.Y.ToString()).Replace(".", GameController.DecSeparator)),
                            Sng(pd[3].Replace("~", targetNPC.Position.Z.ToString()).Replace(".", GameController.DecSeparator)));
                        targetNPC.CreatedWorld = false;
                        if (targetNPC.InCameraFocus() == true) { Screen.Camera.Update(); }
                    }
                    IsReady = true;
                    break;
                }
                case "register": NPC.AddNPCData(argument); IsReady = true; break;
                case "unregister": NPC.RemoveNPCData(argument); IsReady = true; break;
                case "wearskin":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null) { targetNPC.SetupSprite(argument.GetSplit(1), "", false); }
                    IsReady = true;
                    break;
                }
                case "setonlineskin":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    String gjID = argument.GetSplit(1);
                    if (targetNPC != null && gjID.Equals("") == false) { targetNPC.SetupSprite(targetNPC.TextureID, gjID, true); }
                    IsReady = true;
                    break;
                }
                case "move":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC == null) { IsReady = true; break; }
                    int steps = Int(argument.GetSplit(1));
                    Screen.Level.UpdateEntities();
                    if (Started == false)
                    {
                        if (steps < 0) { if (targetNPC.Speed > 0) { targetNPC.Speed *= -1; } }
                        else { if (targetNPC.Speed < 0) { targetNPC.Speed *= -1; } }
                        targetNPC.CanMove = true;
                        targetNPC.Moved += steps.ToPositive();
                        Started = true;
                    }
                    else
                    {
                        if (targetNPC.Moved <= 0.0f) { if (targetNPC.Speed < 0) { targetNPC.Speed *= -1; } IsReady = true; }
                        else { if (targetNPC.InCameraFocus() == true) { Screen.Camera.Update(); } }
                    }
                    break;
                }
                case "setmovey":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null) { targetNPC.MoveY = Sng(argument.GetSplit(1)); }
                    IsReady = true;
                    break;
                }
                case "setscale":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null)
                    {
                        String[] sd = argument.Split(',');
                        targetNPC.Scale = new Vector3(
                            Sng(sd[1].Replace("~", targetNPC.Scale.X.ToString()).Replace(".", GameController.DecSeparator)),
                            Sng(sd[2].Replace("~", targetNPC.Scale.Y.ToString()).Replace(".", GameController.DecSeparator)),
                            Sng(sd[3].Replace("~", targetNPC.Scale.Z.ToString()).Replace(".", GameController.DecSeparator)));
                        if (targetNPC.ModelPath.Equals("") == false) { targetNPC.Scale *= ModelManager.MODELSCALE; }
                    }
                    IsReady = true;
                    break;
                }
                case "setanimateidle":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null) { targetNPC.AnimateIdle = ScriptConversion.ToBoolean(argument.GetSplit(1)); }
                    IsReady = true;
                    break;
                }
                case "setmovement":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null)
                    {
                        switch (argument.GetSplit(1).ToLower())
                        {
                            case "still": targetNPC.Movement = NPC.Movements.Still; break;
                            case "looking": targetNPC.Movement = NPC.Movements.Looking; break;
                            case "faceplayer": targetNPC.Movement = NPC.Movements.FacePlayer; break;
                            case "walk": targetNPC.Movement = NPC.Movements.Walk; break;
                            case "straight": targetNPC.Movement = NPC.Movements.Straight; break;
                            case "turning": targetNPC.Movement = NPC.Movements.Turning; break;
                            case "pokeball": targetNPC.Movement = NPC.Movements.Pokeball; break;
                        }
                    }
                    IsReady = true;
                    break;
                }
                case "setadditionalvalue":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null) { targetNPC.AdditionalValue = argument.Remove(0, argument.IndexOf(",") + 1); }
                    IsReady = true;
                    break;
                }
                case "setaction":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null) { targetNPC.ActionValue = Int(argument.GetSplit(1)); }
                    IsReady = true;
                    break;
                }
                case "moveasync":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC == null) { IsReady = true; break; }
                    int steps = Int(argument.GetSplit(1));
                    Screen.Level.UpdateEntities();
                    if (steps < 0) { if (targetNPC.Speed > 0) { targetNPC.Speed *= -1; } }
                    else { if (targetNPC.Speed < 0) { targetNPC.Speed *= -1; } }
                    targetNPC.CanMove = true;
                    targetNPC.Moved += steps.ToPositive();
                    targetNPC.MoveAsync = true;
                    IsReady = true;
                    break;
                }
                case "dance":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC == null) { IsReady = true; break; }
                    int steps = Int(argument.GetSplit(1));
                    Screen.Level.UpdateEntities();
                    targetNPC.isDancing = true;
                    if (Started == false)
                    {
                        if (steps < 0) { if (targetNPC.Speed > 0) { targetNPC.Speed *= -1; } }
                        else { if (targetNPC.Speed < 0) { targetNPC.Speed *= -1; } }
                        targetNPC.CanMove = true;
                        targetNPC.Moved += steps.ToPositive();
                        Started = true;
                    }
                    else
                    {
                        if (targetNPC.Moved <= 0.0f) { if (targetNPC.Speed < 0) { targetNPC.Speed *= -1; } IsReady = true; }
                        else { if (targetNPC.InCameraFocus() == true) { Screen.Camera.Update(); } }
                    }
                    break;
                }
                case "danceasync":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC == null) { IsReady = true; break; }
                    int steps = Int(argument.GetSplit(1));
                    Screen.Level.UpdateEntities();
                    targetNPC.isDancing = true;
                    if (steps < 0) { if (targetNPC.Speed > 0) { targetNPC.Speed *= -1; } }
                    else { if (targetNPC.Speed < 0) { targetNPC.Speed *= -1; } }
                    targetNPC.CanMove = true;
                    targetNPC.Moved += steps.ToPositive();
                    targetNPC.MoveAsync = true;
                    IsReady = true;
                    break;
                }
                case "turn":
                {
                    NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                    if (targetNPC != null)
                    {
                        int dir = Int(argument.GetSplit(1));
                        targetNPC.faceRotation = dir;
                        targetNPC.FaceDirection = dir;
                        targetNPC.Update();
                        targetNPC.UpdateEntity();
                    }
                    IsReady = true;
                    break;
                }
                case "spawn":
                {
                    String[] args = argument.Split(',');
                    Vector3 pos = new Vector3(Sng(args[0]), Sng(args[1]), Sng(args[2]));
                    int actionValue = args.Length >= 4 ? Int(args[3]) : 0;
                    String additionalValue = args.Length >= 5 ? args[4] : "";
                    String textureID = "0";
                    if (args.Length >= 6)
                    {
                        String tp = args[5];
                        if (tp.ToLower().Equals("<rival.skin>") == true) { tp = Core.Player.RivalSkin; }
                        if (tp.ToLower().Equals("<player.skin>") == true) { tp = Core.Player.Skin; }
                        textureID = tp;
                    }
                    bool animateIdle = args.Length >= 7 ? ScriptConversion.ToBoolean(args[6]) : false;
                    int rotation = args.Length >= 8 ? Int(args[7]) : 0;
                    String npcName = args.Length >= 9 ? args[8] : "";
                    int npcID = args.Length >= 10 ? Int(args[9]) : 0;
                    String movement = args.Length >= 11 ? args[10] : "Still";
                    List<Rectangle> moveRects = [];
                    if (args.Length >= 12)
                    {
                        String rectangles = argument.Remove(0, argument.IndexOf("[[") + 1);
                        rectangles = rectangles.Remove(rectangles.Length - 1, 1);
                        String[] values = rectangles.Split(']');
                        foreach (String v in values)
                        {
                            String vt = v.StartsWith("[") == true ? v.Remove(0, 1) : v;
                            if (vt.Length > 0)
                            {
                                String[] content = vt.Split(',');
                                moveRects.Add(new Rectangle(Int(content[0]), Int(content[1]), Int(content[2]), Int(content[3])));
                            }
                        }
                    }
                    if (Screen.Level.GetNPC(npcID) != null)
                    {
                        Logger.Log(Logger.LogTypes.Message, "ScriptCommander.cs: (@npc." + command + ") An NPC with the ID \"" + npcID + "\" already exists.");
                    }
                    NPC npc = (NPC)Entity.GetNewEntity("NPC", pos, [null!], [0, 0], true, Vector3.Zero, Vector3.One,
                        BaseModel.BillModel, actionValue, additionalValue, true, Vector3.One, -1,
                        Screen.Level.LevelFile!, "", Vector3.Zero,
                        new Object[] { textureID, rotation, npcName, npcID, animateIdle, movement, moveRects });
                    Screen.Level.Entities.Add(npc);
                    IsReady = true;
                    break;
                }
                case "setspeed":
                {
                    if (argument.CountSeperators(",") > 0)
                    {
                        NPC? targetNPC = Screen.Level.GetNPC(Int(argument.GetSplit(0)));
                        if (targetNPC != null) { targetNPC.Speed = Sng(argument.GetSplit(1)) * 0.04f; }
                        else { Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@npc." + command + ") The targeted NPC with ID \"" + Int(argument.GetSplit(0)) + "\" doesn't exist."); }
                    }
                    else { Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@npc." + command + ") Invalid argument passed."); }
                    IsReady = true;
                    break;
                }
                default:
                    Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@npc." + command + ") Command not found.");
                    IsReady = true;
                    break;
            }
        }

        private static void DoDayCare(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "takeegg":
                {
                    String newData = String.Empty;
                    int dayCareID = Int(argument);
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dayCareID.ToString() + "|Egg|") == false)
                        {
                            if (newData.Equals("") == false) { newData += Environment.NewLine; }
                            newData += line;
                        }
                        else
                        {
                            Pokemon? p = Daycare.ProduceEgg(dayCareID);
                            if (p != null) { Core.Player.Pokemons.Add(p); }
                        }
                    }
                    Core.Player.DaycareData = newData;
                    break;
                }
                case "takepokemon":
                {
                    String newData = String.Empty;
                    int dayCareID = Int(argument.GetSplit(0));
                    int pokemonIndex = Int(argument.GetSplit(1));
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dayCareID.ToString() + "|" + pokemonIndex.ToString() + "|") == true)
                        {
                            String data = line.Remove(0, line.IndexOf("{"));
                            int startStep = int.Parse(line.Split('|')[2]);
                            Pokemon p = Pokemon.GetPokemonByData(data);
                            p.GetExperience(Core.Player.DaycareSteps - startStep, true);
                            Core.Player.Pokemons.Add(p);
                        }
                        else
                        {
                            if (newData.Equals("") == false) { newData += Environment.NewLine; }
                            newData += line;
                        }
                    }
                    Core.Player.DaycareData = newData;
                    break;
                }
                case "leavepokemon":
                {
                    int dayCareID = Int(argument.GetSplit(0));
                    int pokemonDaycareIndex = Int(argument.GetSplit(1));
                    int pokemonIndex = Int(argument.GetSplit(2));
                    if (Core.Player.DaycareData.Equals("") == false) { Core.Player.DaycareData += Environment.NewLine; }
                    Core.Player.DaycareData += dayCareID.ToString() + "|" + pokemonDaycareIndex.ToString() + "|" + Core.Player.DaycareSteps + "|0|" + Core.Player.Pokemons[pokemonIndex].GetSaveData();
                    Core.Player.Pokemons.RemoveAt(pokemonIndex);
                    break;
                }
                case "removeegg":
                {
                    String newData = String.Empty;
                    int dayCareID = Int(argument);
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(dayCareID.ToString() + "|Egg|") == false)
                        {
                            if (newData.Equals("") == false) { newData += Environment.NewLine; }
                            newData += line;
                        }
                    }
                    Core.Player.DaycareData = newData;
                    break;
                }
                case "clean":
                {
                    int daycareID = Int(argument);
                    String newData = String.Empty;
                    List<String> lines = [];
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(daycareID + "|") == true) { lines.Add(line); }
                        else { if (newData.Equals("") == false) { newData += Environment.NewLine; } newData += line; }
                    }
                    for (int i = 0; i < lines.Count; i++)
                    {
                        String line = lines[i];
                        String[] data = line.Split('|');
                        if (newData.Equals("") == false) { newData += Environment.NewLine; }
                        if (data[1].Equals("Egg") == true) { newData += daycareID.ToString() + "|Egg|" + data[2]; }
                        else { newData += daycareID.ToString() + "|" + i.ToString() + "|" + data[2] + "|" + data[3] + "|" + line.Remove(0, line.IndexOf("{")); }
                    }
                    Core.Player.DaycareData = newData;
                    break;
                }
                case "cleardata":
                {
                    int daycareID = Int(argument);
                    String newData = String.Empty;
                    foreach (String line in Core.Player.DaycareData.SplitAtNewline())
                    {
                        if (line.StartsWith(daycareID.ToString() + "|") == false)
                        {
                            if (newData.Equals("") == false) { newData += Environment.NewLine; }
                            newData += line;
                        }
                    }
                    Core.Player.DaycareData = newData;
                    break;
                }
                case "call":
                    Daycare.TriggerCall(Int(argument));
                    break;
                default:
                    Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@daycare." + command + ") Command not found.");
                    break;
            }
            IsReady = true;
        }
    }
}
