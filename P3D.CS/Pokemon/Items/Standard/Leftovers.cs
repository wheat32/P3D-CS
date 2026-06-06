using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(146, "Leftovers")]
public class Leftovers : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. The holder's HP == slowly but steadily restored throughout every battle.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int BattlePointsPrice { get; } = 64;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName => Name;
    public Leftovers()
    {
        _textureRectangle = new Rectangle(456, 120, 24, 24);
    }

}
