using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(77, "Sharp Beak")]
public class SharpBeak : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It's a long, sharp beak that boosts the power of Flying-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 50;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public SharpBeak()
    {
        _textureRectangle = new Rectangle(168, 72, 24, 24);
    }

}
