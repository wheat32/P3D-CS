using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;
using P3D.Items;
using P3D.Screens.UI;

namespace P3D;

public class ItemDetailScreen : Screen
{
    private Item _item;
    private bool _canUse = true;

    private int _index = 0;
    private int _trashValue = 1;
    private Texture2D _mainTexture = null!;

    private List<String> _menuItems = [];

    public ItemDetailScreen(Screen currentScreen, Item item, bool canUse)
    {
        PreScreen = currentScreen;
        Identification = Identifications.ItemDetailScreen;

        _item = item;
        _canUse = canUse;

        _mainTexture = TextureManager.GetTexture("GUI\\Menus\\Menu");
    }

    public override void Draw()
    {
        PreScreen!.Draw();
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.ScreenSize.Width, Core.ScreenSize.Height), new Color(0, 0, 0, 150));

        DrawItem();
        if (_menuItems.Count > 0)
            DrawMenu();

        TextBox.Draw();
    }

    private void DrawItem()
    {
        Canvas.DrawImageBorder(TextureManager.GetTexture(_mainTexture, new Rectangle(0, 0, 48, 48)), 2, new Rectangle((int)(Core.windowSize.Width / 2) - 84, 64, 128, 128));
        Core.SpriteBatch.Draw(_item.Texture, new Rectangle((int)(Core.windowSize.Width / 2) - 56, 96, 96, 96), Color.White);
    }

    private void DrawMenu()
    {
        for (int i = 0; i <= _menuItems.Count - 1; i++)
        {
            String text = _menuItems[i];

            Texture2D canvasTexture = i == _index
                ? TextureManager.GetTexture(_mainTexture, new Rectangle(0, 48, 48, 48))
                : TextureManager.GetTexture(_mainTexture, new Rectangle(0, 0, 48, 48));

            int offSetX = 0;
            if (_menuItems.Count > 3)
                offSetX = i < 2 ? -180 : 180;

            int offSetY = i * 128;
            if (_menuItems.Count > 3 && i > 1)
                offSetY -= 2 * 128;

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.windowSize.Width / 2) - 180 + offSetX, 240 + offSetY, 320, 64));
            Core.SpriteBatch.DrawString(FontManager.InGameFont, text, new Vector2((int)(Core.windowSize.Width / 2) - (FontManager.InGameFont.MeasureString(text).X / 2) - 10 + offSetX, 276 + offSetY), Color.Black);

            if (_menuItems[i] == Localization.GetString("item_detail_screen_trash"))
            {
                String trashText = _trashValue + "/" + Core.Player.Inventory.GetItemAmount(_item.ID.ToString());
                Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.windowSize.Width / 2) + 180 + offSetX, 240 + offSetY, 128, 64));
                Core.SpriteBatch.DrawString(FontManager.InGameFont, trashText, new Vector2((int)(Core.windowSize.Width / 2) - (FontManager.InGameFont.MeasureString(trashText).X / 2) + 256 + offSetX, 276 + offSetY), Color.Black);
            }
        }
    }

    public override void Update()
    {
        TextBox.Update();

        if (TextBox.Showing == false)
        {
            if (Core.Player.Inventory.GetItemAmount(_item.ID.ToString()) == 0)
            {
                Core.SetScreen(PreScreen!);
                return;
            }

            if (_menuItems.Count == 0)
                CreateMenuItems();

            if (Controls.Down(true, true, false, true) == true)
            {
                if (Controls.ShiftDown() == true)
                {
                    if (_menuItems[_index] == Localization.GetString("item_detail_screen_trash"))
                        _trashValue += 1;
                }
                else
                {
                    _index += 1;
                }
            }
            if (Controls.Up(true, true, false, true) == true)
            {
                if (Controls.ShiftDown() == true)
                {
                    if (_menuItems[_index] == Localization.GetString("item_detail_screen_trash"))
                        _trashValue -= 1;
                }
                else
                {
                    _index -= 1;
                }
            }

            int increment = _menuItems.Count > 3 ? 2 : 1;
            if (Controls.Right(true, true, false, true) == true) _index += increment;
            if (Controls.Left(true, true, false, true) == true) _index -= increment;

            _index = (int)MathHelper.Clamp(_index, 0, _menuItems.Count - 1);

            if (_menuItems[_index] == Localization.GetString("item_detail_screen_trash"))
            {
                if (Controls.Right(true, false, true, false) == true) _trashValue += 1;
                if (Controls.Left(true, false, true, false) == true) _trashValue -= 1;
                _trashValue = (int)MathHelper.Clamp(_trashValue, 1, Core.Player.Inventory.GetItemAmount(_item.ID.ToString()));
            }

            if (Controls.Accept() == true)
            {
                String selected = _menuItems[_index];
                if (selected == Localization.GetString("item_detail_screen_use"))
                {
                    _item.Use();
                }
                else if (selected == Localization.GetString("item_detail_screen_give"))
                {
                    PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, _item, GiveItem, Localization.GetString("item_detail_screen_give_item") + _item.Name, true);
                    selScreen.Mode = ISelectionScreen.ScreenMode.Selection;
                    selScreen.CanExit = true;
                    selScreen.SelectedObject += GiveItemHandler;
                    Core.SetScreen(selScreen);
                }
                else if (selected == Localization.GetString("item_detail_screen_trash"))
                {
                    Core.Player.Inventory.RemoveItem(_item.ID.ToString(), _trashValue);
                }
                else if (selected == Localization.GetString("item_detail_screen_back"))
                {
                    Core.SetScreen(PreScreen!);
                }
            }

            if (Controls.Dismiss() == true)
                Core.SetScreen(PreScreen!);
        }
    }

    private void GiveItemHandler(Object[] args)
    {
        GiveItem((int)args[0]);
    }

    private void GiveItem(int pokeIndex)
    {
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];

        if (pokemon.IsEgg == false)
        {
            Core.Player.Inventory.RemoveItem(_item.ID.ToString(), 1);

            Item? reItem = null;
            if (pokemon.Item != null)
            {
                reItem = pokemon.Item;
                Core.Player.Inventory.AddItem(reItem.ID.ToString(), 1);
            }

            pokemon.Item = _item;
            TextBox.reDelay = 0.0F;

            String t = Localization.GetString("pokemon_screen_give_item_1") + _item.Name + Localization.GetString("pokemon_screen_give_item_2") + pokemon.GetDisplayName() + Localization.GetString("pokemon_screen_give_item_3");
            if (reItem != null)
                t += Localization.GetString("pokemon_screen_give_item_4") + reItem.Name + Localization.GetString("pokemon_screen_give_item_5");
            else
                t += ".";

            TextBox.Show(t);
        }
        else
        {
            TextBox.Show("Eggs cannot hold items.");
        }
    }

    private void CreateMenuItems()
    {
        if (_item.CanBeUsed == true && _canUse == true)
            _menuItems.Add(Localization.GetString("item_detail_screen_use"));
        if (_item.CanBeHeld == true)
            _menuItems.Add(Localization.GetString("item_detail_screen_give"));
        if (_item.ItemType != ItemTypes.KeyItems && _item.CanBeTossed == true)
            _menuItems.Add(Localization.GetString("item_detail_screen_trash"));
        _menuItems.Add(Localization.GetString("item_detail_screen_back"));
    }
}
