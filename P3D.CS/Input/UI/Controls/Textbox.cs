using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TextCopy;

namespace P3D.UI.GameControls;

/// <summary>A textbox control that handles and displays keyboard input.</summary>
public class Textbox : Control
{
    private const int CHAR_PIXEL_WIDTH = 10;
    private const int SELECTION_ALPHA = 100;

    private KeyboardInput _inputHandler = new KeyboardInput();

    private bool _isPassword = false;
    private char _passwordChar = '●';

    private char[] _carretJumpmarks = " *²³+#°',.;-:><|!\"§%&/()=?{[]}\\@".ToCharArray();
    private KeyboardInput.InputModifier _inputType = KeyboardInput.InputModifier.Latin255;

    private bool _canPaste = true;
    private bool _canCopyAndCut = true;
    private int _maxLength = -1;

    private int _horizontalTextPadding = 1;
    private int _verticalTextPadding = 1;

    private int _carretPosition = 0;
    private int _selectionStart = 0;
    private int _selectionLength = 0;

    public bool IsPassword { get; set; }
    public char PasswordChar { get; set; } = '●';
    public int MaxLength { get; set; } = -1;
    public bool CanPaste { get; set; } = true;
    public bool CanCopyAndCut { get; set; } = true;

    public int CarretPosition
    {
        get => _carretPosition;
        set { _carretPosition = value.Clamp(0, Text.Length); }
    }

    public int SelectionStart
    {
        get => _selectionStart;
        set { _selectionStart = value.Clamp(0, Text.Length); }
    }

    public int SelectionLength
    {
        get => _selectionLength;
        set { _selectionLength = value.Clamp(_selectionStart, Text.Length); }
    }

    public KeyboardInput.InputModifier InputType
    {
        get => _inputType;
        set { _inputType = value; }
    }

    public int HorizontalTextPadding
    {
        get => _horizontalTextPadding;
        set { _horizontalTextPadding = value; }
    }

    public int VerticalTextPadding
    {
        get => _verticalTextPadding;
        set { _verticalTextPadding = value; }
    }

    public String SelectedText
    {
        get
        {
            if (_selectionLength <= 0) return "";
            return Text.Substring(_selectionStart, _selectionLength);
        }
    }

    public Textbox(Screen screenInstance, SpriteFont font) : base(screenInstance)
    {
        Font = font;
    }

    protected override void UpdateClient()
    {
        if (IsFocused == true)
        {
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

            int beforeTextCount = Text.Length;
            int beforeTextCarret = _carretPosition;

            if (_selectionLength > 0)
            {
                String tempTest = "w";
                int tempCarret = 1;
                String testStr = _inputHandler.GetInput(ref tempTest, ref tempCarret, _inputType, true, true);
                if (testStr.Length != 1)
                {
                    Text = Text.Remove(SelectionStart, SelectionLength - SelectionStart);
                    _carretPosition = SelectionStart;
                    SelectionLength = -1;
                    String text = Text;
                    _inputHandler.GetInput(ref text, ref _carretPosition, _inputType, true, true);
                    Text = text;
                }
            }
            else
            {
                String text = Text;
                _inputHandler.GetInput(ref text, ref _carretPosition, _inputType, true, true);
                Text = text;
            }

            if (beforeTextCount == Text.Length)
            {
                _carretPosition = beforeTextCarret;
            }

            if (ControllerHandler.ButtonPressed(Buttons.A))
            {
                Core.SetScreen(new InputScreen(Core.CurrentScreen, "", InputScreen.InputModes.Name,
                    Text, MaxLength, [], ControllerInputCallback));
            }

            CropText();
            UpdateSelection();
        }

        _carretPosition = _carretPosition.Clamp(0, Text.Length);
    }

    private void UpdateSelection()
    {
        if (Controls.Left(true, true, false, false, true, true))
        {
            int carretJumpTo = _carretPosition - 1;
            if (KeyBoardHandler.KeyDown(Keys.LeftControl))
            {
                int jumpTo = 0;
                for (int i = 0; i < _carretPosition - 1; i++)
                {
                    if (Array.IndexOf(_carretJumpmarks, Text[i]) >= 0)
                    {
                        jumpTo = i;
                    }
                }
                carretJumpTo = jumpTo;
            }

            if (SelectionLength == 0)
            {
                _carretPosition = carretJumpTo;
            }
        }
        if (Controls.Right(true, true, false, false, true, true))
        {
            if (KeyBoardHandler.KeyDown(Keys.LeftControl))
            {
                int jumpTo = Text.Length;
                for (int i = _carretPosition; i < Text.Length; i++)
                {
                    if (Array.IndexOf(_carretJumpmarks, Text[i]) >= 0)
                    {
                        jumpTo = i;
                        break;
                    }
                }
                _carretPosition = jumpTo + 1;
            }
            else
            {
                _carretPosition += 1;
            }
        }
    }

