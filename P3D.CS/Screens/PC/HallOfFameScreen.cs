using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;
using P3D.BattleSystem;

namespace P3D;

public class HallOfFameScreen : Screen
{
    public OverworldStorage SavedOverworld = null!;
    private int _alpha = 255;
    private int _alphaFade = -1;
    private bool _loadedLevel = false;
    private String _backgroundLevel = "indigo\\halloffame_interface.dat";

    private int _menuState = 0;
    private Texture2D _texture = null!;

    private List<HallOfFameEntry> _entries = [];
    private int _amountOfEntries = 0;

    private int _scroll = 0;
    private int _cursor = 0;
    private HallOfFameEntry? _selectedEntry;

    private int _preselect = -1;

    public static int TileOffset = 0;

    public class PokemonWrapper
    {
        private Pokemon? _pokemon;
        private String _pokemonData = String.Empty;
        private bool _loaded = false;

        public PokemonWrapper(String pokemonData) { _pokemonData = pokemonData; }
        public PokemonWrapper(Pokemon p) { _loaded = true; _pokemon = p; }

        public Pokemon GetPokemon()
        {
            if (_loaded == false) { _loaded = true; _pokemon = Pokemon.GetPokemonByData(_pokemonData); }
            return _pokemon!;
        }
    }

    public class HallOfFameEntry
    {
        public List<PokemonWrapper> PokemonList = [];
        public int ID = 0;
        public String Name = String.Empty;
        public String PlayTime = String.Empty;
        public String OT = String.Empty;
        public String Skin = String.Empty;
        public String Points = String.Empty;

        public HallOfFameEntry(int id)
        {
            ID = id;
            String[] data = Core.Player.HallOfFameData.SplitAtNewline();
            foreach (String l in data)
            {
                if (l.StartsWith(ID.ToString() + ",{") == true)
                {
                    String pokeData = l.Remove(0, l.IndexOf("{"));
                    PokemonList.Add(new PokemonWrapper(pokeData));
                }
                else if (l.StartsWith(ID.ToString() + ",(") == true)
                {
                    String[] playerData = l.Remove(l.Length - 1, 1).Remove(0, l.IndexOf("(") + 1).Split('|');
                    switch (playerData.Length)
                    {
                        case 4:
                            Name = playerData[0]; PlayTime = playerData[1]; Points = playerData[2]; Skin = playerData[3]; OT = "00000";
                            break;
                        case 5:
                            Name = playerData[0]; PlayTime = playerData[1]; Points = playerData[2]; OT = playerData[3]; Skin = playerData[4];
                            break;
                    }
                }
            }
        }
    }

    public HallOfFameScreen(Screen currentScreen)
        : this(currentScreen, "indigo\\halloffame_interface.dat") { }

    public HallOfFameScreen(Screen currentScreen, int preselect)
        : this(currentScreen, "indigo\\halloffame_interface.dat")
    {
        _preselect = preselect;
        _cursor = _preselect;
    }

    public HallOfFameScreen(Screen currentScreen, String backgroundLevel)
    {
        _backgroundLevel = backgroundLevel;
        SetupScreen(currentScreen);
    }

    public HallOfFameScreen(Screen currentScreen, int preselect, String backgroundLevel)
    {
        _backgroundLevel = backgroundLevel;
        SetupScreen(currentScreen);
        _preselect = preselect;
        _cursor = _preselect;
    }

    private void SetupScreen(Screen currentScreen)
    {
        Identification = Identifications.HallofFameScreen;
        PreScreen = currentScreen;
        CanBePaused = true;
        MouseVisible = true;

        SavedOverworld = new OverworldStorage();
        SavedOverworld.SetToCurrentEnvironment();

        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        LoadEntries();
    }

    private void LoadEntries()
    {
        List<int> ids = [];
        foreach (String line in Core.Player.HallOfFameData.SplitAtNewline())
        {
            if (line.Contains(",") == true)
            {
                String s = line.Remove(line.IndexOf(","));
                if (StringHelper.IsNumeric(s) == true)
                {
                    int id = int.Parse(s);
                    if (ids.Contains(id) == false)
                    {
                        ids.Add(id);
                        _amountOfEntries = id + 1;
                    }
                }
            }
        }
        foreach (int id in ids)
            _entries.Add(new HallOfFameEntry(id));
    }

    public override void ChangeTo()
    {
        if (_loadedLevel == false)
        {
            TextBox.Showing = false;
            PokemonImageView.Showing = false;
            ImageView.Showing = false;
            ChooseBox.Showing = false;

            Effect = new BasicEffectWithAlphaTest(Core.GraphicsDevice);
            Effect.FogEnabled = true;
            SkyDome = new SkyDome();
            Camera = new BattleCamera();

            Level = new Level();
            Level.Load(_backgroundLevel);

            ResetCamera();
            _loadedLevel = true;
        }
    }

