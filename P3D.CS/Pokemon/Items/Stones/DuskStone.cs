using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(136, "Dusk Stone")]
public class DuskStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that makes certain species of Pokémon evolve. It == as dark as dark can be.";
    public DuskStone()
    {
        _textureRectangle = new Rectangle(360, 192, 24, 24);
    }

}
