using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(611, "Sail Fossil")]
public class SailFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived on land. It looks like the impression from a skin sail.";
    public SailFossil()
    {
        _textureRectangle = new Rectangle(24, 48, 24, 24);
    }

}
