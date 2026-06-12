using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(331, "Inquiry Mail")]
public class InquiryMail : MailItem
{
    public override String Description { get; protected set; } = "Stationary designed for writing questions. Let a Pokémon hold it for delivery.";
    public InquiryMail()
    {
        _textureRectangle = new Rectangle(240, 480, 24, 24);
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
