using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(24, "Water Stone")]
public class WaterStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that can make certain species of Pokémon evolve. It == the blue of a pool of clear water.";
    public WaterStone()
    {
        _textureRectangle = new Rectangle(24, 24, 24, 24);
    }

}
