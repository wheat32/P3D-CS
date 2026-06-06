using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(137, "Dawn Stone")]
public class DawnStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that makes certain species of Pokémon evolve. It sparkles like eyes.";
    public DawnStone()
    {
        _textureRectangle = new Rectangle(384, 192, 24, 24);
    }

}
