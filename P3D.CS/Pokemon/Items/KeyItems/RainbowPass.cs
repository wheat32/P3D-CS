using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(284, "Rainbow Pass")]
public class RainbowPass : KeyItem
{
    public override String Description { get; protected set; } = "A pass for ferries between Vermilion && the Sevii Islands. It features a drawing of a rainbow.";
    public RainbowPass()
    {
        _textureRectangle = new Rectangle(144, 264, 24, 24);
    }

}
