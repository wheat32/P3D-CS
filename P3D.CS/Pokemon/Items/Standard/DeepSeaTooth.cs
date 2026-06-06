using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(167, "DeepSeaTooth")]
public class DeepSeaTooth : Item
{
    public override String Description { get; protected set; } = "An item to be held by Clamperl. This fang gleams a sharp silver && raises the holder's Sp. Atk. stat.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int FlingDamage { get; } = 90;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Deep Sea Teeth";
    public DeepSeaTooth()
    {
        _textureRectangle = new Rectangle(312, 216, 24, 24);
    }

}
