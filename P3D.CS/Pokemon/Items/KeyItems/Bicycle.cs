using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(6, "Bicycle")]
public class Bicycle : KeyItem
{
    public override String Description { get; protected set; } = "A folding Bicycle.";
    public override bool CanBeUsed { get; } = true;

    public Bicycle()
    {
        _textureRectangle = new Rectangle(120, 0, 24, 24);
    }

    public override void Use()
    {
        // TODO Phase 4: bicycle ride logic (requires Level.Riding, MusicManager)
    }
}
