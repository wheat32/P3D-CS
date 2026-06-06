using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(655, "Sea Plane Ticket")]
public class SeaPlaneTicket : KeyItem
{
    public override String Description { get; protected set; } = "The ticket required for flying on the sea plane. It has a drawing of a sea plane on it.";
    public SeaPlaneTicket()
    {
        _textureRectangle = new Rectangle(432, 408, 24, 24);
    }

}
