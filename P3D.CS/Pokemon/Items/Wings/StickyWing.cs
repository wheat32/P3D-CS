using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Wings;

[Item(261, "Sticky Wing")]
public class StickyWing : Item
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public override String Description { get; protected set; } = "It's a feather that sticks to other feathers, a Pokémon that holds it will find more feathers.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int FlingDamage { get; } = 20;
    public StickyWing()
    {
        _textureRectangle = new Rectangle(456, 240, 24, 24);
    }

}
