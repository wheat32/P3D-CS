using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(8, "Moon Stone")]
public class MoonStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that can make certain species of Pokémon evolve. It == as black as the night sky.";
    public MoonStone()
    {
        _textureRectangle = new Rectangle(144, 0, 24, 24);
    }

}
