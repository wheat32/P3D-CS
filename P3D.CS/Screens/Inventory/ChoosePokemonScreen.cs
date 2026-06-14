using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;
using P3D.BattleSystem;
using P3D.Items;
using P3D.Screens.UI;

namespace P3D;

public class ChoosePokemonScreen : Screen
{
    private List<Pokemon> _pokemonList = [];
    private List<Pokemon>? _altPokemonList;

    public static int Selected = -1;
    public static bool Exited = false;

    public int index = 0;
    private Texture2D _mainTexture = null!;
    private Texture2D _texture = null!;
    private float _yOffset = 0;

    private Item? _item;
    private String _title = String.Empty;

    private bool _used = false;
    private bool _canExit = true;

    public bool CanChooseFainted = true;
    public bool CanChooseEgg = true;
    public bool CanChooseHMPokemon = true;

    private Action<int>? _choosePokemon;
    public Action<int>? ExitedSub;

    public Attack? LearnAttack;
    public int LearnType = 0;
    private Object? _moveLearnArg = null;

    public ChoosePokemonScreen(Screen currentScreen, Item? item, Action<int>? choosePokemon, String title, bool canExit, bool canChooseFainted, bool canChooseEgg, List<Pokemon>? pokemonList = null)
    {
        PreScreen = currentScreen;
        Identification = Identifications.ChoosePokemonScreen;
        MouseVisible = false;
        CanChat = PreScreen.CanChat;
        CanBePaused = PreScreen.CanBePaused;

        _item = item;
        _title = title;
        _canExit = canExit;
        CanChooseEgg = canChooseEgg;
        CanChooseFainted = canChooseFainted;

        _mainTexture = TextureManager.GetTexture("GUI\\Menus\\Menu");
        _texture = TextureManager.GetTexture(_mainTexture, new Rectangle(0, 0, 48, 48));

        index = Player.Temp.PokemonScreenIndex;
        _choosePokemon = choosePokemon;
        _altPokemonList = pokemonList;

        GetPokemonList();
    }

    public ChoosePokemonScreen(Screen currentScreen, Item? item, Action<int>? choosePokemon, String title, bool canExit)
        : this(currentScreen, item, choosePokemon, title, canExit, true, true) { }

    public ChoosePokemonScreen(Screen currentScreen, Item? item, String title, bool canExit)
        : this(currentScreen, item, null, title, canExit, true, true) { }

    private void GetPokemonList()
    {
        _pokemonList.Clear();
        if (_altPokemonList != null)
        {
            foreach (Pokemon p in _altPokemonList)
                _pokemonList.Add(Pokemon.GetPokemonByData(p.GetSaveData()));
        }
        else
        {
            foreach (Pokemon p in Core.Player.Pokemons)
                _pokemonList.Add(Pokemon.GetPokemonByData(p.GetSaveData()));
        }
    }

    public override void Update()
    {
        TextBox.Update();
        _yOffset += 0.45F;

        if (TextBox.Showing == false)
        {
            if (_used == true)
            {
                Core.SetScreen(PreScreen!);
            }
            else
            {
                if (ChooseBox.Showing == false)
                {
                    if (Controls.Dismiss() == true && _canExit == true)
                    {
                        Exited = true;
                        Selected = -1;
                        if (ExitedSub != null)
                        {
                            _used = true;
                            ExitedSub(index);
                        }
                        else
                        {
                            Core.SetScreen(PreScreen!);
                        }
                    }

                    if (Controls.Accept() == true)
                        ShowMenu();

                    if (Controls.Right(true, false) == true) index += 1;
                    if (Controls.Left(true, false) == true) index -= 1;
                    if (Controls.Down(true, false, false) == true) index += 2;
                    if (Controls.Up(true, false, false) == true) index -= 2;

                    index = (int)MathHelper.Clamp(index, 0, _pokemonList.Count - 1);
                }
                else
                {
                    ChooseBox.Update();
                    if (Controls.Dismiss() == true)
                        ChooseBox.Showing = false;
                    if (Controls.Accept() == true)
                        AcceptMenu();
                }
            }
        }
    }

    private void AcceptMenu()
    {
        switch (ChooseBox.result)
        {
            case 0:
                if (CanChoosePokemon(_pokemonList[index]) == true)
                {
                    Player.Temp.PokemonScreenIndex = index;
                    ChooseBox.Showing = false;
                    Selected = index;
                    if (_choosePokemon != null)
                    {
                        _choosePokemon(index);
                        GetPokemonList();
                    }
                    _used = true;
                    Exited = false;
                }
                else
                {
                    ChooseBox.Showing = false;
                    TextBox.Show("Cannot choose this~Pokémon.");
                }
                break;
            case 1:
                ChooseBox.Showing = false;
                Core.SetScreen(new PokemonStatusScreen(this, index, [], _pokemonList[index], true));
                break;
            case 2:
                ChooseBox.Showing = false;
                break;
        }
    }

