using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(540, "Galladite")]
public class Galladite : MegaStone
{
    public Galladite() : base("Gallade", 475)
    {
        _textureRectangle = new Rectangle(120, 72, 24, 24);
    }

}
