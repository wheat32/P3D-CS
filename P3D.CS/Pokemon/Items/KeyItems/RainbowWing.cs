using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(178, "Rainbow Wing")]
public class RainbowWing : KeyItem
{
    public override String Description { get; protected set; } = "A mystical rainbow feather that sparkles.";
    public RainbowWing()
    {
        _textureRectangle = new Rectangle(408, 144, 24, 24);
    }

}
