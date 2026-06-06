using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(34, "Leaf Stone")]
public class LeafStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that can make certain species of Pokémon evolve. It has an unmistakable leaf pattern.";
    public LeafStone()
    {
        _textureRectangle = new Rectangle(240, 24, 24, 24);
    }

}
