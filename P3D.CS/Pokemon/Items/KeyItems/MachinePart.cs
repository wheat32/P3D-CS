using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(128, "Machine Part")]
public class MachinePart : KeyItem
{
    public override String Description { get; protected set; } = "An important machine part for the Power Plant that was stolen.";
    public MachinePart()
    {
        _textureRectangle = new Rectangle(168, 120, 24, 24);
    }

}
