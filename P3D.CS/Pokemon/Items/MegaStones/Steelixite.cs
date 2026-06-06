using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(552, "Steelixite")]
public class Steelixite : MegaStone
{
    public Steelixite() : base("Steelix", 208)
    {
        _textureRectangle = new Rectangle(168, 96, 24, 24);
    }

}
