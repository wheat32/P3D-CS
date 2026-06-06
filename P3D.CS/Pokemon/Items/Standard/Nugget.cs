using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(36, "Nugget")]
public class Nugget : Item
{
    public override String Description { get; protected set; } = "A nugget of the purest gold that gives off a lustrous gleam in direct light. It can be sold at a high price to shops.";
    public override int PokeDollarPrice { get; protected set; } = 10000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Nugget()
    {
        _textureRectangle = new Rectangle(288, 24, 24, 24);
    }

}
