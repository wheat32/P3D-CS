using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(176, "Grip Claw")]
public class GripClaw : Item
{
    public override String Description { get; protected set; } = "A Pokémon hold item that extends the duration of multiturn attacks like Bind && Wrap.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int FlingDamage { get; } = 90;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public GripClaw()
    {
        _textureRectangle = new Rectangle(168, 216, 24, 24);
    }

}
