using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class PokedexSelectScreen : Screen
{
    private Texture2D _texture;
    private List<PokedexProfile> _profiles = [];
    private int _cursor = 0;

    public struct PokedexProfile
    {
        public int Obtained;
        public int Seen;
        public Pokedex Pokedex;
    }

    public PokedexSelectScreen(Screen currentScreen)
    {
        Identification = Identifications.PokedexSelectScreen;
        PreScreen = currentScreen;
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        MouseVisible = true;
        CanMuteAudio = true;
        CanBePaused = true;

        foreach (Pokedex p in Core.Player.Pokedexes)
        {
            if (p.IsActivated == true)
                _profiles.Add(new PokedexProfile { Pokedex = p, Obtained = p.Obtained, Seen = p.Seen });
        }

        AchievePokedexEmblems();
    }

    private void AchievePokedexEmblems()
    {
        int[] eevee = [134, 135, 136, 196, 197, 470, 471, 700];
        bool hasEevee = true;
        foreach (int e in eevee)
        {
            if (Pokedex.GetEntryType(Core.Player.PokedexData, e.ToString()) < 2)
            {
                hasEevee = false;
                break;
            }
        }
        if (hasEevee == true) GameJolt.Emblem.AchieveEmblem("eevee");

        if (Core.Player.IsGameJoltSave == true)
        {
            if (_profiles.Count > 0 && _profiles[0].Obtained >= _profiles[0].Pokedex.Count)
                GameJolt.Emblem.AchieveEmblem("pokedex");
        }
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, new Color(84, 198, 216));

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 128, y + PokedexScreen.tileOffset, 128, 64), new Rectangle(48, 0, 16, 16), Color.White);

        Canvas.DrawGradient(new Rectangle(0, 0, Core.windowSize.Width, 200), new Color(42, 167, 198), new Color(42, 167, 198, 0), false, -1);
        Canvas.DrawGradient(new Rectangle(0, Core.windowSize.Height - 200, Core.windowSize.Width, 200), new Color(42, 167, 198, 0), new Color(42, 167, 198), false, -1);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokedex_select", "Select a Pokédex"), new Vector2(100, 24), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

        for (int i = 0; i <= _profiles.Count; i++)
        {
            if (i == _profiles.Count)
            {
                Core.SpriteBatch.Draw(_texture, new Rectangle(100, 100 + i * 96, 64, 64), new Rectangle(16, 16, 16, 16), Color.White);
                Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64, 100 + i * 96, 64 * 5, 64), new Rectangle(32, 16, 16, 16), Color.White);
                Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64 * 6, 100 + i * 96, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);
                Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokedex_habitat_dex_name", "Habitat-Dex"), new Vector2(120, 120 + i * 96), Color.Black);
            }
            else
            {
                Pokedex p = _profiles[i].Pokedex;
                Core.SpriteBatch.Draw(_texture, new Rectangle(100, 100 + i * 96, 64, 64), new Rectangle(16, 16, 16, 16), Color.White);
                Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64, 100 + i * 96, 64 * 5, 64), new Rectangle(32, 16, 16, 16), Color.White);
                Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64 * 6, 100 + i * 96, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);
                Core.SpriteBatch.DrawString(FontManager.MainFont, p.Name, new Vector2(120, 120 + i * 96), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MainFont, _profiles[i].Obtained.ToString(), new Vector2(460, 120 + i * 96), Color.Black);

                if (_profiles[i].Obtained >= _profiles[i].Pokedex.Count)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(160, 160, 10, 10), String.Empty), new Rectangle(430, 122 + i * 96, 20, 20), Color.White);
                else if (_profiles[i].Seen + _profiles[i].Obtained >= _profiles[i].Pokedex.Count)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(160, 170, 10, 10), String.Empty), new Rectangle(430, 122 + i * 96, 20, 20), Color.White);
            }
        }

        DrawCursor();
    }

    private void DrawCursor()
    {
        Vector2 cPosition = new Vector2(512, 96 + _cursor * 96 - 40);
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle((int)cPosition.X, (int)cPosition.Y, 64, 64), Color.White);
    }

    public override void Update()
    {
        if (Controls.Up(true, true, true, true, true, true) == true)
        {
            _cursor -= 1;
            if (Controls.ShiftDown() == true) _cursor -= 4;
        }
        if (Controls.Down(true, true, true, true, true, true) == true)
        {
            _cursor += 1;
            if (Controls.ShiftDown() == true) _cursor += 4;
        }
        _cursor = Math.Clamp(_cursor, 0, _profiles.Count);

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = 0; i <= _profiles.Count; i++)
            {
                if (new Rectangle(100, 100 + i * 96, 64 * 7, 64).Contains(MouseHandler.MousePosition) == false) continue;
                if (i == _cursor)
                {
                    SoundManager.PlaySound("select");
                    if (_cursor == _profiles.Count) Core.SetScreen(new PokedexHabitatScreen(this));
                    else Core.SetScreen(new PokedexScreen(this, _profiles[_cursor], null));
                }
                else { _cursor = i; }
            }
        }

        if (Controls.Accept(false, true, true) == true)
        {
            SoundManager.PlaySound("select");
            if (_cursor == _profiles.Count) Core.SetScreen(new PokedexHabitatScreen(this));
            else Core.SetScreen(new PokedexScreen(this, _profiles[_cursor], null));
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            SoundManager.PlaySound("select");
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, PreScreen!, Color.White, false));
        }

        PokedexScreen.tileOffset = (PokedexScreen.tileOffset + 1) % 64;
    }
}

public class PokedexHabitatScreen : Screen
{
    private Texture2D _texture;
    private List<PokedexScreen.Habitat> _habitatList = [];
    private int _cursor = 0;
    private int _scroll = 0;

    public PokedexHabitatScreen(Screen currentScreen)
    {
        Identification = Identifications.PokedexHabitatScreen;
        PreScreen = currentScreen;
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        MouseVisible = true;
        CanMuteAudio = true;
        CanBePaused = true;

        foreach (String file in System.IO.Directory.GetFiles(GameController.GamePath + GameModeManager.ActiveGameMode!.PokeFilePath, "*.*", System.IO.SearchOption.AllDirectories))
        {
            if (file.EndsWith(".poke") == false) continue;
            bool dexInclude = true;
            String[] data = System.IO.File.ReadAllLines(file);
            foreach (String line in data)
            {
                if (line.ToLower().StartsWith("dexinclude=") == false) continue;
                dexInclude = bool.Parse(line.Remove(0, 11));
                break;
            }

            String fileName = Path.GetFileName(file);
            PokedexScreen.Habitat newHabitat = new PokedexScreen.Habitat(file);
            bool exists = false;

            if (dexInclude == false)
            {
                newHabitat.PokemonList.Clear();
                newHabitat.MergeData = [];
            }
            foreach (PokedexScreen.Habitat h in _habitatList)
            {
                if (h.Name.ToLower() != newHabitat.Name.ToLower()) continue;
                exists = true;
                h.Merge(newHabitat);
                break;
            }
            if (exists == false && Core.Player.PokeFiles.Contains(fileName.ToLower()) == true)
                _habitatList.Add(newHabitat);
        }
        _habitatList = _habitatList.OrderBy(h => h.Name).ToList();
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, new Color(84, 198, 216));

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 128, y + PokedexScreen.tileOffset, 128, 64), new Rectangle(48, 0, 16, 16), Color.White);

        Canvas.DrawGradient(new Rectangle(0, 0, Core.windowSize.Width, 200), new Color(42, 167, 198), new Color(42, 167, 198, 0), false, -1);
        Canvas.DrawGradient(new Rectangle(0, Core.windowSize.Height - 200, Core.windowSize.Width, 200), new Color(42, 167, 198, 0), new Color(42, 167, 198), false, -1);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokedex_habitat_select", "Select a Habitat"), new Vector2(100, 24), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

        for (int i = _scroll; i <= _scroll + 5; i++)
        {
            if (i > _habitatList.Count - 1) continue;
            int p = i - _scroll;

            Core.SpriteBatch.Draw(_texture, new Rectangle(100, 100 + p * 96, 64, 64), new Rectangle(16, 16, 16, 16), Color.White);
            Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64, 100 + p * 96, 64 * 8, 64), new Rectangle(32, 16, 16, 16), Color.White);
            Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64 * 9, 100 + p * 96, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

            Core.SpriteBatch.Draw(_habitatList[i].Texture, new Rectangle(120, 108 + p * 96, 64, 48), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("Places_" + _habitatList[i].Name, _habitatList[i].Name), new Vector2(200, 120 + p * 96), Color.Black);

            String t = _habitatList[i].PokemonCaught.ToString() + "/" + _habitatList[i].PokemonList.Count;
            Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(680 - (FontManager.MainFont.MeasureString(t).X * 1.0F) / 2.0F, 120 + p * 96), Color.Black);

            Texture2D? progressTexture = _habitatList[i].ProgressTexture;
            if (progressTexture != null)
                Core.SpriteBatch.Draw(progressTexture, new Rectangle((int)(650 - (FontManager.MainFont.MeasureString(t).X * 1.0F) / 2.0F), 120 + p * 96, 20, 20), Color.White);
        }

        DrawCursor();
    }

    private void DrawCursor()
    {
        Vector2 cPosition = new Vector2(520, 100 + _cursor * 96 - 42);
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle((int)cPosition.X, (int)cPosition.Y, 64, 64), Color.White);
    }

    public override void Update()
    {
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

        _scroll = _habitatList.Count < 7 ? 0 : Math.Clamp(_scroll, 0, _habitatList.Count - 6);
        _cursor = _habitatList.Count < 6 ? Math.Clamp(_cursor, 0, _habitatList.Count - 1) : Math.Clamp(_cursor, 0, 5);

        if (_habitatList.Count > 0)
        {
            if (Controls.Accept(true, false, false) == true)
            {
                for (int i = _scroll; i <= _scroll + 5; i++)
                {
                    if (i > _habitatList.Count - 1) continue;
                    if (new Rectangle(100, 100 + (i - _scroll) * 96, 640, 64).Contains(MouseHandler.MousePosition) == false) continue;
                    if (i == _cursor + _scroll)
                    {
                        SoundManager.PlaySound("select");
                        Core.SetScreen(new PokedexScreen(this, null, _habitatList[_cursor + _scroll]));
                    }
                    else { _cursor = i - _scroll; }
                }
            }

            if (Controls.Accept(false, true, true) == true)
            {
                SoundManager.PlaySound("select");
                Core.SetScreen(new PokedexScreen(this, null, _habitatList[_cursor + _scroll]));
            }
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            Core.SetScreen(PreScreen!);
            SoundManager.PlaySound("select");
        }

        PokedexScreen.tileOffset = (PokedexScreen.tileOffset + 1) % 64;
    }
}

public class PokedexScreen : Screen
{
    public static int tileOffset = 0;

    public enum OrderType { Numeric, Weight, Height, Alphabetically }

    public enum FilterType { Type1, Type2, Name }

    public struct Filter
    {
        public FilterType FilterType;
        public String FilterValue;
    }

    private Texture2D _texture;

    public bool reverseOrder = false;
    public OrderType order = OrderType.Numeric;

    public List<Filter> filters = [];
    public PokedexSelectScreen.PokedexProfile profile;
    public Habitat? cHabitat = null;

    private int _scroll = 0;
    private Vector2 _cursor = new Vector2(0);

    public static Dictionary<int, Pokemon> TempPokemonStorage = [];
    private static Dictionary<int, int> _tempPokemonDexType = [];

    public List<Pokemon> pokemonList = [];
    private SelectMenu _menu = null!;

    public int selectIndexMain = 0;
    public int orderIndexMain = 0;
    public int orderIndexType = 0;
    public int filterIndexMain = 0;
    public int filterIndexName = 0;
    public int filterIndexType1 = 0;
    public int filterIndexType2 = 0;

    public PokedexScreen(Screen currentScreen, PokedexSelectScreen.PokedexProfile? profile, Habitat? habitat)
    {
        Identification = Identifications.PokedexScreen;
        PreScreen = currentScreen;
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        this.profile = profile ?? new PokedexSelectScreen.PokedexProfile();
        cHabitat = habitat;
        MouseVisible = true;
        CanMuteAudio = true;
        CanBePaused = true;

        TempPokemonStorage.Clear();
        _tempPokemonDexType.Clear();

        SetList();

        _menu = new SelectMenu([""], 0, null, 0, String.Empty);
        _menu.Visible = false;
    }

