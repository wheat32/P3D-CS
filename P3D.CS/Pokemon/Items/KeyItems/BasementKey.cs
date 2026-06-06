using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(133, "Basement Key")]
public class BasementKey : KeyItem
{
    public override String Description { get; protected set; } = "A key that opens a door in the Goldenrod Tunnel.";
    public BasementKey()
    {
        _textureRectangle = new Rectangle(288, 120, 24, 24);
    }

}
