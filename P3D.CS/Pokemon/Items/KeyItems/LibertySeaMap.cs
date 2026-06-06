using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(592, "Liberty Sea Map")]
public class LibertySeaMap : KeyItem
{
    public override String Description { get; protected set; } = "A new sea chart that shows the way to Liberty Garden in Unova. It depicts a Lighthouse.";
    public LibertySeaMap()
    {
        _textureRectangle = new Rectangle(408, 288, 24, 24);
    }

}
