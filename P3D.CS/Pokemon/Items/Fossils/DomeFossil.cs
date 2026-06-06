using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(602, "Dome Fossil")]
public class DomeFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived in the sea. It could be a shell || carapace.";
    public DomeFossil()
    {
        _textureRectangle = new Rectangle(24, 0, 24, 24);
    }

}
