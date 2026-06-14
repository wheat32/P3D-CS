using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class ChooseAttackScreen : Screen
{
    public delegate void ChoseAttack(Pokemon pokemon, int attackIndex);

    private Pokemon _pokemon;
    private Texture2D _mainTexture;
    private int _attackIndex = 0;
    private bool _canChooseHMMove = true;
    private bool _canExit = true;
    private ChoseAttack? _doSub;

    public static int Selected = -1;
    public static bool Exited = false;
    public static bool Chosen = false;

    public ChooseAttackScreen(Screen currentScreen, Pokemon pokemon, bool canChooseHmMove, bool canExit, ChoseAttack? doSub)
    {
        Identification = Identifications.ChooseAttackScreen;
        MouseVisible = false;
        CanBePaused = true;
        CanMuteAudio = true;
        CanChat = true;
        CanTakeScreenshot = true;
        CanDrawDebug = true;

        PreScreen = currentScreen;
        _pokemon = pokemon;
        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
        _canChooseHMMove = canChooseHmMove;
        _canExit = canExit;
        _doSub = doSub;
    }

    public ChooseAttackScreen(Screen preScreen, Pokemon pokemon, Items.Item? item,
                               Func<int, bool>? onSelect, String title)
    {
        PreScreen = preScreen;
        _pokemon = pokemon;
        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
        Identification = Identifications.ChooseAttackScreen;
    }

    public override void Update()
    {
        if (TextBox.Showing == false)
        {
            if (Controls.Up(true, true, true, true) == true)
                _attackIndex -= 1;
            if (Controls.Down(true, true, true, true) == true)
                _attackIndex += 1;

            _attackIndex = (int)MathHelper.Clamp(_attackIndex, 0, _pokemon.attacks.Count - 1);

            if (Controls.Accept() == true)
            {
                SoundManager.PlaySound("select");
                ClickYes();
            }
            if (Controls.Dismiss() == true)
            {
                if (_canExit == true)
                {
                    SoundManager.PlaySound("select");
                    ClickNo();
                }
            }
        }
    }

    public override void Draw()
    {
        PreScreen?.Draw();
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(0, 0, 0, 150));
        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        Vector2 p = new Vector2(96, 96);

        if (_pokemon.attacks.Count > 0)
        {
            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)p.X, (int)p.Y, 672, 416));

            BattleSystem.Attack a = _pokemon.attacks[_attackIndex];
            String description = a.Description.Replace("’", "'").CropStringToWidth(FontManager.MainFont, 240);

            String power = a.Power.ToString();
            if (power == "0") power = "-";

            String acc = a.Accuracy.ToString();
            if (acc == "0") acc = "-";

            Core.SpriteBatch.DrawString(FontManager.MainFont,
                Localization.GetString("property_Power", "Power") + ": " + power + Environment.NewLine +
                Localization.GetString("property_Accuracy", "Accuracy") + ": " + acc + Environment.NewLine + Environment.NewLine + description,
                new Vector2((int)(p.X + 352 + 48), p.Y + 48), Color.Black);
            Core.SpriteBatch.Draw(a.GetDamageCategoryImage(),
                new Rectangle((int)(p.X + 672 - 16 - 56), (int)(p.Y + 44), 56, 28), Color.White);

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)p.X, (int)p.Y, 352, 416));

            for (int i = 0; i <= _pokemon.attacks.Count - 1; i++)
                DrawAttack(p, i, _pokemon.attacks[i]);
        }
    }

    private void DrawAttack(Vector2 startPosition, int i, BattleSystem.Attack a)
    {
        Vector2 p = new Vector2(startPosition.X + 16, startPosition.Y + 32 + i * (64 + 32));
        if (i == 4) p.Y += 32;

        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Color fontColor = Color.Black;
        if (_attackIndex == i)
        {
            canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            fontColor = Color.White;
        }

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)p.X + 16, (int)p.Y, 288, 64));

        if (fontColor != Color.Black)
            Core.SpriteBatch.DrawString(FontManager.MainFont, a.Name, new Vector2((int)p.X + 34 + 2, (int)(p.Y + 26 + 2)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, a.Name, new Vector2((int)p.X + 34, (int)(p.Y + 26)), fontColor);

        Color c = fontColor;
        int per = (int)((float)a.CurrentPP / a.MaxPP * 100);
        if (per <= 33 && per > 10) c = Color.Orange;
        else if (per <= 10) c = Color.IndianRed;

        if (c != Color.Black)
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_PP", "PP") + " " + a.CurrentPP + " / " + a.MaxPP, new Vector2(p.X + 96 + 2, (int)(p.Y + 56 + 2)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_PP", "PP") + " " + a.CurrentPP + " / " + a.MaxPP, new Vector2(p.X + 96, (int)(p.Y + 56)), c);
        Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath(), a.Type.GetElementImage(), String.Empty),
            new Rectangle((int)(p.X + 34), (int)(p.Y + 56), 48, 16), Color.White);
    }

    private void ClickYes()
    {
        bool valid = true;
        if (_canChooseHMMove == false)
        {
            BattleSystem.Attack a = _pokemon.attacks[_attackIndex];
            if (a.isHMMove == true)
            {
                valid = false;
                TextBox.Show(Localization.GetString("choose_move_CannotChooseHMMove", "Cannot choose HM move."), [], false, false);
            }
        }
        if (valid == true)
        {
            Selected = _attackIndex;
            Core.SetScreen(PreScreen!);
            Exited = true;
            Chosen = true;
            _doSub?.Invoke(_pokemon, _attackIndex);
        }
    }

    private void ClickNo()
    {
        Selected = -1;
        Core.SetScreen(PreScreen!);
        Exited = true;
        Chosen = false;
    }
}
