using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;

namespace P3D;

public class TransitionScreen : Screen
{
    public Screen OldScreen = null!;
    public new Screen? NewScreen { get; private set; }

    private int _alpha = 0;
    private bool _reduce = false;
    private Action? _doSub;
    private Color _color;
    private bool _noStuff = false;
    private int _speed;

    public TransitionScreen(Screen oldScreen, Screen newScreen, Color color, bool fadeIn)
        : this(oldScreen, newScreen, color, fadeIn, 10) { }

    public TransitionScreen(Screen oldScreen, Screen newScreen, Color color, bool fadeIn, int speed)
    {
        Identification = Identifications.TransitionScreen;

        OldScreen = oldScreen;
        NewScreen = newScreen;
        CanChat = false;

        _color = color;
        _noStuff = true;
        _speed = speed;

        if (fadeIn == true)
        {
            _alpha = 255;
            _reduce = true;
            CanBePaused = newScreen.CanBePaused;
        }
        else
        {
            CanBePaused = oldScreen.CanBePaused;
        }
    }

    public TransitionScreen(Screen oldScreen, Screen newScreen, Color color, bool fadeIn, Action afterTransition)
        : this(oldScreen, newScreen, color, fadeIn, afterTransition, 10) { }

    public TransitionScreen(Screen oldScreen, Screen newScreen, Color color, bool fadeIn, Action afterTransition, int speed)
    {
        OldScreen = oldScreen;
        NewScreen = newScreen;
        CanChat = false;

        _color = color;
        _doSub = afterTransition;
        _speed = speed;

        if (fadeIn == true)
        {
            afterTransition();
            _alpha = 255;
            _reduce = true;
            CanBePaused = newScreen.CanBePaused;
        }
        else
        {
            CanBePaused = oldScreen.CanBePaused;
        }
    }

    public override void Draw()
    {
        if (_reduce == false)
            OldScreen.Draw();
        else
            NewScreen?.Draw();

        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(_color.R, _color.G, _color.B, _alpha));
    }

    public override void Update()
    {
        if (_reduce == false)
        {
            _alpha += _speed;
            if (OldScreen.UpdateFadeOut == true)
                OldScreen.Update();

            if (_alpha >= 255)
            {
                CanBePaused = NewScreen!.CanBePaused;
                _reduce = true;

                if (_noStuff == false)
                    _doSub?.Invoke();

                Identifications[] screens = [Identifications.PokegearScreen, Identifications.OverworldScreen];
                if (screens.Contains(NewScreen!.Identification) == true)
                {
                    if (Screen.Level.Surfing == true)
                        MusicManager.Play("surf", true);
                    else if (Screen.Level.Riding == true)
                        MusicManager.Play("ride", true);
                    else
                        MusicManager.Play(Level.MusicLoop, true, 0.02F);
                }
            }
        }
        else
        {
            _alpha -= _speed;
            if (NewScreen!.UpdateFadeIn == true)
                NewScreen.Update();

            if (_alpha <= 0)
                ChangeScreen();
        }
    }

    private void ChangeScreen()
    {
        Core.SetScreen(NewScreen!);
        if (OldScreen.Identification == Identifications.BattleScreen && Core.CurrentScreen.Identification == Identifications.OverworldScreen)
        {
            if (Core.Player.UsedItemsToCheckScriptDelayFor.Count > 0)
            {
                foreach (String itemEntry in Core.Player.UsedItemsToCheckScriptDelayFor)
                    Core.Player.CheckItemCountScriptDelay(itemEntry);
                Core.Player.UsedItemsToCheckScriptDelayFor.Clear();
            }
        }
    }
}
