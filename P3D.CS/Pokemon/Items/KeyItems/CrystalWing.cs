using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(56, "Crystal Wing")]
public class CrystalWing : KeyItem
{
    public override String Description { get; protected set; } = "A mystical feather entirely made out of crystal.";
    public CrystalWing()
    {
        _textureRectangle = new Rectangle(240, 192, 24, 24);
    }

}
