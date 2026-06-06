using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(90, "Silk Scarf")]
public class SilkScarf : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It's a sumptuous scarf that boosts the power of Normal-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Silk Scarves";
    public SilkScarf()
    {
        _textureRectangle = new Rectangle(264, 192, 24, 24);
    }

}
