using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(299, "Rock Incense")]
public class RockIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This exotic-smelling incense boosts the power of Rock-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public RockIncense()
    {
        _textureRectangle = new Rectangle(312, 264, 24, 24);
    }

}
