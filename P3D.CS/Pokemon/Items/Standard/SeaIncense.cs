using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(264, "Sea Incense")]
public class SeaIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This incense has a curious aroma that boosts the power of Water-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public SeaIncense()
    {
        _textureRectangle = new Rectangle(24, 264, 24, 24);
    }

}
