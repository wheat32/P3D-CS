using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class PokemonImageView
{
    public bool Showing;
    public float Delay = 0.0f;

    private Texture2D? _texture;
    private bool _front = true;

    public void Show(String id, bool shiny, bool front)
    {
        String pokemonID = id.GetSplit(0);
        String pokemonAddition = "xXx";
        if (pokemonID.Contains("_") == true)
        {
            pokemonAddition = PokemonForms.GetAdditionalValueFromDataFile(id.GetSplit(0));
            pokemonID = id.GetSplit(0).GetSplit(0, "_");
        }
        if (pokemonID.Contains(";") == true)
        {
            pokemonAddition = id.GetSplit(0).GetSplit(1, ";");
            pokemonID = id.GetSplit(0).GetSplit(0, ";");
        }
        Pokemon p = Pokemon.GetPokemonByID(int.Parse(pokemonID), pokemonAddition, true);
        p.PlayCry();
        Delay = 8.0f;
        Showing = true;
        p.IsShiny = shiny;
        _texture = p.GetTexture(front);
    }

    public void Show(Pokemon pokemon, bool front)
    {
        pokemon.PlayCry();
        Delay = 8.0f;
        Showing = true;
        _texture = pokemon.GetTexture(front);
    }

    public void Show(Texture2D texture)
    {
        _texture = texture;
        Delay = 8.0f;
        Showing = true;
    }

    public void Update()
    {
        if (Delay > 0.0f)
        {
            Delay -= 0.1f;
            if (Delay <= 0.0f) Delay = 0.0f;
        }
        else if (Delay == 0.0f)
        {
            if (Controls.Accept() == true || Controls.Dismiss() == true)
            {
                Showing = false;
                SoundManager.PlaySound("select");
            }
        }
    }

    public void Draw()
    {
        if (Showing == false || _texture == null) return;

        Vector2 p = Core.GetMiddlePosition(new Size(320, 320));
        Canvas.DrawImageBorder(TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty), 2, new Rectangle((int)p.X, (int)p.Y, 320, 320));

        if (Delay == 0.0f)
            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(@"GUI\Overworld\ImageView"), new Rectangle((int)p.X + 144 + 160 + 16, (int)p.Y + 144 + 160 + 32, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);

        Core.SpriteBatch.Draw(_texture,
            new Rectangle(
                (int)(p.X + 160 + 16 - MathHelper.Min(_texture.Width * 3, 288) / 2),
                (int)(p.Y + 160 + 16 - MathHelper.Min(_texture.Height * 3, 288) / 2),
                MathHelper.Min(_texture.Width * 3, 288),
                MathHelper.Min(_texture.Height * 3, 288)),
            Color.White);
    }
}
