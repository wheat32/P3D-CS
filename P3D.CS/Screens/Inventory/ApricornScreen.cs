using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Items;

namespace P3D;

public class ApricornScreen : Screen
{
    private enum States { CanGive, CanTake, Wait, None }

    private String _owner = String.Empty;
    private States _state = States.Wait;

    private List<String> _apricorns = [];

    private List<ButtonIcon> _buttons = [];
    private List<Label> _labels = [];

    private States _lastUpdateState = States.None;

    private Texture2D _buttonTexture = null!;
    private Texture2D _mainTexture = null!;
    private bool _closing = false;
    private float _enrollY = 0.0F;
    public static float _interfaceFade = 0.0F;

    private int _cursorIndex = 0;

    public ApricornScreen(Screen currentScreen, String owner)
    {
        Identification = Identifications.ApricornScreen;
        _mainTexture = TextureManager.GetTexture("GUI\\Menus\\General");
        PreScreen = currentScreen;
        _buttonTexture = TextureManager.GetTexture("GUI\\Menus\\Menu");
        _owner = owner;
        MouseVisible = true;

        GetSavedApricorns();

        if (CheckApricornStatus() >= 1440)
        {
            if (HasApricorns() == true)
                _state = States.CanTake;
            else
                _state = States.CanGive;
        }
        else
        {
            _state = States.Wait;
        }

        InitializeScreen();
    }

