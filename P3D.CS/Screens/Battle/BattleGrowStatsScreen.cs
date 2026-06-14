using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class BattleGrowStatsScreen : Screen
{
    private Texture2D _mainTexture;

    private int _newMaxHP = 0;
    private int _newAttack = 0;
    private int _newDefense = 0;
    private int _newSpDefense = 0;
    private int _newSpAttack = 0;
    private int _newSpeed = 0;

    private float _delay = 0.0f;

    private Pokemon _pokemon;
    private int[] _oldStats;

    private const int OLD_OFFSET = 192;
    private const int NEW_OFFSET = 240;
    private const int RESULT_OFFSET = 320;

    public BattleGrowStatsScreen(Screen currentScreen, Pokemon p, int[] oldStats)
    {
        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
        PreScreen = currentScreen;
        Identification = Identifications.BattleGrowStatsScreen;

        _newMaxHP = p.MaxHP - oldStats[0];
        _newAttack = p.Attack - oldStats[1];
        _newDefense = p.Defense - oldStats[2];
        _newSpAttack = p.SpAttack - oldStats[3];
        _newSpDefense = p.SpDefense - oldStats[4];
        _newSpeed = p.Speed - oldStats[5];

        _pokemon = p;
        _oldStats = oldStats;
    }

    public override void Draw()
    {
        PreScreen.Draw();

        Vector2 p = new Vector2(Core.windowSize.Width - 544, 32);

        Canvas.DrawImageBorder(TextureManager.GetTexture(_mainTexture, new Rectangle(0, 0, 48, 48)), 2, new Rectangle((int)p.X, (int)p.Y, 480, 352));

        Texture2D pokeTexture = _pokemon.GetMenuTexture();
        if (_pokemon.IsTransformed == true)
        {
            pokeTexture = Pokemon.GetPokemonByID(_pokemon.OriginalNumber, _pokemon.AdditionalData).GetMenuTexture();
        }
        Vector2 pokeTextureScale = new Vector2((float)(32.0 / pokeTexture.Width) * 2, (float)(32.0 / pokeTexture.Height) * 2);
        Core.SpriteBatch.Draw(pokeTexture, new Rectangle((int)(p.X + 20), (int)(p.Y + 20), (int)(pokeTexture.Width * pokeTextureScale.X), (int)(pokeTexture.Height * pokeTextureScale.Y)), Color.White);
        Core.SpriteBatch.DrawString(FontManager.InGameFont, _pokemon.GetDisplayName(), new Vector2(p.X + 90, p.Y + 32), Color.Black);
        String levelText = Localization.GetString("level_up_PokemonReachedLevel", "[POKEMONNAME] reached~level [LEVELNUMBER]!")
            .Replace("[POKEMONNAME]", String.Empty).Replace("[LEVELNUMBER]", _pokemon.Level.ToString());
        Core.SpriteBatch.DrawString(FontManager.InGameFont, levelText,
            new Vector2(p.X + 90 + FontManager.InGameFont.MeasureString(_pokemon.GetDisplayName()).X, p.Y + 32), Color.Black);

        if (_delay >= 3.0f)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("property_MaxHP") + ":", new Vector2(p.X + 32, p.Y + 84), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _oldStats[0].ToString(), new Vector2(p.X + 32 + OLD_OFFSET, p.Y + 84), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("property_Attack") + ":", new Vector2(p.X + 32, p.Y + 124), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _oldStats[1].ToString(), new Vector2(p.X + 32 + OLD_OFFSET, p.Y + 124), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("property_Defense") + ":", new Vector2(p.X + 32, p.Y + 164), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _oldStats[2].ToString(), new Vector2(p.X + 32 + OLD_OFFSET, p.Y + 164), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("property_Sp_Attack") + ":", new Vector2(p.X + 32, p.Y + 204), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _oldStats[3].ToString(), new Vector2(p.X + 32 + OLD_OFFSET, p.Y + 204), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("property_Sp_Defense") + ":", new Vector2(p.X + 32, p.Y + 244), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _oldStats[4].ToString(), new Vector2(p.X + 32 + OLD_OFFSET, p.Y + 244), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("property_Speed") + ":", new Vector2(p.X + 32, p.Y + 284), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _oldStats[5].ToString(), new Vector2(p.X + 32 + OLD_OFFSET, p.Y + 284), Color.Black);
        }

        if (_delay >= 5.0f)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "+ " + _newMaxHP, new Vector2(p.X + 32 + NEW_OFFSET, p.Y + 84), Color.Black);
        }
        if (_delay >= 5.5f)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "+ " + _newAttack, new Vector2(p.X + 32 + NEW_OFFSET, p.Y + 124), Color.Black);
        }
        if (_delay >= 6.0f)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "+ " + _newDefense, new Vector2(p.X + 32 + NEW_OFFSET, p.Y + 164), Color.Black);
        }
        if (_delay >= 6.5f)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "+ " + _newSpAttack, new Vector2(p.X + 32 + NEW_OFFSET, p.Y + 204), Color.Black);
        }
        if (_delay >= 7.0f)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "+ " + _newSpDefense, new Vector2(p.X + 32 + NEW_OFFSET, p.Y + 244), Color.Black);
        }
        if (_delay >= 7.5f)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "+ " + _newSpeed, new Vector2(p.X + 32 + NEW_OFFSET, p.Y + 284), Color.Black);
        }

        if (_delay >= 9.0f)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "= " + _pokemon.MaxHP, new Vector2(p.X + 32 + RESULT_OFFSET, p.Y + 84), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "= " + _pokemon.Attack, new Vector2(p.X + 32 + RESULT_OFFSET, p.Y + 124), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "= " + _pokemon.Defense, new Vector2(p.X + 32 + RESULT_OFFSET, p.Y + 164), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "= " + _pokemon.SpAttack, new Vector2(p.X + 32 + RESULT_OFFSET, p.Y + 204), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "= " + _pokemon.SpDefense, new Vector2(p.X + 32 + RESULT_OFFSET, p.Y + 244), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "= " + _pokemon.Speed, new Vector2(p.X + 32 + RESULT_OFFSET, p.Y + 284), Color.Black);
        }

        if (_delay >= 11.0f)
        {
            int newStat = _newAttack + _newDefense + _newSpAttack + _newMaxHP + _newSpDefense + _newSpeed;
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _pokemon.GetDisplayName() + " got a boost of " + newStat + "!",
                new Vector2(p.X + 32, p.Y + 320), Color.DarkRed);
        }
    }

    public override void Update()
    {
        _delay += 0.1f;

        if (Controls.Accept() == true)
        {
            if (_delay >= 13.0f)
            {
                SoundManager.PlaySound("select");
                Core.SetScreen(PreScreen);
            }
        }
    }
}