    private void SetList()
    {
        pokemonList.Clear();
        TempPokemonStorage.Clear();
        _tempPokemonDexType.Clear();

        int neededEntryType = 0;
        switch (order)
        {
            case OrderType.Alphabetically: neededEntryType = 1; break;
            case OrderType.Height: case OrderType.Weight: neededEntryType = 2; break;
        }

        foreach (Filter f in filters)
        {
            int thisType = f.FilterType == FilterType.Name ? 1 : 2;
            if (thisType > neededEntryType) neededEntryType = thisType;
        }

        List<String> pokeSearchList = [];

        if (cHabitat == null)
        {
            if (profile.Pokedex != null && profile.Pokedex.IncludeExternalPokemon == true)
            {
                if (Pokedex.PokemonMaxCount > 0)
                {
                    for (int i = 1; i <= Pokedex.PokemonMaxCount; i++)
                    {
                        if (profile.Pokedex.HasPokemon(i.ToString(), false) == false)
                        {
                            if (Pokedex.GetEntryType(Core.Player.PokedexData, i.ToString()) > 0)
                                profile.Pokedex.PokemonList.Add(profile.Pokedex.PokemonList.Count + 1, i.ToString());
                        }
                    }
                }
            }
            if (profile.Pokedex != null)
            {
                foreach (String s in profile.Pokedex.PokemonList.Values)
                    pokeSearchList.Add(s);
            }
        }
        else
        {
            foreach (String s in cHabitat.PokemonList)
                pokeSearchList.Add(s);
        }

        for (int i = 0; i <= pokeSearchList.Count - 1; i++)
        {
            if (Pokemon.PokemonDataExists(pokeSearchList[i].GetSplit(0, "_")) == false && Pokemon.PokemonDataExists(pokeSearchList[i].GetSplit(0, ";")) == false) continue;

            int pID;
            String pAD = String.Empty;

            if (pokeSearchList[i].Contains(";") == true)
            {
                pID = int.Parse(pokeSearchList[i].GetSplit(0, ";"));
                pAD = pokeSearchList[i].GetSplit(1, ";");
            }
            else if (pokeSearchList[i].Contains("_") == true)
            {
                String additionalValue = PokemonForms.GetAdditionalValueFromDataFile(pokeSearchList[i]);
                pID = int.Parse(pokeSearchList[i].GetSplit(0, "_"));
                if (additionalValue != String.Empty) pAD = additionalValue;
            }
            else
            {
                pID = int.Parse(pokeSearchList[i]);
            }

            Pokemon p = pAD != String.Empty ? Pokemon.GetPokemonByID(pID, pAD) : Pokemon.GetPokemonByID(pID, pAD, true);

            if (Pokedex.GetEntryType(Core.Player.PokedexData, pokeSearchList[i]) < neededEntryType) continue;

            bool valid = true;
            foreach (Filter f in filters)
            {
                switch (f.FilterType)
                {
                    case FilterType.Name:
                        if (p.GetName(true).ToUpper().StartsWith(f.FilterValue.ToUpper()) == false) { valid = false; }
                        break;
                    case FilterType.Type1:
                        if (p.Type1.Type != BattleSystem.GameModeElementLoader.GetElementByName(f.FilterValue).Type) { valid = false; }
                        break;
                    case FilterType.Type2:
                        if (p.Type2.Type != BattleSystem.GameModeElementLoader.GetElementByName(f.FilterValue).Type) { valid = false; }
                        break;
                }
                if (valid == false) break;
            }

            if (valid == false) continue;

            if (profile.Pokedex != null)
            {
                if (profile.Pokedex.GetPlace(pokeSearchList[i]) != -1)
                    pokemonList.Add(p);
            }
            else
            {
                pokemonList.Add(p);
            }
        }

        switch (order)
        {
            case OrderType.Numeric:
                if (cHabitat == null && profile.Pokedex != null)
                    pokemonList = reverseOrder == true
                        ? pokemonList.OrderByDescending(p => profile.Pokedex.GetPlace(PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true))).ToList()
                        : pokemonList.OrderBy(p => profile.Pokedex.GetPlace(PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true))).ToList();
                else
                    pokemonList = reverseOrder == true
                        ? pokemonList.OrderByDescending(p => PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true)).ToList()
                        : pokemonList.OrderBy(p => PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true)).ToList();
                break;
            case OrderType.Alphabetically:
                pokemonList = reverseOrder == true
                    ? pokemonList.OrderByDescending(p => p.GetName(true)).ToList()
                    : pokemonList.OrderBy(p => p.GetName(true)).ToList();
                break;
            case OrderType.Weight:
                pokemonList = reverseOrder == true
                    ? pokemonList.OrderByDescending(p => p.pokedexEntry?.Weight ?? 0).ToList()
                    : pokemonList.OrderBy(p => p.pokedexEntry?.Weight ?? 0).ToList();
                break;
            case OrderType.Height:
                pokemonList = reverseOrder == true
                    ? pokemonList.OrderByDescending(p => p.pokedexEntry?.Height ?? 0).ToList()
                    : pokemonList.OrderBy(p => p.pokedexEntry?.Height ?? 0).ToList();
                break;
        }

        ClampCursor();
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, new Color(84, 198, 216));

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 128, y + tileOffset, 128, 64), new Rectangle(48, 0, 16, 16), Color.White);

        Canvas.DrawGradient(new Rectangle(0, 0, Core.windowSize.Width, 200), new Color(42, 167, 198), new Color(42, 167, 198, 0), false, -1);
        Canvas.DrawGradient(new Rectangle(0, Core.windowSize.Height - 200, Core.windowSize.Width, 200), new Color(42, 167, 198, 0), new Color(42, 167, 198), false, -1);

        Canvas.DrawRectangle(new Rectangle(50, 30, 564, 90), new Color(42, 167, 198, 150));

        if (cHabitat == null)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, profile.Pokedex?.Name ?? String.Empty, new Vector2(60, 55), Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0.0F);
            String seenObtained = Localization.GetString("pokedex_seen", "Seen:") + " " + Environment.NewLine + Environment.NewLine + Localization.GetString("pokedex_obtained", "Obtained:") + " ";
            int seenCaughtTitleWidth = (int)FontManager.MainFont.MeasureString(seenObtained).X;
            Core.SpriteBatch.DrawString(FontManager.MainFont, seenObtained, new Vector2(420, 45), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, (profile.Seen + profile.Obtained) + Environment.NewLine + Environment.NewLine + profile.Obtained, new Vector2(420 + seenCaughtTitleWidth, 45), Color.Black);
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("Places_" + cHabitat.Name, cHabitat.Name), new Vector2(60, 80), Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0.0F);
            Core.SpriteBatch.Draw(cHabitat.Texture, new Rectangle(60, 32, 64, 48), Color.White);
            String availableObtained = Localization.GetString("pokedex_available", "Available:") + Environment.NewLine + Environment.NewLine + Localization.GetString("pokedex_obtained", "Obtained:");
            int availObtainedTitleWidth = (int)FontManager.MainFont.MeasureString(availableObtained).X;
            Core.SpriteBatch.DrawString(FontManager.MainFont, availableObtained, new Vector2(420, 45), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, cHabitat.PokemonList.Count + Environment.NewLine + Environment.NewLine + cHabitat.PokemonCaught, new Vector2(420 + availObtainedTitleWidth, 45), Color.Black);

            Texture2D? progressTexture = cHabitat.ProgressTexture;
            if (progressTexture != null)
                Core.SpriteBatch.Draw(progressTexture, new Rectangle(134, 46, 20, 20), Color.White);
        }

        if (pokemonList.Count > 0)
        {
            for (int x = 0; x <= 5; x++)
            {
                for (int y = 0; y <= 4; y++)
                {
                    int id = (y + _scroll) * 6 + x;
                    if (id > pokemonList.Count - 1) continue;

                    String dexID = PokemonForms.GetPokemonDataFileName(pokemonList[id].Number, pokemonList[id].AdditionalData, true);

                    if (cHabitat != null || profile.Pokedex?.OriginalCount >= profile.Pokedex?.GetPlace(dexID))
                        Canvas.DrawRectangle(new Rectangle(50 + x * 100, 140 + y * 100, 64, 92), new Color(42, 167, 198, 150));
                    else
                        Canvas.DrawBorder(3, new Rectangle(50 + x * 100, 140 + y * 100, 64, 92), new Color(42, 167, 198, 150));

                    if (TempPokemonStorage.ContainsKey(id + 1) == false)
                    {
                        TempPokemonStorage.Add(id + 1, pokemonList[id]);
                        _tempPokemonDexType.Add(id + 1, Pokedex.GetEntryType(Core.Player.PokedexData, dexID));
                    }
                    Pokemon p = TempPokemonStorage[id + 1];
                    int entryType = _tempPokemonDexType[id + 1];

                    if (_cursor == new Vector2(x, y)) DrawPokemonPreview(p);

                    bool drawBlack = false;
                    if (dexID.Contains(";") == false)
                    {
                        if (entryType == 0)
                        {
                            int formEntry = Pokedex.HasAnyForm(pokemonList[id].Number);
                            if (formEntry > 0)
                            {
                                entryType = formEntry;
                                String[]? pForms = PokemonForms.GetAdditionalDataForms(pokemonList[id].Number);
                                if (dexID.Contains("_") == true && pForms == null) drawBlack = true;
                            }
                        }
                    }

                    if (entryType > 0)
                    {
                        Color c = entryType > 1 ? Color.White : Color.Gray;
                        if (drawBlack == true) c = Color.Black;
                        Texture2D pokeTexture = p.GetMenuTexture();
                        Vector2 pokeScale = new Vector2((float)(32.0 / pokeTexture.Width * 2), (float)(32.0 / pokeTexture.Height * 2));
                        Core.SpriteBatch.Draw(pokeTexture, new Rectangle(50 + x * 100, 140 + y * 100, (int)(pokeTexture.Width * pokeScale.X), (int)(pokeTexture.Height * pokeScale.Y)), c);
                    }

                    String no = cHabitat == null && profile.Pokedex != null
                        ? profile.Pokedex.GetPlace(dexID).ToString()
                        : p.Number.ToString();
                    while (no.Length < 3) no = "0" + no;
                    Core.SpriteBatch.DrawString(FontManager.MainFont, no, new Vector2(50 + x * 100 + (int)(32 - FontManager.MainFont.MeasureString(no).X / 2), 206 + y * 100), Color.White);
                }
            }
        }
        else
        {
            Canvas.DrawGradient(new Rectangle(50, 300, 80, 90), new Color(84, 198, 216), new Color(42, 167, 198, 150), true, -1);
            Canvas.DrawRectangle(new Rectangle(130, 300, 404, 90), new Color(42, 167, 198, 150));
            Canvas.DrawGradient(new Rectangle(534, 300, 80, 90), new Color(42, 167, 198, 150), new Color(84, 198, 216), true, -1);
            String noResults = Localization.GetString("pokedex_search_no_results", "No search results.");
            Core.SpriteBatch.DrawString(FontManager.MainFont, noResults, new Vector2(50 + (int)(564 / 2) - (int)(FontManager.MainFont.MeasureString(noResults).X / 2), 330), Color.White);
        }

        Canvas.DrawRectangle(new Rectangle(670, 30, 480, 90), new Color(42, 167, 198, 150));
        String orderText = order switch
        {
            OrderType.Alphabetically => Localization.GetString("pokedex_order_alphabetically", "A-Z"),
            OrderType.Height => Localization.GetString("pokedex_order_height", "Height"),
            OrderType.Weight => Localization.GetString("pokedex_order_weight", "Weight"),
            _ => Localization.GetString("pokedex_order_numeric", "Numeric")
        };
        String filterText = Localization.GetString("pokedex_filter_none", "None");
        if (filters.Count > 0)
        {
            filterText = String.Empty;
            foreach (Filter f in filters)
            {
                if (filterText != String.Empty) filterText += ", ";
                filterText += f.FilterType switch
                {
                    FilterType.Name => Localization.GetString("pokedex_filter_name", "Name"),
                    FilterType.Type1 => Localization.GetString("pokedex_filter_type1", "Type 1"),
                    FilterType.Type2 => Localization.GetString("pokedex_filter_type2", "Type 2"),
                    _ => String.Empty
                };
            }
        }
        String orderFilterLabel = Localization.GetString("pokedex_order", "Order") + ":" + Environment.NewLine + Localization.GetString("pokedex_filter", "Filter") + ":";
        int orderFilterTitleWidth = (int)FontManager.MainFont.MeasureString(orderFilterLabel).X;
        Core.SpriteBatch.DrawString(FontManager.MainFont, orderFilterLabel + Environment.NewLine + Localization.GetString("pokedex_search_hint", "Press [<system.button(special)>] or Select to search."), new Vector2(685, 45), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, orderText + Environment.NewLine + filterText, new Vector2(685 + orderFilterTitleWidth + (int)FontManager.MainFont.MeasureString(" ").X, 45), Color.Black);

        if (_menu.Visible == true) _menu.Draw();
        else if (pokemonList.Count > 0) DrawCursor();
    }

    private int GetPlace(int pokemonNumber)
    {
        for (int i = 0; i <= pokemonList.Count - 1; i++)
        {
            if (pokemonList[i].Number == pokemonNumber) return i + 1;
        }
        return -1;
    }

    private void DrawPokemonPreview(Pokemon p)
    {
        String dexID = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);
        int entryType = Pokedex.GetEntryType(Core.Player.PokedexData, dexID);

        if (dexID.Contains("_") == false && dexID.Contains(";") == false)
        {
            if (entryType == 0)
            {
                int formEntry = Pokedex.HasAnyForm(p.Number);
                if (formEntry > 0) entryType = formEntry;
            }
        }

        for (int i = 0; i <= 4; i++)
        {
            Canvas.DrawGradient(new Rectangle(650, 300 + i * 40, 50, 2), new Color(255, 255, 255, 10), new Color(255, 255, 255, 255), true, -1);
            Canvas.DrawRectangle(new Rectangle(700, 300 + i * 40, 350, 2), Color.White);
            Canvas.DrawGradient(new Rectangle(1050, 300 + i * 40, 50, 2), new Color(255, 255, 255, 255), new Color(255, 255, 255, 10), true, -1);
        }

        String no = cHabitat == null && profile.Pokedex != null
            ? profile.Pokedex.GetPlace(dexID).ToString()
            : p.Number.ToString();
        while (no.Length < 3) no = "0" + no;

        if (entryType == 0)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, "???" + Environment.NewLine + Environment.NewLine + Localization.GetString("No.", "No.") + " " + no, new Vector2(864, 200), Color.White);
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, p.GetName(true) + Environment.NewLine + Environment.NewLine + Localization.GetString("No.", "No.") + " " + no, new Vector2(864, 200), Color.White);
            Texture2D frontTex = p.GetTexture(true);
            Core.SpriteBatch.Draw(frontTex, new Rectangle((int)(680 - frontTex.Width / 4), (int)(140 - frontTex.Height / 4), Math.Min((int)(frontTex.Width * 2), 256), Math.Min((int)(frontTex.Height * 2), 256)), Color.White);

            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokedex_data_species", "SPECIES"), new Vector2(680, 310), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokedex_data_type", "TYPE"), new Vector2(680, 350), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokedex_data_height", "HEIGHT"), new Vector2(680, 390), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokedex_data_weight", "WEIGHT"), new Vector2(680, 430), Color.Black);

            Canvas.DrawRectangle(new Rectangle(670, 480, 480, 152), new Color(42, 167, 198, 150));

            if (cHabitat != null)
            {
                List<int> encounterTypes = [];
                foreach (Habitat.EncounterPokemon ec in cHabitat.ObtainTypeList)
                {
                    if (ec.PokemonID == PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData) && encounterTypes.Contains(ec.EncounterType) == false)
                        encounterTypes.Add(ec.EncounterType);
                }
                for (int i = 0; i <= encounterTypes.Count - 1; i++)
                    Core.SpriteBatch.Draw(Habitat.GetEncounterTypeImage(encounterTypes[i]), new Rectangle(824 + i * 32, 266, 32, 32), Color.White);
            }

            if (entryType > 1)
            {
                String dexEntrySpecies = p.pokedexEntry?.Species ?? String.Empty;
                String formName = PokemonForms.GetFormName(p);
                if (formName == String.Empty) formName = p.Name;
                if (Localization.TokenExists("pokemon_species_" + formName) == true)
                    dexEntrySpecies = Localization.GetString("pokemon_species_" + formName, p.pokedexEntry?.Species ?? String.Empty);

                Core.SpriteBatch.DrawString(FontManager.MainFont, dexEntrySpecies, new Vector2(850, 310), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MainFont, String.Empty, new Vector2(850, 350), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MainFont, (p.pokedexEntry?.Height ?? 0) + " m", new Vector2(850, 390), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MainFont, (p.pokedexEntry?.Weight ?? 0) + " kg", new Vector2(850, 430), Color.Black);

                Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle(850, 356, 48, 16), p.Type1.GetElementImage(), Color.White);
                if (p.Type2 != null && p.Type2.Type != Element.Types.Blank)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle(900, 356, 48, 16), p.Type2.GetElementImage(), Color.White);

                String dexEntryText = p.pokedexEntry?.Text ?? String.Empty;
                if (Localization.TokenExists("pokemon_desc_" + formName) == true)
                    dexEntryText = Localization.GetString("pokemon_desc_" + formName, p.pokedexEntry?.Text ?? String.Empty);
                Core.SpriteBatch.DrawString(FontManager.MainFont, dexEntryText.CropStringToWidth(FontManager.MainFont, 440), new Vector2(688, 490), Color.Black);

                Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(160, 160, 10, 10), String.Empty), new Rectangle(992, 242, 20, 20), Color.White);
                if (entryType > 2)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\PokemonInfo"), new Rectangle(1033, 243, 18, 18), new Rectangle(16, 0, 9, 9), Color.White);
            }
            else
            {
                Core.SpriteBatch.DrawString(FontManager.MainFont, "??? Pokémon", new Vector2(850, 310), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MainFont, "???", new Vector2(850, 350), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MainFont, "??? m", new Vector2(850, 390), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MainFont, "??? kg", new Vector2(850, 430), Color.Black);
                Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(160, 170, 10, 10), String.Empty), new Rectangle(992, 242, 20, 20), Color.White);
            }
        }
    }

    private void DrawCursor()
    {
        Vector2 cPosition = new Vector2(50 + _cursor.X * 100 + 42, 140 + _cursor.Y * 100 - 42);
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle((int)cPosition.X, (int)cPosition.Y, 64, 64), Color.White);
    }

    public override void Update()
    {
        if (_menu.Visible == true)
        {
            _menu.Update();
        }
        else
        {
            if (Controls.Left(true, true, false, true, true, true) == true) _cursor.X -= 1;
            if (Controls.Right(true, true, false, true, true, true) == true) _cursor.X += 1;
            if (Controls.Up(true, true, true, true, true, true) == true)
            {
                _cursor.Y -= 1;
                if (Controls.ShiftDown() == true) _cursor.Y -= 4;
            }
            if (Controls.Down(true, true, true, true, true, true) == true)
            {
                _cursor.Y += 1;
                if (Controls.ShiftDown() == true) _cursor.Y += 4;
            }

            if (_cursor.X > 5) { _cursor.X = 0; _cursor.Y += 1; }
            if (_cursor.X < 0) { _cursor.X = 5; _cursor.Y -= 1; }

            while (_cursor.Y < 0) { _cursor.Y += 1; _scroll -= 1; }
            while (_cursor.Y > 4) { _cursor.Y -= 1; _scroll += 1; }

            if (Controls.Accept(true, false, false) == true)
            {
                for (int i = 0; i <= 29; i++)
                {
                    int x = i;
                    int y = 0;
                    while (x > 5) { x -= 6; y += 1; }

                    if (new Rectangle(50 + x * 100, 140 + y * 100, 64, 92).Contains(MouseHandler.MousePosition) == false) continue;
                    if (_cursor.X + _cursor.Y * 6 == i)
                    {
                        int dexIndex = (int)((_cursor.Y + _scroll) * 6 + _cursor.X + 1);
                        if (_tempPokemonDexType.ContainsKey(dexIndex) == false) continue;
                        if (_tempPokemonDexType[dexIndex] > 0)
                        {
                            SoundManager.PlaySound("select");
                            Core.SetScreen(new PokedexViewScreen(this, TempPokemonStorage[dexIndex], false, dexIndex - 1));
                        }
                        else
                        {
                            int formEntry = Pokedex.HasAnyForm(TempPokemonStorage[dexIndex].Number);
                            if (formEntry > 0)
                            {
                                SoundManager.PlaySound("select");
                                Core.SetScreen(new PokedexViewScreen(this, TempPokemonStorage[dexIndex], false, dexIndex - 1));
                            }
                        }
                    }
                    else { _cursor.X = x; _cursor.Y = y; }
                }
            }

            ClampCursor();

            if (Controls.Accept(false, true, true) == true)
            {
                int dexIndex = (int)((_cursor.Y + _scroll) * 6 + _cursor.X + 1);
                if (_tempPokemonDexType.ContainsKey(dexIndex) == true && _tempPokemonDexType[dexIndex] > 0)
                {
                    SoundManager.PlaySound("select");
                    Core.SetScreen(new PokedexViewScreen(this, TempPokemonStorage[dexIndex], false, dexIndex - 1));
                }
            }

            if (KeyBoardHandler.KeyPressed(KeyBindings.SpecialKey) == true || ControllerHandler.ButtonPressed(Buttons.Back) == true)
                _menu = new SelectMenu([Localization.GetString("pokedex_order", "Order"), Localization.GetString("pokedex_filter", "Filter"), Localization.GetString("global_reset", "Reset"), Localization.GetString("global_back", "Back")], selectIndexMain, SelectMenu1, 3, "selectmain");

            if (Controls.Dismiss(true, true, true) == true)
            {
                SoundManager.PlaySound("select");
                if (filters.Count > 0 || order != OrderType.Numeric || reverseOrder == true)
                {
                    filters.Clear();
                    reverseOrder = false;
                    order = OrderType.Numeric;
                    SetList();
                }
                else
                {
                    Core.SetScreen(PreScreen!);
                }
            }
        }

        tileOffset = (tileOffset + 1) % 64;
    }

    private void ClampCursor()
    {
        int linesCount = (int)Math.Ceiling(pokemonList.Count / 6.0);

        _scroll = linesCount < 6 ? 0 : Math.Clamp(_scroll, 0, linesCount - 5);

        int maxY = linesCount - _scroll - 1;
        _cursor.Y = Math.Clamp(_cursor.Y, 0, maxY);

        if (_cursor.Y == maxY)
        {
            int maxX = pokemonList.Count;
            while (maxX > 6) maxX -= 6;
            _cursor.X = Math.Clamp(_cursor.X, 0, maxX - 1);
        }
    }

    public override void ChangeTo()
    {
        _tempPokemonDexType.Clear();
        TempPokemonStorage.Clear();
    }

    private void SelectMenu1(SelectMenu s)
    {
        String reverseString = reverseOrder == true ? Localization.GetString("global_yes", "Yes") : Localization.GetString("global_no", "No");
        String sLower = s.SelectedItem.ToLower();
        if (sLower == Localization.GetString("pokedex_order", "Order").ToLower())
            _menu = new SelectMenu([Localization.GetString("pokedex_order_type", "Type"), Localization.GetString("pokedex_order_reverse", "Reverse") + ": " + reverseString, Localization.GetString("global_back", "Back")], orderIndexMain, SelectMenuOrder, 2, "ordermain");
        else if (sLower == Localization.GetString("pokedex_filter", "Filter").ToLower())
            _menu = new SelectMenu([Localization.GetString("pokedex_filter_name", "Name"), Localization.GetString("pokedex_filter_type1", "Type 1"), Localization.GetString("pokedex_filter_type2", "Type 2"), Localization.GetString("global_clear", "Clear"), Localization.GetString("global_back", "Back")], filterIndexMain, SelectMenuFilter, 4, "filtermain");
        else if (sLower == Localization.GetString("global_reset", "Reset").ToLower())
        {
            filters.Clear();
            reverseOrder = false;
            order = OrderType.Numeric;
            SetList();
        }
    }

    private void SelectMenuFilter(SelectMenu s)
    {
        String sLower = s.SelectedItem.ToLower();
        if (sLower == Localization.GetString("pokedex_filter_name", "Name").ToLower())
            _menu = new SelectMenu(["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", Localization.GetString("global_back", "Back")], filterIndexName, SelectMenuNameFilter, -1, "filtername");
        else if (sLower == Localization.GetString("pokedex_filter_type1", "Type 1").ToLower())
        {
            List<String> typeList = ["Normal", "Fire", "Fighting", "Water", "Flying", "Grass", "Poison", "Electric", "Ground", "Psychic", "Rock", "Ice", "Bug", "Dragon", "Ghost", "Dark", "Steel", "Fairy"];
            foreach (Element e in BattleSystem.GameModeElementLoader.LoadedElements) typeList.Add(e.gmOriginalName);
            typeList.Add("Blank"); typeList.Add(Localization.GetString("global_back", "Back"));
            _menu = new SelectMenu(typeList, filterIndexType1, SelectMenuType1Filter, -1, "filtertype1");
        }
        else if (sLower == Localization.GetString("pokedex_filter_type2", "Type 2").ToLower())
        {
            List<String> typeList = ["Normal", "Fire", "Fighting", "Water", "Flying", "Grass", "Poison", "Electric", "Ground", "Psychic", "Rock", "Ice", "Bug", "Dragon", "Ghost", "Dark", "Steel", "Fairy"];
            foreach (Element e in BattleSystem.GameModeElementLoader.LoadedElements) typeList.Add(e.gmOriginalName);
            typeList.Add("Blank"); typeList.Add(Localization.GetString("global_back", "Back"));
            _menu = new SelectMenu(typeList, filterIndexType2, SelectMenuType2Filter, -1, "filtertype2");
        }
        else if (sLower == Localization.GetString("global_clear", "Clear").ToLower())
        {
            filters.Clear();
            SetList();
        }
        else if (sLower == Localization.GetString("global_back", "Back").ToLower())
            _menu = new SelectMenu([Localization.GetString("pokedex_order", "Order"), Localization.GetString("pokedex_filter", "Filter"), Localization.GetString("global_reset", "Reset"), Localization.GetString("global_back", "Back")], selectIndexMain, SelectMenu1, 3, "selectmain");
    }

    private void SelectMenuType1Filter(SelectMenu s)
    {
        if (s.SelectedItem != Localization.GetString("global_back", "Back"))
        {
            filters.RemoveAll(f => f.FilterType == FilterType.Type1);
            filters.Add(new Filter { FilterType = FilterType.Type1, FilterValue = s.SelectedItem });
            SetList();
        }
        else
            _menu = new SelectMenu([Localization.GetString("pokedex_filter_name", "Name"), Localization.GetString("pokedex_filter_type1", "Type 1"), Localization.GetString("pokedex_filter_type2", "Type 2"), Localization.GetString("global_clear", "Clear"), Localization.GetString("global_back", "Back")], filterIndexMain, SelectMenuFilter, 4, "filtermain");
    }

    private void SelectMenuType2Filter(SelectMenu s)
    {
        if (s.SelectedItem != Localization.GetString("global_back", "Back"))
        {
            filters.RemoveAll(f => f.FilterType == FilterType.Type2);
            filters.Add(new Filter { FilterType = FilterType.Type2, FilterValue = s.SelectedItem });
            SetList();
        }
        else
            _menu = new SelectMenu([Localization.GetString("pokedex_filter_name", "Name"), Localization.GetString("pokedex_filter_type1", "Type 1"), Localization.GetString("pokedex_filter_type2", "Type 2"), Localization.GetString("global_clear", "Clear"), Localization.GetString("global_back", "Back")], filterIndexMain, SelectMenuFilter, 4, "filtermain");
    }

    private void SelectMenuNameFilter(SelectMenu s)
    {
        if (s.SelectedItem != Localization.GetString("global_back", "Back"))
        {
            filters.RemoveAll(f => f.FilterType == FilterType.Name);
            filters.Add(new Filter { FilterType = FilterType.Name, FilterValue = s.SelectedItem });
            SetList();
        }
        else
            _menu = new SelectMenu([Localization.GetString("pokedex_filter_name", "Name"), Localization.GetString("pokedex_filter_type1", "Type 1"), Localization.GetString("pokedex_filter_type2", "Type 2"), Localization.GetString("global_clear", "Clear"), Localization.GetString("global_back", "Back")], filterIndexMain, SelectMenuFilter, 4, "filtermain");
    }

    private void SelectMenuOrder(SelectMenu s)
    {
        String reverseString = reverseOrder == true ? Localization.GetString("global_yes", "Yes") : Localization.GetString("global_no", "No");
        String sLower = s.SelectedItem.ToLower();
        if (sLower == Localization.GetString("pokedex_order_type", "Type").ToLower())
            _menu = new SelectMenu([Localization.GetString("pokedex_order_numeric", "Numeric"), Localization.GetString("pokedex_order_alphabetically", "A-Z"), Localization.GetString("pokedex_order_weight", "Weight"), Localization.GetString("pokedex_order_height", "Height"), Localization.GetString("global_back", "Back")], orderIndexType, SelectMenuOrderType, 4, "ordertype");
        else if (sLower == Localization.GetString("pokedex_order_reverse", "Reverse").ToLower() + ": " + reverseString.ToLower())
        {
            reverseOrder = !reverseOrder;
            _menu = new SelectMenu([Localization.GetString("pokedex_order_type", "Type"), Localization.GetString("pokedex_order_reverse", "Reverse") + ": " + reverseString.ToLower(), Localization.GetString("global_back", "Back")], orderIndexMain, SelectMenuOrder, 2, "ordermain");
            SetList();
        }
        else if (sLower == Localization.GetString("global_back", "Back").ToLower())
            _menu = new SelectMenu([Localization.GetString("pokedex_order", "Order"), Localization.GetString("pokedex_filter", "Filter"), Localization.GetString("global_reset", "Reset"), "Back"], selectIndexMain, SelectMenu1, 3, "selectmain");
    }

    private void SelectMenuOrderType(SelectMenu s)
    {
        String reverseString = reverseOrder == true ? Localization.GetString("global_yes", "Yes") : Localization.GetString("global_no", "No");
        String sLower = s.SelectedItem.ToLower();
        if (sLower == Localization.GetString("pokedex_order_numeric", "Numeric").ToLower()) { order = OrderType.Numeric; SetList(); }
        else if (sLower == Localization.GetString("pokedex_order_alphabetically", "A-Z").ToLower()) { order = OrderType.Alphabetically; SetList(); }
        else if (sLower == Localization.GetString("pokedex_order_weight", "Weight").ToLower()) { order = OrderType.Weight; SetList(); }
        else if (sLower == Localization.GetString("pokedex_order_height", "Height").ToLower()) { order = OrderType.Height; SetList(); }
        else if (s.SelectedItem == Localization.GetString("global_back", "Back"))
            _menu = new SelectMenu([Localization.GetString("pokedex_order_type", "Type"), Localization.GetString("pokedex_order_reverse", "Reverse") + ": " + reverseString, Localization.GetString("global_back", "Back")], orderIndexMain, SelectMenuOrder, 2, "ordermain");
    }

    private class SelectMenu
    {
        private List<String> _items = [];
        private int _index = 0;
        private String _rememberVar = String.Empty;
        public delegate void ClickEvent(SelectMenu s);
        private ClickEvent? _clickHandler = null;
        private int _backIndex = 0;
        public bool Visible = true;
        public int Scroll = 0;

        private Texture2D _t1 = null!;
        private Texture2D _t2 = null!;

        public SelectMenu(List<String> items, int index, ClickEvent? clickHandler, int backIndex, String rememberVar = "")
        {
            _items = items;
            _index = index;
            _clickHandler = clickHandler;
            _backIndex = backIndex;
            if (rememberVar != String.Empty) _rememberVar = rememberVar;
            if (_backIndex < 0) _backIndex = _items.Count + _backIndex;
            Visible = true;

            _t1 = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(16, 16, 16, 16), String.Empty);
            _t2 = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(32, 16, 16, 16), String.Empty);
        }

        public void Update()
        {
            if (Visible == false) return;
            if (Controls.Up(true, true, true, true, true, true) == true) _index -= 1;
            if (Controls.Down(true, true, true, true, true, true) == true) _index += 1;
            _index = Math.Clamp(_index, 0, _items.Count - 1);

            if (Core.CurrentScreen is PokedexScreen ds)
            {
                switch (_rememberVar.ToLower())
                {
                    case "selectmain": ds.selectIndexMain = _index; break;
                    case "ordermain": ds.orderIndexMain = _index; break;
                    case "ordertype": ds.orderIndexType = _index; break;
                    case "filtermain": ds.filterIndexMain = _index; break;
                    case "filtername": ds.filterIndexName = _index; break;
                    case "filtertype1": ds.filterIndexType1 = _index; break;
                    case "filtertype2": ds.filterIndexType2 = _index; break;
                }
            }

            for (int i = Scroll; i <= Scroll + 8; i++)
            {
                if (i > _items.Count - 1) continue;
                bool hovering = new Rectangle(Core.windowSize.Width - 270, 66 * ((i + 1) - Scroll), 256, 64).Contains(MouseHandler.MousePosition);
                bool pointerAccept = Controls.Accept(true, false, false) == true && i == _index && hovering == true;
                bool buttonsAccept = Controls.Accept(false, true, true) == true && i == _index;
                bool dismiss = Controls.Dismiss(true, true, true) == true && _backIndex == _index;

                if (pointerAccept == true || buttonsAccept == true || dismiss == true)
                {
                    _clickHandler?.Invoke(this);
                    SoundManager.PlaySound("select");
                    Visible = false;
                }

                if (Controls.Dismiss(true, true, true) == true)
                {
                    _index = _backIndex;
                    _clickHandler?.Invoke(this);
                    SoundManager.PlaySound("select");
                    Visible = false;
                    break;
                }

                if (hovering == true && Controls.Accept(true, false, false) == true) _index = i;
            }

            if (_index - Scroll > 8) Scroll = _index - 8;
            if (_index - Scroll < 0) Scroll = _index;
        }

        public void Draw()
        {
            if (Visible == false) return;
            for (int i = Scroll; i <= Scroll + 8; i++)
            {
                if (i > _items.Count - 1) continue;
                String entry = _items[i];
                String text = entry;

                switch (entry)
                {
                    case "Normal": case "Fire": case "Fighting": case "Water": case "Flying":
                    case "Grass": case "Poison": case "Electric": case "Ground": case "Psychic":
                    case "Rock": case "Ice": case "Bug": case "Dragon": case "Ghost":
                    case "Dark": case "Steel": case "Fairy": case "Blank":
                        text = Localization.GetString("global_pokemon_type_" + entry.ToLower(), entry);
                        break;
                }

                foreach (Element e in BattleSystem.GameModeElementLoader.LoadedElements)
                {
                    if (entry.ToLower() == e.gmOriginalName.ToLower())
                        text = Localization.GetString("global_pokemon_type_" + e.gmOriginalName.ToLower(), entry);
                }

                Vector2 startPos = new Vector2(Core.windowSize.Width - 270, 66 * ((i + 1) - Scroll));

                Core.SpriteBatch.Draw(_t1, new Rectangle((int)startPos.X, (int)startPos.Y, 64, 64), Color.White);
                Core.SpriteBatch.Draw(_t2, new Rectangle((int)(startPos.X + 64), (int)startPos.Y, 64, 64), Color.White);
                Core.SpriteBatch.Draw(_t2, new Rectangle((int)(startPos.X + 128), (int)startPos.Y, 64, 64), Color.White);
                Core.SpriteBatch.Draw(_t1, new Rectangle((int)(startPos.X + 192), (int)startPos.Y, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

                Core.SpriteBatch.DrawString(FontManager.MainFont, text, new Vector2(startPos.X + 128 - (FontManager.MainFont.MeasureString(text).X * 1.4F) / 2, startPos.Y + 15), Color.Black, 0.0F, Vector2.Zero, 1.4F, SpriteEffects.None, 0.0F);

                if (_index != i) continue;
                Vector2 cPosition = new Vector2(startPos.X + 128, startPos.Y - 40);
                Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
                Core.SpriteBatch.Draw(t, new Rectangle((int)cPosition.X, (int)cPosition.Y, 64, 64), Color.White);
            }
        }

        public String SelectedItem => _items[_index];
    }

    public class Habitat
    {
        public enum HabitatTypes
        {
            Grassland = 0,
            Forest = 1,
            WatersEdge = 2,
            Sea = 3,
            Cave = 4,
            Mountain = 5,
            RoughTerrain = 6,
            City = 7
        }

        public struct EncounterPokemon
        {
            public String PokemonID;
            public int EncounterType;
            public int[] Daytimes;
        }

        public String[] MergeData = [];
        public String File = String.Empty;
        public String Name = String.Empty;
        public HabitatTypes HabitatType = HabitatTypes.Grassland;
        public List<String> PokemonList = [];
        public List<EncounterPokemon> ObtainTypeList = [];
        public int PokemonCaught = 0;
        public int PokemonSeen = 0;

        public Habitat(String file)
        {
            Security.FileValidation.CheckFileValid(file, false, "PokedexScreen.cs");
            String[] data = System.IO.File.ReadAllLines(file);
            MergeData = data;
            File = file;

            foreach (String line in data)
            {
                if (line.ToLower().StartsWith("name=") == true)
                    Name = line.Remove(0, 5);
                else if (line.ToLower().StartsWith("type=") == true)
                {
                    HabitatType = line.Remove(0, 5).ToLower() switch
                    {
                        "grassland" => HabitatTypes.Grassland,
                        "forest" => HabitatTypes.Forest,
                        "watersedge" => HabitatTypes.WatersEdge,
                        "sea" => HabitatTypes.Sea,
                        "cave" => HabitatTypes.Cave,
                        "mountain" => HabitatTypes.Mountain,
                        "roughterrain" => HabitatTypes.RoughTerrain,
                        "city" => HabitatTypes.City,
                        _ => HabitatType
                    };
                }
                else if (line.StartsWith("{") == true && line.EndsWith("}") == true)
                {
                    String[] pokemonData = line.Remove(line.Length - 1, 1).Remove(0, 1).Split('|');

                    if (PokemonList.Contains(pokemonData[1]) == false)
                    {
                        PokemonList.Add(pokemonData[1]);
                        int entryType = Pokedex.GetEntryType(Core.Player.PokedexData, pokemonData[1]);
                        if (entryType > 0) PokemonSeen += 1;
                        if (entryType > 1) PokemonCaught += 1;
                    }

                    String[] daytimesData = pokemonData[3].Split(',');
                    List<int> dayTimes = [];
                    foreach (String s in daytimesData)
                    {
                        if (StringHelper.IsNumeric(s) == false) continue;
                        int dt = int.Parse(s);
                        if (dt > 0) dayTimes.Add(dt);
                        else { dayTimes.Clear(); break; }
                    }
                    ObtainTypeList.Add(new EncounterPokemon { PokemonID = pokemonData[1], EncounterType = int.Parse(pokemonData[0]), Daytimes = dayTimes.ToArray() });
                }
            }

            if (Name == String.Empty)
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(file);
                Name = Name[0].ToString().ToUpper() + Name.Remove(0, 1);
            }
        }

        public Texture2D Texture
        {
            get
            {
                int x = (int)HabitatType;
                int y = 0;
                while (x > 1) { x -= 2; y += 1; }
                return TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(x * 64, y * 48, 64, 48), String.Empty);
            }
        }

        public static Texture2D GetEncounterTypeImage(int encounterType)
        {
            int x = 0;
            int y = 4;
            switch (encounterType)
            {
                case 0: x = 0; y = 3; break;
                case 1: x = 0; y = 4; break;
                case 2: x = 1; y = 0; break;
                case 3: x = 0; y = 0; break;
                case 31: x = 0; y = 1; break;
                case 32: x = 0; y = 2; break;
            }
            return TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(128 + x * 32, y * 32, 32, 32), String.Empty);
        }

        public Texture2D? ProgressTexture
        {
            get
            {
                if (PokemonCaught >= PokemonList.Count)
                    return TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(160, 160, 10, 10), String.Empty);
                if (PokemonSeen >= PokemonList.Count)
                    return TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(160, 170, 10, 10), String.Empty);
                return null;
            }
        }

        public void Merge(Habitat h)
        {
            foreach (String line in h.MergeData)
            {
                if (line.StartsWith("{") == false || line.EndsWith("}") == false) continue;
                String[] pokemonData = line.Remove(line.Length - 1, 1).Remove(0, 1).Split('|');

                if (PokemonList.Contains(pokemonData[1]) == false)
                {
                    PokemonList.Add(pokemonData[1]);
                    int entryType = Pokedex.GetEntryType(Core.Player.PokedexData, pokemonData[1]);
                    if (entryType > 0) PokemonSeen += 1;
                    if (entryType > 1) PokemonCaught += 1;
                }
                ObtainTypeList.Add(new EncounterPokemon { PokemonID = pokemonData[1], EncounterType = int.Parse(pokemonData[0]) });
            }
        }

        public bool HasPokemon(String pokemonNumber) => PokemonList.Contains(pokemonNumber);
    }
}

