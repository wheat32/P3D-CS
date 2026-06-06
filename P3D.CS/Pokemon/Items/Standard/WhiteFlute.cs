using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(147, "White Flute")]
public class WhiteFlute : Item
{
    public override String Description { get; protected set; } = "A lovely toy flute to admire. It's made from white glass.";
    public override int PokeDollarPrice { get; protected set; } = 500;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public WhiteFlute()
    {
        _textureRectangle = new Rectangle(456, 192, 24, 24);
    }

}
