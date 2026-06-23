using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D.GameJolt;

public partial class PokegearScreen : Screen
{
    public static int TradeRequestData = -1;
    public static int BattleRequestData = -1;

    public enum MenuScreens : int
    {
        Main = 0, PSS = 1, UserView = 2, PhoneList = 3,
        Frontier = 4, MiniMap = 5, Radio = 6, TradeRequest = 7, BattleRequest = 8
    }

    public enum EntryModes : int
    {
        MainMenu = 0, DisplayUser = 1, TradeRequest = 2, BattleRequest = 3
    }

    private List<String> _functionList = [];

    private int _width = 800;
    private int _height = 460;

    private int[] _cursors;
    public MenuScreens menuIndex = MenuScreens.Main;

    public String PublicKeys = String.Empty;

    public PokegearScreen(Screen currentScreen, EntryModes entryMode, Object[] data)
    {
        Identification = Identifications.PokegearScreen;
        PreScreen = currentScreen;
        MouseVisible = true;
        _cursors = new int[17];

        if (Core.Player.IsGameJoltSave == true)
        {
            _userBanned = LogInScreen.UserBanned(Core.GameJoltSave.GameJoltID);
            APICall apiCall = new APICall(GotPublicKeys);
            apiCall.GetKeys(false, "saveStorageV" + GamejoltSave.VERSION + "|*|*");
        }

        if (_userBanned == false)
        {
            _functionList.Add("PSS");
            if (API.LoggedIn == true && Core.Player.IsGameJoltSave == true && Core.Player.Pokemons.Count > 0)
                _functionList.Add("Battle Spot");
            if ((ActionScript.IsRegistered("pokegear_card_GTS") == true || GameController.IS_DEBUG_ACTIVE == true) && API.LoggedIn == true && Core.Player.IsGameJoltSave == true)
                _functionList.Add("GTS");
            if (API.LoggedIn == true && Core.Player.IsGameJoltSave == true && Core.Player.Pokemons.Count > 0)
                _functionList.Add("Wonder Trade");
        }

        if (ActionScript.IsRegistered("pokegear_remove_phone") == false) _functionList.Add("Phone");
        if (ActionScript.IsRegistered("pokegear_card_radio") == true || GameController.IS_DEBUG_ACTIVE == true) _functionList.Add("Radio");
        if (ActionScript.IsRegistered("pokegear_remove_worldmap") == false) _functionList.Add("World Map");
        if (ActionScript.IsRegistered("pokegear_card_minimap") == true || GameController.IS_DEBUG_ACTIVE == true) _functionList.Add("Minimap");
        if (PlayerStatistics.CountStatistics() > 0) _functionList.Add("Statistics");
        if (ActionScript.IsRegistered("pokegear_card_frontier") == true || GameController.IS_DEBUG_ACTIVE == true) _functionList.Add("Frontier");

        switch (entryMode)
        {
            case EntryModes.MainMenu:
                SoundManager.PlaySound("Pokegear\\pokegear_on");
                menuIndex = (MenuScreens)Player.Temp.LastPokegearPage;
                break;
            case EntryModes.DisplayUser: InitializeUserView(data); break;
            case EntryModes.TradeRequest: InitializeTradeRequest(data); break;
            case EntryModes.BattleRequest: InitializeBattleRequest(data); break;
        }
    }

    private void InitializeUserView(Object[] data)
    {
        _userEmblem = null;
        _userSprite = (Texture2D)data[3];
        _userName = (String)data[2];
        _userOrigin = "Server";
        _userNetworkID = (int)data[0];
        menuIndex = MenuScreens.UserView;
        _userViewPreMenu = MenuScreens.Main;
    }

    private void InitializeTradeRequest(Object[] data)
    {
        menuIndex = MenuScreens.TradeRequest;
        _tradeRequestNetworkID = (int)data[0];
        _tradeRequestGameJoltID = (String)data[1];
        SoundManager.PlaySound("Use_Item", false);
        TradeRequestData = -1;
    }

    private void InitializeBattleRequest(Object[] data)
    {
        menuIndex = MenuScreens.BattleRequest;
        _battleRequestNetworkID = (int)data[0];
        _battleRequestGameJoltID = (String)data[1];
        SoundManager.PlaySound("Use_Item", false);
        BattleRequestData = -1;
    }

    private void GotPublicKeys(String result)
    {
        PublicKeys = result;
    }

    public override void Draw()
    {
        PreScreen!.Draw();
        DrawMain();

        switch (menuIndex)
        {
            case MenuScreens.Main: DrawMainMenu(); break;
            case MenuScreens.PSS: DrawPSS(); break;
            case MenuScreens.UserView: DrawUserView(); break;
            case MenuScreens.PhoneList: DrawPhone(); break;
            case MenuScreens.Frontier: DrawFrontier(); break;
            case MenuScreens.MiniMap: DrawMinimap(); break;
            case MenuScreens.Radio: DrawRadio(); break;
            case MenuScreens.TradeRequest: DrawTradeRequest(); break;
            case MenuScreens.BattleRequest: DrawBattleRequest(); break;
        }
    }

    public override void Update()
    {
        switch (menuIndex)
        {
            case MenuScreens.Main: UpdateMainMenu(); break;
            case MenuScreens.PSS: UpdatePSS(); break;
            case MenuScreens.UserView: UpdateUserView(); break;
            case MenuScreens.PhoneList: UpdatePhone(); break;
            case MenuScreens.Frontier: UpdateFrontier(); break;
            case MenuScreens.MiniMap: UpdateMinimap(); break;
            case MenuScreens.Radio: UpdateRadio(); break;
            case MenuScreens.TradeRequest: UpdateTradeRequest(); break;
            case MenuScreens.BattleRequest: UpdateBattleRequest(); break;
        }

        if (menuIndex != MenuScreens.TradeRequest)
        {
            if (KeyBoardHandler.KeyPressed(KeyBindings.SpecialKey) == true || ControllerHandler.ButtonPressed(Buttons.Back) == true)
            {
                if (menuIndex != MenuScreens.UserView) Player.Temp.LastPokegearPage = (int)menuIndex;
                SoundManager.PlaySound("Pokegear\\pokegear_off");
                Core.SetScreen(PreScreen!);
            }
        }
    }

    private void DrawMain()
    {
        Vector2 startPos = GetStartPosition();

        Canvas.DrawRectangle(new Rectangle((int)startPos.X - 1, (int)startPos.Y - 1, _width + 2, _height + 2), new Color(100, 100, 100));
        Canvas.DrawGradient(new Rectangle((int)startPos.X, (int)startPos.Y, _width, 30), new Color(11, 27, 61), new Color(21, 40, 96), false, -1);
        Canvas.DrawGradient(new Rectangle((int)startPos.X, (int)startPos.Y + 30, _width, _height - 30), new Color(225, 220, 94), new Color(210, 172, 73), false, -1);

        String t = TimeHelpers.GetDisplayTime(DateTime.Now, true);
        String t2 = Localization.GetString("pokegear_screen_main_title", "Pokégear");

        Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(startPos.X + _width - FontManager.MainFont.MeasureString(t).X - 5, startPos.Y + 4), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, t2, new Vector2(startPos.X + 5, startPos.Y + 4), Color.White);

        if (API.LoggedIn == true)
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokegear"), new Rectangle((int)(startPos.X + 10 + FontManager.MainFont.MeasureString(t2).X), (int)(startPos.Y + 8), 16, 16), new Rectangle(80, 112, 16, 16), Color.White);
        if (ConnectScreen.Connected == true)
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokegear"), new Rectangle((int)(startPos.X + 34 + FontManager.MainFont.MeasureString(t2).X), (int)(startPos.Y + 8), 16, 16), new Rectangle(112, 112, 16, 16), Color.White);

