using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(140, "Scope Lens")]
public class ScopeLens : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a lens that boosts the holder's critical-hit ratio.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int BattlePointsPrice { get; } = 64;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Scope Lenses";
    public ScopeLens()
    {
        _textureRectangle = new Rectangle(384, 120, 24, 24);
    }

}