public class PokedexViewScreen : Screen
{
    private int _dexIndex = -1;
    private Pokemon _pokemon = null!;
    private Texture2D _texture = null!;
    private int _page = 0;
    private List<String> _forms = [];
    private int _formIndex = 0;

    private int _entryType = 0;
    private bool _transitionOut = false;

    private int _yOffset = 0;
    private bool _frontView = true;
    private bool _shinyView = false;

    private List<PokemonEvolutionLine> _evolutionLineConnections = [];
    private Vector2 _gridMinimum = new Vector2(0, 0);
    private Vector2 _gridMaximum = new Vector2(0, 0);

    private List<PokedexScreen.Habitat> _habitatList = [];

    private int _fadeMainImage = 0;
    private int _vLineLength = 1;
    private int _mLineLength = 1;
    private bool _playedCry = false;

    private int _scroll = 0;
    private int _cursor = 0;
    private float _scale = 2.0F;

    private class PokemonEvolutionLine
    {
        public List<Tuple<int, int, Pokemon>> ConnectionList = [];

        public PokemonEvolutionLine(List<Vector2> gridPositions, List<String> pokemonIDs)
        {
            if (gridPositions.Count != pokemonIDs.Count) return;
            for (int i = 0; i <= pokemonIDs.Count - 1; i++)
            {
                int dexID;
                String dexAD = String.Empty;

                if (pokemonIDs[i].Contains("_") == true)
                {
                    dexID = int.Parse(pokemonIDs[i].GetSplit(0, "_"));
                    dexAD = PokemonForms.GetAdditionalValueFromDataFile(pokemonIDs[i]);
                }
                else if (pokemonIDs[i].Contains(";") == true)
                {
                    dexID = int.Parse(pokemonIDs[i].GetSplit(0, ";"));
                    dexAD = pokemonIDs[i].GetSplit(1, ";");
                }
                else
                {
                    dexID = int.Parse(pokemonIDs[i]);
                }

                Pokemon p = Pokemon.GetPokemonByID(dexID, dexAD);
                ConnectionList.Add(new Tuple<int, int, Pokemon>((int)gridPositions[i].X, (int)gridPositions[i].Y, p));
            }
        }
    }

