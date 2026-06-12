using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class NotificationPopup
{
    private const int DEFAULT_DELAY = 500;
    private const int NO_DELAY_WAIT_FOR_INPUT = 0;
    private const int BACKGROUNDS_PER_ROW = 3;
    private const int ICONS_PER_ROW = 3;

    private Texture2D? _background;
    private Texture2D? _icon;
    private int _frameSizeBack;
    private int _frameSizeIcon;
    private float _positionY = -12f;

    private bool _started;
    private float _scale = 2f;
    private String _soundEffect = String.Empty;
    private String _text = String.Empty;
    private Size _size = new Size(13, 3);
    private Vector2 _backgroundIndex = Vector2.Zero;
    private Vector2 _iconIndex = Vector2.Zero;
    private int _delay;
    public DateTime _delayDate;

    public bool _waitForInput;
    public bool _interacted;
    public String _scriptFile = String.Empty;
    public bool _forceAccept;
    public bool IsReady { get; set; }

    public void Setup(String text, int delay = DEFAULT_DELAY, int backgroundIndex = 0,
                      int iconIndex = 0, String soundEffect = "", String scriptFile = "",
                      bool forceAccept = false)
    {
        _scale = (float)((int)(Core.SpriteBatch.InterfaceScale() * 2));
        _text = text;

        if (delay != -1)
        {
            if (delay == NO_DELAY_WAIT_FOR_INPUT)
            {
                _waitForInput = true;
            }
            _delay = delay;
        }
        else
        {
            _delay = DEFAULT_DELAY;
        }

        if (backgroundIndex != -1)
        {
            _backgroundIndex = new Vector2(backgroundIndex, 0f);
        }
        else
        {
            _backgroundIndex = Vector2.Zero;
        }

        while (_backgroundIndex.X >= BACKGROUNDS_PER_ROW)
        {
            _backgroundIndex.X -= BACKGROUNDS_PER_ROW;
            _backgroundIndex.Y += 1f;
        }
        if (_backgroundIndex.X < 0f)
        {
            _backgroundIndex.X = 0f;
        }

        Texture2D backTexture = TextureManager.GetTexture(@"Textures\Notifications\Backgrounds");
        _frameSizeBack = (int)(backTexture.Width / BACKGROUNDS_PER_ROW);
        _background = TextureManager.GetTexture(
            backTexture,
            new Rectangle(
                (int)(_backgroundIndex.X * _frameSizeBack),
                (int)(_backgroundIndex.Y * _frameSizeBack),
                _frameSizeBack,
                _frameSizeBack));

        int backGroundScaleY = 1;
        if (Core.SpriteBatch.InterfaceScale() > 1f)
        {
            backGroundScaleY = 2;
        }
        else if (Core.SpriteBatch.InterfaceScale() < 1f)
        {
            backGroundScaleY = -2;
        }

        _positionY = (int)(0 - (int)((_size.Height + backGroundScaleY) * (_frameSizeBack / 3) * _scale) - (_frameSizeBack / 3 * _scale) - 5);

        if (iconIndex != -1)
        {
            _iconIndex = new Vector2(iconIndex, 0f);
        }
        else
        {
            _iconIndex = Vector2.Zero;
        }

        while (_iconIndex.X >= ICONS_PER_ROW)
        {
            _iconIndex.X -= ICONS_PER_ROW;
            _iconIndex.Y += 1f;
        }
        if (_iconIndex.X < 0f)
        {
            _iconIndex.X = 0f;
        }

        Texture2D iconTexture = TextureManager.GetTexture(@"Textures\Notifications\Icons");
        _frameSizeIcon = (int)(iconTexture.Width / ICONS_PER_ROW);
        _icon = TextureManager.GetTexture(
            iconTexture,
            new Rectangle(
                (int)(_iconIndex.X * _frameSizeIcon),
                (int)(_iconIndex.Y * _frameSizeIcon),
                _frameSizeIcon,
                _frameSizeIcon));

        _scriptFile = scriptFile;
        _soundEffect = soundEffect;
        _forceAccept = forceAccept;
    }

    public void Dismiss()
    {
        _waitForInput = false;
        _delayDate = DateTime.Now;
        _interacted = true;
    }

    public void Update()
    {
        if (_started == false)
        {
            _delayDate = DateTime.Now.AddMilliseconds((double)(_delay * 10));
            _started = true;
        }

        int backGroundScaleY = 1;
        if (Core.SpriteBatch.InterfaceScale() > 1f)
        {
            backGroundScaleY = 2;
        }
        else if (Core.SpriteBatch.InterfaceScale() < 1f)
        {
            backGroundScaleY = -2;
        }

        if (_waitForInput == true)
        {
            if (_positionY < 5f)
            {
                _positionY += (int)((0.7 * (_frameSizeBack / 3.0) / (_size.Height + backGroundScaleY)) * Core.SpriteBatch.InterfaceScale());
            }
            else
            {
                if (_soundEffect.Equals(String.Empty) == false)
                {
                    SoundManager.PlaySound(@"Notifications\" + _soundEffect);
                    _soundEffect = String.Empty;
                }
            }
        }
        else
        {
            if (DateTime.Now < _delayDate)
            {
                if (_positionY < 5f)
                {
                    _positionY += (int)((0.7 * (_frameSizeBack / 3.0) / (_size.Height + backGroundScaleY)) * Core.SpriteBatch.InterfaceScale());
                }
                else
                {
                    if (_soundEffect.Equals(String.Empty) == false)
                    {
                        SoundManager.PlaySound(@"Notifications\" + _soundEffect);
                        _soundEffect = String.Empty;
                    }
                }
            }
            else
            {
                int backY = (int)(0 - (int)((_size.Height + backGroundScaleY) * (_frameSizeBack / 3) * _scale) - (_frameSizeBack / 3 * _scale) - 5);
                if (_interacted == true || _forceAccept == true)
                {
                    if (_positionY > backY)
                    {
                        _positionY -= (int)((1.7 * (_frameSizeBack / 3.0) / (_size.Height + backGroundScaleY)) * Core.SpriteBatch.InterfaceScale());
                        if (_positionY <= backY)
                        {
                            _positionY = backY;
                            if (_scriptFile.Equals(String.Empty) == false)
                            {
                                ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(_scriptFile, 0, false, false, "Notification");
                            }
                            IsReady = true;
                        }
                    }
                }
                else
                {
                    if (_positionY > backY)
                    {
                        _positionY -= (int)((1.1 * (_frameSizeBack / 3.0) / (_size.Height + backGroundScaleY)) * Core.SpriteBatch.InterfaceScale());
                        if (_positionY <= backY)
                        {
                            _positionY = backY;
                            IsReady = true;
                        }
                    }
                }
            }
        }
    }

    public void Draw()
    {
        if (_background == null || _icon == null) return;

        String textHeader = _text.GetSplit(0, "*").Replace('~', '\n');
        String textBody = _text.GetSplit(1, "*").Replace('~', '\n');

        while (FontManager.TextFont.MeasureString(textHeader).X * (int)(_scale * 2) >
               (int)((_size.Width * (_frameSizeBack / 3) - _frameSizeBack / 3 * 4) * _scale))
        {
            _size.Width += 1;
        }

        textHeader = textHeader.CropStringToWidth(FontManager.TextFont, (int)(_scale * 2), (int)((_size.Width * (_frameSizeBack / 3) - _frameSizeBack / 3 * 4) * _scale));

        while (FontManager.TextFont.MeasureString(textHeader).Y * (int)(_scale * 2) +
               FontManager.TextFont.MeasureString(textBody).Y * (int)_scale >
               (int)((_size.Height * _frameSizeBack / 3) - _frameSizeBack / 3) * _scale - 5)
        {
            _size.Height += 1;
        }

        int backGroundOffsetX = (int)(Core.windowSize.Width - (_size.Width * (_frameSizeBack / 3 * _scale)) - (_frameSizeBack / 3) * _scale - (5 * Core.SpriteBatch.InterfaceScale()));

        Canvas.DrawImageBorder(
            _background,
            (int)_scale,
            new Rectangle(
                backGroundOffsetX,
                (int)_positionY,
                (int)(_size.Width * (_frameSizeBack / 3) * _scale),
                (int)(_size.Height * (_frameSizeBack / 3) * _scale)));

        Core.SpriteBatch.Draw(
            _icon,
            new Rectangle(
                (int)(backGroundOffsetX + ((_frameSizeBack / 3 + 3) * _scale) - (_icon.Width / 3 * _scale) + 8 * _scale),
                (int)(_positionY + (int)(_size.Height * (_frameSizeBack / 3) * _scale / 2) - (_icon.Height / 2 * _scale)),
                (int)(_icon.Width * _scale),
                (int)(_icon.Height * _scale)),
            Color.White);

        int textOffset = (int)(backGroundOffsetX + _frameSizeBack / 3 * _scale * 4);
        if (textBody.Equals(String.Empty) == false)
        {
            if (textHeader.Equals(String.Empty) == false)
            {
                Core.SpriteBatch.DrawString(FontManager.TextFont,
                    textHeader.CropStringToWidth(FontManager.TextFont, (int)(_scale * 2), (int)((_size.Width * (_frameSizeBack / 3) - _frameSizeBack / 3 * 4) * _scale)),
                    new Vector2(textOffset, (int)(_positionY + _frameSizeBack / 3)),
                    Color.Black, 0f, Vector2.Zero, (float)(_scale * 2), SpriteEffects.None, 0f);
                Core.SpriteBatch.DrawString(FontManager.TextFont,
                    textBody.CropStringToWidth(FontManager.TextFont, (int)_scale, (int)((_size.Width * (_frameSizeBack / 3) - _frameSizeBack / 3 * 4) * _scale)),
                    new Vector2(textOffset, (int)(_positionY + _frameSizeBack / 3 + (FontManager.TextFont.MeasureString(textHeader).Y * _scale * 2))),
                    Color.Black, 0f, Vector2.Zero, (float)_scale, SpriteEffects.None, 0f);
            }
            else
            {
                Core.SpriteBatch.DrawString(FontManager.TextFont,
                    textBody.CropStringToWidth(FontManager.TextFont, (int)_scale, (int)((_size.Width * (_frameSizeBack / 3) - _frameSizeBack / 3 * 4) * _scale)),
                    new Vector2(textOffset, (int)(_positionY + _frameSizeBack / 3)),
                    Color.Black, 0f, Vector2.Zero, (float)_scale, SpriteEffects.None, 0f);
            }
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.TextFont,
                textHeader.CropStringToWidth(FontManager.TextFont, (int)(_scale * 2), (int)((_size.Width * (_frameSizeBack / 3) - _frameSizeBack / 3 * 4) * _scale)),
                new Vector2(textOffset, (int)(_positionY + _frameSizeBack / 3)),
                Color.Black, 0f, Vector2.Zero, (float)(_scale * 2), SpriteEffects.None, 0f);
        }

        String interactText = "[" + Localization.GetString("game_notification_dismiss") + "]";
        if (_scriptFile.Equals(String.Empty) == false || _waitForInput == true)
        {
            interactText = "[" + Localization.GetString("game_notification_accept") + "]";
        }

        Vector2 interactOffset = new Vector2(
            (int)(backGroundOffsetX + (_size.Width * (_frameSizeBack / 3 * _scale)) - FontManager.TextFont.MeasureString(interactText).X * _scale),
            (int)(_positionY + (_size.Height * (_frameSizeBack / 3 * _scale)) + (5 * _scale)));

        Core.SpriteBatch.Draw(
            _background,
            new Rectangle(
                (int)interactOffset.X,
                (int)interactOffset.Y,
                (int)(FontManager.TextFont.MeasureString(interactText).X * _scale),
                (int)(FontManager.TextFont.MeasureString(interactText).Y * _scale)),
            new Rectangle(_frameSizeBack / 3, _frameSizeBack / 3, _frameSizeBack / 3, _frameSizeBack / 3),
            Color.White);
        Core.SpriteBatch.DrawString(FontManager.TextFont,
            interactText,
            interactOffset,
            Color.Black, 0f, Vector2.Zero, (float)_scale, SpriteEffects.None, 0f);
    }
}
