using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(59, "Good Rod")]
public class GoodRod : KeyItem
{
    public override String Description { get; protected set; } = "A new, good-quality fishing rod.";
    public override bool CanBeUsed { get; } = true;

    public GoodRod()
    {
        _textureRectangle = new Rectangle(264, 48, 24, 24);
    }

    public override void Use()
    {
        // TODO Phase 12: fishing encounter logic (requires Level/Spawner)
    }
}
