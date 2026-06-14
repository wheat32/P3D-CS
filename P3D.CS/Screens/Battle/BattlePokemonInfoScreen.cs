using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Items;

namespace P3D;

public class BattlePokemonInfoScreen : Screen
{
    public delegate void DoStuff(int pokeIndex);

    private Pokemon _pokemon;
    private int _index = 0;
    private Texture2D _mainTexture;
    private DoStuff _choosePokemon;
    private int _pokeIndex = 0;
    private BattleSystem.BattleScreen _battleScreen;

    private bool _isItem = false;
    private int _pokeSize = 128;
    private int _itemSize = 0;

    public BattlePokemonInfoScreen(Screen currentScreen, int pokeIndex, DoStuff choosePokemon, BattleSystem.BattleScreen battleScreen)
    {
        PreScreen = currentScreen;
        Identification = Identifications.BattlePokemonScreen;
        _choosePokemon = choosePokemon;
        _pokeIndex = pokeIndex;
        _pokemon = Core.Player.Pokemons[pokeIndex];
        _battleScreen = battleScreen;

        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");

        Logger.Debug(currentScreen.Identification.ToString());
    }

    public override void Update()
    {
        if (Controls.Up(true, true, true, true) == true)
        {
            _index -= 1;
        }
        if (Controls.Down(true, true, true, true) == true)
        {
            _index += 1;
        }

        if (_index < 0)
        {
            _index = 3;
        }
        else if (_index > 3)
        {
            _index = 0;
        }

        if (_isItem == true)
        {
            if (_pokeSize > 0)
            {
                _pokeSize -= 10;
            }
            if (_itemSize < 100)
            {
                _itemSize += 10;
            }
        }
        else
        {
            if (_pokeSize < 128)
            {
                _pokeSize += 10;
            }
            if (_itemSize > 0)
            {
                _itemSize -= 10;
            }
        }

        if (Controls.Dismiss(true, true) == true)
        {
            Core.SetScreen(PreScreen);
        }

        if (Controls.Accept(true, true) == true)
        {
            if (_isItem == true)
            {
                switch (_index)
                {
                    case 0:
                        Core.SetScreen(PreScreen);
                        Item i = Core.Player.Pokemons[_pokeIndex].Item;
                        Core.Player.Inventory.AddItem(i.ID.ToString(), 1);
                        Core.Player.Pokemons[_pokeIndex].Item = null;
                        break;
                    case 1:
                        break;
                    case 2:
                        _isItem = false;
                        _index = 2;
                        break;
                }
            }
            else
            {
                switch (_index)
                {
                    case 0:
                        Core.SetScreen(PreScreen);
                        _choosePokemon(_pokeIndex);
                        break;
                    case 1:
                        Core.SetScreen(new PokemonStatusScreen(this, _pokeIndex, [], Core.Player.Pokemons[_pokeIndex], true));
                        break;
                    case 2:
                        if (_pokemon.Item != null)
                        {
                            _isItem = true;
                            _index = 0;
                        }
                        break;
                    case 3:
                        Core.SetScreen(PreScreen);
                        break;
                }
            }
        }
    }

    public override void Draw()
    {
        PreScreen.Draw();

        Canvas.DrawRectangle(new Rectangle(0, 0, Core.ScreenSize.Width, Core.ScreenSize.Height), new Color(0, 0, 0, 150));

        DrawPreview();
        if (_isItem == false)
        {
            DrawMenuPokemon();
        }
        else
        {
            DrawMenuItem();
        }
    }

    private void DrawPreview()
    {
        Texture2D t = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Canvas.DrawImageBorder(t, 2, new Rectangle(Core.windowSize.Width / 2 - 70, 48, 96, 96));
        Core.SpriteBatch.Draw(_pokemon.GetTexture(true), new Rectangle(Core.windowSize.Width / 2 - 70, 32, _pokeSize, _pokeSize), Color.White);
        if (_pokemon.Item != null)
        {
            Core.SpriteBatch.Draw(_pokemon.Item.Texture, new Rectangle(Core.windowSize.Width / 2 - 70, 64, _itemSize, _itemSize), Color.White);
        }
    }

    private void DrawMenuItem()
    {
        String[] labels = ["Give", "Take", "Back"];
        for (int i = 0; i <= 2; i++)
        {
            Texture2D t = i == _index
                ? TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty)
                : TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

            Canvas.DrawImageBorder(t, 2, new Rectangle(Core.windowSize.Width / 2 - 180, 180 + 128 * i, 320, 64));
            String text = labels[i];
            Core.SpriteBatch.DrawString(FontManager.InGameFont, text,
                new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.InGameFont.MeasureString(text).X / 2) - 7, 215 + 128 * i), Color.Black);
        }
    }

    private void DrawMenuPokemon()
    {
        String[] labels = ["Switch", "Summary", "Item", "Back"];
        for (int i = 0; i <= 3; i++)
        {
            Texture2D t = i == _index
                ? TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty)
                : TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

            Canvas.DrawImageBorder(t, 2, new Rectangle(Core.windowSize.Width / 2 - 180, 180 + 128 * i, 320, 64));
            String text = labels[i];
            Core.SpriteBatch.DrawString(FontManager.InGameFont, text,
                new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.InGameFont.MeasureString(text).X / 2) - 7, 215 + 128 * i), Color.Black);
        }
    }
}
