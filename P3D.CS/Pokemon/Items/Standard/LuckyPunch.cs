using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(30, "Lucky Punch")]
public class LuckyPunch : Item
{
    public override String Description { get; protected set; } = "An item to be held by Chansey. This pair of lucky boxing gloves will boost Chansey's critical-hit ratio.";
    public override int PokeDollarPrice { get; protected set; } = 10;
    public override int FlingDamage { get; } = 40;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public LuckyPunch()
    {
        _textureRectangle = new Rectangle(144, 24, 24, 24);
    }

}
