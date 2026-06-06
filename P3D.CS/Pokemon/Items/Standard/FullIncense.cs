using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(288, "Full Incense")]
public class FullIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This exotic-smelling incense makes the holder bloated && slow moving.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public FullIncense()
    {
        _textureRectangle = new Rectangle(216, 264, 24, 24);
    }

}
