using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(323, "Tunnel Mail")]
public class TunnelMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of a dimly lit coal mine. Let a Pokémon hold it for delivery.";
    public TunnelMail()
    {
        _textureRectangle = new Rectangle(48, 480, 24, 24);
    }

    public override void Use()
    {
        String MailID = String.Empty;
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
