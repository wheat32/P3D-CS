using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(514, "Blastoisinite")]
public class Blastoisinite : MegaStone
{
    public Blastoisinite() : base("Blastoise", 9)
    {
        _textureRectangle = new Rectangle(168, 0, 24, 24);
    }

}
