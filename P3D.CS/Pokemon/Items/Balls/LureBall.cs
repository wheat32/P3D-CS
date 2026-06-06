using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(160, "Lure Ball")]
public class LureBall : BallItem
{
    public override String Description { get; protected set; } = "A Pokéball for catching Pokémon hooked by a Rod when fishing.";
    public override int PokeDollarPrice { get; protected set; } = 150;
    public LureBall()
    {
        _textureRectangle = new Rectangle(120, 144, 24, 24);
    }

}
