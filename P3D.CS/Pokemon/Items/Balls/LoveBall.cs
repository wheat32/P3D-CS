using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(166, "Love Ball")]
public class LoveBall : BallItem
{
    public override String Description { get; protected set; } = "Pokéball for catching Pokémon that are the opposite gender of your Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 150;
    public LoveBall()
    {
        _textureRectangle = new Rectangle(240, 144, 24, 24);
    }

}
