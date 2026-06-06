using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(604, "Root Fossil")]
public class RootFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived in the sea. It looks as if it could be part of a plant's root.";
    public RootFossil()
    {
        _textureRectangle = new Rectangle(72, 0, 24, 24);
    }

}
