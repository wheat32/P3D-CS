using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(125, "HardStone")]
public class HardStone : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a durable stone that boosts the power of Rock-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public HardStone()
    {
        _textureRectangle = new Rectangle(96, 120, 24, 24);
    }

}
