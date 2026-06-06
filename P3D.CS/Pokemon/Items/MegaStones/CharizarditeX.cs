using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(516, "Charizardite X")]
public class CharizarditeX : MegaStone
{
    public CharizarditeX() : base("Charizard", 6)
    {
        _textureRectangle = new Rectangle(216, 0, 24, 24);
    }

}