    public PokedexViewScreen(Screen currentScreen, Pokemon pokemon, bool transitionOut, int dexIndex = -1)
    {
        PreScreen = currentScreen;
        Identification = Identifications.PokedexViewScreen;
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        MouseVisible = true;
        CanMuteAudio = true;
        CanBePaused = true;
        _transitionOut = transitionOut;

        if (_forms.Count > 0)
        {
            _forms.Clear();
            _forms.Add(_pokemon?.Number.ToString() ?? String.Empty);
            _formIndex = 0;
        }

        LoadPokemonData(dexIndex, pokemon);
    }

    public PokedexViewScreen(Screen currentScreen, Pokemon pokemon, bool transitionOut)
        : this(currentScreen, pokemon, transitionOut, -1) { }

    private void LoadPokemonData(int newDexIndex, Pokemon? newPokemon = null, bool playCry = false)
    {
        if (newPokemon != null)
        {
            _evolutionLineConnections.Clear();
            _pokemon = newPokemon;
        }
        if (PreScreen?.Identification == Identifications.PokedexScreen)
        {
            PokedexScreen pDexScreen = (PokedexScreen)PreScreen;
            if (newDexIndex != -1 && newDexIndex != _dexIndex)
            {
                _pokemon = pDexScreen.pokemonList[newDexIndex];
                _dexIndex = newDexIndex;
                _forms.Clear();
                _evolutionLineConnections.Clear();
                _forms.Add(_pokemon.Number.ToString());
                _formIndex = 0;
            }
        }

        String dexID = PokemonForms.GetPokemonDataFileName(_pokemon.Number, _pokemon.AdditionalData);
        if (dexID.Contains("_") == false)
        {
            String[]? additionalForms = PokemonForms.GetAdditionalDataForms(_pokemon.Number);
            if (additionalForms != null && additionalForms.Contains(_pokemon.AdditionalData) == true)
                dexID = _pokemon.Number + ";" + _pokemon.AdditionalData;
            else
                dexID = _pokemon.Number.ToString();
        }

        _entryType = Pokedex.GetEntryType(Core.Player.PokedexData, dexID);

        if (dexID.Contains("_") == false && dexID.Contains(";") == false)
        {
            if (_entryType == 0)
            {
                int formEntry = Pokedex.HasAnyForm(_pokemon.Number);
                if (formEntry > 0) _entryType = formEntry;
            }
        }

        GetYOffset();
        FillHabitats();
        FillEvolutionGrid();

        if (playCry == true)
        {
            String crySuffix = PokemonForms.GetCrySuffix(_pokemon);
            SoundManager.PlayPokemonCry(_pokemon.Number, crySuffix);
        }
    }

