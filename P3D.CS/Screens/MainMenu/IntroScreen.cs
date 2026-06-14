using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class IntroScreen : Screen
{
    private enum IntroStages
    {
        RevealPokemon = 0,
        P3DMoveIn = 1
    }

    private Texture2D _pokemonLogoTexture;
    private Texture2D _3DLogoTexture;
    private IntroStages _introStage = IntroStages.RevealPokemon;

    private int _pokemonRevealStage = 0;
    private int _3Dposition = -100;
    private int _pokemonLogoOffset = 0;

    public IntroScreen()
    {
        Identification = Identifications.IntroScreen;
        CanBePaused = false;
        CanMuteAudio = false;
        CanChat = false;
        CanTakeScreenshot = false;
        CanDrawDebug = false;
        MouseVisible = true;
        CanGoFullscreen = false;

        _pokemonLogoTexture = Core.Content.Load<Texture2D>("GUI/Logos/Pokemon_Small");
        _3DLogoTexture = Core.Content.Load<Texture2D>("GUI/Logos/3D");

        _3Dposition = -(_3DLogoTexture.Height * 2);
        _pokemonLogoOffset = (int)(Core.windowSize.Height / 2 - _pokemonLogoTexture.Height);
    }

    public override void Update()
    {
        switch (_introStage)
        {
            case IntroStages.RevealPokemon:
                UpdateRevealPokemon();
                break;
            case IntroStages.P3DMoveIn:
                Update3DMoveIn();
                break;
        }
    }

    private void UpdateRevealPokemon()
    {
        int textureWidth = _pokemonLogoTexture.Width;
        if (_pokemonRevealStage < textureWidth)
        {
            _pokemonRevealStage += 8;
            if (_pokemonRevealStage >= textureWidth)
            {
                _pokemonRevealStage = textureWidth;
                _introStage = IntroStages.P3DMoveIn;
            }
        }
    }

    private void Update3DMoveIn()
    {
        int p3dLogoWay = (int)(Core.windowSize.Height / 2);

        if (_3Dposition < p3dLogoWay)
        {
            _3Dposition += 6;
        }

        if (_pokemonLogoOffset > (int)(Core.windowSize.Height / 2 - _pokemonLogoTexture.Height * 1.5))
        {
            _pokemonLogoOffset -= 3;
        }
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, Color.Black);

        switch (_introStage)
        {
            case IntroStages.RevealPokemon:
                DrawRevealPokemon();
                break;
            case IntroStages.P3DMoveIn:
                Draw3DMoveIn();
                break;
        }
    }

    private void DrawRevealPokemon()
    {
        Core.SpriteBatch.Draw(_pokemonLogoTexture,
            new Rectangle(
                (int)(Core.windowSize.Width / 2 - _pokemonLogoTexture.Width),
                (int)(Core.windowSize.Height / 2 - _pokemonLogoTexture.Height),
                _pokemonRevealStage * 2,
                _pokemonLogoTexture.Height * 2),
            new Rectangle(0, 0, _pokemonRevealStage, _pokemonLogoTexture.Height),
            Color.White);

        if (_pokemonRevealStage < _pokemonLogoTexture.Width)
        {
            Canvas.DrawGradient(
                new Rectangle(
                    (int)(Core.windowSize.Width / 2 - _pokemonLogoTexture.Width + _pokemonRevealStage * 2 - 60),
                    (int)(Core.windowSize.Height / 2 - _pokemonLogoTexture.Height),
                    60,
                    _pokemonLogoTexture.Height * 2),
                new Color(0, 0, 0, 0), Color.Black, true, -1);
        }
    }

    private void Draw3DMoveIn()
    {
        Core.SpriteBatch.Draw(_3DLogoTexture,
            new Rectangle(
                (int)(Core.windowSize.Width / 2 - _3DLogoTexture.Width),
                _3Dposition,
                _3DLogoTexture.Width * 2,
                _3DLogoTexture.Height * 2),
            Color.White);

        Core.SpriteBatch.Draw(_pokemonLogoTexture,
            new Rectangle(
                (int)(Core.windowSize.Width / 2 - _pokemonLogoTexture.Width),
                _pokemonLogoOffset,
                _pokemonLogoTexture.Width * 2,
                _pokemonLogoTexture.Height * 2),
            Color.White);
    }
}
