using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(520, "Gengarite")]
public class Gengarite : MegaStone
{
    public Gengarite() : base("Gengar", 94)
    {
        _textureRectangle = new Rectangle(72, 24, 24, 24);
    }

}
