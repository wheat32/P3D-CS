using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.BattleSystem;
using P3D.Screens.UI;


namespace P3D;

public class PartyScreen : Screen, ISelectionScreen
{
    private String POKEMON_TITLE = "Pokémon";

    public String SelectButtonText = Localization.GetString("global_select", "Select");

    public int _index = 0;

    private Texture2D _texture = null!;
    private Texture2D _menuTexture = null!;

    // Animation
    private bool _closing = false;
    private float _enrollY = 0f;
    private float _interfaceFade = 0f;
    private Vector2 _cursorPosition = Vector2.Zero;
    public Vector2 _cursorDest = Vector2.Zero;

    private class PokemonAnimation
    {
        public float _shakeV;
        public bool _shakeLeft;
        public int _shakeCount;
    }

    private List<PokemonAnimation> _pokemonAnimations = [];

    private SelectMenu _menu = null!;

    private bool _isSwitching = false;
    private int _switchIndex = -1;

    // Message display
    private float _messageDelay = 0f;
    private String _messageText = String.Empty;
    private bool _messageShowing = false;

    // Choose Mode
    private bool ChooseMode = true;
    private List<Pokemon> PokemonList = [];
    private List<Pokemon>? AltPokemonList = null;

    private int FieldMovePokemonIndex = -1;
    public static int Selected = -1;
    public static bool Exited = false;

    private Items.Item _item = null!;

    private bool _used = false;

    public bool CanChooseFainted = true;
    public bool CanChooseEgg = true;
    public bool CanChooseHMPokemon = true;
    public bool CanChooseFusedPokemon = true;
    public int CannotChooseIndex = -1;

    public delegate void DoStuff(int PokeIndex);
    private DoStuff? ChoosePokemon;
    public Action? ExitedSub;

    public Attack? LearnAttack = null;
    public int LearnType = 0;
    private Object? _moveLearnArg = null;

    public String EvolutionItemID = "-1";

    // Prescreen blur
    private RenderTarget2D? _preScreenTexture;
    private RenderTarget2D _preScreenTarget = null!;
    private Identifications[] _blurScreens =
    {
        Identifications.BattleScreen,
        Identifications.OverworldScreen,
        Identifications.DirectTradeScreen,
        Identifications.WonderTradeScreen,
        Identifications.GTSSetupScreen,
        Identifications.GTSTradeScreen,
        Identifications.PVPLobbyScreen
    };

    private Resources.Blur.BlurHandler _blur = null!;

    public PartyScreen(Screen currentScreen, Items.Item item, DoStuff? choosePokemon, String title,
                       bool canExit, bool canChooseFainted, bool canChooseEgg,
                       List<Pokemon>? pokemonList = null, bool chooseMode = true, int cannotChooseIndex = -1)
    {
        _preScreenTarget = new RenderTarget2D(Core.GraphicsDevice, Core.windowSize.Width, Core.windowSize.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        _blur = new Resources.Blur.BlurHandler(Core.windowSize.Width, Core.windowSize.Height);

        _item = item;
        POKEMON_TITLE = title;
        CanExit = canExit;

        ChooseMode = chooseMode;
        CanChooseEgg = canChooseEgg;
        CanChooseFainted = canChooseFainted;
        CannotChooseIndex = cannotChooseIndex;
        if (choosePokemon != null)
        {
            ChoosePokemon = choosePokemon;
        }
        AltPokemonList = pokemonList;
        GetPokemonList();

        Identification = Identifications.PartyScreen;
        PreScreen = currentScreen;
        IsDrawingGradients = true;

        MouseVisible = false;
        CanChat = PreScreen.CanChat;
        CanBePaused = PreScreen.CanBePaused;

        _index = Player.Temp.PokemonScreenIndex;
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        if (TextureManager.TextureExist("GUI\\Menus\\PokemonInfo_" + Localization.LanguageSuffix))
        {
            _menuTexture = TextureManager.GetTexture("GUI\\Menus\\PokemonInfo_" + Localization.LanguageSuffix);
        }
        else
        {
            _menuTexture = TextureManager.GetTexture("GUI\\Menus\\PokemonInfo");
        }

        if (_index > PokemonList.Count - 1)
        {
            _index = 0;
        }
        _cursorDest = GetBoxPosition(_index);
        _cursorPosition = _cursorDest;

        for (int i = 0; i <= PokemonList.Count - 1; i++)
        {
            _pokemonAnimations.Add(new PokemonAnimation());
        }

        CheckForLegendaryEmblem();
        CheckForUnoDosTresEmblem();
        CheckForBeastEmblem();
        CheckForOverkillEmblem();

        _menu = new SelectMenu([""], 0, null, 0);
        _menu.Visible = false;
    }

    public PartyScreen(Screen currentScreen, Items.Item item, DoStuff? choosePokemon, String title, bool canExit)
        : this(currentScreen, item, choosePokemon, title, canExit, true, true)
    {
    }

    // Overloads accepting Func<int, bool> for callers that pass a bool-returning method group (return value is discarded)
    public PartyScreen(Screen currentScreen, Items.Item item, Func<int, bool>? choosePokemon, String title,
                       bool canExit, bool canChooseFainted, bool canChooseEgg,
                       List<Pokemon>? pokemonList = null, bool chooseMode = true, int cannotChooseIndex = -1)
        : this(currentScreen, item, choosePokemon != null ? (DoStuff)(i => choosePokemon(i)) : null,
               title, canExit, canChooseFainted, canChooseEgg, pokemonList, chooseMode, cannotChooseIndex)
    {
    }

    public PartyScreen(Screen currentScreen, Items.Item item, Func<int, bool>? choosePokemon, String title, bool canExit)
        : this(currentScreen, item, choosePokemon, title, canExit, true, true)
    {
    }

    public PartyScreen(Screen currentScreen)
        : this(currentScreen, new Items.Balls.Pokeball(), null, "Pokémon", true, true, true, null, false)
    {
    }

    private void GetPokemonList()
    {
        if (ChooseMode)
        {
            PokemonList.Clear();
            if (AltPokemonList != null)
            {
                foreach (Pokemon p in AltPokemonList)
                {
                    PokemonList.Add(Pokemon.GetPokemonByData(p.GetSaveData()));
                }
            }
            else
            {
                foreach (Pokemon p in Core.Player.Pokemons)
                {
                    String formData = PokemonForms.GetFormDataInParty(p);
                    if (formData != String.Empty && PokemonForms.GetTypeAdditionFromItem(p) == String.Empty)
                    {
                        p.LoadDefinitions(p.Number, formData);
                        p.ClearTextures();
                    }
                    if (PokemonForms.GetGenderFormMatch(p) == "match")
                    {
                        p.ClearTextures();
                    }
                    PokemonList.Add(Pokemon.GetPokemonByData(p.GetSaveData()));
                }
            }
        }
        else
        {
            foreach (Pokemon p in Core.Player.Pokemons)
            {
                String formData = PokemonForms.GetFormDataInParty(p);
                if (formData != String.Empty)
                {
                    if (PokemonForms.GetTypeAdditionFromItem(p) == String.Empty)
                    {
                        p.LoadDefinitions(p.Number, formData);
                        p.ClearTextures();
                    }
                }
                if (PokemonForms.GetGenderFormMatch(p) == "match")
                {
                    p.ClearTextures();
                }
            }
            PokemonList = Core.Player.Pokemons;
        }
    }

    public override void Draw()
    {
        if (_blurScreens.Contains(PreScreen.Identification))
        {
            DrawPrescreen();
        }
        else
        {
            PreScreen.Draw();
        }

        DrawGradients((int)(255 * _interfaceFade));

        DrawBackground();
        DrawPokemonArea();

        TextBox.Draw();

        if (_messageDelay > 0f)
        {
            float textFade = 1.0f;
            if (_messageDelay <= 1.0f)
            {
                textFade = _messageDelay;
            }

            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 150), (int)(Core.windowSize.Height - 200), 300, 100), new Color(0, 0, 0, (int)(150 * textFade * _interfaceFade)));