    private void ResetCamera()
    {
        BattleCamera bCamera = (BattleCamera)Camera;
        bCamera.Position = new Vector3(10, 1, 14);
        bCamera.Yaw = 0.0F;
        bCamera.Pitch = -0.04F;
        bCamera.TargetPosition = new Vector3(10, 0.3F, 9);
        bCamera.TargetYaw = bCamera.Yaw;
        bCamera.TargetPitch = bCamera.Pitch;
    }

    public override void Draw()
    {
        Level.Draw();

        Color backColor = Screens.UI.ColorProvider.MainColor(false);
        Color gradientColor = Screens.UI.ColorProvider.AccentColor(false);
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(backColor, _alpha));

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 128, y + TileOffset, 128, 64), new Rectangle(48, 0, 16, 16), new Color(255, 255, 255, _alpha));

        Canvas.DrawGradient(new Rectangle(0, 0, (int)Core.windowSize.Width, 200), new Color(gradientColor, _alpha), new Color(gradientColor, 0), false, -1);
        Canvas.DrawGradient(new Rectangle(0, (int)(Core.windowSize.Height - 200), (int)Core.windowSize.Width, 200), new Color(gradientColor, 0), new Color(gradientColor, _alpha), false, -1);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("hall_of_fame_screen_title", "Hall of Fame"), new Vector2(104, 28), Color.Black, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("hall_of_fame_screen_title", "Hall of Fame"), new Vector2(100, 24), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

        if (_selectedEntry != null)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("hall_of_fame_screen_entry_number", "Entry No.") + " " + (_selectedEntry.ID + 1), new Vector2(-1096 + (255 - _alpha) * 5.0F, 74), Color.Black, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("hall_of_fame_screen_entry_number", "Entry No.") + " " + (_selectedEntry.ID + 1), new Vector2(-1100 + (255 - _alpha) * 5.0F, 70), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);
        }

        if (_preselect == -1)
        {
            for (int i = _scroll; i <= _scroll + 5; i++)
            {
                if (i <= _entries.Count - 1)
                {
                    int p = i - _scroll;
                    Core.SpriteBatch.Draw(_texture, new Rectangle(100, 100 + p * 96, 64, 64), new Rectangle(16, 16, 16, 16), new Color(255, 255, 255, _alpha));
                    Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64, 100 + p * 96, 64 * 8, 64), new Rectangle(32, 16, 16, 16), new Color(255, 255, 255, _alpha));
                    Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64 * 9, 100 + p * 96, 64, 64), new Rectangle(16, 16, 16, 16), new Color(255, 255, 255, _alpha), 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

                    Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("hall_of_fame_screen_entry_number", "Entry No.") + " " + (_entries[i].ID + 1), new Vector2(120, 116 + p * 96), new Color(0, 0, 0, _alpha), 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);

                    for (int d = 0; d <= _entries[i].PokemonList.Count - 1; d++)
                    {
                        Texture2D pokeTexture = _entries[i].PokemonList[d].GetPokemon().GetMenuTexture();
                        Vector2 pokeTextureScale = new Vector2((float)(32.0 / pokeTexture.Width), (float)(32.0 / pokeTexture.Height));
                        Core.SpriteBatch.Draw(pokeTexture, new Rectangle(360 + d * 40, 116 + p * 96, (int)(pokeTexture.Width * pokeTextureScale.X), (int)(pokeTexture.Height * pokeTextureScale.Y)), new Color(255, 255, 255, _alpha));
                    }
                }
            }

            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("hall_of_fame_screen_entries", "Entries:") + " ", new Vector2(90 + 64 * 11, 119), new Color(255, 255, 255, _alpha));
            Core.SpriteBatch.DrawString(FontManager.MainFont, _amountOfEntries.ToString(), new Vector2(190 + 64 * 11, 119), new Color(255, 255, 255, _alpha));

            DrawCursor();
        }

        if (_menuState == 2)
        {
            if (Camera.Name == "BattleV2")
            {
                if (((BattleCamera)Camera).IsReady == true)
                    DrawInformation();
            }
        }
    }

    private void DrawCursor()
    {
        Vector2 cPosition = new Vector2(520, 100 + _cursor * 96 - 42);
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle((int)cPosition.X, (int)cPosition.Y, 64, 64), new Color(255, 255, 255, _alpha));
    }

    private void DrawInformation()
    {
        int id = (int)Screen.Camera.Position.X switch
        {
            10 => -1,
            11 => 0,
            9 => 1,
            12 => 2,
            8 => 3,
            13 => 4,
            7 => 5,
            _ => -1
        };

        if (id == -1)
            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width - 500), 50, 450, 192), new Color(0, 0, 0, 150));
        else
            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width - 500), 50, 450, 216), new Color(0, 0, 0, 150));

        Vector2 pos = new Vector2((int)(Core.windowSize.Width - 500), 50);

        if (id == -1)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_player_name", "Player Name") + " " + _selectedEntry!.Name, new Vector2(pos.X + 4, pos.Y + 4), Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0.0F);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_play_time", "Play Time") + Environment.NewLine + Environment.NewLine + Localization.GetString("global_IDNo.", "ID No.") + Environment.NewLine + Environment.NewLine + Localization.GetString("global_points", "Points"), new Vector2(pos.X + 10, pos.Y + 54), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
            Core.SpriteBatch.DrawString(FontManager.MainFont, _selectedEntry.PlayTime + Environment.NewLine + Environment.NewLine + _selectedEntry.OT + Environment.NewLine + Environment.NewLine + _selectedEntry.Points, new Vector2(pos.X + 124, pos.Y + 55), new Color(173, 216, 230), 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
        }
        else
        {
            Pokemon p = _selectedEntry!.PokemonList[id].GetPokemon();
            Texture2D pokeTexture = p.GetMenuTexture();
            Vector2 pokeTextureScale = new Vector2((float)(32.0 / pokeTexture.Width), (float)(32.0 / pokeTexture.Height));
            Core.SpriteBatch.Draw(pokeTexture, new Rectangle((int)(pos.X + 4) - (int)((pokeTexture.Width - 32) / 2), (int)(pos.Y + 6), (int)(pokeTexture.Width * pokeTextureScale.X), (int)(pokeTexture.Height * pokeTextureScale.Y)), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, p.GetDisplayName(), new Vector2(pos.X + 48, pos.Y + 4), Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0.0F);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Level", "Level") + Environment.NewLine + Environment.NewLine + Localization.GetString("property_OT", "OT") + Environment.NewLine + Environment.NewLine + Localization.GetString("hall_of_fame_screen_type1", "Type 1") + Environment.NewLine + Environment.NewLine + Localization.GetString("hall_of_fame_screen_type2", "Type 2"), new Vector2(pos.X + 10, pos.Y + 43), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);

            String s = p.Level + Environment.NewLine + Environment.NewLine + p.OT + " / " + p.CatchTrainerName + Environment.NewLine + Environment.NewLine + p.Type1.ToString();
            if (p.Type2.Type != Element.Types.Blank)
                s += Environment.NewLine + Environment.NewLine + p.Type2.ToString();
            else
                s += Environment.NewLine + Environment.NewLine + Localization.GetString("global_none", "None");

            Core.SpriteBatch.DrawString(FontManager.MainFont, s, new Vector2(pos.X + 124, pos.Y + 44), new Color(173, 216, 230), 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
        }
    }

    public override void Update()
    {
        if (Screen.Effect != null)
        {
            BasicEffectWithAlphaTest eff = Screen.Effect;
            Lighting.UpdateLighting(ref eff);
            Screen.Effect = eff;
        }
        Camera.Update();
        Level.Update();

        switch (_menuState)
        {
            case 0: UpdateMenu(); break;
            case 1:
                _alpha += 3 * _alphaFade;
                if (_alpha >= 255) { _alpha = 255; _menuState = 0; }
                else if (_alpha <= 0) { _alpha = 0; _menuState = 2; }
                break;
            case 2: UpdateCamera(); break;
        }

        TileOffset += 1;
        if (TileOffset >= 64) TileOffset = 0;
    }

    private void UpdateMenu()
    {
        if (KeyBoardHandler.KeyPressed(KeyBindings.SpecialKey) == true || ControllerHandler.ButtonPressed(Buttons.Back) == true)
            _entries.Reverse();

        if (_preselect > -1)
        {
            SelectEntry(_preselect);
            return;
        }
        else if (_preselect == -2)
        {
            ChangeSavedScreen();
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, PreScreen!, Color.Black, false));
            return;
        }

        if (Controls.Down(true, true, true, true, true, true) == true)
        {
            _cursor += 1;
            if (Controls.ShiftDown() == true) _cursor += 4;
        }
        if (Controls.Up(true, true, true, true, true, true) == true)
        {
            _cursor -= 1;
            if (Controls.ShiftDown() == true) _cursor -= 4;
        }

        while (_cursor > 5) { _cursor -= 1; _scroll += 1; }
        while (_cursor < 0) { _cursor += 1; _scroll -= 1; }

        if (_entries.Count < 7)
            _scroll = 0;
        else
            _scroll = _scroll.Clamp(0, _entries.Count - 6);

        if (_entries.Count < 6)
            _cursor = _cursor.Clamp(0, _entries.Count - 1);
        else
            _cursor = _cursor.Clamp(0, 5);

        if (_entries.Count > 0)
        {
            if (Controls.Accept(true, false, false) == true)
            {
                for (int i = _scroll; i <= _scroll + 5; i++)
                {
                    if (i <= _entries.Count - 1)
                    {
                        if (new Rectangle(100, 100 + (i - _scroll) * 96, 640, 64).Contains(MouseHandler.MousePosition) == true)
                        {
                            if (i == _cursor + _scroll)
                                SelectEntry(_scroll + _cursor);
                            else
                                _cursor = i - _scroll;
                        }
                    }
                }
            }

            if (Controls.Accept(false, true, true) == true)
                SelectEntry(_scroll + _cursor);
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            ChangeSavedScreen();
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, PreScreen!, Color.Black, false));
        }
    }

    private void SelectEntry(int index)
    {
        for (int i = Screen.Level.Entities.Count - 1; i >= 0; i--)
        {
            Entity entity = Screen.Level.Entities[i];
            if (entity.EntityID == "NPC" && ((NPC)entity).NPCID == 9001)
                Screen.Level.Entities.RemoveAt(i);
        }

        int d = 1;
        int e = 0;
        foreach (PokemonWrapper pw in _entries[index].PokemonList)
        {
            int x = d;
            if (e == 1) { e = 0; x = -d; d += 1; }
            else { e = 1; }

            NPC n = (NPC)Entity.GetNewEntity("NPC", new Vector3(10 + x, 0, 7), [null], [0, 0], false, new Vector3(0), new Vector3(1), BaseModel.BillModel, 0, String.Empty, true, new Vector3(1), 1, String.Empty, String.Empty, new Vector3(0), new Object[] { PokemonForms.GetOverworldSpriteName(pw.GetPokemon(), false), 2, String.Empty, 9001, true, "Still", new List<Rectangle>() })!;
            Level.Entities.Add(n);
        }

        NPC playerNPC = (NPC)Entity.GetNewEntity("NPC", new Vector3(10, 0, 7), [null], [0, 0], false, new Vector3(0), new Vector3(1), BaseModel.BillModel, 0, String.Empty, true, new Vector3(1), 1, String.Empty, String.Empty, new Vector3(0), new Object[] { _entries[index].Skin, 2, String.Empty, 9001, false, "Still", new List<Rectangle>() })!;
        Level.Entities.Add(playerNPC);

        _selectedEntry = _entries[index];
        _menuState = 1;
        _alphaFade = -1;
        ResetCamera();
    }

    private void UpdateCamera()
    {
        BattleCamera bCamera = (BattleCamera)Screen.Camera;
        if (bCamera.IsReady == true)
        {
            int d = 1, e = 0, max = 0, min = 0;
            foreach (PokemonWrapper pw in _selectedEntry!.PokemonList)
            {
                int x = d;
                if (e == 1) { e = 0; x = -d; d += 1; }
                else { e = 1; }
                if (x < min) min = x;
                if (x > max) max = x;
            }
            min += 10;
            max += 10;

            if (Controls.Left(true, true, false, true, true, true) == true && (int)bCamera.Position.X > min)
            {
                Vector3 tp = bCamera.TargetPosition;
                tp.X = bCamera.Position.X - 1;
                bCamera.TargetPosition = tp;
            }
            if (Controls.Right(true, true, false, true, true, true) == true && (int)bCamera.Position.X < max)
            {
                Vector3 tp = bCamera.TargetPosition;
                tp.X = bCamera.Position.X + 1;
                bCamera.TargetPosition = tp;
            }
        }

        if (Controls.Dismiss() == true)
        {
            _menuState = 1;
            _alphaFade = 1;
            bCamera.TargetPosition = new Vector3(10, 1, 15);
            if (_preselect > -1) _preselect = -2;
        }
    }

    public void ChangeSavedScreen()
    {
        Screen.Level = SavedOverworld.Level;
        Screen.Camera = SavedOverworld.Camera;
        Screen.Effect = SavedOverworld.Effect;
        Screen.SkyDome = SavedOverworld.SkyDome;
        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
    }

    public static int GetHallOfFameCount()
    {
        int count = -1;
        if (Core.Player.HallOfFameData != String.Empty)
        {
            String[] data = Core.Player.HallOfFameData.SplitAtNewline();
            foreach (String l in data)
            {
                if (l.Contains(",") == false) continue;
                String idStr = l.Remove(l.IndexOf(","));
                if (StringHelper.IsNumeric(idStr) == false) continue;
                int id = int.Parse(idStr);
                if (id > count) count = id;
            }
        }
        count += 1;
        if (count > 21) count = 21;
        return count;
    }
}
