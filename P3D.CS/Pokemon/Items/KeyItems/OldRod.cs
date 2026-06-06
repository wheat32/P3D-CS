using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(58, "Old Rod")]
public class OldRod : KeyItem
{
    public override String Description { get; protected set; } = "An old and beat-up fishing rod.";
    public override bool CanBeUsed { get; } = true;

    public OldRod()
    {
        _textureRectangle = new Rectangle(240, 48, 24, 24);
    }

    public override void Use()
    {
        // TODO Phase 4: fishing encounter logic (requires Level/Spawner)
    }
}