            String text = _messageText.Replace("~", " ").CropStringToWidth(FontManager.MainFont, 250);
            Vector2 size = FontManager.MainFont.MeasureString(text);

            Core.SpriteBatch.DrawString(FontManager.MainFont, text, new Vector2((float)(Core.windowSize.Width / 2 - size.X / 2), (float)(Core.windowSize.Height - 150 - size.Y / 2)), new Color(255, 255, 255, (int)(255 * textFade * _interfaceFade)));
        }
    }

    private void DrawPrescreen()
    {
        if (_preScreenTexture == null || _preScreenTexture.IsContentLost)
        {
            Core.SpriteBatch.EndBatch();

            RenderTarget2D target = _preScreenTarget;
            Core.GraphicsDevice.SetRenderTarget(target);
            Core.GraphicsDevice.Clear(Core.BackgroundColor);

            Core.SpriteBatch.BeginBatch();

            PreScreen.Draw();

            Core.SpriteBatch.EndBatch();

            Core.GraphicsDevice.SetRenderTarget(null);

            Core.SpriteBatch.BeginBatch();

            _preScreenTexture = target;
        }

        if (_interfaceFade < 1.0f)
        {
            Core.SpriteBatch.Draw(_preScreenTexture, Core.windowSize, Color.White);
        }
        Core.SpriteBatch.Draw(_blur.Perform(_preScreenTexture), Core.windowSize, new Color(255, 255, 255, (255 * _interfaceFade * 2).Clamp(0, 255)));
    }

    private void DrawBackground()
    {
        Color mainBackgroundColor = Color.White;
        if (_closing && _messageDelay == 0)
        {
            mainBackgroundColor = new Color(255, 255, 255, (int)(255 * _interfaceFade));
        }

        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);

        Canvas.DrawRectangle(new Rectangle(halfWidth - 400, halfHeight - 232, 384, 32), new Color(ColorProvider.MainColor(false).R, ColorProvider.MainColor(false).G, ColorProvider.MainColor(false).B, mainBackgroundColor.A));
        Canvas.DrawRectangle(new Rectangle(halfWidth - 400 + 384, halfHeight - 216, 16, 16), new Color(ColorProvider.MainColor(false).R, ColorProvider.MainColor(false).G, ColorProvider.MainColor(false).B, mainBackgroundColor.A));
        Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 400 + 384, halfHeight - 232, 16, 16), new Rectangle(80, 0, 16, 16), mainBackgroundColor);
        Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 400 + 384 + 16, halfHeight - 216, 16, 16), new Rectangle(80, 0, 16, 16), mainBackgroundColor);

        Core.SpriteBatch.DrawString(FontManager.MainFont, POKEMON_TITLE, new Vector2(halfWidth - 390, halfHeight - 228), mainBackgroundColor);

        for (int y = 0; y <= (int)_enrollY; y += 16)
        {
            for (int x = 0; x <= 800; x += 16)
            {
                Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 400 + x, halfHeight - 200 + y, 16, 16), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
            }
        }

        int modRes = (int)_enrollY % 16;
        if (modRes > 0)
        {
            for (int x = 0; x <= 800; x += 16)
            {
                Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 400 + x, (int)(_enrollY + (halfHeight - 200)), 16, modRes), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
            }
        }
    }

    private void DrawPokemonArea()
    {
        Canvas.DrawBorder(3, new Rectangle((int)_cursorPosition.X - 3, (int)_cursorPosition.Y - 3, 300, 82), new Color(200, 80, 80, (int)(200 * _interfaceFade)));

        if (_isSwitching)
        {
            Vector2 switchPosition = GetBoxPosition(_switchIndex);
            Canvas.DrawBorder(3, new Rectangle((int)switchPosition.X - 6, (int)switchPosition.Y - 6, 306, 88), new Color(80, 80, 200, (int)(200 * _interfaceFade)));
        }

        for (int i = 0; i <= PokemonList.Count - 1; i++)
        {
            DrawPokemon(i);
        }
    }

    private void DrawPokemon(int index)
    {
        Vector2 position = GetBoxPosition(index);
        Pokemon p = PokemonList[index];

        Color backColor = new Color(0, 0, 0, (int)(100 * _interfaceFade));
        if (p.IsShiny && p.IsEgg == false)
        {
            backColor = new Color(57, 59, 29, (int)(100 * _interfaceFade));
        }

        Canvas.DrawGradient(new Rectangle((int)position.X, (int)position.Y, 32, 76), new Color(0, 0, 0, 0), backColor, true, -1);
        Canvas.DrawRectangle(new Rectangle((int)position.X + 32, (int)position.Y, 228, 76), backColor);
        Canvas.DrawGradient(new Rectangle((int)position.X + 260, (int)position.Y, 32, 76), backColor, new Color(0, 0, 0, 0), true, -1);

        Texture2D pokeTexture = p.GetMenuTexture();
        int pokeXOffset = (int)((32 - pokeTexture.Width) / 2);
        int pokeYOffset = (int)((32 - pokeTexture.Height) / 2);

        if (p.IsEgg)
        {
            int percent = (int)((p.EggSteps / (float)p.BaseEggSteps) * 100);
            float shakeMulti = 1.0f;
            if (percent <= 33)
            {
                shakeMulti = 0.2f;
            }
            else if (percent > 33 && percent <= 66)
            {
                shakeMulti = 0.5f;
            }
            else
            {
                shakeMulti = 0.8f;
            }

            Core.SpriteBatch.Draw(pokeTexture, new Rectangle((int)(position.X + 24 - pokeXOffset), (int)(position.Y + 32 + 6 - pokeYOffset), 64, 64), null, new Color(255, 255, 255, (int)(255 * _interfaceFade)),
                             _pokemonAnimations[index]._shakeV * shakeMulti, new Vector2((int)(pokeTexture.Width / 2), (int)(pokeTexture.Width / 4 * 3)), SpriteEffects.None, 0f);

            GetFontRenderer().DrawString(FontManager.MainFont, p.GetDisplayName(), new Vector2(position.X + 156, position.Y + 27), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
        }
        else
        {
            float shakeMulti = (float)((p.HP / (double)p.MaxHP).Clamp(0.2f, 1.0f));

            Core.SpriteBatch.Draw(pokeTexture, new Rectangle((int)(position.X + 24 - pokeXOffset), (int)(position.Y + 32 + 6 - pokeYOffset), 64, 64), null, new Color(255, 255, 255, (int)(255 * _interfaceFade)),
                             _pokemonAnimations[index]._shakeV * shakeMulti, new Vector2((int)(pokeTexture.Width / 2), (int)(pokeTexture.Width / 4 * 3)), SpriteEffects.None, 0f);

            if (p.Item != null)
            {
                Core.SpriteBatch.Draw(p.Item.Texture, new Rectangle((int)position.X + 38, (int)position.Y + 32, 24, 24), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
            }

            GetFontRenderer().DrawString(FontManager.MainFont, p.GetDisplayName(), new Vector2(position.X + 78, position.Y + 5), new Color(255, 255, 255, (int)(255 * _interfaceFade)));

            switch (p.Gender)
            {
                case Pokemon.Genders.Male:
                    Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)(position.X + FontManager.MainFont.MeasureString(p.GetDisplayName()).X + 86), (int)(position.Y + 9), 7, 13), new Rectangle(25, 0, 7, 13), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
                    break;
                case Pokemon.Genders.Female:
                    Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)(position.X + FontManager.MainFont.MeasureString(p.GetDisplayName()).X + 85), (int)(position.Y + 9), 9, 13), new Rectangle(32, 0, 9, 13), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
                    break;
            }

            GetFontRenderer().DrawString(FontManager.MainFont, Localization.GetString("property_Lv.", "Lv.") + " " + p.Level.ToString(), new Vector2(position.X + 4, position.Y + 50), new Color(255, 255, 255, (int)(255 * _interfaceFade)));

            Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)position.X + 102, (int)position.Y + 32, 111, 15), new Rectangle(16, 32, 74, 10), new Color(255, 255, 255, (int)(255 * _interfaceFade)));

            double hpV = (double)p.HP / p.MaxHP;
            int hpWidth;
            if (_closing)
            {
                hpWidth = (int)(104 * hpV);
            }
            else
            {
                hpWidth = (int)((104 * _interfaceFade) * hpV);
            }
            int hpColorX = 0;
            if (hpV < 0.5f)
            {
                hpColorX = 5;
                if (hpV < 0.1f)
                {
                    hpColorX = 10;
                }
            }
            if (p.HP > 0 && hpWidth == 0)
            {
                hpWidth = 1;
            }
            if (hpWidth > 0)
            {
                Color drawColor = new Color(255, 255, 255, (int)(220 * _interfaceFade));

                Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)position.X + 78 + 24, (int)position.Y + 35, 2, 8), new Rectangle(hpColorX, 42, 2, 6), drawColor);
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)position.X + 78 + 24 + 2, (int)position.Y + 35, hpWidth, 8), new Rectangle(hpColorX + 2, 42, 1, 6), drawColor);
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)position.X + 78 + 24 + 2 + hpWidth, (int)position.Y + 35, 2, 8), new Rectangle(hpColorX + 3, 42, 2, 6), drawColor);
            }

            GetFontRenderer().DrawString(FontManager.MainFont, p.HP + " / " + p.MaxHP, new Vector2(position.X + 116, position.Y + 50), new Color(255, 255, 255, (int)(255 * _interfaceFade)));

            Texture2D? statusTexture = BattleStats.GetStatImage(p.Status);
            if (statusTexture != null)
            {
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)position.X + 68, (int)position.Y + 32, 6, 15), new Rectangle(0, 32, 4, 10), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)position.X + 74, (int)position.Y + 32, 28, 15), new Rectangle(16, 32, 4, 10), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
                Core.SpriteBatch.Draw(statusTexture, new Rectangle((int)(position.X + 66), (int)(position.Y + 33), 38, 12), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
            }
            else
            {
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)position.X + 78, (int)position.Y + 32, 24, 15), new Rectangle(0, 32, 16, 10), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
            }

            // Able/unable display (evolution item)
            String itemLabel = String.Empty;
            if (EvolutionItemID != "-1")
            {
                itemLabel = Localization.GetString("party_screen_Unable", "Unable!");
                if (p.IsEgg == false && p.CanEvolve(EvolutionCondition.EvolutionTrigger.ItemUse, EvolutionItemID.ToString()) == true)
                {
                    itemLabel = Localization.GetString("party_screen_Able", "Able!");
                }
            }

            // Able/unable display (TM/HM)
            String attackLabel = String.Empty;
            if (LearnType > 0)
            {
                attackLabel = Localization.GetString("party_screen_Unable", "Unable!");
                switch (LearnType)
                {
                    case 1: // Technical/Hidden Machine
                        if (((Items.Item)_moveLearnArg!).IsGameModeItem == true)
                        {
                            if (((Items.GameModeItem)_moveLearnArg).CanTeach(p) == String.Empty)
                            {
                                attackLabel = Localization.GetString("party_screen_Able", "Able!");
                            }
                        }
                        else
                        {
                            if (((Items.TechMachine)_moveLearnArg!).CanTeach(p) == String.Empty)
                            {
                                attackLabel = Localization.GetString("party_screen_Able", "Able!");
                            }
                        }
                        break;
                    case 2: // Learnable Move
                        if (LearnAttack != null)
                        {
                            bool canLearnMove = false;
                            for (int i = 0; i <= p.attackLearns.Count - 1; i++)
                            {
                                List<Attack> aList = p.attackLearns.Values.ElementAt(i);
                                for (int lA = 0; lA <= aList.Count - 1; lA++)
                                {
                                    if (aList[lA].ID == LearnAttack.ID)
                                    {
                                        canLearnMove = true;
                                    }
                                }
                            }
                            foreach (int eggMoveID in p.eggMoves)
                            {
                                if (eggMoveID == LearnAttack.ID)
                                {
                                    canLearnMove = true;
                                }
                            }
                            foreach (int tMMoveID in p.machines)
                            {
                                if (tMMoveID == LearnAttack.ID)
                                {
                                    canLearnMove = true;
                                }
                            }
                            if (canLearnMove == true)
                            {
                                attackLabel = Localization.GetString("party_screen_Able", "Able!");
                            }
                        }
                        break;
                }
            }
            GetFontRenderer().DrawString(FontManager.MainFont, itemLabel + attackLabel, new Vector2(position.X + 216, position.Y + 28), new Color(255, 255, 255, (int)(255 * _interfaceFade)));
        }

        if (_menu.Visible)
        {
            _menu.Draw();
        }
    }

    protected override SpriteBatch GetFontRenderer()
    {
        if (IsCurrentScreen() && _interfaceFade + 0.01f >= 1.0f)
        {
            return Core.FontRenderer;
        }
        else
        {
            return Core.SpriteBatch;
        }
    }

    public Vector2 GetBoxPosition(int index)
    {
        Vector2 position = Vector2.Zero;

        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);

        position.Y = (float)((Math.Floor(index / 2.0) * 128) + (halfHeight - 200) + 42);

        if (index % 2 == 0)
        {
            position.X = halfWidth - 328;
        }
        else
        {
            position.X = halfWidth + 36;
        }

        return position;
    }

    public void Minimized()
    {
        _cursorDest = GetBoxPosition(_index);
        _cursorPosition = _cursorDest;
    }

    public override void Update()
    {
        if (_cursorDest != GetBoxPosition(_index))
        {
            Minimized();
        }

        if (_pokemonAnimations.Count > 0)
        {
            PokemonAnimation animation = _pokemonAnimations[_index];
            if (animation._shakeLeft)
            {
                animation._shakeV -= 0.035f;
                if (animation._shakeV <= -0.4f)
                {
                    animation._shakeCount -= 1;
                    animation._shakeLeft = false;
                }
            }
            else
            {
                animation._shakeV += 0.035f;
                if (animation._shakeV >= 0.4f)
                {
                    animation._shakeCount -= 1;
                    animation._shakeLeft = true;
                }
            }
        }

        if (_messageDelay > 0f)
        {
            _messageDelay -= 0.1f;
            if (_messageDelay <= 0f)
            {
                _messageDelay = 0f;
            }
        }

        if (ChooseBox.Showing == false)
        {
            TextBox.Update();
        }

        if (_closing && _messageDelay == 0)
        {
            if (_interfaceFade > 0f)
            {
                _interfaceFade = MathHelper.Lerp(0, _interfaceFade, 0.8f);
                if (_interfaceFade < 0f)
                {
                    _interfaceFade = 0f;
                }
            }
            if (_enrollY > 0)
            {
                _enrollY = MathHelper.Lerp(0, _enrollY, 0.8f);
                if (_enrollY <= 0)
                {
                    _enrollY = 0;
                }
            }
            if (_enrollY <= 2.0f)
            {
                Core.SetScreen(PreScreen);
            }
        }
        else
        {
            if (_closing && (Controls.Dismiss() || Controls.Accept()))
            {
                _messageDelay = 0;
            }
            int maxWindowHeight = 400;
            if (_enrollY < maxWindowHeight)
            {
                _enrollY = MathHelper.Lerp(maxWindowHeight, _enrollY, 0.8f);
                if (_enrollY >= maxWindowHeight)
                {
                    _enrollY = maxWindowHeight;
                }
            }
            if (_interfaceFade < 1.0f)
            {
                _interfaceFade = MathHelper.Lerp(1, _interfaceFade, 0.95f);
                if (_interfaceFade > 1.0f)
                {
                    _interfaceFade = 1.0f;
                }
            }

            if (_menu.Visible)
            {
                _menu.Update();
            }
            else
            {
                if (Controls.Down(true, true, false, true, true, true) && _index < PokemonList.Count - 2)
                {
                    _pokemonAnimations[_index]._shakeV = 0;
                    _index += 2;
                    _cursorDest = GetBoxPosition(_index);
                }
                if (Controls.Up(true, true, false, true, true, true) && _index > 1)
                {
                    _pokemonAnimations[_index]._shakeV = 0;
                    _index -= 2;
                    _cursorDest = GetBoxPosition(_index);
                }
                if (Controls.Left(true) && _index > 0)
                {
                    _pokemonAnimations[_index]._shakeV = 0;
                    _index -= 1;
                    _cursorDest = GetBoxPosition(_index);
                }
                if (Controls.Right(true) && _index < PokemonList.Count - 1)
                {
                    _pokemonAnimations[_index]._shakeV = 0;
                    _index += 1;
                    _cursorDest = GetBoxPosition(_index);
                }

                Player.Temp.PokemonScreenIndex = _index;

                _cursorPosition.X = MathHelper.Lerp(_cursorDest.X, _cursorPosition.X, 0.8f);
                _cursorPosition.Y = MathHelper.Lerp(_cursorDest.Y, _cursorPosition.Y, 0.8f);

                if (TextBox.Showing == false && Controls.Accept())
                {
                    if (_isSwitching)
                    {
                        _isSwitching = false;

                        if (_switchIndex != _index)
                        {
                            Pokemon p1 = PokemonList[_switchIndex];
                            Pokemon p2 = PokemonList[_index];
                            SoundManager.PlaySound("Select");
                            PokemonList[_switchIndex] = p2;
                            PokemonList[_index] = p1;
                        }
                    }
                    else
                    {
                        _cursorPosition = _cursorDest;
                        CreateMainMenu();
                    }
                }

                if (TextBox.Showing == false && Controls.Dismiss() && CanExit)
                {
                    if (_isSwitching)
                    {
                        _isSwitching = false;
                    }
                    else
                    {
                        Selected = -1;
                        if (ExitedSub != null)
                        {
                            _used = true;
                            ExitedSub();
                        }
                        SoundManager.PlaySound("Select");
                        _closing = true;
                    }
                }
            }
        }
    }

    private void CreateMainMenu()
    {
        if (Mode == ISelectionScreen.ScreenMode.Default)
        {
            CreateNormalMenu(Localization.GetString("global_summary", "Summary"));
        }
        else if (Mode == ISelectionScreen.ScreenMode.Selection)
        {
            CreateSelectionMenu();
        }
    }

    private void CreateSelectionMenu()
    {
        List<String> items = [];
        items.Add(SelectButtonText);
        items.Add(Localization.GetString("global_summary", "Summary"));
        items.Add(Localization.GetString("global_back", "Back"));

        _menu = new SelectMenu(items, 0, SelectSelectionMenuItem, items.Count - 1);
    }

    private void SelectSelectionMenuItem(SelectMenu selectMenu)
    {
        if (selectMenu.SelectedItem == SelectButtonText)
        {
            if (CanChoosePokemon(PokemonList[_index]) == true)
            {
                Selected = _index;
                FireSelectionEvent(_index);
                GetPokemonList();
                _closing = true;
            }
            else
            {
                TextBox.Show(Localization.GetString("party_screen_CannotChoosePokemon", "Cannot choose this~Pokémon."));
            }
        }
        else if (selectMenu.SelectedItem == Localization.GetString("global_summary", "Summary"))
        {
            Core.SetScreen(new SummaryScreen(this, PokemonList.ToArray(), _index));
        }
    }

    private bool CanChoosePokemon(Pokemon p)
    {
        if (CanChooseFainted == false)
        {
            if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
            {
                return false;
            }
        }
        if (CanChooseEgg == false)
        {
            if (p.IsEgg == true)
            {
                return false;
            }
        }
        if (CanChooseHMPokemon == false)
        {
            if (p.HasHMMove() == true)
            {
                return false;
            }
        }
        if (CanChooseFusedPokemon == false)
        {
            if (p.OriginalNumber == 646)
            {
                if (p.AdditionalData == "black" || p.AdditionalData == "white")
                {
                    return false;
                }
            }
        }
        if (CannotChooseIndex != -1)
        {
            if (_index == CannotChooseIndex)
            {
                return false;
            }
        }
        return true;
    }

    private void CreateNormalMenu(String selectedItem)
    {
        Pokemon p = PokemonList[_index];

        List<String> items = [];
        items.Add(Localization.GetString("global_summary", "Summary"));

        if (p.IsEgg == false)
        {
            if (CanUseMove(p, 19, Badge.HMMoves.Fly) ||
                CanUseMove(p, 560, Badge.HMMoves.Ride) ||
                CanUseMove(p, 148, Badge.HMMoves.Flash) ||
                CanUseMove(p, 15, Badge.HMMoves.Cut) ||
                CanUseMove(p, 208, -1) ||
                CanUseMove(p, 230, -1) ||
                CanUseMove(p, 100, -1) ||
                CanUseMove(p, 91, -1))
            {
                items.Add(Localization.GetString("party_screen_FieldMove", "Field Move"));
            }
        }

        if (Screen.Level.IsBugCatchingContest == false)
        {
            items.Add(Localization.GetString("global_switch", "Switch"));
        }

        if (p.IsEgg == false)
        {
            items.Add(Localization.GetString("global_item", "Item"));
        }

        items.Add(Localization.GetString("global_back", "Back"));

        _menu = new SelectMenu(items, items.IndexOf(selectedItem), SelectedMainMenuItem, items.Count - 1);
    }

    private void CreateFieldMoveMenu()
    {
        Pokemon p = PokemonList[_index];

        List<String> items = [];
        if (CanUseMove(p, 19, Badge.HMMoves.Fly))
            items.Add(Localization.GetString("global_pokemon_move_fly", "Fly"));
        if (CanUseMove(p, 560, Badge.HMMoves.Ride))
            items.Add(Localization.GetString("global_pokemon_move_ride", "Ride"));
        if (CanUseMove(p, 148, Badge.HMMoves.Flash))
            items.Add(Localization.GetString("global_pokemon_move_flash", "Flash"));
        if (CanUseMove(p, 15, Badge.HMMoves.Cut))
            items.Add(Localization.GetString("global_pokemon_move_cut", "Cut"));
        if (CanUseMove(p, 100, -1))
            items.Add(Localization.GetString("global_pokemon_move_teleport", "Teleport"));
        if (CanUseMove(p, 91, -1))
            items.Add(Localization.GetString("global_pokemon_move_dig", "Dig"));
        if (World.GetWeatherFromWeatherType(Screen.Level.WeatherType) == World.Weathers.Clear || GameController.IS_DEBUG_ACTIVE || Core.Player.SandBoxMode == true)
        {
            if (CanUseMove(p, 230, -1))
                items.Add(Localization.GetString("global_pokemon_move_sweetscent", "Sweet Scent"));
        }
        if (CanUseMove(p, 208, -1) && p.MaxHP > 1 && p.HP > Math.Ceiling(p.MaxHP * 0.2))
            items.Add(Localization.GetString("global_pokemon_move_milkdrink", "Milk Drink"));
        if (CanUseMove(p, 135, -1) && p.MaxHP > 1 && p.HP > Math.Ceiling(p.MaxHP * 0.2))
            items.Add(Localization.GetString("global_pokemon_move_softboiled", "Soft-Boiled"));

        items.Add(Localization.GetString("global_back", "Back"));

        _menu = new SelectMenu(items, 0, SelectedFieldMoveMenuItem, items.Count - 1);
    }

    private void CreateItemMenu()
    {
        Pokemon p = PokemonList[_index];

        List<String> items = [];
        items.Add(Localization.GetString("global_give", "Give"));
        if (p.Item != null)
        {
            items.Add(Localization.GetString("global_take", "Take"));
        }
        items.Add(Localization.GetString("global_back", "Back"));

        _menu = new SelectMenu(items, 0, SelectedItemMenuItem, items.Count - 1);
    }

    private bool CanUseMove(Pokemon p, int moveID, int hmMove)
    {
        if (GameController.IS_DEBUG_ACTIVE || Core.Player.SandBoxMode == true)
        {
            return true;
        }
        if (p.IsEgg == false)
        {
            if (hmMove > -1)
            {
                if (Badge.CanUseHMMove((Badge.HMMoves)hmMove) == false)
                {
                    return false;
                }
            }
            foreach (Attack a in p.Attacks)
            {
                if (a.Name.ToLower() == Attack.GetAttackByID(moveID).Name.ToLower())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CanUseMove(Pokemon p, int moveID, Badge.HMMoves hmMove)
    {
        return CanUseMove(p, moveID, (int)hmMove);
    }

    private void SelectedMainMenuItem(SelectMenu selectMenu)
    {
        if (selectMenu.SelectedItem == Localization.GetString("global_summary", "Summary"))
        {
            Core.SetScreen(new SummaryScreen(this, PokemonList.ToArray(), _index));
        }
        else if (selectMenu.SelectedItem == Localization.GetString("party_screen_FieldMove", "Field Move"))
        {
            CreateFieldMoveMenu();
        }
        else if (selectMenu.SelectedItem == Localization.GetString("global_switch", "Switch"))
        {
            _switchIndex = _index;
            _isSwitching = true;
        }
        else if (selectMenu.SelectedItem == Localization.GetString("global_item", "Item"))
        {
            CreateItemMenu();
        }
    }

    private void SelectedFieldMoveMenuItem(SelectMenu selectMenu)
    {
        if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_fly", "Fly"))
            UseFly();
        else if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_ride", "Ride"))
            UseRide();
        else if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_flash", "Flash"))
            UseFlash();
        else if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_cut", "Cut"))
            UseCut();
        else if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_teleport", "Teleport"))
            UseTeleport();
        else if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_dig", "Dig"))
            UseDig();
        else if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_sweetscent", "Sweet Scent"))
            UseSweetScent();
        else if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_milkdrink", "Milk Drink"))
            UseMilkDrink();
        else if (selectMenu.SelectedItem == Localization.GetString("global_pokemon_move_softboiled", "Soft-Boiled"))
            UseSoftboiled();
        else if (selectMenu.SelectedItem == Localization.GetString("global_back", "Back"))
            CreateNormalMenu(Localization.GetString("party_screen_FieldMove", "Field Move"));
    }

    private void SelectedItemMenuItem(SelectMenu selectMenu)
    {
        if (selectMenu.SelectedItem == Localization.GetString("global_give", "Give"))
        {
            NewInventoryScreen selScreen = new NewInventoryScreen(Core.CurrentScreen);
            selScreen.Mode = ISelectionScreen.ScreenMode.Selection;
            selScreen.CanExit = true;
            selScreen.SelectedObject += GiveItemHandler;
            Core.SetScreen(selScreen);
        }
        else if (selectMenu.SelectedItem == Localization.GetString("global_take", "Take"))
        {
            Pokemon p = PokemonList[_index];

            if (p.Item!.IsMail && p.Item.AdditionalData != String.Empty)
            {
                if (p.Item.IsGameModeItem == false)
                {
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MailSystemScreen(Core.CurrentScreen, (Items.MailItem)p.Item), Color.Black, false));
                }
                else
                {
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MailSystemScreen(Core.CurrentScreen, (Items.GameModeItem)p.Item), Color.Black, false));
                }
                p.Item = null;
            }
            else
            {
                ShowMessage(Localization.GetString("party_screen_TakenItemFromPokemon", "Taken [ITEM]~from [POKEMONNAME].").Replace("[ITEM]", p.Item!.OneLineName()).Replace("[POKEMONNAME]", p.GetDisplayName()));
                String itemID;
                if (p.Item.IsGameModeItem)
                {
                    itemID = ((Items.GameModeItem)p.Item).gmID;
                }
                else
                {
                    itemID = p.Item.ID.ToString();
                }
                Core.Player.Inventory.AddItem(itemID, 1);
                p.Item = null;
            }
        }
        else if (selectMenu.SelectedItem == Localization.GetString("global_back", "Back"))
        {
            CreateNormalMenu(Localization.GetString("global_item", "Item"));
        }
    }

    private void GiveItemHandler(Object[] args)
    {
        GiveItem(args[0].ToString()!);
    }

    private void GiveItem(String itemID)
    {
        Items.Item i = Items.Item.GetItemByID(itemID);

        if (i.CanBeHeld)
        {
            Pokemon p = PokemonList[_index];
            Core.Player.Inventory.RemoveItem(itemID, 1);

            String message = String.Empty;

            Items.Item? reItem = p.Item;
            if (reItem != null)
            {
                if (reItem.IsMail && reItem.AdditionalData != String.Empty)
                {
                    Core.Player.Mails.Add(Items.MailItem.GetMailDataFromString(reItem.AdditionalData));
                    message = Localization.GetString("inventory_screen_GiveItem_TakeMail", "Gave <newitem> to <name> and took the Mail to the PC.").Replace("[NEWITEM]", i.OneLineName()).Replace("[POKEMONNAME]", p.GetDisplayName());
                }
                else
                {
                    String reItemID;
                    if (reItem.IsGameModeItem)
                    {
                        reItemID = ((Items.GameModeItem)reItem).gmID;
                    }
                    else
                    {
                        reItemID = reItem.ID.ToString();
                    }
                    Core.Player.Inventory.AddItem(reItemID, 1);
                    message = Localization.GetString("inventory_screen_GiveItem_Switch", "Switched <name>'s <olditem> with the <newitem>.").Replace("[NEWITEM]", i.OneLineName()).Replace("[OLDITEM]", reItem.OneLineName()).Replace("[POKEMONNAME]", p.GetDisplayName());
                }
            }
            else
            {
                message = Localization.GetString("inventory_screen_GiveItem_Give", "Gave <name> the <newitem>.").Replace("[POKEMONNAME]", p.GetDisplayName()).Replace("[NEWITEM]", i.OneLineName());
            }

            p.Item = i;
            ShowMessage(message);
        }
        else
        {
            ShowMessage(Localization.GetString("inventory_screen_CannotGiveToPokemon", "<newitem> cannot be given to a Pokémon.").Replace("[NEWITEM]", i.OneLineName()));
        }
    }

    public void ShowMessage(String text)
    {
        _messageDelay = (float)(text.Length / 1.75);
        _messageText = text;
    }

    public override void SizeChanged()
    {
        _cursorDest = GetBoxPosition(_index);
        _cursorPosition = _cursorDest;
    }

    #region Emblems

    private void CheckForLegendaryEmblem()
    {
        bool hasHoOh = false;
        bool hasLugia = false;
        bool hasSuicune = false;

        foreach (Pokemon p in PokemonList)
        {
            switch (p.Number)
            {
                case 245: hasSuicune = true; break;
                case 249: hasLugia = true; break;
                case 250: hasHoOh = true; break;
            }
        }

        if (hasSuicune && hasLugia && hasHoOh)
        {
            GameJolt.Emblem.AchieveEmblem("legendary");
        }
    }

    private void CheckForUnoDosTresEmblem()
    {
        bool hasArticuno = false;
        bool hasZapdos = false;
        bool hasMoltres = false;

        foreach (Pokemon p in PokemonList)
        {
            switch (p.Number)
            {
                case 144: hasArticuno = true; break;
                case 145: hasZapdos = true; break;
                case 146: hasMoltres = true; break;
            }
        }

        if (hasArticuno && hasZapdos && hasMoltres)
        {
            GameJolt.Emblem.AchieveEmblem("unodostres");
        }
    }

    private void CheckForBeastEmblem()
    {
        bool hasRaikou = false;
        bool hasEntei = false;
        bool hasSuicune2 = false;

        foreach (Pokemon p in PokemonList)
        {
            switch (p.Number)
            {
                case 243: hasRaikou = true; break;
                case 244: hasEntei = true; break;
                case 245: hasSuicune2 = true; break;
            }
        }

        if (hasRaikou && hasEntei && hasSuicune2)
        {
            GameJolt.Emblem.AchieveEmblem("beast");
        }
    }

    private void CheckForOverkillEmblem()
    {
        if (PokemonList.Count == 6)
        {
            bool has100 = true;
            for (int i = 0; i <= 5; i++)
            {
                if (PokemonList[i].Level < 100)
                {
                    has100 = false;
                    break;
                }
            }
            if (has100)
            {
                GameJolt.Emblem.AchieveEmblem("overkill");
            }
        }
    }

    #endregion

    #region Field Moves

    private void UseFlash()
    {
        Screen sc = Core.CurrentScreen;
        while (sc.Identification != Identifications.OverworldScreen && sc.PreScreen != null)
        {
            sc = sc.PreScreen;
        }
        ChooseBox.Showing = false;
        Core.SetScreen(sc);

        if (Screen.Level.IsDark == true)
        {
            String s = "version=2" + Environment.NewLine +
                       "@text.show(" + PokemonList[_index].GetDisplayName() + " <system.token(fieldmove_flash_used)>)" + Environment.NewLine +
                       "@environment.toggledarkness" + Environment.NewLine +
                       "@sound.play(FieldMove_Flash)" + Environment.NewLine +
                       "@text.show(<system.token(fieldmove_flash_AreaLitUp)>)" + Environment.NewLine +
                       ":end";
            PlayerStatistics.Track("Flash used", 1);
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2);
        }
        else
        {
            String s = "version=2" + Environment.NewLine +
                "@text.show(" + PokemonList[_index].GetDisplayName() + " <system.token(fieldmove_flash_used)>)" + Environment.NewLine +
                "@sound.play(FieldMove_Flash)" + Environment.NewLine +
                "@text.show(<system.token(fieldmove_flash_AlreadyLitUp)>)" + Environment.NewLine +
                ":end";
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2);
        }
    }

    private void UseFly()
    {
        Screen sc = Core.CurrentScreen;
        while (sc.Identification != Identifications.OverworldScreen && sc.PreScreen != null)
        {
            sc = sc.PreScreen;
        }
        ChooseBox.Showing = false;
        Core.SetScreen(sc);

        if (Level.CanFly == true || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            if (Screen.Level.CurrentRegion.Contains(",") == true)
            {
                List<String> regions = Screen.Level.CurrentRegion.Split(',').ToList();
                Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, regions, 0, ["Fly", PokemonList[_index]]), Color.White, false));
            }
            else
            {
                String startRegion = Screen.Level.CurrentRegion;
                Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, startRegion, ["Fly", PokemonList[_index]]), Color.White, false));
            }
        }
        else
        {
            TextBox.Show(Localization.GetString("fieldmove_fly_CannotUse", "You cannot Fly~from here!"), [], true, false);
        }
    }

    private void UseCut()
    {
        Screen sc = Core.CurrentScreen;
        while (sc.Identification != Identifications.OverworldScreen && sc.PreScreen != null)
        {
            sc = sc.PreScreen;
        }
        ChooseBox.Showing = false;
        Core.SetScreen(sc);

        List<Entity> grassEntities = Grass.GetGrassTilesAroundPlayer(2.4f);
        if (grassEntities.Count > 0)
        {
            PlayerStatistics.Track("Cut used", 1);
            TextBox.Show(PokemonList[_index].GetDisplayName() + " " + Localization.GetString("fieldmove_cut_used", "used~Cut!"), [], true, false);
            PokemonList[_index].PlayCry();
            foreach (Entity e in grassEntities)
            {
                Screen.Level.Entities.Remove(e);
            }
        }
        else
        {
            TextBox.Show(Localization.GetString("fieldmove_cut_NothingToCut", "There is nothing~to be Cut!"), [], true, false);
        }
    }

    private void UseRide()
    {
        if (Screen.Level.Riding == true)
        {
            if (Screen.Level.RideType == 3)
            {
                TextBox.Show(Localization.GetString("fieldmove_ride_cannot_walk", "You cannot walk here!"), [], true, false);
            }
            else
            {
                Screen.Level.Riding = false;
                Screen.Level.OwnPlayer.SetTexture(Core.Player.TempRideSkin, true);
                Core.Player.Skin = Core.Player.TempRideSkin;

                Screen sc = Core.CurrentScreen;
                while (sc.Identification != Identifications.OverworldScreen && sc.PreScreen != null)
                {
                    sc = sc.PreScreen;
                }
                ChooseBox.Showing = false;
                Core.SetScreen(sc);

                if (Screen.Level.IsRadioOn == false || GameJolt.PokegearScreen.StationCanPlay(Screen.Level.SelectedRadioStation) == false)
                {
                    MusicManager.Play(Level.MusicLoop, true, 0.01f);
                }
            }
        }
        else
        {
            if (Screen.Level.Surfing == false && Screen.Camera.IsMoving == false && Screen.Camera.Turning == false && Level.CanRide() == true)
            {
                Screen sc = Core.CurrentScreen;
                while (sc.Identification != Identifications.OverworldScreen && sc.PreScreen != null)
                {
                    sc = sc.PreScreen;
                }
                ChooseBox.Showing = false;
                Core.SetScreen(sc);

                Screen.Level.Riding = true;
                Core.Player.TempRideSkin = Core.Player.Skin;

                String skin = "[POKEMON|";
                if (PokemonList[_index].IsShiny == true)
                {
                    skin += "S]";
                }
                else
                {
                    skin += "N]";
                }
                skin += PokemonList[_index].Number + PokemonForms.GetOverworldAddition(PokemonList[_index]);

                Screen.Level.OwnPlayer.SetTexture(skin, false);

                SoundManager.PlayPokemonCry(PokemonList[_index].Number, PokemonForms.GetCrySuffix(PokemonList[_index]));

                TextBox.Show(PokemonList[_index].GetDisplayName() + " " + Localization.GetString("fieldmove_ride_used", "used~Ride!"), [], true, false);
                PlayerStatistics.Track("Ride used", 1);

                if (Screen.Level.IsRadioOn == false || GameJolt.PokegearScreen.StationCanPlay(Screen.Level.SelectedRadioStation) == false)
                {
                    MusicManager.Play("ride", true, 0.01f);
                }
            }
            else
            {
                Screen sc = Core.CurrentScreen;
                while (sc.Identification != Identifications.OverworldScreen && sc.PreScreen != null)
                {
                    sc = sc.PreScreen;
                }
                ChooseBox.Showing = false;
                Core.SetScreen(sc);

                TextBox.Show(Localization.GetString("fieldmove_ride_cannot_ride", "You cannot Ride here!"), [], true, false);
            }
        }
    }

    private void UseDig()
    {
        Screen sc = Core.CurrentScreen;
        while (sc.Identification != Identifications.OverworldScreen && sc.PreScreen != null)
        {
            sc = sc.PreScreen;
        }
        ChooseBox.Showing = false;
        Core.SetScreen(sc);

        if (Screen.Level.CanDig == true || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            bool setToFirstPerson = !((OverworldCamera)Screen.Camera).ThirdPerson;

            String s = "version=2\n" +
                "@text.show(" + PokemonList[_index].GetDisplayName() + " <system.token(fieldmove_dig_used)>)\n" +
                "@level.wait(20)\n" +
                "@camera.activatethirdperson\n" +
                "@camera.reset\n" +
                "@camera.fix\n" +
                "@player.turnto(0)\n" +
                "@sound.play(destroy)\n" +
                ":while:<player.position(y)>>" + (Screen.Camera.Position.Y - 1.4).ToString().ReplaceDecSeparator() + "\n" +
                "@player.turn(1)\n" +
                "@player.warp(~,~-0.1,~)\n" +
                "@level.wait(1)\n" +
                ":endwhile\n" +
                "@screen.fadeout\n" +
                "@camera.defix\n" +
                "@player.warp(" + Core.Player.LastRestPlace + "," + Core.Player.LastRestPlacePosition + ",0)\n" +
                "@player.turnto(2)";

            if (setToFirstPerson == true)
            {
                s += Environment.NewLine + "@camera.deactivatethirdperson";
            }
            s += Environment.NewLine +
                "@level.update\n" +
                "@screen.fadein\n" +
                ":end";

            PlayerStatistics.Track("Dig used", 1);
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2);
            if (Screen.Level.Surfing == true)
            {
                Screen.Level.Surfing = false;
                Screen.Level.OwnPlayer.SetTexture(Core.Player.TempSurfSkin, true);
                Core.Player.Skin = Core.Player.TempSurfSkin;

                Screen.Level.OverworldPokemon.warped = true;
                Screen.Level.OverworldPokemon.Visible = false;
            }
        }
        else
        {
            TextBox.Show(Localization.GetString("fieldmove_dig_CannotUse", "Cannot use Dig here."), [], true, false);
        }
    }

    private void UseMilkDrink()
    {
        FieldMovePokemonIndex = _index;

        Screen s = Core.CurrentScreen;
        while (s.Identification != Identifications.OverworldScreen && s.PreScreen != null)
        {
            s = s.PreScreen;
        }
        ChooseBox.Showing = false;
        Core.SetScreen(s);

        PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, Items.Item.GetItemByID("5"), null, Localization.GetString("fieldmove_milkdrink_softboiled_ChoosePokemon", "Choose Pokémon to Heal"), true, false, false)
        {
            Mode = ISelectionScreen.ScreenMode.Selection,
            CanExit = true
        };
        selScreen.SelectedObject += UseMilkDrinkHandler;
        Core.SetScreen(selScreen);
    }

    private void UseMilkDrinkHandler(Object[] args)
    {
        UseMilkDrinkOnPokemon((int)args[0]);
    }

    private void UseMilkDrinkOnPokemon(int healIndex)
    {
        if (healIndex != FieldMovePokemonIndex)
        {
            if (Core.Player.Pokemons[healIndex].HP < Core.Player.Pokemons[healIndex].MaxHP)
            {
                SoundManager.PlaySound("Use_Item", false);
                TextBox.Show(Core.Player.Pokemons[FieldMovePokemonIndex].GetDisplayName() + " " + Localization.GetString("fieldmove_milkdrink_used", "used~Milk Drink!*Some HP was shared~with [POKEMONNAME]!").Replace("[POKEMONNAME]", Core.Player.Pokemons[healIndex].GetDisplayName()));
                int healHP = (int)Math.Ceiling(Core.Player.Pokemons[FieldMovePokemonIndex].MaxHP * 0.2);
                Core.Player.Pokemons[FieldMovePokemonIndex].HP -= healHP;
                Core.Player.Pokemons[healIndex].HP += healHP;
                if (Core.Player.Pokemons[healIndex].HP > Core.Player.Pokemons[healIndex].MaxHP)
                {
                    Core.Player.Pokemons[healIndex].HP = Core.Player.Pokemons[healIndex].MaxHP;
                }
            }
            else
            {
                TextBox.Show(Localization.GetString("fieldmove_milkdrink_softboiled_CannotChoose_FullHP", "[POKEMONNAME] has full~HP already.").Replace("[POKEMONNAME]", Core.Player.Pokemons[healIndex].GetDisplayName()), [], true, false);
            }
        }
        else
        {
            TextBox.Show(Localization.GetString("fieldmove_milkdrink_softboiled_CannotChoose_SamePokemon", "[POKEMONNAME] cannot~heal itself.").Replace("[POKEMONNAME]", Core.Player.Pokemons[FieldMovePokemonIndex].GetDisplayName()), [], true, false);
        }
        FieldMovePokemonIndex = -1;
    }

    private void UseSoftboiled()
    {
        FieldMovePokemonIndex = _index;

        Screen s = Core.CurrentScreen;
        while (s.Identification != Identifications.OverworldScreen && s.PreScreen != null)
        {
            s = s.PreScreen;
        }
        ChooseBox.Showing = false;
        Core.SetScreen(s);

        PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, Items.Item.GetItemByID("5"), null, Localization.GetString("fieldmove_milkdrink_softboiled_ChoosePokemon", "Choose Pokémon to Heal"), true, false, false)
        {
            Mode = ISelectionScreen.ScreenMode.Selection,
            CanExit = true
        };
        selScreen.SelectedObject += UseSoftboiledHandler;
        Core.SetScreen(selScreen);
    }

    private void UseSoftboiledHandler(Object[] args)
    {
        UseSoftboiledOnPokemon((int)args[0]);
    }

    private void UseSoftboiledOnPokemon(int healIndex)
    {
        if (healIndex != FieldMovePokemonIndex)
        {
            if (Core.Player.Pokemons[healIndex].HP < Core.Player.Pokemons[healIndex].MaxHP)
            {
                SoundManager.PlaySound("Use_Item", false);
                TextBox.Show(Core.Player.Pokemons[FieldMovePokemonIndex].GetDisplayName() + " " + Localization.GetString("fieldmove_softboiled_used", "used~Soft-Boiled!*Some HP was shared~with [POKEMONNAME]!").Replace("[POKEMONNAME]", Core.Player.Pokemons[healIndex].GetDisplayName()));
                int healHP = (int)Math.Ceiling(Core.Player.Pokemons[FieldMovePokemonIndex].MaxHP * 0.2);
                Core.Player.Pokemons[FieldMovePokemonIndex].HP -= healHP;
                Core.Player.Pokemons[healIndex].HP += healHP;
                if (Core.Player.Pokemons[healIndex].HP > Core.Player.Pokemons[healIndex].MaxHP)
                {
                    Core.Player.Pokemons[healIndex].HP = Core.Player.Pokemons[healIndex].MaxHP;
                }
            }
            else
            {
                TextBox.Show(Localization.GetString("fieldmove_milkdrink_softboiled_CannotChoose_FullHP", "[POKEMONNAME] has full~HP already.").Replace("[POKEMONNAME]", Core.Player.Pokemons[healIndex].GetDisplayName()), [], true, false);
            }
        }
        else
        {
            TextBox.Show(Localization.GetString("fieldmove_milkdrink_softboiled_CannotChoose_SamePokemon", "[POKEMONNAME] cannot~heal itself.").Replace("[POKEMONNAME]", Core.Player.Pokemons[FieldMovePokemonIndex].GetDisplayName()), [], true, false);
        }
        FieldMovePokemonIndex = -1;
    }

    private void UseSweetScent()
    {
        Screen s = Core.CurrentScreen;
        while (s.Identification != Identifications.OverworldScreen && s.PreScreen != null)
        {
            s = s.PreScreen;
        }
        ChooseBox.Showing = false;
        Core.SetScreen(s);

        String pokeFilePath = GameModeManager.GetPokeFilePath(Screen.Level.LevelFile!.Remove(Screen.Level.LevelFile.Length - 4, 4) + ".poke");
        if (System.IO.File.Exists(pokeFilePath) == true)
        {
            Screen.Level.WalkedSteps = 0;

            Screen.Level.PokemonEncounterData.Position = Screen.Level.OwnPlayer.Position;
            Screen.Level.PokemonEncounterData.EncounteredPokemon = true;
            if (Screen.Level.Surfing == true)
            {
                Screen.Level.PokemonEncounterData.Method = Spawner.EncounterMethods.Surfing;
            }
            else
            {
                Screen.Level.PokemonEncounterData.Method = Spawner.EncounterMethods.Land;
            }

            Screen.Level.PokemonEncounterData.PokeFile = String.Empty;

            Pokemon? p = Spawner.GetPokemon(Screen.Level.LevelFile, Screen.Level.PokemonEncounterData.Method, true, String.Empty);
            if (p != null)
            {
                String scr = "version=2" + Environment.NewLine +
                    "@text.show(" + PokemonList[_index].GetDisplayName() + " <system.token(fieldmove_sweetscent_used)>)" + Environment.NewLine +
                    "@battle.wild(" + p.GetSaveData() + ")" + Environment.NewLine +
                    ":end";
                ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(scr, 2);
                Screen.Level.PokemonEncounterData.EncounteredPokemon = false;
            }
            else
            {
                TextBox.Show(Localization.GetString("fieldmove_sweetscent_CannotUse", "Cannot use Sweet Scent here."), [], true, false);
            }
        }
        else
        {
            TextBox.Show(Localization.GetString("fieldmove_sweetscent_CannotUse", "Cannot use Sweet Scent here."), [], true, false);
        }
    }

    private void UseTeleport()
    {
        Screen sc = Core.CurrentScreen;
        while (sc.Identification != Identifications.OverworldScreen && sc.PreScreen != null)
        {
            sc = sc.PreScreen;
        }
        ChooseBox.Showing = false;
        Core.SetScreen(sc);

        if (Screen.Level.CanTeleport == true || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            bool setToFirstPerson = !((OverworldCamera)Screen.Camera).ThirdPerson;

            String yFinish = (Screen.Camera.Position.Y + 2.9f).ToString().ReplaceDecSeparator();

            String s = "version=2\n" +
                "@text.show(" + PokemonList[_index].GetDisplayName() + " <system.token(fieldmove_teleport_used)>)\n" +
                "@level.wait(20)\n" +
                "@camera.activatethirdperson\n" +
                "@camera.reset\n" +
                "@camera.fix\n" +
                "@player.turnto(0)\n" +
                "@sound.play(teleport)\n" +
                ":while:<player.position(y)><" + yFinish + "\n" +
                "@player.turn(1)\n" +
                "@player.warp(~,~+0.1,~)\n" +
                "@level.wait(1)\n" +
                ":endwhile\n" +
                "@screen.fadeout\n" +
                "@camera.defix\n" +
                "@player.warp(" + Core.Player.LastRestPlace + "," + Core.Player.LastRestPlacePosition + ",0)\n" +
                "@player.turnto(2)";

            if (setToFirstPerson == true)
            {
                s += Environment.NewLine + "@camera.deactivatethirdperson";
            }
            s += Environment.NewLine +
                "@level.update\n" +
                "@screen.fadein\n" +
                ":end";

            PlayerStatistics.Track("Teleport used", 1);
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2);
            if (Screen.Level.Surfing == true)
            {
                Screen.Level.Surfing = false;
                Screen.Level.OwnPlayer.SetTexture(Core.Player.TempSurfSkin, true);
                Core.Player.Skin = Core.Player.TempSurfSkin;

                Screen.Level.OverworldPokemon.warped = true;
                Screen.Level.OverworldPokemon.Visible = false;
            }
        }
        else
        {
            TextBox.Show(Localization.GetString("fieldmove_teleport_CannotUse", "Cannot use Teleport here."), [], true, false);
        }
    }

    #endregion

    // ISelectionScreen implementation
    private ISelectionScreen.ScreenMode _mode = ISelectionScreen.ScreenMode.Default;
    private bool _canExit = true;

    public event Action<Object[]>? SelectedObject;

    private void FireSelectionEvent(int pokemonIndex)
    {
        SelectedObject?.Invoke(new Object[] { pokemonIndex });
    }

    public ISelectionScreen.ScreenMode Mode
    {
        get => _mode;
        set => _mode = value;
    }

    public bool CanExit
    {
        get => _canExit;
        set => _canExit = value;
    }

    public void SetupLearnAttack(Attack a, int learnType, Object? arg)
    {
        LearnAttack = a;
        LearnType = learnType;
        _moveLearnArg = arg;
    }
}
