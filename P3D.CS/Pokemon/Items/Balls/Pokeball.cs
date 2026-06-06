using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(5, "Pokéball")]
public class Pokeball : BallItem
{
    public override String Description { get; protected set; } = "An item for catching Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public Pokeball()
    {
        _textureRectangle = new Rectangle(96, 0, 24, 24);
    }

}