    private bool CanChoosePokemon(Pokemon p)
    {
        if (CanChooseFainted == false && (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted))
            return false;
        if (CanChooseEgg == false && p.IsEgg == true)
            return false;
        if (CanChooseHMPokemon == false && p.HasHMMove() == true)
            return false;
        return true;
    }

    private void ShowMenu()
    {
        ChooseBox.Show([Localization.GetString("global_select", "Select"), Localization.GetString("pokemon_screen_summary"), Localization.GetString("pokemon_screen_back")], 0, []);
    }

    public override void Draw()
    {
        PreScreen!.Draw();

        Canvas.DrawImageBorder(_texture, 2, new Rectangle(60, 100, 800, 480));
        Canvas.DrawImageBorder(_texture, 2, new Rectangle(60, 100, 480, 64));
        Core.SpriteBatch.DrawString(FontManager.InGameFont, _title, new Vector2(142, 132), Color.Black);
        if (_item != null)
            Core.SpriteBatch.Draw(_item.Texture, new Rectangle(78, 124, 48, 48), Color.White);

        if (_canExit == true)
            Core.SpriteBatch.DrawString(FontManager.MiniFont, "Press the E key to go back.", new Vector2(640, 580), Color.DarkGray);

        for (int i = 0; i <= _pokemonList.Count - 1; i++)
            DrawPokemonTile(i, _pokemonList[i]);
        if (_pokemonList.Count < 6)
        {
            for (int i = _pokemonList.Count; i <= 5; i++)
                DrawEmptyTile(i);
        }

        if (ChooseBox.Showing == true)
        {
            Vector2 position = new Vector2(0, 0);
            switch (index)
            {
                case 0: case 2: case 4: position = new Vector2(606, 566 - ChooseBox.Options.Length * 48); break;
                case 1: case 3: case 5: position = new Vector2(60, 566 - ChooseBox.Options.Length * 48); break;
            }
            ChooseBox.Draw(position, true, 1.0F);
        }

        TextBox.Draw();
    }

    private void DrawEmptyTile(int i)
    {
        Vector2 p = i == 0 || i == 2 || i == 4
            ? new Vector2(32, 32 + (48 + 10) * i)
            : new Vector2(416, 32 + (48 + 10) * (i - 1));
        p.X += 80;
        p.Y += 180;

        Core.SpriteBatch.Draw(_texture, new Rectangle((int)p.X, (int)p.Y, 32, 96), new Rectangle(0, 0, 16, 48), Color.White);
        for (float x = p.X + 32; x <= p.X + 288; x += 32)
            Core.SpriteBatch.Draw(_texture, new Rectangle((int)x, (int)p.Y, 32, 96), new Rectangle(16, 0, 16, 48), Color.White);
        Core.SpriteBatch.Draw(_texture, new Rectangle((int)p.X + 320, (int)p.Y, 32, 96), new Rectangle(32, 0, 16, 48), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MiniFont, "EMPTY", new Vector2((int)(p.X + 72), (int)(p.Y + 18)), Color.Black);
    }