    private void ControllerInputCallback(String result)
    {
        Text = result;
        _selectionLength = 0;
        _selectionStart = 0;
        _carretPosition = Text.Length;

        CropText();
        UpdateSelection();
    }

    protected override Rectangle GetClientRectangle()
    {
        int contentHeight = Height;
        if (contentHeight < 0 && Font != null)
        {
            contentHeight = (int)(Font.MeasureString(TESTFORHEIGHTCHARS).Y * FontSize) + 2;
        }

        return new Rectangle(Position.X, Position.Y,
            Width + (BorderWidth * 2), contentHeight + (BorderWidth * 2));
    }

    public void SelectAll()
    {
        _carretPosition = Text.Length;
        _selectionStart = 0;
        _selectionLength = Text.Length;
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
                Logger.Log(Logger.LogTypes.Message, "Textbox.cs: An error occurred while copying text to the clipboard.");
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
                Text = Text.Remove(_selectionStart, _selectionLength);
                Deselect();
            }
            catch (Exception)
            {
                Logger.Log(Logger.LogTypes.Message, "Textbox.cs: An error occurred while cutting text to the clipboard.");
            }
        }
    }

    private void CropText()
    {
        if (Font == null) return;
        if (MaxLength == -1)
        {
            while ((int)(Font.MeasureString(Text).X * FontSize) > Width - _horizontalTextPadding * 2)
            {
                Text = Text.Remove(Text.Length - 1, 1);
            }
        }
        else
        {
            if (Text.Length > MaxLength)
            {
                Text = Text.Remove(MaxLength);
            }
        }
    }

    protected override void DrawClient()
    {
        if (Font == null) return;

        int contentHeight = Height;
        if (contentHeight < 0)
        {
            contentHeight = (int)(Font.MeasureString(TESTFORHEIGHTCHARS).Y * FontSize) + 2;
        }

        if (BorderWidth > 0)
        {
            Core.SpriteBatch.DrawRectangle(
                new Rectangle(Position.X, Position.Y,
                    Width + (BorderWidth * 2), contentHeight + (BorderWidth * 2)),
                BorderColor);
        }

        Core.SpriteBatch.DrawRectangle(
            new Rectangle(Position.X + BorderWidth, Position.Y + BorderWidth, Width, contentHeight),
            BackColor);

        String drawText = Text;
        if (IsPassword == true)
        {
            drawText = String.Empty;
            for (int i = 0; i < Text.Length; i++)
            {
                drawText += PasswordChar.ToString();
            }
        }

        FontRenderer.DrawString(Font, drawText,
            new Vector2(Position.X + BorderWidth + _verticalTextPadding,
                        Position.Y + BorderWidth + 1 + _horizontalTextPadding),
            FontColor, 0f, Vector2.Zero, FontSize, SpriteEffects.None, 0f);

        if (IsFocused == true)
        {
            int carretDrawPosition = 0;
            Vector2 textSize = Font.MeasureString(drawText);

            if (textSize.Y <= 0f)
            {
                textSize.Y = Font.MeasureString(TESTFORHEIGHTCHARS).Y;
            }

            if (_carretPosition == Text.Length)
            {
                carretDrawPosition = (int)(textSize.X * FontSize);
            }
            else
            {
                carretDrawPosition = (int)(Font.MeasureString(drawText.Remove(_carretPosition)).X * FontSize);
            }

            Core.SpriteBatch.DrawLine(
                new Vector2(Position.X + carretDrawPosition + (int)Math.Ceiling(FontSize) + _verticalTextPadding + BorderWidth,
                            Position.Y + BorderWidth + _horizontalTextPadding),
                new Vector2(Position.X + carretDrawPosition + (int)Math.Ceiling(FontSize) + _verticalTextPadding + BorderWidth,
                            Position.Y + textSize.Y * FontSize + BorderWidth + _horizontalTextPadding),
                2.0f, BackColor.Invert());

            int startPoint = SelectionStart;
            int endPoint = SelectionStart + SelectionLength;

            if (SelectionLength < 0)
            {
                startPoint = SelectionStart + SelectionLength;
                endPoint = SelectionStart;
            }

            Core.SpriteBatch.DrawRectangle(
                new Rectangle(
                    Position.X + _verticalTextPadding + BorderWidth + startPoint * CHAR_PIXEL_WIDTH,
                    Position.Y + BorderWidth + _horizontalTextPadding,
                    (endPoint - startPoint) * CHAR_PIXEL_WIDTH,
                    (int)(textSize.Y * FontSize)),
                new Color(0, 122, 230, SELECTION_ALPHA));
        }
    }
}