    private void InitializeScreen()
    {
        if (_lastUpdateState != _state)
        {
            _lastUpdateState = _state;
            _buttons.Clear();
            _labels.Clear();

            int halfWidth = (int)(Core.windowSize.Width / 2);
            int halfHeight = (int)(Core.windowSize.Height / 2);
            int deltaX = halfWidth - 400;
            int deltaY = halfHeight - 200;

            switch (_state)
            {
                case States.Wait:
                    _labels.Add(new Label(Localization.GetString("apricorn_screen_producing").Replace("~", Environment.NewLine), new Vector2(deltaX + 56, deltaY + 48), FontManager.MainFont));
                    _labels.Add(new Label(Localization.GetString("apricorn_screen_backadvice").Replace("~", Environment.NewLine), new Vector2(deltaX + 56, deltaY + 16 + 96 * 2), FontManager.MainFont));
                    break;
                case States.CanGive:
                {
                    Texture2D t = TextureManager.GetTexture("Items\\ItemSheet");
                    _labels.Add(new Label(Localization.GetString("apricorn_screen_choose_apricorns").Replace("~", Environment.NewLine), new Vector2(deltaX + 48, deltaY + 48), FontManager.MainFont));

                    ButtonIcon redApricorn    = new ButtonIcon(GiveApricorn, "0 / " + Core.Player.Inventory.GetItemAmount("85"),  FontManager.MainFont, t, new Rectangle(240, 72, 24, 24), new Vector2(deltaX + 128,     deltaY + 16 + 104),     new Size(48, 48), "85");
                    ButtonIcon blueApricorn   = new ButtonIcon(GiveApricorn, "0 / " + Core.Player.Inventory.GetItemAmount("89"),  FontManager.MainFont, t, new Rectangle(336, 72, 24, 24), new Vector2(deltaX + 128 * 2, deltaY + 16 + 104),     new Size(48, 48), "89");
                    ButtonIcon yellowApricorn = new ButtonIcon(GiveApricorn, "0 / " + Core.Player.Inventory.GetItemAmount("92"),  FontManager.MainFont, t, new Rectangle(384, 72, 24, 24), new Vector2(deltaX + 128 * 3, deltaY + 16 + 104),     new Size(48, 48), "92");
                    ButtonIcon greenApricorn  = new ButtonIcon(GiveApricorn, "0 / " + Core.Player.Inventory.GetItemAmount("93"),  FontManager.MainFont, t, new Rectangle(408, 72, 24, 24), new Vector2(deltaX + 128 * 4, deltaY + 16 + 104),     new Size(48, 48), "93");
                    ButtonIcon whiteApricorn  = new ButtonIcon(GiveApricorn, "0 / " + Core.Player.Inventory.GetItemAmount("97"),  FontManager.MainFont, t, new Rectangle(0,   96, 24, 24), new Vector2(deltaX + 128 * 5, deltaY + 16 + 104),     new Size(48, 48), "97");
                    ButtonIcon blackApricorn  = new ButtonIcon(GiveApricorn, "0 / " + Core.Player.Inventory.GetItemAmount("99"),  FontManager.MainFont, t, new Rectangle(48,  96, 24, 24), new Vector2(deltaX + 128,     deltaY + 16 + 104 * 2), new Size(48, 48), "99");
                    ButtonIcon pinkApricorn   = new ButtonIcon(GiveApricorn, "0 / " + Core.Player.Inventory.GetItemAmount("101"), FontManager.MainFont, t, new Rectangle(72,  96, 24, 24), new Vector2(deltaX + 128 * 2, deltaY + 16 + 104 * 2), new Size(48, 48), "101");

                    ButtonIcon addAllButton = new ButtonIcon(b => AddAllApricorns(), Localization.GetString("apricorn_screen_all"),   FontManager.MainFont, _buttonTexture, new Rectangle(64, 160, 16, 16), new Vector2(deltaX + 128 * 3, deltaY + 16 + 104 * 2), new Size(48, 48), "Clear");
                    ButtonIcon clearButton  = new ButtonIcon(b => ClearApricorns(),  Localization.GetString("apricorn_screen_clear"), FontManager.MainFont, _buttonTexture, new Rectangle(64, 128, 16, 16), new Vector2(deltaX + 128 * 4, deltaY + 16 + 104 * 2), new Size(48, 48), "Clear");
                    ButtonIcon giveButton   = new ButtonIcon(b => Give(),            Localization.GetString("apricorn_screen_ok"),    FontManager.MainFont, _buttonTexture, new Rectangle(48, 128, 16, 16), new Vector2(deltaX + 128 * 5, deltaY + 16 + 104 * 2), new Size(48, 48), "OK");
                    giveButton.Enabled = false;

                    _buttons.AddRange([redApricorn, blueApricorn, yellowApricorn, greenApricorn, whiteApricorn, blackApricorn, pinkApricorn, addAllButton, clearButton, giveButton]);
                    break;
                }
                case States.CanTake:
                    _labels.Add(new Label(Localization.GetString("apricorn_screen_ready").Replace("~", Environment.NewLine), new Vector2(deltaX + 48, deltaY + 48), FontManager.MainFont));
                    ButtonIcon takeButton = new ButtonIcon(b => Take(), Localization.GetString("apricorn_screen_take"), FontManager.MainFont, _buttonTexture, new Rectangle(48, 128, 16, 16), new Vector2(deltaX + 128 * 5, deltaY + 16 + 104 * 2), new Size(48, 48));
                    _buttons.Add(takeButton);
                    break;
            }
        }
    }

    public override void Update()
    {
        TextBox.Update();
        InitializeScreen();

        if (_closing == true)
        {
            if (_interfaceFade > 0.0F)
            {
                _interfaceFade = MathHelper.Lerp(0, _interfaceFade, 0.8F);
                if (_interfaceFade < 0.0F) _interfaceFade = 0.0F;
            }
            if (_enrollY > 0)
            {
                _enrollY = MathHelper.Lerp(0, _enrollY, 0.8F);
                if (_enrollY <= 0) _enrollY = 0;
            }
            if (_enrollY <= 2.0F)
                Core.SetScreen(PreScreen!);
        }
        else
        {
            int maxWindowHeight = 400;
            if (_enrollY < maxWindowHeight)
            {
                _enrollY = MathHelper.Lerp(maxWindowHeight, _enrollY, 0.8F);
                if (_enrollY >= maxWindowHeight) _enrollY = maxWindowHeight;
            }
            if (_interfaceFade < 1.0F)
            {
                _interfaceFade = MathHelper.Lerp(1.0F, _interfaceFade, 0.95F);
                if (_interfaceFade > 1.0F) _interfaceFade = 1.0F;
            }

            if (TextBox.Showing == false)
            {
                switch (_state)
                {
                    case States.Wait:   UpdateWait(); break;
                    case States.CanGive: UpdateGive(); break;
                    case States.CanTake: UpdateTake(); break;
                }

                if (Controls.Dismiss() == true)
                    _closing = true;

                if (_state != States.CanGive)
                {
                    if (Controls.Left(true, true) == true || Controls.Up(true, true) == true)
                    {
                        _cursorIndex -= 1;
                        if (_cursorIndex < 0) _cursorIndex = _buttons.Count - 1;
                    }
                    if (Controls.Down(true, true) == true || Controls.Right(true, true) == true)
                    {
                        _cursorIndex += 1;
                        if (_cursorIndex > _buttons.Count - 1) _cursorIndex = 0;
                    }
                }

                if (_buttons.Count > 0)
                {
                    if (Controls.Accept(false, true, true) == true && _buttons[_cursorIndex].Enabled == true)
                    {
                        SoundManager.PlaySound("select");
                        _buttons[_cursorIndex].Click();
                    }
                }

                foreach (ButtonIcon b in _buttons)
                    b.Update();
            }
        }
    }

