using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class InputScreen : Screen
{
    public static String LastInput = String.Empty;

    public enum InputModes
    {
        Text = 0,
        Numbers = 1,
        Name = 2,
        Pokemon = 3
    }

    private String _defaultName = String.Empty;
    private String _currentText = String.Empty;
    private List<Texture2D> _sprites = [];
    private List<InputButton> _buttons = [];
    private InputModes _inputMode = InputModes.Text;
    private int _maxChars = 100;
    private Vector2 _buttonSelector = new Vector2(0, 0);

    public bool PasswordMode = false;

    public delegate void ConfirmInput(String input);

    private ConfirmInput? _confirmSub;

    public InputScreen(Screen currentScreen, String defaultName, InputModes inputMode,
                       String currentText, int maxChars, List<Texture2D> sprites,
                       ConfirmInput? confirmSub = null)
    {
        PreScreen = currentScreen;
        _defaultName = defaultName;
        _currentText = currentText;
        _sprites = sprites;
        _inputMode = inputMode;
        _maxChars = maxChars;
        _confirmSub = confirmSub;

        InitializeScreen();
    }

    private void InitializeScreen()
    {
        Identification = Identifications.InputScreen;
        CanBePaused = true;
        CanChat = false;
        CanMuteAudio = false;
        MouseVisible = true;

        if (_maxChars < 0)
            _maxChars = 100;

        InputButton.CapsLock = true;

        switch (_inputMode)
        {
            case InputModes.Text:
                InitializeText();
                break;
            case InputModes.Name:
            case InputModes.Pokemon:
                InitializeName();
                break;
            case InputModes.Numbers:
                InitializeNumbers();
                break;
        }

        _buttons.Add(new InputButton(Localization.GetString("input_screen_button_Shift", "Shift"), new Vector2(0, 4), InputButton.ButtonModes.CapsLock, 2));
        _buttons.Add(new InputButton(Localization.GetString("global_delete", "Delete"), new Vector2(2, 4), InputButton.ButtonModes.Delete, 2));
        _buttons.Add(new InputButton(Localization.GetString("input_screen_button_Default", "Default"), new Vector2(4, 4), InputButton.ButtonModes.Default, 2));
        _buttons.Add(new InputButton(Localization.GetString("global_confirm", "Confirm"), new Vector2(6, 4), InputButton.ButtonModes.Enter, 2));
    }

    private void InitializeText()
    {
        String[] chars = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", " ", ".", ",", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", " ", "'", "-", "u", "v", "w", "x", "y", "z", " ", " ", " ", " ", "!", "?", "_"];
        int x = 0;
        int y = 0;

        foreach (String c in chars)
        {
            _buttons.Add(new InputButton(c, new Vector2(x, y), InputButton.ButtonModes.Key, 1));
            x += 1;
            if (x > 12) { x = 0; y += 1; }
        }

        String[] numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", " ", " ", " "];
        x = 0;
        y = 3;
        foreach (String c in numbers)
        {
            _buttons.Add(new InputButton(c, new Vector2(x, y), InputButton.ButtonModes.Key, 1));
            x += 1;
        }
    }

    private void InitializeName()
    {
        String[] chars;
        if (_inputMode == InputModes.Name)
            chars = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", " ", ".", ",", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", " ", "'", "-", "u", "v", "w", "x", "y", "z", " ", " ", " ", " ", "!", "?", "_"];
        else
            chars = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", " ", " ", " ", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", " ", " ", " ", "u", "v", "w", "x", "y", "z", " ", " ", " ", " ", " ", " ", " "];

        int x = 0;
        int y = 0;
        foreach (String c in chars)
        {
            _buttons.Add(new InputButton(c, new Vector2(x, y), InputButton.ButtonModes.Key, 1));
            x += 1;
            if (x > 12) { x = 0; y += 1; }
        }

        String[] numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", " ", " ", " "];
        x = 0;
        y = 3;
        foreach (String c in numbers)
        {
            _buttons.Add(new InputButton(c, new Vector2(x, y), InputButton.ButtonModes.Key, 1));
            x += 1;
        }
    }

    private void InitializeNumbers()
    {
        int x = 0;
        int y = 0;
        String[] numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", " ", " ", " "];

        foreach (String c in numbers)
        {
            _buttons.Add(new InputButton(c, new Vector2(x, y), InputButton.ButtonModes.Key, 1));
            x += 1;
        }
    }

    public override void Draw()
    {
        PreScreen!.Draw();
        Canvas.DrawRectangle(Core.windowSize, new Color(0, 0, 0, 150));

        if (_sprites.Count > 0)
        {
            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2) - 384, 100, 704, 64), Color.White);
            Canvas.DrawBorder(1, new Rectangle((int)(Core.windowSize.Width / 2) - 384, 100, 704, 64), Color.Gray);
            Core.SpriteBatch.Draw(_sprites[0], new Rectangle((int)(Core.windowSize.Width / 2) - 384, 96, 64, 64), Color.White);
        }
        else
        {
            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2) - 320, 100, 640, 64), Color.White);
            Canvas.DrawBorder(1, new Rectangle((int)(Core.windowSize.Width / 2) - 320, 100, 640, 64), Color.Gray);
        }
        Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2) - 316, 104, 632, 56), Color.LightGray);

        String t = _currentText;
        if (PasswordMode == true)
        {
            t = String.Empty;
            for (int cc = 0; cc <= _currentText.Length - 1; cc++)
                t += "*";
        }

        if (_currentText.Length < _maxChars)
            t += "_";

        Core.SpriteBatch.DrawString(FontManager.InGameFont, t, new Vector2((int)(Core.windowSize.Width / 2) - 306, 112), Color.Black);

        Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - (13 * 64) / 2) - 4, 196, (13 * 64) + 8, 5 * 64 + 8), Color.White);
        Canvas.DrawBorder(1, new Rectangle((int)(Core.windowSize.Width / 2 - (13 * 64) / 2) - 4, 196, (13 * 64) + 8, 5 * 64 + 8), Color.Gray);
        foreach (InputButton b in _buttons)
            b.Draw(new Vector2((float)(Core.windowSize.Width / 2 - (13 * 64) / 2), 200), _buttonSelector);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("input_screen_button_CharactersLeft", "Chars left:") + " " + (_maxChars - _currentText.Length).ToString(), new Vector2((int)(Core.windowSize.Width / 2) + 180, 477), Color.Gray);

        Dictionary<Microsoft.Xna.Framework.Input.Buttons, String> d = [];
        d.Add(Microsoft.Xna.Framework.Input.Buttons.A, Localization.GetString("game_interaction_enter", "Enter"));
        d.Add(Microsoft.Xna.Framework.Input.Buttons.B, Localization.GetString("game_interaction_delete", "Delete"));
        d.Add(Microsoft.Xna.Framework.Input.Buttons.X, Localization.GetString("game_interaction_confirm", "Confirm"));
        d.Add(Microsoft.Xna.Framework.Input.Buttons.Back, Localization.GetString("game_interaction_clear", "Clear"));
        DrawGamePadControls(d);
    }

    public override void Update()
    {
        if (Controls.Right(true, true, false, false, true) == true)
        {
            int currentX = (int)_buttonSelector.X;
            int newX = 1000;
            foreach (InputButton b in _buttons)
            {
                if ((int)b.RelPosition.X < newX && (int)b.RelPosition.X > currentX && (int)b.RelPosition.Y == (int)_buttonSelector.Y)
                    newX = (int)b.RelPosition.X;
            }
            if (newX != 1000) _buttonSelector.X = newX;
        }
        if (Controls.Left(true, true, false, false, true) == true)
        {
            int currentX = (int)_buttonSelector.X;
            int newX = -1;
            foreach (InputButton b in _buttons)
            {
                if ((int)b.RelPosition.X > newX && (int)b.RelPosition.X < currentX && (int)b.RelPosition.Y == (int)_buttonSelector.Y)
                    newX = (int)b.RelPosition.X;
            }
            if (newX != -1) _buttonSelector.X = newX;
        }
        if (Controls.Down(true, true, false, false, true) == true)
        {
            int currentY = (int)_buttonSelector.Y;
            int newY = 1000;
            foreach (InputButton b in _buttons)
            {
                if ((int)b.RelPosition.Y < newY && (int)b.RelPosition.Y > currentY && (int)b.RelPosition.X == (int)_buttonSelector.X)
                    newY = (int)b.RelPosition.Y;
            }
            if (newY != 1000) _buttonSelector.Y = newY;
        }
        if (Controls.Up(true, true, false, false, true) == true)
        {
            int currentY = (int)_buttonSelector.Y;
            int newY = -1;
            foreach (InputButton b in _buttons)
            {
                if ((int)b.RelPosition.Y > newY && (int)b.RelPosition.Y < currentY && (int)b.RelPosition.X == (int)_buttonSelector.X)
                    newY = (int)b.RelPosition.Y;
            }
            if (newY != -1) _buttonSelector.Y = newY;
        }

        foreach (InputButton b in _buttons)
            b.Update(new Vector2((float)(Core.windowSize.Width / 2 - (13 * 64) / 2), 200), ref _buttonSelector, this);

        if (Controls.Dismiss(false, false, true) == true)
        {
            if (_currentText.Length > 0)
                _currentText = _currentText.Remove(_currentText.Length - 1, 1);
        }

        if (ControllerHandler.ButtonPressed(Microsoft.Xna.Framework.Input.Buttons.Back) == true)
            _currentText = String.Empty;

        if (ControllerHandler.ButtonPressed(Microsoft.Xna.Framework.Input.Buttons.X) == true)
        {
            if (_buttonSelector == new Vector2(6, 4))
                Confirm();
            else
                _buttonSelector = new Vector2(6, 4);
        }

        if (ControllerHandler.ButtonPressed(Microsoft.Xna.Framework.Input.Buttons.LeftStick) == true || ControllerHandler.ButtonPressed(Microsoft.Xna.Framework.Input.Buttons.RightStick) == true)
            InputButton.CapsLock = !InputButton.CapsLock;

        String bText = _currentText;
        KeyBindings.GetInput(ref _currentText, _maxChars, true, true);

        if (bText != _currentText)
        {
            String[] chars = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", " ", ".", ",", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "'", "-", "u", "v", "w", "x", "y", "z", "!", "?", "_", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"];
            String newText = String.Empty;
            foreach (char c in _currentText)
            {
                if (chars.Contains(c.ToString().ToLower()) == true)
                    newText += c.ToString();
            }
            _currentText = newText;
        }
    }

    public void Confirm()
    {
        String t = _currentText;
        while (t.StartsWith(" ") == true)
            t = t.Remove(0, 1);
        while (t.EndsWith(" ") == true)
            t = t.Remove(t.Length - 1, 1);
        if (t == String.Empty)
            t = _defaultName;
        LastInput = t;

        if (_confirmSub != null)
            _confirmSub(LastInput);

        Core.SetScreen(PreScreen!);
    }

    public class InputButton
    {
        public static bool CapsLock = true;
        private static Texture2D? _texture;
        private static bool _loadedTexture = false;

        private const int RASTER_SIZE = 64;

        public enum ButtonModes
        {
            Enter,
            Delete,
            Default,
            Key,
            CapsLock
        }

        public String DisplayText = "A";
        public String ReturnText = "A";
        public int Size = 64;
        public Vector2 RelPosition = new Vector2(0, 0);
        public ButtonModes ButtonMode = ButtonModes.Key;

        public InputButton(String displayText, Vector2 relPosition, ButtonModes buttonMode, int size)
        {
            if (_loadedTexture == false)
            {
                _loadedTexture = true;
                _texture = TextureManager.GetTexture("GUI\\Menus\\GTS", new Rectangle(368, 112, 1, 32), String.Empty);
            }

            DisplayText = displayText;
            ReturnText = displayText;
            RelPosition = relPosition;
            ButtonMode = buttonMode;
            Size = size * RASTER_SIZE;
        }

        public void Draw(Vector2 startPosition, Vector2 selector)
        {
            Vector2 p = new Vector2(RelPosition.X * RASTER_SIZE + startPosition.X, RelPosition.Y * RASTER_SIZE + startPosition.Y);
            for (int i = 0; i <= Size - 1; i++)
            {
                Color buttonColor = ButtonMode switch
                {
                    ButtonModes.Key => Color.White,
                    ButtonModes.Enter => Color.CadetBlue,
                    ButtonModes.Delete => Color.Tomato,
                    _ => new Color(100, 100, 100)
                };

                if (selector == RelPosition)
                    Core.SpriteBatch.Draw(_texture!, new Rectangle((int)p.X + i, (int)p.Y, 1, RASTER_SIZE), null, buttonColor, 0.0F, Vector2.Zero, SpriteEffects.FlipVertically, 0.0F);
                else
                    Core.SpriteBatch.Draw(_texture!, new Rectangle((int)p.X + i, (int)p.Y, 1, RASTER_SIZE), buttonColor);
            }

            if (selector == RelPosition)
                Canvas.DrawBorder(2, new Rectangle((int)p.X, (int)p.Y, Size, RASTER_SIZE), new Color(255, 0, 0, 100));
            else
                Canvas.DrawBorder(1, new Rectangle((int)p.X, (int)p.Y, Size, RASTER_SIZE), Color.Gray);

            String text = DisplayText;
            if (ButtonMode == ButtonModes.Key)
                text = CapsLock == true ? text.ToUpper() : text.ToLower();

            Color fontColor = ButtonMode != ButtonModes.Key ? Color.White : Color.Black;

            Vector2 midPoint = new Vector2((float)Size / 2 + p.X, (float)RASTER_SIZE / 2 + p.Y);
            SpriteFont f = FontManager.MainFont;
            Core.SpriteBatch.DrawString(f, text, new Vector2(midPoint.X - (float)f.MeasureString(text).X / 2, midPoint.Y - (float)f.MeasureString(text).Y / 2), fontColor);
        }

        public void Update(Vector2 startPosition, ref Vector2 selector, InputScreen s)
        {
            Vector2 p = new Vector2(RelPosition.X * RASTER_SIZE + startPosition.X, RelPosition.Y * RASTER_SIZE + startPosition.Y);

            bool enterKey = false;
            if (Controls.Accept(true, false, false) == true && new Rectangle((int)p.X, (int)p.Y, Size, RASTER_SIZE).Contains(MouseHandler.MousePosition) == true)
            {
                if (selector == RelPosition)
                {
                    enterKey = true;
                    SoundManager.PlaySound("select");
                }
                else
                {
                    selector = RelPosition;
                }
            }
            if (Controls.Accept(false, false, true) == true)
            {
                if (selector == RelPosition)
                {
                    enterKey = true;
                    SoundManager.PlaySound("select");
                }
            }
            if (KeyBoardHandler.KeyPressed(Keys.Enter) == true)
            {
                if (selector == RelPosition)
                {
                    enterKey = true;
                    SoundManager.PlaySound("select");
                }
            }

            if (enterKey == true)
            {
                switch (ButtonMode)
                {
                    case ButtonModes.Key:
                        if (s._currentText.Length < s._maxChars)
                        {
                            String st = DisplayText;
                            if (ButtonMode == ButtonModes.Key)
                                st = CapsLock == true ? ReturnText.ToUpper() : ReturnText.ToLower();
                            s._currentText += st;
                            if (CapsLock == true) CapsLock = false;
                        }
                        break;
                    case ButtonModes.Delete:
                        if (s._currentText != String.Empty)
                            s._currentText = s._currentText.Remove(s._currentText.Length - 1, 1);
                        break;
                    case ButtonModes.Default:
                        s._currentText = s._defaultName;
                        break;
                    case ButtonModes.Enter:
                        s.Confirm();
                        break;
                    case ButtonModes.CapsLock:
                        CapsLock = !CapsLock;
                        break;
                }
            }
        }
    }
}