    private void DrawPokemonTile(int i, Pokemon pokemon)
    {
        Texture2D borderTexture;
        if (i == index)
        {
            borderTexture = pokemon.Status == Pokemon.StatusProblems.Fainted
                ? TextureManager.GetTexture(_mainTexture, new Rectangle(0, 128, 48, 48))
                : TextureManager.GetTexture(_mainTexture, new Rectangle(48, 0, 48, 48));
        }
        else
        {
            borderTexture = pokemon.Status == Pokemon.StatusProblems.Fainted
                ? TextureManager.GetTexture(_mainTexture, new Rectangle(48, 48, 48, 48))
                : TextureManager.GetTexture(_mainTexture, new Rectangle(0, 0, 48, 48));
        }

        Vector2 p = i == 0 || i == 2 || i == 4
            ? new Vector2(32, 32 + (48 + 10) * i)
            : new Vector2(416, 32 + (48 + 10) * (i - 1));
        p.X += 80;
        p.Y += 180;

        Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)p.X, (int)p.Y, 32, 96), new Rectangle(0, 0, 16, 48), Color.White);
        for (float x = p.X + 32; x <= p.X + 288; x += 32)
            Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)x, (int)p.Y, 32, 96), new Rectangle(16, 0, 16, 48), Color.White);
        Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)p.X + 320, (int)p.Y, 32, 96), new Rectangle(32, 0, 16, 48), Color.White);

        if (pokemon.IsEgg == false)
        {
            int barX = (int)((pokemon.HP / pokemon.MaxHP) * 50);
            int barPercentage = (int)((pokemon.HP / pokemon.MaxHP) * 100);

            Rectangle barRectangle = barPercentage >= 50
                ? new Rectangle(113, 0, 1, 4)
                : barPercentage > 10 ? new Rectangle(116, 0, 1, 4) : new Rectangle(115, 0, 1, 4);

            for (int x = 0; x <= barX - 1; x++)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + (x * 2) + 104), (int)(p.Y + 44), 4, 16), barRectangle, Color.White);
            for (int x = barX; x <= 49; x++)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + (x * 2) + 104), (int)(p.Y + 44), 4, 16), new Rectangle(114, 0, 1, 4), Color.White);
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + 100), (int)(p.Y + 44), 4, 16), new Rectangle(112, 0, 1, 4), Color.White);
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + 206), (int)(p.Y + 44), 4, 16), new Rectangle(112, 0, 1, 4), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MiniFont, pokemon.HP + " / " + pokemon.MaxHP, new Vector2((int)(p.X + 120), (int)(p.Y + 64)), Color.Black);
        }

        float offset = (float)Math.Sin(_yOffset);
        if (i == index) offset *= 3;
        if (pokemon.Status == Pokemon.StatusProblems.Fainted) offset = 0;

        Core.SpriteBatch.Draw(pokemon.GetMenuTexture(), new Rectangle((int)(p.X + 5), (int)(p.Y + offset + 10), 64, 64), BattleStats.GetStatColor(pokemon.Status));
        Core.SpriteBatch.DrawString(FontManager.MiniFont, pokemon.GetDisplayName(), new Vector2((int)(p.X + 72), (int)(p.Y + 18)), Color.Black);

        if (pokemon.IsEgg == false)
        {
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + 72), (int)(p.Y + 46), 26, 12), new Rectangle(96, 10, 13, 6), Color.White);

            if (pokemon.Gender == Pokemon.Genders.Male)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + FontManager.MiniFont.MeasureString(pokemon.GetDisplayName()).X + 80), (int)(p.Y + 18), 12, 20), new Rectangle(96, 0, 6, 10), Color.White);
            else if (pokemon.Gender == Pokemon.Genders.Female)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + FontManager.MiniFont.MeasureString(pokemon.GetDisplayName()).X + 80), (int)(p.Y + 18), 12, 20), new Rectangle(102, 0, 6, 10), Color.White);
        }

        if (pokemon.Item != null && pokemon.IsEgg == false)
            Core.SpriteBatch.Draw(pokemon.Item.Texture, new Rectangle((int)(p.X + 40), (int)(p.Y + 42), 24, 24), Color.White);

        String space = String.Empty;
        for (int x = 1; x <= 3 - pokemon.Level.ToString().Length; x++)
            space += " ";

        String attackLabel = String.Empty;
        if (LearnType > 0)
        {
            attackLabel = "Unable!";
            if (LearnType == 1 && _moveLearnArg is TechMachine tm && tm.CanTeach(pokemon) == String.Empty)
                attackLabel = "Able!";
        }

        if (pokemon.IsEgg == false)
        {
            Core.SpriteBatch.DrawString(FontManager.MiniFont, "Lv." + space + pokemon.Level, new Vector2((int)(p.X + 14), (int)(p.Y + 64)), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MiniFont, attackLabel, new Vector2((int)(p.X + 230), (int)(p.Y + 64)), Color.Black);
        }

        Texture2D? statusTexture = BattleStats.GetStatImage(pokemon.Status);
        if (statusTexture != null)
        {
            Canvas.DrawRectangle(new Rectangle((int)(p.X + 216), (int)(p.Y + 44), 42, 16), Color.Gray);
            Core.SpriteBatch.Draw(statusTexture, new Rectangle((int)(p.X + 218), (int)(p.Y + 46), 38, 12), Color.White);
        }
    }

    public void SetupLearnAttack(Attack attack, int learnType, Object arg)
    {
        LearnAttack = attack;
        LearnType = learnType;
        _moveLearnArg = arg;
    }
}
