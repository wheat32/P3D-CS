using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(103, "Slowpoketail")]
public class SlowPokeTail : Item
{
    public override String Description { get; protected set; } = "A very tasty tail of something. It sells for a high price.";
    public override int PokeDollarPrice { get; protected set; } = 9800;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public SlowPokeTail()
    {
        _textureRectangle = new Rectangle(120, 96, 24, 24);
    }

}
