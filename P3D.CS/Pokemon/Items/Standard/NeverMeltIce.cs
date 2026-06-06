using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(107, "Never-Melt Ice")]
public class NeverMeltIce : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It's a piece of ice that repels heat effects && boosts Ice-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public NeverMeltIce()
    {
        _textureRectangle = new Rectangle(216, 96, 24, 24);
    }

}
