using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class TextBox
{
    public float reDelay = 0.0f;   // VB: Screen.TextBox.reDelay = 0.0F reset before Show()

    public static readonly Color DEFAULT_COLOR = new Color(16, 24, 32);
    public static readonly Color PLAYER_COLOR = new Color(0, 0, 180);

    public static Color PlayerColor => PLAYER_COLOR;
    public static Color DefaultColor => DEFAULT_COLOR;

    private const float DEFAULT_DELAY = 0.2f;
    private const float RE_DELAY_VALUE = 1.0f;
    private const float DELAY_STEP = 0.1f;
    private const float SLIDE_IN_SPEED = 8.0f;
    private const int TEXT_BOX_WIDTH_HALF = 240;
    private const int TEXT_BOX_HEIGHT = 144;
    private const int TEXT_BOX_ARROW_SIZE = 24;
    private const int TEXT_BOX_LINE1_OFFSET_Y = 40;
    private const int TEXT_BOX_LINE2_OFFSET_Y = 75;
    private const int TEXT_BOX_TEXT_OFFSET_X = 210;
    private const float TEXT_SPEED_SLOW = 0.3f;
    private const float TEXT_SPEED_NORMAL = 0.2f;
    private const float TEXT_SPEED_FAST = 0.1f;
    private const float TEXT_SPEED_INSTANT = 0.0f;

    public static int TextSpeed = 2;

    public String? Text;
    private int _currentChar;
    private int _currentLine;
    private bool _through;
    private bool _clearNextLine;
    private String[] _showText = new String[2];

    public bool Showing;
    private float _delay = DEFAULT_DELAY;
    private bool _doReDelay = true;
    public float ReDelay = 1.5f;
    public float PositionY;
    public bool CanProceed = true;
    public Color TextColor = new Color(16, 24, 33);

    public FontContainer? TextFont = FontManager.GetFontContainer("textfont");

    private Entity[]? _entities;

    public ChooseBox.DoAnswer? ResultFunction;

    public delegate void FollowUpDelegate();
    public FollowUpDelegate? FollowUp;

    public void Show(String text, ChooseBox.DoAnswer? resultFunction, bool doReDelay,
                     bool checkDelay, Color textColor)
    {
        if (ReDelay != 0f && checkDelay == true)
        {
            return;
        }
        if (Showing == false)
        {
            PositionY = Core.windowSize.Height;
            Showing = true;
        }
        _doReDelay = doReDelay;
        Text = text;
        ResultFunction = resultFunction;
        TextColor = textColor;
        _showText[0] = String.Empty;
        _showText[1] = String.Empty;
        _through = false;
        _currentLine = 0;
        _currentChar = 0;
        _delay = DEFAULT_DELAY;
        _clearNextLine = false;
        FormatText();
    }

    public void Show(String text, Entity[] entities, bool doReDelay, bool checkDelay, Color textColor)
    {
        if (ReDelay != 0f && checkDelay == true)
        {
            return;
        }
        if (Showing == false)
        {
            PositionY = Core.windowSize.Height;
            Showing = true;
        }
        _doReDelay = doReDelay;
        Text = text;
        _entities = entities;
        TextColor = textColor;
        _showText[0] = String.Empty;
        _showText[1] = String.Empty;
        _through = false;
        _currentLine = 0;
        _currentChar = 0;
        _delay = DEFAULT_DELAY;
        _clearNextLine = false;
        FormatText();
    }

    public void Show(String text, Entity[] entities, bool doReDelay, bool checkDelay)
    {
        Show(text, entities, doReDelay, checkDelay, TextColor);
    }

    public void Show(String text, Entity[] entities, bool doReDelay)
    {
        Show(text, entities, doReDelay, true);
    }

    public void Show(String text, Entity[] entities)
    {
        Show(text, entities, true);
    }

    public void Show(String text)
    {
        Show(text, [], false, false);
    }

    public void Hide()
    {
        Showing = false;
        if (_doReDelay == true)
        {
            ReDelay = RE_DELAY_VALUE;
        }
    }

    private void FormatText()
    {
        if (Text == null)
        {
            return;
        }
        Text = Text.Replace("<playername>", Core.Player.Name);
        Text = Text.Replace("<player.name>", Core.Player.Name);
        Text = Text.Replace("<rivalname>", Core.Player.RivalName);
        Text = Text.Replace("<rival.name>", Core.Player.RivalName);
        Text = Text.Replace("[POKE]", "Poké");
        Text = Text.Replace("[POKEMON]", "Pokémon");

        DateTime now = DateTime.Now;
        Text = Text.Replace("<clocktime>",
            now.ToString("t", new CultureInfo("en-US")));
        Text = Text.Replace("<daytime>", World.GetTime().ToString());
    }

    public void Update()
    {
        if (Text == null)
        {
            return;
        }
        if (Showing == true)
        {
            ResetCursor();
            float slideTarget = Core.windowSize.Height - (float)(160.0 * Math.Ceiling(Core.SpriteBatch.InterfaceScale()));
            if (PositionY <= slideTarget)
            {
                if (_through == false)
                {
                    if (_currentChar < Text.Length)
                    {
                        if (_delay <= 0f)
                        {
                            if (Text[_currentChar] == '\\')
                            {
                                if (_currentChar + 1 < Text.Length)
                                {
                                    _showText[_currentLine] += Text[_currentChar + 1];
                                    _currentChar += 2;
                                }
                                else
                                {
                                    _currentChar++;
                                }
                            }
                            else
                            {
                                switch (Text[_currentChar])
                                {
                                    case '~':
                                        if (_currentLine == 1)
                                        {
                                            _through = true;
                                        }
                                        else
                                        {
                                            _currentLine++;
                                        }
                                        break;

                                    case '*':
                                        _currentLine = 0;
                                        _clearNextLine = true;
                                        _through = true;
                                        break;

                                    case '%':
                                        ProcessChooseBox();
                                        break;

                                    default:
                                        _showText[_currentLine] += Text[_currentChar];
                                        break;
                                }
                                _currentChar++;
                            }

                            bool fastForward = KeyBoardHandler.KeyDown(KeyBindings.EnterKey1) == true ||
                                               KeyBoardHandler.KeyDown(KeyBindings.EnterKey2) == true ||
                                               MouseHandler.ButtonDown(MouseHandler.MouseButtons.LeftButton) == true ||
                                               ControllerHandler.ButtonDown(Buttons.A) == true ||
                                               ControllerHandler.ButtonDown(Buttons.B) == true;
                            _delay = fastForward == true ? 0f : GetTextSpeed();
                        }
                        else
                        {
                            _delay -= DELAY_STEP;
                        }
                    }
                    else
                    {
                        _through = true;
                    }
                }
                else
                {
                    if (Controls.Accept() == true || Controls.Dismiss() == true)
                    {
                        SoundManager.PlaySound("select");
                        if (_currentChar >= Text.Length)
                        {
                            if (CanProceed == true)
                            {
                                Showing = false;
                                ResetCursor();
                                if (FollowUp != null)
                                {
                                    FollowUp();
                                    FollowUp = null;
                                }
                                TextFont = FontManager.GetFontContainer("textfont");
                                TextColor = TextBox.DEFAULT_COLOR;
                                if (_doReDelay == true)
                                {
                                    ReDelay = RE_DELAY_VALUE;
                                }
                            }
                        }
                        else
                        {
                            _showText[0] = _clearNextLine == true ? "" : _showText[1];
                            _showText[1] = String.Empty;
                            _through = false;
                            _clearNextLine = false;
                        }
                    }
                }
            }
            else
            {
                PositionY -= (float)(SLIDE_IN_SPEED * Core.SpriteBatch.InterfaceScale());
            }
        }
        else
        {
            if (ReDelay > 0f)
            {
                ReDelay -= DELAY_STEP;
                if (ReDelay < 0f)
                {
                    ReDelay = 0f;
                }
            }
        }
    }

    private static void ResetCursor()
    {
        if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
        {
            Mouse.SetPosition(Core.windowSize.Width / 2, Core.windowSize.Height / 2);
        }
    }

    public void Draw()
    {
        if (Showing == false)
        {
            return;
        }

        double scale = Math.Ceiling(Core.SpriteBatch.InterfaceScale());
        int scaledW = (int)(TEXT_BOX_WIDTH_HALF * scale);
        int scaledH = (int)(TEXT_BOX_HEIGHT * scale);
        int scaledArrow = (int)(TEXT_BOX_ARROW_SIZE * scale);
        int cx = Core.windowSize.Width / 2;

        Core.SpriteBatch.Draw(
            TextureManager.GetTexture(@"GUI\Overworld\TextBox"),
            new Rectangle(cx - scaledW, (int)PositionY, scaledW * 2, scaledH),
            new Rectangle(0, 0, 160, 48), Color.White);

        if (CanProceed == true && _through == true)
        {
            Core.SpriteBatch.Draw(
                TextureManager.GetTexture(@"GUI\Overworld\TextBox"),
                new Rectangle(cx + scaledW - scaledArrow, (int)PositionY + scaledH - scaledArrow,
                    scaledArrow, scaledArrow),
                new Rectangle(0, 48, 24, 24), Color.White);
        }

        if (TextFont == null)
        {
            return;
        }

        float fontScale = TextFont.FontName.ToLower() is "textfont" or "braille"
            ? (float)(2.0 * scale)
            : (float)scale;

        int textX = cx - (int)(TEXT_BOX_TEXT_OFFSET_X * scale);
        Core.SpriteBatch.DrawString(TextFont.SpriteFont, _showText[0],
            new Vector2(textX, (int)PositionY + (int)(TEXT_BOX_LINE1_OFFSET_Y * scale)),
            TextColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
        Core.SpriteBatch.DrawString(TextFont.SpriteFont, _showText[1],
            new Vector2(textX, (int)PositionY + (int)(TEXT_BOX_LINE2_OFFSET_Y * scale)),
            TextColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
    }

    private void ProcessChooseBox()
    {
        if (Text == null)
        {
            return;
        }
        String splitText = Text[((_currentChar + 1))..];
        splitText = splitText[..splitText.IndexOf('%')];
        _through = true;
        String[] options = splitText.Split('|');
        Text = Text.Remove(_currentChar, splitText.Length + 1);
        bool hasEntities = _entities != null && _entities.Length > 0;
        if (hasEntities == false && ResultFunction != null)
        {
            Screen.ChooseBox.Show(options, ResultFunction);
        }
        else if (hasEntities == true)
        {
            Screen.ChooseBox.Show(options, 0, _entities!);
        }
        Screen.ChooseBox.TextFont = TextFont;
    }

    private static float GetTextSpeed()
    {
        switch (TextSpeed)
        {
            case 1:
                return TEXT_SPEED_SLOW;

            case 2:
                return TEXT_SPEED_NORMAL;

            case 3:
                return TEXT_SPEED_FAST;

            case 4:
                return TEXT_SPEED_INSTANT;

            default:
                return TEXT_SPEED_NORMAL;
        }
    }
}
