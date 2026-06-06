using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(173, "Pearl String")]
public class PearlString : Item
{
    public override String Description { get; protected set; } = "Very large pearls that sparkle in a pretty silver color. They can be sold at a high price to shops.";
    public override int PokeDollarPrice { get; protected set; } = 50000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public PearlString()
    {
        _textureRectangle = new Rectangle(408, 216, 24, 24);
    }

}
