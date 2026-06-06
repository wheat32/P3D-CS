using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(605, "Claw Fossil")]
public class ClawFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived in the sea. It appears to be a fragment of a claw.";
    public ClawFossil()
    {
        _textureRectangle = new Rectangle(96, 0, 24, 24);
    }

}
