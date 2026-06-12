using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TextCopy;

namespace P3D;

/// <summary>Handles structured text input from the keyboard.</summary>
public class KeyboardInput
{
    public enum InputModifier
    {
        AllChars = 0,
        Numbers = 1,
        Letters = 2,
        Alphanumeric = 3,
        Latin255 = 4,
        GameJolt = 5
    }

    private const float START_HOLD_DELAY = 3.0f;
    private const float HOLD_DELAY_STEP = 0.1f;

    private float _holdDelay = START_HOLD_DELAY;
    private Keys _holdKey = Keys.A;

    private static readonly Keys[] IGNORE_KEYS =
    {
        Keys.Enter, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Escape,
        Keys.LeftShift, Keys.RightShift, Keys.LeftAlt, Keys.RightAlt,
        Keys.LeftControl, Keys.RightControl, Keys.LeftWindows, Keys.RightWindows,
        Keys.Delete, Keys.Home, Keys.End
    };

    public String GetInput()
    {
        String text = String.Empty;
        return GetInput(ref text, InputModifier.AllChars, true, false);
    }

    public String GetInput(InputModifier modifier)
    {
        String text = String.Empty;
        return GetInput(ref text, modifier, true, false);
    }

    public String GetInput(ref String currentText, InputModifier modifier)
    {
        return GetInput(ref currentText, modifier, true, true);
    }

    public String GetInput(ref String currentText, InputModifier modifier,
                           bool canPaste, bool canDelete)
    {
        int carret = currentText.Length;
        return GetInput(ref currentText, ref carret, modifier, canPaste, canDelete);
    }

    public String GetInput(ref String currentText, ref int carretPosition,
                           InputModifier modifier, bool canPaste, bool canDelete)
    {
        Keys[] pressedKeys = KeyBoardHandler.GetPressedKeys();

        if (pressedKeys.Length > 0)
        {
            foreach (Keys k in pressedKeys)
            {
                bool isCtrl = Controls.CtrlPressed();
                if (k == Keys.V && KeyBoardHandler.KeyPressed(Keys.V) == true && isCtrl == true)
                {
                    if (canPaste == true)
                    {
                        String? paste = ClipboardService.GetText();
                        if (paste != null)
                        {
                            AppendString(ref currentText, paste.Replace(Environment.NewLine, ""),
                                ref carretPosition);
                        }
                    }
                }
                else if (k == Keys.Back || k == Keys.Delete)
                {
                    if (currentText.Length > 0 && canDelete == true)
                    {
                        bool isBackspace = k == Keys.Back;
                        if (_holdDelay <= 0f && _holdKey == k)
                        {
                            RemoveString(ref currentText, ref carretPosition, isBackspace);
                        }
                        else if (KeyBoardHandler.KeyPressed(k) == true)
                        {
                            RemoveString(ref currentText, ref carretPosition, isBackspace);
                            _holdKey = k;
                            _holdDelay = START_HOLD_DELAY;
                        }
                    }
                }
                else
                {
                    if (IGNORE_KEYS.Contains(k) == false)
                    {
                        char? pressedChar = KeyCharConverter.GetCharFromKey(k);
                        if (pressedChar.HasValue == true)
                        {
                            String charStr = pressedChar.ToString()!;
                            if (_holdDelay <= 0f && _holdKey == k)
                            {
                                AppendString(ref currentText, charStr, ref carretPosition);
                            }
                            else if (KeyBoardHandler.KeyPressed(k) == true)
                            {
                                AppendString(ref currentText, charStr, ref carretPosition);
                                _holdKey = k;
                                _holdDelay = START_HOLD_DELAY;
                            }
                        }
                    }
                }
            }

            if (KeyBoardHandler.KeyUp(_holdKey) == true)
            {
                _holdDelay = START_HOLD_DELAY;
            }
            else if (_holdDelay > 0f)
            {
                _holdDelay -= HOLD_DELAY_STEP;
                if (_holdDelay < 0f)
                {
                    _holdDelay = 0f;
                }
            }

            if (modifier != InputModifier.AllChars)
            {
                char[] charRange = GetCharRange(modifier);
                for (int i = 0; i < currentText.Length; i++)
                {
                    if (i <= currentText.Length - 1 && charRange.Contains(currentText[i]) == false)
                    {
                        currentText = currentText.Remove(i, 1);
                        i--;
                    }
                }
            }
        }

        return currentText;
    }

