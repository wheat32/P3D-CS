using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(1, "Masterball")]
public class Masterball : BallItem
{
    public override String Description { get; protected set; } = "The best Pokéball with the ultimate level of performance. With it, you will catch any wild Pokémon without fail.";
    public override bool CanBeTraded { get; protected set; } = false;
    public override float CatchMultiplier { get; } = 255.0F;
    public Masterball()
    {
        _textureRectangle = new Rectangle(0, 0, 24, 24);
    }

}
