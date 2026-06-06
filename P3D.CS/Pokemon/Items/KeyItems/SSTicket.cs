using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(41, "S.S. Ticket")]
public class SSTicket : KeyItem
{
    public override String Description { get; protected set; } = "The ticket required for sailing on the ferry S.S. Aqua. It has a drawing of a ship on it.";
    public SSTicket()
    {
        _textureRectangle = new Rectangle(240, 216, 24, 24);
    }

}
