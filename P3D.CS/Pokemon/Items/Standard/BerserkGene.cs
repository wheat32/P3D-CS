using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(170, "Berserk Gene")]
public class BerserkGene : Item
{
    public override String Description { get; protected set; } = "A strand of DNA that overflows with pulsating energy. It sharply raises offensive capabilities in a pinch, but causes confusion.";
    public override int PokeDollarPrice { get; protected set; } = 3000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BerserkGene()
    {
        _textureRectangle = new Rectangle(192, 240, 24, 24);
    }

}
