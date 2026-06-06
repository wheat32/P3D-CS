using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(110, "Pearl")]
public class Pearl : Item
{
    public override String Description { get; protected set; } = "A rather small pearl that has a very nice silvery sheen to it. It can be sold cheaply to shops.";
    public override int PokeDollarPrice { get; protected set; } = 1400;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Pearl()
    {
        _textureRectangle = new Rectangle(264, 96, 24, 24);
    }

}
