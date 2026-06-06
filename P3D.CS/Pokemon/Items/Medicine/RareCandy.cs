using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(32, "Rare Candy")]
public class RareCandy : MedicineItem
{
    public override String Description { get; protected set; } = "A candy that is packed with energy.";
    public override bool CanBeUsed { get; } = true;

    public RareCandy()
    {
        _textureRectangle = new Rectangle(192, 24, 24, 24);
    }

    public override void Use()
    {
        // TODO Phase 4/6: full Rare Candy logic
    }
}
