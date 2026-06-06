using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(285, "Old Sea Map")]
public class OldSeaMap : KeyItem
{
    public override String Description { get; protected set; } = "A faded sea chart that shows the way to a certain island.";
    public OldSeaMap()
    {
        _textureRectangle = new Rectangle(168, 264, 24, 24);
    }

}
