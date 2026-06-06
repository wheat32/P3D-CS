using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(550, "Sharpedonite")]
public class Sharpedonite : MegaStone
{
    public Sharpedonite() : base("Sharpedo", 319)
    {
        _textureRectangle = new Rectangle(120, 96, 24, 24);
    }

}
