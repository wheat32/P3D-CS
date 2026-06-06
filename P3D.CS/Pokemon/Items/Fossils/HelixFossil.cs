using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(601, "Helix Fossil")]
public class HelixFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived in the sea. It might be a piece of a seashell.";
    public HelixFossil()
    {
        _textureRectangle = new Rectangle(0, 0, 24, 24);
    }

}
