using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(73, "Quick Claw")]
public class QuickClaw : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This light, sharp claw lets the bearer move first occasionally.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int BattlePointsPrice { get; } = 64;
    public override int FlingDamage { get; } = 80;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public QuickClaw()
    {
        _textureRectangle = new Rectangle(96, 72, 24, 24);
    }

}
