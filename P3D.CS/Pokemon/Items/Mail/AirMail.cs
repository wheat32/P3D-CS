using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(313, "Air Mail")]
public class AirMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of colorful letter sets. Let a Pokémon hold it for delivery.";
    public AirMail()
    {
        _textureRectangle = new Rectangle(288, 456, 24, 24);
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
