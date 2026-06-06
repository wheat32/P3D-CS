using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(608, "Cover Fossil")]
public class CoverFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived in the sea. It appears as though it could be part of its back.";
    public CoverFossil()
    {
        _textureRectangle = new Rectangle(72, 24, 24, 24);
    }

}
