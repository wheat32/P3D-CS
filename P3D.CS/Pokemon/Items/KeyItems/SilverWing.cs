using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(71, "Silver Wing")]
public class SilverWing : KeyItem
{
    public override String Description { get; protected set; } = "A strange, silvery feather that sparkles.";
    public SilverWing()
    {
        _textureRectangle = new Rectangle(48, 72, 24, 24);
    }

}
