using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(181, "Safari Ball")]
public class SafariBall : BallItem
{
    public override bool CanBeHeld { get; } = false;
    public override String Description { get; protected set; } = "A special Pokéball that == used only in the Great Marsh && the Safari Zone. It == decorated in a camouflage pattern.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override float CatchMultiplier { get; } = 1.5F;
    public SafariBall()
    {
        _textureRectangle = new Rectangle(72, 144, 24, 24);
    }

}
