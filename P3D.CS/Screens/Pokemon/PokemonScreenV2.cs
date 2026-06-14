using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;
using P3D.BattleSystem;

namespace P3D;

public class PokemonScreenV2 : WindowScreen
{
    private Texture2D _texture = null!;
    private Texture2D _battleTexture = null!;

    public PokemonScreenV2(Screen currentScreen, int startSelectionIndex)
        : base(currentScreen, Identifications.PokemonScreen, "Pokémon")
    {
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        _battleTexture = TextureManager.GetTexture("GUI\\Battle\\Interface");
    }

    public override void Update()
    {
        base.Update();
        if (Controls.Dismiss() == true)
            CloseScreen();
    }

    public override void Draw()
    {
        PreScreen!.Draw();
        base.Draw();
        if (FadedIn == true)
            DrawFirstPokemon();
    }

    private void DrawFirstPokemon()
    {
        Pokemon poke = Core.Player.Pokemons[0];
        Vector2 topLeft = GetPositionInWindowTopLeft(0, 0);

        Canvas.DrawRectangle(OffsetRectangle(new Rectangle((int)topLeft.X + 43, (int)topLeft.Y + 123, 80 * 3, 32 * 3)), new Color(0, 0, 0, 90));

        Core.SpriteBatch.Draw(_texture,
            OffsetRectangle(new Rectangle((int)topLeft.X + 40, (int)topLeft.Y + 120, 80 * 3, 32 * 3)),
            new Rectangle(48, 16, 80, 32),
            Color.White);

        Texture2D pTexture = poke.GetMenuTexture();

        Core.SpriteBatch.Draw(pTexture, OffsetRectangle(new Rectangle((int)topLeft.X + 40, (int)topLeft.Y + 120, pTexture.Width * 2, pTexture.Height * 2)), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MiniFont, poke.GetDisplayName(), OffsetVector(new Vector2(topLeft.X + 108, topLeft.Y + 134)), Color.White, 0.0F, Vector2.Zero, 1.2F, SpriteEffects.None, 0.0F);
        Core.SpriteBatch.DrawString(FontManager.MiniFont, "Lv. " + poke.Level.ToString(), OffsetVector(new Vector2(topLeft.X + 44, topLeft.Y + 188)), Color.White);

        if (poke.Item != null)
            Core.SpriteBatch.Draw(poke.Item.Texture, OffsetRectangle(new Rectangle((int)topLeft.X + 80, (int)topLeft.Y + 160, 24, 24)), Color.White);

        Core.SpriteBatch.Draw(_battleTexture,
            OffsetRectangle(new Rectangle((int)topLeft.X + 108, (int)topLeft.Y + 170, 13 * 2, 6 * 2)),
            new Rectangle(6, 37, 13, 6),
            Color.White);

        float HPpercentage = (100.0F / poke.MaxHP) * poke.HP;
        int HPlength = (int)Math.Ceiling(140 / 100.0F * HPpercentage.Clamp(1, 999));

        if (poke.HP == 0)
        {
            HPlength = 0;
        }
        else
        {
            if (HPlength <= 0) HPlength = 1;
        }
        if (poke.HP == poke.MaxHP)
        {
            HPlength = 140;
        }
        else
        {
            if (HPlength == 140) HPlength = 139;
        }

        int cX = 0;
        if (HPpercentage <= 50.0F && HPpercentage > 15.0F)
            cX = 2;
        else if (HPpercentage <= 15.0F)
            cX = 4;

        Vector2 pos = OffsetVector(new Vector2(topLeft.X + 136, topLeft.Y + 170));

        Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Battle\\Interface"), new Rectangle((int)pos.X, (int)pos.Y, 140, 12), new Rectangle(19, 37, 70, 6), Color.White);

        if (HPlength > 0)
        {
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Battle\\Interface"), new Rectangle((int)pos.X, (int)pos.Y, 2, 12), new Rectangle(cX, 37, 1, 6), Color.White);
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Battle\\Interface"), new Rectangle((int)pos.X + 2, (int)pos.Y, HPlength - 4, 12), new Rectangle(cX + 1, 37, 1, 6), Color.White);
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Battle\\Interface"), new Rectangle((int)pos.X + HPlength - 2, (int)pos.Y, 2, 12), new Rectangle(cX, 37, 1, 6), Color.White);
        }

        Core.SpriteBatch.DrawString(FontManager.MiniFont, poke.HP + " / " + poke.MaxHP, OffsetVector(new Vector2(topLeft.X + 140, topLeft.Y + 188)), Color.White);

        Texture2D? statusTexture = BattleStats.GetStatImage(poke.Status);
        if (statusTexture != null)
            Core.SpriteBatch.Draw(statusTexture, OffsetRectangle(new Rectangle((int)(topLeft.X + 235), (int)(topLeft.Y + 192), 38, 12)), Color.White);
    }
}