    private void UpdateWait() { }

    private void UpdateGive()
    {
        if (Controls.Up(true, true, false) == true || Controls.Down(true, true, false) == true)
        {
            if (_cursorIndex < 5)
                _cursorIndex += 5;
            else
                _cursorIndex -= 5;
        }
        if (Controls.Left(true, true, false) == true)
        {
            if (_cursorIndex <= 0 || _cursorIndex == 5)
                _cursorIndex += 4;
            else
                _cursorIndex -= 1;
        }
        if (Controls.Right(true, true, false) == true)
        {
            if (_cursorIndex >= _buttons.Count - 1 || _cursorIndex == 4)
                _cursorIndex -= 4;
            else
                _cursorIndex += 1;
        }
        if (Controls.Left(true, false, true, false, false, false) == true)
        {
            _cursorIndex -= 1;
            if (_cursorIndex < 0) _cursorIndex = _buttons.Count - 1;
        }
        if (Controls.Right(true, false, true, false, false, false) == true)
        {
            _cursorIndex += 1;
            if (_cursorIndex > _buttons.Count - 1) _cursorIndex = 0;
        }
    }

    private void UpdateTake() { }

    public override void Draw()
    {
        PreScreen!.Draw();
        DrawBackground();

        for (int i = 0; i <= _buttons.Count - 1; i++)
        {
            ButtonIcon b = _buttons[i];
            if (i == _cursorIndex)
            {
                Canvas.DrawRectangle(new Rectangle((int)(b.Position.X - 4), (int)(b.Position.Y - 4), 56, 56), new Color(Color.White, (int)(255 * 0.5 * _interfaceFade)));
                Canvas.DrawBorder(2, new Rectangle((int)(b.Position.X - 4), (int)(b.Position.Y - 4), 56, 56), new Color(Color.White, (int)(255 * _interfaceFade)));
            }
        }

        foreach (ButtonIcon button in _buttons)
            button.Draw();

        foreach (Label label in _labels)
            label.Draw();

        switch (_state)
        {
            case States.Wait:    DrawWait(); break;
            case States.CanGive: DrawGive(); break;
            case States.CanTake: DrawTake(); break;
        }

        TextBox.Draw();
    }

    private void DrawBackground()
    {
        Color mainBackgroundColor = Color.White;
        if (_closing == true)
            mainBackgroundColor = new Color(255, 255, 255, (int)(255 * _interfaceFade));

        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);

        Canvas.DrawRectangle(new Rectangle(halfWidth - 400, halfHeight - 232, 260, 32), new Color(Screens.UI.ColorProvider.MainColor(false).R, Screens.UI.ColorProvider.MainColor(false).G, Screens.UI.ColorProvider.MainColor(false).B, mainBackgroundColor.A));
        Canvas.DrawRectangle(new Rectangle(halfWidth - 140, halfHeight - 216, 16, 16), new Color(Screens.UI.ColorProvider.MainColor(false).R, Screens.UI.ColorProvider.MainColor(false).G, Screens.UI.ColorProvider.MainColor(false).B, mainBackgroundColor.A));
        Core.SpriteBatch.Draw(_mainTexture, new Rectangle(halfWidth - 140, halfHeight - 232, 16, 16), new Rectangle(80, 0, 16, 16), mainBackgroundColor);
        Core.SpriteBatch.Draw(_mainTexture, new Rectangle(halfWidth - 124, halfHeight - 216, 16, 16), new Rectangle(80, 0, 16, 16), mainBackgroundColor);

