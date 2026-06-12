using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(333, "RSVP Mail")]
public class RSVPMail : MailItem
{
    public override String Description { get; protected set; } = "Stationary designed for invitations. Let a Pokémon hold it for delivery.";
    public RSVPMail()
    {
        _textureRectangle = new Rectangle(288, 480, 24, 24);
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
