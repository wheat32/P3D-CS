using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(613, "Fossilized Fish")]
public class FossilizedFish : FossilItem
{
    public override String Description { get; protected set; } = "The fossil of an ancient Pokémon that once lived in the sea. What it looked like == a mystery.";
    public override String PluralName { get; } = "Fossilized Fishes";
    public FossilizedFish()
    {
        _textureRectangle = new Rectangle(48, 72, 24, 24);
    }

}
