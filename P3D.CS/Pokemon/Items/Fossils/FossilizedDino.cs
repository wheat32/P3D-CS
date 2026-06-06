using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(615, "Fossilized Dino")]
public class FossilizedDino : FossilItem
{
    public override String Description { get; protected set; } = "The fossil of an ancient Pokémon that once lived in the sea. What it looked like == a mystery.";
    public FossilizedDino()
    {
        _textureRectangle = new Rectangle(24, 72, 24, 24);
    }

}
