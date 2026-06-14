using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(61, "Super Rod")]
public class SuperRod : KeyItem
{
    public override String Description { get; protected set; } = "An awesome, high-tech fishing rod.";
    public override bool CanBeUsed { get; } = true;

    public SuperRod()
    {
        _textureRectangle = new Rectangle(312, 48, 24, 24);
    }

    public override void Use()
    {
        // TODO Phase 12: fishing encounter logic (requires Level/Spawner)
    }
}
