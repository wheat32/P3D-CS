using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(287, "Rose Incense")]
public class RoseIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This exotic-smelling incense boosts the power of Grass-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public RoseIncense()
    {
        _textureRectangle = new Rectangle(336, 264, 24, 24);
    }

}
