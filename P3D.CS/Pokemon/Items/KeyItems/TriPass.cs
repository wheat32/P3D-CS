using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(265, "Tri-Pass")]
public class TriPass : KeyItem
{
    public override String Description { get; protected set; } = "A pass for ferries between One, Two, && Three Island. It has a drawing of three islands.";
    public TriPass()
    {
        _textureRectangle = new Rectangle(480, 48, 24, 24);
    }

}
