using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(598, "Misty Seed")]
public class MistySeed : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It boosts Sp. Defense on Misty Terrain. It can only be used once.";
    public override int PokeDollarPrice { get; protected set; } = 2000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public MistySeed()
    {
        _textureRectangle = new Rectangle(360, 312, 24, 24);
    }

}
