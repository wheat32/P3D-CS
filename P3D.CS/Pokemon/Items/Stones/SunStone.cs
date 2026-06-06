using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(169, "Sun Stone")]
public class SunStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that can make certain species of Pokémon evolve. It burns as red as the evening sun.";
    public SunStone()
    {
        _textureRectangle = new Rectangle(312, 144, 24, 24);
    }

}