    private static void RemoveString(ref String text, ref int carret, bool isBackspace)
    {
        if (isBackspace == true)
        {
            if (carret > 0)
            {
                text = text.Remove(carret - 1, 1);
                carret--;
            }
        }
        else
        {
            if (carret < text.Length)
            {
                text = text.Remove(carret, 1);
            }
        }
    }

    private static void AppendString(ref String text, String appendText, ref int carret)
    {
        text = text.Insert(carret, appendText);
        carret += appendText.Length;
    }

    private static char[] GetCharRange(InputModifier mod)
    {
        String s = mod switch
        {
            InputModifier.Latin255 =>
                " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
                "¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿" +
                "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞß" +
                "àáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ",
            InputModifier.Letters =>
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
            InputModifier.Numbers =>
                "0123456789",
            InputModifier.Alphanumeric =>
                " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
            InputModifier.GameJolt =>
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
            _ => ""
        };
        return s.ToCharArray();
    }

    // Textbox inner class

    /// <summary>A self-contained text box that handles input and rendering.</summary>
    public class Textbox
    {
        private const String TEST_HEIGHT_CHARS =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz;:_-.,*~+'#1234567890?ß\\/!\"§$%&/()}][{";
        private const int DEFAULT_WIDTH = 120;
        private const int CARRET_LINE_THICKNESS = 1;
        private const int CONTENT_TOP_PADDING = 1;
        private const int BORDER_PADDING = 2;
        private const int NO_MAX_LENGTH = -1;

        private String _text = String.Empty;
        private readonly KeyboardInput _inputHandler = new KeyboardInput();

        private SpriteFont _font;
        private Color _fontColor = Color.Black;
        private Color _backColor = Color.White;
        private Color _borderColor = Color.Black;
        private int _width = DEFAULT_WIDTH;
        private int _height = NO_MAX_LENGTH;
        private int _borderWidth = 1;
        private Point _position = new Point(100, 100);

        private bool _isPassword;
        private char _passwordChar = '●';

        private readonly char[] _carretJumpmarks =
            " *²³+#°',.;-:><|!\"§%&/()=?{[]}\\ @".ToCharArray();
        private InputModifier _inputType = InputModifier.Latin255;

        private bool _canPaste = true;
        private bool _canCopyAndCut = true;
        private int _maxLength = NO_MAX_LENGTH;
        private int _carretPosition;
        private int _selectionStart;
        private int _selectionLength;
        private bool _isFocused = true;

        public Textbox(SpriteFont font)
        {
            _font = font;
        }

        public String Text
        {
            get => _text;
            set => _text = value;
        }

        public bool IsPassword
        {
            get => _isPassword;
            set => _isPassword = value;
        }

        public char PasswordChar
        {
            get => _passwordChar;
            set => _passwordChar = value;
        }

        public int MaxLength
        {
            get => _maxLength;
            set => _maxLength = value;
        }

        public bool CanPaste
        {
            get => _canPaste;
            set => _canPaste = value;
        }

        public bool CanCopyAndCut
        {
            get => _canCopyAndCut;
            set => _canCopyAndCut = value;
        }

        public int Width
        {
            get => _width;
            set => _width = value;
        }

        public int Height
        {
            get => _height;
            set => _height = value;
        }

        public int CarretPosition
        {
            get => _carretPosition;
            set => _carretPosition = value.Clamp(0, _text.Length);
        }

        public int SelectionStart
        {
            get => _selectionStart;
            set => _selectionStart = value.Clamp(0, _text.Length);
        }

        public int SelectionLength
        {
            get => _selectionLength;
            set => _selectionLength = value.Clamp(_selectionStart, _text.Length);
        }

