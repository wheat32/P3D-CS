using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(610, "Jaw Fossil")]
public class JawFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived on the land. It looks as if it could be a piece of a large jaw.";
    public JawFossil()
    {
        _textureRectangle = new Rectangle(0, 48, 24, 24);
    }

}
