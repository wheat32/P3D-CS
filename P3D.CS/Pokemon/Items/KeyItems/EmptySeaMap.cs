using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(292, "Empty Sea Map")]
public class EmptySeaMap : KeyItem
{
    public override String Description { get; protected set; } = "A sea chart on an odd blue parchment showing a point in the open ocean.";
    public EmptySeaMap()
    {
        _textureRectangle = new Rectangle(192, 264, 24, 24);
    }

}
