using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(609, "Plume Fossil")]
public class PlumeFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived in the sky. It looks as if it could come from part of its wing.";
    public PlumeFossil()
    {
        _textureRectangle = new Rectangle(96, 24, 24, 24);
    }

}
