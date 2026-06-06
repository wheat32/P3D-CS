using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(19, "Escape Rope")]
public class EscapeRope : Item
{
    public override String Description { get; protected set; } = "A long and durable rope.";
    public override bool CanBeUsed { get; } = true;

    public EscapeRope()
    {
        _textureRectangle = new Rectangle(408, 0, 24, 24);
    }

    public override void Use()
    {
        // TODO Phase 4: escape rope warp logic (requires Level.CanDig, ActionScript)
    }
}
