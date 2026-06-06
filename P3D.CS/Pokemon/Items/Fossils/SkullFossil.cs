using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(606, "Skull Fossil")]
public class SkullFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived on the land. It appears as though it's part of a head.";
    public SkullFossil()
    {
        _textureRectangle = new Rectangle(0, 24, 24, 24);
    }

}
