using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(545, "Metagrossite")]
public class Metagrossite : MegaStone
{
    public Metagrossite() : base("Metagross", 376)
    {
        _textureRectangle = new Rectangle(0, 96, 24, 24);
    }

}
