using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(511, "Alakazite")]
public class Alakazite : MegaStone
{
    public Alakazite() : base("Alakazam", 65)
    {
        _textureRectangle = new Rectangle(96, 0, 24, 24);
    }

}
