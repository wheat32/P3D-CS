using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(164, "Friend Ball")]
public class FriendBall : BallItem
{
    public override String Description { get; protected set; } = "A Pokéball that makes caught Pokémon more friendly.";
    public override int PokeDollarPrice { get; protected set; } = 150;
    public FriendBall()
    {
        _textureRectangle = new Rectangle(192, 144, 24, 24);
    }

}