        Rectangle dayRect = World.GetTime() switch
        {
            World.DayTimes.Night => new Rectangle(64, 112, 8, 8),
            World.DayTimes.Morning => new Rectangle(72, 112, 8, 8),
            World.DayTimes.Day => new Rectangle(64, 120, 8, 8),
            World.DayTimes.Evening => new Rectangle(72, 120, 8, 8),
            _ => new Rectangle(64, 120, 8, 8)
        };
        Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokegear"), new Rectangle((int)(startPos.X + _width - FontManager.MainFont.MeasureString(t).X - 34), (int)(startPos.Y + 6), 16, 16), dayRect, Color.White);
    }

    // ---- Main Menu ----

    private void DrawMainMenu()
    {
        Vector2 startPos = GetStartPosition();
        int x = 0, y = 0;

        for (int i = 0; i <= _functionList.Count - 1; i++)
        {
            String f = _functionList[i];
            Texture2D? t = null;
            String displayText1 = String.Empty;
            String displayText2 = String.Empty;

            switch (f)
            {
                case "PSS": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(64, 32, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_PlayerSearchSystem", "PSS"); break;
                case "Wonder Trade": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(0, 64, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_WonderTrade", "Wonder Trade"); break;
                case "Phone": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(96, 32, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_Phone", "Phone"); break;
                case "Radio": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(32, 32, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_Radio", "Radio"); break;
                case "GTS": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(64, 0, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_GlobalTradeSystem", "GTS"); break;
                case "Frontier": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(32, 64, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_BattleFrontier", "Frontier"); break;
                case "World Map": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(64, 64, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_WorldMap", "World Map"); break;
                case "Minimap": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(96, 64, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_Minimap", "Minimap"); break;
                case "Battle Spot": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(32, 96, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_BattleSpot", "Battle Spot"); break;
                case "Statistics": t = TextureManager.GetTexture("GUI\\Menus\\pokegear", new Rectangle(32, 0, 32, 32), String.Empty); displayText1 = Localization.GetString("pokegear_screen_main_function_Statistics", "Statistics"); break;
            }

            if (displayText1.Contains("~") == true)
            {
                displayText2 = displayText1.Remove(0, displayText1.IndexOf("~") + 1);
                displayText1 = displayText1.Remove(displayText1.IndexOf("~"));
            }

            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokegear"), new Rectangle((int)(startPos.X + (x * 176) + 80 + 20), (int)(startPos.Y + 64 + (y * 128)), 64, 64), new Rectangle(0, 0, 32, 32), Color.White);
            if (t != null)
                Core.SpriteBatch.Draw(t, new Rectangle((int)(startPos.X + (x * 176) + 80 + 20), (int)(startPos.Y + 64 + (y * 128)), 64, 64), new Rectangle(0, 0, 32, 32), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, displayText1, new Vector2((int)(startPos.X + (x * 176) + 80 + 20) + 32 - FontManager.MainFont.MeasureString(displayText1).X / 2.0F, (int)(startPos.Y + 136 + (y * 128))), Color.Black);
            if (displayText2 != String.Empty)
                Core.SpriteBatch.DrawString(FontManager.MainFont, displayText2, new Vector2((int)(startPos.X + (x * 176) + 80 + 20) + 32 - FontManager.MainFont.MeasureString(displayText2).X / 2.0F, (int)(startPos.Y + 136 + (y * 128))), Color.Black);

            if (_cursors[0] == i)
                Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokegear"), new Rectangle((int)(startPos.X + (x * 176) + 80 + 20), (int)(startPos.Y + 64 + (y * 128)), 64, 64), new Rectangle(0, 32, 32, 32), Color.White);

            x += 1;
            while (x > 3) { x -= 4; y += 1; }
        }
    }

    private void UpdateMainMenu()
    {
        if (Controls.Right(true, true, true, true, true, true) == true) _cursors[0] += 1;
        if (Controls.Left(true, true, true, true, true, true) == true) _cursors[0] -= 1;
        if (Controls.Up(true, true, false, true, true, true) == true) _cursors[0] -= 4;
        if (Controls.Down(true, true, false, true, true, true) == true) _cursors[0] += 4;

        _cursors[0] = Math.Clamp(_cursors[0], 0, _functionList.Count - 1);

        if (Controls.Accept(true, false, false) == true)
        {
            Vector2 startPos = GetStartPosition();
            int pressedIndex = -1;
            for (int x = 0; x <= 3; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    Rectangle r = new Rectangle((int)(startPos.X + (x * 176) + 80 + 20), (int)(startPos.Y + 64 + (y * 128)), 64, 64);
                    if (r.Contains(MouseHandler.MousePosition) == true) pressedIndex = x + y * 4;
                }
            }
            if (pressedIndex > -1 && _functionList.Count - 1 >= pressedIndex)
            {
                if (_cursors[0] == pressedIndex) { SoundManager.PlaySound("select"); PressedMainMenuButton(); }
                else _cursors[0] = pressedIndex;
            }
        }

        if (Controls.Accept(false, true, true) == true) { SoundManager.PlaySound("select"); PressedMainMenuButton(); }

        if (Controls.Dismiss(true, true) == true)
        {
            SoundManager.PlaySound("Pokegear\\pokegear_off");
            Core.SetScreen(PreScreen!);
        }
    }

    private void PressedMainMenuButton()
    {
        switch (_functionList[_cursors[0]])
        {
            case "PSS": menuIndex = MenuScreens.PSS; break;
            case "Battle Spot": Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new RegisterBattleScreen(Core.CurrentScreen), Color.White, false)); break;
            case "Phone": menuIndex = MenuScreens.PhoneList; break;
            case "GTS": Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new GTSMainScreen(Core.CurrentScreen), Color.White, false)); break;
            case "Frontier": menuIndex = MenuScreens.Frontier; break;
            case "World Map":
                String argument = Screen.Level!.CurrentRegion;
                if (argument.Contains(",") == true)
                {
                    List<String> regions = argument.Split(',').ToList();
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, regions, 0, ["view"]), Color.White, false));
                }
                else
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, argument, ["view"]), Color.White, false));
                break;
            case "Minimap": menuIndex = MenuScreens.MiniMap; break;
            case "Radio": menuIndex = MenuScreens.Radio; break;
            case "Wonder Trade": Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new WonderTradeScreen(Core.CurrentScreen), Color.White, false)); break;
            case "Statistics": Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new StatisticsScreen(Core.CurrentScreen), Color.White, false)); break;
        }
    }

    // ---- PSS ----

    private int _pssMenuIndex = 0;
    private bool _userBanned = false;

    private void DrawPSS()
    {
        switch (_pssMenuIndex)
        {
            case 0: DrawRanklist(); break;
            case 1: DrawFriendList(); break;
            case 2: DrawLocalList(); break;
        }
    }

    private void UpdatePSS()
    {
        switch (_pssMenuIndex)
        {
            case 0: UpdateRanklist(); break;
            case 1: UpdateFriendList(); break;
            case 2: UpdateLocalList(); break;
        }

        if (Controls.Right(true, true, false, true, true, true) == true) _pssMenuIndex += 1;
        if (Controls.Left(true, true, false, true, true, true) == true) _pssMenuIndex -= 1;

        Vector2 startPos = GetStartPosition();
        Rectangle recRank = new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 46), 184, 32);
        Rectangle recFriends = new Rectangle((int)(startPos.X + 40 + (680 + 32) / 2 - (168 + 32) / 2), (int)(startPos.Y + 46), 184, 32);
        Rectangle recLocal = new Rectangle((int)(startPos.X + 40 + 680 - 168), (int)(startPos.Y + 46), 184, 32);

        if (Controls.Accept(true, false, false) == true)
        {
            if (recRank.Contains(MouseHandler.MousePosition)) _pssMenuIndex = 0;
            if (recFriends.Contains(MouseHandler.MousePosition)) _pssMenuIndex = 1;
            if (recLocal.Contains(MouseHandler.MousePosition)) _pssMenuIndex = 2;
        }

        if (_pssMenuIndex > 2) _pssMenuIndex = 0;
        if (_pssMenuIndex < 0) _pssMenuIndex = 2;

        if (Controls.Dismiss(true, true, true) == true) menuIndex = MenuScreens.Main;
    }

    private void DrawPSSTabHeaders(int activeTab)
    {
        Vector2 startPos = GetStartPosition();
        Texture2D pgt = TextureManager.GetTexture("GUI\\Menus\\pokegear");

        void DrawTab(int xOff, bool flipped)
        {
            SpriteEffects eff = flipped ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + xOff), (int)(startPos.Y + 46), 16, 32), new Rectangle(96, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + xOff + 16), (int)(startPos.Y + 46), 168, 32), new Rectangle(102, 112, 4, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + xOff + 16 + 168), (int)(startPos.Y + 46), 16, 32), new Rectangle(104, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
        }

        DrawTab(40, activeTab != 0);
        DrawTab(40 + (680 + 32) / 2 - (168 + 32) / 2, activeTab != 1);
        DrawTab(40 + 680 - 168, activeTab != 2);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_pss_title_Ranklist", "PSS Ranklist"), new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 48)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_pss_title_Friendlist", "PSS Friendlist"), new Vector2((int)(startPos.X + 48 + (680 + 32) / 2 - (168 + 32) / 2), (int)(startPos.Y + 48)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_pss_title_Passby", "PSS Passby"), new Vector2((int)(startPos.X + 48 + 680 - 168), (int)(startPos.Y + 48)), Color.Black);
    }

    private void DrawEmblemRow(Texture2D spriteTexture, String username, String rankText, int rowY, bool selected)
    {
        Vector2 startPos = GetStartPosition();
        Texture2D pgt = TextureManager.GetTexture("GUI\\Menus\\pokegear");
        SpriteEffects eff = selected ? SpriteEffects.FlipVertically : SpriteEffects.None;
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 40), rowY, 16, 32), new Rectangle(96, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 56), rowY, 680, 32), new Rectangle(102, 112, 4, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 736), rowY, 16, 32), new Rectangle(104, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);

        Size frameSize = new Size(spriteTexture.Width / 3, spriteTexture.Height / 4);
        float frameScale = 1.0F;
        if (spriteTexture.Width == spriteTexture.Height / 2) frameSize.Width = spriteTexture.Width / 2;
        else if (spriteTexture.Width == spriteTexture.Height) frameSize.Width = spriteTexture.Width / 4;
        if (frameSize.Width > 32) frameScale = 32.0F / frameSize.Width;
        if (frameSize.Width <= 16) frameScale = 2.0F;

        Core.SpriteBatch.Draw(spriteTexture, new Rectangle((int)(startPos.X + 64 - frameSize.Width * frameScale / 2), (int)(rowY + 16 - frameSize.Height * frameScale / 2), (int)(frameSize.Width * frameScale), (int)(frameSize.Height * frameScale)), new Rectangle(0, frameSize.Height * 2, frameSize.Width, frameSize.Height), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, username, new Vector2((int)(startPos.X + 88), rowY), Color.Black);
        if (rankText != String.Empty)
            Core.SpriteBatch.DrawString(FontManager.MainFont, rankText, new Vector2((int)(startPos.X + 396), rowY), Color.Black);
    }

    // ---- Ranklist ----

    private int _rankListScroll = 0;
    private int _rankListSelect = 0;
    private int _ownRank = 0;
    private List<Emblem> _rankingList = [];
    private bool _initializedRanklist = false;

    private void DrawRanklist()
    {
        Vector2 startPos = GetStartPosition();
        DrawPSSTabHeaders(0);

        if (Core.Player.IsGameJoltSave == true)
        {
            Emblem ownE = new Emblem(Core.Player.Name, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Points, Core.GameJoltSave.Gender, Core.GameJoltSave.Emblem);
            String ownRankText = _ownRank > -1 ? Localization.GetString("pokegear_screen_pss_RankNumber", "Rank [NUMBER]").Replace("[NUMBER]", _ownRank.ToString()) : String.Empty;
            DrawEmblemRow(ownE.SpriteTexture, ownE.Username, ownRankText, (int)(startPos.Y + 80), false);

            if (_userBanned == false)
            {
                if (PublicKeys != String.Empty)
                {
                    for (int i = 0; i <= 8; i++)
                    {
                        int index = i + _rankListScroll;
                        if (index > _rankingList.Count - 1) continue;
                        Emblem e = _rankingList[index];

                        if (e.DoneLoading == true)
                            DrawEmblemRow(e.SpriteTexture, e.Username, Localization.GetString("pokegear_screen_pss_RankNumber", "Rank [NUMBER]").Replace("[NUMBER]", (index + 1).ToString()) + " (" + e.Points + ")", (int)(startPos.Y + 80 + 36 + i * 36), index == _rankListSelect);
                        else
                        {
                            if (e.startedLoading == false) e.StartLoading(e.Username);
                            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_loading", "Loading") + LoadingDots.Dots, new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 80 + 36 + i * 36)), Color.Black);
                        }
                    }
                    Canvas.DrawScrollBar(new Vector2(startPos.X + _width - 32, startPos.Y + 88), 100, 9, _rankListScroll, new Size(4, 300), false, new Color(252, 196, 68), new Color(217, 120, 18));
                }
                else
                    Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_loading", "Loading") + LoadingDots.Dots, new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 80 + 36)), Color.Black);
            }
        }
        else
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_pss_NoGameJoltProfile", "You are not logged in with a GameJolt profile."), new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 88)), Color.Black);
    }

    private void UpdateRanklist()
    {
        if (_initializedRanklist == false) { InitializeRanklist(); return; }
        if (_rankingList.Count == 0) return;

        int c = Controls.ShiftDown() == true ? 5 : 1;
        Vector2 startPos = GetStartPosition();

        if (Controls.Up(true, true, false, true, true, true) == true) { if (_rankListSelect == _rankListScroll) _rankListScroll -= c; _rankListSelect -= c; }
        if (Controls.Down(true, true, false, true, true, true) == true) { if (_rankListSelect == _rankListScroll + 8) _rankListScroll += c; _rankListSelect += c; }
        if (Controls.Up(true, false, true, false, false, false) == true) { _rankListScroll -= c; _rankListSelect -= c; }
        if (Controls.Down(true, false, true, false, false, false) == true) { _rankListScroll += c; _rankListSelect += c; }

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = 0; i <= 8; i++)
            {
                Rectangle r = new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 80 + 36 + i * 36), 680, 32);
                if (r.Contains(MouseHandler.MousePosition) == false) continue;
                if (_rankListSelect == i + _rankListScroll) { SoundManager.PlaySound("select"); PSSRanklistDisplayUser(); }
                else _rankListSelect = i + _rankListScroll;
                break;
            }
        }

        if (_rankListScroll > _rankListSelect) _rankListScroll = _rankListSelect;
        if (_rankListScroll + 8 < _rankListSelect) _rankListScroll = _rankListSelect - 8;
        _rankListScroll = Math.Clamp(_rankListScroll, 0, Math.Max(0, _rankingList.Count - 9));
        _rankListSelect = Math.Clamp(_rankListSelect, 0, _rankingList.Count - 1);

        if (Controls.Accept(false, true, true) == true) { SoundManager.PlaySound("select"); PSSRanklistDisplayUser(); }
    }

    private void InitializeRanklist()
    {
        if (PublicKeys == String.Empty) return;
        _rankingList.Clear();
        _initializedRanklist = true;
        if (Core.Player.IsGameJoltSave == true && _userBanned == false)
        {
            new APICall(GotDataRanklist).FetchTable(100, "14908");
            new APICall(GotOwnRank).FetchUserRank("14908", Core.GameJoltSave.Points);
        }
    }

    private void GotDataRanklist(String result)
    {
        foreach (API.JoltValue item in API.HandleData(result))
            if (item.Name.ToLower() == "user") _rankingList.Add(new Emblem(item.Value, PublicKeys, false));
    }

    private void GotOwnRank(String result)
    {
        foreach (API.JoltValue item in API.HandleData(result))
            if (item.Name.ToLower() == "rank") _ownRank = int.Parse(item.Value);
    }

    private void PSSRanklistDisplayUser()
    {
        _userEmblem = _rankingList[_rankListSelect];
        _userSprite = _userEmblem.SpriteTexture;
        _userName = _userEmblem.Username;
        _userOrigin = "GJ";
        foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
        {
            if (p.GameJoltId != String.Empty && _userEmblem.GameJoltID == p.GameJoltId)
            {
                _userOrigin += "|Server";
                _userNetworkID = p.ServersID;
            }
        }
        menuIndex = MenuScreens.UserView;
        _userViewPreMenu = MenuScreens.PSS;
    }

    // ---- Friends ----

    private bool _initializedFriends = false;
    private List<Emblem> _friendList = [];
    private int _friendListScroll = 0;
    private int _friendListSelect = 0;

    private void DrawFriendList()
    {
        Vector2 startPos = GetStartPosition();
        DrawPSSTabHeaders(1);

        if (Core.Player.IsGameJoltSave == true)
        {
            Emblem ownE = new Emblem(Core.Player.Name, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Points, Core.GameJoltSave.Gender, Core.GameJoltSave.Emblem);
            String ownRankText = _ownRank > -1 ? Localization.GetString("pokegear_screen_pss_RankNumber", "Rank [NUMBER]").Replace("[NUMBER]", _ownRank.ToString()) : String.Empty;
            DrawEmblemRow(ownE.SpriteTexture, ownE.Username, ownRankText, (int)(startPos.Y + 80), false);

            if (_userBanned == false)
            {
                if (PublicKeys != String.Empty)
                {
                    for (int i = 0; i <= 8; i++)
                    {
                        int index = i + _friendListScroll;
                        if (index > _friendList.Count - 1) continue;
                        Emblem e = _friendList[index];

                        if (e.DoneLoading == true)
                            DrawEmblemRow(e.SpriteTexture, e.Username, Localization.GetString("pokegear_screen_pss_LevelNumber", "Level [NUMBER]").Replace("[NUMBER]", Emblem.GetPlayerLevel(e.Points).ToString()) + " (" + e.Points + ")", (int)(startPos.Y + 80 + 36 + i * 36), index == _friendListSelect);
                        else
                        {
                            if (e.startedLoading == false) e.StartLoading(e.Username);
                            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_loading", "Loading") + LoadingDots.Dots, new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 80 + 36 + i * 36)), Color.Black);
                        }
                    }
                    Canvas.DrawScrollBar(new Vector2(startPos.X + _width - 32, startPos.Y + 88), _friendList.Count, 9, _friendListScroll, new Size(4, 300), false, new Color(252, 196, 68), new Color(217, 120, 18));
                }
                else
                    Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_loading", "Loading") + LoadingDots.Dots, new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 80 + 36)), Color.Black);
            }
        }
        else
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_pss_NoGameJoltProfile", "You are not logged in with a GameJolt profile."), new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 88)), Color.Black);
    }

    private void UpdateFriendList()
    {
        if (_initializedFriends == false) { InitializeFriends(); return; }
        if (_friendList.Count == 0) return;

        int c = Controls.ShiftDown() == true ? 5 : 1;
        Vector2 startPos = GetStartPosition();

        if (Controls.Up(true, true, false, true, true, true) == true) { if (_friendListSelect == _friendListScroll) _friendListScroll -= c; _friendListSelect -= c; }
        if (Controls.Down(true, true, false, true, true, true) == true) { if (_friendListSelect == _friendListScroll + 8) _friendListScroll += c; _friendListSelect += c; }
        if (Controls.Up(true, false, true, false, false, false) == true) { _friendListScroll -= c; _friendListSelect -= c; }
        if (Controls.Down(true, false, true, false, false, false) == true) { _friendListScroll += c; _friendListSelect += c; }

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = 0; i <= 8; i++)
            {
                Rectangle r = new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 80 + i * 36), 680, 32);
                if (r.Contains(MouseHandler.MousePosition) == false) continue;
                if (_friendListSelect == i + _friendListScroll) { SoundManager.PlaySound("select"); PSSFriendlistDisplayUser(); }
                else _friendListSelect = i + _friendListScroll;
                break;
            }
        }

        if (_friendListScroll > _friendListSelect) _friendListScroll = _friendListSelect;
        if (_friendListScroll + 8 < _friendListSelect) _friendListScroll = _friendListSelect - 8;
        _friendListScroll = Math.Clamp(_friendListScroll, 0, Math.Max(0, _friendList.Count - 9));
        _friendListSelect = Math.Clamp(_friendListSelect, 0, _friendList.Count - 1);

        if (Controls.Accept(false, true, true) == true) { SoundManager.PlaySound("select"); PSSFriendlistDisplayUser(); }
    }

    private void InitializeFriends()
    {
        if (PublicKeys == String.Empty) return;
        _initializedFriends = true;
        _friendList.Clear();
        foreach (String friendID in Core.GameJoltSave.Friends.Split(','))
            new APICall(GotFriendListData).FetchUserdataByID(friendID);
    }

    private void GotFriendListData(String result)
    {
        foreach (API.JoltValue item in API.HandleData(result))
        {
            if (item.Name.ToLower() != "username") continue;
            _friendList.Add(new Emblem(item.Value, PublicKeys, false));
            _friendList = _friendList.OrderBy(f => f.Username).ToList();
            return;
        }
    }

    private void PSSFriendlistDisplayUser()
    {
        _userEmblem = _friendList[_friendListSelect];
        _userSprite = _userEmblem.SpriteTexture;
        _userName = _userEmblem.Username;
        _userOrigin = "GJ";
        foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
        {
            if (p.GameJoltId != String.Empty && _userEmblem.GameJoltID == p.GameJoltId)
                _userOrigin += "|Server";
        }
        menuIndex = MenuScreens.UserView;
        _userViewPreMenu = MenuScreens.PSS;
    }

    // ---- Local (Passby) ----

    public class LocalPlayer
    {
        public int NetworkID = 0;
        public Texture2D Sprite = null!;
        public String Name = String.Empty;
        public String GamejoltID = String.Empty;

        public LocalPlayer(int networkID) { NetworkID = networkID; }

        public void LoadOnlineSprite()
        {
            System.Threading.Thread t = new System.Threading.Thread(() => DownloadSprite());
            t.IsBackground = true;
            t.Start();
        }

        private void DownloadSprite()
        {
            if (GamejoltID != String.Empty)
            {
                Texture2D? tex = Emblem.GetOnlineSprite(GamejoltID);
                if (tex != null) Sprite = tex;
            }
        }
    }

    private List<LocalPlayer> _localList = [];
    private bool _initializedLocals = false;
    private int _localScroll = 0;
    private int _localSelect = 0;

    private void FillLocalList()
    {
        _localList.Clear();
        if (ConnectScreen.Connected == false) return;
        foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
        {
            if (p.ServersID == Core.ServersManager.ID) continue;
            String tPath = NetworkPlayer.GetTexturePath(p.Skin);
            Texture2D t = TextureManager.TextureExist(tPath) == true ? TextureManager.GetTexture(tPath) : TextureManager.GetTexture("Textures\\NPC\\0");
            LocalPlayer lP = new LocalPlayer(p.ServersID) { GamejoltID = p.GameJoltId, Name = p.Name, Sprite = t };
            lP.LoadOnlineSprite();
            _localList.Add(lP);
        }
    }

    private void DrawLocalList()
    {
        Vector2 startPos = GetStartPosition();
        DrawPSSTabHeaders(2);

        if (_initializedLocals == false) return;

        if (_localList.Count > 0)
        {
            for (int i = 0; i <= 8; i++)
            {
                int index = i + _localScroll;
                if (index > _localList.Count - 1) continue;
                LocalPlayer lP = _localList[index];
                bool selected = index == _localSelect;

                DrawEmblemRow(lP.Sprite, lP.Name, String.Empty, (int)(startPos.Y + 80 + i * 36), selected);
            }
            Canvas.DrawScrollBar(new Vector2(startPos.X + _width - 32, startPos.Y + 88), _localList.Count, 9, _localScroll, new Size(4, 300), false, new Color(252, 196, 68), new Color(217, 120, 18));
        }
        else
        {
            String msg = ConnectScreen.Connected == true
                ? Localization.GetString("pokegear_screen_pss_NoOtherPlayersOnServer", "No other players connected to server.") + Environment.NewLine + "\"" + JoinServerScreen.SelectedServer!.GetName() + "\"."
                : Localization.GetString("pokegear_screen_pss_YouArePlayingLocally", "You are playing locally.");
            Core.SpriteBatch.DrawString(FontManager.MainFont, msg, new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 88)), Color.Black);
        }
    }

    private void UpdateLocalList()
    {
        if (_initializedLocals == false) { _initializedLocals = true; FillLocalList(); return; }
        if (_localList.Count == 0) return;

        int c = Controls.ShiftDown() == true ? 5 : 1;
        Vector2 startPos = GetStartPosition();

        if (Controls.Up(true, true, false, true, true, true) == true) { if (_localSelect == _localScroll) _localScroll -= c; _localSelect -= c; }
        if (Controls.Down(true, true, false, true, true, true) == true) { if (_localSelect == _localScroll + 8) _localScroll += c; _localSelect += c; }
        if (Controls.Up(true, false, true, false, false, false) == true) { _localScroll -= c; _localSelect -= c; }
        if (Controls.Down(true, false, true, false, false, false) == true) { _localScroll += c; _localSelect += c; }

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = 0; i <= 8; i++)
            {
                Rectangle r = new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 80 + i * 36), 680, 32);
                if (r.Contains(MouseHandler.MousePosition) == false) continue;
                if (_localSelect == i + _localScroll) { SoundManager.PlaySound("select"); PSSLocallistDisplayUser(); }
                else _localSelect = i + _localScroll;
                break;
            }
        }

        if (_localScroll > _localSelect) _localScroll = _localSelect;
        if (_localScroll + 8 < _localSelect) _localScroll = _localSelect - 8;
        _localScroll = Math.Clamp(_localScroll, 0, Math.Max(0, _localList.Count - 9));
        _localSelect = Math.Clamp(_localSelect, 0, _localList.Count - 1);

        if (Controls.Accept(false, true, true) == true) { SoundManager.PlaySound("select"); PSSLocallistDisplayUser(); }
    }

    private void PSSLocallistDisplayUser()
    {
        _userEmblem = _localList[_localSelect].GamejoltID != String.Empty ? new Emblem(_localList[_localSelect].GamejoltID, 0) : null;
        _userSprite = _localList[_localSelect].Sprite;
        _userName = _localList[_localSelect].Name;
        _userOrigin = "Server";
        _userNetworkID = _localList[_localSelect].NetworkID;
        menuIndex = MenuScreens.UserView;
        _userViewPreMenu = MenuScreens.PSS;
    }

    // ---- User View ----

    private Emblem? _userEmblem = null;
    private Texture2D? _userSprite = null;
    private String _userName = String.Empty;
    private String _userOrigin = String.Empty;
    private MenuScreens _userViewPreMenu = MenuScreens.Main;
    private int _userNetworkID = -1;

    private void DrawUserView()
    {
        Vector2 startPos = GetStartPosition();
        Texture2D pgt = TextureManager.GetTexture("GUI\\Menus\\pokegear");

        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 80), (int)(startPos.Y + 46), 16, 32), new Rectangle(96, 112, 8, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 96), (int)(startPos.Y + 46), 480, 32), new Rectangle(102, 112, 4, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 576), (int)(startPos.Y + 46), 16, 32), new Rectangle(104, 112, 8, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 300), (int)(startPos.Y + 46), 16, 32), new Rectangle(104, 112, 8, 16), Color.White);

        if (_userSprite != null)
        {
            Size fS = new Size(_userSprite.Width / 3, _userSprite.Height / 4);
            float fScale = 1.0F;
            if (_userSprite.Width == _userSprite.Height / 2) fS.Width = _userSprite.Width / 2;
            else if (_userSprite.Width == _userSprite.Height) fS.Width = _userSprite.Width / 4;
            if (fS.Width > 32) fScale = 32.0F / fS.Width;
            if (fS.Width <= 16) fScale = 2.0F;
            Core.SpriteBatch.Draw(_userSprite, new Rectangle((int)(startPos.X + 64 - fS.Width * fScale / 2), (int)(startPos.Y + 62 - fS.Height * fScale / 2), (int)(fS.Width * fScale), (int)(fS.Height * fScale)), new Rectangle(0, fS.Height * 2, fS.Width, fS.Height), Color.White);
        }
        Core.SpriteBatch.DrawString(FontManager.MainFont, _userName, new Vector2((int)(startPos.X + 88), (int)(startPos.Y + 48)), Color.Black);

        if (_userOrigin.Contains("GJ") == true) Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 284), (int)(startPos.Y + 46), 16, 16), new Rectangle(80, 112, 16, 16), Color.White);
        if (_userOrigin.Contains("Server") == true) Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 308), (int)(startPos.Y + 46), 16, 16), new Rectangle(112, 112, 16, 16), Color.White);

        bool sameServer = false;
        bool sameConnection = Core.Player.IsGameJoltSave == true ? _userEmblem != null : _userEmblem == null;
        if (ConnectScreen.Connected == true)
            foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
                if (p.Name == _userName) sameServer = true;

        for (int x = 0; x <= 4; x++)
        {
            Color c = Color.White;
            String label = String.Empty;
            Vector2 texV = Vector2.Zero;

            switch (x)
            {
                case 0: label = Localization.GetString("pokegear_screen_pss_user_function_Battle", "Battle"); if (sameServer == false) c = Color.Gray; texV = new Vector2(32, 96); break;
                case 1: label = Localization.GetString("pokegear_screen_pss_user_function_Trade", "Trade"); if (sameServer == false || sameConnection == false) c = Color.Gray; texV = new Vector2(0, 96); break;
                case 2: label = Localization.GetString("pokegear_screen_pss_user_function_PersonalMessage", "PM"); if (sameServer == false) c = Color.Gray; break;
                case 3: label = Localization.GetString("pokegear_screen_pss_user_function_Friend", "Friend"); if (_userEmblem == null) c = Color.Gray; break;
                case 4: label = Localization.GetString("pokegear_screen_pss_user_function_Favorite", "Favorite"); if (_userEmblem == null) c = Color.Gray; break;
            }

            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + (x * 96) + 76), (int)(startPos.Y + 100), 64, 64), new Rectangle(0, 0, 32, 32), c);
            if (texV != Vector2.Zero) Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + (x * 96) + 76), (int)(startPos.Y + 100), 64, 64), new Rectangle((int)texV.X, (int)texV.Y, 32, 32), c);
            if (_cursors[1] == x) Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + (x * 96) + 76), (int)(startPos.Y + 100), 64, 64), new Rectangle(0, 32, 32, 32), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, label, new Vector2((int)(startPos.X + (x * 96) + 76) + 32 - FontManager.MainFont.MeasureString(label).X / 2.0F, (int)(startPos.Y + 170)), Color.Black);
        }

        _userEmblem?.Draw(new Vector2(startPos.X + 40, startPos.Y + 240), 4);
    }

    private void UpdateUserView()
    {
        if (Controls.Left(true, true) == true) _cursors[1] -= 1;
        if (Controls.Right(true, true) == true) _cursors[1] += 1;
        _cursors[1] = Math.Clamp(_cursors[1], 0, 4);

        bool mouseAccept = false;
        if (Controls.Accept(true, false, false) == true)
        {
            Vector2 startPos = GetStartPosition();
            int pressedIndex = -1;
            for (int x = 0; x <= 4; x++)
            {
                if (new Rectangle((int)(startPos.X + (x * 96) + 76), (int)(startPos.Y + 100), 64, 64).Contains(MouseHandler.MousePosition) == true) pressedIndex = x;
            }
            if (pressedIndex > -1)
            {
                if (_cursors[1] == pressedIndex) { SoundManager.PlaySound("select"); mouseAccept = true; }
                else _cursors[1] = pressedIndex;
            }
        }

        if (Controls.Accept(false, true, true) == true || mouseAccept == true)
        {
            bool sameServer = false;
            bool sameConnection = Core.Player.IsGameJoltSave == true ? _userEmblem != null : _userEmblem == null;
            if (ConnectScreen.Connected == true)
                foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
                    if (p.Name == _userName) sameServer = true;

            switch (_cursors[1])
            {
                case 0:
                    if (sameServer == true) { SoundManager.PlaySound("select"); Core.SetScreen(new PVPLobbyScreen(Core.CurrentScreen, _userNetworkID, true)); }
                    break;
                case 1:
                    if (sameServer == true && sameConnection == true) { SoundManager.PlaySound("select"); Core.SetScreen(new DirectTradeScreen(Core.CurrentScreen, _userNetworkID, true)); }
                    break;
                case 2:
                    if (sameServer == true)
                    {
                        ChatScreen chat = new ChatScreen(Core.CurrentScreen);
                        chat.EnterPMChat(_userName);
                        Core.SetScreen(chat);
                    }
                    break;
                case 3: break;
                case 4: break;
            }
        }

        if (Controls.Dismiss(true, true, true) == true) { SoundManager.PlaySound("select"); menuIndex = _userViewPreMenu; }
    }

    // ---- Phone ----

    public static String Call_Flag = String.Empty;

    private struct Contact
    {
        public String Name;
        public String ID;
        public String Texture;
        public String Location;
        public bool CanRandomCall;
    }

    private List<Contact> _phoneContacts = [];
    private bool _initializedPhone = false;
    private int _phoneScroll = 0;
    private int _phoneSelect = 0;

    private void DrawPhone()
    {
        Vector2 startPos = GetStartPosition();
        Texture2D pgt = TextureManager.GetTexture("GUI\\Menus\\pokegear");

        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 46), 16, 32), new Rectangle(96, 112, 8, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 56), (int)(startPos.Y + 46), 192, 32), new Rectangle(102, 112, 4, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 248), (int)(startPos.Y + 46), 16, 32), new Rectangle(104, 112, 8, 16), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_main_function_Phone", "Phone"), new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 48)), Color.Black);

        if (_phoneContacts.Count > 0)
        {
            for (int i = 0; i <= 8; i++)
            {
                int index = i + _phoneScroll;
                if (index > _phoneContacts.Count - 1) continue;
                Contact contact = _phoneContacts[index];
                SpriteEffects eff = index == _phoneSelect ? SpriteEffects.FlipVertically : SpriteEffects.None;

                Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 80 + i * 36), 16, 32), new Rectangle(96, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
                Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 56), (int)(startPos.Y + 80 + i * 36), 680, 32), new Rectangle(102, 112, 4, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
                Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 736), (int)(startPos.Y + 80 + i * 36), 16, 32), new Rectangle(104, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);

                Texture2D sprite = TextureManager.GetTexture("Textures\\NPC\\" + contact.Texture);
                Size fS = new Size(sprite.Width / 3, sprite.Height / 4);
                float fScale = 1.0F;
                if (sprite.Width == sprite.Height / 2) fS.Width = sprite.Width / 2;
                else if (sprite.Width == sprite.Height) fS.Width = sprite.Width / 4;
                if (fS.Width > 32) fScale = 32.0F / fS.Width;
                if (fS.Width <= 16) fScale = 2.0F;
                Core.SpriteBatch.Draw(sprite, new Rectangle((int)(startPos.X + 48), (int)(startPos.Y + 96 - fS.Height * fScale / 2 + i * 36), (int)(fS.Width * fScale), (int)(fS.Height * fScale)), new Rectangle(0, fS.Height * 2, fS.Width, fS.Height), Color.White);
                Core.SpriteBatch.DrawString(FontManager.MainFont, ScriptVersion2.ScriptCommander.Parse(contact.Name)?.ToString() ?? contact.Name, new Vector2((int)(startPos.X + 88), (int)(startPos.Y + 84 + i * 36)), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_location", "Location") + ": " + Localization.GetString("Places_" + contact.Location, contact.Location), new Vector2((int)(startPos.X + 396), (int)(startPos.Y + 84 + i * 36)), Color.Black);
            }
            Canvas.DrawScrollBar(new Vector2(startPos.X + _width - 32, startPos.Y + 88), _phoneContacts.Count, 9, _phoneScroll, new Size(4, 300), false, new Color(252, 196, 68), new Color(217, 120, 18));
        }
    }

    private void UpdatePhone()
    {
        if (_initializedPhone == false) { InitializePhone(); return; }
        if (_phoneContacts.Count > 0)
        {
            int c = Controls.ShiftDown() == true ? 5 : 1;
            Vector2 startPos = GetStartPosition();

            if (Controls.Up(true, true, false, true, true, true) == true) { if (_phoneSelect == _phoneScroll) _phoneScroll -= c; _phoneSelect -= c; }
            if (Controls.Down(true, true, false, true, true, true) == true) { if (_phoneSelect == _phoneScroll + 8) _phoneScroll += c; _phoneSelect += c; }
            if (Controls.Up(true, false, true, false, false, false) == true) { _phoneScroll -= c; _phoneSelect -= c; }
            if (Controls.Down(true, false, true, false, false, false) == true) { _phoneScroll += c; _phoneSelect += c; }

            if (Controls.Accept(true, false, false) == true)
            {
                for (int i = 0; i <= 8; i++)
                {
                    if (new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 80 + i * 36), 712, 32).Contains(MouseHandler.MousePosition) == false) continue;
                    if (_phoneSelect == i + _phoneScroll) { SoundManager.PlaySound("select"); SelectPhoneContact(); }
                    else _phoneSelect = i + _phoneScroll;
                    break;
                }
            }

            if (_phoneScroll > _phoneSelect) _phoneScroll = _phoneSelect;
            if (_phoneScroll + 8 < _phoneSelect) _phoneScroll = _phoneSelect - 8;
            _phoneScroll = Math.Clamp(_phoneScroll, 0, Math.Max(0, _phoneContacts.Count - 9));
            _phoneSelect = Math.Clamp(_phoneSelect, 0, _phoneContacts.Count - 1);

            if (Controls.Accept(false, true, true) == true) { SoundManager.PlaySound("select"); SelectPhoneContact(); }
        }

        if (Controls.Dismiss(true, true, true) == true) { SoundManager.PlaySound("select"); menuIndex = MenuScreens.Main; }
    }

    private void SelectPhoneContact()
    {
        String chosenID = _phoneContacts[_phoneSelect].ID;
        Call_Flag = "calling";
        Player.Temp.LastPokegearPage = (int)menuIndex;
        Core.SetScreen(PreScreen!);
        ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript("phone\\" + chosenID, 0, false, false, "PhoneCall");
    }

    private void InitializePhone()
    {
        _initializedPhone = true;
        String file = GameModeManager.GetContentFilePath("Data\\contacts.dat");
        if (System.IO.File.Exists(file) == false) return;

        String[] reg = Core.Player.RegisterData.Split(',');
        String[] contactData = System.IO.File.ReadAllLines(file);

        foreach (String r in reg)
        {
            if (r.StartsWith("phone_contact_") == false) continue;
            String newID = r.Remove(0, "phone_contact_".Length);
            foreach (String line in contactData)
            {
                if (line.StartsWith(newID + "|") == false) continue;
                String[] ncd = line.Split('|');
                _phoneContacts.Add(new Contact { ID = ncd[0], Name = ncd[1], Texture = ncd[2], Location = ncd[3], CanRandomCall = bool.Parse(ncd[4]) });
            }
        }
        _phoneContacts = _phoneContacts.OrderBy(c => c.ID).ToList();
    }

    public static void CallID(String id, bool checkRegistered, bool checkLocation)
    {
        String file = GameModeManager.GetContentFilePath("Data\\contacts.dat");
        Security.FileValidation.CheckFileValid(file, false, "PokegearScreen.cs");
        String[] contactData = System.IO.File.ReadAllLines(file);
        String[] reg = Core.Player.RegisterData.Split(',');
        List<Contact> tempContacts = [];

        foreach (String r in reg)
        {
            if (r.StartsWith("phone_contact_") == false) continue;
            String newID = r.Remove(0, "phone_contact_".Length);
            if (newID != id) continue;
            foreach (String line in contactData)
            {
                if (line.StartsWith(newID + "|") == false && checkRegistered == true) continue;
                String[] ncd = line.Split('|');
                tempContacts.Add(new Contact { ID = ncd[0], Name = ncd[1], Texture = ncd[2], Location = ncd[3], CanRandomCall = bool.Parse(ncd[4]) });
            }
        }

        int count = 0;
        foreach (Contact c in tempContacts) if (c.Location.ToLower() != Level.MapName.ToLower() || checkLocation == false) count += 1;
        if (count == 0) return;

        String chosenID = "-1";
        while (chosenID == "-1")
        {
            Contact nC = tempContacts[Core.Random.Next(0, tempContacts.Count)];
            if (nC.Location.ToLower() != Level.MapName.ToLower() || checkLocation == false) chosenID = nC.ID;
        }
        Call_Flag = "receiving";
        ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript("phone\\" + chosenID, 0, false, false, "PhoneReceiving");
    }

    public static void RandomCall()
    {
        if (Core.CurrentScreen.Identification != Identifications.OverworldScreen) return;
        String file = GameModeManager.GetContentFilePath("Data\\contacts.dat");
        if (System.IO.File.Exists(file) == false) return;

        Security.FileValidation.CheckFileValid(file, false, "PokegearScreen.cs");
        String[] contactData = System.IO.File.ReadAllLines(file);
        String[] reg = Core.Player.RegisterData.Split(',');
        List<Contact> tempContacts = [];

        foreach (String r in reg)
        {
            if (r.StartsWith("phone_contact_") == false) continue;
            String newID = r.Remove(0, "phone_contact_".Length);
            foreach (String line in contactData)
            {
                if (line.StartsWith(newID + "|") == false) continue;
                String[] ncd = line.Split('|');
                tempContacts.Add(new Contact { ID = ncd[0], Name = ncd[1], Texture = ncd[2], Location = ncd[3], CanRandomCall = bool.Parse(ncd[4]) });
            }
        }

        int count = 0;
        foreach (Contact c in tempContacts) if (c.CanRandomCall == true && c.Location.ToLower() != Level.MapName.ToLower()) count += 1;
        if (count == 0) return;

        String chosenID = "-1";
        while (chosenID == "-1")
        {
            Contact nC = tempContacts[Core.Random.Next(0, tempContacts.Count)];
            if (nC.CanRandomCall == true && nC.Location.ToLower() != Level.MapName.ToLower()) chosenID = nC.ID;
        }
        Call_Flag = "receiving";
        ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript("phone\\" + chosenID, 0, false, false, "PhoneReceiving");
    }

    // ---- Frontier ----

    public class FrontierSymbol
    {
        public String Name = String.Empty;
        public Texture2D Texture = null!;
        public String Description = String.Empty;
    }

    private List<FrontierSymbol> _frontierList = [];
    private bool _initializedFrontier = false;

    private void DrawFrontier()
    {
        Vector2 startPos = GetStartPosition();
        Texture2D pgt = TextureManager.GetTexture("GUI\\Menus\\pokegear");

        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 46), 16, 32), new Rectangle(96, 112, 8, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 56), (int)(startPos.Y + 46), 224, 32), new Rectangle(102, 112, 4, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 280), (int)(startPos.Y + 46), 16, 32), new Rectangle(104, 112, 8, 16), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_frontier_title", "Frontier Emblems"), new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 48)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_frontier_CurrentBattlePoints", "Current BP: [NUMBER]").Replace("[NUMBER]", Core.Player.BP.ToString()), new Vector2((int)(startPos.X + 320), (int)(startPos.Y + 48)), Color.Black);

        if (_frontierList.Count > 0)
        {
            for (int x = 0; x <= _frontierList.Count - 1; x++)
            {
                Core.SpriteBatch.Draw(_frontierList[x].Texture, new Rectangle((int)(startPos.X + (x * 96) + 83 + 40), (int)(startPos.Y + 107), 50, 50), Color.White);
                if (_cursors[2] == x)
                {
                    Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + (x * 96) + 76 + 40), (int)(startPos.Y + 100), 64, 64), new Rectangle(0, 32, 32, 32), Color.White);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, _frontierList[x].Name, new Vector2((int)(startPos.X + (x * 96) + 76) + 40 + 32 - FontManager.MainFont.MeasureString(_frontierList[x].Name).X / 2.0F, (int)(startPos.Y + 170)), Color.Black);
                    Canvas.DrawGradient(new Rectangle((int)(startPos.X + 48), (int)(startPos.Y + 220), _width - 100, _height - 260), Color.White, Color.Gray, false, -1);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, _frontierList[x].Description, new Vector2((int)(startPos.X + 55), (int)(startPos.Y + 225)), Color.Black);
                }
            }
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_frontier_NoFrontierEmblems", "You don't own any Frontier Emblems yet."), new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 88)), Color.Black);
            Canvas.DrawGradient(new Rectangle((int)(startPos.X + 48), (int)(startPos.Y + 220), _width - 100, _height - 260), Color.White, Color.Gray, false, -1);
        }
    }

    private void UpdateFrontier()
    {
        if (_initializedFrontier == false) InitializeFrontier();
        else
        {
            if (Controls.Right(true, true, true, true, true, true) == true) _cursors[2] += 1;
            if (Controls.Left(true, true, true, true, true, true) == true) _cursors[2] -= 1;
            if (_frontierList.Count > 0) _cursors[2] = Math.Clamp(_cursors[2], 0, _frontierList.Count - 1);
        }
        if (Controls.Dismiss(true, true, true) == true) menuIndex = MenuScreens.Main;
    }

    private void InitializeFrontier()
    {
        _frontierList.Clear();
        _initializedFrontier = true;

        String file = GameModeManager.GetContentFilePath("Data\\badges.dat");
        Security.FileValidation.CheckFileValid(file, false, "Badge.cs");
        String[] data = System.IO.File.ReadAllLines(file);

        foreach (String line in data)
        {
            if (line.Contains("|") == false || StringHelper.IsNumeric(line.GetSplit(0, "|")) == true) continue;

            bool hasGold = line.GetSplit(0, "|").EndsWith("_gold") && ActionScript.IsRegistered(line.GetSplit(0, "|")) == true;
            bool hasSilver = false;
            if (line.GetSplit(0, "|").EndsWith("_silver") && ActionScript.IsRegistered(line.GetSplit(0, "|")) == true)
            {
                String goldKey = line.GetSplit(0, "|").Remove(line.GetSplit(0, "|").Length - "_silver".Length) + "_gold";
                hasSilver = ActionScript.IsRegistered(goldKey) == false;
            }
            if (hasSilver == false && hasGold == false) continue;

            String emblemName = String.Empty, emblemDesc = String.Empty;
            Texture2D? emblemTexture = null;

            String[] lineData = line.Split('|');
            for (int i = 1; i <= lineData.Length - 1; i++)
            {
                if (lineData[i].ToLower().StartsWith("name=")) emblemName = Localization.GetString(line.GetSplit(0, "|") + "_name", lineData[i].Remove(0, 5));
                else if (lineData[i].ToLower().StartsWith("description=")) emblemDesc = Localization.GetString(line.GetSplit(0, "|") + "_description", lineData[i].Remove(0, 12));
                else if (lineData[i].ToLower().StartsWith("texture="))
                {
                    String[] texData = lineData[i].Remove(0, 8).Split(',');
                    emblemTexture = TextureManager.GetTexture(texData[0], new Rectangle(int.Parse(texData[1]), int.Parse(texData[2]), int.Parse(texData[3]), int.Parse(texData[4])), String.Empty);
                }
            }
            if (emblemName == String.Empty && Localization.TokenExists(line.GetSplit(0, "|") + "_name") == true) emblemName = Localization.GetString(line.GetSplit(0, "|") + "_name");
            if (emblemDesc == String.Empty && Localization.TokenExists(line.GetSplit(0, "|") + "_description") == true) emblemDesc = Localization.GetString(line.GetSplit(0, "|") + "_description");

            if (emblemTexture != null)
                _frontierList.Add(new FrontierSymbol { Name = emblemName.Replace("~", Environment.NewLine), Description = emblemDesc.Replace("~", Environment.NewLine), Texture = emblemTexture });
        }
    }

    // ---- Minimap ----

    private Minimap? _miniMap = null;
    private bool _initializedMinimap = false;

    private void DrawMinimap()
    {
        Vector2 startPos = GetStartPosition();
        Texture2D pgt = TextureManager.GetTexture("GUI\\Menus\\pokegear");

        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 405), (int)(startPos.Y + 46), 16, 32), new Rectangle(96, 112, 8, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 421), (int)(startPos.Y + 46), 128, 32), new Rectangle(102, 112, 4, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 549), (int)(startPos.Y + 46), 16, 32), new Rectangle(104, 112, 8, 16), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_main_function_Minimap", "Minimap"), new Vector2((int)(startPos.X + 412), (int)(startPos.Y + 48)), Color.Black);

        if (_initializedMinimap == true)
        {
            Canvas.DrawBorder(1, new Rectangle((int)(startPos.X + 57), (int)(startPos.Y + 47), 21 * 16 + 2, 21 * 16 + 2), Color.Black);
            _miniMap!.Draw(new Vector2(-(startPos.X + 88), -(startPos.Y + 80)));
        }
    }

    private void UpdateMinimap()
    {
        if (_initializedMinimap == false) { _initializedMinimap = true; _miniMap = new Minimap(); _miniMap.Initialize(); }
        if (Controls.Dismiss(true, true, true) == true) menuIndex = MenuScreens.Main;
    }

    // ---- Radio ----

    public class RadioStation
    {
        public decimal ChannelMin;
        public decimal ChannelMax;
        public decimal OverwriteMin;
        public decimal OverwriteMax;
        public String Name = String.Empty;
        public String Region = String.Empty;
        public List<World.DayTimes> DayTimes = [];
        public List<String> Expansions = [];
        public String Music = String.Empty;
        public String Content = String.Empty;
        public bool CanBeOverwritten = true;
        public String Activation = String.Empty;
        public String ActivationRegister = String.Empty;

        public RadioStation(String input)
        {
            String[] data = input.Split('|');

            if (data[0].Contains("-") == true)
            {
                String[] cd = data[0].Split('-');
                ChannelMin = decimal.Parse(cd[0].Replace(".", GameController.DecSeparator));
                ChannelMax = decimal.Parse(cd[1].Replace(".", GameController.DecSeparator));
            }
            else { ChannelMin = decimal.Parse(data[0].Replace(".", GameController.DecSeparator)); ChannelMax = ChannelMin; }

            if (data[1].Contains("-") == true)
            {
                String[] cd = data[1].Split('-');
                OverwriteMin = decimal.Parse(cd[0].Replace(".", GameController.DecSeparator));
                OverwriteMax = decimal.Parse(cd[1].Replace(".", GameController.DecSeparator));
            }
            else { OverwriteMin = decimal.Parse(data[1].Replace(".", GameController.DecSeparator)); OverwriteMax = OverwriteMin; }

            Name = ScriptVersion2.ScriptCommander.Parse(data[2])?.ToString() ?? data[2];
            Region = data[3];

            foreach (String s in data[4].Split(','))
                if (StringHelper.IsNumeric(s) == true) DayTimes.Add((World.DayTimes)int.Parse(s));

            if (data[5] != String.Empty)
                foreach (String exp in data[5].Split(',')) Expansions.Add(exp);

            Music = data[6];
            Content = data[7];
            CanBeOverwritten = bool.Parse(data[8]);
            Activation = data[9];
            ActivationRegister = data[10];
        }

        public bool CanListen()
        {
            if (DayTimes.Contains(World.GetTime()) == false) return false;
            String[] regions = Screen.Level!.CurrentRegion.ToLower().Split(',');
            if (regions.Contains(Region.ToLower()) == false) return false;

            foreach (String exp in Expansions)
                if (exp.ToLower() != "radio" && ActionScript.IsRegistered("pokegear_card_radio" + exp) == false) return false;

            if (Activation == "1" && Screen.Level.AllowedRadioChannels.Contains(ChannelMin) == false) return false;
            if (ActivationRegister != "0" && ActionScript.IsRegistered(ActivationRegister) == false) return false;
            return true;
        }

        public List<RadioStation> OverwriteChannels(List<RadioStation> channelList)
        {
            if (CanListen() == false) return channelList;
            List<RadioStation> newList = [];
            foreach (RadioStation c in channelList)
                if (c.CanBeOverwritten == false || c.ChannelMin < OverwriteMin || c.ChannelMax > OverwriteMax) newList.Add(c);
            newList.Add(this);
            return newList;
        }

        public bool IsInterfering(decimal frequency) => frequency >= ChannelMin && frequency <= ChannelMax;

        public List<String> GenerateText()
        {
            String output = "...~...~...~...~...~...";

            switch (Content.ToLower())
            {
                case "[pokedexentry]":
                {
                    List<int> triedIDs = [];
                    int chosenID = -1;
                    String chosenAD = String.Empty;

                    while (chosenID == -1 && triedIDs.Count < Pokedex.PokemonMaxCount)
                    {
                        int id = Core.Random.Next(1, Pokedex.PokemonMaxCount + 1);
                        if (triedIDs.Contains(id) == true) continue;
                        List<String> formList = PokemonForms.GetDataFileForms(id);
                        if (formList.Count > 0)
                        {
                            int formIndex = Core.Random.Next(0, formList.Count);
                            while (formList.Count > 0)
                            {
                                if (Pokedex.GetEntryType(Core.Player.PokedexData, formList[formIndex]) < 2)
                                {
                                    formList.RemoveAt(formIndex);
                                    if (formList.Count == 0) break;
                                    formIndex = Core.Random.Next(0, formList.Count);
                                }
                                else
                                {
                                    if (formList[formIndex].Contains("_") == true) { chosenID = int.Parse(formList[formIndex].GetSplit(0, "_")); chosenAD = PokemonForms.GetAdditionalValueFromDataFile(formList[formIndex]); }
                                    else chosenID = int.Parse(formList[formIndex]);
                                    break;
                                }
                            }
                            if (chosenID == -1) triedIDs.Add(id);
                        }
                        else
                        {
                            if (Pokedex.GetEntryType(Core.Player.PokedexData, id.ToString()) < 2) triedIDs.Add(id);
                            else chosenID = id;
                        }
                    }

                    if (chosenID > -1)
                    {
                        Pokemon p = Pokemon.GetPokemonByID(chosenID, chosenAD);
                        String dexText = p.pokedexEntry?.Text ?? String.Empty;
                        String dexSpecies = p.pokedexEntry?.Species ?? String.Empty;
                        String formName = PokemonForms.GetFormName(p);
                        if (formName == String.Empty) formName = p.Name;
                        if (Localization.TokenExists("pokemon_desc_" + formName) == true) dexText = Localization.GetString("pokemon_desc_" + formName, dexText);
                        if (Localization.TokenExists("pokemon_species_" + formName) == true) dexSpecies = Localization.GetString("pokemon_species_" + formName, dexSpecies);
                        output = Localization.GetString("pokegear_screen_radio_PokédexShow_Content", "Welcome to the Pokédex Show! Today, we are going to look at the entry of [NAME]! Its entry reads:~\"[DEXENTRY]\"~Wow, that is interesting! Also, [NAME] is [HEIGHT]m high and weighs [WEIGHT]kg.~Isn't that amazing?~[NAME] is part of the [SPECIES] species.~That's all the information we have. Tune in next time!")
                            .Replace("[NAME]", p.GetName(true)).Replace("[DEXENTRY]", dexText).Replace("[HEIGHT]", (p.pokedexEntry?.Height ?? 0).ToString()).Replace("[WEIGHT]", (p.pokedexEntry?.Weight ?? 0).ToString()).Replace("[SPECIES]", dexSpecies);
                    }
                    break;
                }
                case "[randompokemon]":
                {
                    String[] levels = ["route29.dat", "route30.dat", "route31.dat", "route32.dat", "route33.dat", "route36.dat", "route37.dat", "route38.dat", "route39.dat", "routes\\route34.dat", "routes\\route35.dat", "routes\\route42.dat", "routes\\route43.dat", "routes\\route44.dat", "routes\\route45.dat", "routes\\route46.dat"];
                    String cLevel = levels[Core.Random.Next(0, levels.Length)];
                    Pokemon? p = Spawner.GetPokemon(cLevel, Spawner.EncounterMethods.Land, false);
                    String levelName = cLevel;
                    if (levelName.Contains("\\") == true) levelName = levelName.Remove(0, levelName.LastIndexOf("\\") + 1);
                    levelName = levelName.Remove(levelName.Length - 4);
                    levelName = levelName[0].ToString().ToUpper() + levelName.Remove(0, 1);
                    if (levelName.Length > 5) levelName = levelName.Substring(0, 5) + " " + levelName.Remove(0, 5);
                    if (p != null) output = Localization.GetString("pokegear_screen_radio_PokemonTalk_Content", "Professor Oak's Pokémon Talk! With Mary!~~Professor Oak: [NAME] has been spotted on [LOCATION].~Mary: [NAME]! How smart! How inspiring!").Replace("[NAME]", p.GetName(true)).Replace("[LOCATION]", Localization.GetString("Places_" + levelName, levelName));
                    break;
                }
                case "[unown]":
                {
                    String[] words = ["doom", "dark", "help", "join us", "stay", "lost", "vanish", "always there", "no eyes"];
                    output = String.Empty;
                    for (int x = 0; x <= 50; x++)
                    {
                        if (output != String.Empty) output += "~";
                        if (Core.Random.Next(0, 3) == 0)
                        {
                            String wordString = words[Core.Random.Next(0, words.Length)];
                            int wordIndex = 0;
                            for (int w = 0; w <= words.Length - 1; w++) { if (words[w] == wordString) { wordIndex = w + 1; break; } }
                            output += Localization.GetString("pokegear_screen_radio_UnownMessage" + wordIndex, wordString);
                        }
                        else output += "... ... ...";
                    }
                    break;
                }
                case "[luckychannel]":
                    output = Localization.GetString("pokegear_screen_radio_LuckyChannel_Content", "We are not broadcasting at the moment.~We will ensure that we can provide our service to you again as soon as possible.");
                    break;
                case "[placesandpeople]":
                {
                    String[] phrases = ["is actually great.", "is always happy.", "is cute.", "is definitely odd!", "is inspiring!", "is just my type.", "is just so-so.", "is kind of weird.", "is precious.", "is quite noisy.", "is right for me?", "is so cool, no?", "is sort of OK.", "is sort of lazy.", "is somewhat bold.", "is too picky!"];
                    String[] people = ["Youngster Joey", "Youngster Mike", "Bug Catcher Don", "School Kid Danny", "Ace Trainer Quinn", "Bug Catcher Rob", "Bug Catcher Doug", "Bug Catcher Ed", "Youngster Warren", "Youngster Jimmy", "Firebreather Otis", "Firebreather Burt", "Picknicker Hope", "Bird Keeper Hank", "Picnicker Sharon", "Poké Fan Rex", "Poké Fan Allan", "Biker Dwayne", "Biker Harris", "Biker Zeke", "Super Nerd Sam", "Super Nerd Tom", "Picnicker Edna", "Camper Sid", "Camper Dean", "Hiker Tim", "Picnicker Heidi", "Hiker Sidney", "Poké Fan Robert", "Hiker Jim", "Psychic Fidel", "Youngster Jason", "Youngster Owen", "Psychic Herman", "Fisherman Kile", "Fisherman Martin", "Fisherman Stephen", "Fisherman Barney"];
                    output = Localization.GetString("pokegear_screen_radio_PlacesAndPeople_Intro", "This is Places and People~with Lily.~Let's have a look at some~interesting people today.");
                    for (int x = 0; x <= 10; x++)
                    {
                        String phraseString = phrases[Core.Random.Next(0, phrases.Length)];
                        int phraseIndex = 0;
                        for (int ph = 0; ph <= phrases.Length - 1; ph++) { if (phrases[ph] == phraseString) { phraseIndex = ph + 1; break; } }
                        String person = people[Core.Random.Next(0, people.Length)];
                        String trainerClass = Localization.GetString("TrainerClass_" + person.Remove(person.LastIndexOf(" ")).Replace(" ", "").Replace("♀", "_Female").Replace("♂", "_Male"), person.Remove(person.LastIndexOf(" ")));
                        String personName = person.Remove(0, person.LastIndexOf(" ") + 1);
                        output += "~" + trainerClass + " " + personName + " " + Localization.GetString("pokegear_screen_radio_PlacesAndPeople_Phrase" + phraseIndex, phraseString);
                    }
                    output += Localization.GetString("pokegear_screen_radio_PlacesAndPeople_Outro", "This is it for now.~Tune in next time.");
                    break;
                }
                default:
                    output = ScriptVersion2.ScriptCommander.Parse(Content)?.ToString() ?? String.Empty;
                    break;
            }

            List<String> outputList = [];
            foreach (String e in output.Split('~'))
                outputList.AddRange(e.CropStringToWidth(FontManager.InGameFont, 480).SplitAtNewline());
            outputList.Insert(0, String.Empty);
            return outputList;
        }
    }

    private List<RadioStation> _radioStations = [];
    private bool _initializedRadio = false;
    private decimal _radioCursor = 0M;
    private String _currentSong = String.Empty;
    private RadioStation? _currentStation = null;
    private List<String> _broadcastLines = [];
    private float _lineDelay = 0.0F;

    private void DrawRadio()
    {
        Vector2 startPos = GetStartPosition();
        String freq = _radioCursor % 1 == 0 ? ((int)_radioCursor).ToString() : _radioCursor.ToString();
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_radio_Tuning", "Tuning:") + " " + freq, new Vector2(startPos.X + 80, startPos.Y + 50), Color.Black);

        Canvas.DrawRectangle(new Rectangle((int)(startPos.X + 168), (int)(startPos.Y + 110), 420, 5), Color.Black);
        for (int i = 0; i <= 21; i++)
            Canvas.DrawRectangle(new Rectangle((int)(startPos.X + 168 + i * 20), (int)(startPos.Y + 108), 2, 9), Color.Black);

        int cursorPos = (int)(_radioCursor * 20);
        Canvas.DrawRectangle(new Rectangle((int)(startPos.X + 166 + cursorPos), (int)(startPos.Y + 105), 8, 15), Color.White);

        String text1 = _currentStation == null ? Localization.GetString("pokegear_screen_radio_NoChannelsFound", "...No channels found...") : Localization.GetString("pokegear_screen_radio_YouAreListeningTo", "You are listening to:") + Environment.NewLine + _currentStation.Name;
        Core.SpriteBatch.DrawString(FontManager.MainFont, text1, new Vector2(startPos.X + 80, startPos.Y + 152), Color.Black);

        String text2 = String.Empty;
        if (Screen.Level!.IsRadioOn == true)
            text2 = Localization.GetString("pokegear_screen_radio_BackgroundStation", "Background station:") + Environment.NewLine + Screen.Level.SelectedRadioStation!.Name + "." + Environment.NewLine + Localization.GetString("pokegear_screen_radio_BackgroundStation_StopListening", "Press Accept to remove.").Replace("~", Environment.NewLine).Replace("*", Environment.NewLine);
        else if (_currentStation != null)
            text2 = Localization.GetString("pokegear_screen_radio_BackgroundStation_StartListening", "Press Accept to listen~to this station in~the background.").Replace("~", Environment.NewLine).Replace("*", Environment.NewLine);

        if (text2 != String.Empty)
            Core.SpriteBatch.DrawString(FontManager.MainFont, text2, new Vector2(startPos.X + 80 + (int)(FontManager.MainFont.MeasureString(text1).X) + 64, startPos.Y + 152), Color.Black);

        Texture2D canvasTexture = TextureManager.GetTexture("GUI\\Menus\\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)startPos.X + 84, (int)startPos.Y + 260, 96 * 6, 96));

        if (_broadcastLines.Count > 0)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _broadcastLines[0], new Vector2((int)startPos.X + 84 + 96 * 6 / 2 - (int)(FontManager.InGameFont.MeasureString(_broadcastLines[0]).X / 2), (int)startPos.Y + 286), Color.Black);
            if (_broadcastLines.Count > 1)
                Core.SpriteBatch.DrawString(FontManager.InGameFont, _broadcastLines[1], new Vector2((int)startPos.X + 84 + 96 * 6 / 2 - (int)(FontManager.InGameFont.MeasureString(_broadcastLines[1]).X / 2), (int)startPos.Y + 320), Color.Black);
        }
    }

    private void UpdateRadio()
    {
        if (_initializedRadio == false) { InitializeRadio(); return; }

        decimal iniCursor = _radioCursor;
        if (Controls.Right(true, true) == true) { _radioCursor += 0.5M; if (Controls.ShiftDown() == true) _radioCursor += 1.5M; }
        if (Controls.Left(true, true) == true) { _radioCursor -= 0.5M; if (Controls.ShiftDown() == true) _radioCursor -= 1.5M; }
        _radioCursor = Math.Clamp(_radioCursor, 0M, 21M);

        if (iniCursor != _radioCursor) { _broadcastLines.Clear(); CheckForStation(); }

        if (Controls.Accept(true, true, true) == true)
        {
            if (_currentSong == String.Empty)
            {
                Screen.Level!.IsRadioOn = false;
                Screen.Level.SelectedRadioStation = null;
            }
            else
            {
                if (Screen.Level!.IsRadioOn == true && GetSelectedStation()?.IsInterfering(Screen.Level.SelectedRadioStation!.ChannelMin) == true)
                {
                    Screen.Level.IsRadioOn = false;
                    Screen.Level.SelectedRadioStation = null;
                }
                else
                {
                    Screen.Level.IsRadioOn = !Screen.Level.IsRadioOn;
                    Screen.Level.SelectedRadioStation = _currentStation;
                    Player.Temp.RadioStation = _radioCursor;
                }
            }
        }

        if (_currentStation != null)
        {
            if (_broadcastLines.Count == 0) { _broadcastLines = _currentStation.GenerateText(); _lineDelay = 13.0F; }
            else if (_lineDelay > 0.0F) { _lineDelay -= 0.1F; if (_lineDelay <= 0.0F) { _lineDelay = 13.0F; _broadcastLines.RemoveAt(0); } }
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            if (_currentStation == null)
            {
                if (Screen.Level!.Surfing == true) MusicManager.Play("surf", true);
                else if (Screen.Level.Riding == true) MusicManager.Play("ride", true);
                else MusicManager.Play(Level.MusicLoop, true);
            }
            SoundManager.PlaySound("select");
            menuIndex = MenuScreens.Main;
        }
    }

    private RadioStation? GetSelectedStation()
    {
        foreach (RadioStation station in _radioStations)
            if (station.IsInterfering(_radioCursor) == true) return station;
        return null;
    }

    private void CheckForStation()
    {
        _currentSong = String.Empty;
        foreach (RadioStation station in _radioStations)
        {
            if (station.IsInterfering(_radioCursor) == false) continue;
            MusicManager.Play(station.Music, true);
            _currentSong = station.Music;
            _currentStation = station;
            return;
        }
        MusicManager.PlayNoMusic();
        _currentStation = null;
    }

    private void InitializeRadio()
    {
        _initializedRadio = true;
        String[] radioData = System.IO.File.ReadAllLines(GameModeManager.GetContentFilePath("Data\\channels.dat"));
        foreach (String line in radioData)
        {
            if (line.StartsWith("{") == false || line.EndsWith("}") == false) continue;
            String trimmed = line.Remove(line.Length - 1, 1).Remove(0, 1);
            RadioStation r = new RadioStation(trimmed);
            _radioStations = r.OverwriteChannels(_radioStations);
        }
        _radioCursor = Player.Temp.RadioStation;
        CheckForStation();
        Logger.Debug("Initialized Radio with " + _radioStations.Count + " stations.");
    }

    public static bool StationCanPlay(RadioStation? station)
    {
        if (station == null) return false;
        String file = GameModeManager.GetContentFilePath("Data\\channels.dat");
        Security.FileValidation.CheckFileValid(file, false, "PokegearScreen.cs");
        List<RadioStation> stations = [];
        foreach (String line in System.IO.File.ReadAllLines(file))
        {
            if (line.StartsWith("{") == false || line.EndsWith("}") == false) continue;
            String trimmed = line.Remove(line.Length - 1, 1).Remove(0, 1);
            stations = new RadioStation(trimmed).OverwriteChannels(stations);
        }
        foreach (RadioStation s in stations)
            if (s.ChannelMin == station.ChannelMin && s.ChannelMax == station.ChannelMax && s.Name == station.Name) return true;
        return false;
    }

    // ---- Trade Request ----

    private int _tradeRequestNetworkID = 0;
    private Texture2D? _tradeRequestTexture = null;
    private String? _tradeRequestName = null;
    private String _tradeRequestGameJoltID = String.Empty;
    private int _tradeRequestCursor = 0;

    private void DrawTradeRequest()
    {
        Vector2 startPos = GetStartPosition();
        Texture2D pgt = TextureManager.GetTexture("GUI\\Menus\\pokegear");

        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 46), 16, 32), new Rectangle(96, 112, 8, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 56), (int)(startPos.Y + 46), 192, 32), new Rectangle(102, 112, 4, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 248), (int)(startPos.Y + 46), 16, 32), new Rectangle(104, 112, 8, 16), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_pss_user_function_Trade", "Trade"), new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 48)), Color.Black);

        if (_tradeRequestTexture == null) return;
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_pss_TradeRequest", "The player \"[NAME]\" wants to trade with you.").Replace("[NAME]", _tradeRequestName), new Vector2((int)(startPos.X + 124), (int)(startPos.Y + 80)), Color.Black);

        Size fS = new Size(_tradeRequestTexture.Width / 3, _tradeRequestTexture.Height / 4);
        float fScale = 1.0F;
        if (_tradeRequestTexture.Width == _tradeRequestTexture.Height / 2) fS.Width = _tradeRequestTexture.Width / 2;
        else if (_tradeRequestTexture.Width == _tradeRequestTexture.Height) fS.Width = _tradeRequestTexture.Width / 4;
        if (fS.Width > 32) fScale = 0.5F;
        Core.SpriteBatch.Draw(_tradeRequestTexture, new Rectangle((int)(startPos.X + 104 - fS.Width * fScale / 2), (int)(startPos.Y + 96 - fS.Height * fScale / 2), (int)(fS.Width * fScale), (int)(fS.Height * fScale)), new Rectangle(0, fS.Height * 2, fS.Width, fS.Height), Color.White);

        for (int i = 0; i <= 1; i++)
        {
            SpriteEffects eff = i == _tradeRequestCursor ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 80), (int)(startPos.Y + 152 + i * 64), 16, 32), new Rectangle(96, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 96), (int)(startPos.Y + 152 + i * 64), 192, 32), new Rectangle(102, 112, 4, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 288), (int)(startPos.Y + 152 + i * 64), 16, 32), new Rectangle(104, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
            String label = i == 0 ? Localization.GetString("global_yes", "Yes") : Localization.GetString("global_no", "No");
            Core.SpriteBatch.DrawString(FontManager.MainFont, label, new Vector2((int)(startPos.X + 88), (int)(startPos.Y + 155 + i * 64)), Color.Black);
        }
    }

    private void UpdateTradeRequest()
    {
        Vector2 startPos = GetStartPosition();
        bool playerExists = false;
        foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
            if (p.ServersID == _tradeRequestNetworkID) { playerExists = true; break; }

        if (playerExists == false) { CloseTradeRequest(); return; }

        if (_tradeRequestTexture == null)
        {
            foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
            {
                if (p.ServersID != _tradeRequestNetworkID) continue;
                String tPath = NetworkPlayer.GetTexturePath(p.Skin);
                _tradeRequestTexture = TextureManager.TextureExist(tPath) == true ? TextureManager.GetTexture(tPath) : TextureManager.GetTexture("Textures\\NPC\\0");
                _tradeRequestName = p.Name;
                Core.StartThreadedSub(_ => DownloadTradeRequestSprite());
            }
            return;
        }

        if (Controls.Down(true, true) == true) _tradeRequestCursor += 1;
        if (Controls.Up(true, true) == true) _tradeRequestCursor -= 1;
        _tradeRequestCursor = Math.Clamp(_tradeRequestCursor, 0, 1);

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (new Rectangle((int)(startPos.X + 80), (int)(startPos.Y + 152 + i * 64), 224, 32).Contains(MouseHandler.MousePosition) == false) continue;
                if (i == _tradeRequestCursor)
                {
                    SoundManager.PlaySound("select");
                    if (i == 0) { CloseTradeRequest(); Core.SetScreen(new DirectTradeScreen(Core.CurrentScreen, _tradeRequestNetworkID, false)); }
                    else CloseTradeRequest();
                }
                else _tradeRequestCursor = i;
            }
        }

        if (Controls.Accept(false, true, true) == true)
        {
            SoundManager.PlaySound("select");
            if (_tradeRequestCursor == 0) { CloseTradeRequest(); Core.SetScreen(new DirectTradeScreen(Core.CurrentScreen, _tradeRequestNetworkID, false)); }
            else CloseTradeRequest();
        }
        if (Controls.Dismiss() == true) { SoundManager.PlaySound("select"); CloseTradeRequest(); }
    }

    private void DownloadTradeRequestSprite()
    {
        if (_tradeRequestGameJoltID != String.Empty)
        {
            Texture2D? tex = Emblem.GetOnlineSprite(_tradeRequestGameJoltID);
            if (tex != null) _tradeRequestTexture = tex;
        }
    }

    private void CloseTradeRequest() => Core.SetScreen(PreScreen!);

    // ---- Battle Request ----

    private int _battleRequestNetworkID = 0;
    private Texture2D? _battleRequestTexture = null;
    private String? _battleRequestName = null;
    private String _battleRequestGameJoltID = String.Empty;
    private int _battleRequestCursor = 0;

    private void DrawBattleRequest()
    {
        Vector2 startPos = GetStartPosition();
        Texture2D pgt = TextureManager.GetTexture("GUI\\Menus\\pokegear");

        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 40), (int)(startPos.Y + 46), 16, 32), new Rectangle(96, 112, 8, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 56), (int)(startPos.Y + 46), 192, 32), new Rectangle(102, 112, 4, 16), Color.White);
        Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 248), (int)(startPos.Y + 46), 16, 32), new Rectangle(104, 112, 8, 16), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, "Battle", new Vector2((int)(startPos.X + 48), (int)(startPos.Y + 48)), Color.Black);

        if (_battleRequestTexture == null) return;
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_pss_BattleRequest", "The player \"[NAME]\" wants to battle with you.").Replace("[NAME]", _battleRequestName), new Vector2((int)(startPos.X + 84), (int)(startPos.Y + 80)), Color.Black);

        Size fS = new Size(_battleRequestTexture.Width / 3, _battleRequestTexture.Height / 4);
        float fScale = 1.0F;
        if (_battleRequestTexture.Width == _battleRequestTexture.Height / 2) fS.Width = _battleRequestTexture.Width / 2;
        else if (_battleRequestTexture.Width == _battleRequestTexture.Height) fS.Width = _battleRequestTexture.Width / 4;
        if (fS.Width > 32) fScale = 0.5F;
        Core.SpriteBatch.Draw(_battleRequestTexture, new Rectangle((int)(startPos.X + 64 - fS.Width * fScale / 2), (int)(startPos.Y + 96 - fS.Height * fScale / 2), (int)(fS.Width * fScale), (int)(fS.Height * fScale)), new Rectangle(0, fS.Height * 2, fS.Width, fS.Height), Color.White);

        for (int i = 0; i <= 1; i++)
        {
            SpriteEffects eff = i == _battleRequestCursor ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 80), (int)(startPos.Y + 152 + i * 64), 16, 32), new Rectangle(96, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 96), (int)(startPos.Y + 152 + i * 64), 192, 32), new Rectangle(102, 112, 4, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
            Core.SpriteBatch.Draw(pgt, new Rectangle((int)(startPos.X + 288), (int)(startPos.Y + 152 + i * 64), 16, 32), new Rectangle(104, 112, 8, 16), Color.White, 0.0F, Vector2.Zero, eff, 0.0F);
            String label = i == 0 ? Localization.GetString("global_yes", "Yes") : Localization.GetString("global_no", "No");
            Core.SpriteBatch.DrawString(FontManager.MainFont, label, new Vector2((int)(startPos.X + 88), (int)(startPos.Y + 155 + i * 64)), Color.Black);
        }
    }

    private void UpdateBattleRequest()
    {
        Vector2 startPos = GetStartPosition();
        bool playerExists = false;
        foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
            if (p.ServersID == _battleRequestNetworkID) { playerExists = true; break; }

        if (playerExists == false) { CloseBattleRequest(); return; }

        if (_battleRequestTexture == null)
        {
            foreach (Servers.Player p in Core.ServersManager.PlayerCollection)
            {
                if (p.ServersID != _battleRequestNetworkID) continue;
                String tPath = NetworkPlayer.GetTexturePath(p.Skin);
                _battleRequestTexture = TextureManager.TextureExist(tPath) == true ? TextureManager.GetTexture(tPath) : TextureManager.GetTexture("Textures\\NPC\\0");
                _battleRequestName = p.Name;
                Core.StartThreadedSub(_ => DownloadBattleRequestSprite());
            }
            return;
        }

        if (Controls.Down(true, true) == true) _battleRequestCursor += 1;
        if (Controls.Up(true, true) == true) _battleRequestCursor -= 1;
        _battleRequestCursor = Math.Clamp(_battleRequestCursor, 0, 1);

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (new Rectangle((int)(startPos.X + 80), (int)(startPos.Y + 152 + i * 64), 224, 32).Contains(MouseHandler.MousePosition) == false) continue;
                if (i == _battleRequestCursor)
                {
                    SoundManager.PlaySound("select");
                    if (i == 0) { CloseBattleRequest(); Core.SetScreen(new PVPLobbyScreen(Core.CurrentScreen, _battleRequestNetworkID, false)); }
                    else CloseBattleRequest();
                }
                else _battleRequestCursor = i;
            }
        }

        if (Controls.Accept(false, true, true) == true)
        {
            SoundManager.PlaySound("select");
            if (_battleRequestCursor == 0) { CloseBattleRequest(); Core.SetScreen(new PVPLobbyScreen(Core.CurrentScreen, _battleRequestNetworkID, false)); }
            else CloseBattleRequest();
        }
        if (Controls.Dismiss() == true) { SoundManager.PlaySound("select"); CloseBattleRequest(); }
    }

    private void DownloadBattleRequestSprite()
    {
        if (_battleRequestGameJoltID != String.Empty)
        {
            Texture2D? tex = Emblem.GetOnlineSprite(_battleRequestGameJoltID);
            if (tex != null) _battleRequestTexture = tex;
        }
    }

    private void CloseBattleRequest() => Core.SetScreen(PreScreen!);

    private Vector2 GetStartPosition() =>
        new Vector2((int)(Core.windowSize.Width / 2 - _width / 2), (int)(Core.windowSize.Height - _height));
}
