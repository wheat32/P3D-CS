using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(66, "Red Scale")]
public class RedScale : KeyItem
{
    public override String Description { get; protected set; } = "A scale from the red Gyarados. It glows red like a flame.";
    public RedScale()
    {
        _textureRectangle = new Rectangle(432, 48, 24, 24);
    }

}
