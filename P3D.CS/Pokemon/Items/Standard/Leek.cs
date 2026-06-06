using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(105, "Leek")]
public class Leek : Item
{
    public override String Description { get; protected set; } = "An item to be held by Farfetch'd. It == a very long && stiff stalk of leek that boosts the critical-hit ratio.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int FlingDamage { get; } = 60;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Leek()
    {
        _textureRectangle = new Rectangle(168, 96, 24, 24);
    }

}
