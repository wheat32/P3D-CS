using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(510, "Aggronite")]
public class Aggronite : MegaStone
{
    public Aggronite() : base("Aggron", 306)
    {
        _textureRectangle = new Rectangle(72, 0, 24, 24);
    }

}
