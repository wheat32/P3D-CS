using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(263, "Odd Incense")]
public class OddIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This exotic-smelling incense boosts the power of Psychic-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public OddIncense()
    {
        _textureRectangle = new Rectangle(0, 264, 24, 24);
    }

}
