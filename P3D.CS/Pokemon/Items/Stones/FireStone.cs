using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(22, "Fire Stone")]
public class FireStone : StoneItem
{
    public override String Description { get; protected set; } = "A peculiar stone that can make certain species of Pokémon evolve. The stone has a fiery orange heart.";
    public FireStone()
    {
        _textureRectangle = new Rectangle(480, 0, 24, 24);
    }

}
