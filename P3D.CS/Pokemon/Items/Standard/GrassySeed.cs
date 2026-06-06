using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(597, "Grassy Seed")]
public class GrassySeed : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It boosts Defense on Grassy Terrain. It can only be used once.";
    public override int PokeDollarPrice { get; protected set; } = 2000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public GrassySeed()
    {
        _textureRectangle = new Rectangle(336, 312, 24, 24);
    }

}
