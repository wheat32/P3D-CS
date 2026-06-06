using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(517, "Charizardite Y")]
public class CharizarditeY : MegaStone
{
    public CharizarditeY() : base("Charizard", 6)
    {
        _textureRectangle = new Rectangle(0, 24, 24, 24);
    }

}
