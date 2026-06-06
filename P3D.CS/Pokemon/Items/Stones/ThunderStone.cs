using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(23, "Thunder Stone")]
public class ThunderStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that can make certain species of Pokémon evolve. It has a distinct thunderbolt pattern.";
    public ThunderStone()
    {
        _textureRectangle = new Rectangle(0, 24, 24, 24);
    }

}
