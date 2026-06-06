using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(614, "Fossilized Drake")]
public class FossilizedDrake : FossilItem
{
    public override String Description { get; protected set; } = "The fossil of an ancient Pokémon that once roamed the land. What it looked like == a mystery.";
    public FossilizedDrake()
    {
        _textureRectangle = new Rectangle(72, 72, 24, 24);
    }

}
