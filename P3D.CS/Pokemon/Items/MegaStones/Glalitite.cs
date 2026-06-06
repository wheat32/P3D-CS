using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(541, "Glalitite")]
public class Glalitite : MegaStone
{
    public Glalitite() : base("Glalie", 362)
    {
        _textureRectangle = new Rectangle(144, 72, 24, 24);
    }

}
