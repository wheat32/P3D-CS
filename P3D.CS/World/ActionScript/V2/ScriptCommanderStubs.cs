using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.ScriptVersion2
{
    public static partial class ScriptCommander
    {
        private static void DoRegister(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "register":
                    if (argument.Contains(",") == true)
                    {
                        String[] args = argument.Split(',');
                        ActionScript.RegisterID(args[0], args[1], args[2]);
                    }
                    else
                    {
                        ActionScript.RegisterID(argument);
                    }
                    break;
                case "unregister":
                    if (argument.Contains(",") == true)
                    {
                        String[] args = argument.Split(',');
                        ActionScript.UnregisterID(args[0], args[1]);
                    }
                    else
                    {
                        ActionScript.UnregisterID(argument);
                    }
                    break;
                case "change":
                {
                    String[] args = argument.Split(',');
                    ActionScript.ChangeRegister(args[0], args[1]);
                    break;
                }
                case "registertime":
                {
                    String[] args = argument.Split(',');
                    String format = "days";
                    bool isValidFormat = true;
                    switch (args[2].ToLower())
                    {
                        case "day": case "days": format = "days"; break;
                        case "hour": case "hours": format = "hours"; break;
                        case "minute": case "minutes": format = "minutes"; break;
                        case "second": case "seconds": format = "seconds"; break;
                        case "year": case "years": format = "years"; break;
                        case "week": case "weeks": format = "weeks"; break;
                        case "month": case "months": format = "months"; break;
                        case "dayofweek": format = "dayofweek"; break;
                        default: isValidFormat = false; break;
                    }
                    int value = -1;
                    bool validValue = true;
                    if (int.TryParse(args[1], out value) == false) { validValue = false; }
                    else if (value < 0) { validValue = false; }

                    if (validValue == true)
                    {
                        if (isValidFormat == true)
                        {
                            ActionScript.RegisterID("[TIME|" + ActionScript.TimeToUnix(DateTime.Now) + "|" + args[1] + "|" + format + "]" + args[0]);
                        }
                        else
                        {
                            Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@register." + command + ") Invalid date format used for time based register!");
                        }
                    }
                    else
                    {
                        Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@register." + command + ") Invalid value used for time based register!");
                    }
                    break;
                }
                default:
                    Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@register." + command + ") Command not found.");
                    break;
            }
            IsReady = true;
        }

        private static void DoScript(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
            {
                switch (command.ToLower())
                {
                    case "start":
                        ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(argument, 0, true, false, "ScriptCommand");
                        break;
                    case "text":
                        ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(argument, 1, true, false, "ScriptCommand");
                        break;
                    case "run":
                        ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(argument, 2, true, false, "ScriptCommand");
                        break;
                    case "delay":
                        if (argument.Contains(",") == true)
                        {
                            String[] args = argument.Split(',');
                            if (ActionScript.IsRegistered("SCRIPTDELAY_" + args[0].ToLower()) == true)
                            {
                                ActionScript.UnregisterID("SCRIPTDELAY_" + args[0], "str");
                                ActionScript.UnregisterID("SCRIPTDELAY_" + args[0]);
                            }
                            if (Core.Player.ScriptDelaySteps > 0 && args[2].ToLower().Equals("steps") == true)
                            {
                                String[] data = Core.Player.RegisterData.Split(',');
                                foreach (String line in data)
                                {
                                    if (line.StartsWith("[") == true && line.Contains("]") == true && line.EndsWith("]") == false)
                                    {
                                        String lineName = line.Remove(0, line.IndexOf("]") + 1);
                                        if (lineName.StartsWith("SCRIPTDELAY_") == true)
                                        {
                                            Object[] registerContent = ActionScript.GetRegisterValue(lineName);
                                            String delayType = ((String)registerContent[0]).GetSplit(0, ";");
                                            if (delayType.ToLower().Equals("steps") == true)
                                            {
                                                ActionScript.UnregisterID(lineName, "str");
                                                ActionScript.UnregisterID(lineName);
                                                Core.Player.ScriptDelaySteps = 0;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            switch (args[2].ToLower())
                            {
                                case "steps":
                                    Core.Player.ScriptDelaySteps = Int(args[3]);
                                    bool displaySteps = false;
                                    if (args.Length > 4) { displaySteps = ScriptConversion.ToBoolean(args[4]); }
                                    Core.Player.ScriptDelayDisplaySteps = displaySteps;
                                    break;
                                case "itemcount":
                                {
                                    String compareType = "";
                                    switch (args[4].ToLower())
                                    {
                                        case "equal": compareType = "equal"; break;
                                        case "below": compareType = "below"; break;
                                        case "equalorbelow": compareType = "equalorbelow"; break;
                                        case "above": compareType = "above"; break;
                                        case "equalorabove": compareType = "equalorabove"; break;
                                    }
                                    if (args[3].Equals("") == false && compareType.Equals("") == false && StringHelper.IsNumeric(args[5]) == true)
                                    {
                                        String entry = args[0] + "," + args[3] + "," + compareType + "," + Int(args[5]);
                                        if (Core.Player.ScriptDelayItems.Contains(entry) == false)
                                        {
                                            if (Core.Player.ScriptDelayItems.Equals("") == false)
                                            {
                                                Core.Player.ScriptDelayItems += ";";
                                            }
                                            Core.Player.ScriptDelayItems += entry;
                                        }
                                    }
                                    break;
                                }
                            }
                            ActionScript.RegisterID("SCRIPTDELAY_" + args[0], "str", args[2].ToLower() + ";" + args[1]);
                        }
                        IsReady = true;
                        break;
                    case "cleardelay":
                        if (argument.Equals("") == false)
                        {
                            Object[] registerContent = ActionScript.GetRegisterValue("SCRIPTDELAY_" + argument);
                            if (registerContent[0] != null)
                            {
                                String delayType = ((String)registerContent[0]).GetSplit(0, ";");
                                switch (delayType.ToLower())
                                {
                                    case "steps":
                                        Core.Player.ScriptDelaySteps = 0;
                                        Core.Player.ScriptDelayDisplaySteps = false;
                                        break;
                                    case "itemcount":
                                    {
                                        List<String> itemDelayList = Core.Player.ScriptDelayItems.Split(';').ToList();
                                        for (int i = 0; i < itemDelayList.Count; i++)
                                        {
                                            if (itemDelayList[i].GetSplit(0, ",").Equals(argument) == true)
                                            {
                                                itemDelayList.RemoveAt(i);
                                                break;
                                            }
                                        }
                                        Core.Player.ScriptDelayItems = String.Join(";", itemDelayList);
                                        break;
                                    }
                                }
                            }
                            ActionScript.UnregisterID("SCRIPTDELAY_" + argument, "str");
                            ActionScript.UnregisterID("SCRIPTDELAY_" + argument);
                        }
                        IsReady = true;
                        break;
                }
            }
            IsReady = true;
        }

        private static void DoScreen(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "showmessagebox":
                {
                    MessageBox messageBox = new MessageBox(Core.CurrentScreen);
                    String[] colorSplit = argument.Split('|');
                    switch (colorSplit.Length)
                    {
                        case 1:
                            messageBox.Show(argument.Replace("~", Environment.NewLine).Replace("*", Environment.NewLine + Environment.NewLine));
                            break;
                        case 2:
                            messageBox.Show(colorSplit[0].Replace("~", Environment.NewLine).Replace("*", Environment.NewLine + Environment.NewLine),
                                new Color(Int(colorSplit[1].GetSplit(0)), Int(colorSplit[1].GetSplit(1)), Int(colorSplit[1].GetSplit(2))));
                            break;
                        case 3:
                            messageBox.Show(colorSplit[0].Replace("~", Environment.NewLine).Replace("*", Environment.NewLine + Environment.NewLine),
                                new Color(Int(colorSplit[1].GetSplit(0)), Int(colorSplit[1].GetSplit(1)), Int(colorSplit[1].GetSplit(2))),
                                new Color(Int(colorSplit[2].GetSplit(0)), Int(colorSplit[2].GetSplit(1)), Int(colorSplit[2].GetSplit(2))));
                            break;
                        default:
                            messageBox.Show(colorSplit[0].Replace("~", Environment.NewLine).Replace("*", Environment.NewLine + Environment.NewLine),
                                new Color(Int(colorSplit[1].GetSplit(0)), Int(colorSplit[1].GetSplit(1)), Int(colorSplit[1].GetSplit(2))),
                                new Color(Int(colorSplit[2].GetSplit(0)), Int(colorSplit[2].GetSplit(1)), Int(colorSplit[2].GetSplit(2))),
                                new Color(Int(colorSplit[3].GetSplit(0)), Int(colorSplit[3].GetSplit(1)), Int(colorSplit[3].GetSplit(2))));
                            break;
                    }
                    IsReady = true;
                    CanContinue = false;
                    break;
                }
                case "storagesystem":
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new StorageSystemScreen(Core.CurrentScreen), Color.Black, false));
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "apricornkurt":
                    Core.SetScreen(new ApricornScreen(Core.CurrentScreen, "Kurt"));
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "trade":
                {
                    String storeData = argument.GetSplit(0);
                    bool canBuy = ScriptConversion.ToBoolean(argument.GetSplit(1));
                    bool canSell = ScriptConversion.ToBoolean(argument.GetSplit(2));
                    String currencyIndicator = "P";
                    if (argument.CountSplits() > 3 && argument.GetSplit(3).Equals("") == false)
                    {
                        currencyIndicator = argument.GetSplit(3);
                    }
                    String shopIdentifier = "";
                    if (argument.CountSplits() > 4) { shopIdentifier = argument.GetSplit(4); }
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new TradeScreen(Core.CurrentScreen, storeData, canBuy, canSell, currencyIndicator, shopIdentifier), Color.Black, false));
                    IsReady = true;
                    CanContinue = false;
                    break;
                }
                case "townmap":
                    if (argument.Contains(",") == true)
                    {
                        List<String> regions = argument.Split(',').ToList();
                        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, regions, 0, ["view"]), Color.Black, false));
                    }
                    else
                    {
                        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, argument, ["view"]), Color.Black, false));
                    }
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "donation":
                    Core.SetScreen(new DonationScreen(Core.CurrentScreen));
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "blackout":
                    if (Screen.Level.BlackOutScript.Equals("") == false)
                    {
                        ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(Screen.Level.BlackOutScript, 0, false);
                    }
                    else
                    {
                        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new BlackOutScreen(Core.CurrentScreen), Color.Black, false));
                    }
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "fadein":
                {
                    int fadeSpeed = 5;
                    int fadeLimit = 0;
                    if (argument.Equals("") == false)
                    {
                        if (argument.Contains(",") == true)
                        {
                            fadeSpeed = Int(argument.GetSplit(0, ","));
                            fadeLimit = Int(argument.GetSplit(1, ","));
                        }
                        else { fadeSpeed = Int(argument); }
                    }
                    if (OverworldScreen.FadeValue > fadeLimit)
                    {
                        OverworldScreen.FadeValue -= fadeSpeed;
                        if (OverworldScreen.FadeValue <= fadeLimit) { OverworldScreen.FadeValue = fadeLimit; IsReady = true; }
                    }
                    else { IsReady = true; }
                    break;
                }
                case "fadeout":
                {
                    int fadeSpeed = 5;
                    int fadeLimit = 255;
                    if (argument.Equals("") == false)
                    {
                        if (argument.Contains(",") == true)
                        {
                            fadeSpeed = Int(argument.GetSplit(0, ","));
                            fadeLimit = Int(argument.GetSplit(1, ","));
                        }
                        else { fadeSpeed = Int(argument); }
                    }
                    if (OverworldScreen.FadeValue < fadeLimit)
                    {
                        OverworldScreen.FadeValue += fadeSpeed;
                        if (OverworldScreen.FadeValue >= fadeLimit) { OverworldScreen.FadeValue = fadeLimit; IsReady = true; }
                    }
                    else { IsReady = true; }
                    break;
                }
                case "fadeoutcolor":
                    if (String.IsNullOrEmpty(argument) == false)
                    {
                        OverworldScreen.FadeColor = new Color(Int(argument.GetSplit(0)), Int(argument.GetSplit(1)), Int(argument.GetSplit(2)));
                    }
                    else { OverworldScreen.FadeColor = Color.Black; }
                    IsReady = true;
                    break;
                case "setfade":
                    OverworldScreen.FadeValue = Int(argument).Clamp(0, 255);
                    IsReady = true;
                    break;
                case "showpokemon":
                    Screen.PokemonImageView.Show(Int(argument.GetSplit(0)), ScriptConversion.ToBoolean(argument.GetSplit(1)), ScriptConversion.ToBoolean(argument.GetSplit(2)));
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "showimage":
                {
                    Texture2D texture = TextureManager.GetTexture(argument.GetSplit(0));
                    String sound = "";
                    if (argument.Split(',').Length > 1)
                    {
                        sound = argument.GetSplit(1);
                        if (argument.Split(',').Length > 2)
                        {
                            texture = TextureManager.GetTexture(argument.GetSplit(0), new Rectangle(Int(argument.GetSplit(2)), Int(argument.GetSplit(3)), Int(argument.GetSplit(4)), Int(argument.GetSplit(5))), "");
                        }
                    }
                    Screen.ImageView.Show(texture, sound);
                    IsReady = true;
                    CanContinue = false;
                    break;
                }
                case "credits":
                {
                    String ending = "Johto";
                    bool canBeSkipped = false;
                    if (argument.Equals("") == false)
                    {
                        if (argument.Split(',').Length > 1) { canBeSkipped = ScriptConversion.ToBoolean(argument.GetSplit(1)); }
                        if (argument.GetSplit(0).Equals("") == false) { ending = argument.GetSplit(0); }
                    }
                    CreditsScreen creditsScreen = new CreditsScreen(Core.CurrentScreen);
                    Core.SetScreen(creditsScreen);
                    creditsScreen.InitializeScreen(ending, canBeSkipped);
                    IsReady = true;
                    CanContinue = false;
                    break;
                }
                case "halloffame":
                    if (argument.Equals("") == false)
                    {
                        if (argument.Contains(",") == false)
                        {
                            if (argument.EndsWith(".dat") == false)
                            {
                                Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new HallOfFameScreen(Core.CurrentScreen, Int(argument)), Color.Black, false));
                            }
                            else
                            {
                                Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new HallOfFameScreen(Core.CurrentScreen, argument), Color.Black, false));
                            }
                        }
                        else
                        {
                            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new HallOfFameScreen(Core.CurrentScreen, Int(argument.GetSplit(0, ",")), argument.GetSplit(1, ",")), Color.Black, false));
                        }
                    }
                    else
                    {
                        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new HallOfFameScreen(Core.CurrentScreen), Color.Black, false));
                    }
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "teachmoves":
                {
                    String[] args = argument.Split(',');
                    if (args.Length == 1)
                    {
                        Core.SetScreen(new TeachMovesScreen(Core.CurrentScreen, Int(argument)));
                    }
                    else
                    {
                        int pokeIndex = Int(args[0]);
                        List<BattleSystem.Attack> moves = [];
                        for (int i = 1; i < args.Length; i++)
                        {
                            if (StringHelper.IsNumeric(args[i]) == true)
                            {
                                moves.Add(BattleSystem.Attack.GetAttackByID(Int(args[i])));
                            }
                        }
                        Core.SetScreen(new TeachMovesScreen(Core.CurrentScreen, pokeIndex, moves.ToArray()));
                    }
                    IsReady = true;
                    CanContinue = false;
                    break;
                }
                case "mailsystem":
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MailSystemScreen(Core.CurrentScreen), Color.Black, false));
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "pvp":
                    if (Core.ServersManager.ID == 0)
                    {
                        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new PVPLobbyScreen(Core.CurrentScreen, 1, true), Color.Black, false));
                    }
                    else
                    {
                        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new PVPLobbyScreen(Core.CurrentScreen, 0, false), Color.Black, false));
                    }
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "input":
                {
                    String[] data = argument.Split(',');
                    String defaultName = "";
                    InputScreen.InputModes inputMode = InputScreen.InputModes.Text;
                    String currentText = "";
                    int maxChars = 14;
                    List<Texture2D> textureList = [];
                    if (data.Length > 0) { defaultName = data[0]; }
                    if (data.Length > 1)
                    {
                        if (ScriptConversion.IsArithmeticExpression(data[1]) == true)
                        {
                            inputMode = (InputScreen.InputModes)Int(data[1]);
                        }
                        else
                        {
                            switch (data[1].ToLower())
                            {
                                case "text": inputMode = InputScreen.InputModes.Text; break;
                                case "name": inputMode = InputScreen.InputModes.Name; break;
                                case "numbers": inputMode = InputScreen.InputModes.Numbers; break;
                                case "pokemon": inputMode = InputScreen.InputModes.Pokemon; break;
                            }
                        }
                    }
                    if (data.Length > 2) { currentText = data[2]; }
                    if (data.Length > 3) { maxChars = Int(data[3]); }
                    if (data.Length > 4)
                    {
                        for (int i = 4; i < data.Length; i++)
                        {
                            String[] tData = data[i].Split('|');
                            if (tData.Length == 1) { textureList.Add(TextureManager.GetTexture(tData[0])); }
                            else { textureList.Add(TextureManager.GetTexture(tData[0], new Rectangle(Int(tData[1]), Int(tData[2]), Int(tData[3]), Int(tData[4])), "")); }
                        }
                    }
                    Core.SetScreen(new InputScreen(Core.CurrentScreen, defaultName, inputMode, currentText, maxChars, textureList));
                    IsReady = true;
                    CanContinue = false;
                    break;
                }
                case "mysteryevent":
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MysteryEventScreen(Core.CurrentScreen), Color.White, false));
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "secretbase":
                    Core.SetScreen(new SecretBaseScreen());
                    IsReady = true;
                    CanContinue = false;
                    break;
                case "voltorbflip":
                    if (Core.Player.Inventory.GetItemAmount("54") > 0)
                    {
                        if (VoltorbFlip.VoltorbFlipScreen.TotalCoins == -1)
                        {
                            Core.SetScreen(new VoltorbFlip.VoltorbFlipScreen(Core.CurrentScreen));
                        }
                        if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
                        {
                            VoltorbFlip.VoltorbFlipScreen.CurrentLevel = 1;
                            VoltorbFlip.VoltorbFlipScreen.PreviousLevel = 1;
                            if (VoltorbFlip.VoltorbFlipScreen.TotalCoins > 0)
                            {
                                Screen.TextBox.Show(Localization.GetString("VoltorbFlip_AfterGame_Won1", "You've won") + " " + VoltorbFlip.VoltorbFlipScreen.TotalCoins + " " + Localization.GetString("VoltorbFlip_AfterGame_Won2", "Coins!"));
                                Core.Player.Coins += VoltorbFlip.VoltorbFlipScreen.TotalCoins;
                                PlayerStatistics.Track("Obtained Coins", VoltorbFlip.VoltorbFlipScreen.TotalCoins);
                                VoltorbFlip.VoltorbFlipScreen.TotalCoins = -1;
                                IsReady = true;
                            }
                            else
                            {
                                Screen.TextBox.Show(Localization.GetString("VoltorbFlip_AfterGame_Lost", "Too bad, you didn't win~any Coins!*Better luck next time!"));
                                VoltorbFlip.VoltorbFlipScreen.TotalCoins = -1;
                                IsReady = true;
                            }
                        }
                    }
                    else
                    {
                        Screen.TextBox.Show(Localization.GetString("VoltorbFlip_BeforeGame_NoCoinCase", "You don't have a Coin Case!~Come back when you have one!"));
                        IsReady = true;
                    }
                    CanContinue = false;
                    break;
                case "skinselection":
                    if (global::Screens.MainMenu.NewNewGameScreen.CharacterSelectionScreen.SelectedSkin.Equals("") == false)
                    {
                        IsReady = true;
                    }
                    break;
                default:
                    IsReady = true;
                    break;
            }
        }

        private static void DoMusic(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "play":
                {
                    bool loopSong = true;
                    bool fadeIntoSong = false;
                    if (argument.Split(',').Length > 1 && argument.GetSplit(1, ",").Equals("") == false)
                    {
                        loopSong = ScriptConversion.ToBoolean(argument.GetSplit(1, ","));
                    }
                    if (argument.Split(',').Length > 2) { fadeIntoSong = ScriptConversion.ToBoolean(argument.GetSplit(2, ",")); }
                    if (fadeIntoSong == false)
                    {
                        MusicManager.Play(argument.GetSplit(0, ","), true, loopSong);
                    }
                    else
                    {
                        MusicManager.Play(argument.GetSplit(0, ","), true, 0.01f, loopSong);
                    }
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
                    {
                        Screen.Level.MusicLoop = loopSong == true ? argument.GetSplit(0, ",") : "silence";
                    }
                    break;
                }
                case "forceplay":
                {
                    bool loopSong = true;
                    bool fadeIntoSong = false;
                    if (argument.Split(',').Length > 1) { loopSong = ScriptConversion.ToBoolean(argument.GetSplit(1, ",")); }
                    if (argument.Split(',').Length > 2) { fadeIntoSong = ScriptConversion.ToBoolean(argument.GetSplit(2, ",")); }
                    MusicManager.ForceMusic = argument.GetSplit(0, ",");
                    if (fadeIntoSong == false) { MusicManager.Play(argument.GetSplit(0, ","), true, loopSong); }
                    else { MusicManager.Play(argument.GetSplit(0, ","), true, 0.01f, loopSong); }
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
                    {
                        Screen.Level.MusicLoop = loopSong == true ? argument.GetSplit(0, ",") : "silence";
                    }
                    break;
                }
                case "unforce": MusicManager.ForceMusic = ""; break;
                case "setmusicloop":
                    if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
                    {
                        Screen.Level.MusicLoop = argument;
                    }
                    break;
                case "stop": MusicManager.Stop(); break;
                case "mute": MusicManager.SetMuted(true); break;
                case "unmute": MusicManager.SetMuted(false); break;
                case "pause": MusicManager.SetPaused(true); break;
                case "resume": MusicManager.SetPaused(false); break;
            }
            IsReady = true;
        }

        private static void DoSound(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "play":
                {
                    String sound = argument;
                    bool stopMusic = false;
                    if (argument.Contains(",") == true)
                    {
                        sound = argument.GetSplit(0);
                        stopMusic = ScriptConversion.ToBoolean(argument.GetSplit(1));
                    }
                    if (sound.Equals("healing") == true) { sound = "Heal_Party"; }
                    SoundManager.PlaySound(sound, stopMusic);
                    break;
                }
                case "playadvanced":
                {
                    String[] args = argument.Split(',');
                    String sound = args[0];
                    bool stopMusic = ScriptConversion.ToBoolean(args[1]);
                    float pitch = Sng(args[2]);
                    float pan = Sng(args[3]);
                    float volume = Sng(args[4]);
                    SoundManager.PlaySound(sound, pitch, pan, volume, stopMusic);
                    break;
                }
            }
            IsReady = true;
        }

        private static void DoOverworldPokemon(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;

            switch (command.ToLower())
            {
                case "hide": Screen.Level.OverworldPokemon.Visible = false; IsReady = true; break;
                case "show": Screen.Level.OverworldPokemon.Visible = true; IsReady = true; break;
                case "toggle": Screen.Level.OverworldPokemon.Visible = !Screen.Level.OverworldPokemon.Visible; IsReady = true; break;
                default: IsReady = true; break;
            }
        }

        private static void DoEnvironment(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "setweather": Screen.Level.WeatherType = Int(argument); break;
                case "setregionweather": World.RegionWeather = (World.Weathers)Int(argument); World.RegionWeatherSet = true; break;
                case "resetregionweather": World.RegionWeather = World.GetRegionWeather(World.CurrentSeason); World.RegionWeatherSet = false; break;
                case "setseason":
                    World.setSeason = (argument.Equals("") == true || Int(argument) == -1) ? -1 : Int(argument);
                    break;
                case "setcanfly": Screen.Level.CanFly = ScriptConversion.ToBoolean(argument); break;
                case "setcandig": Screen.Level.CanDig = ScriptConversion.ToBoolean(argument); break;
                case "setcanteleport": Screen.Level.CanTeleport = ScriptConversion.ToBoolean(argument); break;
                case "setwildpokemongrass": Screen.Level.WildPokemonGrass = ScriptConversion.ToBoolean(argument); break;
                case "setwildpokemonwater": Screen.Level.WildPokemonWater = ScriptConversion.ToBoolean(argument); break;
                case "setwildpokemoneverywhere": Screen.Level.WildPokemonFloor = ScriptConversion.ToBoolean(argument); break;
                case "setisdark": Screen.Level.IsDark = ScriptConversion.ToBoolean(argument); break;
                case "setrenderdistance":
                    switch (argument.ToLower())
                    {
                        case "0": case "tiny": Core.GameOptions.RenderDistance = 0; break;
                        case "1": case "small": Core.GameOptions.RenderDistance = 1; break;
                        case "2": case "normal": Core.GameOptions.RenderDistance = 2; break;
                        case "3": case "far": Core.GameOptions.RenderDistance = 3; break;
                        case "4": case "extreme": Core.GameOptions.RenderDistance = 4; break;
                    }
                    break;
                case "toggledarkness": Screen.Level.IsDark = !Screen.Level.IsDark; break;
                case "setdaytime":
                {
                    int daytime = Int(argument);
                    if (daytime > 0 && daytime <= 4)
                    {
                        World.setDaytime = daytime - 1;
                        Screen.Level.DayTime = daytime;
                    }
                    else
                    {
                        World.setDaytime = -1;
                        Screen.Level.DayTime = (int)World.GetTime() + 1;
                    }
                    break;
                }
                case "setenvironmenttype":
                {
                    int newtype = Int(argument);
                    if (newtype >= 0 && newtype <= 5) { Screen.Level.EnvironmentType = newtype; }
                    break;
                }
            }
            Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
            IsReady = true;
        }

        private static void DoText(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "notification":
                {
                    NotificationPopup popup = new NotificationPopup();
                    String[] args = argument.Split(',');
                    switch (args.Length)
                    {
                        case 1: popup.Setup(args[0]); break;
                        case 2: popup.Setup(args[0], Int(args[1])); break;
                        case 3: popup.Setup(args[0], Int(args[1]), Int(args[2])); break;
                        case 4: popup.Setup(args[0], Int(args[1]), Int(args[2]), Int(args[3])); break;
                        case 5: popup.Setup(args[0], Int(args[1]), Int(args[2]), Int(args[3]), args[4]); break;
                        case 6: popup.Setup(args[0], Int(args[1]), Int(args[2]), Int(args[3]), args[4], args[5]); break;
                        default: popup.Setup(args[0], Int(args[1]), Int(args[2]), Int(args[3]), args[4], args[5], ScriptConversion.ToBoolean(args[6])); break;
                    }
                    OverworldScreen owScreen = (OverworldScreen)Core.CurrentScreen;
                    if (args.Length == 8 && ScriptConversion.ToBoolean(args[7]) == true && owScreen.NotificationPopupList.Count > 0)
                    {
                        owScreen.NotificationPopupList[0]._delayDate = DateTime.Now;
                        if (owScreen.NotificationPopupList.Count > 1)
                        {
                            owScreen.NotificationPopupList.RemoveRange(1, owScreen.NotificationPopupList.Count - 2);
                        }
                    }
                    if (args.Length >= 7 && ScriptConversion.ToBoolean(args[6]) == true)
                    {
                        owScreen.NotificationPopupList.Insert(0, popup);
                    }
                    else
                    {
                        owScreen.NotificationPopupList.Add(popup);
                    }
                    break;
                }
                case "show":
                    Screen.TextBox.reDelay = 0.0f;
                    Screen.TextBox.Show(argument, [], false, false);
                    CanContinue = false;
                    break;
                case "setfont":
                {
                    FontContainer? f = FontManager.GetFontContainer(argument);
                    Screen.TextBox.TextFont = f != null ? f : FontManager.GetFontContainer("textfont");
                    break;
                }
                case "debug":
                    Logger.Debug("DEBUG: " + argument);
                    break;
                case "log":
                    Logger.Log(Logger.LogTypes.Debug, argument);
                    break;
                case "color":
                {
                    String[] args = argument.Split(',');
                    if (args.Length == 1)
                    {
                        switch (args[0].ToLower())
                        {
                            case "playercolor": case "player": Screen.TextBox.TextColor = TextBox.PlayerColor; break;
                            case "defaultcolor": case "default": Screen.TextBox.TextColor = TextBox.DefaultColor; break;
                            default: Screen.TextBox.TextColor = Extensions.ColorFromName(args[0]); break;
                        }
                    }
                    else if (args.Length == 3)
                    {
                        Screen.TextBox.TextColor = new Color(Int(args[0]), Int(args[1]), Int(args[2]));
                    }
                    break;
                }
            }
            IsReady = true;
        }

        private static void DoOptions(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "show":
                {
                    if (Screen.TextBox != null && Screen.TextBox.Text != null && Screen.TextBox.Text.Length > 0)
                    {
                        Screen.TextBox.Showing = true;
                    }
                    List<String> optionList = argument.Split(',').ToList();
                    for (int i = 0; i < optionList.Count; i++)
                    {
                        if (optionList[i].Equals("[TEXT=FALSE]") == true)
                        {
                            optionList.RemoveAt(i);
                            if (Screen.TextBox != null) { Screen.TextBox.Showing = false; }
                            i -= 1;
                        }
                    }
                    ActionScript.CSL().WhenIndex += 1;
                    Screen.ChooseBox.Show(optionList.ToArray(), 0, true);
                    CanContinue = false;
                    break;
                }
                case "setcancelindex":
                    ChooseBox.CancelIndex = Int(argument);
                    break;
            }
            IsReady = true;
        }

        private static void DoLevel(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "wait":
                    if (StringHelper.IsNumeric(Value) == false) { Value = argument; }
                    if (Int(Value) > 0) { Value = (Int(Value) - 1).ToString(); }
                    else { Value = ""; IsReady = true; }
                    break;
                case "waitforsave":
                    bool doWait = false;
                    if (Core.Player.IsGameJoltSave == true)
                    {
                        if (SaveGameHelpers.GameJoltSaveDone() == false) { doWait = true; }
                        else { SaveGameHelpers.ResetSaveCounter(); }
                    }
                    if (doWait == false) { IsReady = true; }
                    break;
                case "update":
                    Screen.Level.Update();
                    Screen.Level.UpdateEntities();
                    Screen.Camera.Update();
                    IsReady = true;
                    break;
                case "waitforevents":
                    bool shouldWait = false;
                    foreach (Entity e in Screen.Level.Entities)
                    {
                        if (e.EntityID.Equals("NPC") == true)
                        {
                            NPC npc = (NPC)e;
                            if (npc.MoveAsync == true && npc.Moved != 0.0f) { shouldWait = true; break; }
                        }
                    }
                    if (shouldWait == false) { IsReady = true; }
                    break;
                case "load":
                    Screen.Level = new Level();
                    Screen.Level.Load(argument);
                    IsReady = true;
                    break;
                case "reload":
                    Screen.Level.WarpData.WarpDestination = Screen.Level.LevelFile;
                    Screen.Level.WarpData.WarpPosition = Screen.Camera.Position;
                    Screen.Level.WarpData.WarpRotations = 0;
                    Screen.Level.WarpData.DoWarpInNextTick = true;
                    Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                    Screen.Level.WarpData.WarpSound = null!;
                    IsReady = true;
                    break;
                case "setsafari":
                    Screen.Level.IsSafariZone = ScriptConversion.ToBoolean(argument);
                    IsReady = true;
                    break;
                case "setridetype":
                    Screen.Level.RideType = Int(argument).Clamp(0, 3);
                    IsReady = true;
                    break;
            }
        }

        private static void DoStorage(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "set":
                {
                    String type = argument.GetSplit(0);
                    String name = argument.GetSplit(1);
                    String val = argument.Remove(0, type.Length + name.Length + 2);
                    ScriptStorage.SetObject(type, name, val);
                    break;
                }
                case "update":
                {
                    String type = argument.GetSplit(0);
                    String name = argument.GetSplit(1);
                    String operation = argument.GetSplit(2);
                    String val = argument.Remove(0, type.Length + name.Length + operation.Length + 3);
                    String currentValue = ScriptStorage.GetObject(type, name).ToString() ?? "";
                    if (StringHelper.IsNumeric(val.Replace(".", GameController.DecSeparator)) == true &&
                        StringHelper.IsNumeric(currentValue.Replace(".", GameController.DecSeparator)) == true)
                    {
                        switch (operation.ToLower())
                        {
                            case "+": case "plus": case "add": case "addition":
                                if (ScriptConversion.IsArithmeticExpression(currentValue) == true && ScriptConversion.IsArithmeticExpression(val) == true)
                                    val = Dbl(currentValue.Replace(".", GameController.DecSeparator) + "+" + val.Replace(".", GameController.DecSeparator)).ToString();
                                else val = currentValue + val;
                                break;
                            case "-": case "minus": case "subtract": case "subtraction":
                                val = Dbl(currentValue.Replace(".", GameController.DecSeparator) + "-" + val.Replace(".", GameController.DecSeparator)).ToString();
                                break;
                            case "*": case "multiply": case "multiplication":
                                val = Dbl(currentValue.Replace(".", GameController.DecSeparator) + "*" + val.Replace(".", GameController.DecSeparator)).ToString();
                                break;
                            case "/": case ":": case "divide": case "division":
                                val = Dbl(currentValue.Replace(".", GameController.DecSeparator) + "/" + val.Replace(".", GameController.DecSeparator)).ToString();
                                break;
                        }
                        ScriptStorage.SetObject(type, name, val);
                    }
                    else
                    {
                        ScriptStorage.SetObject(type, name, currentValue + val);
                    }
                    break;
                }
                case "clear":
                    ScriptStorage.Clear();
                    break;
            }
            IsReady = true;
        }

        private static void DoChat(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;

            switch (command.ToLower())
            {
                case "clear": Chat.ClearChat(); break;
                default: Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@chat." + command + ") Command not found."); break;
            }
            IsReady = true;
        }

        private static void DoPokedex(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "setautodetect":
                    Pokedex.AutoDetect = ScriptConversion.ToBoolean(argument);
                    break;
                case "changeentry":
                {
                    bool forceChange = false;
                    if (argument.Split(',').Length > 2) { forceChange = ScriptConversion.ToBoolean(argument.GetSplit(2, ",")); }
                    Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, argument.GetSplit(0, ","), Int(argument.GetSplit(1, ",")), forceChange);
                    break;
                }
                default:
                    Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@pokedex." + command + ") Command not found.");
                    break;
            }
            IsReady = true;
        }

        private static void DoRadio(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument.Replace(".", GameController.DecSeparator);

            switch (command.ToLower())
            {
                case "allowchannel":
                    Screen.Level.AllowedRadioChannels.Add((decimal)Dbl(argument));
                    break;
                case "blockchannel":
                {
                    decimal d = (decimal)Dbl(argument);
                    if (Screen.Level.AllowedRadioChannels.Contains(d) == true)
                    {
                        Screen.Level.AllowedRadioChannels.Remove(d);
                    }
                    break;
                }
                default:
                    Logger.Log(Logger.LogTypes.Warning, "ScriptCommander.cs: (@radio." + command + ") Command not found.");
                    break;
            }
            IsReady = true;
        }

        private static void DoSystem(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            switch (command.ToLower())
            {
                case "endnewgame":
                {
                    String[] args = argument.Split(',');
                    global::Screens.MainMenu.NewNewGameScreen.EndNewGame(args[0], Sng(args[1]), Sng(args[2]), Sng(args[3]), Int(args[4]));
                    IsReady = true;
                    break;
                }
                case "replacetextures":
                {
                    String path = argument.GetSplit(0, ",");
                    ContentPackManager.Load(GameController.GamePath + GameModeManager.ActiveGameMode.ContentPath + "Data\\" + path + ".dat", true);
                    if (argument.Split(',').Length == 1 || (argument.Split(',').Length > 1 && ScriptConversion.ToBoolean(argument.GetSplit(1, ",")) == true))
                    {
                        Screen.Level.WarpData.WarpDestination = Screen.Level.LevelFile;
                        Screen.Level.WarpData.WarpPosition = Screen.Camera.Position;
                        Screen.Level.WarpData.WarpRotations = 0;
                        Screen.Level.WarpData.DoWarpInNextTick = true;
                        Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw;
                        Screen.Level.WarpData.WarpSound = null!;
                    }
                    IsReady = true;
                    break;
                }
                default:
                    IsReady = true;
                    break;
            }
        }

        private static void DoTitle(String subClass)
        {
            String command = ScriptComparer.GetSubClassArgumentPair(subClass).Command;
            String argument = ScriptComparer.GetSubClassArgumentPair(subClass).Argument;

            if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
            {
                OverworldScreen owScreen = (OverworldScreen)Core.CurrentScreen;
                switch (command.ToLower())
                {
                    case "add":
                    {
                        OverworldScreen.Title t = new OverworldScreen.Title();
                        List<String> args = Script.ParseArguments(argument);
                        for (int i = 0; i < args.Count; i++)
                        {
                            String arg = args[i];
                            switch (i)
                            {
                                case 0: t.Text = arg; break;
                                case 1: t.Delay = Sng(arg); break;
                                case 2: t.TextColor = new Color((byte)Int(arg).Clamp(0, 255), t.TextColor.G, t.TextColor.B); break;
                                case 3: t.TextColor = new Color(t.TextColor.R, (byte)Int(arg).Clamp(0, 255), t.TextColor.B); break;
                                case 4: t.TextColor = new Color(t.TextColor.R, t.TextColor.G, (byte)Int(arg).Clamp(0, 255)); break;
                                case 5: t.Scale = Sng(arg); break;
                                case 6: t.IsCentered = ScriptConversion.ToBoolean(arg); break;
                                case 7: t.Position = new Microsoft.Xna.Framework.Vector2(Sng(arg), t.Position.Y); break;
                                case 8: t.Position = new Microsoft.Xna.Framework.Vector2(t.Position.X, Sng(arg)); break;
                            }
                        }
                        owScreen.Titles.Add(t);
                        break;
                    }
                    case "clear":
                        owScreen.Titles.Clear();
                        break;
                }
            }
            IsReady = true;
        }
    }
}
