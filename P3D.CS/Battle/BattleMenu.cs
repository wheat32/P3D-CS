using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;
using P3D.Items;

namespace P3D.BattleSystem;

public class BattleMenu
{
    // -----------------------------------------------------------------------
    // Enums / delegates
    // -----------------------------------------------------------------------

    public enum MenuStates
    {
        Main,
        Moves,
    }

    public delegate void D_MainMenuClick(BattleScreen battleScreen);
    public delegate void D_MoveMenuClick(BattleScreen battleScreen);

    // -----------------------------------------------------------------------
    // Public state
    // -----------------------------------------------------------------------

    public bool CanInteract = true;
    public MenuStates MenuState = MenuStates.Main;
    public bool Visible = false;

    // -----------------------------------------------------------------------
    // Private state
    // -----------------------------------------------------------------------

    private static String _battleInterfaceTexture = @"GUI\Battle\Interface";

    private int _allItemsExtended = 0;
    private int _selectedItemExtended = 0;
    private MenuStates _nextMenuState = MenuStates.Main;
    private bool _retractMenu = false;
    private bool _isRetracting = false;
    private bool _isExtracting = true;

    private int _moveMenuIndex = 0;
    private int _moveMenuNextIndex = 0;
    private int _moveMenuLastIndex = 0;
    private List<MoveMenuItem> _moveMenuItemList = [];
    private String _moveMenuCreatedID = String.Empty;
    private int _moveMenuAlpha = 255;
    private bool _moveMenuChoseMove = false;

    private int _mainMenuIndex = 0;
    private int _mainMenuNextIndex = 0;
    private List<MainMenuItem> _mainMenuItemList = [];
    private int _mainMenuTeamPreviewAlpha = 0;
    private int _mainMenuTeamPreviewLastIndex = -1;

    // -----------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------

    public BattleMenu()
    {
        Reset();
        if (TextureManager.TextureExist(@"GUI\Battle\Interface_" + Localization.LanguageSuffix) == true)
        {
            _battleInterfaceTexture = @"GUI\Battle\Interface_" + Localization.LanguageSuffix;
        }
        else
        {
            _battleInterfaceTexture = @"GUI\Battle\Interface";
        }

        Screen.Identifications[] blockInteractScreen =
        {
            Screen.Identifications.PartyScreen,
            Screen.Identifications.SummaryScreen,
            Screen.Identifications.PauseScreen,
            Screen.Identifications.ChatScreen,
        };
        if (blockInteractScreen.Contains(Core.CurrentScreen.Identification) == true)
        {
            CanInteract = false;
        }
        else
        {
            CanInteract = true;
        }
    }

    // -----------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------

    public void Reset()
    {
        _moveMenuAlpha = 255;
        _moveMenuChoseMove = false;
        MenuState = MenuStates.Main;
    }

    public void Draw(BattleScreen battleScreen)
    {
        if (battleScreen.IsCurrentScreen() == true)
        {
            DrawPokemonStats(new Vector2(50, 50), battleScreen.OpponentPokemon!, battleScreen, false, (battleScreen.IsTrainerBattle == false));

            if (battleScreen.BattleMode != BattleScreen.BattleModes.Safari)
            {
                DrawPokemonStats(new Vector2(Core.ScreenSize.Width - 280, Core.ScreenSize.Height - 100), battleScreen.SelfPokemon!, battleScreen, true, false);
            }

            DrawPokeBalls(new Vector2(Core.ScreenSize.Width - 292, Core.ScreenSize.Height - 112), battleScreen, Core.Player.Pokemons, false);
            if (battleScreen.IsTrainerBattle == true)
            {
                DrawPokeBalls(new Vector2(38, 38), battleScreen, battleScreen.Trainer!.Pokemons, true);
            }

            switch (MenuState)
            {
                case MenuStates.Main:
                    DrawMainMenu(battleScreen);
                    break;
                case MenuStates.Moves:
                    DrawMoveMenu(battleScreen);
                    break;
            }

            DrawWeather(battleScreen);
            DrawTerrain(battleScreen);
        }
    }

    public void Update(ref BattleScreen battleScreen)
    {
        Screen.Identifications[] blockInteractScreen =
        {
            Screen.Identifications.PartyScreen,
            Screen.Identifications.SummaryScreen,
            Screen.Identifications.PauseScreen,
            Screen.Identifications.ChatScreen,
        };
        if (blockInteractScreen.Contains(Core.CurrentScreen.Identification) == true ||
            (battleScreen.BattleQuery.Count > 0 && battleScreen.BattleQuery[0].QueryType == QueryObject.QueryTypes.Textbox))
        {
            CanInteract = false;
        }
        else
        {
            CanInteract = true;
        }
        if (CanInteract == true)
        {
            switch (MenuState)
            {
                case MenuStates.Main:
                    UpdateMainMenu(ref battleScreen);
                    break;
                case MenuStates.Moves:
                    UpdateMoveMenu(battleScreen);
                    break;
            }
        }
    }

    // -----------------------------------------------------------------------
    // Drawing helpers
    // -----------------------------------------------------------------------

    private void DrawWeather(BattleScreen battleScreen)
    {
        int y = -1;
        int x = 0;
        String t = String.Empty;

        switch (battleScreen.FieldEffects.Weather)
        {
            case BattleWeather.WeatherTypes.Sunny:
                y = 0;
                t = "Sunny";
                break;
            case BattleWeather.WeatherTypes.Rain:
                y = 34;
                t = "Rain";
                break;
            case BattleWeather.WeatherTypes.Sandstorm:
                y = 68;
                t = "Sandstorm";
                break;
            case BattleWeather.WeatherTypes.Hailstorm:
                y = 102;
                t = "Hailstorm";
                break;
            case BattleWeather.WeatherTypes.Foggy:
                x = 88;
                y = 0;
                t = "Foggy";
                break;
            case BattleWeather.WeatherTypes.Snow:
                x = 88;
                y = 34;
                t = "Snow";
                break;
        }

        if (y > -1)
        {
            Core.SpriteBatch.DrawInterface(
                TextureManager.GetTexture(@"GUI\Battle\WeatherIcons"),
                new Rectangle(22, Core.ScreenSize.Height - 90, 176, 68),
                new Rectangle(x, y, 88, 34),
                Color.White);
            Core.SpriteBatch.DrawInterfaceString(
                FontManager.MainFont,
                t,
                new Vector2(110 - FontManager.MainFont.MeasureString(t).X / 2, Core.ScreenSize.Height - 44),
                Color.Black);
        }
    }

    private void DrawTerrain(BattleScreen battleScreen)
    {
        int y = -1;
        int x = 0;
        String t = String.Empty;

        FieldEffects fe = battleScreen.FieldEffects;
        if (fe.ElectricTerrain > 0)
        {
            x = 352;
            y = 0;
            t = "Electric Terrain";
        }
        if (fe.GrassyTerrain > 0)
        {
            x = 352;
            y = 34;
            t = "Grassy Terrain";
        }
        if (fe.MistyTerrain > 0)
        {
            x = 352;
            y = 68;
            t = "Misty Terrain";
        }
        if (fe.PsychicTerrain > 0)
        {
            x = 352;
            y = 102;
            t = "Psychic Terrain";
        }

        if (y > -1)
        {
            Core.SpriteBatch.DrawInterface(
                TextureManager.GetTexture(@"GUI\Battle\WeatherIcons"),
                new Rectangle(222, Core.ScreenSize.Height - 90, 176, 68),
                new Rectangle(x, y, 88, 34),
                Color.White);
            Core.SpriteBatch.DrawInterfaceString(
                FontManager.MainFont,
                t,
                new Vector2(310 - FontManager.MainFont.MeasureString(t).X / 2, Core.ScreenSize.Height - 44),
                Color.Black);
        }
    }

    private void DrawPokemonStats(Vector2 pos, Pokemon p, BattleScreen battleScreen, bool largeStatsDisplay, bool drawCaught)
    {
        Color shinyHue = Color.White;

        if (p.IsShiny == true)
        {
            shinyHue = Color.Gold;
        }
        if (_moveMenuChoseMove == true)
        {
            shinyHue.A = (byte)_moveMenuAlpha.Clamp(0, 255);
        }

        if (largeStatsDisplay == true)
        {
            // Background
            Core.SpriteBatch.DrawInterface(
                TextureManager.GetTexture(_battleInterfaceTexture),
                new Rectangle((int)pos.X + 14, (int)pos.Y + 14, 182, 42),
                new Rectangle(0, 0, 91, 21),
                shinyHue);

            // Name
            String nameInformation = p.GetDisplayName() + " " + Localization.GetString("property_Lv.", "Lv.") + " " + p.Level.ToString();

            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, nameInformation, new Vector2(pos.X + 2, pos.Y + 2), new Color(0, 0, 0, _moveMenuAlpha));
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, nameInformation, new Vector2(pos.X, pos.Y), shinyHue);

