using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(593, "Ice Stone")]
public class IceStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that can make certain species of Pokémon evolve. It has an unmistakable snowflake pattern.";
    public IceStone()
    {
        _textureRectangle = new Rectangle(144, 312, 24, 24);
    }

}
