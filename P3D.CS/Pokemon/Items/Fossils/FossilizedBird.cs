using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(612, "Fossilized Bird")]
public class FossilizedBird : FossilItem
{
    public override String Description { get; protected set; } = "The fossil of an ancient Pokémon that once soared through the sky. What it looked like == a mystery.";
    public FossilizedBird()
    {
        _textureRectangle = new Rectangle(0, 72, 24, 24);
    }

}
