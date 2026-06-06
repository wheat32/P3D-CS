using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(130, "Lost Item")]
public class LostItem : KeyItem
{
    public override String Description { get; protected set; } = "The Poké Doll lost by the Copycat.";
    public LostItem()
    {
        _textureRectangle = new Rectangle(216, 120, 24, 24);
    }

}