        Core.SpriteBatch.DrawString(FontManager.ChatFont, Localization.GetString("apricorn_screen_apricorns", "Apricorns"), new Vector2(halfWidth - 390, halfHeight - 228), mainBackgroundColor);

        for (int y = 0; y <= (int)_enrollY; y += 16)
        {
            for (int x = 0; x <= 800; x += 16)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle(halfWidth - 400 + x, halfHeight - 200 + y, 16, 16), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
        }

        int modRes = (int)_enrollY % 16;
        if (modRes > 0)
        {
            for (int x = 0; x <= 800; x += 16)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle(halfWidth - 400 + x, (int)(_enrollY + (halfHeight - 200)), 16, modRes), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
        }
    }

    private void DrawWait() { }

    private void DrawTake()
    {
        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);
        int deltaX = halfWidth - 400;
        int deltaY = halfHeight - 200;

        int[] ballIDs = [159, 160, 165, 164, 161, 157, 166];

        for (int i = 0; i <= 6; i++)
        {
            int x = i, y = 0;
            while (x > 4) { x -= 5; y += 1; }

            Item item = Item.GetItemByID(ballIDs[i].ToString())!;
            Vector2 textSize = FontManager.MainFont.MeasureString("x" + _apricorns[i]);

            Core.SpriteBatch.Draw(item.Texture, new Rectangle(deltaX + 128 + x * 128, deltaY + 16 + 104 + y * 104, 48, 48), new Color(Color.White, _interfaceFade));
            Canvas.DrawRectangle(new Rectangle((int)(deltaX + 128 + x * 128 + 48.0 / 2 - textSize.X / 2 - 8), (int)(deltaY + 16 + 104 + y * 104 + 48 + 8), (int)(textSize.X + 16), (int)(textSize.Y + 16)), new Color(Color.Black, (int)(255 * 0.4F * _interfaceFade)));
            Core.SpriteBatch.DrawString(FontManager.MainFont, "x" + _apricorns[i], new Vector2((int)(48.0 / 2) - (int)(textSize.X / 2) + deltaX + 128 + x * 128, 48 + deltaY + 16 + 104 + y * 104 + (int)(textSize.Y / 2)), new Color(Color.White, _interfaceFade));
        }
    }

    private void DrawGive() { }

    private void GiveApricorn(ButtonIcon b)
    {
        int apricornID = b.AdditionalValue switch
        {
            "85" => 0, "89" => 1, "92" => 2, "93" => 3, "97" => 4, "99" => 5, "101" => 6, _ => 0
        };

        if (Core.Player.Inventory.GetItemAmount(b.AdditionalValue) > int.Parse(_apricorns[apricornID]))
        {
            _apricorns[apricornID] = (int.Parse(_apricorns[apricornID]) + 1).ToString();

            if (HasApricorns() == true)
            {
                foreach (ButtonIcon button in _buttons)
                {
                    if (button.AdditionalValue == Localization.GetString("apricorn_screen_ok"))
                        button.Enabled = true;
                }
            }
        }

        AdjustButtonTexts();
    }

    private void AdjustButtonTexts()
    {
        foreach (ButtonIcon b in _buttons)
        {
            if (StringHelper.IsNumeric(b.AdditionalValue) == true)
            {
                int apricornID = b.AdditionalValue switch
                {
                    "85" => 0, "89" => 1, "92" => 2, "93" => 3, "97" => 4, "99" => 5, "101" => 6, _ => 0
                };
                b.Text = _apricorns[apricornID] + " / " + Core.Player.Inventory.GetItemAmount(b.AdditionalValue);
            }
        }
    }

    private void ClearApricorns()
    {
        foreach (ButtonIcon button in _buttons)
        {
            if (button.AdditionalValue == Localization.GetString("apricorn_screen_ok"))
                button.Enabled = false;
        }
        _apricorns = ["0", "0", "0", "0", "0", "0", "0"];
        AdjustButtonTexts();
    }

    private void AddAllApricorns()
    {
        _apricorns =
        [
            Core.Player.Inventory.GetItemAmount("85").ToString(),
            Core.Player.Inventory.GetItemAmount("89").ToString(),
            Core.Player.Inventory.GetItemAmount("92").ToString(),
            Core.Player.Inventory.GetItemAmount("93").ToString(),
            Core.Player.Inventory.GetItemAmount("97").ToString(),
            Core.Player.Inventory.GetItemAmount("99").ToString(),
            Core.Player.Inventory.GetItemAmount("101").ToString(),
        ];

        if (HasApricorns() == true)
        {
            foreach (ButtonIcon button in _buttons)
            {
                if (button.AdditionalValue == Localization.GetString("apricorn_screen_ok"))
                    button.Enabled = true;
            }
        }

        AdjustButtonTexts();
    }

    private void Give()
    {
        _state = States.Wait;

        Core.Player.Inventory.RemoveItem("85", int.Parse(_apricorns[0]));
        Core.Player.Inventory.RemoveItem("89", int.Parse(_apricorns[1]));
        Core.Player.Inventory.RemoveItem("92", int.Parse(_apricorns[2]));
        Core.Player.Inventory.RemoveItem("93", int.Parse(_apricorns[3]));
        Core.Player.Inventory.RemoveItem("97", int.Parse(_apricorns[4]));
        Core.Player.Inventory.RemoveItem("99", int.Parse(_apricorns[5]));
        Core.Player.Inventory.RemoveItem("101", int.Parse(_apricorns[6]));

        DateTime d = DateTime.Now;
        String s = "{" + _owner + "|" +
            _apricorns[0] + "," +
            _apricorns[1] + "," +
            _apricorns[2] + "," +
            _apricorns[3] + "," +
            _apricorns[4] + "," +
            _apricorns[5] + "," +
            _apricorns[6] + "|" +
            d.Year + "," + d.Month + "," + d.Day + "," + d.Hour + "," + d.Minute + "," + d.Second + "}";

        if (Core.Player.ApricornData != String.Empty)
            Core.Player.ApricornData += Environment.NewLine;

        Core.Player.ApricornData += s;
    }

    private void Take()
    {
        _state = States.CanGive;

        String text = Core.Player.Name + " " + Localization.GetString("apricorn_screen_obtain");
        int[] ballIDs    = [159, 160, 165, 164, 161, 157, 166];
        String[] apricornItemIDs = ["159", "160", "165", "164", "161", "157", "166"];

        for (int i = 0; i <= 6; i++)
        {
            if (int.Parse(_apricorns[i]) > 0)
            {
                Core.Player.Inventory.AddItem(apricornItemIDs[i], int.Parse(_apricorns[i]));
                String comma = (i == 0) ? String.Empty : ",";
                text += comma + "~" + _apricorns[i] + "  " + Item.GetItemByID(ballIDs[i].ToString())!.Name;
            }
        }

        text += ".";
        ClearApricorns();

        String s = String.Empty;
        String[] data = Core.Player.ApricornData.SplitAtNewline();

        for (int i = 0; i <= data.Length - 1; i++)
        {
            if (data[i].StartsWith("{" + _owner + "|") == false)
            {
                if (s != String.Empty) s += Environment.NewLine;
                s += data[i];
            }
        }

        Core.Player.ApricornData = s;
        TextBox.Show(text, [], true, false);
    }

    private bool HasApricorns()
    {
        for (int i = 0; i <= 6; i++)
        {
            if (int.Parse(_apricorns[i]) > 0) return true;
        }
        return false;
    }

    private void GetSavedApricorns()
    {
        ClearApricorns();

        if (Core.Player.ApricornData != String.Empty)
        {
            List<String> apricornsData = Core.Player.ApricornData.SplitAtNewline().ToList();

            for (int i = 0; i <= apricornsData.Count - 1; i++)
            {
                if (i < apricornsData.Count)
                {
                    String apricorn = apricornsData[i];
                    apricorn = apricorn.Remove(0, 1);
                    apricorn = apricorn.Remove(apricorn.Length - 1, 1);

                    String[] apricornData = apricorn.Split('|');
                    if (apricornData[0] == _owner)
                        _apricorns = apricornData[1].Split(',').ToList();
                }
            }
        }
    }

    private int CheckApricornStatus()
    {
        int diff = 1440;

        if (Core.Player.ApricornData != String.Empty)
        {
            List<String> apricornsData = Core.Player.ApricornData.SplitAtNewline().ToList();

            for (int i = 0; i <= apricornsData.Count - 1; i++)
            {
                if (i < apricornsData.Count)
                {
                    String apricorn = apricornsData[i];
                    apricorn = apricorn.Remove(0, 1);
                    apricorn = apricorn.Remove(apricorn.Length - 1, 1);

                    String[] apricornData = apricorn.Split('|');
                    if (apricornData[0] == _owner)
                    {
                        String[] d = apricornData[2].Split(',');
                        DateTime gaveDate = new DateTime(int.Parse(d[0]), int.Parse(d[1]), int.Parse(d[2]), int.Parse(d[3]), int.Parse(d[4]), int.Parse(d[5]));
                        diff = (int)(DateTime.Now - gaveDate).TotalMinutes;
                    }
                }
            }
        }

        return diff;
    }

    private class Label
    {
        public Vector2 Position;
        public String Text;
        public Color ForeColor = Color.White;
        public SpriteFont Font;

        public Label(String text, Vector2 position, SpriteFont font)
            : this(text, position, Color.White, font) { }

        public Label(String text, Vector2 position, Color foreColor, SpriteFont font)
        {
            Position = position;
            Text = text;
            ForeColor = foreColor;
            Font = font;
        }

        public void Draw()
        {
            Core.SpriteBatch.DrawString(Font, Text, Position, new Color(ForeColor, _interfaceFade));
        }
    }

    public class ButtonIcon
    {
        public String Text;
        public SpriteFont Font;
        public Vector2 Position;
        public Texture2D Texture;
        public Rectangle TextureRectangle;
        public Size Size;
        public String AdditionalValue = String.Empty;
        public bool Enabled = true;

        private int _state = 0;
        private Action<ButtonIcon> _doSub;

        public ButtonIcon(Action<ButtonIcon> doOnClick, String text, SpriteFont font, Texture2D texture, Rectangle textureRectangle, Vector2 position, Size size, String additionalValue = "")
        {
            _doSub = doOnClick;
            Text = text;
            Font = font;
            Position = position;
            Size = size;
            Texture = texture;
            TextureRectangle = textureRectangle;
            AdditionalValue = additionalValue;
        }

        public void Update()
        {
            if (Enabled == true)
            {
                Point mousePosition = new Point(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y);
                _state = 0;

                if (new Rectangle((int)Position.X, (int)Position.Y, Size.Width, Size.Height).Contains(mousePosition) == true)
                {
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        _doSub(this);
                    else if (MouseHandler.ButtonDown(MouseHandler.MouseButtons.LeftButton) == true)
                        _state = 2;
                    else
                        _state = 1;
                }
            }
        }

        public void Draw()
        {
            Color bColor = new Color(Color.White.R - (_state * 50), Color.White.G - (_state * 50), Color.White.B - (_state * 50));
            if (Enabled == false) bColor = Color.DarkGray;

            Core.SpriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Size.Width, Size.Height), TextureRectangle, new Color(bColor, _interfaceFade));

            Vector2 textSize = Font.MeasureString(Text);
            Color tColor = new Color(Color.White, _interfaceFade);
            Canvas.DrawRectangle(new Rectangle((int)(Position.X + Size.Width / 2 - textSize.X / 2 - 8), (int)(Position.Y + Size.Height + 8), (int)(textSize.X + 16), (int)(textSize.Y + 16)), new Color(Color.Black, (int)(255 * 0.4F * _interfaceFade)));
            Core.SpriteBatch.DrawString(Font, Text, new Vector2((int)(Size.Width / 2.0) - (int)(textSize.X / 2) + Position.X, Size.Height + Position.Y + (int)(textSize.Y / 2)), tColor);
        }

        public void Click()
        {
            _doSub(this);
        }
    }
}
