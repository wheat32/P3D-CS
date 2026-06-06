using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(528, "Medichamite")]
public class Medichamite : MegaStone
{
    public Medichamite() : base("Medicham", 308)
    {
        _textureRectangle = new Rectangle(24, 48, 24, 24);
    }

}
