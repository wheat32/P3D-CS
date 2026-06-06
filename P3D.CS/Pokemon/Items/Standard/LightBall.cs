using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(163, "Light Ball")]
public class LightBall : Item
{
    public override String Description { get; protected set; } = "An item to be held by Pikachu. It's a puzzling orb that boosts its Attack && Sp. Atk. stats.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public LightBall()
    {
        _textureRectangle = new Rectangle(168, 144, 24, 24);
    }

}
