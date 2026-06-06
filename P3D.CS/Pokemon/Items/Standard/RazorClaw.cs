using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(184, "Razor Claw")]
public class RazorClaw : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a sharply hooked claw that ups the holder's critical-hit ratio.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override int BattlePointsPrice { get; } = 48;
    public override int FlingDamage { get; } = 80;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public RazorClaw()
    {
        _textureRectangle = new Rectangle(480, 144, 24, 24);
    }

}
