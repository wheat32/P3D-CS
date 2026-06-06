using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(55, "Itemfinder")]
public class Itemfinder : KeyItem
{
    public override String Description { get; protected set; } = "It checks for unseen items.";
    public override bool CanBeUsed { get; } = true;

    public Itemfinder()
    {
        _textureRectangle = new Rectangle(192, 48, 24, 24);
    }

    public override void Use()
    {
        // TODO Phase 4: item finder logic (requires Level/Entity system)
    }
}
