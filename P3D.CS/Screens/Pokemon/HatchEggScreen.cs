using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class HatchEggScreen : Screen
{
    private List<Pokemon> _pokemons;
    private Texture2D _background;
    private Texture2D _egg;

    private int _stage = 0;
    private float _delay = 4.0f;
    private float _size = 0.0f;

    private bool _canRename = true;
    private String _message = String.Empty;

    private Pokemon _cPokemon;

    public HatchEggScreen(Screen currentScreen, List<Pokemon> pokemon, bool canRename = true, String message = "")
    {
        PreScreen = currentScreen;
        PlayerStatistics.Track("Eggs hatched", 1);

        Identification = Identifications.HatchEggScreen;
        _pokemons = pokemon;

        _cPokemon = _pokemons[0];
        _cPokemon.EggSteps = 0;
        Core.Player.Pokemons.Add(_cPokemon);

        _canRename = canRename;
        _message = message;

        String dexID = PokemonForms.GetPokemonDataFileName(_cPokemon.Number, _cPokemon.AdditionalData);
        if (dexID.Contains("_") == false)
        {
            if (PokemonForms.GetAdditionalDataForms(_cPokemon.Number) != null &&
                PokemonForms.GetAdditionalDataForms(_cPokemon.Number)!.Contains(_cPokemon.AdditionalData) == true)
                dexID = _cPokemon.Number + ";" + _cPokemon.AdditionalData;
            else
                dexID = _cPokemon.Number.ToString();
        }

        if (_cPokemon.IsShiny == true)
            Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 3);
        else
            Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 2);

        _pokemons.Remove(_cPokemon);

        if (Screen.Level?.OverworldPokemon != null)
            Screen.Level.OverworldPokemon.Visible = false;

        _background = TextureManager.GetTexture(@"GUI\EggBreak", new Rectangle(0, 0, 256, 192), String.Empty);
        _egg = GetEggTexture();

        MusicManager.PlayNoMusic();
    }

    private Texture2D GetEggTexture()
    {
        return TextureManager.GetTexture(@"GUI\EggBreak", new Rectangle(0 + 28 * _stage, 192, 28, 30), String.Empty);
    }

    public override void Draw()
    {
        Core.SpriteBatch.Draw(_background, new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), Color.White);

        if (_stage < 6)
        {
            Core.SpriteBatch.Draw(_egg, new Rectangle(
                (int)(Core.windowSize.Width / 2 - _egg.Width),
                (int)(Core.windowSize.Height / 2 - _egg.Height),
                _egg.Width * 2,
                _egg.Height * 2), Color.White);
        }

        Texture2D pokeTexture = _cPokemon.GetTexture(true);
        Core.SpriteBatch.Draw(pokeTexture, new Rectangle(
            (int)(Core.windowSize.Width / 2 - pokeTexture.Width * _size / 2),
            (int)(Core.windowSize.Height / 2 - pokeTexture.Height * _size / 1.5f),
            (int)(pokeTexture.Width * _size),
            (int)(pokeTexture.Height * _size)), Color.White);

        TextBox.Draw();
        ChooseBox.Draw();
    }

    public override void Update()
    {
        ChooseBox.Update();
        if (ChooseBox.Showing == false)
            TextBox.Update();

        if (TextBox.Showing == false && ChooseBox.Showing == false)
        {
            if (_stage < 6)
            {
                if (_delay > 0.0f)
                    _delay -= 0.1f;

                if (_delay <= 0.0f)
                {
                    _delay = 4.0f;
                    _stage += 1;

                    if (_stage == 6)
                        SoundManager.PlaySound("egg_hatch");
                    else
                        SoundManager.PlaySound(@"Battle\Attacks\Normal\Pound");

                    _egg = GetEggTexture();
                }
            }
            else if (_stage == 6)
            {
                if (_size < 4.0f)
                {
                    _size += 0.08f;
                }
                else
                {
                    String musicLoop = Screen.Level?.CurrentRegion.Split(',')[0] + "_wild_defeat" ?? "wild_defeat";
                    if (MusicManager.SongExists(musicLoop) == false)
                        musicLoop = "wild_defeat";
                    MusicManager.Play(musicLoop);
                    _cPokemon.PlayCry();
                    SoundManager.PlaySound("success", true);
                    _stage = 7;

                    String t = Localization.GetString("hatch_egg_screen_congratulations", "Congratulations!~Your egg hatched into~[POKEMONNAME]!")
                                          .Replace("[POKEMONNAME]", _cPokemon.GetName());
                    if (_message != String.Empty)
                        t = _message;

                    if (_canRename == true)
                    {
                        TextBox.Show(
                            t + "*" + Localization.GetString("hatch_egg_screen_givenickname", "Do you want to give~a nickname to the freshly~hatched [POKEMONNAME]?")
                                .Replace("[POKEMONNAME]", _cPokemon.GetName()) + "%Yes|No%",
                            ResultFunction, false, false, TextBox.DefaultColor);
                    }
                    else
                    {
                        TextBox.Show(t, [], false, false);
                    }
                }
            }
            else if (_stage == 7)
            {
                if (IsCurrentScreen() == true)
                {
                    EndScene();
                    _stage = 8;
                }
            }
        }
    }

    private void ResultFunction(int result)
    {
        if (result == 0)
            Core.SetScreen(new NameObjectScreen(Core.CurrentScreen, _cPokemon));
    }

    private void EndScene()
    {
        if (_pokemons.Count == 0)
            Core.SetScreen(new TransitionScreen(this, PreScreen!, Color.White, false));
        else
            Core.SetScreen(new HatchEggScreen(PreScreen!, _pokemons));
    }
}
