using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(326, "BridgeMail S")]
public class BridgeMailS : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of a sky-piercing bridge. Let a Pokémon hold it for use.";
    public BridgeMailS()
    {
        _textureRectangle = new Rectangle(120, 480, 24, 24);
    }

    public override void Use()
    {
        String MailID = "";
        if (IsGameModeItem == true)
        {
            MailID = gmID;
        }
        else
        {
            MailID = ID.ToString();
        }
        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MailSystemScreen(Core.CurrentScreen, MailID), Color.Black, false));
    }

}
