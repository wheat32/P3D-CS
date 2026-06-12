using Microsoft.Xna.Framework;

namespace P3D;

public class RouteSign
{
    private const float SLIDE_DOWN_LIMIT = -60f;
    private const float SLIDE_IN_SPEED = 1.2f;
    private const float VISIBLE_POSITION = 5f;
    private const float DELAY_INITIAL = 13f;
    private const float DELAY_DECREMENT = 0.1f;
    private const int TEXT_OFFSET_Y = 13;
    private const int SIGN_WIDTH = 316;
    private const int SIGN_HEIGHT = 60;
    private const int SIGN_MARGIN = 5;

    private float _positionY = SLIDE_DOWN_LIMIT;
    private bool _show;
    private float _delay;
    private String _text = String.Empty;

    public void Setup(String newText)
    {
        if (newText.ToLower().Equals(_text.ToLower()) == false)
        {
            _show = true;
            _delay = DELAY_INITIAL;
            _text = newText;
        }
    }

    public void Hide()
    {
        _show = false;
    }

    public void Update()
    {
        if (_delay > 0f)
        {
            if (_positionY < VISIBLE_POSITION)
            {
                _positionY += SLIDE_IN_SPEED;
            }
            _delay -= DELAY_DECREMENT;
            if (_delay <= 0f)
            {
                _delay = 0f;
            }
        }
        else
        {
            if (_positionY > SLIDE_DOWN_LIMIT)
            {
                _positionY -= SLIDE_IN_SPEED;
                if (_positionY <= SLIDE_DOWN_LIMIT)
                {
                    _show = false;
                }
            }
        }
    }

    public void Draw()
    {
        if (_show == false) return;

        String placeString = Localization.GetString("Places_" + _text, _text);
        int pX = (int)(SIGN_WIDTH / 2) - (int)(FontManager.InGameFont.MeasureString(placeString).X / 2);

        Core.SpriteBatch.DrawInterface(
            TextureManager.GetTexture(@"GUI\Overworld\Sign"),
            new Rectangle(SIGN_MARGIN, (int)_positionY, SIGN_WIDTH, SIGN_HEIGHT),
            Color.White);
        Core.SpriteBatch.DrawInterfaceString(
            FontManager.InGameFont,
            placeString,
            new Vector2(pX, (int)_positionY + TEXT_OFFSET_Y),
            Color.Black);
    }
}
