using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(135, "Shiny Stone")]
public class ShinyStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that makes certain species of Pokémon evolve. It shines with a dazzling light.";
    public ShinyStone()
    {
        _textureRectangle = new Rectangle(336, 192, 24, 24);
    }

}
