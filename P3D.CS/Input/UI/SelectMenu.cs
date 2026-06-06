using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

/// <summary>A menu that displays multiple selectable options to the player.</summary>
public class SelectMenu
{
    private const int MAX_VISIBLE_ITEMS = 8;
    private const int MENU_RIGHT_OFFSET = 270;
    private const int MENU_ROW_HEIGHT = 72;
    private const int MENU_ITEM_SIZE = 64;
    private const int MENU_ITEM_WIDTH = 256;
    private const int TEXT_LEFT_PADDING = 20;
    private const int TEXT_HALF_ROW = 32;
    private const int CURSOR_X_OFFSET = 128;
    private const int CURSOR_Y_OFFSET = 40;
    private const float CURSOR_LERP = 0.6f;

    public delegate void ClickEvent(SelectMenu s);

    private List<String> _items = [];
    private int _index = 0;
    private ClickEvent? _clickHandler = null;
    private int _backIndex = 0;

    public bool Visible = true;
    public int Scroll = 0;

    private Texture2D? _t1;
    private Texture2D? _t2;
    private Vector2 _cursorPos;
    private Vector2 _cursorDest;

    public SelectMenu(List<String> items, int index, ClickEvent clickHandler, int backIndex)
    {
        _items = items;
        _index = index;
        _clickHandler = clickHandler;
        _backIndex = backIndex;
        if (_backIndex < 0)
        {
            _backIndex = _items.Count + _backIndex;
        }
        Visible = true;

        _t1 = TextureManager.GetTexture("GUI\\Menus\\General",
            new Microsoft.Xna.Framework.Rectangle(16, 16, 16, 16), "");
        _t2 = TextureManager.GetTexture("GUI\\Menus\\General",
            new Microsoft.Xna.Framework.Rectangle(32, 16, 16, 16), "");

        SetCursorDest();
        _cursorPos = _cursorDest;
    }

    public String SelectedItem => _items[_index];

    public void Update()
    {
        if (Visible == true)
        {
            _cursorPos.Y = (int)MathHelper.Lerp(_cursorDest.Y, _cursorPos.Y, CURSOR_LERP);

            if (Controls.Up(true, true, true, true, true, true) == true)
            {
                _index -= 1;
                if (_index < 0)
                {
                    _index = _items.Count - 1;
                }
            }
            if (Controls.Down(true, true, true, true, true, true) == true)
            {
                _index += 1;
                if (_index > _items.Count - 1)
                {
                    _index = 0;
                }
            }

            bool playSelectSound = false;

            if (Core.CurrentScreen.MouseVisible == true)
            {
                for (int i = Scroll; i <= Scroll + MAX_VISIBLE_ITEMS; i++)
                {
                    if (i <= _items.Count - 1)
                    {
                        Microsoft.Xna.Framework.Rectangle itemRect = new Microsoft.Xna.Framework.Rectangle(
                            Core.windowSize.Width - MENU_RIGHT_OFFSET,
                            MENU_ROW_HEIGHT * ((i + 1) - Scroll),
                            MENU_ITEM_WIDTH, MENU_ITEM_SIZE);

                        bool mouseAccept = Controls.Accept(true, false, false) == true &&
                                          i == _index && itemRect.Contains(MouseHandler.MousePosition);
                        bool keyboardAccept = Controls.Accept(false, true, true) == true && i == _index;
                        bool dismissBackMatch = Controls.Dismiss(true, true, true) == true && _backIndex == _index;

                        if (mouseAccept || keyboardAccept || dismissBackMatch)
                        {
                            playSelectSound = true;
                            _clickHandler?.Invoke(this);
                            Visible = false;
                        }
                        if (Controls.Dismiss(true, true, true) == true)
                        {
                            _index = _backIndex;
                            playSelectSound = true;
                            _clickHandler?.Invoke(this);
                            Visible = false;
                        }
                        if (itemRect.Contains(MouseHandler.MousePosition) && Controls.Accept(true, false, false) == true)
                        {
                            _index = i;
                        }
                    }
                }
            }
            else
            {
                for (int i = Scroll; i <= Scroll + MAX_VISIBLE_ITEMS; i++)
                {
                    if (Controls.Accept(true, true, true) == true && i == _index)
                    {
                        playSelectSound = true;
                        _clickHandler?.Invoke(this);
                        Visible = false;
                        break;
                    }
                    if (Controls.Dismiss(true, true, true) == true)
                    {
                        _index = _backIndex;
                        playSelectSound = true;
                        _clickHandler?.Invoke(this);
                        Visible = false;
                        break;
                    }
                }
            }

            if (playSelectSound == true)
            {
                SoundManager.PlaySound("select");
            }
        }

        if (_index - Scroll > MAX_VISIBLE_ITEMS)
        {
            Scroll = _index - MAX_VISIBLE_ITEMS;
        }
        if (_index - Scroll < 0)
        {
            Scroll = _index;
        }

        SetCursorDest();
    }

    public void Draw()
    {
        if (Visible == true)
        {
            for (int i = Scroll; i <= Scroll + MAX_VISIBLE_ITEMS; i++)
            {
                if (i <= _items.Count - 1)
                {
                    String text = _items[i];
                    Vector2 startPos = new Vector2(Core.windowSize.Width - MENU_RIGHT_OFFSET,
                        MENU_ROW_HEIGHT * ((i + 1) - Scroll));

                    Core.SpriteBatch.Draw(_t1, new Microsoft.Xna.Framework.Rectangle(
                        (int)startPos.X, (int)startPos.Y, MENU_ITEM_SIZE, MENU_ITEM_SIZE), Color.White);
                    Core.SpriteBatch.Draw(_t2, new Microsoft.Xna.Framework.Rectangle(
                        (int)startPos.X + MENU_ITEM_SIZE, (int)startPos.Y, MENU_ITEM_SIZE, MENU_ITEM_SIZE), Color.White);
                    Core.SpriteBatch.Draw(_t2, new Microsoft.Xna.Framework.Rectangle(
                        (int)startPos.X + MENU_ITEM_SIZE * 2, (int)startPos.Y, MENU_ITEM_SIZE, MENU_ITEM_SIZE), Color.White);
                    Core.SpriteBatch.Draw(_t1,
                        new Microsoft.Xna.Framework.Rectangle(
                            (int)startPos.X + MENU_ITEM_SIZE * 3, (int)startPos.Y, MENU_ITEM_SIZE, MENU_ITEM_SIZE),
                        null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);

                    if (FontManager.MainFont != null)
                    {
                        Core.SpriteBatch.DrawString(FontManager.MainFont, text,
                            new Vector2(
                                (int)(startPos.X + TEXT_LEFT_PADDING),
                                (int)(startPos.Y + TEXT_HALF_ROW - FontManager.MainFont.MeasureString(text).Y / 2)),
                            Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                    }
                }
            }
        }

        Vector2 cPosition = new Vector2((int)(_cursorPos.X + CURSOR_X_OFFSET),
                                        (int)(_cursorPos.Y - CURSOR_Y_OFFSET));
        Texture2D cursor = TextureManager.GetTexture("GUI\\Menus\\General",
            new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), "");
        Core.SpriteBatch.Draw(cursor,
            new Microsoft.Xna.Framework.Rectangle((int)cPosition.X, (int)cPosition.Y,
                MENU_ITEM_SIZE, MENU_ITEM_SIZE), Color.White);
    }

    private void SetCursorDest()
    {
        _cursorDest = new Vector2(Core.windowSize.Width - MENU_RIGHT_OFFSET,
            MENU_ROW_HEIGHT * (_index + 1 - Scroll));
    }
}
