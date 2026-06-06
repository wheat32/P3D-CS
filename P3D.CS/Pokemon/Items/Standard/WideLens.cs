using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(171, "Wide Lens")]
public class WideLens : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It's a magnifying lens that slightly boosts the accuracy of moves.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Wide Lenses";
    public WideLens()
    {
        _textureRectangle = new Rectangle(144, 216, 24, 24);
    }

}
