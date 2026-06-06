using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(117, "Miracle Seed")]
public class MiracleSeed : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a seed imbued with life force that boosts the power of Grass-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public MiracleSeed()
    {
        _textureRectangle = new Rectangle(432, 96, 24, 24);
    }

}
