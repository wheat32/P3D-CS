using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(506, "Life Orb")]
public class LifeOrb : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It boosts the power of moves, but at the cost of some HP on each hit.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public LifeOrb()
    {
        _textureRectangle = new Rectangle(240, 240, 24, 24);
    }

}
