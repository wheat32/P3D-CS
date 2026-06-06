using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(81, "Poison Barb")]
public class PoisonBarb : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This small, poisonous barb boosts the power of Poison-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 70;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public PoisonBarb()
    {
        _textureRectangle = new Rectangle(192, 72, 24, 24);
    }

}