    private void FillEvolutionGrid()
    {
        if (_pokemon.evolutionLines.Count > 0)
        {
            for (int e = 0; e <= _pokemon.evolutionLines.Count - 1; e++)
            {
                List<Vector2> gridPositions = [];
                List<String> pokemonIDs = [];
                String[] dataEntries = _pokemon.evolutionLines[e].Split(',');
                for (int i = 0; i <= dataEntries.Length - 1; i++)
                {
                    pokemonIDs.Add(dataEntries[i].GetSplit(0, "\\"));
                    Vector2 position = new Vector2(int.Parse(dataEntries[i].GetSplit(1, "\\")));
                    if (dataEntries[i].Split('\\').Length > 2)
                        position.Y = int.Parse(dataEntries[i].GetSplit(2, "\\"));

                    if (_gridMinimum.X > position.X) _gridMinimum.X = position.X;
                    if (_gridMinimum.Y > position.Y) _gridMinimum.Y = position.Y;
                    if (_gridMaximum.X < position.X) _gridMaximum.X = position.X;
                    if (_gridMaximum.Y < position.Y) _gridMaximum.Y = position.Y;

                    gridPositions.Add(position);
                }
                PokemonEvolutionLine evoline = new PokemonEvolutionLine(gridPositions, pokemonIDs);
                _evolutionLineConnections.Add(evoline);

                foreach (String f in pokemonIDs)
                {
                    if (int.Parse(f.GetSplit(0, "_").GetSplit(0, ";")) == _pokemon.Number)
                    {
                        if (_forms.Contains(f) == false) _forms.Add(f);
                    }
                }
            }
        }
        else
        {
            List<Vector2> gridPositions = [];
            List<String> pokemonIDs = [];

            if (_pokemon.Devolution != "0")
            {
                int devoID = int.Parse(_pokemon.Devolution.GetSplit(0, "_").GetSplit(0, ";"));
                String devoAD = String.Empty;
                if (_pokemon.Devolution.Contains("_") == true) devoAD = PokemonForms.GetAdditionalValueFromDataFile(_pokemon.Devolution);
                else if (_pokemon.Devolution.Contains(";") == true) devoAD = _pokemon.Devolution.GetSplit(1, ";");

                Pokemon devoP = Pokemon.GetPokemonByID(devoID, devoAD, true);
                if (devoP.Devolution != "0") pokemonIDs.Add(devoP.Devolution);
                pokemonIDs.Add(_pokemon.Devolution);
            }

            String dexData = PokemonForms.GetPokemonDataFileName(_pokemon.Number, _pokemon.AdditionalData, true);
            pokemonIDs.Add(dexData);

            if (_pokemon.evolutionConditions.Count > 0)
            {
                pokemonIDs.Add(_pokemon.evolutionConditions[0].Evolution);

                int evoID = int.Parse(_pokemon.evolutionConditions[0].Evolution.GetSplit(0, "_").GetSplit(0, ";"));
                String evoAD = String.Empty;
                if (_pokemon.evolutionConditions[0].Evolution.Contains("_") == true) evoAD = PokemonForms.GetAdditionalValueFromDataFile(_pokemon.evolutionConditions[0].Evolution);
                else if (_pokemon.evolutionConditions[0].Evolution.Contains(";") == true) evoAD = _pokemon.evolutionConditions[0].Evolution.GetSplit(1, ";");

                Pokemon evoP = Pokemon.GetPokemonByID(evoID, evoAD, true);
                if (evoP.evolutionConditions.Count > 0) pokemonIDs.Add(evoP.evolutionConditions[0].Evolution);
            }

            int centerIndex = 0;
            for (int i = 0; i <= pokemonIDs.Count - 1; i++)
            {
                if (int.Parse(pokemonIDs[i].GetSplit(0, "_").GetSplit(0, ";")) == _pokemon.Number)
                    centerIndex = i;
            }

            switch (pokemonIDs.Count)
            {
                case 1:
                    _gridMinimum = new Vector2(centerIndex); _gridMaximum = new Vector2(centerIndex);
                    gridPositions.Add(new Vector2(centerIndex, 0));
                    break;
                case 2:
                    _gridMinimum = new Vector2(0 - 2 * centerIndex, 0); _gridMaximum = new Vector2(2 - 2 * centerIndex, 0);
                    gridPositions.Add(new Vector2(0 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(2 - 2 * centerIndex, 0));
                    break;
                case 3:
                    _gridMinimum = new Vector2(0 - 2 * centerIndex, 0); _gridMaximum = new Vector2(4 - 2 * centerIndex, 0);
                    gridPositions.Add(new Vector2(0 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(2 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(4 - 2 * centerIndex, 0));
                    break;
                case 4:
                    _gridMinimum = new Vector2(0 - 2 * centerIndex, 0); _gridMaximum = new Vector2(6 - 2 * centerIndex, 0);
                    gridPositions.Add(new Vector2(0 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(2 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(4 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(6 - 2 * centerIndex, 0));
                    break;
                case 5:
                    _gridMinimum = new Vector2(0 - 2 * centerIndex, 0); _gridMaximum = new Vector2(8 - 2 * centerIndex, 0);
                    gridPositions.Add(new Vector2(0 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(2 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(4 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(6 - 2 * centerIndex, 0)); gridPositions.Add(new Vector2(8 - 2 * centerIndex, 0));
                    break;
            }

            _evolutionLineConnections.Add(new PokemonEvolutionLine(gridPositions, pokemonIDs));
        }
    }

    private void FillHabitats()
    {
        _habitatList.Clear();
        foreach (String file in System.IO.Directory.GetFiles(GameController.GamePath + GameModeManager.ActiveGameMode!.PokeFilePath, "*.*", System.IO.SearchOption.AllDirectories))
        {
            if (file.EndsWith(".poke") == false) continue;
            bool dexInclude = true;
            String[] data = System.IO.File.ReadAllLines(file);
            foreach (String line in data)
            {
                if (line.ToLower().StartsWith("dexinclude=") == false) continue;
                dexInclude = bool.Parse(line.Remove(0, 11));
                break;
            }

            String fileName = Path.GetFileName(file);
            PokedexScreen.Habitat newHabitat = new PokedexScreen.Habitat(file);
            bool exists = false;

            if (dexInclude == false)
            {
                newHabitat.PokemonList.Clear();
                newHabitat.MergeData = [];
            }
            foreach (PokedexScreen.Habitat h in _habitatList)
            {
                if (h.Name.ToLower() != newHabitat.Name.ToLower()) continue;
                exists = true;
                h.Merge(newHabitat);
                break;
            }
            if (exists == false && Core.Player.PokeFiles.Contains(fileName.ToLower()) == true)
                _habitatList.Add(newHabitat);
        }
        _habitatList = _habitatList.OrderBy(h => h.Name).ToList();

        String dexIDCurrent = PokemonForms.GetPokemonDataFileName(_pokemon.Number, _pokemon.AdditionalData);
        _habitatList.RemoveAll(h => h.HasPokemon(dexIDCurrent) == false);
    }

    private void GetYOffset()
    {
        Texture2D t = _pokemon.GetTexture(_frontView, _shinyView);
        _yOffset = -1;

        Color[] cArr = new Color[t.Width * t.Height];
        t.GetData(cArr);

        bool found = false;
        for (int y = 0; y <= t.Height - 1 && found == false; y++)
        {
            for (int x = 0; x <= t.Width - 1; x++)
            {
                if (cArr[x + y * t.Height] == Color.Transparent) continue;
                _yOffset = y;
                found = true;
                break;
            }
        }
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, new Color(84, 198, 216));

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 128, y + PokedexScreen.tileOffset, 128, 64), new Rectangle(48, 0, 16, 16), Color.White);

        Canvas.DrawGradient(new Rectangle(0, 0, Core.windowSize.Width, 200), new Color(42, 167, 198), new Color(42, 167, 198, 0), false, -1);
        Canvas.DrawGradient(new Rectangle(0, Core.windowSize.Height - 200, Core.windowSize.Width, 200), new Color(42, 167, 198, 0), new Color(42, 167, 198), false, -1);

        switch (_page)
        {
            case 0: DrawPage1(); break;
            case 1: DrawPage2(); break;
            case 2: DrawPage3(); break;
        }

        Core.SpriteBatch.Draw(_texture, new Rectangle(20, 20, 64, 64), new Rectangle(16, 16, 16, 16), Color.White);
        Core.SpriteBatch.Draw(_texture, new Rectangle(20 + 64, 20, 64 * 8, 64), new Rectangle(32, 16, 16, 16), Color.White);
        Core.SpriteBatch.Draw(_texture, new Rectangle(20 + 64 * 9, 20, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);
        Core.SpriteBatch.Draw(_texture, new Rectangle(20 + 64, 20, 64 * 5, 64), new Rectangle(32, 16, 16, 16), Color.White);
        Core.SpriteBatch.Draw(_texture, new Rectangle(20 + 64 * 6, 20, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

        Texture2D pokeTexture = _pokemon.GetMenuTexture();
        Vector2 pokeScale = new Vector2((float)(32.0 / pokeTexture.Width * 2), (float)(32.0 / pokeTexture.Height * 2));

        if (_entryType > 0)
        {
            Core.SpriteBatch.Draw(pokeTexture, new Rectangle(28, 20, (int)(pokeTexture.Width * pokeScale.X), (int)(pokeTexture.Height * pokeScale.Y)), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, _pokemon.GetName(true), new Vector2(100, 36), Color.Black);
        }
        else
        {
            Core.SpriteBatch.Draw(pokeTexture, new Rectangle(28, 20, (int)(pokeTexture.Width * pokeScale.X), (int)(pokeTexture.Height * pokeScale.Y)), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, "???", new Vector2(100, 36), Color.Black);
        }

        if (_entryType == 1)
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(160, 170, 10, 10), String.Empty), new Rectangle(64 * 6 + 40, 42, 20, 20), Color.White);
        else if (_entryType > 1)
        {
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\pokedexhabitat", new Rectangle(160, 160, 10, 10), String.Empty), new Rectangle(64 * 6 + 40, 42, 20, 20), Color.White);
            if (_entryType > 2)
                Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\PokemonInfo"), new Rectangle(64 * 6 - 1, 42, 18, 18), new Rectangle(16, 0, 9, 9), Color.White);
        }

        if (_mLineLength == 100)
        {
            if (_page == 0 || _page == 1)
                Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 70, Core.windowSize.Height / 2 - 32, 64, 64), new Rectangle(0, 16, 16, 16), Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);
            if (_page == 1 || _page == 2)
                Core.SpriteBatch.Draw(_texture, new Rectangle(6, Core.windowSize.Height / 2 - 32, 64, 64), new Rectangle(0, 16, 16, 16), Color.White);
        }

        if (_forms.Count > 1)
        {
            int canSwitchCount = _forms.Count(f => Pokedex.GetEntryType(Core.Player.PokedexData, f) > 0);
            String currentDexID = PokemonForms.GetPokemonDataFileName(_pokemon.Number, _pokemon.AdditionalData, true);
            if (canSwitchCount > 1 || (canSwitchCount == 1 && Pokedex.GetEntryType(Core.Player.PokedexData, currentDexID) == 0))
            {
                String hint = Localization.GetString("pokedex_data_form_switch_hint", "Press [Shift] to switch forms");
                Core.SpriteBatch.DrawString(FontManager.MainFont, hint, new Vector2(Core.windowSize.Width - (int)FontManager.MainFont.MeasureString(hint).X - 128, 36), Color.White);
            }
        }

        String pageLabel = _page switch
        {
            0 => Localization.GetString("pokedex_data_page_details", "Details"),
            1 => Localization.GetString("pokedex_data_page_habitat", "Habitat"),
            _ => Localization.GetString("pokedex_data_page_evolution", "Evolution")
        };
        Core.SpriteBatch.DrawString(FontManager.MainFont, pageLabel, new Vector2(480, 36), Color.Black);
    }

    private void DrawPage1()
    {
        Texture2D frontTex = _pokemon.GetTexture(_frontView, _shinyView);
        Vector2 v = Core.GetMiddlePosition(new Size(Math.Min(frontTex.Width * 4, 512), Math.Min(frontTex.Height * 4, 512)));

        Color textureColor = Color.White;
        String[]? pForms = PokemonForms.GetAdditionalDataForms(_pokemon.Number);
        if (_entryType == 0 && pForms == null) textureColor = new Color(0, 0, 0, 0);

        if (_fadeMainImage == 255)
        {
            Vector2 mV = Core.GetMiddlePosition(new Size(0, 0));
            Canvas.DrawLine(Color.Black, new Vector2(mV.X + 40, mV.Y - 40), new Vector2(mV.X + _vLineLength, mV.Y - _vLineLength), 2);
            Canvas.DrawLine(Color.Black, new Vector2(mV.X + 40, mV.Y + 40), new Vector2(mV.X + _vLineLength, mV.Y + _vLineLength), 2);
            Canvas.DrawLine(Color.Black, new Vector2(mV.X - 40, mV.Y - 40), new Vector2(mV.X - _vLineLength, mV.Y - _vLineLength), 2);
            Canvas.DrawLine(Color.Black, new Vector2(mV.X - 40, mV.Y + 40), new Vector2(mV.X - _vLineLength, mV.Y + _vLineLength), 2);

            if (_vLineLength == 140)
            {
                Canvas.DrawLine(Color.Black, new Vector2(mV.X + 140, mV.Y - 140), new Vector2(mV.X + (140 + _mLineLength), mV.Y - 140), 2);
                Canvas.DrawLine(Color.Black, new Vector2(mV.X + 139, mV.Y + 140), new Vector2(mV.X + (140 + _mLineLength), mV.Y + 140), 2);
                Canvas.DrawLine(Color.Black, new Vector2(mV.X - 139, mV.Y - 140), new Vector2(mV.X - (140 + _mLineLength), mV.Y - 140), 2);
                Canvas.DrawLine(Color.Black, new Vector2(mV.X - 139, mV.Y + 140), new Vector2(mV.X - (140 + _mLineLength), mV.Y + 140), 2);
            }

            if (_mLineLength == 100)
            {
                if (_entryType > 1)
                {
                    String dexEntryText = _pokemon.pokedexEntry?.Text ?? String.Empty;
                    String dexEntrySpecies = _pokemon.pokedexEntry?.Species ?? String.Empty;
                    String formName = PokemonForms.GetFormName(_pokemon);
                    if (formName == String.Empty) formName = _pokemon.Name;

                    if (Localization.TokenExists("pokemon_desc_" + formName) == true)
                        dexEntryText = Localization.GetString("pokemon_desc_" + formName, dexEntryText);
                    if (Localization.TokenExists("pokemon_species_" + formName) == true)
                        dexEntrySpecies = Localization.GetString("pokemon_species_" + formName, dexEntrySpecies);

                    Core.SpriteBatch.DrawString(FontManager.MainFont, (_pokemon.pokedexEntry?.Height ?? 0) + " m", new Vector2((int)(mV.X + 250), (int)(mV.Y - 152)), Color.Black);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, (_pokemon.pokedexEntry?.Weight ?? 0) + " kg", new Vector2((int)(mV.X + 250), (int)(mV.Y + 128)), Color.Black);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, dexEntrySpecies, new Vector2((int)(mV.X - 248 - FontManager.MainFont.MeasureString(dexEntrySpecies).X), (int)(mV.Y - 152)), Color.Black);

                    if (_pokemon.Type2 != null && _pokemon.Type2.Type != Element.Types.Blank)
                    {
                        Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle((int)(mV.X - 450), (int)(mV.Y + 123), 96, 32), _pokemon.Type1.GetElementImage(), Color.White);
                        Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle((int)(mV.X - 350), (int)(mV.Y + 123), 96, 32), _pokemon.Type2.GetElementImage(), Color.White);
                    }
                    else
                    {
                        Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle((int)(mV.X - 350), (int)(mV.Y + 123), 96, 32), _pokemon.Type1.GetElementImage(), Color.White);
                    }

                    String croppedDesc = dexEntryText.CropStringToWidth(FontManager.MainFont, 720);
                    Vector2 descSize = FontManager.MainFont.MeasureString(croppedDesc);
                    Canvas.DrawRectangle(new Rectangle((int)(mV.X - descSize.X / 2 - 16), (int)(mV.Y + 192 - 16), (int)(descSize.X + 32), (int)(descSize.Y + 32)), new Color(42, 167, 198, 150));
                    Core.SpriteBatch.DrawString(FontManager.MainFont, croppedDesc, new Vector2((int)(mV.X - descSize.X / 2), (int)(mV.Y + 192)), Color.Black);
                }
                else
                {
                    Core.SpriteBatch.DrawString(FontManager.MainFont, "??? m", new Vector2((int)(mV.X + 250), (int)(mV.Y - 152)), Color.Black);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, "??? kg", new Vector2((int)(mV.X + 250), (int)(mV.Y + 128)), Color.Black);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, "??? Pokémon", new Vector2((int)(mV.X - 248 - FontManager.MainFont.MeasureString("??? Pokémon").X), (int)(mV.Y - 152)), Color.Black);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, "???", new Vector2((int)(mV.X - 248 - FontManager.MainFont.MeasureString("???").X), (int)(mV.Y + 128)), Color.Black);
                }
            }
        }

        Core.SpriteBatch.Draw(frontTex, new Rectangle((int)v.X, (int)v.Y - _yOffset * 2 + 32, Math.Min(frontTex.Width * 4, 512), Math.Min(frontTex.Height * 4, 512)), new Color(textureColor.R, textureColor.G, textureColor.B, textureColor.A));
    }

    private void DrawPage2()
    {
        if (_habitatList.Count == 0)
        {
            int cx = Core.windowSize.Width / 2;
            int cy = Core.windowSize.Height / 2;
            Canvas.DrawGradient(new Rectangle(cx - 282, cy - 45, 80, 90), new Color(84, 198, 216), new Color(42, 167, 198, 150), true, -1);
            Canvas.DrawRectangle(new Rectangle(cx - 202, cy - 45, 404, 90), new Color(42, 167, 198, 150));
            Canvas.DrawGradient(new Rectangle(cx + 202, cy - 45, 80, 90), new Color(42, 167, 198, 150), new Color(84, 198, 216), true, -1);
            String areaUnknown = Localization.GetString("pokedex_habitat_area_unknown", "Area Unknown.");
            Core.SpriteBatch.DrawString(FontManager.MainFont, areaUnknown, new Vector2(cx - (int)(FontManager.MainFont.MeasureString(areaUnknown).X / 2), cy - 15), Color.White);
        }
        else
        {
            for (int i = _scroll; i <= _scroll + 4; i++)
            {
                if (i > _habitatList.Count - 1) continue;
                int p = i - _scroll;

                Core.SpriteBatch.Draw(_texture, new Rectangle(100, 160 + p * 96, 64, 64), new Rectangle(16, 16, 16, 16), Color.White);
                Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64, 160 + p * 96, 64 * 9, 64), new Rectangle(32, 16, 16, 16), Color.White);
                Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64 * 10, 160 + p * 96, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

                Core.SpriteBatch.Draw(_habitatList[i].Texture, new Rectangle(120, 168 + p * 96, 64, 48), Color.White);
                Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("Places_" + _habitatList[i].Name, _habitatList[i].Name), new Vector2(200, 176 + p * 96), Color.Black);

                List<int> encounterTypes = [];
                for (int j = 0; j <= _habitatList[i].ObtainTypeList.Count - 1; j++)
                {
                    if (_habitatList[i].ObtainTypeList[j].PokemonID == PokemonForms.GetPokemonDataFileName(_pokemon.Number, _pokemon.AdditionalData) && encounterTypes.Contains(_habitatList[i].ObtainTypeList[j].EncounterType) == false)
                        encounterTypes.Add(_habitatList[i].ObtainTypeList[j].EncounterType);
                }
                for (int j = 0; j <= encounterTypes.Count - 1; j++)
                    Core.SpriteBatch.Draw(PokedexScreen.Habitat.GetEncounterTypeImage(encounterTypes[j]), new Rectangle(560 + j * 40, 176 + p * 96, 32, 32), Color.White);
            }
            DrawCursor();
        }
    }

    private void DrawCursor()
    {
        Vector2 cPosition = new Vector2(520, 160 + _cursor * 96 - 42);
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle((int)cPosition.X, (int)cPosition.Y, 64, 64), Color.White);
    }

    private void DrawPage3()
    {
        bool noEvo = _evolutionLineConnections == null || _evolutionLineConnections.Count == 0 ||
                     (_evolutionLineConnections.Count == 1 && _evolutionLineConnections[0].ConnectionList.Count <= 1);
        if (noEvo == true)
        {
            int cx = Core.windowSize.Width / 2;
            int cy = Core.windowSize.Height / 2;
            Canvas.DrawGradient(new Rectangle(cx - 282, cy - 45, 80, 90), new Color(84, 198, 216), new Color(42, 167, 198, 150), true, -1);
            Canvas.DrawRectangle(new Rectangle(cx - 202, cy - 45, 404, 90), new Color(42, 167, 198, 150));
            Canvas.DrawGradient(new Rectangle(cx + 202, cy - 45, 80, 90), new Color(42, 167, 198, 150), new Color(84, 198, 216), true, -1);
            String noEvoText = _pokemon.GetName(true) + " " + Localization.GetString("pokedex_evolution_no_evolutions", "doesn't evolve.");
            Core.SpriteBatch.DrawString(FontManager.MainFont, noEvoText, new Vector2(cx - (int)(FontManager.MainFont.MeasureString(noEvoText).X / 2), cy - 15), Color.White);
        }
        else
        {
            List<String> connectionLines = [];
            Vector2 centerVector = Core.GetMiddlePosition(new Size((int)(64 * _scale), (int)(64 * _scale)));

            for (int e = 0; e <= _evolutionLineConnections!.Count - 1; e++)
            {
                for (int l = 0; l <= _evolutionLineConnections[e].ConnectionList.Count - 1; l++)
                {
                    if (_evolutionLineConnections[e].ConnectionList.Count > 1 && l > 0)
                    {
                        connectionLines.Add($"{_evolutionLineConnections[e].ConnectionList[l - 1].Item1}_{_evolutionLineConnections[e].ConnectionList[l - 1].Item2},{_evolutionLineConnections[e].ConnectionList[l].Item1}_{_evolutionLineConnections[e].ConnectionList[l].Item2}");
                    }
                }
            }

            for (int i = 0; i <= connectionLines.Count - 1; i++)
            {
                Vector2 lineStart = new Vector2(int.Parse(connectionLines[i].GetSplit(0, ",").GetSplit(0, "_")), int.Parse(connectionLines[i].GetSplit(0, ",").GetSplit(1, "_")));
                Vector2 lineEnd = new Vector2(int.Parse(connectionLines[i].GetSplit(1, ",").GetSplit(0, "_")), int.Parse(connectionLines[i].GetSplit(1, ",").GetSplit(1, "_")));
                Canvas.DrawLine(Color.Black,
                    new Vector2(centerVector.X + (lineStart.X * (64 * _scale)) + (_scale * 32), centerVector.Y + (_scale * 32) + (lineStart.Y * (48 * _scale))),
                    new Vector2(centerVector.X + (lineEnd.X * (64 * _scale)) + (_scale * 32), centerVector.Y + (_scale * 32) + (lineEnd.Y * (48 * _scale))), 2);
            }

            for (int x = (int)_gridMinimum.X; x <= (int)_gridMaximum.X; x++)
            {
                for (int y = (int)_gridMinimum.Y; y <= (int)_gridMaximum.Y; y++)
                {
                    Pokemon? pokemon = null;
                    Vector2 position = Vector2.Zero;
                    for (int c = 0; c <= _evolutionLineConnections.Count - 1; c++)
                    {
                        for (int i = 0; i <= _evolutionLineConnections[c].ConnectionList.Count - 1; i++)
                        {
                            if (_evolutionLineConnections[c].ConnectionList[i].Item1 == x && _evolutionLineConnections[c].ConnectionList[i].Item2 == y)
                            {
                                position = new Vector2(_evolutionLineConnections[c].ConnectionList[i].Item1, _evolutionLineConnections[c].ConnectionList[i].Item2);
                                pokemon = _evolutionLineConnections[c].ConnectionList[i].Item3;
                            }
                        }
                    }
                    if (pokemon == null) continue;
                    String dexID = PokemonForms.GetPokemonDataFileName(pokemon.Number, pokemon.AdditionalData, true);
                    Texture2D pokeTexture = pokemon.GetMenuTexture();
                    Vector2 pokeScale = new Vector2((float)(32.0 / pokeTexture.Width * 2), (float)(32.0 / pokeTexture.Height * 2));
                    Color c2 = Pokedex.GetEntryType(Core.Player.PokedexData, dexID) == 0 ? Color.Black : Color.White;
                    Core.SpriteBatch.Draw(pokeTexture, new Rectangle((int)(centerVector.X + (position.X * (int)(64 * _scale))), (int)(centerVector.Y + (position.Y * (48 * _scale))), (int)(pokeTexture.Width * pokeScale.X * _scale), (int)(pokeTexture.Height * pokeScale.Y * _scale)), c2);
                }
            }

            for (int x = (int)_gridMinimum.X; x <= (int)_gridMaximum.X; x++)
            {
                for (int y = (int)_gridMinimum.Y; y <= (int)_gridMaximum.Y; y++)
                {
                    Pokemon? pokemon = null;
                    Vector2 position = Vector2.Zero;
                    for (int c = 0; c <= _evolutionLineConnections.Count - 1; c++)
                    {
                        for (int i = 0; i <= _evolutionLineConnections[c].ConnectionList.Count - 1; i++)
                        {
                            if (_evolutionLineConnections[c].ConnectionList[i].Item1 == x && _evolutionLineConnections[c].ConnectionList[i].Item2 == y)
                            {
                                position = new Vector2(_evolutionLineConnections[c].ConnectionList[i].Item1, _evolutionLineConnections[c].ConnectionList[i].Item2);
                                pokemon = _evolutionLineConnections[c].ConnectionList[i].Item3;
                            }
                        }
                    }
                    if (pokemon == null) continue;
                    String dexID = PokemonForms.GetPokemonDataFileName(pokemon.Number, pokemon.AdditionalData, true);
                    Texture2D pokeTexture = pokemon.GetMenuTexture();
                    Vector2 pokeScale = new Vector2((float)(32.0 / pokeTexture.Width * 2), (float)(32.0 / pokeTexture.Height * 2));
                    if (Pokedex.GetEntryType(Core.Player.PokedexData, dexID) == 0) continue;
                    String name = pokemon.GetName(true);
                    float nameScale = (int)(_scale / 2);
                    Vector2 namePos = new Vector2((int)(centerVector.X + (position.X * (64 * _scale)) + (int)(pokeTexture.Width * pokeScale.X / 2 * _scale) - (FontManager.MainFont.MeasureString(name).X / 2 * (float)(_scale / 2))), (int)(centerVector.Y + position.Y * (48 * _scale) + (64 * _scale)));
                    Core.SpriteBatch.DrawString(FontManager.MainFont, name, namePos + new Vector2(2), Color.Black, 0.0F, Vector2.Zero, nameScale, SpriteEffects.None, 0.0F);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, name, namePos, Color.White, 0.0F, Vector2.Zero, nameScale, SpriteEffects.None, 0.0F);
                }
            }
        }
    }

    public override void Update()
    {
        if (Controls.Dismiss(true, true, true) == true)
        {
            SoundManager.PlaySound("select");
            if (_transitionOut == true) Core.SetScreen(new TransitionScreen(this, PreScreen!, Color.White, false));
            else Core.SetScreen(PreScreen!);
        }

        if (_dexIndex > -1 && PreScreen?.Identification == Identifications.PokedexScreen)
        {
            PokedexScreen pDexScreen = (PokedexScreen)PreScreen;
            if (Controls.Up(true, true, false, true, true, true) == true && _dexIndex > 0)
            {
                int index = _dexIndex - 1;
                String pokeID = PokemonForms.GetPokemonDataFileName(pDexScreen.pokemonList[index].Number, pDexScreen.pokemonList[index].AdditionalData, true);
                String newID = String.Empty;
                while (newID == String.Empty && index > 0)
                {
                    int formEntry = Pokedex.HasAnyForm(pDexScreen.pokemonList[index].Number);
                    if (Pokedex.GetEntryType(Core.Player.PokedexData, pokeID) > 0 || formEntry > 0)
                    {
                        newID = formEntry > 0 ? pDexScreen.pokemonList[index].Number.ToString() : pokeID;
                        break;
                    }
                    index -= 1;
                    pokeID = PokemonForms.GetPokemonDataFileName(pDexScreen.pokemonList[index].Number, pDexScreen.pokemonList[index].AdditionalData, true);
                    if (Pokedex.GetEntryType(Core.Player.PokedexData, pokeID) > 0) { newID = pokeID; break; }
                }
                if (newID != String.Empty && (Pokedex.GetEntryType(Core.Player.PokedexData, newID) > 0 || Pokedex.HasAnyForm(pDexScreen.pokemonList[index].Number) > 0))
                {
                    _vLineLength = 1; _mLineLength = 1; _fadeMainImage = 0;
                    LoadPokemonData(index, null, true);
                }
            }

            if (Controls.Down(true, true, false, true, true, true) == true && _dexIndex < pDexScreen.pokemonList.Count - 1)
            {
                int index = _dexIndex + 1;
                String pokeID = PokemonForms.GetPokemonDataFileName(pDexScreen.pokemonList[index].Number, pDexScreen.pokemonList[index].AdditionalData, true);
                String newID = String.Empty;
                while (newID == String.Empty && index < pDexScreen.pokemonList.Count - 1)
                {
                    int formEntry = Pokedex.HasAnyForm(pDexScreen.pokemonList[index].Number);
                    if (Pokedex.GetEntryType(Core.Player.PokedexData, pokeID) > 0 || formEntry > 0)
                    {
                        newID = formEntry > 0 ? pDexScreen.pokemonList[index].Number.ToString() : pokeID;
                        break;
                    }
                    index += 1;
                    pokeID = PokemonForms.GetPokemonDataFileName(pDexScreen.pokemonList[index].Number, pDexScreen.pokemonList[index].AdditionalData, true);
                    if (Pokedex.GetEntryType(Core.Player.PokedexData, pokeID) > 0) { newID = pokeID; break; }
                }
                if (newID != String.Empty && (Pokedex.GetEntryType(Core.Player.PokedexData, newID) > 0 || Pokedex.HasAnyForm(pDexScreen.pokemonList[index].Number) > 0))
                {
                    _vLineLength = 1; _mLineLength = 1; _fadeMainImage = 0;
                    LoadPokemonData(index, null, true);
                }
            }
        }

        if (Controls.ShiftPressed() == true || ControllerHandler.ButtonPressed(Buttons.Back) == true)
        {
            if (_forms.Count > 0)
            {
                int originalFormIndex = _formIndex;
                _formIndex += 1;
                if (_formIndex > _forms.Count - 1) _formIndex = 0;

                String formID = _forms[_formIndex];
                if (formID != String.Empty && Pokedex.GetEntryType(Core.Player.PokedexData, formID) == 0)
                {
                    while (Pokedex.GetEntryType(Core.Player.PokedexData, formID) == 0)
                    {
                        _formIndex += 1;
                        if (_formIndex > _forms.Count - 1) _formIndex = 0;
                        formID = _forms[_formIndex];
                        if (_formIndex == originalFormIndex) break;
                    }
                }

                if (_forms[_formIndex] != String.Empty)
                {
                    int pokeID = int.Parse(_forms[_formIndex].GetSplit(0, "_").GetSplit(0, ";"));
                    String pokeAD = String.Empty;
                    if (_forms[_formIndex].Contains("_") == true) pokeAD = PokemonForms.GetAdditionalValueFromDataFile(_forms[_formIndex]);
                    else if (_forms[_formIndex].Contains(";") == true) pokeAD = _forms[_formIndex].GetSplit(1, ";");

                    Pokemon newPokemon = Pokemon.GetPokemonByID(pokeID, pokeAD, true);
                    bool playCry = _formIndex != originalFormIndex;
                    LoadPokemonData(-1, newPokemon, playCry);
                }
            }
        }

        UpdateIntro();

        if (_mLineLength == 100)
        {
            if (Controls.Right(true, true, false, true, true, true) == true) _page += 1;
            if (Controls.Left(true, true, false, true, true, true) == true) _page -= 1;

            if (Controls.Accept(true, false, false) == true)
            {
                if ((_page == 0 || _page == 1) && new Rectangle(Core.windowSize.Width - 70, Core.windowSize.Height / 2 - 32, 64, 64).Contains(MouseHandler.MousePosition) == true)
                {
                    SoundManager.PlaySound("select");
                    _page += 1;
                }
                if ((_page == 1 || _page == 2) && new Rectangle(6, Core.windowSize.Height / 2 - 32, 64, 64).Contains(MouseHandler.MousePosition) == true)
                {
                    SoundManager.PlaySound("select");
                    _page -= 1;
                }
            }

            _page = Math.Clamp(_page, 0, 2);

            switch (_page)
            {
                case 0: UpdatePage1(); break;
                case 1: UpdatePage2(); break;
                case 2: UpdatePage3(); break;
            }
        }

        PokedexScreen.tileOffset = (PokedexScreen.tileOffset + 1) % 64;
    }

    private void UpdateIntro()
    {
        if (_fadeMainImage < 255)
        {
            _fadeMainImage += 10;
            if (_fadeMainImage >= 255)
            {
                _fadeMainImage = 255;
                if (_playedCry == false)
                {
                    _playedCry = true;
                    String crySuffix = PokemonForms.GetCrySuffix(_pokemon);
                    SoundManager.PlayPokemonCry(_pokemon.Number, crySuffix);
                }
            }
        }
        else
        {
            if (_vLineLength < 140)
            {
                _vLineLength += 10;
                if (_vLineLength >= 140) _vLineLength = 140;
            }
            else if (_mLineLength < 100)
            {
                _mLineLength += 10;
                if (_mLineLength >= 100) _mLineLength = 100;
            }
        }
    }

    private void UpdatePage1()
    {
        if (Controls.Accept(true, true, true) == true)
        {
            SoundManager.PlaySound("select");
            if (_entryType > 2)
            {
                if (_frontView == false) _shinyView = !_shinyView;
                _frontView = !_frontView;
            }
            else
            {
                _frontView = !_frontView;
            }
            GetYOffset();
        }
    }

    private void UpdatePage2()
    {
        if (_habitatList.Count == 0) return;

        if (Controls.Down(true, true, true, true, true, true) == true) _cursor += 1;
        if (Controls.Up(true, true, true, true, true, true) == true) _cursor -= 1;

        if (_cursor > 4) { _cursor = 4; _scroll += 1; }
        if (_cursor < 0) { _cursor = 0; _scroll -= 1; }

        _scroll = _habitatList.Count < 6 ? 0 : Math.Clamp(_scroll, 0, _habitatList.Count - 5);
        _cursor = _habitatList.Count < 5 ? Math.Clamp(_cursor, 0, _habitatList.Count - 1) : Math.Clamp(_cursor, 0, 4);

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = _scroll; i <= _scroll + 4; i++)
            {
                if (i > _habitatList.Count - 1) continue;
                if (new Rectangle(100, 160 + (i - _scroll) * 96, 640, 64).Contains(MouseHandler.MousePosition) == false) continue;
                if (i == _cursor + _scroll) { SoundManager.PlaySound("select"); Core.SetScreen(new PokedexScreen(this, null, _habitatList[_cursor + _scroll])); }
                else { _cursor = i - _scroll; }
            }
        }

        if (Controls.Accept(false, true, true) == true)
        {
            SoundManager.PlaySound("select");
            Core.SetScreen(new PokedexScreen(this, null, _habitatList[_cursor + _scroll]));
        }
    }

    private void UpdatePage3()
    {
        if (Controls.Accept(true, false, false) == true && _dexIndex > -1)
        {
            Vector2 centerVector = Core.GetMiddlePosition(new Size((int)(64 * _scale), (int)(64 * _scale)));

            Pokemon? clickedPokemon = null;
            Vector2 mPosition = MouseHandler.MousePosition.ToVector2();
            for (int c = 0; c <= _evolutionLineConnections!.Count - 1; c++)
            {
                for (int i = 0; i <= _evolutionLineConnections[c].ConnectionList.Count - 1; i++)
                {
                    Texture2D tex = _evolutionLineConnections[c].ConnectionList[i].Item3.GetMenuTexture();
                    Vector2 ps = new Vector2((float)(32.0 / tex.Width * 2), (float)(32.0 / tex.Height * 2));
                    Rectangle rect = new Rectangle(
                        (int)(centerVector.X + _evolutionLineConnections[c].ConnectionList[i].Item1 * (int)(64 * _scale)),
                        (int)(centerVector.Y + _evolutionLineConnections[c].ConnectionList[i].Item2 * (48 * _scale)),
                        (int)(tex.Width * ps.X * _scale), (int)(tex.Height * ps.Y * _scale));
                    if (rect.Contains(mPosition) == true) clickedPokemon = _evolutionLineConnections[c].ConnectionList[i].Item3;
                }
            }

            if (clickedPokemon != null)
            {
                PokedexScreen pDexScreen = (PokedexScreen)PreScreen!;
                String dexID = PokemonForms.GetPokemonDataFileName(clickedPokemon.Number, clickedPokemon.AdditionalData, true);
                List<String> tempForms = [];
                int switchIndex = _dexIndex;

                if (clickedPokemon.Number != _pokemon.Number)
                {
                    for (int pEntry = 0; pEntry <= pDexScreen.pokemonList.Count - 1; pEntry++)
                    {
                        if (pDexScreen.pokemonList[pEntry].Number == clickedPokemon.Number && pDexScreen.pokemonList[pEntry].AdditionalData == clickedPokemon.AdditionalData)
                        { switchIndex = pEntry; break; }
                    }
                    if (switchIndex == _dexIndex)
                    {
                        for (int pEntry = 0; pEntry <= pDexScreen.pokemonList.Count - 1; pEntry++)
                        {
                            if (pDexScreen.pokemonList[pEntry].Number == clickedPokemon.Number) { switchIndex = pEntry; break; }
                        }
                    }
                    tempForms.Add(clickedPokemon.Number.ToString());
                    if (clickedPokemon.evolutionLines.Count > 0)
                    {
                        foreach (String eLine in clickedPokemon.evolutionLines)
                        {
                            List<String> pokemonIDs = [];
                            foreach (String entry in eLine.Split(',')) pokemonIDs.Add(entry.GetSplit(0, "\\"));
                            foreach (String f in pokemonIDs)
                            {
                                if (int.Parse(f.GetSplit(0, "_").GetSplit(0, ";")) == clickedPokemon.Number && _forms.Contains(f) == false)
                                    tempForms.Add(f);
                            }
                        }
                    }
                    else tempForms.AddRange(_forms);
                }
                else
                {
                    if (_forms.Count > 0) tempForms.AddRange(_forms);
                }

                int fIndex = -1;
                for (int f = 0; f <= tempForms.Count - 1; f++)
                {
                    if (tempForms[f] == dexID && Pokedex.GetEntryType(Core.Player.PokedexData, dexID) > 0) fIndex = f;
                }

                if (fIndex != -1 && tempForms[fIndex] != String.Empty)
                {
                    _formIndex = fIndex;
                    int pokeID = int.Parse(tempForms[_formIndex].GetSplit(0, "_").GetSplit(0, ";"));
                    String pokeAD = String.Empty;
                    if (tempForms[_formIndex].Contains("_") == true) pokeAD = PokemonForms.GetAdditionalValueFromDataFile(tempForms[_formIndex]);
                    else if (tempForms[_formIndex].Contains(";") == true) pokeAD = tempForms[_formIndex].GetSplit(1, ";");
                    Pokemon newPokemon = Pokemon.GetPokemonByID(pokeID, pokeAD, true);
                    _forms = tempForms;
                    LoadPokemonData(switchIndex, newPokemon, true);
                }
                else if (_dexIndex > -1)
                {
                    String switchDexID = PokemonForms.GetPokemonDataFileName(pDexScreen.pokemonList[switchIndex].Number, pDexScreen.pokemonList[switchIndex].AdditionalData, true);
                    if (Pokedex.GetEntryType(Core.Player.PokedexData, switchDexID) > 0)
                    {
                        _vLineLength = 1; _mLineLength = 1; _fadeMainImage = 0;
                        LoadPokemonData(switchIndex, null, true);
                    }
                }
            }
        }

        if (Controls.Up(true, false, true, false, false, false) == true || ControllerHandler.ButtonPressed(Buttons.RightTrigger) == true || KeyBoardHandler.KeyPressed(Keys.OemPlus) == true)
            _scale += 0.5F;
        if (Controls.Down(true, false, true, false, false, false) == true || ControllerHandler.ButtonPressed(Buttons.LeftTrigger) == true || KeyBoardHandler.KeyPressed(Keys.OemMinus) == true)
            _scale -= 0.5F;

        _scale = Math.Clamp(_scale, 0.5F, 4.0F);
    }
}
