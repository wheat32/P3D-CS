using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(95, "Mystic Water")]
public class MysticWater : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This teardrop-shaped gem boosts the power of Water-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public MysticWater()
    {
        _textureRectangle = new Rectangle(456, 72, 24, 24);
    }

}