        public InputModifier InputType
        {
            get => _inputType;
            set => _inputType = value;
        }

        public void Update()
        {
            if (_isFocused == false)
            {
                return;
            }

            if (Controls.CtrlPressed() == true)
            {
                if (KeyBoardHandler.KeyPressed(Keys.A) == true)
                {
                    SelectAll();
                }
                if (KeyBoardHandler.KeyPressed(Keys.C) == true)
                {
                    Copy();
                }
                if (KeyBoardHandler.KeyPressed(Keys.X) == true)
                {
                    Cut();
                }
            }

            int beforeCount = _text.Length;
            _inputHandler.GetInput(ref _text, InputModifier.AllChars, true, true);
            CropText();
            if (beforeCount != _text.Length)
            {
                _carretPosition += _text.Length - beforeCount;
            }
        }

        public void SelectAll()
        {
            _carretPosition = _text.Length;
            _selectionStart = 0;
            _selectionLength = _text.Length;
        }

        public void Deselect()
        {
            _selectionLength = 0;
        }

        public void Copy()
        {
            if (_selectionLength > 0)
            {
                try
                {
                    ClipboardService.SetText(SelectedText);
                }
                catch (Exception)
                {
                    Logger.Log(Logger.LogTypes.Message,
                        "KeyboardInput.cs: An error occurred while copying text to the clipboard.");
                }
            }
        }

        public void Cut()
        {
            if (_selectionLength > 0)
            {
                try
                {
                    ClipboardService.SetText(SelectedText);
                    _text = _text.Remove(_selectionStart, _selectionLength);
                    Deselect();
                }
                catch (Exception)
                {
                    Logger.Log(Logger.LogTypes.Message,
                        "KeyboardInput.cs: An error occurred while cutting text to the clipboard.");
                }
            }
        }

        public String SelectedText
        {
            get
            {
                if (_selectionLength <= 0)
                {
                    return "";
                }
                return _text.Substring(_selectionStart, _selectionLength);
            }
        }

        private void CropText()
        {
            if (_maxLength == NO_MAX_LENGTH)
            {
                while ((int)_font.MeasureString(_text).X > _width)
                {
                    _text = _text[..^1];
                }
            }
            else
            {
                if (_text.Length > _maxLength)
                {
                    _text = _text[.._maxLength];
                }
            }
        }

        public void Draw()
        {
            int contentHeight = _height >= 0
                ? _height
                : (int)_font.MeasureString(TEST_HEIGHT_CHARS).Y + BORDER_PADDING;

            if (_borderWidth > 0)
            {
                Core.SpriteBatch.DrawRectangle(new Rectangle(
                    _position.X,
                    _position.Y,
                    _width + _borderWidth * 2,
                    contentHeight + _borderWidth * 2),
                    _borderColor);
            }

            Core.SpriteBatch.DrawRectangle(new Rectangle(
                _position.X + _borderWidth,
                _position.Y + _borderWidth,
                _width, contentHeight),
                _backColor);

            Core.SpriteBatch.DrawString(_font, _text,
                new Vector2(_position.X + _borderWidth, _position.Y + _borderWidth + CONTENT_TOP_PADDING),
                _fontColor);

            int carretX = _carretPosition == _text.Length
                ? (int)_font.MeasureString(_text).X
                : (int)_font.MeasureString(_text.Remove(_carretPosition)).X;

            Core.SpriteBatch.DrawLine(
                new Vector2(_position.X + carretX + CONTENT_TOP_PADDING, _position.Y),
                new Vector2(_position.X + carretX + CONTENT_TOP_PADDING, _position.Y + contentHeight),
                CARRET_LINE_THICKNESS, Color.Green);
        }

        public void Focus()
        {
            _isFocused = true;
        }

        public void DeFocus()
        {
            _isFocused = false;
            _selectionStart = 0;
            _selectionLength = 0;
        }

        public bool IsFocused()
        {
            return _isFocused;
        }
    }
}