            // Gender
            if (p.Gender == Pokemon.Genders.Male)
            {
                Core.SpriteBatch.DrawInterface(
                    TextureManager.GetTexture(_battleInterfaceTexture),
                    new Rectangle((int)(pos.X + 6 + FontManager.MainFont.MeasureString(nameInformation).X), (int)pos.Y, 12, 20),
                    new Rectangle(0, 104, 6, 10),
                    new Color(255, 255, 255, _moveMenuAlpha));
            }
            else if (p.Gender == Pokemon.Genders.Female)
            {
                Core.SpriteBatch.DrawInterface(
                    TextureManager.GetTexture(_battleInterfaceTexture),
                    new Rectangle((int)(pos.X + 6 + FontManager.MainFont.MeasureString(nameInformation).X), (int)pos.Y, 12, 20),
                    new Rectangle(6, 104, 6, 10),
                    new Color(255, 255, 255, _moveMenuAlpha));
            }

            // HP indicator
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, p.HP + "/" + p.MaxHP, new Vector2(pos.X + 102, pos.Y + 37 + 3), new Color(0, 0, 0, _moveMenuAlpha));
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, p.HP + "/" + p.MaxHP, new Vector2(pos.X + 100, pos.Y + 35 + 3), shinyHue);

            // EXP bar
            if (BattleScreen.CanReceiveEXP == true)
            {
                int nextLvExp = p.NeedExperience(p.Level + 1) - p.NeedExperience(p.Level);
                int currentExp = p.Experience - p.NeedExperience(p.Level);
                if (p.Level == 1)
                {
                    nextLvExp = p.NeedExperience(p.Level + 1);
                    currentExp = p.Experience;
                }

                int needExp = nextLvExp - currentExp;

                if (p.Level == int.Parse(GameModeManager.GetGameRuleValue("MaxLevel", "100")))
                {
                    nextLvExp = 0;
                }
                else
                {
                    int barPercentage = (int)((currentExp / (double)nextLvExp) * 100);
                    if (barPercentage > 0)
                    {
                        int expLength = (int)Math.Ceiling(144 / 100.0 * barPercentage);

                        if (currentExp == 0)
                        {
                            expLength = 0;
                        }
                        else
                        {
                            if (expLength <= 0)
                            {
                                expLength = 1;
                            }
                        }
                        if (expLength == 144)
                        {
                            expLength = 143;
                        }

                        Texture2D t2 = TextureManager.GetTexture(_battleInterfaceTexture);
                        for (int dX = 0; dX <= expLength; dX += 4)
                        {
                            Core.SpriteBatch.DrawInterface(t2, new Rectangle((int)pos.X + 50 + dX, (int)pos.Y + 64, 4, 6), new Rectangle(0, 43, 2, 3), new Color(255, 255, 255, _moveMenuAlpha));
                        }
                    }
                }
            }
        }
        else
        {
            // Smaller stats display
            // Background
            Core.SpriteBatch.DrawInterface(
                TextureManager.GetTexture(_battleInterfaceTexture),
                new Rectangle((int)pos.X + 14, (int)pos.Y + 14, 182, 32),
                new Rectangle(0, 21, 91, 16),
                shinyHue);

            // Name
            String nameInformation = p.GetDisplayName() + " " + Localization.GetString("property_Lv.", "Lv.") + " " + p.Level.ToString();

            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, nameInformation, new Vector2(pos.X + 2, pos.Y + 2), new Color(0, 0, 0, _moveMenuAlpha));
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, nameInformation, new Vector2(pos.X, pos.Y), shinyHue);

            // Gender
            if (p.Gender == Pokemon.Genders.Male)
            {
                Core.SpriteBatch.DrawInterface(
                    TextureManager.GetTexture(_battleInterfaceTexture),
                    new Rectangle((int)(pos.X + 6 + FontManager.MainFont.MeasureString(nameInformation).X), (int)pos.Y, 12, 20),
                    new Rectangle(0, 104, 6, 10),
                    new Color(255, 255, 255, _moveMenuAlpha));
            }
            else if (p.Gender == Pokemon.Genders.Female)
            {
                Core.SpriteBatch.DrawInterface(
                    TextureManager.GetTexture(_battleInterfaceTexture),
                    new Rectangle((int)(pos.X + 6 + FontManager.MainFont.MeasureString(nameInformation).X), (int)pos.Y, 12, 20),
                    new Rectangle(6, 104, 6, 10),
                    new Color(255, 255, 255, _moveMenuAlpha));
            }
        }

        float hpPercentage = (100.0f / p.MaxHP) * p.HP;
        int hpLength = (int)Math.Ceiling(140 / 100.0 * hpPercentage.Clamp(1, 999));

        if (p.HP == 0)
        {
            hpLength = 0;
        }
        else
        {
            if (hpLength <= 0)
            {
                hpLength = 1;
            }
        }
        if (p.HP == p.MaxHP)
        {
            hpLength = 140;
        }
        else
        {
            if (hpLength == 140)
            {
                hpLength = 139;
            }
        }

        int cX = 0;
        if (hpPercentage <= 75.0f && hpPercentage > 25.0f)
        {
            cX = 2;
        }
        else if (hpPercentage <= 25.0f)
        {
            cX = 4;
        }

        if (hpLength > 0)
        {
            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(_battleInterfaceTexture), new Rectangle((int)pos.X + 54, (int)pos.Y + 26, 2, 12), new Rectangle(cX, 37, 1, 6), new Color(255, 255, 255, _moveMenuAlpha));
            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(_battleInterfaceTexture), new Rectangle((int)pos.X + 2 + 54, (int)pos.Y + 26, hpLength - 4, 12), new Rectangle(cX + 1, 37, 1, 6), new Color(255, 255, 255, _moveMenuAlpha));
            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(_battleInterfaceTexture), new Rectangle((int)pos.X + hpLength - 2 + 54, (int)pos.Y + 26, 2, 12), new Rectangle(cX, 37, 1, 6), new Color(255, 255, 255, _moveMenuAlpha));
        }

        int caughtX = 0;
        Texture2D? statusTexture = BattleStats.GetStatImage(p.Status);
        if (statusTexture != null)
        {
            Core.SpriteBatch.DrawInterface(statusTexture, new Rectangle((int)pos.X + 12, (int)pos.Y + 26, 38, 12), new Color(255, 255, 255, _moveMenuAlpha));
            caughtX = -16;
        }

        if (drawCaught == true)
        {
            String dexID = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);

            if (Pokedex.GetEntryType(Core.Player.PokedexData, dexID) > 1)
            {
                Core.SpriteBatch.DrawInterface(
                    TextureManager.GetTexture(_battleInterfaceTexture),
                    new Rectangle((int)pos.X + caughtX, (int)pos.Y + 22, 20, 20),
                    new Rectangle(0, 46, 10, 10),
                    new Color(255, 255, 255, _moveMenuAlpha));
            }
        }
    }

    private void DrawPokeBalls(Vector2 pos, BattleScreen battleScreen, List<Pokemon> pokemonList, bool mirrored)
    {
        if (mirrored == true)
        {
            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(_battleInterfaceTexture), new Rectangle((int)pos.X, (int)pos.Y, 160, 14), new Rectangle(128, 7, 80, 7), new Color(255, 255, 255, _moveMenuAlpha));
        }
        else
        {
            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(_battleInterfaceTexture), new Rectangle((int)pos.X, (int)pos.Y, 160, 14), new Rectangle(128, 0, 80, 7), new Color(255, 255, 255, _moveMenuAlpha));
        }

        bool mouseHovers = false;

        int startX = 12;
        if (mirrored == true)
        {
            startX = 76;
        }

        for (int i = 0; i <= 5; i++)
        {
            int texturePos = 0;

            if (pokemonList.Count - 1 >= i)
            {
                Pokemon p = pokemonList[i];
                if (p.Status == Pokemon.StatusProblems.Fainted)
                {
                    texturePos = 10;
                }
                else if (p.Status == Pokemon.StatusProblems.None)
                {
                    texturePos = 0;
                }
                else
                {
                    texturePos = 30;
                }

                if (MouseHandler.IsInRectangle(new Rectangle((int)pos.X + startX + 12 * i, (int)pos.Y + 1, 10, 10)) == true && mirrored == false && _moveMenuChoseMove == false)
                {
                    Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(_battleInterfaceTexture), new Rectangle((int)pos.X + startX + 12 * i - 27, (int)pos.Y - 86, 64, 84), new Rectangle(128, 16, 32, 42), new Color(255, 255, 255, _mainMenuTeamPreviewAlpha));
                    Core.SpriteBatch.DrawInterface(pokemonList[i].GetMenuTexture(true), new Rectangle((int)pos.X + startX + 12 * i - 27, (int)pos.Y - 86, 64, 64), new Color(255, 255, 255, _mainMenuTeamPreviewAlpha));

                    if (_mainMenuTeamPreviewAlpha < 255)
                    {
                        _mainMenuTeamPreviewAlpha += 25;
                        if (_mainMenuTeamPreviewAlpha > 255)
                        {
                            _mainMenuTeamPreviewAlpha = 255;
                        }
                    }

                    _mainMenuTeamPreviewLastIndex = i;
                    mouseHovers = true;
                }
            }
            else
            {
                texturePos = 20;
            }
            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(_battleInterfaceTexture), new Rectangle((int)pos.X + startX + 12 * i, (int)pos.Y + 1, 10, 10), new Rectangle(texturePos, 46, 10, 10), new Color(255, 255, 255, _moveMenuAlpha));
        }

        if (mouseHovers == false && mirrored == false)
        {
            if (_mainMenuTeamPreviewAlpha > 0)
            {
                _mainMenuTeamPreviewAlpha -= 25;
                if (_mainMenuTeamPreviewAlpha < 0)
                {
                    _mainMenuTeamPreviewAlpha = 0;
                }
                if (_mainMenuTeamPreviewLastIndex > -1)
                {
                    Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(_battleInterfaceTexture), new Rectangle((int)pos.X + startX + 12 * _mainMenuTeamPreviewLastIndex - 27, (int)pos.Y - 86, 64, 84), new Rectangle(128, 16, 32, 42), new Color(255, 255, 255, _mainMenuTeamPreviewAlpha));
                    Core.SpriteBatch.DrawInterface(pokemonList[_mainMenuTeamPreviewLastIndex].GetMenuTexture(true), new Rectangle((int)pos.X + startX + 12 * _mainMenuTeamPreviewLastIndex - 27, (int)pos.Y - 86, 64, 64), new Color(255, 255, 255, _mainMenuTeamPreviewAlpha));
                }
            }
        }
    }

    // -----------------------------------------------------------------------
    // Menu animation shared logic
    // -----------------------------------------------------------------------

    private void UpdateMenuOptions(ref int menuIndex, ref int nextMenuIndex, int itemListCount)
    {
        bool canSelect = true;

        if (_retractMenu == false)
        {
            if (_isRetracting == true)
            {
                if (_selectedItemExtended > 0)
                {
                    _selectedItemExtended -= 60;
                    if (_selectedItemExtended <= 0)
                    {
                        _selectedItemExtended = 0;
                        _isRetracting = false;
                        menuIndex = nextMenuIndex;
                        _isExtracting = true;
                    }
                }

                canSelect = false;
            }
            else if (_isExtracting == true)
            {
                if (_selectedItemExtended < 300)
                {
                    _selectedItemExtended += 60;
                    if (_selectedItemExtended >= 300)
                    {
                        _selectedItemExtended = 300;
                        _isExtracting = false;
                    }
                }

                canSelect = false;
            }
        }

        if (_retractMenu == true)
        {
            if (_allItemsExtended > 0)
            {
                _allItemsExtended -= 10;
                _selectedItemExtended -= 60;
                if (_allItemsExtended <= 0)
                {
                    _allItemsExtended = 0;
                    _selectedItemExtended = 0;
                    _retractMenu = false;
                    MenuState = _nextMenuState;
                    _isExtracting = true;
                }
            }

            canSelect = false;
        }
        else if (_allItemsExtended < 130)
        {
            _allItemsExtended += 10;
            canSelect = false;
        }

        if (canSelect == true)
        {
            if (Controls.Down(true) == true)
            {
                nextMenuIndex = menuIndex + 1;

                if (nextMenuIndex == itemListCount)
                {
                    nextMenuIndex = 0;
                }

                _isRetracting = true;
            }
            if (Controls.Up(true) == true)
            {
                nextMenuIndex = menuIndex - 1;

                if (nextMenuIndex == -1)
                {
                    nextMenuIndex = itemListCount - 1;
                }

                _isRetracting = true;
            }
        }
    }

    // -----------------------------------------------------------------------
    // Main menu
    // -----------------------------------------------------------------------

    private void DrawMainMenu(BattleScreen battleScreen)
    {
        foreach (MainMenuItem m in _mainMenuItemList)
        {
            m.Draw(_allItemsExtended, _selectedItemExtended, (m.Index == _mainMenuIndex));
        }
    }

    private void UpdateMainMenu(ref BattleScreen battleScreen)
    {
        if (battleScreen.ClearMainMenuTime == true)
        {
            _mainMenuItemList.Clear();
            battleScreen.ClearMainMenuTime = false;
        }
        if (_mainMenuItemList.Count == 0)
        {
            CreateMainMenuItems(ref battleScreen);
        }
        if (battleScreen.OwnFaint == true)
        {
            if (battleScreen.BattleQuery[0].QueryType != QueryObject.QueryTypes.ScreenFade)
            {
                _tempBattleScreen = battleScreen;

                Player.Temp.PokemonScreenIndex = battleScreen.SelfPokemonIndex;
                PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, Item.GetItemByID(5.ToString()), ShowPokemonMenu, Localization.GetString("party_screen_ChoosePokemon", "Choose Pokémon"), false)
                {
                    Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
                    CanExit = false,
                };
                selScreen.SelectedObject += ShowPokemonMenuHandler;

                Core.SetScreen(selScreen);
            }
        }
        if (_retractMenu == false)
        {
            foreach (MainMenuItem m in _mainMenuItemList)
            {
                m.Update(battleScreen, _allItemsExtended, (m.Index == _mainMenuIndex));
            }
        }

        UpdateMenuOptions(ref _mainMenuIndex, ref _mainMenuNextIndex, _mainMenuItemList.Count);
    }

    private void CreateMainMenuItems(ref BattleScreen battleScreen)
    {
        _mainMenuItemList.Clear();
        switch (battleScreen.BattleMode)
        {
            case BattleScreen.BattleModes.Safari:
            {
                String safariBallText = Localization.GetString("item_name_181", "Safari Ball") + " x" + Core.Player.Inventory.GetItemAmount(181.ToString()).ToString();
                if (Core.Player.Inventory.GetItemAmount(181.ToString()) == 0)
                {
                    safariBallText = Localization.GetString("battle_NoSafariBalls", "No Safari Balls.");
                }
                _mainMenuItemList.Add(new MainMenuItem(4, safariBallText, 0, MainMenuUseSafariBall));
                _mainMenuItemList.Add(new MainMenuItem(0, Localization.GetString("battle_action_ThrowMud", "Throw Mud"), 1, MainMenuThrowMud));
                _mainMenuItemList.Add(new MainMenuItem(0, Localization.GetString("battle_action_ThrowBait", "Throw Bait"), 2, MainMenuThrowBait));
                _mainMenuItemList.Add(new MainMenuItem(3, Localization.GetString("battle_action_Run", "Run"), 3, MainMenuRun));
                break;
            }
            case BattleScreen.BattleModes.BugContest:
            {
                _mainMenuItemList.Add(new MainMenuItem(0, Localization.GetString("battle_action_Battle", "Battle"), 0, MainMenuOpenBattleMenu));

                String sportBallText = Localization.GetString("item_name_177", "Sport Ball") + " x" + Core.Player.Inventory.GetItemAmount(177.ToString()).ToString();
                if (Core.Player.Inventory.GetItemAmount(177.ToString()) == 0)
                {
                    sportBallText = Localization.GetString("battle_NoSportBalls", "No Sport Balls.");
                }
                _mainMenuItemList.Add(new MainMenuItem(4, sportBallText, 1, MainMenuUseSportBall));
                _mainMenuItemList.Add(new MainMenuItem(1, "Pokémon", 2, MainMenuOpenPokemon));
                _mainMenuItemList.Add(new MainMenuItem(3, Localization.GetString("battle_action_Run", "Run"), 3, MainMenuRun));
                break;
            }
            case BattleScreen.BattleModes.Standard:
            {
                if (battleScreen.OwnFaint == true)
                {
                    _mainMenuIndex = 0;
                    _mainMenuNextIndex = 0;
                    if (battleScreen.IsRemoteBattle == true && battleScreen.IsHost == false)
                    {
                        _mainMenuItemList.Add(new MainMenuItem(1, "Pokémon", 0, MainMenuOpenPokemon));
                        battleScreen.OwnFaint = false;
                    }
                }
                else if (battleScreen.OppFaint == true && battleScreen.IsRemoteBattle == true)
                {
                    if (battleScreen.IsHost == true)
                    {
                        battleScreen.BattleQuery.Clear();
                        battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                        battleScreen.Battle.InitializeRound(battleScreen, new BattleRoundConst
                        {
                            StepType = BattleRoundConst.StepTypes.Text,
                            Argument = "The client sends the next pokemon!",
                        });
                    }
                    else
                    {
                        battleScreen.SelfStatistics.Switches += 1;
                        battleScreen.BattleQuery.Clear();
                        battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                        battleScreen.SendClientCommand("TEXT|" + "The host sends the next pokemon!");
                    }
                    battleScreen.OppFaint = false;
                }
                else
                {
                    _mainMenuItemList.Add(new MainMenuItem(0, Localization.GetString("battle_action_Battle", "Battle"), 0, MainMenuOpenBattleMenu));
                    _mainMenuItemList.Add(new MainMenuItem(1, "Pokémon", 1, MainMenuOpenPokemon));
                    if (BattleScreen.CanUseItems == true)
                    {
                        _mainMenuItemList.Add(new MainMenuItem(2, Localization.GetString("battle_action_Bag", "Bag"), 2, MainMenuOpenBag));

                        if (battleScreen.IsTrainerBattle == false)
                        {
                            _mainMenuItemList.Add(new MainMenuItem(3, Localization.GetString("battle_action_Run", "Run"), 3, MainMenuRun));
                            MainMenuAddMegaEvolution(battleScreen, 4);
                        }
                        else
                        {
                            MainMenuAddMegaEvolution(battleScreen, 3);
                        }
                    }
                    else
                    {
                        if (battleScreen.IsTrainerBattle == false)
                        {
                            _mainMenuItemList.Add(new MainMenuItem(3, Localization.GetString("battle_action_Run", "Run"), 2, MainMenuRun));
                            MainMenuAddMegaEvolution(battleScreen, 3);
                        }
                        else
                        {
                            MainMenuAddMegaEvolution(battleScreen, 2);
                        }
                    }
                }
                break;
            }
            case BattleScreen.BattleModes.PvP:
            {
                _mainMenuItemList.Add(new MainMenuItem(0, Localization.GetString("battle_action_Battle", "Battle"), 0, MainMenuOpenBattleMenu));
                _mainMenuItemList.Add(new MainMenuItem(1, "Pokémon", 1, MainMenuOpenPokemon));
                _mainMenuItemList.Add(new MainMenuItem(3, Localization.GetString("battle_action_Surrender", "Surrender"), 2, MainMenuOpenBag));
                MainMenuAddMegaEvolution(battleScreen, 3);
                break;
            }
        }
    }

    private void MainMenuAddMegaEvolution(BattleScreen battleScreen, int index)
    {
        // Requires Mega Bracelet (ID 576)
        if ((Core.Player.Inventory.GetItemAmount(576.ToString()) > 0) == false)
        {
            return;
        }

        if (_mainMenuIndex >= _mainMenuItemList.Count - 1)
        {
            _mainMenuIndex = 0;
        }

        if (battleScreen.FieldEffects.MegaEvolved.Self == true)
        {
            return;
        }

        int pokeIndex = battleScreen.SelfPokemonIndex;
        if (battleScreen.FieldEffects.MegaEvolved.Self == false)
        {
            if (Core.Player.Pokemons[pokeIndex].Item != null)
            {
                if (Core.Player.Pokemons[pokeIndex].Item!.IsGameModeItem == true)
                {
                    GameModeItem gmItem = (GameModeItem)Core.Player.Pokemons[pokeIndex].Item!;
                    if (gmItem.gmIsMegaStone == true)
                    {
                        if (Core.Player.Pokemons[pokeIndex].Number == gmItem.gmMegaPokemonNumber)
                        {
                            _mainMenuItemList.Add(new MainMenuItem(5, Localization.GetString("battle_action_MegaEvolve", "Mega Evolve!"), index, MainMenuMegaEvolve));
                        }
                    }
                }
                else
                {
                    if (Core.Player.Pokemons[pokeIndex].Item!.IsMegaStone == true)
                    {
                        MegaStone megaStone = (MegaStone)Core.Player.Pokemons[pokeIndex].Item!;
                        if (megaStone.MegaPokemonNumber == Core.Player.Pokemons[pokeIndex].Number)
                        {
                            _mainMenuItemList.Add(new MainMenuItem(5, Localization.GetString("battle_action_MegaEvolve", "Mega Evolve!"), index, MainMenuMegaEvolve));
                        }
                    }
                }
            }
        }
    }

    // -----------------------------------------------------------------------
    // Main menu action handlers
    // -----------------------------------------------------------------------

    private void MainMenuOpenBattleMenu(BattleScreen battleScreen)
    {
        _retractMenu = true;
        _nextMenuState = MenuStates.Moves;
        PartyScreen.Selected = -1;
        if (_moveMenuIndex != _moveMenuLastIndex)
        {
            _moveMenuNextIndex = _moveMenuLastIndex;
            _moveMenuIndex = _moveMenuLastIndex;
        }
        battleScreen.BattleQuery.Clear();
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(11, 0.5f, 14.0f), new Vector3(11, 0.5f, 14.0f),
            Screen.Camera.Speed, Screen.Camera.Speed,
            -(MathHelper.PiOver4 + 0.3f), -(MathHelper.PiOver4 + 0.3f),
            -0.3f, -0.3f,
            0.04f, 0.04f);
        battleScreen.BattleQuery.AddRange(new QueryObject[] { q });
    }

    private void MainMenuOpenPokemon(BattleScreen battleScreen)
    {
        _tempBattleScreen = battleScreen;

        Player.Temp.PokemonScreenIndex = battleScreen.SelfPokemonIndex;
        PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, Item.GetItemByID(5.ToString()), ShowPokemonMenu, Localization.GetString("party_screen_ChoosePokemon", "Choose Pokémon"), true)
        {
            Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
            CanExit = true,
        };
        selScreen.SelectedObject += ShowPokemonMenuHandler;

        Core.SetScreen(selScreen);
    }

    private void MainMenuOpenBag(BattleScreen battleScreen)
    {
        _tempBattleScreen = battleScreen;
        NewInventoryScreen selScreen = new NewInventoryScreen(Core.CurrentScreen)
        {
            Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
            CanExit = true,
        };

        selScreen.SelectedObject += SelectedItemHandler;
        Core.SetScreen(selScreen);
    }

    private void MainMenuRun(BattleScreen battleScreen)
    {
        if (BattleCalculation.CanRun(true, battleScreen) == true && BattleScreen.CanRun == true)
        {
            battleScreen.BattleQuery.Clear();
            battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
            battleScreen.BattleQuery.Add(battleScreen.FocusOwnPlayer());
            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\running", false));
            battleScreen.BattleQuery.Add(new TextQueryObject("Got away safely!"));
            battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
            Battle.Won = true;
            Battle.Fled = true;
        }
        else
        {
            bool trapped = false;
            Pokemon p = battleScreen.SelfPokemon!;
            Pokemon op = battleScreen.OpponentPokemon!;

            if (op.Ability.Name.ToLower().Equals("shadow tag") && (p.Ability.Name.ToLower().Equals("shadow tag") == false) && op.HP > 0)
            {
                trapped = true;
            }

            if (op.Ability.Name.ToLower().Equals("arena trap") && op.HP > 0 && battleScreen.FieldEffects.IsGrounded(true, battleScreen) == true)
            {
                trapped = true;
            }

            if (op.Ability.Name.ToLower().Equals("magnet pull") && op.HP > 0)
            {
                if (p.Type1.Type == Element.Types.Steel || (p.Type2 != null && p.Type2.Type == Element.Types.Steel))
                {
                    trapped = true;
                }
            }

            if (trapped == true)
            {
                Screen.TextBox.Show(Localization.GetString("battle_cannot_run_ability", "Failed to run away because of~") + Localization.GetString("ability_name_" + op.Ability.ID.ToString(), op.Ability.Name) + ".", [], true, true);
            }
            else
            {
                battleScreen.BattleQuery.Clear();
                battleScreen.BattleQuery.Add(battleScreen.FocusBattle());
                battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                battleScreen.Battle.InitializeRound(battleScreen, new BattleRoundConst
                {
                    StepType = BattleRoundConst.StepTypes.Text,
                    Argument = Localization.GetString("battle_cannot_run", "Failed to run away."),
                });
            }
        }
    }

    private void MainMenuUseSafariBall(BattleScreen battleScreen)
    {
        if (Core.Player.CanCatchPokemon() == true)
        {
            if (Core.Player.Inventory.GetItemAmount(181.ToString()) > 0)
            {
                Core.Player.Inventory.RemoveItem(181.ToString(), 1);
                battleScreen.BattleQuery.Clear();
                battleScreen.BattleQuery.Add(battleScreen.FocusBattle());
                battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                Core.SetScreen(new BattleCatchScreen(battleScreen, Item.GetItemByID(181.ToString())));

                Core.Player.UsedItemsToCheckScriptDelayFor.Add("181");

                String safariBallText = Localization.GetString("item_name_181", "Safari Ball") + " x" + Core.Player.Inventory.GetItemAmount(181.ToString()).ToString();
                if (Core.Player.Inventory.GetItemAmount(181.ToString()) == 0)
                {
                    safariBallText = Localization.GetString("battle_NoSafariBalls", "No Safari Balls.");
                }
                _mainMenuItemList[0].Text = safariBallText;
            }
        }
        else
        {
            Screen.TextBox.Show(Localization.GetString("battle_NoMoreRoomInPC", "There's no more room for~Pokémon in the PC!"), [], true, true);
        }
    }

    private void MainMenuUseSportBall(BattleScreen battleScreen)
    {
        if (Core.Player.Inventory.GetItemAmount(177.ToString()) > 0)
        {
            Core.Player.Inventory.RemoveItem(177.ToString(), 1);
            battleScreen.BattleQuery.Clear();
            battleScreen.BattleQuery.Add(battleScreen.FocusBattle());
            battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
            Core.SetScreen(new BattleCatchScreen(battleScreen, Item.GetItemByID(177.ToString())));

            String sportBallText = "Sport Ball x" + Core.Player.Inventory.GetItemAmount(177.ToString()).ToString();
            if (Core.Player.Inventory.GetItemAmount(177.ToString()) == 0)
            {
                sportBallText = "No Sport Balls.";
            }
            _mainMenuItemList[0].Text = sportBallText;
        }
    }

    private void MainMenuThrowMud(BattleScreen battleScreen)
    {
        if (battleScreen.PokemonSafariStatus > 0)
        {
            battleScreen.PokemonSafariStatus = 0;
        }
        battleScreen.PokemonSafariStatus -= Core.Random.Next(2, 5);
        battleScreen.BattleQuery.Clear();
        battleScreen.BattleQuery.Add(battleScreen.FocusBattle());
        battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
        battleScreen.Battle.InitializeRound(battleScreen, new BattleRoundConst
        {
            StepType = BattleRoundConst.StepTypes.Text,
            Argument = "Threw Mud at " + battleScreen.OpponentPokemon!.GetDisplayName() + "!",
        });
    }

    private void MainMenuThrowBait(BattleScreen battleScreen)
    {
        if (battleScreen.PokemonSafariStatus < 0)
        {
            battleScreen.PokemonSafariStatus = 0;
        }
        battleScreen.PokemonSafariStatus += Core.Random.Next(2, 5);
        battleScreen.BattleQuery.Clear();
        battleScreen.BattleQuery.Add(battleScreen.FocusBattle());
        battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
        battleScreen.Battle.InitializeRound(battleScreen, new BattleRoundConst
        {
            StepType = BattleRoundConst.StepTypes.Text,
            Argument = "Threw a Bait at " + battleScreen.OpponentPokemon!.GetDisplayName() + "!",
        });
    }

    private void MainMenuMegaEvolve(BattleScreen battleScreen)
    {
        _retractMenu = true;
        _nextMenuState = MenuStates.Moves;

        battleScreen.BattleQuery.Clear();
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(11, 0.5f, 14.0f), new Vector3(11, 0.5f, 14.0f),
            Screen.Camera.Speed, Screen.Camera.Speed,
            -(MathHelper.PiOver4 + 0.3f), -(MathHelper.PiOver4 + 0.3f),
            -0.3f, -0.3f,
            0.04f, 0.04f);
        battleScreen.BattleQuery.AddRange(new QueryObject[] { q });

        battleScreen.IsMegaEvolvingOwn = true;
        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            String additionalData = Core.Player.Pokemons[i].AdditionalData;
            switch (additionalData)
            {
                case "mega":
                case "mega_x":
                case "mega_y":
                    battleScreen.IsMegaEvolvingOwn = false;
                    break;
                default:
                    // do nothing
                    break;
            }
        }
    }

    // -----------------------------------------------------------------------
    // Move menu
    // -----------------------------------------------------------------------

    private void DrawMoveMenu(BattleScreen battleScreen)
    {
        foreach (MoveMenuItem m in _moveMenuItemList)
        {
            m.Draw(_allItemsExtended, _selectedItemExtended, (m.Index == _moveMenuIndex), battleScreen);
        }
    }

    private void UpdateMoveMenu(BattleScreen battleScreen)
    {
        if (battleScreen.ClearMoveMenuTime == true)
        {
            _moveMenuItemList.Clear();
            battleScreen.ClearMoveMenuTime = false;
        }
        if (_moveMenuIndex > _moveMenuItemList.Count - 1)
        {
            _moveMenuIndex = 0;
            _moveMenuNextIndex = 0;
        }

        if (_moveMenuChoseMove == true)
        {
            _moveMenuAlpha -= 15;
            if (_moveMenuAlpha <= 0)
            {
                _moveMenuAlpha = 0;
                if (battleScreen.SelfPokemon!.attacks[_moveMenuIndex].swapsOutSelfPokemon == true)
                {
                    if (PartyScreen.Selected == -1 && Core.Player.CountFightablePokemon > 1)
                    {
                        PartyScreen selScreen = new PartyScreen(
                            Core.CurrentScreen,
                            Item.GetItemByID(5.ToString()),
                            null,
                            Localization.GetString("party_screen_ChoosePokemon", "Choose Pokémon"),
                            false,
                            false,
                            false)
                        {
                            Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
                            CanExit = false,
                            CannotChooseIndex = battleScreen.SelfPokemonIndex,
                            SelectButtonText = Localization.GetString("global_switch", "Switch"),
                        };
                        selScreen.SelectedObject += null;

                        Core.SetScreen(selScreen);
                    }
                    bool lastFightingPokemon = false;
                    if (Core.Player.CountFightablePokemon == 1)
                    {
                        lastFightingPokemon = true;
                    }
                    if (Core.CurrentScreen.Identification != Screen.Identifications.PartyScreen && (PartyScreen.Selected != -1 || lastFightingPokemon == true))
                    {
                        if (battleScreen.SelfPokemon!.attacks[_moveMenuIndex].ID == 226)
                        {
                            battleScreen.FieldEffects.BatonPassIndex = (PartyScreen.Selected, battleScreen.FieldEffects.BatonPassIndex.Opponent);
                        }
                        else
                        {
                            battleScreen.FieldEffects.SwapIndex = (PartyScreen.Selected, battleScreen.FieldEffects.SwapIndex.Opponent);
                        }
                        PartyScreen.Selected = -1;
                        MoveMenuStartRound(battleScreen);
                        Visible = false;
                    }
                }
                else
                {
                    MoveMenuStartRound(battleScreen);
                    Visible = false;
                }
            }
        }
        else
        {
            UseStruggle(battleScreen);

            if (_moveMenuItemList.Count == 0 || _moveMenuCreatedID.Equals(battleScreen.SelfPokemon!.IndividualValue) == false)
            {
                if (_moveMenuCreatedID.Equals(battleScreen.SelfPokemon!.IndividualValue) == false)
                {
                    _moveMenuIndex = 0;
                }
                CreateMoveMenuItems(battleScreen);
                if (_moveMenuIndex != _moveMenuLastIndex)
                {
                    _moveMenuNextIndex = _moveMenuLastIndex;
                    _moveMenuIndex = _moveMenuLastIndex;

                    if (_moveMenuIndex > _moveMenuItemList.Count - 1)
                    {
                        _moveMenuNextIndex = 0;
                        _moveMenuIndex = 0;
                        _moveMenuLastIndex = 0;
                    }
                }
                _moveMenuCreatedID = battleScreen.SelfPokemon!.IndividualValue;
            }
            if (_retractMenu == false)
            {
                foreach (MoveMenuItem m in _moveMenuItemList)
                {
                    m.Update(battleScreen, _allItemsExtended, (m.Index == _moveMenuIndex));
                }
            }

            UpdateMenuOptions(ref _moveMenuIndex, ref _moveMenuNextIndex, _moveMenuItemList.Count);

            if (Controls.Dismiss(true, true, true) == true && _retractMenu == false && _isExtracting == false && _isRetracting == false)
            {
                SoundManager.PlaySound("select");
                _retractMenu = true;
                _nextMenuState = MenuStates.Main;

                battleScreen.BattleQuery.Clear();
                for (int i = 0; i <= 99; i++)
                {
                    battleScreen.InsertCasualCameramove();
                }

                CameraQueryObject fQ = (CameraQueryObject)battleScreen.BattleQuery[0];

                CameraQueryObject q = new CameraQueryObject(fQ.StartPosition, Screen.Camera.Position, 0.06f, 0.06f, fQ.StartYaw, Screen.Camera.Yaw, fQ.TargetPitch, Screen.Camera.Pitch, 0.06f, 0.06f);
                battleScreen.BattleQuery.Insert(0, q);

                battleScreen.IsMegaEvolvingOwn = false;
            }

            if (battleScreen.BattleQuery.Count == 0)
            {
                for (int i = 0; i <= 50; i++)
                {
                    CameraQueryObject q = new CameraQueryObject(new Vector3(11.5f, 0.5f, 13.0f), Screen.Camera.Position, 0.001f, Screen.Camera.Yaw, Screen.Camera.Pitch);
                    CameraQueryObject q1 = new CameraQueryObject(new Vector3(11, 0.5f, 14.0f), new Vector3(11.5f, 0.5f, 13.0f), 0.001f, Screen.Camera.Yaw, Screen.Camera.Pitch);
                    battleScreen.BattleQuery.AddRange(new QueryObject[] { q, q1 });
                }
            }
        }
    }

    private void CreateMoveMenuItems(BattleScreen battleScreen)
    {
        _moveMenuItemList.Clear();

        for (int i = 0; i <= battleScreen.SelfPokemon!.attacks.Count - 1; i++)
        {
            _moveMenuItemList.Add(new MoveMenuItem(i, battleScreen.SelfPokemon!.attacks[i], MoveMenuChooseMove));
        }
    }

    private void MoveMenuChooseMove(BattleScreen battleScreen)
    {
        _moveMenuChoseMove = true;
        _moveMenuAlpha = 255;
    }

    private void MoveMenuStartRound(BattleScreen battleScreen)
    {
        if (battleScreen.IsRemoteBattle == true && battleScreen.IsHost == false)
        {
            battleScreen.BattleQuery.Clear();
            battleScreen.BattleQuery.Add(battleScreen.FocusBattle());
            battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
            String switchPokeSuffix = String.Empty;
            if (battleScreen.FieldEffects.BatonPassIndex.Self != -1)
            {
                switchPokeSuffix = ";BATON;" + battleScreen.FieldEffects.BatonPassIndex.Self;
            }
            else if (battleScreen.FieldEffects.SwapIndex.Self != -1)
            {
                switchPokeSuffix = ";SWAP;" + battleScreen.FieldEffects.SwapIndex.Self;
            }
            if (battleScreen.IsMegaEvolvingOwn == true)
            {
                battleScreen.SendClientCommand("MEGA|" + battleScreen.SelfPokemon!.attacks[_moveMenuIndex].ID.ToString() + switchPokeSuffix);
                battleScreen.IsMegaEvolvingOwn = false;
                battleScreen.FieldEffects.MegaEvolved = (true, battleScreen.FieldEffects.MegaEvolved.Opponent);
            }
            else
            {
                battleScreen.SendClientCommand("MOVE|" + battleScreen.SelfPokemon!.attacks[_moveMenuIndex].ID.ToString() + switchPokeSuffix);
            }
        }
        else
        {
            if (battleScreen.IsMegaEvolvingOwn == true)
            {
                battleScreen.FieldEffects.MegaEvolved = (true, battleScreen.FieldEffects.MegaEvolved.Opponent);
            }
            battleScreen.SelfStatistics.Moves += 1;
            battleScreen.BattleQuery.Clear();
            battleScreen.BattleQuery.Add(battleScreen.FocusBattle());
            battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
            battleScreen.Battle.InitializeRound(battleScreen, new BattleRoundConst
            {
                StepType = BattleRoundConst.StepTypes.Move,
                Argument = battleScreen.SelfPokemon!.attacks[_moveMenuIndex],
            });
        }
        _moveMenuChoseMove = false;
        _moveMenuAlpha = 255;
        _moveMenuItemList.Clear();
    }

    private void UseStruggle(BattleScreen battleScreen)
    {
        if (battleScreen.SelfPokemon!.CountPP() <= 0)
        {
            battleScreen.BattleQuery.Clear();
            battleScreen.BattleQuery.Add(battleScreen.FocusBattle());
            battleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
            battleScreen.Battle.InitializeRound(battleScreen, new BattleRoundConst
            {
                StepType = BattleRoundConst.StepTypes.Move,
                Argument = Attack.GetAttackByID(165),
            });
        }
    }

    // -----------------------------------------------------------------------
    // Switch Pokémon (static helpers)
    // -----------------------------------------------------------------------

    private static BattleScreen _tempBattleScreen = null!;

    private static void ShowPokemonMenuHandler(Object[] args)
    {
        ShowPokemonMenu((int)args[0]);
    }

    private static bool ShowPokemonMenu(int pokeIndex)
    {
        SwitchPokemonTo(pokeIndex);
        return true;
    }

    private static void SwitchPokemonTo(int pokeIndex)
    {
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];

        if (pokeIndex == _tempBattleScreen.SelfPokemonIndex)
        {
            if (pokemon.Status != Pokemon.StatusProblems.Fainted)
            {
                Screen.TextBox.Show(pokemon.GetDisplayName() + " " + Localization.GetString("battle_switch_already_in_battle", "is already~in battle!"), [], true, false);
            }
            else
            {
                Screen.TextBox.Show(pokemon.GetDisplayName() + " " + Localization.GetString("battle_switch_fainted", "is fainted!"), [], true, false);
            }
        }
        else
        {
            if (pokemon.IsEgg == false)
            {
                if (pokemon.Status != Pokemon.StatusProblems.Fainted)
                {
                    if (BattleCalculation.CanSwitch(_tempBattleScreen, true) == false)
                    {
                        Screen.TextBox.Show(Localization.GetString("battle_cannot_switch", "Cannot switch out."), [], true, false);
                    }
                    else
                    {
                        if (_tempBattleScreen.IsRemoteBattle == true && _tempBattleScreen.IsHost == false)
                        {
                            _tempBattleScreen.OppFaint = false;
                            _tempBattleScreen.SelfStatistics.Switches += 1;
                            _tempBattleScreen.BattleQuery.Clear();
                            _tempBattleScreen.BattleQuery.Add(_tempBattleScreen.FocusBattle());
                            _tempBattleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                            _tempBattleScreen.SendClientCommand("SWITCH|" + pokeIndex.ToString());
                            PartyScreen.Selected = -1;
                        }
                        else
                        {
                            _tempBattleScreen.BattleQuery.Clear();
                            _tempBattleScreen.BattleQuery.Add(_tempBattleScreen.FocusBattle());
                            _tempBattleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                            _tempBattleScreen.Battle.InitializeRound(_tempBattleScreen, new BattleRoundConst
                            {
                                StepType = BattleRoundConst.StepTypes.Switch,
                                Argument = pokeIndex.ToString(),
                            });
                            PartyScreen.Selected = -1;
                        }
                    }
                }
                else
                {
                    Screen.TextBox.Show(pokemon.GetDisplayName() + " " + Localization.GetString("battle_switch_fainted", "is fainted!"), [], true, false);
                }
            }
            else
            {
                Screen.TextBox.Show(Localization.GetString("battle_switch_egg", "Cannot switch in~the egg!"), [], true, false);
            }
        }
    }

    // -----------------------------------------------------------------------
    // Use item (static helpers)
    // -----------------------------------------------------------------------

    private static String _tempItemID = "-1";

    private static void SelectedItemHandler(Object[] args)
    {
        SelectedItem((String)args[0]);
    }

    private static void SelectedItem(String itemID)
    {
        Item item = Item.GetItemByID(itemID);

        if (item.CanBeUsedInBattle == true)
        {
            if (item.IsBall == true)
            {
                if (Core.Player.CanCatchPokemon() == false && _tempBattleScreen.IsTrainerBattle == false)
                {
                    Screen.TextBox.Show(Localization.GetString("battle_NoMoreRoomInPC", "There's no more room for~Pokémon in the PC!"), [], true, true);
                }
                else
                {
                    Core.Player.Inventory.RemoveItem(itemID, 1);
                    if (_tempBattleScreen.IsTrainerBattle == false)
                    {
                        if (BattleScreen.CanCatch == true || GameModeManager.GetGameRuleValue("OnlyCaptureFirst", "0").Equals("1") == true && Core.Player.PokeFiles.Contains(BattleScreen.TempPokeFile) == false)
                        {
                            _tempBattleScreen.BattleQuery.Clear();
                            _tempBattleScreen.BattleQuery.Add(_tempBattleScreen.FocusBattle());
                            _tempBattleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                            Core.SetScreen(new BattleCatchScreen(_tempBattleScreen, Item.GetItemByID(itemID)));
                            PlayerStatistics.Track("[4]Poké Balls used", 1);
                        }
                        else
                        {
                            _tempBattleScreen.BattleQuery.Clear();
                            _tempBattleScreen.BattleQuery.Add(_tempBattleScreen.FocusBattle());
                            _tempBattleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                            _tempBattleScreen.Battle.InitializeRound(_tempBattleScreen, new BattleRoundConst
                            {
                                StepType = BattleRoundConst.StepTypes.Text,
                                Argument = "The wild Pokémon blocked the Pokéball!",
                            });
                        }
                    }
                    else
                    {
                        _tempBattleScreen.BattleQuery.Clear();
                        _tempBattleScreen.BattleQuery.Add(_tempBattleScreen.FocusBattle());
                        _tempBattleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));
                        _tempBattleScreen.Battle.InitializeRound(_tempBattleScreen, new BattleRoundConst
                        {
                            StepType = BattleRoundConst.StepTypes.Text,
                            Argument = "Hey! Don't be a thief!",
                        });
                    }
                }
            }
            else
            {
                _tempItemID = itemID;

                if (item.BattleSelectPokemon == true)
                {
                    PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, item, item.UseOnPokemon, Localization.GetString("global_use", "Use") + " " + item.OneLineName(), true)
                    {
                        Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
                        CanExit = true,
                    };
                    selScreen.SelectedObject += UseItemHandler;

                    Core.SetScreen(selScreen);
                }
                else
                {
                    Screen cScreen = Core.CurrentScreen;
                    while (cScreen.PreScreen != null && cScreen.Identification.Equals(Screen.Identifications.BattleScreen) == false)
                    {
                        cScreen = cScreen.PreScreen;
                    }
                    UseItem(((BattleScreen)cScreen).SelfPokemonIndex);
                }
            }
        }
    }

    private static void UseItemHandler(Object[] args)
    {
        UseItem((int)args[0]);
    }

    private static void UseItem(int pokeIndex)
    {
        Item item = Item.GetItemByID(_tempItemID);

        bool useOnOwn = false;
        bool useOnOpp = false;

        Screen cScreen = Core.CurrentScreen;
        while (cScreen.PreScreen != null && cScreen.Identification.Equals(Screen.Identifications.BattleScreen) == false)
        {
            cScreen = cScreen.PreScreen;
        }

        if (cScreen.Identification.Equals(Screen.Identifications.BattleScreen) == true)
        {
            if (item.IsGameModeItem == true)
            {
                GameModeItem gmItem = (GameModeItem)item;
                if (gmItem.gmUseOnOwnEffects != null)
                {
                    useOnOwn = gmItem.UseOnPokemon(pokeIndex);
                }
                if (gmItem.gmUseOnOppEffects != null && gmItem.gmUseOnOwnEffects == null)
                {
                    useOnOpp = gmItem.UseOnOppPokemon((BattleScreen)cScreen);
                }
            }
            else
            {
                useOnOwn = item.UseOnPokemon(pokeIndex);
            }
        }
        if (useOnOwn == true || useOnOpp == true)
        {
            String[] cudChewBerries = { "oran", "sitrus", "figy", "wiki", "mago", "aguav", "iapapa", "liechi", "ganlon", "salac", "petaya", "apicot", "lansat", "starf", "lum", "rawst", "aspear", "cheri", "chesto" };
            if (Core.Player.Pokemons[pokeIndex].Ability.Name.ToLower().Equals("cud chew") && item.IsBerry == true && cudChewBerries.Contains(item.Name.ToLower()))
            {
                _tempBattleScreen.FieldEffects.CudChewBerry = (item, _tempBattleScreen.FieldEffects.CudChewBerry.Opponent);
                _tempBattleScreen.FieldEffects.CudChewIndex = (pokeIndex, _tempBattleScreen.FieldEffects.CudChewIndex.Opponent);
            }
            if (item.ReplacesBattleQuery == false)
            {
                _tempBattleScreen.BattleQuery.Clear();
                _tempBattleScreen.BattleQuery.Add(_tempBattleScreen.FocusBattle());
                _tempBattleScreen.BattleQuery.Insert(0, new ToggleMenuQueryObject(true));

                _tempBattleScreen.Battle.InitializeRound(_tempBattleScreen, new BattleRoundConst
                {
                    StepType = BattleRoundConst.StepTypes.Item,
                    Argument = _tempItemID,
                });
            }
        }
    }

    // -----------------------------------------------------------------------
    // Nested menu item classes
    // -----------------------------------------------------------------------

    public class MainMenuItem
    {
        private Texture2D _iconSelected;
        private Texture2D _iconUnselected;
        private int _iconFading;

        public String Text;
        public int Index;

        private Texture2D _texture;
        private D_MainMenuClick _clickAction;

        public MainMenuItem(int iconIndex, String text, int index, D_MainMenuClick clickAction)
        {
            if (iconIndex > 4)
            {
                _iconUnselected = TextureManager.GetTexture(_battleInterfaceTexture, new Rectangle(160 + (iconIndex - 5) * 24, 56, 24, 24), String.Empty);
                _iconSelected = TextureManager.GetTexture(_battleInterfaceTexture, new Rectangle(160 + (iconIndex - 5) * 24, 80, 24, 24), String.Empty);
            }
            else
            {
                _iconUnselected = TextureManager.GetTexture(_battleInterfaceTexture, new Rectangle(iconIndex * 24, 56, 24, 24), String.Empty);
                _iconSelected = TextureManager.GetTexture(_battleInterfaceTexture, new Rectangle(iconIndex * 24, 80, 24, 24), String.Empty);
            }

            _iconFading = 0;
            Text = text;
            Index = index;
            _texture = TextureManager.GetTexture(@"GUI\Menus\General");
            _clickAction = clickAction;
        }

        public void Draw(int allExtended, int selExtended, bool isSelected)
        {
            int extraExtended = 0;
            if (isSelected == true)
            {
                Canvas.DrawGradient(new Rectangle(Core.windowSize.Width - 295, 100 + Index * 96, 295, 112), new Color(42, 167, 198, 0), new Color(42, 167, 198, (selExtended + allExtended)), true, -1);
                extraExtended = selExtended;
            }
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - (allExtended + extraExtended), 116 + Index * 96, 80, 80), new Rectangle(16, 16, 16, 16), Color.White);
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - (allExtended + extraExtended) + 80, 116 + Index * 96, allExtended + extraExtended - 80, 80), new Rectangle(32, 16, 16, 16), Color.White);

            Core.SpriteBatch.Draw(_iconUnselected, new Rectangle(Core.windowSize.Width - (allExtended + extraExtended) + 28, 132 + Index * 96, 48, 48), Color.White);
            if (isSelected == true)
            {
                Core.SpriteBatch.Draw(_iconSelected, new Rectangle(Core.windowSize.Width - (allExtended + extraExtended) + 28, 132 + Index * 96, 48, 48), new Color(255, 255, 255, (selExtended + allExtended)));
                Core.SpriteBatch.DrawString(FontManager.MainFont, Text, new Vector2((int)(Core.windowSize.Width - (allExtended + extraExtended) + 86), (int)(144 + Index * 96)), new Color(0, 0, 0, (selExtended + allExtended)));
            }
            else
            {
                if (_iconFading > 0)
                {
                    Core.SpriteBatch.Draw(_iconSelected, new Rectangle(Core.windowSize.Width - allExtended + 28, 132 + Index * 96, 48, 48), new Color(255, 255, 255, _iconFading));
                }
            }
        }

        public void Update(BattleScreen battleScreen, int allExtended, bool isSelected)
        {
            Activate(battleScreen, allExtended, isSelected);
            if (isSelected == false)
            {
                if (MouseHandler.IsInRectangle(new Rectangle(Core.windowSize.Width - allExtended + 28, 132 + Index * 96, 48, 48)) == true)
                {
                    if (_iconFading < 255)
                    {
                        _iconFading += 15;
                        if (_iconFading > 255)
                        {
                            _iconFading = 255;
                        }
                    }
                }
                else
                {
                    if (_iconFading > 0)
                    {
                        _iconFading -= 15;
                        if (_iconFading < 0)
                        {
                            _iconFading = 0;
                        }
                    }
                }
            }
            else
            {
                _iconFading = 255;
            }
        }

        public void Activate(BattleScreen battleScreen, int allExtended, bool isSelected)
        {
            if (battleScreen.BattleMenu._isExtracting == false && battleScreen.BattleMenu._isRetracting == false)
            {
                if (Controls.Accept(false, true, true) == true && isSelected == true)
                {
                    SoundManager.PlaySound("select");
                    _clickAction(battleScreen);
                }
                if (Controls.Accept(true, false, false) == true)
                {
                    if (MouseHandler.IsInRectangle(new Rectangle(Core.windowSize.Width - 295, 116 + Index * 96, 295, 80)) == true)
                    {
                        if (isSelected == true)
                        {
                            SoundManager.PlaySound("select");
                            _clickAction(battleScreen);
                        }
                        else
                        {
                            battleScreen.BattleMenu._mainMenuNextIndex = Index;
                            battleScreen.BattleMenu._isRetracting = true;
                        }
                    }
                }
            }
        }
    }

    public class MoveMenuItem
    {
        private Attack _move;
        public int Index = 0;

        private Texture2D _texture;
        private D_MoveMenuClick _clickAction;

        public MoveMenuItem(int index, Attack move, D_MoveMenuClick clickAction)
        {
            Index = index;
            _move = move;
            _clickAction = clickAction;
            _texture = TextureManager.GetTexture(@"GUI\Menus\General");
        }

        public void Draw(int allExtended, int selExtended, bool isSelected, BattleScreen battleScreen)
        {
            int deductAlpha = 255 - battleScreen.BattleMenu._moveMenuAlpha;

            int extraExtended = 0;
            if (isSelected == true)
            {
                Canvas.DrawGradient(new Rectangle(Core.windowSize.Width - 295, 100 + Index * 96, 295, 112), new Color(42, 167, 198, 0), new Color(42, 167, 198, (selExtended + allExtended) - deductAlpha), true, -1);
                extraExtended = selExtended;
            }
            Color backgroundDrawColor = Color.White;
            FieldEffects fe = battleScreen.FieldEffects;
            if (_move.Disabled > 0 || fe.Encore.Self > 0 && (fe.EncoreMove.Self == null || fe.EncoreMove.Self.ID != _move.ID))
            {
                backgroundDrawColor = new Color(210, 210, 210);
            }
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - (allExtended + extraExtended), 116 + Index * 96, 80, 80), new Rectangle(16, 16, 16, 16), new Color(backgroundDrawColor.R, backgroundDrawColor.G, backgroundDrawColor.B, 255 - deductAlpha));
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - (allExtended + extraExtended) + 80, 116 + Index * 96, allExtended + extraExtended - 80, 80), new Rectangle(32, 16, 16, 16), new Color(backgroundDrawColor.R, backgroundDrawColor.G, backgroundDrawColor.B, 255 - deductAlpha));

            Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath(), _move.Type.GetElementImage(), String.Empty), new Rectangle(Core.windowSize.Width - (allExtended + extraExtended) + 28, 132 + Index * 96, 48, 16), new Color(255, 255, 255, 255 - deductAlpha));

            if (isSelected == true)
            {
                if (_move.Disabled > 0 || fe.Encore.Self > 0 && (fe.EncoreMove.Self == null || fe.EncoreMove.Self.ID != _move.ID))
                {
                    Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("battle_MoveDisabled", "Disabled!"), new Vector2((int)(Core.windowSize.Width - (allExtended + extraExtended) + 28), (int)(152 + Index * 96)), Color.Black);
                }
                else
                {
                    Color ppColor = GetPPColor();
                    ppColor.A = (byte)(extraExtended + allExtended - deductAlpha).Clamp(0, 255);

                    Core.SpriteBatch.DrawString(FontManager.MainFont, _move.CurrentPP + "/" + _move.MaxPP, new Vector2((int)(Core.windowSize.Width - (allExtended + extraExtended) + 28), (int)(152 + Index * 96)), ppColor);
                }
                Core.SpriteBatch.DrawString(FontManager.MainFont, _move.Name, new Vector2((int)(Core.windowSize.Width - (allExtended + extraExtended) + 86), (int)(132 + Index * 96)), new Color(0, 0, 0, (selExtended + allExtended) - deductAlpha));
            }
            else
            {
                Core.SpriteBatch.DrawString(FontManager.MainFont, _move.Name, new Vector2(Core.windowSize.Width - (allExtended + extraExtended) + 28, 152 + Index * 96), new Color(0, 0, 0, 255 - (extraExtended + allExtended) - deductAlpha));
            }
        }

        private Color GetPPColor()
        {
            Color c = Color.Black;
            int per = (int)((_move.CurrentPP / (float)_move.MaxPP) * 100);

            if (per <= 50 && per > 25)
            {
                c = Color.Orange;
            }
            else if (per <= 25)
            {
                c = Color.IndianRed;
            }

            return c;
        }

        public void Update(BattleScreen battleScreen, int allExtended, bool isSelected)
        {
            Activate(battleScreen, allExtended, isSelected);
        }

        public void Activate(BattleScreen battleScreen, int allExtended, bool isSelected)
        {
            if (battleScreen.BattleMenu._isExtracting == false && battleScreen.BattleMenu._isRetracting == false)
            {
                if (_move.CurrentPP > 0 || isSelected == false)
                {
                    FieldEffects fe = battleScreen.FieldEffects;

                    if (Controls.Accept(false, true, true) == true && isSelected == true)
                    {
                        SoundManager.PlaySound("select");
                        if (_move.Disabled == 0 && fe.Encore.Self == 0 || fe.EncoreMove.Self != null && fe.EncoreMove.Self.ID == _move.ID)
                        {
                            battleScreen.BattleMenu._moveMenuLastIndex = Index;
                            _clickAction(battleScreen);
                        }
                    }
                    if (Controls.Accept(true, false, false) == true)
                    {
                        if (MouseHandler.IsInRectangle(new Rectangle(Core.windowSize.Width - 295, 116 + Index * 96, 295, 80)) == true)
                        {
                            if (isSelected == true)
                            {
                                SoundManager.PlaySound("select");
                                if (_move.Disabled == 0 && fe.Encore.Self == 0 || fe.EncoreMove.Self != null && fe.EncoreMove.Self.ID == _move.ID)
                                {
                                    battleScreen.BattleMenu._moveMenuLastIndex = Index;
                                    _clickAction(battleScreen);
                                }
                            }
                            else
                            {
                                battleScreen.BattleMenu._moveMenuNextIndex = Index;
                                battleScreen.BattleMenu._moveMenuLastIndex = Index;
                                battleScreen.BattleMenu._isRetracting = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
