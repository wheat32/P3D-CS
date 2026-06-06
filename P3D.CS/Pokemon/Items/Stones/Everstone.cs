using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(112, "Everstone")]
public class Everstone : Item
{
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. A Pokémon holding this peculiar stone == prevented from evolving.";
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public Everstone()
    {
        _textureRectangle = new Rectangle(312, 96, 24, 24);
    }

}
