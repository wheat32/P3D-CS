using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(524, "Kangaskhanite")]
public class Kangaskhanite : MegaStone
{
    public Kangaskhanite() : base("Kangaskhan", 115)
    {
        _textureRectangle = new Rectangle(168, 24, 24, 24);
    }

}
