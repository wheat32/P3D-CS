using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(596, "Electric Seed")]
public class ElectricSeed : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It boosts Defense on Electric Terrain. It can only be used once.";
    public override int PokeDollarPrice { get; protected set; } = 2000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ElectricSeed()
    {
        _textureRectangle = new Rectangle(312, 312, 24, 24);
    }

}
