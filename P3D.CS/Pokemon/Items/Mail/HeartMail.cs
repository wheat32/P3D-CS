using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(318, "Heart Mail")]
public class HeartMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of giant heart patterns. Let a Pokémon hold it for delivery.";
    public HeartMail()
    {
        _textureRectangle = new Rectangle(432, 456, 24, 24);
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
